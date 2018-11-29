using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Scripts.PixelTracery;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

  /**
  * The MIT License (MIT)
  * 
  *  Copyright © 2018 Alexey Radyuk
  * 
  * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and 
  * associated documentation files (the «Software»), to deal in the Software without restriction, including 
  * without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies 
  * of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
  * 
  * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
  * 
  * THE SOFTWARE IS PROVIDED «AS IS», WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
  * INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
  * IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
  * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR 
  * THE USE OR OTHER DEALINGS IN THE SOFTWARE.
  */

public class State {
  public string Name;
  public float Time;
  public bool Active;
}

// The class simulates the download of the pictures with tape
// Attention! This code is not an emulator of the ZX-spectrum platform, it just draws on the screen and sounds something like zx-spectrum did.
public class ZxScreenLoader : MonoBehaviour {

  [Header ("Set the screen material")]
  public Material ScreenMaterial;

  public const string FONT_PATH = "SpriteFonts/ZxSpectrumFont";

  public const int DRAW_MONOCHROME_SPEED = 100;
  public const int DRAW_ATTRIBUTES_SPEED = 10;

  public const int FULL_SCREEN_WIDTH = 320;
  public const int FULL_SCREEN_HEIGHT = 240;
  public const int SCREEN_WIDTH = 256;
  public const int SCREEN_HEIGHT = 192;
  public const int BORDER_WIDTH = 32;
  public const int BORDER_HEIGHT = 24;
  public const int SPRITE_SIZE = 8;
  public const int THIRD_LEN = SCREEN_WIDTH * SPRITE_SIZE * SPRITE_SIZE;
  public const int MONOCHROME_LEN = 6144;
  public const int ATTR_LEN = 768;
  public const int SCREEN_SPR_WIDTH = SCREEN_WIDTH / SPRITE_SIZE;
  public const int SCREEN_SPR_HEIGHT = SCREEN_HEIGHT / SPRITE_SIZE;

  public const int PILOT_H = 10;
  public const int FLASH_DELAY_FRAMES = 18;

  private string DisplayName;
  private ZxPalettes.Type Palette = ZxPalettes.Type.Orthodox;

  private Texture2D texture;
  private Color32[] clearColors;
  private Color32[] borderMask;

  private Sprite[] fontSprites;
  private Dictionary<char, Sprite> fontDict;

  private byte[] scrData;
  private List<Color32> monochromeImg;
  private List<int> rawData;

  private State[] states = new State[] {
    new State () { Name = "LongPilot", Time = 5 },
    new State () { Name = "LoadHeaderData", Time = 0.1f },
    new State () { Name = "Mute", Time = 1 },
    new State () { Name = "ShortPilot", Time = 2 },
    new State () { Name = "LoadData", Time = -1 },
    new State () { Name = "LoadingComplete", Time = -1 },
  };

  private float pilotPos;

  private ZxTapeAudioEmulator audioEngine;
  private FileLoader fileLoader;

  private int frame;
  private int _state;
  private float timerStart;

  private bool drawStarted;

  private float scrPartTime;
  private float elapsedTime;

  private Action onComplete;

  void Awake () {
    texture = new Texture2D (FULL_SCREEN_WIDTH, FULL_SCREEN_HEIGHT);
    ScreenMaterial.mainTexture = texture;
    initBorderMask ();

    if (fontSprites == null) {
      fontSprites = Resources.LoadAll<Sprite> (FONT_PATH);
      fontDict = fontSprites.PixToFontDictionary ();
    }
    if (audioEngine == null) audioEngine = gameObject.AddComponent<ZxTapeAudioEmulator> ();
    if (fileLoader == null) fileLoader = gameObject.AddComponent<FileLoader> ();

    Reset ();
  }

  public void Load (string name, string path, float volume, ZxPalettes.Type palette, Action onComplete) {
    this.DisplayName = name;
    this.Palette = palette;
    this.onComplete = onComplete;

    audioEngine.SetVolume (volume);
    texture.PixClear (ref clearColors, getWhiteColor ());

    if (fileLoader.IsWellFormedUriString (path)) {
      fileLoader.Load (path, onScreenLoaded);
    } else {
      fileLoader.LoadFromStreamingAssets (path, onScreenLoaded);
    }
  }

  public void Reset () {
    frame = 0;
    // random time for LongPilot
    states[0].Time = Random.Range (4f, 5f);
    SetState (-1);
    pilotPos = 0;
    drawStarted = false;

    texture.PixClear (ref clearColors, getWhiteColor ());
  }

  private void Update () {
    frame++;

    var state = GetState ();
    if (state < 0) return;

    if (!audioEngine.IsPlaying ()) {
      scrPartTime = 0;
      elapsedTime = audioEngine.GetElapsedTime ();
    }

    // if the time is less than zero, the state is switched "manually", NOT by time
    if (states[state].Time >= 0 && Time.time - timerStart > states[state].Time) {
      states[state].Active = false;
      timerStart = Time.time;
      state = Mathf.Min (++state, states.Length - 1);
      SetState (state);
    }

    switch (state) {
      case 0: // LongPilot
        audioEngine.SetPilotSignal ();
        drawPilot ();
        break;

      case 1: // HeaderData
        audioEngine.SetUnitSignal ();
        drawLoadingLines ();
        if (frame % 3 == 0) texture.Apply ();
        break;

      case 2: // Mute
        audioEngine.SetEmptySignal ();
        border (5);
        var alphaColor = fontSprites[0].PixGetColors () [0];
        var shortName = DisplayName.Substring (0, Math.Min (8, DisplayName.Length));
        texture.PixPrintText ("Bytes: " + shortName, fontDict, BORDER_WIDTH, BORDER_HEIGHT + SPRITE_SIZE, alphaColor, getColor (0), fixedWidth : SPRITE_SIZE);
        texture.Apply ();
        break;

      case 3: // ShortPilot
        drawPilot ();
        audioEngine.SetPilotSignal ();
        break;

      case 4: // DrawingData
        if (!drawStarted) {
          drawStarted = true;
          audioEngine.SetEmptySignal ();
          audioEngine.InitAudioData (rawData);
          StartCoroutine (drawCoroutine (onDrawingComplete));
        }
        drawLoadingLines ();
        break;

      default:
        // Mute 
        audioEngine.SetEmptySignal ();

        // draw flash attributes
        if (frame % FLASH_DELAY_FRAMES == 0) updateAttributes ();

        // draw idle border
        // if (frame % 3 == 0) {
        // 	var color = frame % 180 > 60 ? 2 : 5;
        // 	border(color);
        // }
        break;
    }
  }

  private void updateAttributes () {
    for (var sprY = 0; sprY < SCREEN_SPR_HEIGHT; sprY++) {
      for (var sprX = 0; sprX < SCREEN_SPR_WIDTH; sprX++) {
        var attr = scrData[MONOCHROME_LEN + sprY * SCREEN_SPR_WIDTH + sprX];
        var flash = attr > 127;
        if (!flash) continue;

        var ink = attr - 8 * (attr / 8);
        var paper = (attr / 8) - 8 * (attr / 64);
        var bright = (attr / 64) == 1 | (attr / 64) == 3;

        for (var localY = 0; localY < SPRITE_SIZE; localY++) {
          for (var localX = 0; localX < SPRITE_SIZE; localX++) {
            var x = sprX * SPRITE_SIZE + localX;
            var y = sprY * SPRITE_SIZE + localY;

            var inkIx = bright ? ink + 8 : ink;
            var paperIx = bright ? paper + 8 : paper;
            var inkColor = getColor (inkIx);
            var paperColor = getColor (paperIx);
            var screenColor = texture.PixGetPixel (BORDER_WIDTH + x, BORDER_HEIGHT + y);
            var color = screenColor.Equals (inkColor) ? paperColor : inkColor;

            texture.PixSetPixel (BORDER_WIDTH + x, BORDER_HEIGHT + y, color);
          }
        }
      }
    }
    texture.Apply ();
  }

  private void SetState (int state) {
    if (state >= states.Length) return;

    _state = state;
    // TODO for test only
    // var stateName = _state > 0 ? states[_state].Name : "None";
    // Debug.Log ("State: " + stateName);
  }

  private int GetState () {
    return _state;
  }

  private void initBorderMask () {
    // draw mask
    texture.PixClear (ref clearColors, Color.white);
    texture.PixRectangle (BORDER_WIDTH, BORDER_HEIGHT, SCREEN_WIDTH, SCREEN_HEIGHT, Color.black, true);
    texture.Apply ();
    // get mask
    borderMask = texture.PixGetPixels (0, 0, FULL_SCREEN_WIDTH, FULL_SCREEN_HEIGHT);
  }

  private int[] numToBin (int dec, int length) {
    var res = new int[length];
    var ix = 0;
    while (length-- > 0) {
      res[ix] = (dec >> length) & 1;
      ix++;
    }
    return res;
  }

  private void onScreenLoaded (byte[] data) {
    this.scrData = data;

    if (data == null || data.Length == 0) {
      Debug.LogError ("Screen not loaded");
      Reset ();
      if (onComplete != null) onComplete ();
      return;
    }

    InitRawData ();
    InitMonochromeImg ();

    timerStart = Time.time;
    SetState (0);
  }

  private void InitRawData () {
    rawData = new List<int> ();
    for (var i = 0; i < scrData.Length; i++) {
      var binary = numToBin (scrData[i], SPRITE_SIZE);
      for (var ix = 0; ix < SPRITE_SIZE; ix++) {
        rawData.Add (binary[ix]);
      }
    }
  }

  private void InitMonochromeImg () {
    monochromeImg = rawData.GetRange (0, MONOCHROME_LEN * SPRITE_SIZE).
      Select (x => x == 0 ? getWhiteColor () : getBlackColor ()).ToList ();
  }

  private Color32 getWhiteColor () {
    return getColor (7);
  }

  private Color32 getBlackColor () {
    return getColor (0);
  }

  private IEnumerator drawCoroutine (Action onComplete) {
    var counter = 0;
    elapsedTime = audioEngine.GetElapsedTime ();

    var timePerPixel = audioEngine.MonochromeDrawingTime / (256 * 192);
    scrPartTime = timePerPixel * DRAW_MONOCHROME_SPEED;

    // drawing monochrome
    for (var third = 0; third < 3 * THIRD_LEN; third += THIRD_LEN) {
      for (var ix = 0; ix < 8 * SCREEN_WIDTH; ix += SCREEN_WIDTH) {
        for (var k = 0; k < 8; k++) {
          for (var i = 0; i < SCREEN_WIDTH; i++) {
            var calcIx = i + k * 8 * SCREEN_WIDTH + ix + third;
            var color = monochromeImg[counter++];
            texture.PixSetPixel (BORDER_WIDTH + calcIx % SCREEN_WIDTH, BORDER_HEIGHT + calcIx / SCREEN_WIDTH, color);

            if (counter % DRAW_MONOCHROME_SPEED == 0) {
              yield return new WaitUntil (() => audioEngine.GetElapsedTime () - elapsedTime >= scrPartTime);

              elapsedTime = audioEngine.GetElapsedTime ();
              texture.Apply ();

              var correctDelta = 0.005f;
              if (counter - elapsedTime / timePerPixel > correctDelta) scrPartTime += correctDelta;
              if (counter - elapsedTime / timePerPixel < -correctDelta) scrPartTime -= correctDelta;
            }
          }
        }
      }
    }
    texture.Apply ();

    var monochromeDrawingTime = audioEngine.GetElapsedTime ();
    var timePerAttr = (audioEngine.GetClipLength () - monochromeDrawingTime) / 768;
    scrPartTime = timePerAttr * DRAW_ATTRIBUTES_SPEED;
    elapsedTime = audioEngine.GetElapsedTime ();
    counter = 0;

    // drawing attributes
    for (var sprY = 0; sprY < SCREEN_SPR_HEIGHT; sprY++) {
      for (var sprX = 0; sprX < SCREEN_SPR_WIDTH; sprX++) {
        var attr = scrData[MONOCHROME_LEN + sprY * SCREEN_SPR_WIDTH + sprX];
        var ink = attr - 8 * (attr / 8);
        var paper = (attr / 8) - 8 * (attr / 64);
        var bright = (attr / 64) == 1 | (attr / 64) == 3;
        // var flash = attr > 127; // now not used

        for (var localY = 0; localY < SPRITE_SIZE; localY++) {
          for (var localX = 0; localX < SPRITE_SIZE; localX++) {
            var x = sprX * SPRITE_SIZE + localX;
            var y = sprY * SPRITE_SIZE + localY;
            var putColor = ink;
            var screenColor = texture.PixGetPixel (BORDER_WIDTH + x, BORDER_HEIGHT + y);
            if (!screenColor.Equals (getColor (0))) putColor = paper;
            var colorIx = bright ? putColor + 8 : putColor;
            var color = getColor (colorIx);
            texture.PixSetPixel (BORDER_WIDTH + x, BORDER_HEIGHT + y, color);
          }
        }

        counter++;
        if (counter % DRAW_ATTRIBUTES_SPEED == 0) {
          if (scrPartTime > 0) yield return new WaitUntil (() => audioEngine.GetElapsedTime () - elapsedTime >= scrPartTime);

          elapsedTime = audioEngine.GetElapsedTime ();
          texture.Apply ();

          var correctDeltaTime = 0.0005f;
          var targetCount = (elapsedTime - monochromeDrawingTime) / timePerAttr;
          var delta = counter - targetCount;
          if (delta > 0) scrPartTime += correctDeltaTime;
          if (delta < 0) scrPartTime -= correctDeltaTime;
        }
      }
    }

    texture.Apply ();

    if (onComplete != null) onComplete ();
  }

  private void onDrawingComplete () {
    SetState (5);
    border (0);
    if (onComplete != null) onComplete ();
  }

  private Color32 getColor (int colorIx) {
    return ZxPalettes.GetColor (colorIx, Palette);
  }

  private void border (int colorIx) {
    texture.PixRectangle (0, 0, FULL_SCREEN_WIDTH, FULL_SCREEN_HEIGHT,
      getColor (colorIx), true, borderMask, FULL_SCREEN_WIDTH, FULL_SCREEN_HEIGHT);
    texture.Apply ();
  }

  private void drawPilot () {
    // drawing lines on the border too often makes no sense
    if (frame % 3 != 0) return;

    for (int i = 0; i < FULL_SCREEN_HEIGHT; i++) {
      var colorIx = (pilotPos + i) % 20 > 10 ? 2 : 5;
      var color = getColor (colorIx);
      texture.PixLine (0, i, FULL_SCREEN_WIDTH, i, color, borderMask, FULL_SCREEN_WIDTH, FULL_SCREEN_HEIGHT);
    }
    pilotPos += Random.Range (0, 4);
    texture.Apply ();
  }

  private void drawLoadingLines () {
    // drawing lines on the border too often makes no sense
    if (frame % 3 != 0) return;

    for (int i = 0; i < FULL_SCREEN_HEIGHT; i++) {
      var colorIx = 1;
      var lineHeight = Random.Range (2, 4);
      if (i % lineHeight == 0) {
        colorIx = Random.Range (0f, 1f) > 0.5 ? 1 : 6;
        texture.PixRectangle (0, i, FULL_SCREEN_WIDTH, lineHeight, getColor (colorIx), true, borderMask, FULL_SCREEN_WIDTH, FULL_SCREEN_HEIGHT);
      }
    }
  }

}
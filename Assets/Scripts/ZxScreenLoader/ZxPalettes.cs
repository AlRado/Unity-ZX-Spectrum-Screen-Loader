using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

  // Is a software representation of a hardware color palettes of Zx-Spectrum
public class ZxPalettes {

  public enum Type {
    Orthodox,
    Pulsar,
    Alone,
    Electroscale,
  }

  public static Dictionary<Type, List<Color32>> COLORS = new Dictionary<Type, List<Color32>> () { 
    { Type.Orthodox, parsePalette ("0000000000cda70000a700cd00b70000b7cda7b700a7b7cd0000000000ffd00000d000ff00e40000e4ffd0e400d0e4ff") },
    { Type.Pulsar, parsePalette ("0000000000cdcd0000cd00cd00cd0000cdcdcdcd00cdcdcd0000000000ffff0000ff00ff00ff0000ffffffff00ffffff") },
    { Type.Alone, parsePalette ("0000000000a0a00000a000a000a00000a0a0a0a000a0a0a00000000000ffff0000ff00ff00ff0000ffffffff00ffffff") },
    { Type.Electroscale, parsePalette ("3e414c4e515f5e62736e73867e839a8e94ad9ea4c1aeb5d43e414c525564666a7c7a7f948e93ada2a8c5b5bcddc9d1f5") },
  };

  private static List<Color32> parsePalette (string palette) {
    var count = palette.Length / 6;
    var colorList = new List<Color32> ();
    for (int i = 0; i < count; i++) {
      colorList.Add (HexToColor (palette.Substring (i * 6, 6)));
    }

    return colorList;
  }

  private static Color32 HexToColor (string hex) {
    var r = byte.Parse (hex.Substring (0, 2), System.Globalization.NumberStyles.HexNumber);
    var g = byte.Parse (hex.Substring (2, 2), System.Globalization.NumberStyles.HexNumber);
    var b = byte.Parse (hex.Substring (4, 2), System.Globalization.NumberStyles.HexNumber);
    return new Color32 (r, g, b, 255);
  }

  public static Color32 GetColor (int colorIx, Type palType) {
    var palette = COLORS[palType];
    var newColorIx = Mathf.Abs(colorIx) % palette.Count;

    return palette[newColorIx];
  }

  public static byte GetColorIx (Color32 color, Type palType) {
    var palette = COLORS[palType];

    return (byte)palette.FindIndex (c => Object.Equals (c, color));
  }

}
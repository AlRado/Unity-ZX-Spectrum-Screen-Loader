using System;
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

// The class simulating the sounds of loading pictures from tape
[RequireComponent (typeof (AudioSource))]
public class ZxTapeAudioEmulator : MonoBehaviour {

    public const int MONOCHROME_BITS_LEN = 6144 * 8;

    // sound tone
    public enum Frequency {
        None = 0,
        Pilot = 807,
        Unit = 197, // 0.1978 ms
        Null = 489 // 0.489 ms
    }

    // signal length
    public const int UNIT_LEN = 18;
    public const int NULL_LEN = 48;

    private Frequency frequency;

    private float gain = 0.5f;
    private float phase;
    private int sampling_frequency = 44000;

    private float increment;

    private AudioSource audioSource;

    [HideInInspector] public float MonochromeDrawingTime;

    private void Awake () {
        frequency = Frequency.None;
        audioSource = gameObject.GetComponent<AudioSource> ();
        audioSource.spatialBlend = 1;
        audioSource.minDistance = 0.1f;
        audioSource.maxDistance = 5f;
    }

    public void SetVolume (float volume) {
        audioSource.volume = volume;
        gain = volume / 8;
    }

    public void InitAudioData (List<int> rawData) {
        var audioData = GetAudioData (rawData);
        var clip = AudioClip.Create ("SoundsOfLoading", audioData.Length, 1, sampling_frequency, false);
        clip.SetData (audioData, 0);
        audioSource.Stop ();
        audioSource.clip = clip;
        audioSource.Play ();
    }

    public float[] GetAudioData (List<int> rawData) {
        List<float> audioData = new List<float> ();

        for (var i = 0; i < rawData.Count; i++) {
            var dataValue = rawData[i];
            var freq = dataValue == 0 ? Frequency.Unit : Frequency.Null;
            var increment = (int) freq * 2 * Mathf.PI / sampling_frequency;

            phase = phase + increment;
            var len = dataValue == 0 ? UNIT_LEN : NULL_LEN;
            var audioValue = audioSource.volume * 200 * Mathf.Sin (phase);
            for (int j = 0; j < len; j++) {
                audioData.Add (audioValue);
            }

            if (i == MONOCHROME_BITS_LEN) {
                // trick
                var tempClip = AudioClip.Create ("TempClip", audioData.Count, 1, sampling_frequency, false);
                tempClip.SetData (audioData.ToArray (), 0);
                MonochromeDrawingTime = tempClip.length;
            }

            if (phase > 2 * Mathf.PI) phase = 0;
        }

        return audioData.ToArray ();
    }

    public void SetEmptySignal () {
        setSignalFrequency (Frequency.None);
    }

    public void SetPilotSignal () {
        setSignalFrequency (Frequency.Pilot);
    }

    public void SetUnitSignal () {
        setSignalFrequency (Frequency.Unit);
    }

    public void SetNullSignal () {
        setSignalFrequency (Frequency.Null);
    }

    private void setSignalFrequency (Frequency frequency) {
        this.frequency = frequency;
    }

    public float GetElapsedTime () {
        return audioSource.time;
    }

    public bool IsPlaying () {
        return audioSource.isPlaying;
    }

    public float GetClipLength () {
        return audioSource.clip.length;
    }

    private void OnAudioFilterRead (float[] data, int channels) {
        if (frequency == Frequency.None) return;

        // update increment in case frequency has changed
        increment = (int) frequency * 2 * Mathf.PI / sampling_frequency;
        for (var i = 0; i < data.Length; i = i + channels) {
            phase = phase + increment;
            // this is where we copy audio data to make them �available� to Unity
            data[i] = (float) (gain * Mathf.Sin (phase));
            // if we have stereo, we copy the mono data to each channel
            if (channels == 2) data[i + 1] = data[i];
            if (phase > 2 * Math.PI) phase = 0;
        }
    }

}
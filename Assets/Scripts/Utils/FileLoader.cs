using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
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

public class FileLoader : MonoBehaviour {

    /**
      fileName: name with extension, e.g. Screen.scr 
     */
    public void LoadFromStreamingAssets (string fileName, Action<byte[]> callback) {
        var path = getDataPath (fileName);
        Load (path, callback);
    }

    public void Load (string path, Action<byte[]> callback) {
        Debug.Log ("Load from path: " + path);
        StartCoroutine (loadBinaryAssetCoroutine (path, callback));
    }

    private IEnumerator loadBinaryAssetCoroutine (string path, Action<byte[]> callback) {
        using (WWW www = new WWW (path)) {
            yield return www;

            if (!string.IsNullOrEmpty (www.error)) Debug.LogError (www.error + ", path: " + path);

            if (callback != null) callback (www.bytes);
        }
    }

    private string getDataPath (string fileName) {
        var streamingAssetsPath = "";
#if UNITY_IPHONE
        streamingAssetsPath = Application.dataPath + "/Raw";
#endif

#if UNITY_ANDROID
        streamingAssetsPath = "jar:file://" + Application.dataPath + "!/assets";
#endif

#if UNITY_STANDALONE || UNITY_EDITOR
        streamingAssetsPath = Application.dataPath + "/StreamingAssets";
#endif

        return Path.Combine (streamingAssetsPath, fileName);
    }

    public bool IsWellFormedUriString (string uriString, UriKind uriKind = UriKind.Absolute) {
        Uri uriResult;
        return Uri.TryCreate (uriString, UriKind.Absolute, out uriResult) &&
            (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
       
    }

}
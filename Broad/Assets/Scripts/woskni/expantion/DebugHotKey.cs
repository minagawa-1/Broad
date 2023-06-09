using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;

public class DebugHotKey : EditorWindow
{
    /// <summary>エディタ再生・リロード</summary>
    /// <remarks>F5キー</remarks>
    [MenuItem("Woskni/Editor/Play _F5")]
    public static void PlayEditor()
    {
        // 再生
        if (!EditorApplication.isPlaying)
            EditorApplication.isPlaying = true;

        // リロード
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    /// <summary>エディタ停止</summary>
    /// <remarks>Ctrl + F5キー</remarks>
    [MenuItem("Woskni/Editor/Stop %F5")]
    public static void StopEditor()
    {
        if (EditorApplication.isPlaying)
            EditorApplication.isPlaying = false;
    }

    /// <summary>エディタ一時停止</summary>
    /// <remarks>Shift + F5キー</remarks>
    [MenuItem("Woskni/Editor/Pause #F5")]
    public static void PauseEditor()
    {
        if (EditorApplication.isPlaying)
            EditorApplication.isPaused = !EditorApplication.isPaused;
    }

    [MenuItem("Woskni/Editor/ScreenShot _F12")]
    public static void ScreenShot()
    {
        string filePath = Application.dataPath + "/";
        string date = System.DateTime.Now.ToString("yy-MM-dd_HH-mm-ss");

        ScreenCapture.CaptureScreenshot(filePath + "ss_" + date + ".png");
    }
}
#endif
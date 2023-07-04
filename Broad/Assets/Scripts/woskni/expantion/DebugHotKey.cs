using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;

public class DebugHotKey : EditorWindow
{
    [MenuItem("Woskni/Editor/ヘルプ出力 _F1")]
    public static void LogHelp()
    {
        string help = "Help: "
            + "\n[F1]ヘルプ出力"
            + "\n[F2]TimeScale減"
            + "\n[F3]TimeScale増"
            + "\n[F4]TimeScaleリセット"
            + "\n[F5]シーン再生・リロード ([ctrl+F5]: シーン停止, [shift+F5]: シーン一時停止"
            + "\n[F6]FPS出力"
            + "\n[F7]"
            + "\n[F8]セーブデータ出力"
            + "\n[F9]スクリーンショットの撮影";

        Debug.Log(help);
    }

    [MenuItem("Woskni/Editor/TimeScale減 _F2")] public static void AddTimeScale() { if (EditorApplication.isPlaying) Time.timeScale = Mathf.Max(0f, Time.timeScale - 0.1f); }
    [MenuItem("Woskni/Editor/TimeScale増 _F3")] public static void SubTimeScale() { if (EditorApplication.isPlaying) Time.timeScale = Mathf.Min(100f, Time.timeScale + 0.1f); }
    [MenuItem("Woskni/Editor/TimeScaleリセット _F4")] public static void ResetTimeScale() { if (EditorApplication.isPlaying) Time.timeScale = 1f; }

    /// <summary>エディタ再生・リロード</summary>
    /// <remarks>F5キー</remarks>
    [MenuItem("Woskni/Editor/シーン再生・リロード _F5")]
    public static void PlayEditor() {
        if (!EditorApplication.isPlaying) EditorApplication.isPlaying = true;   // 再生
        else SceneManager.LoadScene(SceneManager.GetActiveScene().name);        // リロード
    }

    /// <summary>エディタ停止</summary>
    /// <remarks>Ctrl + F5キー</remarks>
    [MenuItem("Woskni/Editor/シーン停止 %F5")]
    public static void StopEditor() { if (EditorApplication.isPlaying) EditorApplication.isPlaying = false; }

    /// <summary>エディタ一時停止</summary>
    /// <remarks>Shift + F5キー</remarks>
    [MenuItem("Woskni/Editor/シーン一時停止 #F5")]
    public static void PauseEditor() { if (EditorApplication.isPlaying) EditorApplication.isPaused = !EditorApplication.isPaused; }

    /// <summary>Debug.Log(FPS)</summary>
    [MenuItem("Woskni/Editor/FPS出力 _F6")] 
    public static void LogHPS() { Debug.Log($"FPS: {1f / Time.deltaTime}"); }

    [MenuItem("Woskni/Editor/セーブデータ出力 _F8")]
    public static void LogSaveData() { SaveSystem.ConfirmData(); }

    /// <summary>スクリーンショット撮影</summary>
    [MenuItem("Woskni/Editor/スクリーンショットの撮影 _F9")]
    public static void PrintScreenshot() => Screenshot.TakeScreenshot();
}
#endif
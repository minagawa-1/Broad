using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;

public class DebugHotKey : EditorWindow
{
    /// <summary>�G�f�B�^�Đ��E�����[�h</summary>
    /// <remarks>F5�L�[</remarks>
    [MenuItem("Woskni/Editor/Play _F5")]
    public static void PlayEditor()
    {
        // �Đ�
        if (!EditorApplication.isPlaying)
            EditorApplication.isPlaying = true;

        // �����[�h
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    /// <summary>�G�f�B�^��~</summary>
    /// <remarks>Ctrl + F5�L�[</remarks>
    [MenuItem("Woskni/Editor/Stop %F5")]
    public static void StopEditor()
    {
        if (EditorApplication.isPlaying)
            EditorApplication.isPlaying = false;
    }

    /// <summary>�G�f�B�^�ꎞ��~</summary>
    /// <remarks>Shift + F5�L�[</remarks>
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
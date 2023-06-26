using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

public class Screenshot
{
    public static void TakeScreenshot()
    {
        string fileName = $"Screenshot({System.DateTime.Now:yyyy-MM-dd[HH-mm-ss]}).png";
        string path = $"{Application.dataPath}/{fileName}";

        ScreenCapture.CaptureScreenshot(path);
        Debug.Log("Screenshot: " + path);
    }
}
#endif
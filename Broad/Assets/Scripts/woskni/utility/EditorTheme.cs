using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

public class EditorTheme
{
    public static Color GetThemeColor()
    {
        float rgb = (float)(EditorGUIUtility.isProSkin ? 56 : 200) / 255f;

        return new Color(rgb, rgb, rgb);
    }
}
#endif
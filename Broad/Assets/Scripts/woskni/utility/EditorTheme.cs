using UnityEditor;
using UnityEngine;

public class EditorTheme
{
    public static Color GetThemeColor()
    {
        float rgb = (float)(EditorGUIUtility.isProSkin ? 56 : 200) / 255f;

        return new Color(rgb, rgb, rgb);
    }
}

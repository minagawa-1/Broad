using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

public class EditorTheme
{
    /// <summary>エディターテーマの種類</summary>
    public enum ThemeType
    {
        Light,
        Dark
    }

    /// <summary>ライトテーマの背景色</summary>
    public static Color lightThemeColor => new Color32(200, 200, 200, 255);

    /// <summary>ライトテーマのアイコン色</summary>
    public static Color lightIconColor => new Color32(86, 86, 86, 255);

    /// <summary>ダークテーマの背景色</summary>
    public static Color darkThemeColor  => new Color32(56, 56, 56, 255);

    /// <summary>ダークテーマのアイコン色</summary>
    public static Color darkIconColor => new Color32(194, 194, 194, 255);

    /// <summary>現在のテーマ</summary>
    public static ThemeType theme => EditorGUIUtility.isProSkin ? ThemeType.Dark : ThemeType.Light;

    /// <summary>現在のテーマの背景色</summary>
    public static Color GetThemeColor(ThemeType? themeType = null) => (themeType ?? theme) == ThemeType.Light ? lightThemeColor : darkThemeColor;

    /// <summary>現在のテーマのアイコン色</summary>
    public static Color GetIconColor(ThemeType? themeType = null) => (themeType ?? theme) == ThemeType.Light ? lightIconColor : darkIconColor;

}
#endif
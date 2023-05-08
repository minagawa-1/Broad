using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColorExpansion
{
    /// <summary>アルファ値を変更した色を取得</summary>
    /// <param name="a">変更するアルファ値</param>
    public static Color GetAlphaColor(this Color color, float a) => new Color(color.r, color.g, color.b, a);



    /// <summary>色相を取得</summary>
    public static float GetHue(this Color color)
    {
        Color.RGBToHSV(color, out float hue, out _, out _);
        return hue;
    }

    /// <summary>彩度を取得</summary>
    public static float GetSaturation(this Color color)
    {
        Color.RGBToHSV(color, out _, out float saturation, out _);
        return saturation;
    }

    /// <summary>明度を取得</summary>
    public static float GetBrightness(this Color color)
    {
        Color.RGBToHSV(color, out _, out _, out float brightness);
        return brightness;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColorExpansion
{
    /// <summary>�A���t�@�l��ύX�����F���擾</summary>
    /// <param name="a">�ύX����A���t�@�l</param>
    public static Color GetAlphaColor(this Color color, float a) => new Color(color.r, color.g, color.b, a);



    /// <summary>�F�����擾</summary>
    public static float GetHue(this Color color)
    {
        Color.RGBToHSV(color, out float hue, out _, out _);
        return hue;
    }

    /// <summary>�ʓx���擾</summary>
    public static float GetSaturation(this Color color)
    {
        Color.RGBToHSV(color, out _, out float saturation, out _);
        return saturation;
    }

    /// <summary>���x���擾</summary>
    public static float GetBrightness(this Color color)
    {
        Color.RGBToHSV(color, out _, out _, out float brightness);
        return brightness;
    }
}

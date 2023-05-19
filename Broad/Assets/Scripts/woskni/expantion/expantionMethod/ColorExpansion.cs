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
    public static float GetValue(this Color color)
    {
        Color.RGBToHSV(color, out _, out _, out float value);
        return value;
    }

    /// <summary>HSVの設定</summary>
    /// <remarks>個別で設定したHSVの色を返す</remarks>
    /// <param name="h">色相</param>
    /// <param name="s">彩度</param>
    /// <param name="v">明度</param>
    public static Color SetHSV(this Color color, float? h = null, float? s = null, float? v = null)
        => Color.HSVToRGB(h ?? color.GetHue(), s ?? color.GetSaturation(), v ?? color.GetValue());

    /// <summary>コントラスト調整</summary>
    /// <param name="contrast">コントラスト倍率 (0 to)</param>
    public static Color Contrast(this Color color, float contrast = 1f)
    {
        // 色のRGB成分を取得
        float r = color.r;
        float g = color.g;
        float b = color.b;

        // RGB成分の平均輝度を計算
        float brightness = (r + g + b) / 3f;

        // コントラスト倍率に基づいて新しいRGB成分を計算
        r = Mathf.Clamp(brightness + (r - brightness) * contrast, 0f, 1f);
        g = Mathf.Clamp(brightness + (g - brightness) * contrast, 0f, 1f);
        b = Mathf.Clamp(brightness + (b - brightness) * contrast, 0f, 1f);

        // 新しいColorを返す
        return new Color(r, g, b, color.a);
    }
}

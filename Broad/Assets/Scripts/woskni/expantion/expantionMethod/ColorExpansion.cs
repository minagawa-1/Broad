using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColorExpansion
{
    /// <summary>RGB値をカラーコードで取得</summary>
    /// <remarks>例: Color(1.0, 0.5, 0.0) => #FF8000FF</remarks>
    /// <param name="isUpper">大文字か否か</param>
    public static string ToHex(this Color color, bool isUpper = true)
    {
        int r = Mathf.RoundToInt(color.r * 255);
        int g = Mathf.RoundToInt(color.g * 255);
        int b = Mathf.RoundToInt(color.b * 255);
        int a = Mathf.RoundToInt(color.a * 255);

        string colorCode = string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", r, g, b, a);

        return isUpper ? colorCode : colorCode.ToLower();
    }

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

    /// <summary>相対色を取得</summary>
    /// <remarks>標準色から均等に離れた色相の色を取得</remarks>
    /// <param name="color">標準色</param>
    /// <param name="num">取得個数</param>
    public static Color[] GetRelativeColor(this Color color, int num)
    {
        Color[] colors = new Color[num];

        float hue = color.GetHue();
        float hueStep = 1f / num;

        for (int i = 0; i < num; i++)
        {
            hue += hueStep;

            if (hue >= 1f) hue -= 1f;

            colors[i] = color.SetHSV(h: hue);
        }

        return colors;
    }
}

public static class Color32Expansion
{
    /// <summary>RGB値をカラーコードで取得</summary>
    /// <remarks>例: Color(1.0, 0.5, 0.0) => #FF8000FF</remarks>
    /// <param name="isUpper">大文字か否か</param>
    public static string ToHex(this Color32 color, bool isUpper = true)
    {
        string colorCode = string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", color.r, color.g, color.b, color.a);

        return isUpper ? colorCode : colorCode.ToLower();
    }

    /// <summary>アルファ値を変更した色を取得</summary>
    /// <param name="a">変更するアルファ値</param>
    public static Color32 GetAlphaColor(this Color32 color, byte a) => new Color32(color.r, color.g, color.b, a);

    /// <summary>色相を取得</summary>
    public static float GetHue(this Color32 color)
    {
        Color.RGBToHSV(color, out float hue, out _, out _);
        return hue;
    }

    /// <summary>彩度を取得</summary>
    public static float GetSaturation(this Color32 color)
    {
        Color.RGBToHSV(color, out _, out float saturation, out _);
        return saturation;
    }

    /// <summary>明度を取得</summary>
    public static float GetValue(this Color32 color)
    {
        Color.RGBToHSV(color, out _, out _, out float value);
        return value;
    }

    /// <summary>HSVの設定</summary>
    /// <remarks>個別で設定したHSVの色を返す</remarks>
    /// <param name="h">色相</param>
    /// <param name="s">彩度</param>
    /// <param name="v">明度</param>
    public static Color32 SetHSV(this Color32 color, float? h = null, float? s = null, float? v = null)
        => Color.HSVToRGB(h ?? color.GetHue(), s ?? color.GetSaturation(), v ?? color.GetValue());

    /// <summary>コントラスト調整</summary>
    /// <param name="contrast">コントラスト倍率 (0 to)</param>
    public static Color32 Contrast(this Color32 color, float contrast = 1f)
    {
        // 色のRGB成分を取得
        float r = (float)color.r / 255f;
        float g = (float)color.g / 255f;
        float b = (float)color.b / 255f;

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
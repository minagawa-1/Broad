using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColorExpansion
{
    /// <summary>RGB�l���J���[�R�[�h�Ŏ擾</summary>
    /// <remarks>��: Color(1.0, 0.5, 0.0) => #FF8000FF</remarks>
    /// <param name="isUpper">�啶�����ۂ�</param>
    public static string ToHex(this Color color, bool isUpper = true)
    {
        int r = Mathf.RoundToInt(color.r * 255);
        int g = Mathf.RoundToInt(color.g * 255);
        int b = Mathf.RoundToInt(color.b * 255);
        int a = Mathf.RoundToInt(color.a * 255);

        string colorCode = string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", r, g, b, a);

        return isUpper ? colorCode : colorCode.ToLower();
    }

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
    public static float GetValue(this Color color)
    {
        Color.RGBToHSV(color, out _, out _, out float value);
        return value;
    }

    /// <summary>HSV�̐ݒ�</summary>
    /// <remarks>�ʂŐݒ肵��HSV�̐F��Ԃ�</remarks>
    /// <param name="h">�F��</param>
    /// <param name="s">�ʓx</param>
    /// <param name="v">���x</param>
    public static Color SetHSV(this Color color, float? h = null, float? s = null, float? v = null)
        => Color.HSVToRGB(h ?? color.GetHue(), s ?? color.GetSaturation(), v ?? color.GetValue());

    /// <summary>�R���g���X�g����</summary>
    /// <param name="contrast">�R���g���X�g�{�� (0 to)</param>
    public static Color Contrast(this Color color, float contrast = 1f)
    {
        // �F��RGB�������擾
        float r = color.r;
        float g = color.g;
        float b = color.b;

        // RGB�����̕��ϋP�x���v�Z
        float brightness = (r + g + b) / 3f;

        // �R���g���X�g�{���Ɋ�Â��ĐV����RGB�������v�Z
        r = Mathf.Clamp(brightness + (r - brightness) * contrast, 0f, 1f);
        g = Mathf.Clamp(brightness + (g - brightness) * contrast, 0f, 1f);
        b = Mathf.Clamp(brightness + (b - brightness) * contrast, 0f, 1f);

        // �V����Color��Ԃ�
        return new Color(r, g, b, color.a);
    }

    /// <summary>���ΐF���擾</summary>
    /// <remarks>�W���F����ϓ��ɗ��ꂽ�F���̐F���擾</remarks>
    /// <param name="color">�W���F</param>
    /// <param name="num">�擾��</param>
    public static Color[] GetRelativeColor(this Color color, int num)
    {
        Color[] colors = new Color[num];
        float hue = color.GetHue();
        float hueStep = 1f / num;

        for (int i = 0; i < num; i++)
        {
            hue += hue > 1f ? hueStep - 1f : hueStep;
            colors[i] = color.SetHSV(h: hue);
        }

        return colors;
    }
}

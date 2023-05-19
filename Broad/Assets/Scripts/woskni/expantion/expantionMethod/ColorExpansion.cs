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
}

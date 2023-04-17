using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColorExpansion
{
    /// <summary>�A���t�@�l��ύX�����F���擾</summary>
    /// <param name="a">�ύX����A���t�@�l</param>
    public static Color GetAlphaColor(this Color color, float a) => new Color(color.r, color.g, color.b, a);
}

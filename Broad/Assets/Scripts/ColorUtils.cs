using UnityEngine;

public class ColorUtils : MonoBehaviour
{
    /// <summary>���ΐF���擾</summary>
    /// <remarks>�W���F����ϓ��ɗ��ꂽ�F���̐F���擾</remarks>
    /// <param name="color">�W���F</param>
    /// <param name="num">�擾��</param>
    public static Color[] GetRelativeColor(Color color, int num)
    {
        Color[] colors = new Color[num];
        float hue = color.GetHue();
        float hueStep = 1f / num;

        for (int i = 0; i < num; i++)
        {
            hue += hueStep;
            if (hue > 1f) hue -= 1f;
            colors[i] = color.SetHSV(h: hue);
        }

        return colors;
    }
}

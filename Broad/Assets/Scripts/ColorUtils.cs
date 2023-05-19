using UnityEngine;

public class ColorUtils : MonoBehaviour
{
    /// <summary>相対色を取得</summary>
    /// <remarks>標準色から均等に離れた色相の色を取得</remarks>
    /// <param name="color">標準色</param>
    /// <param name="num">取得個数</param>
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public static class TextExpansion
{
    /// <summary>ハイライト処理（一定時間で拡縮を行い、一定時間で戻す）</summary>
    /// <param name="scale">拡縮後の拡大率</param>
    /// <param name="duration">ハイライト時間</param>
    public static void DoHighlight(this RectTransform rectTransform, float scale = 0.1f, float duration = 0.2f)
    {
        if (DOTween.IsTweening(rectTransform)) return;

        rectTransform.DOPunchScale(new Vector3(scale, scale, scale), duration);
    }

    /// <summary>指定範囲にリマップした値を返す</summary>
    /// <param name="minValue">最小値</param>
    /// <param name="maxValue">最大値</param>
    public static float Number2Value(this Dropdown dropdown, float minValue, float maxValue)
    {
        // 元の範囲の値を0から1の範囲に正規化
        float normalizedValue = Mathf.InverseLerp(0, dropdown.options.Count - 1, dropdown.value);

        // 正規化された値を新しい範囲にリマップ
        return Mathf.Lerp(minValue, maxValue, normalizedValue);
    }

    public static int Value2Number(this Dropdown dropdown, float value, float minValue, float maxValue)
    {
        // 元の範囲の値を0から1の範囲に正規化
        float normalizedValue = Mathf.InverseLerp(minValue, maxValue, value);

        // 正規化された値を新しい範囲にリマップ
        return (int)Mathf.Lerp(0, dropdown.options.Count - 1, normalizedValue);
    }
}

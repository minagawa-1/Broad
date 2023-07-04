using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public static class TextExpansion
{
    public static void DoHighlight(this RectTransform rectTransform, float scale = 0.1f, float duration = 0.2f)
    {
        if (DOTween.IsTweening(rectTransform)) return;

        rectTransform.DOPunchScale(new Vector3(scale, scale, scale), duration);
    }
}

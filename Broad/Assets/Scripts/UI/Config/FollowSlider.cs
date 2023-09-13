using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FollowSlider : MonoBehaviour
{
    [SerializeField]               Scrollbar m_Scrollbar;
    [SerializeField,Range(0f, 1f)] float m_FollowedPosition;
    [SerializeField]               float m_FollowDuration;

    public void OnSelected()
    {
        DOTween.To(()  => m_Scrollbar.value, (n) => m_Scrollbar.value = n, m_FollowedPosition, m_FollowDuration)
            .SetEase(Ease.OutCubic);
    }
}

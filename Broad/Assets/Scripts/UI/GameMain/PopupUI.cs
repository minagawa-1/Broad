﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

using UnityEngine.InputSystem;

public class PopupUI : MonoBehaviour
{
    public enum PopupType
    {
        /// <summary>位置決定後の待機ポップアップ</summary>
        Wait,

        /// <summary>全手札が設置不可能な時のポップアップ</summary>
        Unsetable,

        /// <summary>任意でスキップを選択したときのポップアップ</summary>
        Skip,
    }

    [Chapter("コンポーネント")]
    [SerializeField] Image  m_PopupBackground;
    [SerializeField] Text   m_PopupText;
    [SerializeField] HandUI m_HandUI;

    [Chapter("その他")]

    [Header("ポップアップ所要時間")]
    [SerializeField] float m_BackgroundDuration;
    [SerializeField] float m_TextDuration;

    [Header("ポップアップ文字")]
    [Multiline(2), SerializeField] string m_WaitPlayerText;
    [Multiline(2), SerializeField] string m_UnsetablePlayerText;
    [Multiline(2), SerializeField] string m_SkipPlayerText;

    public bool isShowing { get; private set; }

    bool IsTweening => DOTween.IsTweening(m_PopupText)
                    || DOTween.IsTweening(m_PopupText.rectTransform)
                    || DOTween.IsTweening(m_PopupBackground);

    /// <summary>ポップアップ表示</summary>
    public void ShowPopup(PopupType popupType)
    {
        if (IsTweening || isShowing) return;

        ReflectPopupType(popupType);

        Sequence sequence = DOTween.Sequence();

        // 背景を表示
        sequence.Append(m_PopupBackground.DOFade(0.75f, m_BackgroundDuration).SetEase(Ease.OutCubic));

        // テキストを表示
        sequence.Join(m_PopupText.DOFade(1f, m_TextDuration).SetEase(Ease.OutCubic).SetDelay(0.1f));
        sequence.Join(m_PopupText.rectTransform.DOScale(1f, m_TextDuration).SetEase(Ease.OutBack));

        sequence.AppendCallback(() => isShowing = true);
    }

    /// <summary>ポップアップ非表示</summary>
    public void HidePopup()
    {
        if (IsTweening || !isShowing) return;

        Sequence sequence = DOTween.Sequence();

        // テキストを非表示
        sequence.Append(m_PopupText.DOFade(0f, m_TextDuration).SetEase(Ease.OutCubic));
        sequence.Join(m_PopupText.rectTransform.DOScale(0.5f, m_TextDuration).SetEase(Ease.OutBack));

        // 背景を非表示
        sequence.Join(m_PopupBackground.DOFade(0f, m_BackgroundDuration).SetEase(Ease.OutCubic).SetDelay(0.1f));

        sequence.AppendCallback(() => isShowing = false);
    }

    void ReflectPopupType(PopupType popupType)
    {
        // テキスト内容の更新
        switch (popupType)
        {
            case PopupType.Wait:
                m_PopupText.text = m_WaitPlayerText;

                // 大将にする（手札を後面にする）
                transform.SetSiblingIndex(transform.parent.childCount - 1);
                break;

            case PopupType.Unsetable:
                m_PopupText.text = m_UnsetablePlayerText;

                m_HandUI.Interactate();

                // 次鋒にする（手札を前面にする）
                transform.SetSiblingIndex(2);
                break;

            case PopupType.Skip:
                m_PopupText.text = m_SkipPlayerText;

                m_HandUI.Interactate();

                // 次鋒にする（手札を前面にする）
                transform.SetSiblingIndex(2);
                break;
        }
    }
}

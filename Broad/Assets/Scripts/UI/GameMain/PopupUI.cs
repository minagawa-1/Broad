using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

using UnityEngine.InputSystem;

public class PopupUI : MonoBehaviour
{
    public enum PopupType
    {
        Wait,
        Unsetable,
    }

    [Chapter("コンポーネント")]
    [SerializeField] Image m_PopupBackground;
    [SerializeField] Text  m_PopupText;

    [Chapter("その他")]

    [Header("ポップアップ所要時間")]
    [SerializeField] float m_BackgroundDuration;
    [SerializeField] float m_TextDuration;

    [Header("ポップアップ文字")]
    [Multiline(2), SerializeField] string m_WaitPlayerText;
    [Multiline(2), SerializeField] string m_UnsetablePlayerText;

    public bool isShowing { get; private set; }

    bool IsTweening => DOTween.IsTweening(m_PopupText)
                    || DOTween.IsTweening(m_PopupText.rectTransform)
                    || DOTween.IsTweening(m_PopupBackground);

    private void Update()
    {
        // Startボタンを押したときの処理
        if(Gamepad.current.startButton.wasPressedThisFrame)
        {
            // Selectボタンを押しながらStartボタンを押すとUnsetableポップアップを表示
            PopupType popupType = Gamepad.current.selectButton.isPressed ? PopupType.Unsetable : PopupType.Wait;

            if (isShowing) HidePopup();
            else           ShowPopup(popupType);
        }
    }

    /// <summary>ポップアップ表示</summary>
    public void ShowPopup(PopupType popupType)
    {
        if (IsTweening || isShowing) return;

        ReflectPopupType(popupType);

        Sequence sequence = DOTween.Sequence();

        // 背景を表示
        sequence.Append(m_PopupBackground.DOFade(0.5f, m_BackgroundDuration).SetEase(Ease.OutCubic));

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

    /// <summary>ポップアップUIの見た目を種類に合わせて反映する</summary>
    /// <param name="popupType">反映させる種類</param>
    void ReflectPopupType(PopupType type)
    {
        // テキスト内容の更新
        switch (type)
        {
            case PopupType.Wait:
                m_PopupText.text = m_WaitPlayerText;

<<<<<<< HEAD
                // 一番下にする（手札を後面にする）
=======
                // 大将にする
>>>>>>> 7770864a5be58c031180ca8880d2646eb9644104
                transform.SetSiblingIndex(transform.parent.childCount - 1);
                break;

            case PopupType.Unsetable:
                m_PopupText.text = m_UnsetablePlayerText;

<<<<<<< HEAD
                m_HandUI.Interactate();

                // 下から２番目にする（手札を前面にする）
                transform.SetSiblingIndex(transform.parent.childCount - 2);
                break;

            case PopupType.Skip:
                m_PopupText.text = m_SkipPlayerText;

                m_HandUI.Interactate();

                // 下から２番目にする（手札を前面にする）
                transform.SetSiblingIndex(transform.parent.childCount - 2);
=======
                // 次鋒にする
                transform.SetSiblingIndex(1);
>>>>>>> 7770864a5be58c031180ca8880d2646eb9644104
                break;
        }
    }
}

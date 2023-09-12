using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using DG.Tweening;

public class Demo : MonoBehaviour
{
    [Name("デモの非表示時間")]
    [SerializeField] float m_DemoHideTime;

    [Header("UI表示の所要時間")]
    [SerializeField] float m_PopupDuration;

    [Chapter("コンポーネント")]
    [SerializeField] Image m_EdgeImage;
    [SerializeField] Text m_TitleText;
    [SerializeField] Text m_PushText;

    RawImage m_RawImage;
    VideoPlayer m_VideoPlayer;

    woskni.Timer m_DemoHideTimer;

    public bool showing { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        m_DemoHideTimer = new woskni.Timer(m_DemoHideTime);
        m_EdgeImage.rectTransform.localScale = Vector3.zero;

        m_RawImage = GetComponent<RawImage>();
        m_VideoPlayer = GetComponent<VideoPlayer>();

        showing = false;

        // 画面の最上位に移動する
        transform.parent.SetParent(Transition.instance.fadeCanvasGroup.transform.parent);
        transform.parent.SetAsFirstSibling();
    }

    // Update is called once per frame
    void Update()
    {
        if(!showing)
        {
            m_DemoHideTimer.Update();

            if (WasPressedAnyButton()) m_DemoHideTimer.Reset();

            if (m_DemoHideTimer.IsFinished())
            {
                showing = true;

                ShowDemo();
            }
        }
        else
        {
            // いずれかのボタンが押されたら
            if (WasPressedAnyButton())
            {
                showing = false;

                HideDemo();
            }
        }
    }

    void ShowDemo()
    {
        m_DemoHideTimer.Reset();
        m_VideoPlayer.Play();

        Sequence sequence = DOTween.Sequence();

        // 背景を表示
        sequence.Append(m_EdgeImage.DOFade(0.75f, m_PopupDuration).SetEase(Ease.OutCubic));
        sequence.Join(m_EdgeImage.rectTransform.DOScale(1f, m_PopupDuration).SetEase(Ease.OutBack));

        // テキストを表示
        sequence.Join(m_TitleText.DOFade(1f, m_PopupDuration - 0.1f).SetEase(Ease.OutCubic).SetDelay(0.1f));
        sequence.Join(m_PushText .DOFade(1f, m_PopupDuration - 0.1f).SetEase(Ease.OutCubic).SetDelay(0.1f));
        sequence.Join(m_RawImage .DOFade(1f, m_PopupDuration - 0.1f).SetEase(Ease.OutCubic).SetDelay(0.1f));
    }

    void HideDemo()
    {
        Sequence sequence = DOTween.Sequence();

        // テキストを表示
        sequence.Append(m_TitleText.DOFade(0f, m_PopupDuration).SetEase(Ease.InCubic));
        sequence.Join  (m_PushText .DOFade(0f, m_PopupDuration).SetEase(Ease.InCubic));
        sequence.Join  (m_RawImage .DOFade(0f, m_PopupDuration).SetEase(Ease.InCubic));

        // 背景を表示
        sequence.Join(m_EdgeImage.DOFade(0f, m_PopupDuration - 0.1f).SetEase(Ease.InCubic).SetDelay(0.1f));
        sequence.Join(m_EdgeImage.rectTransform.DOScale(0f, m_PopupDuration - 0.1f).SetEase(Ease.InBack).SetDelay(0.1f));

        // 映像の停止（Pause関数と違ってStop関数の場合、次回再生時は最初からになる）
        sequence.AppendCallback(m_VideoPlayer.Stop);
    }

    bool WasPressedAnyButton()
    {
        if (Gamepad.current != null)
        {
            foreach (ButtonControl button in Gamepad.current.allControls)
                if (button.wasPressedThisFrame) return true;
        }

        if (Keyboard.current != null)
        {
            foreach (ButtonControl button in Keyboard.current.allControls)
                if (button.wasPressedThisFrame) return true;
        }

        return false;
    }
}

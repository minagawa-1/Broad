using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class Demo : MonoBehaviour
{
    [Name("デモの非表示時間")]
    [SerializeField] float m_DemoHideTime;

    [Header("UI表示の所要時間")]
    [SerializeField] float m_PopupDuration;

    [Chapter("コンポーネント")]
    [SerializeField] Image m_EdgeImage;
    [SerializeField] Text  m_TitleText;
    [SerializeField] Text  m_PushText;

    RawImage m_RawImage;
    VideoPlayer m_VideoPlayer;

    woskni.Timer m_DemoHideTimer;

    private InputAction m_AnyKeyAction = new InputAction(type: InputActionType.PassThrough, binding: "*/<Button>", interactions: "Press");

    private void OnEnable() => m_AnyKeyAction.Enable();
    private void OnDisable() => m_AnyKeyAction.Disable();

    public bool showing { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        m_RawImage = GetComponent<RawImage>();
        m_VideoPlayer = GetComponent<VideoPlayer>();

        // タイマーを初期化
        m_DemoHideTimer = new woskni.Timer(m_DemoHideTime);

        // 外枠の拡大率を初期化
        m_EdgeImage.rectTransform.localScale = Vector3.zero;

        // 色を初期化
        m_EdgeImage.color = m_EdgeImage.color.GetAlphaColor(0f);
        m_TitleText.color = m_TitleText.color.GetAlphaColor(0f);
        m_PushText .color = m_PushText .color.GetAlphaColor(0f);
        m_RawImage .color = m_RawImage .color.GetAlphaColor(0f);

        showing = false;

        // 画面の最上位に移動する
        transform.parent.SetParent(Transition.instance.fadeCanvasGroup.transform.parent);
        transform.parent.SetAsLastSibling();
    }

    // Update is called once per frame
    void Update()
    {
        if(!showing)
        {
            // タイトルシーン以外が開かれている場合はポップアップを開かない
            if (SceneManager.sceneCount > 1 || SceneManager.GetActiveScene().buildIndex != (int)Scene.TitleScene)
            {
                // タイマーのリセット
                if (m_DemoHideTimer.IsStarted()) m_DemoHideTimer.Reset();
                return;
            }

            m_DemoHideTimer.Update();

            if (m_AnyKeyAction.triggered) m_DemoHideTimer.Reset();

            if (m_DemoHideTimer.IsFinished())
            {
                showing = true;

                ShowDemo();
            }
        }
        else
        {
            // いずれかのボタンが押されたら
            if (m_AnyKeyAction.triggered)
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
}

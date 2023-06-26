using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class StartButtonState : MonoBehaviour
{
    [Header("コンポーネント")]
    [SerializeField] Text m_StartText;
    [SerializeField] Text m_MatchingText;
    [SerializeField] Image m_Banner;

    [Chapter("テキストのDOTween前後情報")]
    [Header("テキストのサイズ・位置情報")]
    [SerializeField] int m_EasedStartTextFontSize;
    [SerializeField] Vector3 m_EasedStartTextOffset;

    [Chapter("遷移時間")]
    [Header("Button")]
    [SerializeField] float m_ButtonStartTime;
    [SerializeField] float m_ButtonEndTime;

    [Header("Banner")]
    [SerializeField] float m_BannerStartTime;
    [SerializeField] float m_BannerEndTime;

    int m_InitStartTextFontSize;
    Vector3 m_InitStartTextPosition;

    private void Start()
    {
        m_InitStartTextFontSize = m_StartText.fontSize;
        m_InitStartTextPosition = m_StartText.rectTransform.position;
    }

    public void DoStartMatch()
    {
        DoFontSize(m_EasedStartTextFontSize, m_ButtonStartTime, Ease.OutCubic);
        m_MatchingText.DOPause();
        m_MatchingText.DOFade(1f, m_ButtonStartTime).SetEase(Ease.OutCubic);

        m_StartText.rectTransform.DOMove(m_InitStartTextPosition + m_EasedStartTextOffset, m_ButtonStartTime).SetEase(Ease.OutCubic);
        m_Banner.rectTransform.DOMoveY(0f, m_BannerStartTime).SetEase(Ease.OutCubic);
    }

    public void DoCancelMatch(System.Action<int> setPlayerNum)
    {
        DoFontSize(m_InitStartTextFontSize, m_ButtonEndTime, Ease.InCubic);
        m_MatchingText.DOPause();
        m_MatchingText.DOFade(0f, m_ButtonEndTime).SetEase(Ease.InCubic).OnComplete(() => setPlayerNum.Invoke(1));

        m_StartText.rectTransform.DOMove(m_InitStartTextPosition, m_ButtonEndTime).SetEase(Ease.InCubic);
        m_Banner.rectTransform.DOMoveY(-m_Banner.rectTransform.rect.height, m_BannerEndTime).SetEase(Ease.InCubic);
    }

    void DoFontSize(int endSize, float time, Ease ease)
    {
        DOTween.To(() => m_StartText.fontSize, size => m_StartText.fontSize = size, endSize, time).SetEase(ease);
    }
}

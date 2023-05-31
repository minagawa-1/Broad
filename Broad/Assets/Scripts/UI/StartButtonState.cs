using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class StartButtonState : MonoBehaviour
{
    [Header("�R���|�[�l���g")]
    [SerializeField] Text m_StartText;
    [SerializeField] Text m_MatchingText;

    [Chapter("�e�L�X�g��DOTween�O����")]
    [Header("�e�L�X�g�̃T�C�Y�E�ʒu���")]
    [SerializeField] int m_EasedStartTextFontSize;
    [SerializeField] Vector3 m_EasedStartTextOffset;

    [Header("�J�ڎ���")]
    [SerializeField] float m_StartTime;
    [SerializeField] float m_EndTime;

    int m_InitStartTextFontSize;
    Vector3 m_InitStartTextPosition;

    private void Start()
    {
        m_InitStartTextFontSize = m_StartText.fontSize;
        m_InitStartTextPosition = m_StartText.rectTransform.position;
    }

    public void DoStartMatch()
    {
        Vector3 startTextPos = m_InitStartTextPosition + m_EasedStartTextOffset;

        DoFontSize(m_EasedStartTextFontSize, startTextPos, m_StartTime);

        m_MatchingText.DOFade(1f, m_EndTime);
    }

    public void DoEndMatch()
    {
        DoFontSize(m_InitStartTextFontSize, m_InitStartTextPosition, m_EndTime);

        m_MatchingText.DOFade(0f, m_EndTime);
    }

    void DoFontSize(int endSize, Vector3 endPosition, float time)
    {
        DOTween.To(() => m_StartText.fontSize, size => m_StartText.fontSize = size, endSize, time).SetEase(Ease.OutCubic);

        m_StartText.rectTransform.DOMove(endPosition, time);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BlinkGraphic : MonoBehaviour
{
    [SerializeField] float m_BlinkTime;

    [Header("0～1: BlinkUp" + "\n" +
            "1～2: BlinkDown")]
    [SerializeField, Range(0f, 2f)] float m_TimeOffset;

    [Header("イージングの種類")]
    [SerializeField] Ease m_UpEase = Ease.OutQuart;
    [SerializeField] Ease m_DownEase = Ease.InQuart;

    [Header("遷移後のカラー")]
    [SerializeField] Color m_AfterColor = Color.white;

    Graphic m_Graphic;

    Color m_BeforeColor;

    // Start is called before the first frame update
    void Start()
    {
        m_Graphic = GetComponent<Graphic>();

        m_BeforeColor = m_Graphic.color;

        // 最初は上がりか下がりかを判断する
        bool isUp = m_TimeOffset < 1f;

        m_Graphic.color = isUp ? m_BeforeColor : m_AfterColor;
        if (isUp) BlinkUp(); else BlinkDown();

        DOTween.Goto(m_Graphic, isUp ? m_TimeOffset : m_TimeOffset - 1f, true);
    }

    void BlinkUp()
    {
        m_Graphic.DOColor(m_AfterColor, m_BlinkTime).SetEase(m_UpEase).OnComplete(() => BlinkDown());
    }

    void BlinkDown()
    {
        m_Graphic.DOColor(m_BeforeColor, m_BlinkTime).SetEase(m_DownEase).OnComplete(() => BlinkUp());
    }
}

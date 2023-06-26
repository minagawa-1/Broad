using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BlinkGraphic : MonoBehaviour
{
    [SerializeField] float m_BlinkTime;

    Graphic m_Graphic;

    // Start is called before the first frame update
    void Start()
    {
        m_Graphic = GetComponent<Graphic>();
        m_Graphic.color = m_Graphic.color.GetAlphaColor(0f);

        BlinkUp();
    }

    void BlinkUp()
    {
        if (GameSetting.instance.playersColor.Length > 0) return;

        m_Graphic.DOFade(1f, m_BlinkTime).SetEase(Ease.OutQuart).OnComplete(() => BlinkDown());
    }

    void BlinkDown()
    {
        if (GameSetting.instance.playersColor.Length > 0) return;

        m_Graphic.DOFade(0f, m_BlinkTime).SetEase(Ease.InQuart).OnComplete(() => BlinkUp());
    }
}

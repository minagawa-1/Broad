using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SynchronizeAlpha : MonoBehaviour
{
    Shadow[] m_Shadows;
    Outline[] m_Outlines;

    Graphic m_Graphic;

    // Start is called before the first frame update
    void Start()
    {
        m_Shadows = GetComponents<Shadow>();
        m_Outlines = GetComponents<Outline>();

        m_Graphic = GetComponent<Graphic>();
    }

    // Update is called once per frame
    void Update()
    {
        float alpha = m_Graphic.color.a;

        foreach (Shadow shadow in m_Shadows)
            shadow.effectColor = shadow.effectColor.GetAlphaColor(alpha);

        foreach (Outline outline in m_Outlines)
            outline.effectColor = outline.effectColor.GetAlphaColor(alpha);
    }
}

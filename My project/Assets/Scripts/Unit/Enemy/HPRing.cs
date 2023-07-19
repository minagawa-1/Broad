using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPRing : MonoBehaviour
{
    [SerializeField] Enemy m_Enemy;

    [SerializeField] UnityEngine.UI.Image m_Image;

    int m_MaxHP;

    // Start is called before the first frame update
    void Start()
    {
        // HP�ő�l��Enemy��HP����
        m_MaxHP = m_Enemy.stock;
    }

    // Update is called once per frame
    void Update()
    {
        m_Image.fillAmount = m_Enemy.stock / (float)m_MaxHP;	// Slider�Ɍ���HP��K�p
    }
}

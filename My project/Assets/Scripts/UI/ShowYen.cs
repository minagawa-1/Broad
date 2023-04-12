using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//Š‹à•\¦ƒNƒ‰ƒX
public class ShowYen : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI m_YenText = null;

    void Update()
    {
        m_YenText.text = SaveSystem.m_SaveData.money.ToString();
    }
}

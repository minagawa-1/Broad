using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrentTimeText : MonoBehaviour
{
    [SerializeField] Text m_Text;
    [SerializeField] TimeCycle m_TimeCycle;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float dateTime = m_TimeCycle.m_CurrentTime;

        int hour = (int)dateTime;
        int minute = (int)((dateTime - (float)hour) * 60f);

        m_Text.text = "éûçèÅF" + hour.ToString() + " éû " + (minute < 10 ? "0" : "") + minute.ToString() + " ï™";
    }
}

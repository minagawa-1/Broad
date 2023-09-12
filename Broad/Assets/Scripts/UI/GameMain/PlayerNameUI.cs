using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNameUI : MonoBehaviour
{
    RectTransform m_RectTransform;

    List<(Image icon, Text text)> m_Contents;

    // Start is called before the first frame update
    void Start()
    {
        m_RectTransform = GetComponent<RectTransform>();

        for(int i = 0; i < transform.childCount; ++i)
        {
            Image icon = transform.GetChild(i).GetComponentInChildren<Image>();
            Text  text = transform.GetChild(i).GetComponentInChildren<Text>();

            icon.color = GameSetting.instance.playerColors[i];
            text.text = GameSetting.instance.playerNames[i];

            Debug.Log($"PlayerNameUI.text: {text.text}");

            m_Contents.Add((icon, text));
        }
    }
}

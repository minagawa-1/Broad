using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNameUI : MonoBehaviour
{
    List<(Image icon, Text text)> m_Contents;

    // Start is called before the first frame update
    void Start()
    {
        m_Contents = new List<(Image icon, Text text)>();

        for (int i = 0; i < transform.childCount; ++i)
        {
            Image icon = transform.GetChild(i).GetComponentInChildren<Image>();
            Text text = transform.GetChild(i).GetComponentInChildren<Text>();

            icon.color = GameSetting.instance.playerColors[i];
            text.text = GameSetting.instance.playerNames[i];

            m_Contents.Add((icon, text));
        }
    }
}

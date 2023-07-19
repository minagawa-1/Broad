using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DebugText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI m_Text = null;
    [SerializeField] float m_UpdateInterval = 0f;

    woskni.Timer m_Timer;

    // Start is called before the first frame update
    void Start()
    {
        m_Timer.Setup(m_UpdateInterval);
    }

    // Update is called once per frame
    void Update()
    {
        m_Timer.Update(false);

        if (!m_Timer.IsFinished()) return;

        m_Timer.Reset();

        var debug = (bullet: "", fps: "");
        //debug.bullet = nameof(debug.bullet) + ":" + BulletManager.m_BulletList.Count.ToString();
        debug.fps = (Time.deltaTime == 0f ? 0f : 1f / Time.deltaTime).ToString("f2");

        bool isFPS60 = (Time.deltaTime == 0f ? 0f : 1f / Time.deltaTime) >= 60f;
        debug.fps = "<color=#" + (isFPS60 ? "ffffff" : "ff0000") + ">" + debug.fps + "</color>";

        m_Text.text = debug.bullet + "\n" + nameof(debug.fps) + ":" + debug.fps;
    }
}

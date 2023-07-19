using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Trademark : MonoBehaviour
{
    [SerializeField] int m_FrameRate = 60;
    [SerializeField] float m_WaitTime;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = m_FrameRate;

        StartCoroutine(WaitTitle());
    }

    IEnumerator WaitTitle()
    {
        yield return new WaitForSeconds(m_WaitTime);

        SaveSystem.Load();
        woskni.Scene.Change(woskni.Scenes.Title);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonGuideActivate : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (Config.data != null) gameObject.SetActive(Config.data.buttonGuide);
    }

    /// <summary>アクティブ・非アクティブを設定する</summary>
    public void SetActive() => gameObject.SetActive(Config.data.buttonGuide);
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//タグクラス
public class WeaponTag : MonoBehaviour
{
    [SerializeField] Text       m_Text          = null;                 //テキストコンポーネント
    public RectTransform        rectTransform   { get; private set; }   //矩形トランスフォーム

    /// <summary>初期化</summary>
    /// <param name="text">タグの内容</param>
    public void Setup(string text)
    {
        //矩形トランスフォーム取得
        rectTransform = GetComponent<RectTransform>();

        //文字列設定
        m_Text.text = "#" + text;

        //大きさ設定
        float offset = rectTransform.sizeDelta.y - m_Text.preferredHeight;
        rectTransform.sizeDelta = new Vector2(m_Text.preferredWidth + offset, rectTransform.sizeDelta.y);
    }
}

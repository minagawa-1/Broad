using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class RemainingTurnUI : MonoBehaviour
{
    [SerializeField] woskni.RangeInt m_FontSizeRange;
    [SerializeField] Color m_TenseColor;

    [Header("残り何ターン目から緊迫状態を開始するか")]
    [SerializeField] int m_StartTenseTurn;

    [Header("\"のこり\"の部分のテキスト内容")]
    [SerializeField] string m_RemainingText;

    Text m_TurnText;

    Color m_BasisColor;

    // Start is called before the first frame update
    void Start()
    {
        m_TurnText = GetComponent<Text>();
        m_BasisColor = m_TurnText.color;
    }

    public void UpdateTurnUI(Hand hand)
    {
        var tenseRange = new woskni.RangeInt(1, m_StartTenseTurn);

        int turn = hand.deck.deck.Count + (GameSetting.hand_blocks - hand.FindBlank().Length);

        float rate = Mathf.InverseLerp(tenseRange.min, tenseRange.max, turn);

        m_TurnText.fontSize = (int)Mathf.Lerp(m_FontSizeRange.max, m_FontSizeRange.min, rate);

        m_TurnText.color = Color.Lerp(m_TenseColor, m_BasisColor, rate);

        m_TurnText.text = $"{m_RemainingText}\n{turn}";
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode, RequireComponent(typeof(Text))]
public class SliderValue : MonoBehaviour
{
    [SerializeField] Slider m_Slider;

    [SerializeField] bool m_IsTextValue;

    [ShowIf("m_IsTextValue")]
    [SerializeField] string[] m_Values;

    Text m_Text;

    private void OnEnable()
    {
        m_Text = GetComponent<Text>();
    }

    void Update()
    {
        if (m_Text == null) m_Text = GetComponent<Text>();

        if (!m_Slider.wholeNumbers) return;

        System.Array.Resize(ref m_Values, GetLength());
    }

    int GetLength() => (int)(m_Slider.maxValue - m_Slider.minValue) + 1;
    int GetValue() => (int)(m_Slider.value - m_Slider.minValue);

    public void OnChanged()
    {
        if (m_IsTextValue)
        {
            // floatスライダー or テキストの内容が変更後と同じ の場合は何もしない
            if (!m_Slider.wholeNumbers || m_Text.text == m_Values[GetValue()]) return;

            System.Array.Resize(ref m_Values, GetLength());

            m_Text.text = m_Values[GetValue()];
        }
        else
        {
            m_Text.text = m_Slider.value.ToString();
        }
    }
}

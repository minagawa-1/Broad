using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//�^�O�N���X
public class WeaponTag : MonoBehaviour
{
    [SerializeField] Text       m_Text          = null;                 //�e�L�X�g�R���|�[�l���g
    public RectTransform        rectTransform   { get; private set; }   //��`�g�����X�t�H�[��

    /// <summary>������</summary>
    /// <param name="text">�^�O�̓��e</param>
    public void Setup(string text)
    {
        //��`�g�����X�t�H�[���擾
        rectTransform = GetComponent<RectTransform>();

        //������ݒ�
        m_Text.text = "#" + text;

        //�傫���ݒ�
        float offset = rectTransform.sizeDelta.y - m_Text.preferredHeight;
        rectTransform.sizeDelta = new Vector2(m_Text.preferredWidth + offset, rectTransform.sizeDelta.y);
    }
}

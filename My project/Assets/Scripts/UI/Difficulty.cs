using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//��Փx�N���X
public class Difficulty : MonoBehaviour
{
    [SerializeField] Color  m_HardestColor      = Color.red;    //��ԓ�����̐F
    [SerializeField] Color  m_EasiestColor      = Color.green;  //��ԈՂ������̐F
    [SerializeField] Image  m_SliderFillArea    = null;         //�h��Ԃ��G���A
    [SerializeField] Slider m_Slider            = null;         //�X���C�_�[
    [SerializeField] float  m_BaseScreenHeight  = 1920f;        //��ʍ�����l
    Transform               m_SliderTransform   = null;         //�X���C�_�[�̃g�����X�t�H�[��
    Vector3                 m_PointerPos        = Vector3.zero; //�|�C���^�[�ʒu
    float                   m_Hight             = 0.0f;         //�X���C�_�[����
    bool                    m_TouchFlag         = false;        //�^�b�`�t���Os

    /// <summary>��Փx�擾</summary>
    public float GetDifficulty() { return m_Slider.value; }

    /// <summary>�X���C�_�[�̈ʒu���Z�b�g</summary>
    public void ResetSlider() { m_Slider.value = SaveSystem.m_SaveData.difficulty; }

    void Start()
    {
        //���擾
        m_Hight = m_Slider.GetComponent<RectTransform>().sizeDelta.y;
        m_SliderTransform = m_Slider.transform;

        //�f�[�^�ǂݍ���
        m_Slider.value = SaveSystem.m_SaveData.difficulty;
    }

    void Update()
    {
        //�Q�[�W�̊����ŃQ�[�W�̐F��ς���
        Color.RGBToHSV(m_EasiestColor, out float startH, out float s, out float v);
        Color.RGBToHSV(m_HardestColor, out float endH, out s, out v);
        float h = woskni.Easing.Linear(m_Slider.value, m_Slider.maxValue, startH, endH);
        m_SliderFillArea.color = Color.HSVToRGB(h, s, v);

        //�^�b�`����ĂȂ���Ή������Ȃ�
        if (!m_TouchFlag) return;

        //�|�C���^�[�ʒu�擾
        m_PointerPos = woskni.InputManager.GetInputPosition(Application.platform);

        //�X���C�_�[�ʒu����
        float sliderPosY = m_SliderTransform.position.y - (m_Hight * (Screen.height / m_BaseScreenHeight) / 2);
        //�^�b�`�ʒu�Ƃ̍����犄�����Z�o
        float ratio = (m_PointerPos.y - sliderPosY) / (m_Hight * (Screen.height / m_BaseScreenHeight));

        //�������
        m_Slider.value = ratio;
    }

    /// <summary>�^�b�`����</summary>
    public void PointerDown()
    {
        m_TouchFlag = true;
    }

    /// <summary>�^�b�`���</summary>
    public void PointerUp()
    {
        m_TouchFlag = false;
    }
}

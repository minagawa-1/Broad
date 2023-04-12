using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//����w���s�_�C�A���O�N���X
public class NotAllowedBuyDialog : MonoBehaviour
{
    [SerializeField] CanvasGroup    m_CanvasGroup   = null;
    [SerializeField] float          m_VanishTime    = 1.0f;
    woskni.Timer m_VanishTimer;

    private void Start()
    {
        //�ŏ��͔�\���ɂ���
        m_CanvasGroup.alpha = 0f;

        //�^�C�}�[�Z�b�g
        m_VanishTimer.Setup(m_VanishTime);

        //�^�C�}�[�������I��
        m_VanishTimer.Fin();
    }

    private void Update()
    {
        //�^�C�}�[���I�����Ă����炱���ŏI��
        if (m_VanishTimer.IsFinished()) return;

        //�^�C�}�[�X�V
        m_VanishTimer.Update();

        //���炩�ɓ����ɂ���
        m_CanvasGroup.alpha = woskni.Easing.InQuintic(m_VanishTimer.time, m_VanishTimer.limit, 1f, 0f);

        //�^�C�}�[���I��������덷��␳
        if (m_VanishTimer.IsFinished()) m_CanvasGroup.alpha = 0f;

    }

    /// <summary>�_�C�A���O�\��</summary>
    public void ShowDialog()
    {
        //�^�C�}�[���Z�b�g
        m_VanishTimer.Reset();

        //�����x���Z�b�g
        m_CanvasGroup.alpha = 1f;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugPlayer : MonoBehaviour
{
    public static bool isDebug = false;

    //�v���C���[�N���X���擾
    [SerializeField] Player m_Player;
    [SerializeField] Text   m_DebugText;
    [SerializeField] Enemy  m_Enemy;

    [Header("�ӏ܃��[�h�ɂȂ�Ƃ��ɔ�\���ɂȂ���")]
    [SerializeField] Image          m_EnemyHPRing       = null; //�G��HP�����O
    [SerializeField] Image          m_EnemyHPRingShadow = null; //�G��HP�����O�̉e
    [SerializeField] Image          m_EnemyPosition     = null; //�G�ʒu
    [SerializeField] Image          m_PlayerPosition    = null; //�v���C���[�ʒu
    [SerializeField] SpriteRenderer m_PlayerImage       = null; //�v���C���[�摜
    [SerializeField] SpriteRenderer m_WandImage         = null; //�����h�摜
    [SerializeField] SpriteRenderer m_PlayerCoreImage   = null; //�v���C���[�̓����蔻��

    void Update()
    {
        if (woskni.KeyBoard.GetAndKeyDown(KeyCode.Alpha1))
        {
            m_Player.invincibleFlag = !m_Player.invincibleFlag;

            //m_DebugText.enabled = m_Player.invincibleFlag;
        }
        if (woskni.KeyBoard.GetAndKeyDown(KeyCode.Alpha2))
        {
            m_Enemy.Damage(10000000, 0f);
        }
        if (woskni.KeyBoard.GetAndKeyDown(KeyCode.Alpha3))
        {
            bool flag = m_EnemyHPRing.enabled;

            m_EnemyHPRing.enabled       = !flag;
            m_EnemyHPRingShadow.enabled = !flag;
            m_EnemyPosition.enabled     = !flag;
            m_PlayerPosition.enabled    = !flag;
            m_PlayerImage.enabled       = !flag;
            m_WandImage.enabled         = !flag;
            m_PlayerCoreImage.enabled   = !flag;
            m_Player.invincibleFlag     = !m_Player.invincibleFlag;
            isDebug                     = !isDebug;
        }
    }
}

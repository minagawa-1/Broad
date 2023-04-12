using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//����w���_�C�A���O�N���X
public class WeaponBuyDialog : MonoBehaviour
{
    [SerializeField] Image                  m_WeaponIcon            = null; //����A�C�R��
    [SerializeField] TextMeshProUGUI        m_WeaponNameText        = null; //���햼�e�L�X�g
    [SerializeField] TextMeshProUGUI        m_WeaponPriceText       = null; //���퉿�i�e�L�X�g
    [SerializeField] NotAllowedBuyDialog    m_NotAllowedBuyDaialog  = null; //����w���s�_�C�A���O
    [SerializeField] WeaponInfo             m_WeaponInfoUI          = null; //������UI
    [Space(12)]
    [SerializeField] WeaponData             m_WeaponData            = null; //����f�[�^
    int                                     m_Price                 = 0;    //���i
    int                                     m_WeaponID              = 0;    //����ID

    /// <summary>�_�C�A���O�\��</summary>
    /// <param name="weapon_id">����ID</param>
    public void ShowDialog(int weapon_id)
    {
        //�����F������
        m_WeaponPriceText.color = Color.white;

        //����f�[�^���畐�탊�X�g�擾
        List<WeaponData.Weapon> weapon_list = m_WeaponData.GetWeaponList();

        //ID�Ɖ��i�ۑ�
        m_WeaponID  = weapon_id;
        m_Price     = weapon_list[m_WeaponID].weaponPrice;
        //�_�C�A���O�ɕ\��������̎擾
        m_WeaponIcon.sprite     = weapon_list[m_WeaponID].weaponImage;
        m_WeaponNameText.text   = weapon_list[m_WeaponID].weaponName;
        m_WeaponPriceText.text  = "��" + m_Price.ToString();

        //������������Ȃ������當���F��Ԃɂ���
        if (SaveSystem.m_SaveData.money < m_Price)
            m_WeaponPriceText.color = Color.red;
    }

    /// <summary>�w������</summary>
    public void Yes()
    {
        //������������Ȃ�������I��
        if (SaveSystem.m_SaveData.money < m_Price)
        {
            m_NotAllowedBuyDaialog.ShowDialog();
            gameObject.SetActive(false);
            return;
        }

        //�����t���O���グ�ď��������猸�Z����
        SaveSystem.m_SaveData.hasWeapons[m_WeaponID] = true;
        SaveSystem.m_SaveData.money -= m_Price;

        //����𑕔�
        SaveSystem.m_SaveData.eqipWeaponID = m_WeaponID;
        TemporarySavingBounty.equipWeapon  = m_WeaponData.GetWeaponAt(m_WeaponID);

        //UI�ɏ��𔽉f������
        m_WeaponInfoUI.SetWeaponIndex(m_WeaponID);

        //�f�[�^��ۑ�
        SaveSystem.Save();

        //�_�C�A���O�����
        gameObject.SetActive(false);
    }

    /// <summary>�w�����Ȃ�</summary>
    public void No()
    {
        //�_�C�A���O�����
        gameObject.SetActive(false);
    }
}

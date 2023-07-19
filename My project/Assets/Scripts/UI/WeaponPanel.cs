using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//����p�l���N���X
public class WeaponPanel : MonoBehaviour
{
    [SerializeField] Image      m_IconImage             = null;         //�A�C�R���摜
    [SerializeField] Image      m_EquipImage            = null;         //�������摜
    [SerializeField] GameObject m_LockedObject          = null;         //���b�N�I�u�W�F�N�g
    WeaponScrollView            m_WeaponScrollView      = null;         //����X�N���[���r���[
    Vector3                     m_FirstPointerPosition  = Vector3.zero; //�������̃|�C���^�[�ʒu
    public int                  m_WeaponID      { get; private set; }   //����ID

    /// <summary>������</summary>
    /// <param name="id">����ID</param>
    /// <param name="icon">����A�C�R��</param>
    /// <param name="scrollView">����X�N���[���r���[�R���|�[�l���g</param>
    public void Setup(int id, Sprite icon, WeaponScrollView scrollView)
    {
        m_WeaponID = id;
        m_IconImage.sprite = icon;
        m_WeaponScrollView = scrollView;
        m_EquipImage.enabled = false;

        //�Z�[�u�f�[�^�̏������X�g���烍�b�N�I�u�W�F�N�g�̏�Ԃ����߂�
        m_LockedObject.SetActive(!SaveSystem.m_SaveData.hasWeapons[m_WeaponID]);

#if true
        if (m_WeaponScrollView.m_WeaponData.GetWeaponAt(id).presetIndex.Count <= 0)
        {
            m_LockedObject.GetComponent<Image>().color = Color.yellow;
        }
#endif
    }

    private void Update()
    {
        //�Z�[�u�f�[�^�̏������X�g���烍�b�N�I�u�W�F�N�g�̏�Ԃ����߂�
        m_LockedObject.SetActive(!SaveSystem.m_SaveData.hasWeapons[m_WeaponID]);

        //�������摜�̕\��
        m_EquipImage.enabled = SaveSystem.m_SaveData.eqipWeaponID == m_WeaponID;
    }

    /// <summary>�|�C���^�[�������ꂽ</summary>
    public void PointerDown()
    {
        m_WeaponScrollView.PointerDown();

        m_FirstPointerPosition = woskni.InputManager.GetInputPosition(Application.platform);
    }

    /// <summary>�|�C���^�[���h���b�O����Ă���</summary>
    public void PointerDrag()
    {
        m_WeaponScrollView.PointerDrag();
    }

    /// <summary>�|�C���^�[�����ꂽ</summary>
    public void PointerUp()
    {
        Vector3 pointyer_pos = woskni.InputManager.GetInputPosition(Application.platform);

        //�|�C���^�[�̈ʒu�����������炻���܂ŗ���Ă��Ȃ�������p�l�����N���b�N���ꂽ����ɂ���
        if (Mathf.Abs(pointyer_pos.x - m_FirstPointerPosition.x) < 10.0f)
            m_WeaponScrollView.PanelClick(m_WeaponID);
        else
            m_WeaponScrollView.PointerUp();
    }
}

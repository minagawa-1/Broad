using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//������N���X
public class WeaponInfo : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI    m_WeaponName        = null; //���햼
    [SerializeField] Text               m_WeaponDescription = null; //�������
    [SerializeField] Text               m_WeaponPrice       = null; //������z
    [SerializeField] Text               m_YenMark           = null; //�~�}�[�N
    [SerializeField] GameObject         m_TagPrefab         = null; //�^�O�̃v���n�u
    [SerializeField] Transform          m_WeaponTagsParent  = null; //����^�O�̐e
    [SerializeField] float              m_TagInterval       = 10;   //�^�O�̔z�u�Ԋu
    [Space(24)]
    [SerializeField] WeaponData         m_WeaponData        = null; //����f�[�^
    List<float>                         m_TagWidthList      = new List<float>();

    /// <summary>����ԍ��ݒ�</summary>
    /// <param name="index">����ԍ�</param>
    public void SetWeaponIndex(int index)
    {
        //����擾
        WeaponData.Weapon weapon = m_WeaponData.GetWeaponList()[index];

        //�擾�������킩�疼�O�Ɛ����Ƌ��z���擾
        m_WeaponName.text           = weapon.weaponName;
        m_WeaponDescription.text    = weapon.description;
        //���z�͂��łɏ������Ă������킾������u�|�|�|�v�ɂ���
        string price = weapon.weaponPrice.ToString();
        m_YenMark.enabled = true;
        if (SaveSystem.m_SaveData.hasWeapons[index])
        {
            price = "-----";
            m_YenMark.enabled = false;
        }
        m_WeaponPrice.text = price;

        //�����̃^�O��S�ď����ă��X�g���폜����
        foreach(Transform child in m_WeaponTagsParent)
            Destroy(child.gameObject);
        m_TagWidthList.Clear();

        //����ɕR�Â��^�O�̐������J��Ԃ�
        foreach(string tagText in weapon.tagList)
        {
            //�^�O����
            WeaponTag tag = Instantiate(m_TagPrefab, m_WeaponTagsParent).GetComponent<WeaponTag>();

            //�^�O�̏�����
            tag.Setup(tagText);

            //�^�O�𐮗񂳂���
            AlignmentTags(tag);
        }
    }

    /// <summary>�^�O����</summary>
    /// <param name="tag">���ׂ����^�O</param>
    void AlignmentTags(WeaponTag tag)
    {
        //�^�O���X�g�̒��g�����݂���
        if(m_TagWidthList.Count > 0)
        {
            //����
            float additive = 0.0f;

            //����Ԋu�ƃ^�O�̒����𑫂�
            foreach (float width in m_TagWidthList)
                additive += m_TagInterval + width;

            //���ׂ����^�O��X�ʒu�����炷
            tag.rectTransform.localPosition += new Vector3(additive, 0f, 0f);
        }

        //�Ō�ɐ���Ԋu�̕��������炷
        tag.rectTransform.localPosition += new Vector3(m_TagInterval, 0f, 0f);

        //���X�g�Ƀ^�O�̒�����ǉ�����
        m_TagWidthList.Add(tag.rectTransform.sizeDelta.x);
    }
}

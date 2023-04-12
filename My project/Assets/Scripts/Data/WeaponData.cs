using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable/Create WeaponData")]
public class WeaponData : ScriptableObject
{
    //����\����
    [System.Serializable]
    public struct Weapon
    {
        [Header("�\�����")]
        public string       weaponName;     //���햼
        public Sprite       weaponImage;    //����摜
        public int          weaponPrice;    //���퉿�i
        public List<string> tagList;        //�^�O���X�g
        public List<int>    presetIndex;     //�v���Z�b�g�ԍ�


        [Multiline(2)]
        public string       description;    //�����e�L�X�g

        [Header("��\�����")]
        public string       bulletyType;    //�e�^�C�v
        public float        bulletSpeed;    //�e��
    }

    [woskni.EnumIndex(typeof(WeaponName))]
    [SerializeField] List<Weapon>                   weaponList;         //���탊�X�g

    /// <summary>���탊�X�g�擾</summary>
    /// <returns>���탊�X�g</returns>
    public List<Weapon> GetWeaponList() { return weaponList; }

    /// <summary>�ԍ����w�肵�ĕ�������擾</summary>
    /// <param name="index">�v�f�ԍ�</param>
    /// <returns>�w�蕐����</returns>
    public Weapon GetWeaponAt(int index) { return weaponList[index]; }
}

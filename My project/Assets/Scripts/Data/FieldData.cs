using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable/Create FieldData")]
public class FieldData : ScriptableObject
{
    //�t�B�[���h���\����
    [System.Serializable]
    public struct Field
    {
        public string       name;               //�t�B�[���h��
        public string       BGM;                //�t�B�[���hBGM��
        public Sprite       sprite;             //�t�B�[���h�摜
        public Texture      normal;             //�t�B�[���h�@���}�b�v

        [Range(0f, 1f)]
        public float        metallic;           //���^���b�N�l

        [Range(0f, 1f)]
        public float        smoothness;         //�X���[�X�l�X�l
        public float        normalIntensity;    //�@���}�b�v�l

        public List<int>    gimmickIndex;       //�M�~�b�N�ԍ�
    }

    [woskni.EnumIndex(typeof(FieldName))]
    [SerializeField] List<Field> fieldList; //�t�B�[���h���X�g

    /// <summary>�t�B�[���h���X�g�擾</summary>
    /// <returns>�t�B�[���h���X�g</returns>
    public List<Field> GetFieldList() { return fieldList; }

    /// <summary>�ԍ����w�肵�ăt�B�[���h�����擾</summary>
    /// <param name="index">�v�f�ԍ�</param>
    /// <returns>�w��t�B�[���h���</returns>
    public Field GetFieldAt(int index) { return fieldList[index]; }
}

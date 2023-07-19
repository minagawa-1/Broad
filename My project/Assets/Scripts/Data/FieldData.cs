using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable/Create FieldData")]
public class FieldData : ScriptableObject
{
    //フィールド情報構造体
    [System.Serializable]
    public struct Field
    {
        public string       name;               //フィールド名
        public string       BGM;                //フィールドBGM名
        public Sprite       sprite;             //フィールド画像
        public Texture      normal;             //フィールド法線マップ

        [Range(0f, 1f)]
        public float        metallic;           //メタリック値

        [Range(0f, 1f)]
        public float        smoothness;         //スムースネス値
        public float        normalIntensity;    //法線マップ値

        public List<int>    gimmickIndex;       //ギミック番号
    }

    [woskni.EnumIndex(typeof(FieldName))]
    [SerializeField] List<Field> fieldList; //フィールドリスト

    /// <summary>フィールドリスト取得</summary>
    /// <returns>フィールドリスト</returns>
    public List<Field> GetFieldList() { return fieldList; }

    /// <summary>番号を指定してフィールド情報を取得</summary>
    /// <param name="index">要素番号</param>
    /// <returns>指定フィールド情報</returns>
    public Field GetFieldAt(int index) { return fieldList[index]; }
}

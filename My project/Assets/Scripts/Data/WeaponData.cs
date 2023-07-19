using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable/Create WeaponData")]
public class WeaponData : ScriptableObject
{
    //武器構造体
    [System.Serializable]
    public struct Weapon
    {
        [Header("表示情報")]
        public string       weaponName;     //武器名
        public Sprite       weaponImage;    //武器画像
        public int          weaponPrice;    //武器価格
        public List<string> tagList;        //タグリスト
        public List<int>    presetIndex;     //プリセット番号


        [Multiline(2)]
        public string       description;    //説明テキスト

        [Header("非表示情報")]
        public string       bulletyType;    //弾タイプ
        public float        bulletSpeed;    //弾速
    }

    [woskni.EnumIndex(typeof(WeaponName))]
    [SerializeField] List<Weapon>                   weaponList;         //武器リスト

    /// <summary>武器リスト取得</summary>
    /// <returns>武器リスト</returns>
    public List<Weapon> GetWeaponList() { return weaponList; }

    /// <summary>番号を指定して武器情報を取得</summary>
    /// <param name="index">要素番号</param>
    /// <returns>指定武器情報</returns>
    public Weapon GetWeaponAt(int index) { return weaponList[index]; }
}

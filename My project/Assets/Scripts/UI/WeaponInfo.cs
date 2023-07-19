using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//武器情報クラス
public class WeaponInfo : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI    m_WeaponName        = null; //武器名
    [SerializeField] Text               m_WeaponDescription = null; //武器説明
    [SerializeField] Text               m_WeaponPrice       = null; //武器金額
    [SerializeField] Text               m_YenMark           = null; //円マーク
    [SerializeField] GameObject         m_TagPrefab         = null; //タグのプレハブ
    [SerializeField] Transform          m_WeaponTagsParent  = null; //武器タグの親
    [SerializeField] float              m_TagInterval       = 10;   //タグの配置間隔
    [Space(24)]
    [SerializeField] WeaponData         m_WeaponData        = null; //武器データ
    List<float>                         m_TagWidthList      = new List<float>();

    /// <summary>武器番号設定</summary>
    /// <param name="index">武器番号</param>
    public void SetWeaponIndex(int index)
    {
        //武器取得
        WeaponData.Weapon weapon = m_WeaponData.GetWeaponList()[index];

        //取得した武器から名前と説明と金額を取得
        m_WeaponName.text           = weapon.weaponName;
        m_WeaponDescription.text    = weapon.description;
        //金額はすでに所持していた武器だったら「－－－」にする
        string price = weapon.weaponPrice.ToString();
        m_YenMark.enabled = true;
        if (SaveSystem.m_SaveData.hasWeapons[index])
        {
            price = "-----";
            m_YenMark.enabled = false;
        }
        m_WeaponPrice.text = price;

        //既存のタグを全て消してリストも削除する
        foreach(Transform child in m_WeaponTagsParent)
            Destroy(child.gameObject);
        m_TagWidthList.Clear();

        //武器に紐づくタグの数だけ繰り返す
        foreach(string tagText in weapon.tagList)
        {
            //タグ生成
            WeaponTag tag = Instantiate(m_TagPrefab, m_WeaponTagsParent).GetComponent<WeaponTag>();

            //タグの初期化
            tag.Setup(tagText);

            //タグを整列させる
            AlignmentTags(tag);
        }
    }

    /// <summary>タグ整列</summary>
    /// <param name="tag">並べたいタグ</param>
    void AlignmentTags(WeaponTag tag)
    {
        //タグリストの中身が存在する
        if(m_TagWidthList.Count > 0)
        {
            //増分
            float additive = 0.0f;

            //整列間隔とタグの長さを足す
            foreach (float width in m_TagWidthList)
                additive += m_TagInterval + width;

            //並べたいタグのX位置をずらす
            tag.rectTransform.localPosition += new Vector3(additive, 0f, 0f);
        }

        //最後に整列間隔の分だけずらす
        tag.rectTransform.localPosition += new Vector3(m_TagInterval, 0f, 0f);

        //リストにタグの長さを追加する
        m_TagWidthList.Add(tag.rectTransform.sizeDelta.x);
    }
}

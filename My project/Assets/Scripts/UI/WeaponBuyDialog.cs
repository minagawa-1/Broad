using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//武器購入ダイアログクラス
public class WeaponBuyDialog : MonoBehaviour
{
    [SerializeField] Image                  m_WeaponIcon            = null; //武器アイコン
    [SerializeField] TextMeshProUGUI        m_WeaponNameText        = null; //武器名テキスト
    [SerializeField] TextMeshProUGUI        m_WeaponPriceText       = null; //武器価格テキスト
    [SerializeField] NotAllowedBuyDialog    m_NotAllowedBuyDaialog  = null; //武器購入不可ダイアログ
    [SerializeField] WeaponInfo             m_WeaponInfoUI          = null; //武器情報UI
    [Space(12)]
    [SerializeField] WeaponData             m_WeaponData            = null; //武器データ
    int                                     m_Price                 = 0;    //価格
    int                                     m_WeaponID              = 0;    //武器ID

    /// <summary>ダイアログ表示</summary>
    /// <param name="weapon_id">武器ID</param>
    public void ShowDialog(int weapon_id)
    {
        //文字色初期化
        m_WeaponPriceText.color = Color.white;

        //武器データから武器リスト取得
        List<WeaponData.Weapon> weapon_list = m_WeaponData.GetWeaponList();

        //IDと価格保存
        m_WeaponID  = weapon_id;
        m_Price     = weapon_list[m_WeaponID].weaponPrice;
        //ダイアログに表示する情報の取得
        m_WeaponIcon.sprite     = weapon_list[m_WeaponID].weaponImage;
        m_WeaponNameText.text   = weapon_list[m_WeaponID].weaponName;
        m_WeaponPriceText.text  = "￥" + m_Price.ToString();

        //所持金が足りなかったら文字色を赤にする
        if (SaveSystem.m_SaveData.money < m_Price)
            m_WeaponPriceText.color = Color.red;
    }

    /// <summary>購入する</summary>
    public void Yes()
    {
        //所持金が足りなかったら終了
        if (SaveSystem.m_SaveData.money < m_Price)
        {
            m_NotAllowedBuyDaialog.ShowDialog();
            gameObject.SetActive(false);
            return;
        }

        //所持フラグを上げて所持金から減算する
        SaveSystem.m_SaveData.hasWeapons[m_WeaponID] = true;
        SaveSystem.m_SaveData.money -= m_Price;

        //武器を装備
        SaveSystem.m_SaveData.eqipWeaponID = m_WeaponID;
        TemporarySavingBounty.equipWeapon  = m_WeaponData.GetWeaponAt(m_WeaponID);

        //UIに情報を反映させる
        m_WeaponInfoUI.SetWeaponIndex(m_WeaponID);

        //データを保存
        SaveSystem.Save();

        //ダイアログを閉じる
        gameObject.SetActive(false);
    }

    /// <summary>購入しない</summary>
    public void No()
    {
        //ダイアログを閉じる
        gameObject.SetActive(false);
    }
}

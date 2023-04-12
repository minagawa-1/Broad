using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//武器パネルクラス
public class WeaponPanel : MonoBehaviour
{
    [SerializeField] Image      m_IconImage             = null;         //アイコン画像
    [SerializeField] Image      m_EquipImage            = null;         //装備中画像
    [SerializeField] GameObject m_LockedObject          = null;         //ロックオブジェクト
    WeaponScrollView            m_WeaponScrollView      = null;         //武器スクロールビュー
    Vector3                     m_FirstPointerPosition  = Vector3.zero; //押下時のポインター位置
    public int                  m_WeaponID      { get; private set; }   //武器ID

    /// <summary>初期化</summary>
    /// <param name="id">武器ID</param>
    /// <param name="icon">武器アイコン</param>
    /// <param name="scrollView">武器スクロールビューコンポーネント</param>
    public void Setup(int id, Sprite icon, WeaponScrollView scrollView)
    {
        m_WeaponID = id;
        m_IconImage.sprite = icon;
        m_WeaponScrollView = scrollView;
        m_EquipImage.enabled = false;

        //セーブデータの所持リストからロックオブジェクトの状態を決める
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
        //セーブデータの所持リストからロックオブジェクトの状態を決める
        m_LockedObject.SetActive(!SaveSystem.m_SaveData.hasWeapons[m_WeaponID]);

        //装備中画像の表示
        m_EquipImage.enabled = SaveSystem.m_SaveData.eqipWeaponID == m_WeaponID;
    }

    /// <summary>ポインターが押された</summary>
    public void PointerDown()
    {
        m_WeaponScrollView.PointerDown();

        m_FirstPointerPosition = woskni.InputManager.GetInputPosition(Application.platform);
    }

    /// <summary>ポインターがドラッグされている</summary>
    public void PointerDrag()
    {
        m_WeaponScrollView.PointerDrag();
    }

    /// <summary>ポインターが離れた</summary>
    public void PointerUp()
    {
        Vector3 pointyer_pos = woskni.InputManager.GetInputPosition(Application.platform);

        //ポインターの位置が押下時からそこまで離れていなかったらパネルがクリックされた判定にする
        if (Mathf.Abs(pointyer_pos.x - m_FirstPointerPosition.x) < 10.0f)
            m_WeaponScrollView.PanelClick(m_WeaponID);
        else
            m_WeaponScrollView.PointerUp();
    }
}

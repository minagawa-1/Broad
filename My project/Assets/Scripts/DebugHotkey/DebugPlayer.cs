using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugPlayer : MonoBehaviour
{
    public static bool isDebug = false;

    //プレイヤークラスを取得
    [SerializeField] Player m_Player;
    [SerializeField] Text   m_DebugText;
    [SerializeField] Enemy  m_Enemy;

    [Header("鑑賞モードになるときに非表示になるやつら")]
    [SerializeField] Image          m_EnemyHPRing       = null; //敵のHPリング
    [SerializeField] Image          m_EnemyHPRingShadow = null; //敵のHPリングの影
    [SerializeField] Image          m_EnemyPosition     = null; //敵位置
    [SerializeField] Image          m_PlayerPosition    = null; //プレイヤー位置
    [SerializeField] SpriteRenderer m_PlayerImage       = null; //プレイヤー画像
    [SerializeField] SpriteRenderer m_WandImage         = null; //ワンド画像
    [SerializeField] SpriteRenderer m_PlayerCoreImage   = null; //プレイヤーの当たり判定

    void Update()
    {
        if (woskni.KeyBoard.GetAndKeyDown(KeyCode.Alpha1))
        {
            m_Player.invincibleFlag = !m_Player.invincibleFlag;

            //m_DebugText.enabled = m_Player.invincibleFlag;
        }
        if (woskni.KeyBoard.GetAndKeyDown(KeyCode.Alpha2))
        {
            m_Enemy.Damage(10000000, 0f);
        }
        if (woskni.KeyBoard.GetAndKeyDown(KeyCode.Alpha3))
        {
            bool flag = m_EnemyHPRing.enabled;

            m_EnemyHPRing.enabled       = !flag;
            m_EnemyHPRingShadow.enabled = !flag;
            m_EnemyPosition.enabled     = !flag;
            m_PlayerPosition.enabled    = !flag;
            m_PlayerImage.enabled       = !flag;
            m_WandImage.enabled         = !flag;
            m_PlayerCoreImage.enabled   = !flag;
            m_Player.invincibleFlag     = !m_Player.invincibleFlag;
            isDebug                     = !isDebug;
        }
    }
}

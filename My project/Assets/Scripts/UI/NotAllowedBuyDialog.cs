using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//武器購入不可ダイアログクラス
public class NotAllowedBuyDialog : MonoBehaviour
{
    [SerializeField] CanvasGroup    m_CanvasGroup   = null;
    [SerializeField] float          m_VanishTime    = 1.0f;
    woskni.Timer m_VanishTimer;

    private void Start()
    {
        //最初は非表示にする
        m_CanvasGroup.alpha = 0f;

        //タイマーセット
        m_VanishTimer.Setup(m_VanishTime);

        //タイマーを強制終了
        m_VanishTimer.Fin();
    }

    private void Update()
    {
        //タイマーが終了していたらここで終了
        if (m_VanishTimer.IsFinished()) return;

        //タイマー更新
        m_VanishTimer.Update();

        //滑らかに透明にする
        m_CanvasGroup.alpha = woskni.Easing.InQuintic(m_VanishTimer.time, m_VanishTimer.limit, 1f, 0f);

        //タイマーが終了したら誤差を補正
        if (m_VanishTimer.IsFinished()) m_CanvasGroup.alpha = 0f;

    }

    /// <summary>ダイアログ表示</summary>
    public void ShowDialog()
    {
        //タイマーリセット
        m_VanishTimer.Reset();

        //透明度リセット
        m_CanvasGroup.alpha = 1f;
    }
}

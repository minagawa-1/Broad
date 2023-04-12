using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//フィールド選択クラス
public class FieldSelect : MonoBehaviour
{
    //選択状態
    enum SelectState
    {
        Bounty,
        Difficulty,
    }

    [SerializeField] DifficultyFire m_DifficultyFire            = null;                 //難易度エフェクト
    [SerializeField] Title          m_Title                     = null;                 //タイトルスクリプト
    [SerializeField] Transform      m_Difficulty                = null;                 //難易度パネル
    [SerializeField] Transform      m_BountySelect              = null;                 //バウンティ選択パネル
    [SerializeField] float          m_MoveTime                  = 0.25f;                //移動時間
    SelectState                     m_SelectState               = SelectState.Bounty;   //画面状態
    Vector3                         m_FirstDifficultyPosition   = Vector3.zero;         //難易度パネルの初期位置
    Vector3                         m_FirstBountySelectPosition = Vector3.zero;         //バウンティ選択パネルの初期位置
    bool                            m_ChangeFlag                = false;                //画面変更フラグ
    woskni.Timer                    m_ChangeTimer;                                      //画面変更タイマー
    float                           m_HomePositionX             = 0.0f;                 //イージング開始位置X
    float                           m_DestinationPositionX      = 0.0f;                 //イージング終了位置X

    void Start()
    {
        //初期位置保存
        m_FirstDifficultyPosition   = m_Difficulty.localPosition;
        m_FirstBountySelectPosition = m_BountySelect.localPosition;

        //タイマーセットアップ
        m_ChangeTimer.Setup(m_MoveTime);
    }

    void Update()
    {
        m_DifficultyFire.m_IsShake = m_SelectState == SelectState.Difficulty;

        //変更しないなら何もしない
        if (!m_ChangeFlag) return;

        //タイマー更新
        m_ChangeTimer.Update();

        //変数用意
        Vector3 pos = m_Difficulty.localPosition;
        float time  = m_ChangeTimer.time;
        float limit = m_ChangeTimer.limit;

        //イージング
        pos.x = woskni.Easing.OutSine(time, limit, m_HomePositionX, m_DestinationPositionX);
        m_Difficulty.localPosition = pos;

        //タイマー終了でフラグを下げる
        if(m_ChangeTimer.IsFinished())
            m_ChangeFlag = false;
    }

    /// <summary>難易度の表示変更</summary>
    /// <param name="selectState">選択状態</param>
    void ChangeDifficulty(SelectState selectState)
    {
        m_SelectState = selectState;

        //フラグを上げる
        m_ChangeFlag = true;

        //タイマーリセット
        m_ChangeTimer.Reset();

        //初期値・目的値保存
        switch (m_SelectState)
        {
            case SelectState.Bounty:
                m_HomePositionX         = m_FirstBountySelectPosition.x;
                m_DestinationPositionX  = m_FirstDifficultyPosition.x;
                break;
            case SelectState.Difficulty:
                m_HomePositionX         = m_FirstDifficultyPosition.x;
                m_DestinationPositionX  = m_FirstBountySelectPosition.x;
                break;
        }
    }

    /// <summary>難易度表示</summary>
    public void ShowDifficulty()
    {
        //表示変更
        ChangeDifficulty(SelectState.Difficulty);

        //スライダーの位置リセット
        m_Difficulty.GetComponent<Difficulty>().ResetSlider();
    }

    /// <summary>難易度非表示</summary>
    public void HideDifficulty()
    {
        //表示変更
        ChangeDifficulty(SelectState.Bounty);

        //スライダーの位置保存
        SaveSystem.m_SaveData.difficulty = m_Difficulty.GetComponent<Difficulty>().GetDifficulty();
        SaveSystem.Save();
    }

    /// <summary>戻るボタン押下</summary>
    public void PressReturnButton()
    {
        //状態を見てどこに戻すか決める
        switch (m_SelectState)
        {
            case SelectState.Bounty:        m_Title.UndoWindowState();  break;
            case SelectState.Difficulty:    HideDifficulty();           break;
        }
    }

    /// <summary>シーン変更</summary>
    public void ChangeScene()
    {
        //難易度設定
        Difficulty difficulty = m_Difficulty.GetComponent<Difficulty>();
        TemporarySavingBounty.difficulty = difficulty.GetDifficulty();

        //フラグリセット
        int bounty_id = TemporarySavingBounty.bountyIndex;
        SaveSystem.m_SaveData.bounties[bounty_id].completionFlag = false;

        //データ保存
        SaveSystem.m_SaveData.difficulty = difficulty.GetDifficulty();
        SaveSystem.Save();

        woskni.SoundManager.ChangeVolume("日光浴とバウンティ", 0f, 1f);
        woskni.SoundManager.ChangeVolume("月光浴とバウンティ", 0f, 1f);

        woskni.Scene.Change(woskni.Scenes.GameMain);
    }
}

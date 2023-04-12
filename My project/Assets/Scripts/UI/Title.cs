using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//タイトル画面クラス
public class Title : MonoBehaviour
{
    //画面状態
    [System.Serializable]
    public enum WINDOW_STATE
    {
        Title,          //タイトル
        FieldSelect,    //ステージ選択
        Settings,       //設定
    }

    [Header("コンポーネント")]
    [SerializeField] Transform  m_Title         = null; // タイトル
    [SerializeField] Transform  m_FieldSelect   = null; // ステージ選択
    [SerializeField] Transform  m_Settings      = null; // 設定

    [Header("移動時間")]
    [SerializeField] float      m_Movetime      = 1f;   // 移動時間
    [woskni.Name("データ削除までの時間"), SerializeField] float m_Deletetime = 180f;

    [Header("曲リスト")]
    [SerializeField] string[]   m_Musics        = null; //楽曲リスト

    Transform       myTransform                 = null;                 // 自身のトランスフォーム
    Vector3         m_FirstTitlePosition        = Vector3.zero;         // タイトル初期位置
    Vector3         m_FirstFieldSelectPosition  = Vector3.zero;         // ステージ選択初期位置
    Vector3         m_FirstSettingsPosition     = Vector3.zero;         // 設定初期位置
    Vector3         m_StartPosition             = Vector3.zero;         // イージング開始位置
    Vector3         m_EndPosition               = Vector3.zero;         // イージング終了位置
    WINDOW_STATE    m_WindowState               = WINDOW_STATE.Title;   // 画面状態
    WINDOW_STATE    m_BeforeWindowState         = WINDOW_STATE.Title;   // 画面状態(一個前)
    woskni.Timer    m_MoveTimer;                                        // 移動タイマー
    woskni.Timer    m_DeleteTimer;                                      // データ削除タイマー
    int             m_CurrentMusicID            = 0;                    //現在のBGMの番号

    void Start()
    {
        woskni.SoundManager.StopAll();
        PlayTitleBGM(System.DateTime.Now);

        //トランスフォーム保存
        myTransform = transform;

        //初期位置保存
        m_FirstTitlePosition         = m_Title.localPosition;
        m_FirstFieldSelectPosition   = -m_FieldSelect.localPosition;
        m_FirstSettingsPosition      = -m_Settings.localPosition;

        //タイマーセットアップ
        m_MoveTimer.Setup(m_Movetime);
        m_DeleteTimer.Setup(m_Deletetime);

        //前のシーンにより画面状態を決める
        if((woskni.Scene.m_LastScene == woskni.Scenes.GameMain ||
            woskni.Scene.m_LastScene == woskni.Scenes.Result)  &&
           !SaveSystem.m_SaveData.AFKflag)
        {
            ChangeWindowState_FieldSelect();
            m_MoveTimer.Fin();
        }
    }

    void Update()
    {
        //タイマー稼働中なら
        if (!m_MoveTimer.IsFinished())
        {
            //タイマー更新
            m_MoveTimer.Update();

            //イージングで移動
            float limit = m_MoveTimer.limit;
            float time = m_MoveTimer.time;
            Vector3 pos = myTransform.localPosition;
            pos.x = woskni.Easing.OutSine(time, limit, m_StartPosition.x, m_EndPosition.x);
            pos.y = woskni.Easing.OutSine(time, limit, m_StartPosition.y, m_EndPosition.y);
            myTransform.localPosition = pos;
        }
        //タイマー終了なら
        else
            //誤差補正
            myTransform.localPosition = m_EndPosition;

        //タイトル画面 かつ タイマー終了 かつ 入力なし
        if (m_WindowState == WINDOW_STATE.Title)
        {
            m_DeleteTimer.Update();
            if (m_DeleteTimer.IsFinished())
            {
                SaveSystem.TrialReset();
                m_DeleteTimer.Reset();
            }
            else if (woskni.InputManager.IsButton())
                m_DeleteTimer.Reset();
        }

#if true
        if (Input.GetKeyDown(KeyCode.H))
        {
            Debug.Log("バウンティ更新");
            GameObject.Find("BountyScrollView").GetComponent<BountyScrollView>().BountiesReset();
            if (m_Musics.Length > 0)
            {
                int num = Random.Range(0, m_Musics.Length);
                while (m_CurrentMusicID == num)
                    num = Random.Range(0, m_Musics.Length);
                CrossFade(m_Musics[num], 1.5f, false);
                m_CurrentMusicID = num;
            }
        }
#else
        //指定時間を過ぎたら更新処理
        if (SaveSystem.IsUpdatedDay(System.DateTime.Now,SaveSystem.m_SaveData.GetLastSaveTime()))
        {
            Debug.Log("バウンティ更新");
            GameObject.Find("BountyScrollView").GetComponent<BountyScrollView>().BountiesReset();
            if (m_Musics.Length > 0)
            {
                int num = Random.Range(0, m_Musics.Length);
                while (m_CurrentMusicID == num)
                    num = Random.Range(0, m_Musics.Length);
                CrossFade(m_Musics[num]);
                m_CurrentMusicID = num;
            }
        }
#endif
    }

    void CrossFade(string nextMusicName, float fadeTime, bool isBeginning)
    {
        woskni.SoundManager.SoundPlayer currentMusic = woskni.SoundManager.FindSoundPlayer(m_Musics[m_CurrentMusicID]);
        float volume = currentMusic.source.volume;
        float time = 0.0f;
        if (!isBeginning) time = currentMusic.source.time;
        woskni.SoundManager.Play(nextMusicName, true, 0.01f, playTime: time);
        woskni.SoundManager.SoundPlayer nextMusic = woskni.SoundManager.FindSoundPlayer(nextMusicName);

        woskni.SoundManager.ChangeVolume(currentMusic.audio.clip, 0f, fadeTime);
        woskni.SoundManager.ChangeVolume(nextMusic.audio.clip, volume, fadeTime);
    }

    void PlayTitleBGM(System.DateTime time)
    {
        //  6時 ～ 18時 は上曲
        if (new woskni.RangeInt(6, 18 - 1).IsIn(time.Hour))
            woskni.SoundManager.Play("日光浴とバウンティ", true, SaveSystem.m_SaveData.BGMvolume);
        // 18時 ～  6時 は下曲
        else
            woskni.SoundManager.Play("月光浴とバウンティ", true, SaveSystem.m_SaveData.BGMvolume);
    }

    /// <summary>画面変更</summary>
    /// <param name="state">変更したい画面</param>
    public void ChangeWindowState(WINDOW_STATE state)
    {
        m_BeforeWindowState = m_WindowState;
        m_WindowState = state;

        m_MoveTimer.Reset();

        //ひとつ前のステートの位置を取得
        switch (m_BeforeWindowState)
        {
            case WINDOW_STATE.Title:        m_StartPosition = m_FirstTitlePosition;       break;
            case WINDOW_STATE.FieldSelect:  m_StartPosition = m_FirstFieldSelectPosition; break;
            case WINDOW_STATE.Settings:     m_StartPosition = m_FirstSettingsPosition;    break;
        }

        //今のステートの位置を取得
        switch (m_WindowState)
        {
            case WINDOW_STATE.Title:        m_EndPosition = m_FirstTitlePosition;        break;
            case WINDOW_STATE.FieldSelect:  m_EndPosition = m_FirstFieldSelectPosition;  break;
            case WINDOW_STATE.Settings:     m_EndPosition = m_FirstSettingsPosition;     break;
        }
    }

    /// <summary>画面変更(フィールド選択)</summary>
    public void ChangeWindowState_FieldSelect()
    {
        ChangeWindowState(WINDOW_STATE.FieldSelect);
    }

    /// <summary>画面変更(設定画面)</summary>
    public void ChangeWindowState_Settings()
    {
        ChangeWindowState(WINDOW_STATE.Settings);
    }

    /// <summary>画面を一つ前に戻す</summary>
    public void UndoWindowState()
    {
        ChangeWindowState(m_BeforeWindowState);
        SaveSystem.Save();
    }
}

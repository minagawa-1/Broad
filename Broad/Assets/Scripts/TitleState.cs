using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleState : MonoBehaviour
{
    [Header("コンポーネント")]
    [SerializeField] StartButtonState m_StartButtonState;
    [SerializeField] GameSetting m_GameSetting;
    [SerializeField] Text[] m_MatchingTexts;

    [Chapter("その他")]
    [Header("待機タイマー")]
    [SerializeField] woskni.Timer m_MatchTimer;

    [ReadOnly] public List<string> m_MatchPlayers = new List<string>();

    [System.Serializable]
    enum MatchState
    {
        /// <summary>マッチングしていない</summary>
        None,

        /// <summary>マッチング開始直後</summary>
        StartMatch,

        /// <summary>マッチング中</summary>
        Matching,

        /// <summary>マッチング中止直後</summary>
        EndMatch,

        /// <summary>マッチング完了</summary>
        Matched,
    }
    MatchState m_MatchState;

    // Start is called before the first frame update
    void Start()
    {
        m_MatchPlayers = new List<string>();
        m_MatchPlayers.Add("You");

        m_GameSetting.playerNum = 1;

        m_MatchState = MatchState.None;
    }

    // Update is called once per frame
    void Update()
    {
        switch (m_MatchState)
        {
            case MatchState.None:       None();         break;
            case MatchState.StartMatch: StartMatch();   break;
            case MatchState.Matching:   Matching();     break;
            case MatchState.EndMatch:   EndMatch();     break;
            case MatchState.Matched:    Matched();      break;
        }
    }

    /// <summary>マッチングしていない</summary>
    void None() 
    {
    }

    /// <summary>マッチング開始直後</summary>
    void StartMatch()
    {
        m_StartButtonState.DoStartMatch();


        m_MatchState = MatchState.Matching;
    }

    /// <summary>マッチング中</summary>
    void Matching()
    {
        if (m_GameSetting.playerNum > 1) m_MatchTimer.Update();

        if (Input.GetKeyDown(KeyCode.A))
        {
            SetPlayerNum(m_GameSetting.playerNum + 1);
            m_MatchTimer.Reset();
        }

        if (m_MatchTimer.IsFinished())
        {
            SceneManager.Instance.LoadScene(Scene.GameMainScene);

            m_MatchState = MatchState.Matched;
        }
    }

    /// <summary>マッチング中止直後</summary>
    void EndMatch()
    {
        m_StartButtonState.DoEndMatch(SetPlayerNum);

        m_MatchState = MatchState.None;
    }

    /// <summary>マッチング完了</summary>
    void Matched()
    {
        
    }

    // プレイヤー人数をGameSettingに反映させる
    void SetPlayerNum(int playerNum)
    {
        m_GameSetting.playerNum = playerNum;

        foreach (Text text in m_MatchingTexts)
            text.text = $"Matching... ( {playerNum}人 )";
    }

    /// <summary>マッチング状態の切り替え</summary>
    public void ChangeState()
    {
        switch (m_MatchState)
        {
            case MatchState.None:       m_MatchState = MatchState.StartMatch;   break;
            case MatchState.StartMatch: m_MatchState = MatchState.StartMatch;   break;
            case MatchState.Matching:   m_MatchState = MatchState.EndMatch;     break;
            case MatchState.EndMatch:   m_MatchState = MatchState.EndMatch;     break;
            case MatchState.Matched:    m_MatchState = MatchState.EndMatch;     break;
        }
    }
}

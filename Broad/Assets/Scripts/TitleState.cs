using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleState : MonoBehaviour
{
    [Header("コンポーネント")]
    [SerializeField] StartButtonState m_StartButtonState;

    public List<string> m_MatchPlayers = new List<string>();

    [System.Serializable]
    enum MatchState
    {
        None,
        StartMatch,
        Matching,
        EndMatch,
        Matched,
    }
    MatchState m_MatchState;

    // Start is called before the first frame update
    void Start()
    {
        m_MatchPlayers = new List<string>();
        m_MatchPlayers.Add("You");

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

    void None() 
    {
    }

    void StartMatch()
    {
        m_StartButtonState.DoStartMatch();


        m_MatchState = MatchState.Matching;
    }

    void Matching()
    {

    }

    void EndMatch()
    {
        m_StartButtonState.DoEndMatch();

        m_MatchState = MatchState.None;
    }

    void Matched()
    {

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

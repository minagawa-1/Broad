using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleState : MonoBehaviour
{
    [Header("�R���|�[�l���g")]
    [SerializeField] StartButtonState m_StartButtonState;
    [SerializeField] GameSetting m_GameSetting;
    [SerializeField] Text[] m_MatchingTexts;

    [Chapter("���̑�")]
    [Header("�ҋ@�^�C�}�[")]
    [SerializeField] woskni.Timer m_MatchTimer;

    [ReadOnly] public List<string> m_MatchPlayers = new List<string>();

    [System.Serializable]
    enum MatchState
    {
        /// <summary>�}�b�`���O���Ă��Ȃ�</summary>
        None,

        /// <summary>�}�b�`���O�J�n����</summary>
        StartMatch,

        /// <summary>�}�b�`���O��</summary>
        Matching,

        /// <summary>�}�b�`���O���~����</summary>
        EndMatch,

        /// <summary>�}�b�`���O����</summary>
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

    /// <summary>�}�b�`���O���Ă��Ȃ�</summary>
    void None() 
    {
    }

    /// <summary>�}�b�`���O�J�n����</summary>
    void StartMatch()
    {
        m_StartButtonState.DoStartMatch();


        m_MatchState = MatchState.Matching;
    }

    /// <summary>�}�b�`���O��</summary>
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

    /// <summary>�}�b�`���O���~����</summary>
    void EndMatch()
    {
        m_StartButtonState.DoEndMatch(SetPlayerNum);

        m_MatchState = MatchState.None;
    }

    /// <summary>�}�b�`���O����</summary>
    void Matched()
    {
        
    }

    // �v���C���[�l����GameSetting�ɔ��f������
    void SetPlayerNum(int playerNum)
    {
        m_GameSetting.playerNum = playerNum;

        foreach (Text text in m_MatchingTexts)
            text.text = $"Matching... ( {playerNum}�l )";
    }

    /// <summary>�}�b�`���O��Ԃ̐؂�ւ�</summary>
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

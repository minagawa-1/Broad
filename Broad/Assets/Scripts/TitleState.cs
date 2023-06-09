using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using Mirror.Discovery;

public class TitleState : NetworkDiscovery
{
    [Chapter("��NetworkDiscovery")]
    [Header("�R���|�[�l���g")]
    [SerializeField] StartButtonState m_StartButtonState;
    [SerializeField] GameSetting m_GameSetting;
    [SerializeField] Text[] m_MatchingTexts;

    [Chapter("�ڑ����")]
    [Header("�Ԋu")]
    [SerializeField] private int m_ConnectIntervalFrame;
    [SerializeField] private woskni.Timer m_WaitTimer;

    [Header("�ڑ����s��")]
    [SerializeField] private int m_ConnectTryCount;

    [Chapter("���̑�")]
    [Header("�ҋ@�^�C�}�[")]
    [SerializeField] woskni.Timer m_MatchTimer;

    NetworkManager m_NetworkManager;        // �l�b�g���[�N�}�l�[�W���[
    ServerResponse m_DiscoverdServer;       // ���������T�[�o�[

    CancellationTokenSource m_CancelConnectServer;      // �������ɃL�����Z�������邽�߂̃\�[�X
    CancellationToken m_CancelConnectToken;       // �����L�����Z���g�[�N��

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
        CancelMatch,

        /// <summary>�}�b�`���O����</summary>
        CompleteMatch,
    }

    MatchState m_MatchState;

    void Awake()
    {
        // ConnectionData����M������ReceivedConnectData�����s����悤�ɓo�^
        NetworkClient.RegisterHandler<ConnectionData>(ReceivedConnectionData);

        m_GameSetting.playerNum = 1;

        m_MatchState = MatchState.None;

        // �R�[���o�b�N����
        // �T�[�o�[������������Ă΂��
        OnServerFound.AddListener(ServerResponse =>
        {
            // �������T�[�o�[���X�|���X������
            m_DiscoverdServer = ServerResponse;

            // Debug.Log�ŕ\��
            Debug.Log("ServerFound");
        });
    }

    // Update is called once per frame
    void Update()
    {
        switch (m_MatchState)
        {
            case MatchState.None:           None();             break;
            case MatchState.StartMatch:     StartMatch();       break;
            case MatchState.Matching:       Matching();         break;
            case MatchState.CancelMatch:    CancelMatch();      break;
            case MatchState.CompleteMatch:  CompleteMatch();    break;
        }
    }

    /// <summary>�}�b�`���O���Ă��Ȃ�</summary>
    void None() 
    {
    }

    /// <summary>�}�b�`���O�J�n��</summary>
    void StartMatch()
    {
        m_StartButtonState.DoStartMatch();

        // �ڑ��L�����Z���g�[�N������
        m_CancelConnectServer = new CancellationTokenSource();
        m_CancelConnectToken = m_CancelConnectServer.Token;

        // �T�[�o�[����
        // UniTask�̔񓯊�������Forget()��t���ČĂ�
        // �L�����Z���p�g�[�N����n�����ŁAawait������񓯊�������Cancel()�̎��s�^�C�~���O�ŏ����̒��f���\
        TryConnect(m_CancelConnectToken).Forget();

        m_MatchState = MatchState.Matching;
    }

    /// <summary>�}�b�`���O��</summary>
    void Matching()
    {
        // [���f�o�b�O�p] �����I�ɃQ�[���X�^�[�g
        if(Input.GetMouseButtonDown(1)) m_MatchState = MatchState.CompleteMatch;

        // �l�����Q�l�����̏ꍇ��return
        if (m_GameSetting.playerNum < 2) return;

        m_MatchTimer.Update(false);

        if(m_MatchTimer.IsFinished())
        {
            m_MatchTimer.Reset();

            m_MatchState = MatchState.CompleteMatch;
        }
    }

    /// <summary>�}�b�`���O���~��</summary>
    void CancelMatch()
    {
        m_StartButtonState.DoCancelMatch(SetPlayerNum);

        // �T�[�o�[�������~
        StopDiscovery();

        // �l�����Q�l�ȏ�̏ꍇ
        if (m_GameSetting.playerNum > 1)
        {
            // �z�X�g�E�N���C�A���g�̒�~
            NetworkManager.singleton.StopHost();
            NetworkManager.singleton.StopClient();
        }

        // �񓯊������̒�~
        // TokenSorce�̃L�����Z���Ɣp��������
        Cancel(m_CancelConnectServer);

        m_MatchState = MatchState.None;
    }

    /// <summary>�}�b�`���O����</summary>
    void CompleteMatch()
    {
        // 1�l�̏ꍇ�A2�`4�Ń����_���Ȑl���ɂ���
        if (GameSetting.instance.playerNum <= 1) GameSetting.instance.playerNum = Random.Range(2, 5);

        // �V�[���J�ڏ���
        Transition.Instance.LoadScene(Scene.GameMainScene, m_NetworkManager);
    }

    /// <summary>�v���C���[�l����GameSetting�ɔ��f������</summary>
    /// <param name="playerNum">�v���C���[�l��</param>
    void SetPlayerNum(int playerNum)
    {
        // �Œ�ł��P�l�ȏ�
        playerNum = Mathf.Max(1, playerNum);

        // �ݒ�ɂ͔��f������
        m_GameSetting.playerNum = playerNum;

        if (m_MatchingTexts.Length == 0 || m_NetworkManager == null) return;

        foreach (Text text in m_MatchingTexts)
            if(text != null)text.text = $"Matching... ( {playerNum}�l )";
    }

    /// <summary>�T�[�o�[����</summary>
    /// <async>�񓯊�<async>
    /// <await>�w�肵�����ԁE�����������܂ő҂�<await>
    /// [async/await] �͏�����񓯊��ɍs���̂ł͂Ȃ��A�񓯊��̏�����҂d�g��
    /// <param name="token">�L�����Z���p�g�[�N��</param>
    async UniTaskVoid TryConnect(CancellationToken token)
    {
        // NetworkManager�擾
        m_NetworkManager = NetworkManager.singleton;

        // �ڑ������
        int tryCount = 0;

        // �T�[�o�[�����J�n
        StartDiscovery();

        // Server���́AClient��active�Ȍ��葱����
        while (!m_NetworkManager.isNetworkActive)
        {
            // m_ConnectIntervalFrame�����̃t���[����x�点�Ď��s
            await UniTask.Delay(m_ConnectIntervalFrame, cancellationToken: token);

            // �T�[�o�[��������
            // URI��URL(Web��ɂ���t�@�C���̏Z��)��
            // URN(�l�b�g���[�N��̑��݂��镶���Ȃǂ̃��\�[�X����ӂɎ��ʂ��邽�߂̎��ʎq)�̑���
            if (m_DiscoverdServer.uri != null)
            {
                Debug.Log("Start Client");

                // �N���C�A���g�Ƃ��ĊJ�n
                // �擾����URI���g���ăT�[�o�[�ɐڑ�����
                m_NetworkManager.StartClient(m_DiscoverdServer.uri);

                // �T�[�o�[�������~�߂�
                StopDiscovery();
            }
            else
            {
                Debug.Log("Try Connect...");

                // �J�E���g�𑝂₷
                tryCount++;

                // �����񐔂��K��l�ɒB�����Ƃ�
                if (tryCount > m_ConnectTryCount)
                {
                    Debug.Log("Start Host");

                    // �z�X�g�Ƃ��ĊJ�n
                    m_NetworkManager.StartHost();

                    // �T�[�o�[�̐錾������
                    // ��������Ȃ��ƁA�T�[�o�[��������Ȃ�
                    AdvertiseServer();

                    // �T�[�o�[���X�|���X�����邩Debug.Log�ŕ\��
                    if (m_DiscoverdServer.uri != null) Debug.Log("ServerURI : exist");
                    else Debug.Log("ServerURI : not found");
                }
            }
        }
    }

    /// <summary>�ڑ��f�[�^��M</summary>
    /// <param name="recivedData">��M�f�[�^</param>
    void ReceivedConnectionData(ConnectionData receivedData)
    {
        // �^�C�}�[���Z�b�g
        m_MatchTimer.Reset();

        // playerNum�Ɍ��݂̃v���C���[���𔽉f
        SetPlayerNum(receivedData.playerCount);
    }

    /// <summary>�L�����Z��</summary>
    /// <param name="tokenSource">�L�����Z���g�[�N���\�[�X</param>
    void Cancel(CancellationTokenSource tokenSource)
    {
        // �������z�X�g�łȂ��Ȃ�A���^�[��
        if (!NetworkClient.activeHost) return;

        // �L�����Z��
        tokenSource.Cancel();

        // �p��
        tokenSource.Dispose();
    }

    public void ChangeMatchState()
    {
        switch (m_MatchState)
        {
            case MatchState.None:     m_MatchState = MatchState.StartMatch;  break;
            case MatchState.Matching: m_MatchState = MatchState.CancelMatch; break;

            default: break;
        }
    }
}

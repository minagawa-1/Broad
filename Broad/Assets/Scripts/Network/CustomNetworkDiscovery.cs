using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using Mirror.Discovery;

public class CustomNetworkDiscovery : NetworkDiscovery
{
    [Header("�}���`�v���C�J�n�p�{�^��")]
    [SerializeField] Button m_MultiPlayButton = null;
    [Header("�߂�p�{�^��")]
    [SerializeField] Button m_BackButton = null;
    [Header("�Q�[���J�n�{�^��")]
    [SerializeField] Button m_PlayButton = null;

    [Header("�v���C�l���m�F�p�e�L�X�g")]
    [SerializeField] Text m_PlayerCountText = null;
    [Header("�ڑ��J�n�p�e�L�X�g")]
    [SerializeField] Text m_ConnectionStateText = null;
    [Header("�v���C���[�ԍ��p�e�L�X�g")]
    [SerializeField] Text m_PlayerIndexText = null;

    [Header("�ڑ��Ԋu")]
    [SerializeField] private int m_ConnectIntervalTime;
    [Header("�ҋ@����")]
    [SerializeField] private int m_WaitTime;
    [Header("�ڑ����s��")]
    [SerializeField] private int m_ConnectTryCount;

    [Header("�Q�[�����C���V�[����")]
    [SerializeField, Scene] string m_GameMainSceneName = null;
    [Header("�����V�[���؂�ւ�����")]
    [SerializeField] private int m_AutSceneChangeTime;

    CustomNetworkManager m_CNetworkManager;   // �J�X�^���l�b�g���[�N�}�l�[�W���[
    NetworkManager m_NetworkManager;        // �l�b�g���[�N�}�l�[�W���[
    ServerResponse m_DiscoverdServer;       // ���������T�[�o�[

    CancellationTokenSource m_CancelConnectServer;      // �������ɃL�����Z�������邽�߂̃\�[�X
    CancellationToken m_CancelConnectToken;       // �����L�����Z���g�[�N��

    CancellationTokenSource m_CancelChangeScene;        // �V�[���؂�ւ��L�����Z���\�[�X
    CancellationToken m_CancelChangeSceneToken;   // �V�[���؂�ւ��L�����Z���g�[�N��

    // �N���C�A���g�̐ڑ���Ԃ�WAITING��Ԃɕ\������
    const string connection_status_client_waiting = "Waiting start...";

    // �z�X�g�̐ڑ���Ԃ�WAITING��Ԃɕ\������
    const string connection_status_host_waiting = "Waiting other player...";

    // �ڑ���Ԃ�SUCCESS��Ԃɕ\������
    const string connection_status_success = "Success!";

    bool m_isHostReady;                 // �z�X�g���������t���O
    int m_CurrentPlayerCount = 0;             // �v���C���[��

    /// <summary>�폜</summary>
    private void OnDestroy()
    {
        // �V�[���J�ڎ��ȂǂɃT�[�o�[�������~
        StopDiscovery();
    }

    private void Awake()
    {
        // HostReadyData����M������AReceivedReadyData�����s����悤�ɓo�^
        NetworkClient.RegisterHandler<HostReadyData>(ReceivedReadyData);

        // ConnectionData����M������ReceivedConnectData�����s����悤�ɓo�^
        NetworkClient.RegisterHandler<ConnectionData>(ReceivedConnectionData);

        // PlayerData����M������ReceivedPlayerData�����s����悤�ɓo�^
        NetworkClient.RegisterHandler<PlayerData>(ReceivedPlayerData);

        // NetworkManager��T���āACustomNetworkManager���擾
        m_CNetworkManager = GameObject.Find("NetworkManager").GetComponent<CustomNetworkManager>();

        // �R�[���o�b�N����
        // �T�[�o�[������������Ă΂��
        OnServerFound.AddListener(ServerResponse =>
        {
            // �������T�[�o�[������
            m_DiscoverdServer = ServerResponse;

            // Debug.Log�ŕ\��
            Debug.Log("ServerFound");
        });

        // MultiPlayButton�������ꂽ��Ă�
        m_MultiPlayButton.onClick.AddListener(() =>
        {
            Debug.Log("Search Connection");

            // �e�{�^���̕\���E��\����؂�ւ���
            m_BackButton.gameObject.SetActive(true);
            m_MultiPlayButton.gameObject.SetActive(false);

            // �e�e�L�X�g�̕\��
            m_ConnectionStateText.gameObject.SetActive(true);
            m_PlayerCountText.gameObject.SetActive(true);
            m_PlayerIndexText.gameObject.SetActive(true);

            // �L�����Z���g�[�N���p�\�[�X�擾
            m_CancelConnectServer = new CancellationTokenSource();
            // �L�����Z���p�g�[�N������
            m_CancelConnectToken = m_CancelConnectServer.Token;

            // �V�[���؂�ւ��L�����Z���g�[�N������
            m_CancelChangeScene = new CancellationTokenSource();
            m_CancelChangeSceneToken = m_CancelChangeScene.Token;

            // �T�[�o�[����
            // UniTask�̔񓯊�������Forget()��t���ČĂ�
            // �L�����Z���p�g�[�N����n�����ŁAawait������񓯊�������Cancel()�̎��s�^�C�~���O�ŏ����̒��f���\
            TryConnect(m_CancelConnectToken).Forget();
        });

        // BuckButton�������ꂽ
        m_BackButton.onClick.AddListener(() =>
        {
            Debug.Log("Cancel");

            // �T�[�o�[�������~
            StopDiscovery();

            // �z�X�g��~
            NetworkManager.singleton.StopHost();
            // �N���C�A���g��~
            NetworkManager.singleton.StopClient();

            // �񓯊������̒�~
            // TokenSorce�̃L�����Z���Ɣp��������
            Cancel(m_CancelConnectServer);

            Cancel(m_CancelChangeScene);
        });

        // PlayButton�������ꂽ
        m_PlayButton.onClick.AddListener(() =>
        {
            Debug.Log("Ready Ok");

            // �z�X�g�����f�[�^�ݒ�
            HostReadyData readyData = new HostReadyData() { isHostReady = true };
            // �S�N���C�A���g�ɑ��M
            NetworkServer.SendToAll(readyData);

            // m_PlayButton���\��
            m_PlayButton.gameObject.SetActive(false);
        });
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
            // connect_interval_time���x�点�Ď��s
            await UniTask.Delay(m_ConnectIntervalTime, cancellationToken: token);

            // �L�����Z�����ꂽ��A���^�[��
            if (token.IsCancellationRequested)
            {
                // ��O�X���[�𓊂���
                token.ThrowIfCancellationRequested();
                return;
            }
            // �T�[�o�[��������
            // URI��URL(Web��ɂ���t�@�C���̏Z��)��
            // URN(�l�b�g���[�N��̑��݂��镶���Ȃǂ̃��\�[�X����ӂɎ��ʂ��邽�߂̎��ʎq)�̑���
            if (m_DiscoverdServer.uri != null)
            {
                Debug.Log("Start Client");

                // �擾����URI���g���ăN���C�A���g�Ƃ��ĊJ�n����
                m_NetworkManager.StartClient(m_DiscoverdServer.uri);

                // �N���C�A���g�ҋ@���e�L�X�g������
                m_ConnectionStateText.text = connection_status_client_waiting;

                // �T�[�o�[�������~�߂�
                StopDiscovery();

                // �z�X�g�̏����������܂őҋ@
                await UniTask.WaitUntil(() => m_isHostReady, cancellationToken: token);

                // �L�����Z���v�����ꂽ��
                if (token.IsCancellationRequested) token.ThrowIfCancellationRequested();

                // �ڑ������p�e�L�X�g�ɐ؂�ւ�
                m_ConnectionStateText.text = connection_status_success;
            }
            else
            {
                Debug.Log("Try Connect...");

                // �J�E���g�𑝂₷
                tryCount++;

                // �J�E���g��connect_try_count�𒴂���
                if (tryCount > m_ConnectTryCount)
                {
                    Debug.Log("Start Host");

                    // �z�X�g�Ƃ��ĊJ�n
                    m_NetworkManager.StartHost();

                    // �T�[�o�[�̐錾������
                    // ��������Ȃ��ƁA�T�[�o�[��������Ȃ�
                    AdvertiseServer();

                    // �T�[�o�[���X�|���X�����邩Debug.Log�ŕ\��
                    if (m_DiscoverdServer.uri != null) Debug.Log("ServerURI : NotNull");
                    else Debug.Log("ServerURI : null");

                    // �z�X�g�ҋ@���e�L�X�g�ɐ؂�ւ�
                    m_ConnectionStateText.text = connection_status_host_waiting;

                    // �z�X�g�̏����������܂őҋ@
                    await UniTask.WaitUntil(() => m_isHostReady, cancellationToken: token);

                    // �ڑ������p�e�L�X�g�ɐ؂�ւ�
                    m_ConnectionStateText.text = connection_status_success;

                    // wait_time���ҋ@
                    await UniTask.Delay(m_WaitTime, cancellationToken: token);

                    if (token.IsCancellationRequested) token.ThrowIfCancellationRequested();

                    // �V�[���ύX
                    m_NetworkManager.ServerChangeScene(m_GameMainSceneName);
                }
            }
        }
    }

    /// <summary>�����V�[���؂�ւ�</summary>
    /// <param name="cahangeTime">�؂�ւ�����</param>
    /// <param name="token">�L�����Z���p�g�[�N��</param>
    async UniTaskVoid AutChangeScene(int cahangeTime, CancellationToken token)
    {
        // �z�X�g����Ȃ��Ȃ�A���^�[��
        if (!NetworkClient.activeHost) return;

        // ���ԂɂȂ�܂őҋ@
        await UniTask.Delay(cahangeTime, cancellationToken: token);

        // �v���C���[�l����2�l��菭�Ȃ�
        if ( m_CurrentPlayerCount < 2)
        {
            // PlayButton���active�ɂ���
            m_PlayButton.gameObject.SetActive(false);

            // �V�[���ύX���L�����Z��
            Cancel(m_CancelChangeScene);

            if (token.IsCancellationRequested)
            {
                token.ThrowIfCancellationRequested();
                return;
            }
        }

        // �V�[���ύX
        m_NetworkManager.ServerChangeScene(m_GameMainSceneName);
    }

    private void Update()
    {

    }

    /// <summary>�z�X�g�������ł������̃f�[�^����M</summary>
    /// <param name="reciveData">��M�f�[�^</param>
    void ReceivedReadyData(HostReadyData reciveData)
    {
        // �����o�������̃t���O����
        m_isHostReady = reciveData.isHostReady;
    }

    /// <summary>�v���C���[�ԍ��\��</summary>
    /// <param name="recivedData">��M�f�[�^</param>
    void ReceivedConnectionData(ConnectionData receivedData)
    {
        if (m_PlayButton == null) return;

        m_CurrentPlayerCount = receivedData.playerCount;

        // �v���C���[�l����\��
        m_PlayerCountText.text = m_CurrentPlayerCount + "/" + m_NetworkManager.maxConnections;

        // �������z�X�g���v���C���[�l����2�l�ȏ�
        if (NetworkClient.activeHost && m_CurrentPlayerCount >= 2)
        {
            // PlayButton��active�ɂ���
            m_PlayButton.gameObject.SetActive(true);

            // �����ŃV�[���ύX�J�n
            AutChangeScene(m_AutSceneChangeTime, m_CancelChangeSceneToken).Forget();
        }
    }

    /// <summary>�v���C���[�ԍ���\��</summary>
    /// <param name="receivedData">��M�f�[�^</param>
    void ReceivedPlayerData(PlayerData receivedData)
    {
        // �z�X�g�����Ȃ��Ȃ牽�����Ȃ�
        if (m_PlayButton == null) return;

        // �v���C���[�ԍ���\��
        m_PlayerIndexText.text = $"You {receivedData.index}P";
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
}

using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class CustomNetworkManager : NetworkManager
{

    [Header("�v���C���[�v���n�u���X�g")]
    [SerializeField] PlayerPrefabsList m_PlalyerPrefabsList = null;

    [Header("�}�b�`���O�V�[��")]
    [SerializeField, Scene] string m_MatchingScene = null;

    const string m_IPAddress = "localhost";         // IP�A�h���X

    public static PlayerData playerData;            // �v���C���[�f�[�^

    public int m_PlayerIndex = 0;                  // �v���C���[�ԍ�

    /// <summary>�T�[�o�[�ɃN���C�A���g���ڑ�������</summary>
    /// <param name="conn">�N���C�A���g�̐ڑ�</param>
    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        base.OnServerConnect(conn);

        // �}�b�`���O�V�[���݂̂ł̏���
        //if (m_MatchingScene.Contains(SceneManager.GetActiveScene().name))
        //{
        //    //�ڑ��ԍ�(�N���C�A���g���w�肷��Ƃ���Key�l�ɂȂ�)
        //    int connectionIndex = 0;

        //    DecisionPlayerIndex(connectionIndex);

        //    if (NetworkServer.connections[connectionIndex] != null)
        //    {
        //        // �ڑ����Ă����N���C�A���g�����
        //        NetworkConnectionToClient toClient = NetworkServer.connections[connectionIndex];

        //        playerData = new PlayerData { index = m_PlayerIndex };

        //        // �N���C�A���g���w�肵�āA�f�[�^�𑗐M
        //        toClient.Send(playerData);
        //    }

        //    // PlayerData�̑��M
        //    ConnectionData sendData = new ConnectionData { playerCount = NetworkServer.connections.Count };
        //    NetworkServer.SendToAll(sendData);

        //    connectionIndex++;
        //}
    }

    /// <summary>�T�[�o�[�V�[���؂�ւ���</summary>
    /// <param name="sceneName">�V�[����</param>
    public override void OnServerSceneChanged(string sceneName)
    {
        base.OnServerSceneChanged(sceneName);

        // �N���C�A���g�̃��[�J���v���C���[�����Ȃ��Ȃ�A�N���C�A���g����̃f�[�^�Ńv���C���[����������
        // Registerhandler<Message�N���X>(�֐�)�ŁAMessage����M������w�肵���֐������s����悤�ɓo�^����
        if (NetworkClient.localPlayer == null) NetworkServer.RegisterHandler<PlayerData>(CreatePlayer);
    }

    public override void OnClientSceneChanged()
    {
        base.OnClientSceneChanged();

        // �f�[�^�ݒ�
        playerData = new PlayerData { index = m_PlayerIndex, position = Vector3.one };

        // Server�Ƀf�[�^���M
        Send(playerData);
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        base.OnServerDisconnect(conn);

        // PlayerCount���đ��M
        ConnectionData sendData = new ConnectionData { playerCount = NetworkServer.connections.Count };
        NetworkServer.SendToAll(sendData);
    }

    /// <summary>�N���C�A���g��~</summary>
    public override void OnStopClient()
    {
        base.OnStopClient();

        // �}�b�`���O�V�[���ɖ߂�
        //SceneManager.LoadScene(m_MatchingScene);
    }

    [Server]
    /// <summary>�v���C���[�ԍ������߂�</summary>
    int DecisionPlayerIndex(int connectionIndex)
    {
        // connectionIndex���ő�ڑ����Ŋ������]���n���B
        return m_PlayerIndex = connectionIndex % NetworkServer.maxConnections;
    }

    /// <summary>�v���C���[����</summary>
    /// <param name="connection">�N���C�A���g�ւ̐ڑ�</param>
    /// <param name="receptionData">��M�f�[�^</param>
    public void CreatePlayer(NetworkConnectionToClient connection, PlayerData receptionData)
    {
        // client�̃v���C���[�v���n�u�𐶐�
        GameObject clietnPlayer = Instantiate(m_PlalyerPrefabsList.m_PlayerPrefabsList[receptionData.index].gameObject);
        clietnPlayer.transform.position = receptionData.position;

        // �T�[�o�[�Ƀv���C���[��ǉ�
        NetworkServer.AddPlayerForConnection(connection, clietnPlayer);
    }

    [Client]
    public static void Send(int index, Vector3 postion)
    {
        // �f�[�^�ݒ�
        playerData = new PlayerData(index, postion);

        // Server�Ƀf�[�^���M
        NetworkClient.Send(playerData);
    }

    [Client]
    public static void Send(PlayerData data)
    {
        // Server�Ƀf�[�^���M
        NetworkClient.Send(data);
    }
}

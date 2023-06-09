using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class CustomNetworkManager : NetworkManager
{

    //[Header("�v���C���[�v���n�u���X�g")]
    //[SerializeField] PlayerPrefabsList m_PlalyerPrefabsList = null;

    const string m_IPAddress = "localhost";         // IP�A�h���X

    public static List<PlayerData> playerDatas;            // �v���C���[�f�[�^���X�g
    public static List<NetworkConnectionToClient> clientDatas; // �ڑ����Ă���N���C�A���g���X�g

    public override void Awake()
    {
        base.Awake();

        // ���X�g�̏�����
        playerDatas = new List<PlayerData>();
        clientDatas = new List<NetworkConnectionToClient>();

        // PlayerData����M������AReceivedPlayerData�����s����悤�ɓo�^
        NetworkClient.RegisterHandler<PlayerData>(ReceivedPlayerData);
    }

    /// <summary>�T�[�o�[�ɃN���C�A���g���ڑ�������</summary>
    /// <param name="conn">�ڑ����Ă����N���C�A���g</param>
    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        base.OnServerConnect(conn);

        // �}�b�`���O�V�[���݂̂ł̏���
        if (SceneManager.GetActiveScene().name == Scene.TitleScene)
        {
            if (conn != null)
            {
                // �N���C�A���g���ڑ����Ă�����A����ǉ�
                playerDatas.Add(new PlayerData(NetworkServer.connections.Count - 1, Color.clear));
                clientDatas.Add(conn);
            }

            // PlayerData�̑��M
            ConnectionData sendData = new ConnectionData(NetworkServer.connections.Count);
            NetworkServer.SendToAll(sendData);
        }
    }

    /// <summary>�T�[�o�[�V�[���؂�ւ���</summary>
    /// <param name="sceneName">�V�[����</param>
    public override void OnServerSceneChanged(string sceneName)
    {
        for (int i = 0; i < playerDatas.Count; ++i)
        {
            // �v���C���[�F�����肵�āAplayerData�ɔ��f������
            GameSetting.instance.SetupPlayerColor();

            // plalyerDatas�̒��g���X�V���đ��M
            playerDatas[i] = new PlayerData(i, GameSetting.instance.playerColors[i]);
            NetworkServer.SendToAll(playerDatas[i]);
        }

        base.OnServerSceneChanged(sceneName);

        // �N���C�A���g�̃��[�J���v���C���[�����Ȃ��Ȃ�A�N���C�A���g����̃f�[�^�Ńv���C���[����������
        // Registerhandler<Message�N���X>(�֐�)�ŁAMessage����M������w�肵���֐������s����悤�ɓo�^����
        //if (NetworkClient.localPlayer == null) NetworkServer.RegisterHandler<PlayerData>(CreatePlayer);
    }

    public override void OnClientSceneChanged()
    {
        base.OnClientSceneChanged();

        Debug.Log("Client SceneChange!");
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        base.OnServerDisconnect(conn);

        var removeData = playerDatas.Find(p => p.index == conn.connectionId);

        if (removeData.index == conn.connectionId)
            Debug.Log("find");

        // �폜�������N���C�A���g
        var removeClient = clientDatas.Find(c => c == conn);

        // �ؒf�����v���C���[����Ō�̃v���C���[�܂ł̔ԍ���-1����
        for (int i = removeData.index; i < playerDatas.Count; ++i) playerDatas[i].SetIndex(playerDatas[i].index - 1);

        // conn�Ɉ�v����PlayerData�������ă��X�g����폜
        playerDatas.Remove(removeData);

        // PlayerCount���đ��M
        ConnectionData sendData = new ConnectionData { playerCount = NetworkServer.connections.Count };
        NetworkServer.SendToAll(sendData);
    }

    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();

        // �}�b�`���O�V�[���ɖ߂�
        SceneManager.LoadScene(Scene.TitleScene);
    }

    /// <summary>�N���C�A���g��~</summary>
    public override void OnStopClient()
    {
        base.OnStopClient();
    }

    /// <summary>�v���C���[����</summary>
    /// <param name="connection">�N���C�A���g�ւ̐ڑ�</param>
    /// <param name="receptionData">��M�f�[�^</param>
    //public void CreatePlayer(NetworkConnectionToClient connection, PlayerData receptionData)
    //{
    //    // client�̃v���C���[�v���n�u�𐶐�
    //    GameObject clietnPlayer = Instantiate(m_PlalyerPrefabsList.m_PlayerPrefabsList[receptionData.index].gameObject);
    //    clietnPlayer.transform.position = receptionData.position;

    //    // �T�[�o�[�Ƀv���C���[��ǉ�
    //    NetworkServer.AddPlayerForConnection(connection, clietnPlayer);
    //}

    [Client]
    public static void Send(int index)
    {
        // �ԍ�����v����PlayerData��Server�ɑ��M
        NetworkClient.Send(playerDatas.Find(p => p.index == index));
    }

    [Client]
    public static void Send(PlayerData data)
    {
        // Server�Ƀf�[�^���M
        NetworkClient.Send(data);
    }

    /// <summary>PlayerData��M</summary>
    /// <param name="receivedData">��M�f�[�^</param>
    void ReceivedPlayerData(PlayerData receivedData)
    {
        // playerColors�Ƀv���C���[���̐F�ݒ������
        GameSetting.instance.playerColors[receivedData.index] = receivedData.color;
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class CustomNetworkManager : NetworkManager
{

    [Header("プレイヤープレハブリスト")]
    [SerializeField] PlayerPrefabsList m_PlalyerPrefabsList = null;

    [Header("マッチングシーン")]
    [SerializeField, Scene] string m_MatchingScene = null;

    const string m_IPAddress = "localhost";         // IPアドレス

    public static PlayerData playerData;            // プレイヤーデータ

    public int m_PlayerIndex = 0;                  // プレイヤー番号

    /// <summary>サーバーにクライアントが接続した時</summary>
    /// <param name="conn">クライアントの接続</param>
    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        base.OnServerConnect(conn);

        // マッチングシーンのみでの処理
        //if (m_MatchingScene.Contains(SceneManager.GetActiveScene().name))
        //{
        //    //接続番号(クライアントを指定するときのKey値になる)
        //    int connectionIndex = 0;

        //    DecisionPlayerIndex(connectionIndex);

        //    if (NetworkServer.connections[connectionIndex] != null)
        //    {
        //        // 接続してきたクライアントを特定
        //        NetworkConnectionToClient toClient = NetworkServer.connections[connectionIndex];

        //        playerData = new PlayerData { index = m_PlayerIndex };

        //        // クライアントを指定して、データを送信
        //        toClient.Send(playerData);
        //    }

        //    // PlayerDataの送信
        //    ConnectionData sendData = new ConnectionData { playerCount = NetworkServer.connections.Count };
        //    NetworkServer.SendToAll(sendData);

        //    connectionIndex++;
        //}
    }

    /// <summary>サーバーシーン切り替え後</summary>
    /// <param name="sceneName">シーン名</param>
    public override void OnServerSceneChanged(string sceneName)
    {
        base.OnServerSceneChanged(sceneName);

        // クライアントのローカルプレイヤーがいないなら、クライアントからのデータでプレイヤー生成をする
        // Registerhandler<Messageクラス>(関数)で、Messageを受信したら指定した関数を実行するように登録する
        if (NetworkClient.localPlayer == null) NetworkServer.RegisterHandler<PlayerData>(CreatePlayer);
    }

    public override void OnClientSceneChanged()
    {
        base.OnClientSceneChanged();

        // データ設定
        playerData = new PlayerData { index = m_PlayerIndex, position = Vector3.one };

        // Serverにデータ送信
        Send(playerData);
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        base.OnServerDisconnect(conn);

        // PlayerCountを再送信
        ConnectionData sendData = new ConnectionData { playerCount = NetworkServer.connections.Count };
        NetworkServer.SendToAll(sendData);
    }

    /// <summary>クライアント停止</summary>
    public override void OnStopClient()
    {
        base.OnStopClient();

        // マッチングシーンに戻る
        //SceneManager.LoadScene(m_MatchingScene);
    }

    [Server]
    /// <summary>プレイヤー番号を決める</summary>
    int DecisionPlayerIndex(int connectionIndex)
    {
        // connectionIndexを最大接続数で割った余りを渡す。
        return m_PlayerIndex = connectionIndex % NetworkServer.maxConnections;
    }

    /// <summary>プレイヤー生成</summary>
    /// <param name="connection">クライアントへの接続</param>
    /// <param name="receptionData">受信データ</param>
    public void CreatePlayer(NetworkConnectionToClient connection, PlayerData receptionData)
    {
        // clientのプレイヤープレハブを生成
        GameObject clietnPlayer = Instantiate(m_PlalyerPrefabsList.m_PlayerPrefabsList[receptionData.index].gameObject);
        clietnPlayer.transform.position = receptionData.position;

        // サーバーにプレイヤーを追加
        NetworkServer.AddPlayerForConnection(connection, clietnPlayer);
    }

    [Client]
    public static void Send(int index, Vector3 postion)
    {
        // データ設定
        playerData = new PlayerData(index, postion);

        // Serverにデータ送信
        NetworkClient.Send(playerData);
    }

    [Client]
    public static void Send(PlayerData data)
    {
        // Serverにデータ送信
        NetworkClient.Send(data);
    }
}

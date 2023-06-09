using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class CustomNetworkManager : NetworkManager
{

    //[Header("プレイヤープレハブリスト")]
    //[SerializeField] PlayerPrefabsList m_PlalyerPrefabsList = null;

    const string m_IPAddress = "localhost";         // IPアドレス

    public static List<PlayerData> playerDatas;            // プレイヤーデータリスト
    public static List<NetworkConnectionToClient> clientDatas; // 接続しているクライアントリスト

    public override void Awake()
    {
        base.Awake();

        // リストの初期化
        playerDatas = new List<PlayerData>();
        clientDatas = new List<NetworkConnectionToClient>();

        // PlayerDataを受信したら、ReceivedPlayerDataを実行するように登録
        NetworkClient.RegisterHandler<PlayerData>(ReceivedPlayerData);
    }

    /// <summary>サーバーにクライアントが接続した時</summary>
    /// <param name="conn">接続してきたクライアント</param>
    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        base.OnServerConnect(conn);

        // マッチングシーンのみでの処理
        if (SceneManager.GetActiveScene().name == Scene.TitleScene)
        {
            if (conn != null)
            {
                // クライアントが接続してきたら、情報を追加
                playerDatas.Add(new PlayerData(NetworkServer.connections.Count - 1, Color.clear));
                clientDatas.Add(conn);
            }

            // PlayerDataの送信
            ConnectionData sendData = new ConnectionData(NetworkServer.connections.Count);
            NetworkServer.SendToAll(sendData);
        }
    }

    /// <summary>サーバーシーン切り替え後</summary>
    /// <param name="sceneName">シーン名</param>
    public override void OnServerSceneChanged(string sceneName)
    {
        for (int i = 0; i < playerDatas.Count; ++i)
        {
            // プレイヤー色を決定して、playerDataに反映させる
            GameSetting.instance.SetupPlayerColor();

            // plalyerDatasの中身を更新して送信
            playerDatas[i] = new PlayerData(i, GameSetting.instance.players[i].color);
            NetworkServer.SendToAll(playerDatas[i]);
        }

        base.OnServerSceneChanged(sceneName);

        // クライアントのローカルプレイヤーがいないなら、クライアントからのデータでプレイヤー生成をする
        // Registerhandler<Messageクラス>(関数)で、Messageを受信したら指定した関数を実行するように登録する
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

        // 削除したいクライアント
        var removeClient = clientDatas.Find(c => c == conn);

        // 切断したプレイヤーから最後のプレイヤーまでの番号を-1する
        for (int i = removeData.index; i < playerDatas.Count; ++i) playerDatas[i].SetIndex(playerDatas[i].index - 1);

        // connに一致するPlayerDataを見つけてリストから削除
        playerDatas.Remove(removeData);

        // PlayerCountを再送信
        ConnectionData sendData = new ConnectionData { playerCount = NetworkServer.connections.Count };
        NetworkServer.SendToAll(sendData);
    }

    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();

        // マッチングシーンに戻る
        SceneManager.LoadScene(Scene.TitleScene);
    }

    /// <summary>クライアント停止</summary>
    public override void OnStopClient()
    {
        base.OnStopClient();
    }

    /// <summary>プレイヤー生成</summary>
    /// <param name="connection">クライアントへの接続</param>
    /// <param name="receptionData">受信データ</param>
    //public void CreatePlayer(NetworkConnectionToClient connection, PlayerData receptionData)
    //{
    //    // clientのプレイヤープレハブを生成
    //    GameObject clietnPlayer = Instantiate(m_PlalyerPrefabsList.m_PlayerPrefabsList[receptionData.index].gameObject);
    //    clietnPlayer.transform.position = receptionData.position;

    //    // サーバーにプレイヤーを追加
    //    NetworkServer.AddPlayerForConnection(connection, clietnPlayer);
    //}

    [Client]
    public static void Send(int index)
    {
        // 番号が一致するPlayerDataをServerに送信
        NetworkClient.Send(playerDatas.Find(p => p.index == index));
    }

    [Client]
    public static void Send(PlayerData data)
    {
        // Serverにデータ送信
        NetworkClient.Send(data);
    }

    /// <summary>PlayerData受信</summary>
    /// <param name="receivedData">受信データ</param>
    void ReceivedPlayerData(PlayerData receivedData)
    {
        // playerColorsにプレイヤー分の色設定をする
        GameSetting.instance.players[receivedData.index].color = receivedData.color;
    }
}

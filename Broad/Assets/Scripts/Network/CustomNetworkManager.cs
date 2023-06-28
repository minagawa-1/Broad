using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using Cysharp.Threading.Tasks;

public class CustomNetworkManager : NetworkManager
{
    public List<PlayerData> playersData;            // プレイヤーデータリスト
    public List<NetworkConnectionToClient> clientsData; // 接続しているクライアントリスト

    public int readyCount = 0;   // 準備完了したプレイヤーの数

    public override void Awake()
    {
        // サーバー側が受信した時にデータに対応した関数を実行するように登録
        NetworkServer.RegisterHandler<ReadyData>(ServerReceivedReadyData);

        // リストの初期化
        playersData = new List<PlayerData>();
        clientsData = new List<NetworkConnectionToClient>();

        base.Awake();
    }

    /// <summary>サーバーにクライアントが接続した時</summary>
    /// <param name="conn">接続してきたクライアント</param>
    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        // タイトルシーンのみでの処理
        if (SceneManager.GetActiveScene().name == Scene.TitleScene)
        {
            if (conn != null)
            {
                // クライアントが接続してきたら、情報を追加
                playersData.Add(new PlayerData(NetworkServer.connections.Count - 1));
                clientsData.Add(conn);

                // 接続してきたクライアントにplayerDataを送信
                conn.Send(playersData[conn.connectionId]);
            }

            // PlayerDataの送信
            ConnectionData sendData = new ConnectionData(NetworkServer.connections.Count);
            NetworkServer.SendToAll(sendData);
        }

        base.OnServerConnect(conn);
    }

    /// <summary>サーバーシーン切り替え直後</summary>
    /// <param name="sceneName"></param>
    public override void OnServerSceneChanged(string sceneName)
    {
        base.OnServerSceneChanged(sceneName);
    }

    /// <summary>クライアントからの接続が切れた</summary>
    /// <param name="conn">接続が切れたクライアント情報</param>
    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        var removeData = playersData.Find(p => p.index == conn.connectionId);

        // 削除したいクライアント
        var removeClient = clientsData.Find(c => c == conn);

        // 切断したプレイヤーから最後のプレイヤーまでの番号を-1する
        for (int i = removeData.index; i < playersData.Count; ++i)
            playersData[i] = new PlayerData(playersData[i].index - 1);

        // connに一致するPlayerDataを見つけてリストから削除
        playersData.Remove(removeData);

        // PlayerCountを再送信
        ConnectionData sendData = new ConnectionData(NetworkServer.connections.Count);
        NetworkServer.SendToAll(sendData);

        base.OnServerDisconnect(conn);
    }

    public override void OnClientDisconnect()
    {
        // マッチングシーンに戻る
        SceneManager.LoadScene(Scene.TitleScene);

        base.OnClientDisconnect();
    }

    /// <summary>全クライアントが準備が完了したかを判定</summary>
    /// <param name="connection">接続してきたクライアント</param>
    /// <param name="receivedData">受信データ</param>
    void ServerReceivedReadyData(NetworkConnectionToClient connection, ReadyData receivedData)
    {
        // isReadyがtrueならカウントを増やす
        if (receivedData.isReady) readyCount++;

        Debug.Log("Ready Count : " + readyCount);
    }
}

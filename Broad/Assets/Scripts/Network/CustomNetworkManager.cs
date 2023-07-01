using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using Cysharp.Threading.Tasks;

public class CustomNetworkManager : NetworkManager
{
    public List<PlayerData> playerDataList;            // プレイヤーデータリスト
    public List<NetworkConnectionToClient> clientDataList; // 接続しているクライアントリスト

    public override void Awake()
    {
        // リストの初期化
        playerDataList = new List<PlayerData>();
        clientDataList = new List<NetworkConnectionToClient>();

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
                playerDataList.Add(new PlayerData(NetworkServer.connections.Count - 1));
                clientDataList.Add(conn);

                // 接続してきたクライアントにplayerDataを送信
                conn.Send(playerDataList[conn.connectionId]);
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

        var removeData = playerDataList.Find(p => p.index == conn.connectionId);

        // 削除したいクライアント
        var removeClient = clientDataList.Find(c => c == conn);

        // 切断したプレイヤーから最後のプレイヤーまでの番号を-1する
        for (int i = removeData.index; i < playerDataList.Count; ++i)
            playerDataList[i] = new PlayerData(playerDataList[i].index - 1);

        // connに一致するPlayerDataを見つけてリストから削除
        playerDataList.Remove(removeData);

        // PlayerCountを再送信
        ConnectionData sendData = new ConnectionData(NetworkServer.connections.Count);
        NetworkServer.SendToAll(sendData);

        base.OnServerDisconnect(conn);
    }

    /// <summary>サーバーへの接続が切れた</summary>
    public override void OnClientDisconnect()
    {
        // マッチングシーンに戻る
        SceneManager.LoadScene(Scene.TitleScene);

        base.OnClientDisconnect();
    }

    public override void OnStopServer()
    {
        playerDataList.Clear();
        clientDataList.Clear();

        base.OnStopServer();
    }
}

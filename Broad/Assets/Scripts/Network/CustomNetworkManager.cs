using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using Cysharp.Threading.Tasks;

public class CustomNetworkManager : NetworkManager
{
    public List<PlayerData> playerDataList;                 // プレイヤーデータリスト
    public List<NetworkConnectionToClient> clientDataList;  // 接続しているクライアントリスト

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
        if (SceneManager.GetActiveScene().name == Scene.TitleScene.ToString())
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

    /// <summary>クライアントからの接続が切れた</summary>
    /// <param name="conn">接続が切れたクライアント情報</param>
    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        // 自分以外のプレイヤーがいなければ終了
        if (NetworkServer.connections.Count < 2) StopServer();

        // プレイヤー人数が2人以上の場合
        else
        {
            // 削除したいプレイヤーデータ
            var removeData = playerDataList.Find(p => p.selfIndex == conn.connectionId);

            // 全クライアントにゲームメイン中にぬけたプレイヤーの情報を送信
            if (SceneManager.GetActiveScene().name == Scene.GameMainScene.ToString())
                NetworkServer.SendToAll(removeData);

            // 削除したいクライアント
            var removeClient = clientDataList.Find(c => c == conn);

            // connに一致するPlayerDataを見つけてリストから削除
            playerDataList.Remove(removeData);

            // PlayerCountを再送信
            ConnectionData sendData = new ConnectionData(NetworkServer.connections.Count);
            NetworkServer.SendToAll(sendData);
        }

        base.OnServerDisconnect(conn);
    }

    /// <summary>サーバーへの接続が切れた</summary>
    public override void OnClientDisconnect()
    {
        // 次のマッチングに盤面情報を持ち越さないように空にする
        GameManager.board.data = null;

        // マッチングシーンに戻る
        SceneManager.LoadScene((int)Scene.TitleScene);

        base.OnClientDisconnect();
    }

    public override void OnStopServer()
    {
        // 次のマッチングに影響が出ないようにリストのクリア
        playerDataList.Clear();
        clientDataList.Clear();

        // 次のマッチングに盤面情報を持ち越さないように空にする
        GameManager.board.data = null;

        // マッチングシーンに戻る
        SceneManager.LoadScene((int)Scene.TitleScene);

        base.OnStopServer();
    }

    
}

using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using Mirror.Discovery;

public class CustomNetworkDiscovery : NetworkDiscovery
{
    [Header("マルチプレイ開始用ボタン")]
    [SerializeField] Button m_MultiPlayButton = null;
    [Header("戻る用ボタン")]
    [SerializeField] Button m_BackButton = null;
    [Header("ゲーム開始ボタン")]
    [SerializeField] Button m_PlayButton = null;

    [Header("プレイ人数確認用テキスト")]
    [SerializeField] Text m_PlayerCountText = null;
    [Header("接続開始用テキスト")]
    [SerializeField] Text m_ConnectionStateText = null;
    [Header("プレイヤー番号用テキスト")]
    [SerializeField] Text m_PlayerIndexText = null;

    [Header("接続間隔")]
    [SerializeField] private int m_ConnectIntervalTime;
    [Header("待機時間")]
    [SerializeField] private int m_WaitTime;
    [Header("接続試行回数")]
    [SerializeField] private int m_ConnectTryCount;

    [Header("ゲームメインシーン名")]
    [SerializeField, Scene] string m_GameMainSceneName = null;
    [Header("自動シーン切り替え時間")]
    [SerializeField] private int m_AutSceneChangeTime;

    CustomNetworkManager m_CNetworkManager;   // カスタムネットワークマネージャー
    NetworkManager m_NetworkManager;        // ネットワークマネージャー
    ServerResponse m_DiscoverdServer;       // 見つけたいサーバー

    CancellationTokenSource m_CancelConnectServer;      // 検索時にキャンセルをするためのソース
    CancellationToken m_CancelConnectToken;       // 検索キャンセルトークン

    CancellationTokenSource m_CancelChangeScene;        // シーン切り替えキャンセルソース
    CancellationToken m_CancelChangeSceneToken;   // シーン切り替えキャンセルトークン

    // クライアントの接続状態がWAITING状態に表示する
    const string connection_status_client_waiting = "Waiting start...";

    // ホストの接続状態がWAITING状態に表示する
    const string connection_status_host_waiting = "Waiting other player...";

    // 接続状態がSUCCESS状態に表示する
    const string connection_status_success = "Success!";

    bool m_isHostReady;                 // ホスト準備完了フラグ
    int m_CurrentPlayerCount = 0;             // プレイヤー数

    /// <summary>削除</summary>
    private void OnDestroy()
    {
        // シーン遷移時などにサーバー検索を停止
        StopDiscovery();
    }

    private void Awake()
    {
        // HostReadyDataを受信したら、ReceivedReadyDataを実行するように登録
        NetworkClient.RegisterHandler<ReadyData>(ReceivedReadyData);

        // ConnectionDataを受信したらReceivedConnectDataを実行するように登録
        NetworkClient.RegisterHandler<ConnectionData>(ReceivedConnectionData);

        // PlayerDataを受信したらReceivedPlayerDataを実行するように登録
        NetworkClient.RegisterHandler<PlayerData>(ReceivedPlayerData);

        // NetworkManagerを探して、CustomNetworkManagerを取得
        m_CNetworkManager = GameObject.Find("NetworkManager").GetComponent<CustomNetworkManager>();

        // コールバック処理
        // サーバーが見つかったら呼ばれる
        OnServerFound.AddListener(ServerResponse =>
        {
            // 見つけたサーバーを入れる
            m_DiscoverdServer = ServerResponse;

            // Debug.Logで表示
            Debug.Log("ServerFound");
        });

        // MultiPlayButtonが押されたら呼ぶ
        m_MultiPlayButton.onClick.AddListener(() =>
        {
            Debug.Log("Search Connection");

            // 各ボタンの表示・非表示を切り替える
            m_BackButton.gameObject.SetActive(true);
            m_MultiPlayButton.gameObject.SetActive(false);

            // 各テキストの表示
            m_ConnectionStateText.gameObject.SetActive(true);
            m_PlayerCountText.gameObject.SetActive(true);
            m_PlayerIndexText.gameObject.SetActive(true);

            // キャンセルトークン用ソース取得
            m_CancelConnectServer = new CancellationTokenSource();
            // キャンセル用トークン生成
            m_CancelConnectToken = m_CancelConnectServer.Token;

            // シーン切り替えキャンセルトークン生成
            m_CancelChangeScene = new CancellationTokenSource();
            m_CancelChangeSceneToken = m_CancelChangeScene.Token;

            // サーバー検索
            // UniTaskの非同期処理はForget()を付けて呼ぶ
            // キャンセル用トークンを渡す事で、awaitをする非同期処理でCancel()の実行タイミングで処理の中断が可能
            TryConnect(m_CancelConnectToken).Forget();
        });

        // BuckButtonが押された
        m_BackButton.onClick.AddListener(() =>
        {
            Debug.Log("Cancel");

            // サーバー検索を停止
            StopDiscovery();

            // ホスト停止
            NetworkManager.singleton.StopHost();
            // クライアント停止
            NetworkManager.singleton.StopClient();

            // 非同期処理の停止
            // TokenSorceのキャンセルと廃棄をする
            Cancel(m_CancelConnectServer);

            Cancel(m_CancelChangeScene);
        });

        // PlayButtonが押された
        m_PlayButton.onClick.AddListener(() =>
        {
            Debug.Log("Ready Ok");

            // ホスト準備データ設定
            ReadyData readyData = new ReadyData() { isReady = true };
            // 全クライアントに送信
            NetworkServer.SendToAll(readyData);

            // m_PlayButtonを非表示
            m_PlayButton.gameObject.SetActive(false);
        });
    }

    /// <summary>サーバー検索</summary>
    /// <async>非同期<async>
    /// <await>指定した時間・条件が揃うまで待つ<await>
    /// [async/await] は処理を非同期に行うのではなく、非同期の処理を待つ仕組み
    /// <param name="token">キャンセル用トークン</param>
    async UniTaskVoid TryConnect(CancellationToken token)
    {
        // NetworkManager取得
        m_NetworkManager = NetworkManager.singleton;

        // 接続挑戦回数
        int tryCount = 0;

        // サーバー検索開始
        StartDiscovery();

        // Server又は、Clientがactiveな限り続ける
        while (!m_NetworkManager.isNetworkActive)
        {
            // connect_interval_time分遅らせて実行
            await UniTask.Delay(m_ConnectIntervalTime, cancellationToken: token);

            // キャンセルされたら、リターン
            if (token.IsCancellationRequested)
            {
                // 例外スローを投げる
                token.ThrowIfCancellationRequested();
                return;
            }
            // サーバーを見つけた
            // URIはURL(Web上にあるファイルの住所)と
            // URN(ネットワーク上の存在する文書などのリソースを一意に識別するための識別子)の総称
            if (m_DiscoverdServer.uri != null)
            {
                Debug.Log("Start Client");

                // 取得したURIを使ってクライアントとして開始する
                m_NetworkManager.StartClient(m_DiscoverdServer.uri);

                // クライアント待機中テキストを入れる
                m_ConnectionStateText.text = connection_status_client_waiting;

                // サーバー検索を止める
                StopDiscovery();

                // ホストの準備が整うまで待機
                await UniTask.WaitUntil(() => m_isHostReady, cancellationToken: token);

                // キャンセル要求されたか
                if (token.IsCancellationRequested) token.ThrowIfCancellationRequested();

                // 接続成功用テキストに切り替え
                m_ConnectionStateText.text = connection_status_success;
            }
            else
            {
                Debug.Log("Try Connect...");

                // カウントを増やす
                tryCount++;

                // カウントがconnect_try_countを超えた
                if (tryCount > m_ConnectTryCount)
                {
                    Debug.Log("Start Host");

                    // ホストとして開始
                    m_NetworkManager.StartHost();

                    // サーバーの宣言をする
                    // これをしないと、サーバーが見つからない
                    AdvertiseServer();

                    // サーバーレスポンスがあるかDebug.Logで表示
                    if (m_DiscoverdServer.uri != null) Debug.Log("ServerURI : NotNull");
                    else Debug.Log("ServerURI : null");

                    // ホスト待機中テキストに切り替え
                    m_ConnectionStateText.text = connection_status_host_waiting;

                    // ホストの準備が整うまで待機
                    await UniTask.WaitUntil(() => m_isHostReady, cancellationToken: token);

                    // 接続成功用テキストに切り替え
                    m_ConnectionStateText.text = connection_status_success;

                    // wait_time分待機
                    await UniTask.Delay(m_WaitTime, cancellationToken: token);

                    if (token.IsCancellationRequested) token.ThrowIfCancellationRequested();

                    // シーン変更
                    m_NetworkManager.ServerChangeScene(m_GameMainSceneName);
                }
            }
        }
    }

    /// <summary>自動シーン切り替え</summary>
    /// <param name="cahangeTime">切り替え時間</param>
    /// <param name="token">キャンセル用トークン</param>
    async UniTaskVoid AutChangeScene(int cahangeTime, CancellationToken token)
    {
        // ホストじゃないなら、リターン
        if (!NetworkClient.activeHost) return;

        // 時間になるまで待機
        await UniTask.Delay(cahangeTime, cancellationToken: token);

        // プレイヤー人数が2人より少ない
        if ( m_CurrentPlayerCount < 2)
        {
            // PlayButtonを非activeにする
            m_PlayButton.gameObject.SetActive(false);

            // シーン変更をキャンセル
            Cancel(m_CancelChangeScene);

            if (token.IsCancellationRequested)
            {
                token.ThrowIfCancellationRequested();
                return;
            }
        }

        // シーン変更
        m_NetworkManager.ServerChangeScene(m_GameMainSceneName);
    }

    private void Update()
    {

    }

    /// <summary>ホストが準備できたかのデータを受信</summary>
    /// <param name="reciveData">受信データ</param>
    void ReceivedReadyData(ReadyData reciveData)
    {
        // 準備出来たかのフラグを代入
        m_isHostReady = reciveData.isReady;
    }

    /// <summary>プレイヤー番号表示</summary>
    /// <param name="recivedData">受信データ</param>
    void ReceivedConnectionData(ConnectionData receivedData)
    {
        if (m_PlayButton == null) return;

        m_CurrentPlayerCount = receivedData.playerCount;

        // プレイヤー人数を表示
        m_PlayerCountText.text = m_CurrentPlayerCount + "/" + m_NetworkManager.maxConnections;

        // 自分がホストかつプレイヤー人数が2人以上
        if (NetworkClient.activeHost && m_CurrentPlayerCount >= 2)
        {
            // PlayButtonをactiveにする
            m_PlayButton.gameObject.SetActive(true);

            // 自動でシーン変更開始
            AutChangeScene(m_AutSceneChangeTime, m_CancelChangeSceneToken).Forget();
        }
    }

    /// <summary>プレイヤー番号を表示</summary>
    /// <param name="receivedData">受信データ</param>
    void ReceivedPlayerData(PlayerData receivedData)
    {
        // ホストがいないなら何もしない
        if (m_PlayButton == null) return;

        // プレイヤー番号を表示
        m_PlayerIndexText.text = $"You {receivedData.index}P";
    }

    /// <summary>キャンセル</summary>
    /// <param name="tokenSource">キャンセルトークンソース</param>
    void Cancel(CancellationTokenSource tokenSource)
    {
        // 自分がホストでないなら、リターン
        if (!NetworkClient.activeHost) return;

        // キャンセル
        tokenSource.Cancel();

        // 廃棄
        tokenSource.Dispose();
    }
}

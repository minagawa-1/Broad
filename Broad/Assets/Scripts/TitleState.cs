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
    [Chapter("▲NetworkDiscovery")]
    [Header("コンポーネント")]
    [SerializeField] StartButtonState m_StartButtonState;
    [SerializeField] GameSetting m_GameSetting;
    [SerializeField] Text[] m_MatchingTexts;

    [Chapter("接続情報")]
    [Header("間隔(ms)")]
    [SerializeField, Min(1)] private int m_ConnectInterval = 1;

    [Header("接続試行回数")]
    [SerializeField] private int m_ConnectTryCount;

    [Chapter("その他")]
    [Header("待機タイマー")]
    [SerializeField] woskni.Timer m_MatchTimer;

    NetworkManager m_NetworkManager;        // ネットワークマネージャー
    ServerResponse m_DiscoverdServer;       // 見つけたいサーバー

    CancellationTokenSource m_CancelConnectServer;      // 検索時にキャンセルをするためのソース
    CancellationToken m_CancelConnectToken;       // 検索キャンセルトークン

    [System.Serializable]
    enum MatchState
    {
        /// <summary>マッチングしていない</summary>
        None,

        /// <summary>マッチング開始直後</summary>
        StartMatch,

        /// <summary>マッチング中</summary>
        Matching,

        /// <summary>マッチング中止直後</summary>
        CancelMatch,

        /// <summary>マッチング完了</summary>
        CompleteMatch,
    }

    MatchState m_MatchState;

    void Awake()
    {
        // ConnectionDataを受信したらReceivedConnectDataを実行するように登録
        NetworkClient.RegisterHandler<ConnectionData>(ReceivedConnectionData);

        m_GameSetting.players = new PlayerData[1];
        m_GameSetting.players[0] = new PlayerData(0, Color.clear);

        m_MatchState = MatchState.None;

        // コールバック処理
        // サーバーが見つかったら呼ばれる
        OnServerFound.AddListener(ServerResponse =>
        {
            // 見つけたサーバーレスポンスを入れる
            m_DiscoverdServer = ServerResponse;

            // Debug.Logで表示
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

    /// <summary>マッチングしていない</summary>
    void None() 
    {
    }

    /// <summary>マッチング開始時</summary>
    void StartMatch()
    {
        m_StartButtonState.DoStartMatch();

        // 接続キャンセルトークン生成
        m_CancelConnectServer = new CancellationTokenSource();
        m_CancelConnectToken = m_CancelConnectServer.Token;

        // サーバー検索
        // UniTaskの非同期処理はForget()を付けて呼ぶ
        // キャンセル用トークンを渡す事で、awaitをする非同期処理でCancel()の実行タイミングで処理の中断が可能
        TryConnect(m_CancelConnectToken).Forget();

        m_MatchState = MatchState.Matching;
    }

    /// <summary>マッチング中</summary>
    void Matching()
    {
        // [※デバッグ用] 強制的にゲームスタート
        if(Input.GetMouseButtonDown(1)) m_MatchState = MatchState.CompleteMatch;

        // 人数が２人未満の場合はreturn
        if (m_GameSetting.players.Length < 2) return;

        m_MatchTimer.Update(false);

        if(m_MatchTimer.IsFinished())
        {
            m_MatchTimer.Reset();

            m_MatchState = MatchState.CompleteMatch;
        }
    }

    /// <summary>マッチング中止時</summary>
    void CancelMatch()
    {
        m_StartButtonState.DoCancelMatch(SetPlayerNum);

        // サーバー検索を停止
        StopDiscovery();

        // 人数が２人以上の場合
        if (m_GameSetting.players.Length > 1)
        {
            // ホスト・クライアントの停止
            NetworkManager.singleton.StopHost();
            NetworkManager.singleton.StopClient();
        }

        // 非同期処理の停止
        // TokenSorceのキャンセルと廃棄をする
        Cancel(m_CancelConnectServer);

        m_MatchState = MatchState.None;
    }

    /// <summary>マッチング完了</summary>
    void CompleteMatch()
    {
        // 1人の場合、2～4でランダムな人数にする
        if (GameSetting.instance.players.Length <= 1)
            GameSetting.instance.players = new PlayerData[Random.Range(2, 5)];

        // シーン遷移処理
        Transition.Instance.LoadScene(Scene.GameMainScene, m_NetworkManager);
    }

    /// <summary>プレイヤー人数をGameSettingに反映させる</summary>
    /// <param name="playerNum">プレイヤー人数</param>
    void SetPlayerNum(int playerNum)
    {
        // 最低でも１人以上
        playerNum = Mathf.Max(1, playerNum);

        // 設定には反映させる
        GameSetting.instance.players = new PlayerData[playerNum];

        if (m_MatchingTexts.Length == 0 || m_NetworkManager == null) return;

        foreach (Text text in m_MatchingTexts)
            if(text != null)text.text = $"Matching... ( {playerNum}人 )";
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
            // (m_ConnectInterval)ms分待機して実行
            await UniTask.Delay(m_ConnectInterval, cancellationToken: token);

            // サーバーを見つけた
            // URIはURL(Web上にあるファイルの住所)と
            // URN(ネットワーク上の存在する文書などのリソースを一意に識別するための識別子)の総称
            if (m_DiscoverdServer.uri != null)
            {
                Debug.Log("Start Client");

                // クライアントとして開始
                // 取得したURIを使ってサーバーに接続する
                m_NetworkManager.StartClient(m_DiscoverdServer.uri);

                // サーバー検索を止める
                StopDiscovery();
            }
            else
            {
                Debug.Log("Try Connect...");

                // カウントを増やす
                tryCount++;

                // 検索回数が規定値に達したとき
                if (tryCount > m_ConnectTryCount)
                {
                    Debug.Log("Start Host");

                    // ホストとして開始
                    m_NetworkManager.StartHost();

                    // サーバーの宣言をする
                    // これをしないと、サーバーが見つからない
                    AdvertiseServer();

                    // サーバーレスポンスがあるかDebug.Logで表示
                    if (m_DiscoverdServer.uri != null) Debug.Log("ServerURI : exist");
                    else Debug.Log("ServerURI : not found");
                }
            }
        }
    }

    /// <summary>接続データ受信</summary>
    /// <param name="recivedData">受信データ</param>
    void ReceivedConnectionData(ConnectionData receivedData)
    {
        // タイマーリセット
        m_MatchTimer.Reset();

        // playerNumに現在のプレイヤー数を反映
        SetPlayerNum(receivedData.playerCount);
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

using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Mirror;
using Mirror.Discovery;

public class TitleState : NetworkDiscovery
{
    [Chapter("▲NetworkDiscovery")]
    [Header("コンポーネント")]
    [SerializeField] GameSetting m_GameSetting;
    [SerializeField] StartButtonState m_StartButtonState;
    [SerializeField] Text[] m_MatchingTexts;
    [SerializeField] Button m_DeckButton;
    [SerializeField] Button m_ConfigButton;
    [SerializeField] CustomNetworkManager m_CustomNetworkManager;

    [Header("シーン遷移用に保持するコンポーネント")]
    [SerializeField] AudioListener m_AudioListener;
    [SerializeField] UnityEngine.EventSystems.EventSystem m_EventSystem;

    [Chapter("接続情報")]
    [Header("間隔(ms)")]
    [SerializeField, Min(1)] int m_ConnectInterval = 1;

    [Header("接続試行回数")]
    [SerializeField] int m_ConnectTryCount;

    [Chapter("その他")]
    [Header("待機タイマー")]
    [SerializeField] woskni.Timer m_MatchTimer;

    NetworkManager m_NetworkManager;        // ネットワークマネージャー
    ServerResponse m_DiscoverdServer;       // 見つけたいサーバー

    CancellationTokenSource m_CancelConnectServer;      // 検索時にキャンセルをするためのソース
    CancellationToken       m_CancelConnectToken;       // 検索キャンセルトークン

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
        // ホストがメッセージデータを受信したら対応した関数を実行するように登録
        NetworkServer.ReplaceHandler<PlayerData>(HostReceivedPlayerData);

        // クライアントがメッセージデータを受信したら対応した関数を実行するように登録
        NetworkClient.ReplaceHandler<PlayerData>(ReceivedPlalyerData);
        NetworkClient.ReplaceHandler<ConnectionData>(ReceivedConnectionData);
        NetworkClient.ReplaceHandler<ColorData>(ReceivedColorData);
        NetworkClient.ReplaceHandler<NameData>(ReceivedNameData);

        // GameSettingの初期化
        if (m_GameSetting.playerColors.Length == 0 || m_GameSetting.playerColors[0].a == 0f)
        {
            m_GameSetting.selfIndex       = 0;
            m_GameSetting.playerColors    = new Color[1];
            m_GameSetting.playerColors[0] = SaveSystem.data.lastColor;
        }

        // プレイヤー名をGameSettingに保存
        m_GameSetting.playerNames = new string[4];
        m_GameSetting.playerNames[0] = Config.data.playerName;

        m_MatchState = MatchState.None;

        // コールバック処理
        // サーバーが見つかったら呼ばれる
        OnServerFound.AddListener(ServerResponse =>
        {
            // 見つけたサーバーレスポンスを入れる
            m_DiscoverdServer = ServerResponse;
        });
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(m_MatchState);

        switch (m_MatchState)
        {
            case MatchState.None:           None();          break;
            case MatchState.StartMatch:     StartMatch();    break;
            case MatchState.Matching:       Matching();      break;
            case MatchState.CancelMatch:    CancelMatch();   break;
            case MatchState.CompleteMatch:  CompleteMatch(); break;
        }
    }

    /// <summary>マッチングしていない</summary>
    void None()
    {
        // Altキー検知
        if (Keyboard.current.leftAltKey.isPressed || Keyboard.current.rightAltKey.isPressed)
        {
            // Alt + R
            if (Keyboard.current.rKey.wasPressedThisFrame)
            {
                // データを初期化してデバッグログに残す
                SaveSystem.Reset();
                SaveSystem.ConfirmData();
            }
        }
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
        // キャンセル用トークンを渡す事で、awaitをしているタイミングでCancel()をすると処理の中断が可能
        TryConnect(m_CancelConnectToken).Forget();

        m_MatchState = MatchState.Matching;
    }

    /// <summary>マッチング中</summary>
    void Matching()
    {
        // [※デバッグ用] Sキー・△ボタン・Xボタンで強制スタート
        if (Gamepad .current != null && Gamepad .current.buttonNorth.isPressed) m_MatchState = MatchState.CompleteMatch;
        if (Keyboard.current != null && Keyboard.current.sKey.isPressed)        m_MatchState = MatchState.CompleteMatch;

        // Xキー・×ボタン・Bボタンでキャンセル
        if (Gamepad .current != null && Gamepad .current.buttonSouth.isPressed) m_MatchState = MatchState.CancelMatch;
        if (Keyboard.current != null && Keyboard.current.xKey.isPressed)        m_MatchState = MatchState.CancelMatch;

        // 人数が２人未満の場合はreturn
        if (NetworkServer.connections.Count < 2) return;

        m_MatchTimer.Update(false);

        if (m_MatchTimer.IsFinished())
        {
            m_MatchTimer.Reset();

            m_MatchState = MatchState.CompleteMatch;
        }
    }

    /// <summary>マッチング中止時</summary>
    void CancelMatch()
    {
        if (!NetworkClient.active) return;

        m_StartButtonState.DoCancelMatch(SetPlayerNum);

        // サーバー検索を停止
        StopDiscovery();

        NetworkManager.singleton.StopHost();

        // 接続していたサーバーのURIを破棄
        m_DiscoverdServer.uri = null;

        // 非同期処理の停止
        // TokenSorceのキャンセルと廃棄をする
        Cancel(m_CancelConnectServer);

        m_MatchState = MatchState.None;
    }

    /// <summary>マッチング完了</summary>
    void CompleteMatch()
    {
        // プレイヤーカラー設定
        if (!Transition.instance.fading)
        {
            SetupPlayerColor();

            SetupPlayerName().Forget();

            // シーン遷移処理
            Transition.instance.LoadScene(Scene.GameMainScene.ToString(), m_NetworkManager, 1f, 0.5f);
        }
    }

    /// <summary>プレイヤー人数をGameSettingに反映させる</summary>
    /// <param name="playerNum">プレイヤー人数</param>
    void SetPlayerNum(int playerNum)
    {
        // 最低でも１人以上
        playerNum = Mathf.Max(1, playerNum);

        if (m_MatchingTexts.Length == 0 || m_NetworkManager == null) return;

        foreach (Text text in m_MatchingTexts)
            if (text != null) text.text = $"Matching... ( {playerNum}人 )";
    }

    /// <summary>プレイヤーカラーの設定</summary>
    void SetupPlayerColor()
    {
        // ホストのみ色の設定をする
        if (NetworkClient.activeHost)
        {
            float h = Random.value;
            float s = Random.Range(0.2f, 0.5f);
            float v = Random.Range(0.9f, 1f);
            Color color1P = Color.HSVToRGB(h, s, v);

            // 1Pから相対的に離れた色相の色配列を取得
            m_GameSetting.playerColors = color1P.GetRelativeColor(NetworkServer.maxConnections);

            // ColorDataをクライアント全員に送信
            ColorData color = new ColorData(m_GameSetting.playerColors);
            NetworkServer.SendToAll(color);
        }
    }

    async UniTask SetupPlayerName()
    {
//////////// ▼共通の処理▼ /////////////////////////////////////////////////////////////////////////////////////////////////////
        // サーバーに自分のプレイヤー情報を送信
        PlayerData playerData = new PlayerData(m_GameSetting.selfIndex, m_GameSetting.playerNames[0]);
        NetworkClient.Send(playerData);

/////////// ▼ホストの処理▼ ///////////////////////////////////////////////////////////////////////////////////////////////////
        if (NetworkClient.activeHost)
        {
            // [最大接続人数] - [サーバーに接続している人数]でCPUの数を求める
            int cpuCount = NetworkServer.maxConnections - NetworkServer.connections.Count;

            // プレイヤー名を設定していない数
            int unsetNameCount = 0;
            for (int i = 0; i < m_GameSetting.playerNames.Length; ++i)
                if (m_GameSetting.playerNames[i] == null) unsetNameCount++;

            // プレイヤー全員の名前が揃うまで待機
            await UniTask.WaitUntil(() => unsetNameCount == cpuCount);

            // PlayerNamesにCPU用の名前を追加
            for (int i = 0; i < cpuCount; ++i)
                m_GameSetting.playerNames[i + 1] = $"CPU[{i + 1}]";

            // 全クライアントに名前情報を送信
            NameData nameData = new NameData(m_GameSetting.playerNames);
            NetworkServer.SendToAll(nameData);
        }
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
            await UniTask.DelayFrame(m_ConnectInterval, cancellationToken: token);

            // サーバーレスポンスがあるかDebug.Logで表示
            if (m_DiscoverdServer.uri != null) Debug.Log("ServerURI : exist");
            else Debug.Log("ServerURI : not found");

            // サーバーを見つけた
            // URIはURL(Web上にあるファイルの住所)と
            // URN(ネットワーク上の存在する文書などのリソースを一意に識別するための識別子)の総称
            if (m_DiscoverdServer.uri != null)
            {
                // クライアントとして開始
                // 取得したURIを使ってサーバーに接続する
                m_NetworkManager.StartClient(m_DiscoverdServer.uri);

                Debug.Log("Start Client");

                // サーバー検索を止める
                StopDiscovery();

                Debug.Log("Start Client");
            }
            else
            {
                // カウントを増やす
                tryCount++;

                // 検索回数が規定値に達したとき
                if (tryCount > m_ConnectTryCount)
                {
                    // ホストとして開始
                    m_NetworkManager.StartHost();

                    // サーバーの宣言をする
                    // これをしないと、サーバーが見つからない
                    AdvertiseServer();

                    Debug.Log("Start Host");
                }
            }
        }
    }


///////////// ▼ホストの処理▼ /////////////////////////////////////////////////////////////////////////////////////////////////
/// <summary>クライアントからのプレイヤー情報を取得</summary>
/// <param name="playerData">プレイヤー情報</param>
    void HostReceivedPlayerData(PlayerData playerData)
    {
        m_GameSetting.playerNames[playerData.selfIndex] = playerData.name;
    }

///////////// ▼クライアントの処理▼ ////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>ホストからプレイヤーデータ受信</summary>
    /// <param name="playerData">プレイヤーデータ</param>
    void ReceivedPlalyerData(PlayerData playerData)
    {
        m_GameSetting.selfIndex = playerData.selfIndex;
    }

    /// <summary>ホストから接続データ受信</summary>
    /// <param name="connectionData">接続データ</param>
    void ReceivedConnectionData(ConnectionData connectionData)
    {
        // タイマーリセット
        m_MatchTimer.Reset();

        // playerNumに現在のプレイヤー数を反映
        SetPlayerNum(connectionData.playerCount);
    }

    /// <summary>カラーデータ受信</summary>
    /// <param name="colorData">受信データ</param>
    void ReceivedColorData(ColorData colorData)
    {
        // 受信したデータをplayersColorに入れる
        m_GameSetting.playerColors = colorData.color;
    }

    /// <summary>ホストからプレイヤー名情報を受信</summary>
    /// <param name="nameData">プレイヤー名情報</param>
    void ReceivedNameData(NameData nameData)
    {
        m_GameSetting.playerNames = nameData.name;
    }

//////////////// ▼共通の処理▼ ////////////////////////////////////////////////////////////////////////////////

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
            case MatchState.None:     m_MatchState = MatchState.StartMatch; break;
            case MatchState.Matching: m_MatchState = MatchState.CancelMatch; break;

            default: break;
        }
    }

    public void OpenScene(string sceneName)
    {
        if (m_MatchState >= MatchState.CompleteMatch) return;

        m_EventSystem.enabled = false;
        m_AudioListener.enabled = false;

        SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
    }

    public void OnClosedScene(Scene scene)
    {
        m_EventSystem.SetSelectedGameObject((scene == Scene.DeckScene ? m_DeckButton : m_ConfigButton).gameObject);
        m_EventSystem.enabled = true;
        m_AudioListener.enabled = true;
    }
}

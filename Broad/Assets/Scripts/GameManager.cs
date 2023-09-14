using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
using Mirror;

[RequireComponent(typeof(MeshCombiner))]
public partial class GameManager : MonoBehaviour
{
    [Chapter("コンポーネント")]

    [Header("マス目用プレファブ")]
    [SerializeField] GameObject m_SquarePrefab = null;

    [Header("設置不可マス用プレファブ")]
    [SerializeField] GameObject m_UnsetbleSquarePrefab = null;

    [Header("生成したいブロックのプレファブ")]
    [SerializeField] GameObject m_BlockPrefab = null;

    [Header("カメラの最遠のときのOffset値")]
    [SerializeField] Vector3 m_FurthestCameraOffset;

    [Header("コンポーネント")]
    [SerializeField] BlockManager m_BlockManager = null;     // ブロックマネージャー
    [SerializeField] DonutChart   m_DonutChart   = null;     // ドーナツチャート

    // ネットワークマネージャー
    CustomNetworkManager m_NetworkManager = null;

    // 盤面管理オブジェクト
    GameObject m_BoardManagerObject = null;
    GameObject m_UnsetableParent = null;
    GameObject m_SetableParent = null;

    public List<BoardData> orderBoardDataList;  // ターン内での各プレイヤーの設置申請情報

    public static Board board;                  // 盤面

    public static Vector2Int boardSize;         // 盤面のサイズ

    public int readyCount = 0;                  // 準備完了したプレイヤーの数

    public bool isReceived;                     // データを受信できたか

    public List<GameObject> createdBlockList;   // 生成したブロックを格納するリスト

    /// <summary>CPUの情報</summary>
    public class CPUData
    {
        public Hand         hand;
        public CPU.CPUType  type;
        public woskni.Range priorityRange;
    }

    public List<CPUData> cpuList;              // CPUの手札のリスト

    private void Awake()
    {
        // サーバー側がデータを受信したときに対応した関数を実行するように登録
        NetworkServer.RegisterHandler<ReadyData>(ServerReceivedReadyData);
        NetworkServer.ReplaceHandler<BoardData>(ServerReceivedBoardData);

        // クライアント側がデータを受信したときに対応した関数を実行するように登録
        NetworkClient.RegisterHandler<BoardData>(ClientReceivedBoardData);
        NetworkClient.ReplaceHandler<PlayerData>(ClientReceivedPlayerData);

        // シーンをまたいで存在するNetworkManagerの取得
        m_NetworkManager = GameObject.Find(nameof(NetworkManager)).GetComponent<CustomNetworkManager>();

        m_BoardManagerObject = new GameObject("BoardManager");
        m_SetableParent = new GameObject("SetableSquares");
        m_UnsetableParent = new GameObject("UnsetableSquares");

        // 盤面管理オブジェクトを親にする
        m_SetableParent.transform.SetParent(m_BoardManagerObject.transform);
        m_UnsetableParent.transform.SetParent(m_BoardManagerObject.transform);

        // GameMainに到着したことをホストに伝える
        ReadyData readyData = new ReadyData(true);
        NetworkClient.Send(readyData);

        // リストを初期化
        orderBoardDataList = new List<BoardData>();
        createdBlockList = new List<GameObject>();
        cpuList = new List<CPUData>();

        // CPUの手札の設定をする
        if (NetworkClient.activeHost)
        {
            for (int i = NetworkServer.connections.Count; i < NetworkServer.maxConnections; ++i)
            {
                Blocks[] deckBlocks = new Blocks[GameSetting.deck_blocks];

                // CPUの情報
                var cpu = new CPUData();
                {
                    // CPUの種類
                    cpu.type = woskni.Enum.AtRandom<CPU.CPUType>();

                    // アルゴリズムの優先度
                    float min = Random.value;
                    cpu.priorityRange = new woskni.Range(min, Random.Range(min, 1f));

                    // ブロックの個数傾向
                    var unitsRange = new woskni.RangeInt(1, 10);
                    var densityRange = new woskni.Range(0f, 1f);
                    
                    if (cpu.type == CPU.CPUType.Larger) {
                        unitsRange.min = new woskni.RangeInt(1, 6 ).Lerp(unitsRange.Random(), unitsRange);
                        unitsRange.max = new woskni.RangeInt(5, 10).Lerp(unitsRange.Random(), unitsRange);
                    }
                    else if (cpu.type == CPU.CPUType.Densitic) {
                        densityRange.min = new woskni.Range(0f, 0.7f).Lerp(densityRange.Random(), densityRange);
                        densityRange.max = new woskni.Range(0.3f, 1f).Lerp(densityRange.Random(), densityRange);
                    }

                    // ブロックの抽選処理
                    for (int k = 0; k < GameSetting.deck_blocks; ++k)
                        deckBlocks[k] = LotteryBlocks.Lottery(unitsRange.Random(), densityRange.Random());

                    // 手札情報
                    cpu.hand = new Hand(new Deck(deckBlocks), GameSetting.hand_blocks);
                }

                Debug.Log
                    ("Player[" + i + "]: { type:" + cpu.type.ToString() + ", priority: " + cpu.priorityRange.ToString("F2") + "}");

                cpuList.Add(cpu);
            }
        }
    }

    private void Start()
    {
        // UIのローカライズ
        Localization.Setup();
        Localization.Correct();

        // ボード生成
        LayOutBoard().Forget();
    }

    /// <summary>ボードの生成</summary>
    private async UniTaskVoid LayOutBoard()
    {

/////////////// ▼ホストの処理▼ ///////////////////////////////////////////////////////////////////////////////////////////////////

        // ホストのみボード設定をする
        if (NetworkClient.activeHost)
        {
            // 以下の条件のいずれかに一致するまで待機
            // ① プレイヤー全員が準備完了する
            // ② プレイヤーの人数が１人
            await UniTask.WaitUntil(() => readyCount == NetworkServer.connections.Count ||
                                            NetworkServer.connections.Count < 2);

            // readyCountのリセット
            readyCount -= NetworkServer.connections.Count;

            // ボードサイズをランダムに設定
            boardSize = RandomVector2Int(GameSetting.instance.minBoardSize, GameSetting.instance.maxBoardSize);

            // 盤面のサイズを渡す
            board = new Board(boardSize.x, boardSize.y);

            // 設置不可マスの決定
            board.ShaveBoard();

            // 全クライアントにボード情報を送信
            BoardData sendData = new BoardData { board = board };
            NetworkServer.SendToAll(sendData);
        }

///////////////// ▼共通の処理▼ ///////////////////////////////////////////////////////////////////////////////////////////////////

        // ボードの情報が入るまで待機
        await UniTask.WaitUntil(() => board.data != null && board.data.Length > 0);

        // ボードの作成
        LayOutSquare(m_SquarePrefab, m_UnsetbleSquarePrefab, boardSize);

        // カメラの位置を移動
        float rate = Mathf.InverseLerp(GameSetting.instance.minBoardSize.y, GameSetting.instance.maxBoardSize.y, boardSize.y);
        Vector3 offset = new Vector3(boardSize.x / 2f, rate * m_FurthestCameraOffset.y, rate * m_FurthestCameraOffset.z);
        Camera.main.transform.position = Camera.main.transform.position.Offset(offset);

        // 背景の作成
        CreateBackGround(m_UnsetbleSquarePrefab, boardSize);

        m_BlockManager.CreateMaterials();

        // 設置可能マスのメッシュの結合処理
        //CombineMeshes().Forget();
    }

    // メッシュの結合処理
    async UniTaskVoid CombineMeshes()
    {
        await UniTask.DelayFrame(1);

        // 設置可能マスのメッシュ結合
        MeshCombiner.Combine(m_SetableParent.transform.GetChildren().ToGameObjects(), "SetableBoard", m_BoardManagerObject.transform);

        // 設置不可マスと枠外背景のメッシュの結合
        GameObject[] background = GameObject.Find("Background").transform.GetChildren().ToGameObjects();
        GameObject[] unsetable = m_UnsetableParent.transform.GetChildren().ToGameObjects().Concat(background).ToArray();
        var us = MeshCombiner.Combine(unsetable, "UnsetableBoard", m_BoardManagerObject.transform);

        // もういらないので消し飛ばす
        Destroy(m_SetableParent);
        Destroy(m_UnsetableParent);
    }

    /// <summary>マス目を敷き詰める</summary>
    /// <param name="squarePrefab">プレファブ</param>
    /// <param name="unsetablePrefab">プレファブ</param>
    /// <param name="boardSize">盤面の縦横幅</param>
    /// <param name="squareSize">マスのサイズ</param>
    /// <param name="parentTransform">親のトランスフォーム</param>
    void LayOutSquare(GameObject setablePrefab, GameObject unsetablePrefab, Vector2Int boardSize)
    {
        // オブジェクトの生成
        // y軸方向の生成
        for (int y = 0; y < boardSize.y; y++)
        {
            // x軸方向の生成
            for (int x = 0; x < boardSize.x; x++)
            {
                // 設置状況の初期化
                if (board.GetBoardData(x, y) > 0) board.SetBoardData(0, x, y);

                // マス目生成
                GameObject newSquare = Instantiate(board.GetBoardData(x, y) == 0 ? setablePrefab : unsetablePrefab);

                // 新しく生成したオブジェクトの名前・親・座標・スケールを設定する
                newSquare.gameObject.name = $"Square[{x}, {y}]";
                newSquare.transform.parent = board.GetBoardData(x, y) == 0 ? m_SetableParent.transform : m_UnsetableParent.transform;
                newSquare.transform.position = new Vector3(x, -0.1f, -y);
                newSquare.transform.position = newSquare.transform.position;
                newSquare.transform.localScale = newSquare.transform.localScale;
            }
        }
    }

    /// <summary>他プレイヤーのブロックを生成</summary>
    /// <param name="after">設置後の盤面情報</param>
    /// <param name="before">設置前の盤面情報</param>
    /// <returns>生成したブロックのオブジェクト</returns>
    public List<GameObject> ComplementBoard(Board after, Board before)
    {
        createdBlockList = new List<GameObject>();
        createdBlockList.Clear();

        for (int y = 0; y < boardSize.y; ++y)
        {
            for (int x = 0; x < boardSize.x; ++x)
            {
                // 設置しない盤・設置できない盤の場合はcontinue
                if (after.GetBoardData(x, y) <= 0) continue;

                // このターンでの[x, y]座標において、設置情報に変化がない場合はcontinue
                if (after.GetBoardData(x, y) == before.GetBoardData(x, y)) continue;

                // プレファブの生成
                GameObject newBlock = Instantiate(m_BlockPrefab);

                // 名前・親オブジェクト・座標・マテリアルを設定
                newBlock.gameObject.name = $"Block[{x}, {y}]new";
                newBlock.transform.parent = m_BlockManager.blockParent.transform;
                newBlock.transform.position = new Vector3Int(x, 1, -y);
                newBlock.GetComponent<MeshRenderer>().material = m_BlockManager.m_SetBlockMaterials[after.GetBoardData(x, y) - 1];
                newBlock.transform.position = newBlock.transform.position;
                newBlock.transform.localScale = newBlock.transform.localScale;

                createdBlockList.Add(newBlock);
            }
        }

        return createdBlockList;
    }

    /// <summary>重複除去</summary>
    /// <param name="setData"><b>タプルの設置データ</b>
    /// <br></br>player: プレイヤー番号
    /// <br></br>set: 設置するボード上の位置</param>
    /// <returns>重複しているオブジェクト</returns>
    public List<GameObject> RidDuplicate(BoardData[] setData)
    {
        var duplicateList = new List<GameObject>();

        int[,] newBoard = board.GetBoard();

        // Board -> bool[,]変換
        // ボードのサイズ分調べる
        for (int y = 0; y < setData[0].board.height; y++)
        {
            for (int x = 0; x < setData[0].board.width; x++)
            {
                // 座標に対して設置申請を出しているplayer番号を抽出（スキップしたプレイヤーは無視）
                int[] players = setData.Where(d => d.board.GetBoardData(x, y) != 0).Select(d => d.player).ToArray();

                // その座標の設置申請が2人以上だった場合は0を、 1人だった場合はplayer番号を代入
                switch (players.Length)
                {
                    case 0: break;
                    case 1: newBoard[x, y] = players[0]; break;

                    // 重複
                    default:
                        newBoard[x, y] = board.GetBoardData(new Vector2Int(x, y));

                        // x, yに該当する座標のブロックをこのターンに生成したブロックリストから抽出
                        var dups = createdBlockList.Where(obj => ToInt(obj.transform.position) == new Vector3Int(x, 0, -y)).ToArray();

                        duplicateList.AddRange(dups);

                        break;
                }
            }
        }

        // 判定後のボードデータを作る
        Board checkedBoard = new Board(boardSize.x ,boardSize.y);

        checkedBoard.SetBoard(newBoard);

        // テキストスクリプトに重複判定後の盤面情報を出力
        OutputDebugText(checkedBoard, "board[,].txt");

        // プレイヤー全員に判定後のボード情報を送信
        BoardData boardData = new BoardData(checkedBoard);
        NetworkServer.SendToAll(boardData);

        return duplicateList;
    }

    /// <summary>プレイヤーがゲームから抜けた</summary>
    /// <param name="playerIndex">プレイヤー番号</param>
    void PlayerExits(int playerIndex)
    {
        // 抜けたプレイヤーが設置した場所を探す
        for (int y = 0; y < boardSize.y; ++y)
        {
            for (int x = 0; x < boardSize.x; ++x)
            {
                // 設置個所が判明
                if (board.GetBoardData(x, y) == playerIndex)
                {
                    // 一致した箇所を0に変更
                    board.SetBoardData(0, x, y);

                    // 座標と一致する名前のブロックを削除
                    Transform destroyObject = m_BlockManager.blockParent.transform.Find($"Block[{x}, {y}]");

                    // 一致した座標のブロックを削除
                    Destroy(destroyObject.gameObject);
                }
            }
        }

        // ドーナツチャートを更新
        m_DonutChart.UpdateDonut();
    }

    /// <summary> 背景生成 </summary>
    /// <param name="prefab">プレファブ</param>
    /// <param name="boardSize">ボードのサイズ</param>
    void CreateBackGround(GameObject prefab, Vector2Int boardSize)
    {
        // 親の生成
        GameObject parent = new GameObject("Background");

        // 盤面の最大サイズ + 余白 の長さ
        const float size = 48f;

        // 左
        GameObject left = Instantiate(prefab, parent.transform);
        left.name = "BackgroundLeft";
        left.transform.localScale = new Vector3(size / 2f - boardSize.x / 2f, left.transform.localScale.y, size);
        left.transform.position = new Vector3(-left.transform.localScale.x / 2f - 0.5f, -0.1f, -boardSize.y / 2f + 0.5f);

        // 右
        GameObject right = Instantiate(left, parent.transform);
        right.name = "BackgroundRight";
        right.transform.position = right.transform.position.Offset(x: boardSize.x + left.transform.localScale.x);

        // 上
        GameObject top = Instantiate(prefab, parent.transform);
        top.name = "BackgroundTop";
        top.transform.localScale = new Vector3(boardSize.x, top.transform.localScale.y, size / 2f - boardSize.y / 2f);
        top.transform.position = new Vector3(boardSize.x / 2f - 0.5f, -0.1f, top.transform.localScale.z / 2 + 0.5f);

        // 下
        GameObject bottom = Instantiate(top, parent.transform);
        bottom.name = "BackgroundBottom";
        top.transform.position = bottom.transform.position.Offset(z: -boardSize.y - top.transform.localScale.z);
    }

    // 盤面情報をでバック用テキストスクリプトに出力
    static void OutputDebugText(Board board, string filePath = "debugText.txt")
    {
        string debugText = "";

        for (int y = 0; y < GameManager.boardSize.y; ++y)
        {
            for (int x = 0; x < GameManager.boardSize.x; ++x)
            {
                int n = board.GetBoardData(x, y);

                switch (n)
                {
                    case -1: debugText += "　"; break;
                    case 0: debugText += "・"; break;
                    default: debugText += n.ToString().ToFullWidth(); break;
                }
            }

            debugText += "\n";
        }


        TextOperate.WriteFile(filePath, debugText);
    }

/////////////// ▼ ホストの処理 ▼ /////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>クライアントからの準備完了の合図を受信</summary>
    /// <param name="connection">接続してきたクライアント</param>
    /// <param name="readyData">合図用データ</param>
    void ServerReceivedReadyData(NetworkConnectionToClient connection, ReadyData readyData)
    {
        // データがtrueなことを確認
        if (readyData.isReady) readyCount++;
    }

    /// <summary>サーバーボードデータ受信</summary>
    /// <param name="connection">サーバーに接続しているクライアント</param>
    /// <param name="receivedData">受信データ</param>
    void ServerReceivedBoardData(NetworkConnectionToClient connection, BoardData receivedData)
    {
        // 受信したデータをリストに追加
        orderBoardDataList.Add(receivedData);
    }

//////////////// ▼共通の処理▼ /////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>クライアント側で盤面情報を取得</summary>
    /// <param name="boardData">盤面データ</param>
    void ClientReceivedBoardData(BoardData boardData)
    {
        // 重複判定をした盤面情報を受信したらフラグをtrueにする
        if (board.data != null) isReceived = true;

        // 新しい盤面情報を取得
        board = boardData.board;

        // 盤面のサイズを取得
        boardSize = new Vector2Int(boardData.board.width, boardData.board.height);
    }

    /// <summary>クライアント側でプレイヤーデータを受信</summary>
    /// <param name="playerData">プレイヤーデータ</param>
    void ClientReceivedPlayerData(PlayerData playerData)
    {
        // ゲーム中に抜けたプレイヤーのブロックを削除
        // ※ selfIndexに1加算しているのは、設置時に1加算しているためそっちに合わせる
        PlayerExits(playerData.selfIndex + 1);
    }

    // 範囲内でランダムな座標を返す関数
    Vector2Int RandomVector2Int(Vector2Int min, Vector2Int max)
        => new Vector2Int(Random.Range(min.x, max.x), Random.Range(min.y, max.y));

    Vector3Int ToInt(Vector3 v) => new Vector3Int((int)v.x, (int)v.y, (int)v.z);
}

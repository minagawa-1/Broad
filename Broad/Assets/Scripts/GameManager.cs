using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
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
    [SerializeField] CalcBroad    m_CalcBroad    = null;     // つながっているブロックの数を計算

    // ネットワークマネージャー
    CustomNetworkManager m_NetworkManager    = null;

    // 盤面管理オブジェクト
    GameObject m_BoardManagerObject = null;
    GameObject m_UnsetableParent = null;
    GameObject m_SetableParent = null;

    public List<BoardData> boardsData;          // 全プレイヤーのボードデータを格納する

    public static  Board board;             // 盤面
    public static  Board oldBoard;          // 前のターンの盤面

    public static Vector2Int boardSize;     // 盤面のサイズ

    public bool    isReceived;              // データを受信できたか

    private void Awake()
    {
        // サーバー側がデータを受信したときに対応した関数を実行するように登録
        NetworkServer.ReplaceHandler<BoardData>(ServerReceivedBoardData);

        // クライアント側がデータを受信したときに対応した関数を実行するように登録
        NetworkClient.RegisterHandler<BoardData>(ClientReceivedBoardData);

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
        boardsData = new List<BoardData>();
    }

    private void Start()
    {
        m_State = State.Placement;

        // ボード生成
        LayOutBoard().Forget();
    }

    /// <summary>ボードの生成</summary>
    private async UniTaskVoid LayOutBoard()
    {

/////////////// ▼ホストの処理▼ ///////////////////////////////////////////////////////////////////////////////////////////////

        // ホストのみボード設定をする
        if (NetworkClient.activeHost)
        {
            // 以下の条件のいずれかに一致するまで待機
            // ① プレイヤー全員が準備完了する
            // ② プレイヤーの人数が１人
            await UniTask.WaitUntil(() => m_NetworkManager.readyCount == NetworkServer.connections.Count ||
                                            NetworkServer.connections.Count < 2);

            // readyCountのリセット
            m_NetworkManager.readyCount = 0;

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

///////////////// ▼クライアントの処理▼ ///////////////////////////////////////////////////////////////////////////////////////

        // ボードの情報が入るまで待機
        await UniTask.WaitUntil(() => board.data != null);

        // ボードの作成
        LayOutSquare(m_SquarePrefab, m_UnsetbleSquarePrefab, boardSize);

        // カメラの位置を移動
        float rate = Mathf.InverseLerp(GameSetting.instance.minBoardSize.y, GameSetting.instance.maxBoardSize.y, boardSize.y);
        Vector3 offset = new Vector3(boardSize.x / 2f, rate * m_FurthestCameraOffset.y, rate * m_FurthestCameraOffset.z);
        Camera.main.transform.position = Camera.main.transform.position.Offset(offset);

        // 背景の作成
        CreateBackGround(m_UnsetbleSquarePrefab, boardSize);

        // playersColorの中身が入るまで待機
        await UniTask.WaitUntil(() => GameSetting.instance.playersColor.Length > 0);

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
                newSquare.gameObject.name = "Square[" + x + "," + y + "]";
                newSquare.transform.parent = board.GetBoardData(x, y) == 0 ? m_SetableParent.transform : m_UnsetableParent.transform;
                newSquare.transform.position = new Vector3(x, -0.1f, -y);
                newSquare.transform.position = newSquare.transform.position;
                newSquare.transform.localScale = newSquare.transform.localScale;
            }
        }
    }

    /// <summary>盤面情報をみて足りていないブロックを補う</summary>
    /// <param name="newBoard">新しい盤面情報</param>
    /// <param name="oldBoard">古い盤面情報</param>
    /// <param name="settablePrefab">ブロックプレファブ</param>
    public void ComplementBoard(int[,] afterBoard, int[,] beforeBoard)
    {
        for (int y = 0; y < boardSize.y; ++y)
        {
            for (int x = 0; x < boardSize.x; ++x)
            {
                // 設置しない盤・設置できない盤の場合はcontinue
                if (afterBoard[x, y] <= 0) continue;

                // このターンでの[x, y]座標において、設置情報に変化がない場合はcontinue
                if (afterBoard[x, y] == beforeBoard[x, y]) continue;

                Debug.Log("X : " + x);
                Debug.Log("Y : " + y);

                Debug.Log("newBoard : " + afterBoard[x, y]);
                Debug.Log("oldBoard : " + beforeBoard[x, y]);

                // 一致しない値取得
                int index = afterBoard[x, y];

                // プレファブの生成
                GameObject newBlock = Instantiate(m_BlockPrefab);

                // 名前・親オブジェクト・座標・マテリアルを設定
                newBlock.gameObject.name = "Block[" + x + "," + y + "]";
                newBlock.transform.parent = m_SetableParent.transform;
                newBlock.transform.position = new Vector3(x, 0f, -y);
                newBlock.GetComponent<MeshRenderer>().material = m_BlockManager.m_SetBlockMaterials[index - 1];
                newBlock.transform.position = newBlock.transform.position;
                newBlock.transform.localScale = newBlock.transform.localScale;
                
            }
        }
    }

    /// <summary>重複除去</summary>
    /// <param name="setData"><b>タプルの設置データ</b>
    /// <br></br>player: プレイヤー番号
    /// <br></br>set: 設置するボード上の位置</param>
    public void RidDuplicate((int player, bool[,] set)[] setData)
    {
        int[,] newBoard = board.GetBoard();

        // Board -> bool[,]変換
        // ボードのサイズ分調べる
        for (int y = 0; y < setData[0].set.GetLength(1); y++)
        {
            for (int x = 0; x < setData[0].set.GetLength(0); x++)
            {
                // 座標に対して設置申請を出しているplayer番号を抽出
                int[] players = setData.Where(d => d.set[x, y]).Select(d => d.player).ToArray();

                // その座標の設置申請が2人以上だった場合は0を、 1人だった場合はplayer番号を代入
                switch (players.Length)
                {
                    case 0:                                 break;
                    case 1:  newBoard[x, y] = players[0];   break;
                    default: newBoard[x, y] = 0;            break;
                }
            }
        }

        // 判定後のボードデータを作る
        board.SetBoard(newBoard);

        // プレイヤー全員に判定後のボード情報を送信
        BoardData boardData = new BoardData { board = board };
        NetworkServer.SendToAll(boardData);
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

    /// <summary>サーバーボードデータ受信</summary>
    /// <param name="connection">サーバーに接続しているクライアント</param>
    /// <param name="receivedData">受信データ</param>
    void ServerReceivedBoardData(NetworkConnectionToClient connection, BoardData receivedData)
    {
        // 受信したデータをリストに追加
        boardsData.Add(receivedData);
    }

    /// <summary>クライアント側でボード情報を取得</summary>
    /// <param name="receivedData">受信データ</param>
    void ClientReceivedBoardData(BoardData receivedData)
    {
        // 前のターンの盤面情報を更新する
        if (board.data != null)
        {
            oldBoard = board;

            // 受信フラグをtrueにする
            isReceived = true;
        }

        // 新しい盤面情報を取得
        board = receivedData.board;

        // 盤面のサイズを取得
        boardSize = new Vector2Int(receivedData.board.width, receivedData.board.height);

        Debug.Log("board : " + board.data);
    }

    // 範囲内でランダムな座標を返す関数
    Vector2Int RandomVector2Int(Vector2Int min, Vector2Int max)
        => new Vector2Int(Random.Range(min.x, max.x), Random.Range(min.y, max.y));
}

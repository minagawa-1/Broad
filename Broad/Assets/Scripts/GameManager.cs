using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameManager : MonoBehaviour
{
    [Header("マス目用プレファブ")]
    [SerializeField] GameObject m_SquarePrefab = null;

    [Header("設置不可マス用プレファブ")]
    [SerializeField] GameObject m_UnsetbleSquarePrefab = null;

    [Header("カメラの最遠のときのOffset値")]
    [SerializeField] Vector3 m_FurthestCameraOffset;

    // 盤面管理オブジェクト
    GameObject m_BoardManagerObject = null;
    GameObject m_UnsetableParent = null;
    GameObject m_SetableParent = null;

    // 盤面
    [HideInInspector] public int[,] board;

    // 盤面のサイズ
    [HideInInspector] public Vector2Int boardSize;

    // マス目サイズ
    public Vector2 m_SquareSize;

    // Start is called before the first frame update
    void Start()
    {
        m_BoardManagerObject = new GameObject("BoardManager");
        m_SetableParent =  new GameObject("SetableSquares");
        m_UnsetableParent = new GameObject("UnsetableSquares");

        m_State = State.Placement;

        // 盤面管理オブジェクトを親にする
        m_SetableParent.transform.SetParent(m_BoardManagerObject.transform);
        m_UnsetableParent.transform.SetParent(m_BoardManagerObject.transform);

        // 盤面の設定
        SetupBoard();
    }

    

    // 盤面の設定
    void SetupBoard()
    {
        // ボードサイズをランダムに設定
        boardSize = RandomVector2Int(GameSetting.instance.minBoardSize, GameSetting.instance.maxBoardSize);

        // 盤面のサイズを渡す
        board = new int[boardSize.x, boardSize.y];

        // 設置不可マスの決定
        ShaveBoard(boardSize);

        // ボードの作成
        LayOutSquare(m_SquarePrefab, m_UnsetbleSquarePrefab, boardSize, m_SquareSize);

        // カメラの位置を移動
        float rate = Mathf.InverseLerp(GameSetting.instance.minBoardSize.y, GameSetting.instance.maxBoardSize.y, boardSize.y);
        Vector3 offset = new Vector3(boardSize.x / 2f, rate * m_FurthestCameraOffset.y, rate * m_FurthestCameraOffset.z);
        Camera.main.transform.position = Camera.main.transform.position.Offset(offset);

        // 背景の作成
        CreateBackGround(m_UnsetbleSquarePrefab, boardSize);

        // 設置可能マスのメッシュの結合処理
        StartCoroutine(CombineMeshes());

        // 範囲内でランダムな座標を返す関数内の関数
        Vector2Int RandomVector2Int(Vector2Int min, Vector2Int max)
            => new Vector2Int(Random.Range(min.x, max.x), Random.Range(min.y, max.y));

        // メッシュの結合処理
        IEnumerator CombineMeshes()
        {
            yield return new WaitForEndOfFrame();

            var combiner = GetComponent<MeshCombiner>();

            // 設置可能マスのメッシュ結合
            combiner.Combine(m_SetableParent.GetChildren(), "SetableBoard", m_BoardManagerObject.transform);

            // 設置不可マスと枠外背景のメッシュの結合
            GameObject[] background = GameObject.Find("Background").GetChildren();
            GameObject[] unsetable = m_UnsetableParent.GetChildren().Concat(background).ToArray();
            combiner.Combine(unsetable, "UnsetableBoard", m_BoardManagerObject.transform);

            // もういらないので消し飛ばす
            Destroy(m_SetableParent);
            Destroy(m_UnsetableParent);
        }
    }

    

    /// <summary>マス目を敷き詰める</summary>
    /// <param name="squarePrefab">プレファブ</param>
    /// <param name="unsetablePrefab">プレファブ</param>
    /// <param name="boardSize">盤面の縦横幅</param>
    /// <param name="squareSize">マスのサイズ</param>
    /// <param name="parentTransform">親のトランスフォーム</param>
    void LayOutSquare(GameObject setablePrefab, GameObject unsetablePrefab, Vector2Int boardSize, Vector2 squareSize)
    {
        // オブジェクトの生成
        // y軸方向の生成
        for (int y = 0; y < boardSize.y; y++)
        {
            // x軸方向の生成
            for (int x = 0; x < boardSize.x; x++)
            {
                // 設置状況の初期化
                if (board[x, y] > 0) board[x, y] = 0;

                // マス目生成
                GameObject newSquare = Instantiate(board[x, y] == 0 ? setablePrefab : unsetablePrefab);

                // 新しく生成したオブジェクトの名前・親・座標・スケールを設定する
                newSquare.gameObject.name       = "Square[" + x + "," + y + "]";
                newSquare.transform.parent      = board[x, y] == 0 ? m_SetableParent.transform : m_UnsetableParent.transform;
                newSquare.transform.position    = new Vector3( x, -0.1f, -y);
                newSquare.transform.position    = Multi(newSquare.transform.position, new Vector3(squareSize.x, 1f, squareSize.y));
                newSquare.transform.localScale  = Multi(newSquare.transform.localScale, new Vector3(squareSize.x, 1f, squareSize.y));
            }
        }
    }

    /// <summary>設置不可マスの決定</summary>
    /// <remarks>パーリンノイズで決定する</remarks>
    /// <param name="boardSize">ボードサイズ</param>
    void ShaveBoard(Vector2Int boardSize)
    {
        // パーリンノイズのシード値
        Vector2 seed = new Vector2(Random.value, Random.value) * 100f;

        for(int y = 0; y < boardSize.y; ++y)
        {
            for(int x = 0; x < boardSize.x; ++x)
            {
                // パーリンノイズのサンプリングをして設置不可マスにする確率を決める
                Vector2 value = new Vector2( x, y) * GameSetting.instance.perlinScale + seed;
                float perlinValue = Mathf.PerlinNoise(value.x, value.y);

                if (perlinValue >= GameSetting.instance.boardViability)
                {
                    // 設置不可にする
                    board[x, y] = -1;
                }
            }
        }
    }

    /// <summary> 背景生成 </summary>
    /// <param name="prefab">プレファブ</param>
    /// <param name="boardSize">ボードのサイズ</param>
    void CreateBackGround(GameObject prefab, Vector2Int boardSize)
    {
        // 親の生成
        GameObject parent           = new GameObject("Background");

        // 盤面の最大サイズ + 余白 の長さ
        const float size = 48f;

        // 左
        GameObject left             = Instantiate(prefab, parent.transform);
        left.name                   = "BackgroundLeft";
        left.transform.localScale   = new Vector3(size / 2f - boardSize.x / 2f, left.transform.localScale.y, size);
        left.transform.position     = new Vector3(-left.transform.localScale.x / 2f - 0.5f, -0.1f, -boardSize.y / 2f + 0.5f);

        // 右
        GameObject right            = Instantiate(left, parent.transform);
        right.name                  = "BackgroundRight";
        right.transform.position    = right.transform.position.Offset(x: boardSize.x + left.transform.localScale.x);

        // 上
        GameObject top              = Instantiate(prefab, parent.transform);
        top.name                    = "BackgroundTop";
        top.transform.localScale    = new Vector3(boardSize.x, top.transform.localScale.y, size / 2f - boardSize.y / 2f);
        top.transform.position      = new Vector3(boardSize.x / 2f - 0.5f, -0.1f, top.transform.localScale.z / 2 + 0.5f);

        // 下
        GameObject bottom           = Instantiate(top, parent.transform);
        bottom.name                 = "BackgroundBottom";
        top.transform.position      = bottom.transform.position.Offset(z: -boardSize.y - top.transform.localScale.z);
    }

    /// <summary>ベクトル同士の乗算</summary>
    /// <param name="v">ベクトル（複数設定可能）</param>
    /// <returns>乗算した奴に決まってんだろjk</returns>
    Vector3 Multi(params Vector3[] v)
    {
        for (int i = 1; i < v.Length; ++i)
            v[0] = new Vector3(v[0].x * v[i].x, v[0].y * v[i].y, v[0].z * v[i].z);

        return v[0];
    }
}

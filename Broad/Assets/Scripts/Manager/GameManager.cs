using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("マス目用プレファブ")]
    [SerializeField] GameObject m_SquarePrefab = null;

    [Header("設置不可マス用プレファブ")]
    [SerializeField] GameObject m_UnsetBleSquarePrefab = null;

    [Header("マス目の生存率")]
    [SerializeField, Range(0f, 1f)] float m_BoardViability = 0.8f;

    [Header("パーリンノイズの大きさ")]
    [SerializeField] float m_PerlinScale = 0.1f;

    [Header("盤面の最小、最大サイズ")]
    [SerializeField] Vector2Int m_MinBoardSize;
    [SerializeField] Vector2Int m_MaxBoardSize;

    [Header("ボードの親に設定したいオブジェクトのトランスフォーム")]
    [SerializeField] Transform m_SetableParentTransform = null;

    [Header("ボードの親に設定したいオブジェクトのトランスフォーム")]
    [SerializeField] Transform m_UnsetableParentTransform = null;

    // マス目サイズ
    [SerializeField] Vector2 m_SquareSize;


    // マス目の持つ情報
    public struct Square
    {
        // ゲームオブジェクト
        public GameObject       gameObject;
        // スプライトレンダラー
        public MeshRenderer     meshRenderer;
        // 座標：左上がVector2Int( 0, 0)
        public Vector2Int       position;
        // 状況（-1: 設置不可, 0：未設置, n: nPが設置済み)
        public int              state;

        // コンストラクタ
        public Square(GameObject gameObject, MeshRenderer meshRenderer, Vector2Int position, int state)
        {
            this.gameObject = gameObject;
            this.meshRenderer = meshRenderer;
            this.position = position;
            this.state = state;
        }
    }

    // 盤面
    Square[,] m_Board;

    // 盤面のサイズ
    Vector2Int m_BoardSize;

    // ボードマスのステータス
    int[] m_SquareState = { -1, 0, 1, 2, 3, 4 };

    // Start is called before the first frame update
    void Start()
    {
        // 盤面の設定
        SetupBoard();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 盤面の設定
    void SetupBoard()
    {
        // ボードサイズをランダムに設定
        m_BoardSize = RandomVector2Int(m_MinBoardSize, m_MaxBoardSize);

        // 盤面のサイズを渡す
        m_Board = new Square[m_BoardSize.x, m_BoardSize.y];

        // 設置不可マスの決定
        ShaveBoard(m_BoardSize);

        // ボードの作成
        LayOutSquare(m_SquarePrefab, m_UnsetBleSquarePrefab, m_BoardSize, m_SquareSize, m_SetableParentTransform);

        // 背景の作成
        CreateBackGround(m_UnsetBleSquarePrefab, m_BoardSize);
        
        // 範囲内でランダムな座標を返す関数内の関数
        Vector2Int RandomVector2Int(Vector2Int min, Vector2Int max)
            => new Vector2Int(Random.Range(min.x, max.x), Random.Range(min.y, max.y));
    }

    /// <summary>マス目を敷き詰める</summary>
    /// <param name="squarePrefab">プレファブ</param>
    /// <param name="unsetablePrefab">プレファブ</param>
    /// <param name="boardSize">盤面の縦横幅</param>
    /// <param name="squareSize">マスのサイズ</param>
    /// <param name="parentTransform">親のトランスフォーム</param>
    void LayOutSquare(GameObject setablePrefab, GameObject unsetablePrefab, Vector2Int boardSize, Vector2 squareSize, Transform parentTransform)
    {
        // オブジェクトの生成
        // y軸方向の生成
        for (int y = 0; y < boardSize.y; y++)
        {
            // x軸方向の生成
            for (int x = 0; x < boardSize.x; x++)
            {
                // 設置状況の初期化
                if (m_Board[x, y].state > 0) m_Board[x, y].state = 0;

                // マス目生成
                GameObject newSquare = Instantiate(m_Board[x, y].state == 0 ? setablePrefab : unsetablePrefab);

                // 新しく生成したオブジェクトの名前、親、座標を設定する
                newSquare.gameObject.name = "Square[" + x + "," + y + "]";
                newSquare.transform.parent = parentTransform;
                newSquare.transform.position = new Vector3((float)x - (float)boardSize.x / 2f + 0.5f, -0.1f, (float)y - (float)boardSize.y / 2f + 0.5f);
                newSquare.transform.position = Multi(newSquare.transform.position, new Vector3(squareSize.x, 1f, squareSize.y));
                newSquare.transform.localScale = Multi(newSquare.transform.localScale, new Vector3(squareSize.x, 1f, squareSize.y));

                // ボードにマス目情報を格納
                m_Board[x, y] = new Square(newSquare, newSquare.GetComponent<MeshRenderer>(), new Vector2Int(x, y), m_Board[x, y].state);
            }
        }
    }

    /// <summary> 設置不可マスに代わりのオブジェクトを設置する </summary>
    /// <param name="squarePrefab">プレファブ</param>
    /// <param name="boardSize">ボードのサイズ</param>
    /// <param name="squareSize">オブジェクトサイズ</param>
    /// <param name="parentTransform">親のトランスフォーム</param>
    void ShaveBoard(Vector2Int boardSize)
    {
        // パーリンノイズのシード値
        Vector2 seed = new Vector2(Random.value, Random.value) * 100f;

        for(int y = 0; y < boardSize.y; ++y)
        {
            for(int x = 0; x < boardSize.x; ++x)
            {
                // パーリンノイズのサンプリングをして設置不可マスにする確率を決める
                Vector2 value = new Vector2( x, y) * m_PerlinScale + seed;
                float perlinValue = Mathf.PerlinNoise(value.x, value.y);

                if (perlinValue >= m_BoardViability)
                {
                    // 設置不可にする
                    m_Board[x, y].state = -1;
                }
            }
        }
    }

    /// <summary> 背景生成 </summary>
    /// <param name="prefab">プレファブ</param>
    /// <param name="boardSize">ボードのサイズ</param>
    void CreateBackGround(GameObject prefab, Vector2Int boardSize)
    {
        // 盤面の最大サイズ + 余白 の長さ
        const float size = 48f;

        // 左
        GameObject left             = Instantiate(prefab);
        left.transform.localScale = new Vector3(size / 2f - m_BoardSize.x / 2f, left.transform.localScale.y, size);
        left.transform.position     = new Vector3(-m_BoardSize.x / 2f - left.transform.localScale.x / 2f, -0.1f, 0f);
        

        // 右
        GameObject right            = Instantiate(left);
        right.transform.position    = new Vector3(-right.transform.position.x, right.transform.position.y, right.transform.position.z);

        // 上
        GameObject top              = Instantiate(prefab);
        top.transform.localScale    = new Vector3(m_BoardSize.x, top.transform.localScale.y, size / 2f - m_BoardSize.y / 2f);
        top.transform.position      = new Vector3(0f, -0.1f, -m_BoardSize.y / 2f - top.transform.localScale.z / 2f);

        // 下
        GameObject under            = Instantiate(top);
        top.transform.position = new Vector3(top.transform.position.x, top.transform.position.y, -top.transform.position.z);
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

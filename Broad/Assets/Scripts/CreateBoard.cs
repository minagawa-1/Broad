using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateBoard : MonoBehaviour
{
    [Header("プレファブ(雛型)")]
    [SerializeField] GameObject m_SquarePrefab = null;

    [Header("ボードの生存率")]
    [SerializeField, Range(0f, 1f)] float m_BoardViability = 0.8f;
    [SerializeField] float m_PerlinScale = 0.1f;

    [Header("ランダム生成される盤面のサイズ")]
    [SerializeField] Vector2Int m_MinBoardSize;
    [SerializeField] Vector2Int m_MaxBoardSize;

    // 盤面で構成されるマスの情報
    public struct Square
    {
        public GameObject       gameObject;     // ゲームオブジェクト
        public SpriteRenderer   spriteRenderer; // スプライトレンダラー
        public Vector2Int       position;       // 位置 : 左上がVector2Int(0,0)
        public bool             setable;        // ブロックを設置可能なマスか

        // コンストラクタ
        public Square(GameObject gameObject, SpriteRenderer spriteRenderer, Vector2Int position, bool setable)
        {
            this.gameObject     = gameObject;
            this.spriteRenderer = spriteRenderer;
            this.position       = position;
            this.setable        = setable;
        }
    }

    // 盤面
    Square[,] m_Board;

    // 盤面のサイズ
    Vector2Int m_BoardSize;

    // Start is called before the first frame update
    void Start()
    {
        SetupBoard();
    }

    void SetupBoard()
    {
        // ボードのサイズを決定
        m_BoardSize = RandomVector2Int(m_MinBoardSize, m_MaxBoardSize);

        m_Board = new Square[m_BoardSize.x, m_BoardSize.y];

        // マスを敷き詰める
        LayOutSquare(m_SquarePrefab, m_BoardSize, new Vector2(0.8f, 0.8f));

        // パーリンノイズを使ってマスを消す作業を行う
        ShaveBoard(m_BoardSize);

        // 範囲内のランダムな座標を返す関数内関数
        Vector2Int RandomVector2Int(Vector2Int min, Vector2Int max)
             => new Vector2Int(Random.Range(min.x, max.x), Random.Range(min.y, max.y));
    }

    // マスを敷き詰める
    void LayOutSquare(GameObject prefab, Vector2Int size, Vector2 squareSize)
    {
        for (int y = 0; y < size.y; ++y) {
            for (int x = 0; x < size.x; ++x)
            {
                SpriteRenderer newObjectRenderer = Instantiate(prefab).GetComponent<SpriteRenderer>();

                // 名前・親・位置座標の設定
                newObjectRenderer.gameObject.name = "Square[" + x + ", " + y + "]";
                newObjectRenderer.transform.parent = transform;
                newObjectRenderer.transform.position = new Vector3((float)x - (float)size.x / 2f + 0.5f, 
                                                                   (float)y - (float)size.y / 2f + 0.5f) * squareSize;

                // マスのリストに格納
                m_Board[x, y] = new Square(newObjectRenderer.gameObject, newObjectRenderer, new Vector2Int(x, y), true);
            }
        }
    }

    // パーリンノイズによるボードの添削
    void ShaveBoard(Vector2Int size)
    {
        Vector2 seed = new Vector2(Random.value, Random.value) * 100f;       // パーリンノイズのシード値

        for (int y = 0; y < size.y; ++y) {
            for (int x = 0; x < size.x; ++x)
            {
                // パーリンノイズをサンプリングして、設置不可マスにする確率を決定
                Vector2 value = new Vector2(x, y) * m_PerlinScale + seed;
                float perlinValue = Mathf.PerlinNoise(value.x, value.y);

                // 確立に基づきマスを設置不可にする
                if (perlinValue >= m_BoardViability)
                {
                    m_Board[x, y].setable = false;
                    m_Board[x, y].spriteRenderer.enabled = false;
                }
            }
        }
    }
}

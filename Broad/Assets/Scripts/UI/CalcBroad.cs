using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CalcBroad : MonoBehaviour
{
    [Header("コンポーネント")]
    [SerializeField] Text m_PlayerText;
    [SerializeField] Text m_BroadValueText;
    [SerializeField] GameManager m_GameManager;
    [SerializeField] BlockManager m_BlockManager;

    // Start is called before the first frame update
    void Start()
    {
        m_PlayerText.text = "";

        for (int i = 0; i < GameSetting.instance.playerNum; ++i)
        {
            string colorCode = ColorUtility.ToHtmlStringRGB(GameSetting.instance.playerColors[i]);

            m_PlayerText.text += "<size=60><color=#" + colorCode + ">" + (i + 1) + " P</color> : </size>\n";
        }
    }

    // Update is called once per frame
    void Update()
    {
        int[] counts = Calc();

        m_BroadValueText.text = "";

        for (int i = 0; i < GameSetting.instance.playerNum; ++i)
            m_BroadValueText.text += counts[i] + " <size=50>㎡</size>\n";
    }

    int[] Calc()
    {
        int[] counts = new int[GameSetting.instance.playerNum];

        for (int i = 0; i < GameSetting.instance.playerNum; ++i)
            counts[i] = GetLargestArea(m_GameManager.board, i + 1);

        return counts;
    }

    /// <summary>指定されたプレイヤーの最大領域を取得</summary>
    /// <param name="board">盤面の配列</param>
    /// <param name="player">プレイヤー番号</param>
    /// <returns>最大領域のサイズ</returns>
    int GetLargestArea(int[,] board, int player)
    {
        int width = board.GetLength(0);
        int height = board.GetLength(1);

        int[] parent = new int[width * height]; // Union-Findの親配列
        int[] size = new int[width * height]; // 各グループのサイズを保持する配列

        for (int i = 0; i < parent.Length; i++) {
            parent[i] = i;
            size[i] = 1;
        }

        int maxArea = 0;

        (int x, int y)[] directions = { (-1, 0), (1, 0), (0, -1), (0, 1) };

        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++)
            {
                if (board[x, y] != player) continue;
                
                int index = y * width + x;

                foreach (var direction in directions)
                {
                    Vector2Int v = new Vector2Int(x + direction.x, y + direction.y);

                    if (new RectInt(0, 0, width, height).Contains(v) && board[v.x, v.y] == player)
                    {
                        int newIndex = v.y * width + v.x;
                        Union(parent, size, index, newIndex);
                    }
                }

                maxArea = Mathf.Max(maxArea, size[Find(parent, index)]);
            }
        }

        return maxArea;
    }

    /// <summary>指定した要素の親を探す</summary>
    /// <param name="parent">親を格納した配列</param>
    /// <param name="index">親を探す要素のインデックス</param>
    /// <returns>要素の親</returns>
    int Find(int[] parent, int index)
    {
        int root = index;

        while (root != parent[root]) root = parent[root];

        // パス圧縮: 直接親を設定
        for (int i = index; i != root;)
        {
            int next = parent[i];
            parent[i] = root;
            i = next;
        }

        return root;
    }

    /// <summary>2つの要素を指定したインデックスのグループとして結合</summary>
    /// <remarks>Union操作により、グループの結合とサイズの更新が行われる</remarks>
    /// <param name="parent">親を格納した配列</param>
    /// <param name="size">各グループのサイズを格納した配列</param>
    /// <param name="index1">結合する要素の1つ目のインデックス</param>
    /// <param name="index2">結合する要素の2つ目のインデックス</param>
    void Union(int[] parent, int[] size, int index1, int index2)
    {
        int root1 = Find(parent, index1);
        int root2 = Find(parent, index2);

        // 同一なら何もせずに終了
        if (root1 == root2) return;
        
        if (size[root1] < size[root2])
        {
            // root2をroot1の親として設定し、root2のサイズを増やす
            parent[root1] = root2;
            size[root2] += size[root1];
        }
        else
        {
            // root1をroot2の親として設定し、root1のサイズを増やす
            parent[root2] = root1;
            size[root1] += size[root2];
        }
    }
}

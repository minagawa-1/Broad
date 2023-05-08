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

        for (int i = 0; i < m_GameManager.playerNum; ++i)
        {
            string colorCode = ColorUtility.ToHtmlStringRGB(m_BlockManager.playerColors[i]);

            m_PlayerText.text += "<size=60><color=#" + colorCode + ">" + (i + 1) + " P</color> : </size>\n";
        }
    }

    // Update is called once per frame
    void Update()
    {
        int[] counts = Calc();

        m_BroadValueText.text = "";

        for (int i = 0; i < m_GameManager.playerNum; ++i)
            m_BroadValueText.text += counts[i] + " <size=50>㎡</size>\n";
    }

    int[] Calc()
    {
        int[] counts = new int[m_GameManager.playerNum];

        for (int i = 0; i < m_GameManager.playerNum; ++i)
            counts[i] = GetLargestArea(m_GameManager.m_Board, i + 1);

        return counts;
    }

    int GetLargestArea(int[,] board, int player)
    {
        int rows = board.GetLength(0);
        int cols = board.GetLength(1);

        int[] parent = new int[rows * cols]; // Union Findの親配列
        int[] size = new int[rows * cols]; // 各グループのサイズを保持する配列

        for (int i = 0; i < parent.Length; i++)
        {
            parent[i] = i;
            size[i] = 1;
        }

        int maxArea = 0;

        int[][] directions = new int[][] { new int[] { -1, 0 }, new int[] { 1, 0 }, new int[] { 0, -1 }, new int[] { 0, 1 } };

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                if (board[row, col] == player)
                {
                    int index = row * cols + col;
                    foreach (int[] direction in directions)
                    {
                        int newRow = row + direction[0];
                        int newCol = col + direction[1];

                        if (newRow >= 0 && newRow < rows && newCol >= 0 && newCol < cols && board[newRow, newCol] == player)
                        {
                            int newIndex = newRow * cols + newCol;
                            Union(parent, size, index, newIndex);
                        }
                    }

                    maxArea = Mathf.Max(maxArea, size[Find(parent, index)]);
                }
            }
        }

        return maxArea;
    }

    int Find(int[] parent, int index)
    {
        if (parent[index] != index)
            parent[index] = Find(parent, parent[index]); // パス圧縮

        return parent[index];
    }

    void Union(int[] parent, int[] size, int index1, int index2)
    {
        int root1 = Find(parent, index1);
        int root2 = Find(parent, index2);

        if (root1 != root2)
        {
            if (size[root1] < size[root2])
            {
                parent[root1] = root2;
                size[root2] += size[root1];
            }
            else
            {
                parent[root2] = root1;
                size[root1] += size[root2];
            }
        }
    }



}

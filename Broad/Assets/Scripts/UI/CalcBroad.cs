using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CalcBroad : MonoBehaviour
{
    [Header("�R���|�[�l���g")]
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
            m_BroadValueText.text += counts[i] + " <size=50>�u</size>\n";
    }

    int[] Calc()
    {
        int[] counts = new int[GameSetting.instance.playerNum];

        for (int i = 0; i < GameSetting.instance.playerNum; ++i)
            counts[i] = GetLargestArea(m_GameManager.board, i + 1);

        return counts;
    }

    /// <summary>�w�肳�ꂽ�v���C���[�̍ő�̈���擾</summary>
    /// <param name="board">�Ֆʂ̔z��</param>
    /// <param name="player">�v���C���[�ԍ�</param>
    /// <returns>�ő�̈�̃T�C�Y</returns>
    int GetLargestArea(int[,] board, int player)
    {
        int width = board.GetLength(0);
        int height = board.GetLength(1);

        int[] parent = new int[width * height]; // Union-Find�̐e�z��
        int[] size = new int[width * height]; // �e�O���[�v�̃T�C�Y��ێ�����z��

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

    /// <summary>�w�肵���v�f�̐e��T��</summary>
    /// <param name="parent">�e���i�[�����z��</param>
    /// <param name="index">�e��T���v�f�̃C���f�b�N�X</param>
    /// <returns>�v�f�̐e</returns>
    int Find(int[] parent, int index)
    {
        int root = index;

        while (root != parent[root]) root = parent[root];

        // �p�X���k: ���ڐe��ݒ�
        for (int i = index; i != root;)
        {
            int next = parent[i];
            parent[i] = root;
            i = next;
        }

        return root;
    }

    /// <summary>2�̗v�f���w�肵���C���f�b�N�X�̃O���[�v�Ƃ��Č���</summary>
    /// <remarks>Union����ɂ��A�O���[�v�̌����ƃT�C�Y�̍X�V���s����</remarks>
    /// <param name="parent">�e���i�[�����z��</param>
    /// <param name="size">�e�O���[�v�̃T�C�Y���i�[�����z��</param>
    /// <param name="index1">��������v�f��1�ڂ̃C���f�b�N�X</param>
    /// <param name="index2">��������v�f��2�ڂ̃C���f�b�N�X</param>
    void Union(int[] parent, int[] size, int index1, int index2)
    {
        int root1 = Find(parent, index1);
        int root2 = Find(parent, index2);

        // ����Ȃ牽�������ɏI��
        if (root1 == root2) return;
        
        if (size[root1] < size[root2])
        {
            // root2��root1�̐e�Ƃ��Đݒ肵�Aroot2�̃T�C�Y�𑝂₷
            parent[root1] = root2;
            size[root2] += size[root1];
        }
        else
        {
            // root1��root2�̐e�Ƃ��Đݒ肵�Aroot1�̃T�C�Y�𑝂₷
            parent[root2] = root1;
            size[root1] += size[root2];
        }
    }
}

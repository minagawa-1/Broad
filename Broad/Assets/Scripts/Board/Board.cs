using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Board
{
    public int[] data;   // �{�[�h���

    public int width;   // �{�[�h���
    public int height;   // �{�[�h���

    public Board(int width, int height)
    {
        this.width = width;
        this.height = height;

        data = new int[width * height];
    }

    public void Reset()
    {
        data = new int[width * height];
    }

    /// <summary>�ݒu�s�}�X�̌���</summary>
    /// <remarks>�p�[�����m�C�Y�Ō��肷��</remarks>
    /// <param name="boardSize">�{�[�h�T�C�Y</param>
    public void ShaveBoard()
    {
        // �p�[�����m�C�Y�̃V�[�h�l
        Vector2 seed = new Vector2(Random.value, Random.value) * 100f;

        for (int y = 0; y < width; ++y)
        {
            for (int x = 0; x < height; ++x)
            {
                // �p�[�����m�C�Y�̃T���v�����O�����Đݒu�s�}�X�ɂ���m�������߂�
                Vector2 value = new Vector2(x, y) * GameSetting.instance.perlinScale + seed;
                float perlinValue = Mathf.PerlinNoise(value.x, value.y);

                if (perlinValue >= GameSetting.instance.boardViability)
                {
                    // �ݒu�s�ɂ���
                    SetBoardData(-1, x, y);
                }
            }
        }
    }

    /// <summary>�񎟌��̃{�[�h�����擾</summary>
    public int[,] GetBoard()
    {
        int[,] deserializedBoard = new int[width, height];

        // 74(6,3) => 3, 6
        for (var i = 0; i < width * height; i++)
        {
            int x = i / deserializedBoard.GetLength(1);
            int y = i % deserializedBoard.GetLength(1);

            deserializedBoard[x, y] = data[i];
        }

        return deserializedBoard;
    }

    /// <summary>�񎟌��̃{�[�h����ݒ�</summary>
    public void SetBoard(int[,] deserializedBoard)
    {
        for (int y = 0; y < height; ++y)
            for (int x = 0; x < width; ++x)
                data[y * width + x] = deserializedBoard[x, y];
    }

    /// <summary>���W����{�[�h�����擾</summary>
    /// <param name="x">x���W</param>
    /// <param name="y">y���W</param>
    public int GetBoardData(int x, int y) => data[y * width + x];

    /// <summary>���W����{�[�h�����擾</summary>
    /// <param name="position">x���W</param>
    public int GetBoardData(Vector2Int position) => data[position.y * width + position.x];

    /// <summary>���W����{�[�h����ݒ�</summary>
    /// <param name="value">�{�[�h�̏�Ԓl</param>
    /// <param name="x">x���W</param>
    /// <param name="y">y���W</param>
    public void SetBoardData(int value, int x, int y) => data[y * width + x] = value;

    /// <summary>���W����{�[�h�����擾</summary>
    /// <param name="value">�{�[�h�̏�Ԓl</param>
    /// <param name="position">x���W</param>
    public void SetBoardData(int value, Vector2Int position) => data[position.y * width + position.x] = value;
}

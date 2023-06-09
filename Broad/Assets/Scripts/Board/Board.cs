using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Board
{
    public int[] data;   // ボード情報

    public int width;   // ボード情報
    public int height;   // ボード情報

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

    /// <summary>設置不可マスの決定</summary>
    /// <remarks>パーリンノイズで決定する</remarks>
    /// <param name="boardSize">ボードサイズ</param>
    public void ShaveBoard()
    {
        // パーリンノイズのシード値
        Vector2 seed = new Vector2(Random.value, Random.value) * 100f;

        for (int y = 0; y < width; ++y)
        {
            for (int x = 0; x < height; ++x)
            {
                // パーリンノイズのサンプリングをして設置不可マスにする確率を決める
                Vector2 value = new Vector2(x, y) * GameSetting.instance.perlinScale + seed;
                float perlinValue = Mathf.PerlinNoise(value.x, value.y);

                if (perlinValue >= GameSetting.instance.boardViability)
                {
                    // 設置不可にする
                    SetBoardData(-1, x, y);
                }
            }
        }
    }

    /// <summary>二次元のボード情報を取得</summary>
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

    /// <summary>二次元のボード情報を設定</summary>
    public void SetBoard(int[,] deserializedBoard)
    {
        for (int y = 0; y < height; ++y)
            for (int x = 0; x < width; ++x)
                data[y * width + x] = deserializedBoard[x, y];
    }

    /// <summary>座標からボード情報を取得</summary>
    /// <param name="x">x座標</param>
    /// <param name="y">y座標</param>
    public int GetBoardData(int x, int y) => data[y * width + x];

    /// <summary>座標からボード情報を取得</summary>
    /// <param name="position">x座標</param>
    public int GetBoardData(Vector2Int position) => data[position.y * width + position.x];

    /// <summary>座標からボード情報を設定</summary>
    /// <param name="value">ボードの状態値</param>
    /// <param name="x">x座標</param>
    /// <param name="y">y座標</param>
    public void SetBoardData(int value, int x, int y) => data[y * width + x] = value;

    /// <summary>座標からボード情報を取得</summary>
    /// <param name="value">ボードの状態値</param>
    /// <param name="position">x座標</param>
    public void SetBoardData(int value, Vector2Int position) => data[position.y * width + position.x] = value;
}

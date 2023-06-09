using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blocks
{
    // ブロックの形状
    public bool[,] shape;

    public Vector2Int position;

    public Vector2 center;

    /// <summary>コンストラクタ</summary>
    /// <param name="shape">形状データ</param>
    public Blocks(bool[,] shape, Vector2Int position)
    {
        this.shape = shape;

        // int型で位置を格納しているため、必ず整数が入る
        center = new Vector2(GetWidth() / 2, GetHeight() / 2);

        // 縦横が偶数なら完全に真ん中にする
        if (GetWidth() % 2 == 0 && GetHeight() % 2 == 0) center = center.Offset(-0.5f, -0.5f);

        this.position = position;
    }

    /// <summary>ブロックの数</summary>
    public int GetBlockNum()
    {
        int counter = 0;

        for (int y = 0; y < GetHeight(); ++y)
            for (int x = 0; x < GetWidth(); ++x)
                if (shape[x, y]) counter++;

        return counter;
    }

    /// <summary>ブロックスの横幅</summary>
    public int GetWidth() => shape.GetLength(0);

    /// <summary>ブロックスの縦幅</summary>
    public int GetHeight() => shape.GetLength(1);

    /// <summary>その位置に設置可能か</summary>
    /// <param name="board">ボード情報</param>
    /// <param name="player">プレイヤー番号</param>
    public bool IsSetable(Board board, int player)
    {
        for (int y = 0; y < GetHeight(); ++y)
        {
            for (int x = 0; x < GetWidth(); ++x)
            {
                // shapeの参照位置がfalseなら、次に進む
                if (!shape[x, y]) continue;

                // 配列外参照
                if (position.x + x < 0 || board.width <= position.x + x) return false;
                if (position.y + y < 0 || board.height <= position.y + y) return false;

                // 渡された座標のstateが-1もしくは同じプレイヤー番号ならfalse
                if (board.GetBoardData(position.x + x, position.y + y) == -1
                 || board.GetBoardData(position.x + x, position.y + y) == player) return false;
            }
        }

        return true;
    }

    /// <summary>左回転</summary>
    public bool[,] RotateLeft()
    {
        // 配列の中身を左回転させる
        bool[,] newShape = new bool[GetHeight(), GetWidth()];
        for (int y = 0; y < GetHeight(); y++)
            for (int x = 0; x < GetWidth(); x++)
                newShape[y, GetWidth() - x - 1] = shape[x, y];

        // centerを回転後の中心位置にする
        center = new Vector2(center.y, GetWidth() - center.x - 1);

        return shape = newShape;
    }

    /// <summary>右回転</summary>
    public bool[,] RotateRight()
    {
        // 配列の中身を右回転させる
        bool[,] newShape = new bool[GetHeight(), GetWidth()];
        for (int y = 0; y < GetHeight(); y++)
            for (int x = 0; x < GetWidth(); x++)
                newShape[GetHeight() - y - 1, x] = shape[x, y];

        // centerを回転後の中心位置にする
        center = new Vector2(GetHeight() - center.y - 1, center.x);

        return shape = newShape;
    }
}
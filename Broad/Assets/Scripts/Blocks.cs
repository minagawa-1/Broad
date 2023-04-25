using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blocks
{
    // ブロックの形状
    public bool[,] shape;

    /// <summary>コンストラクタ</summary>
    /// <param name="shape">形状データ</param>
    public Blocks(bool[,] shape)
    {
        this.shape = shape;
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
    /// <param name="blocks">ブロック情報</param>
    /// <param name="x">ｘ座標（ブロックスの左上基準）</param>
    /// <param name="y">ｙ座標（ブロックスの左上基準）</param>
    /// <param name="player">プレイヤー番号</param>
    public bool IsSetable(GameManager.Square[,] board, Blocks blocks, int x, int y, int player) => IsSetable(board, blocks, new Vector2Int(x, y), player);

    /// <summary>その位置に設置可能か</summary>
    /// <param name="board">ボード情報</param>
    /// <param name="blocks">ブロック情報</param>
    /// <param name="pos">座標（ブロックスの左上基準）</param>
    /// <param name="player">プレイヤー番号</param>
    public bool IsSetable(GameManager.Square[,] board, Blocks blocks, Vector2Int position, int player)
    {
        for (int y = 0; y < blocks.GetHeight(); ++y)
        {
            for (int x = 0; x < blocks.GetWidth(); ++x)
            {
                // shapeの参照位置がfalseなら、次に進む
                if (!blocks.shape[x, y]) continue;

                string boardLog = "board: (" + board.GetLength(0) + ", " + board.GetLength(1) + ")";
                string blockLog = "参照: (" + blocks.GetWidth() + ", " + blocks.GetHeight() + ")";

                Debug.Log(boardLog + "\n" + blockLog);

                // 渡された座標のstateが-1もしくは同じプレイヤー番号ならfalse
                if (board[position.x + x, position.y + y].state == -1
                 || board[position.x + x, position.y + y].state == player) return false;
            }
        }

        return true;
    }
}
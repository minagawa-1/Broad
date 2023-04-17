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
        int counter = 0; ;

        for(int y = 0; y < GetHeight(); ++y)
            for (int x = 0; x < GetWidth(); ++x)
                if(shape[x, y]) counter++;

        return counter;
    }

    /// <summary>ブロックスの横幅</summary>
    public int GetWidth() => shape.GetLength(1);

    /// <summary>ブロックスの縦幅</summary>
    public int GetHeight() => shape.GetLength(0);

    /// <summary>その位置に設置可能か</summary>
    /// <param name="board">ボード情報</param>
    /// <param name="x">ｘ座標（ブロックスの左上基準）</param>
    /// <param name="y">ｙ座標（ブロックスの左上基準）</param>
    bool IsSetable(GameManager.Square[,] board, int x, int y) => IsSetable(board, new Vector2Int(x, y));

    /// <summary>その位置に設置可能か</summary>
    /// <param name="board">ボード情報</param>
    /// <param name="pos">座標（ブロックスの左上基準）</param>
    bool IsSetable(GameManager.Square[,] board, Vector2Int pos)
    {
        // 設置可能か頑張って調べる処理を下に記述してね


        return true;
    }
}

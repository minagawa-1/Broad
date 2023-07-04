using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

[System.Serializable]
public class Blocks
{
    /// <summary>形状</summary>
    public bool[,] shape;

    public string serializedShape;

    /// <summary>盤面座標(左上基準)</summary>
    public Vector2Int position;

    /// <summary>中心位置・回転軸</summary>
    public Vector2 center;

    /// <summary>ブロックスの密度</summary>
    public float density;

    /// <summary>コンストラクタ</summary>
    /// <param name="shape">形状データ</param>
    public Blocks(bool[,] shape, Vector2Int position, float density)
    {
        this.shape = shape;
        this.density = density;

        // int型で位置を格納しているため、必ず整数が入る
        center = new Vector2(width / 2, height / 2);

        // 縦横が偶数なら完全に真ん中にする
        if (width % 2 == 0 && height % 2 == 0) center = center.Offset(-0.5f, -0.5f);

        this.position = position;
    }

    /// <summary>ブロックの数</summary>
    public int GetBlockNum()
    {
        int counter = 0;

        for (int y = 0; y < height; ++y)
            for (int x = 0; x < width; ++x)
                if (shape[x, y]) counter++;

        return counter;
    }

    /// <summary>ブロックスの横幅</summary>
    public int width => shape.GetLength(0);

    /// <summary>ブロックスの縦幅</summary>
    public int height => shape.GetLength(1);

    /// <summary>その位置に設置可能か</summary>
    /// <param name="board">ボード情報</param>
    /// <param name="player">プレイヤー番号</param>
    public bool IsSetable(Board board, int player)
    {
        for (int y = 0; y < height; ++y)
        {
            for (int x = 0; x < width; ++x)
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
        bool[,] newShape = new bool[height, width];
        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                newShape[y, width - x - 1] = shape[x, y];

        // centerを回転後の中心位置にする
        center = new Vector2(center.y, width - center.x - 1);

        return shape = newShape;
    }

    /// <summary>右回転</summary>
    public bool[,] RotateRight()
    {
        // 配列の中身を右回転させる
        bool[,] newShape = new bool[height, width];
        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                newShape[height - y - 1, x] = shape[x, y];

        // centerを回転後の中心位置にする
        center = new Vector2(height - center.y - 1, center.x);

        return shape = newShape;
    }

    /// <summary>ブロックスの画像を取得</summary>
    /// <param name="blockSprite">単一ブロックの画像</param>
    public Texture2D GetBlocksTexture(Sprite blockSprite)
    {
        // blockSpriteがnullなら、画像を設定せずに終了
        if (blockSprite == null) return null;

        // 単ブロックの画像サイズ
        var blockSize = new Vector2Int(blockSprite.texture.width, blockSprite.texture.height);

        // 最終的に返すTexure
        var texture = new Texture2D(width * blockSize.x, height * blockSize.y);

        // 背景色は透明にする
        Color32[] pixels = texture.GetPixels32();
        for (int i = 0; i < pixels.Length; i++) pixels[i] = Color.clear;
        texture.SetPixels32(pixels);

        // フィルターモードの設定
        texture.filterMode = FilterMode.Point;

        // 画像情報
        Color32[] spritePixels = blockSprite.texture.GetPixels32();

        int cnt = GetBlockNum();

        for (int y = 0; y < height; ++y)
        {
            for (int x = 0; x < width; ++x)
            {
                if (!shape[x, y]) continue;

                texture.SetPixels32(x * blockSize.x, y * blockSize.y, blockSize.x, blockSize.x, spritePixels);

                if (--cnt <= 0) break;
            }
        }

        // 画像情報を決定して格納
        texture.Apply();

        return texture;
    }

    public void DeserializeShape()
    {
        shape = JsonConvert.DeserializeObject<bool[,]>(serializedShape); 
    }
}
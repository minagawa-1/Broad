using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck
{
    /// <summary>デッキ</summary>
    public List<Blocks> deck { get; private set; }

    /// <summary>コンストラクタ</summary>
    /// <param name="multiBlocks">山札に入れるブロックス</param>
    public Deck(params Blocks[] multiBlocks)
    {
        deck = new List<Blocks>();

        deck.AddRange(multiBlocks);
    }

    public void AddBlocks(params Blocks[] blocks)
    {
        deck.AddRange(blocks);
    }

    /// <summary>山札からドロー</summary>
    /// <param name="index">取得するブロックス番号</param>
    /// <returns>ドローされたブロックス</returns>
    public Blocks Draw()
    {
        // 山札にブロックスがない場合はnullを返す
        if (deck.Count == 0) return null;

        // 山札の0番目を削除してreturn
        var blocks = deck[0];
        deck.RemoveAt(0);

        return blocks;
    }

    public void Shuffle() => deck.Shuffle();
}

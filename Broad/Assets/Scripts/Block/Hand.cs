using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>手札</summary>
public class Hand
{
    /// <summary>手札に入れられるブロックスの数</summary>
    public int maxHandBlocks { get; private set; }

    /// <summary>手札の配列情報</summary>
    public Blocks[] hand { get; private set; }

    /// <summary>デッキクラス</summary>
    public readonly Deck deck;

    /// <summary>コンストラクタ</summary>
    /// <param name="deck">ドローするためのデッキ</param>
    /// <param name="maxHandBlocks">手札の最大ブロックス数</param>
    public Hand(Deck deck, int maxHandBlocks = 4)
    {
        this.maxHandBlocks = maxHandBlocks;

        // デッキのコピーを作成して取得
        this.deck = deck;

        hand = new Blocks[maxHandBlocks];

        // 手札の数だけドローする
        for (int i = 0; i < maxHandBlocks; ++i) DrawAt();
    }

    /// <summary>山札から手札にドロー</summary>
    /// <param name="index">取得するブロックス番号</param>
    /// <returns>ドローされたブロックス</returns>
    public Blocks DrawAt()
    {
        // 手札から空いている場所を探す
        int[] blanks = FindBlank();

        // 手札が満杯ならエラーを吐いて終了
        if(blanks.Length == 0) Debug.LogError($"手札が満杯です。max: {hand.Length}");

        // 山札からドローしてreturn
        hand[blanks[0]] = deck.Draw();

        return hand[blanks[0]];
    }

    /// <summary>手札からブロックス情報を取得(場には出さない)</summary>
    /// <param name="index">取得するブロックス番号</param>
    public Blocks GetBlocksAt(int index)
    {
        // 配列外参照していればエラーを吐いて終了
        Debug.Assert(!hand.IsProtrude(index), $"配列外参照: hand[{index}] <- Length: {hand.Length}");

        return hand[index];
    }

    /// <summary>手札にブロックス情報を設定</summary>
    /// <param name="index">設定するブロックス番号</param>
    /// <param name="blocks">設定するブロックス情報</param>
    public void SetBlocksAt(int index, Blocks blocks)
    {
        // 配列外参照していればエラーを吐いて終了
        Debug.Assert(!hand.IsProtrude(index), $"配列外参照: hand[{index}] <- Length: {hand.Length}");

        hand[index] = blocks;
    }

    /// <summary>中身のない手札を探す</summary>
    /// <returns></returns>
    public int[] FindBlank()
    {
        List<int> nullIndices = new List<int>();

        for (int i = 0; i < hand.Length; i++)
            if (hand[i] == null)
                nullIndices.Add(i);

        return nullIndices.ToArray();
    }
}

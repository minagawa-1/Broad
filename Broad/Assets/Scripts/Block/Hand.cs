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
    private readonly Deck m_Deck;

    /// <summary>コンストラクタ</summary>
    /// <param name="deck">ドローするためのデッキ</param>
    /// <param name="maxHandBlocks">手札の最大ブロックス数</param>
    public Hand(Deck deck, int maxHandBlocks = 4)
    {
        this.maxHandBlocks = maxHandBlocks;

        // デッキのコピーを作成して取得
        m_Deck = new Deck(deck.deck.ToArray());

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
        Debug.Assert(blanks.Length > 0, $"手札が満杯です。max: {hand.Length}");

        // 山札からドローしてreturn
        hand[blanks[0]] = m_Deck.Draw();

        return hand[blanks[0]];
    }

    /// <summary>手札から場に出す</summary>
    /// <param name="index">取得するブロックス番号</param>
    /// <returns>場に出すブロックス</returns>
    public Blocks PlayAt(int index)
    {
        // 配列外参照していればエラーを吐いて終了
        Debug.Assert(!hand.IsProtrude(index), $"配列外参照: hand[{index}] <- Length: {hand.Length}");

        // 参照した手札にブロックスがない場合はエラーを吐いて終了
        Debug.Assert(!(hand[index] == null), $"{index}番目のブロックスがNullです");

        // 手札のindex番目を返して手札から削除
        var blocks = hand[index];
        hand[index] = null;
        return blocks;
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

    public int[] FindBlank()
    {
        List<int> nullIndices = new List<int>();

        for (int i = 0; i < hand.Length; i++)
            if (hand[i] == null)
                nullIndices.Add(i);

        return nullIndices.ToArray();
    }
}

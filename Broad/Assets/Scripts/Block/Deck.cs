using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck
{
    /// <summary>�f�b�L</summary>
    public List<Blocks> deck { get; private set; }

    /// <summary>�R���X�g���N�^</summary>
    /// <param name="multiBlocks">�R�D�ɓ����u���b�N�X</param>
    public Deck(params Blocks[] multiBlocks)
    {
        deck = new List<Blocks>();

        deck.AddRange(multiBlocks);
    }

    public void AddBlocks(params Blocks[] blocks)
    {
        deck.AddRange(blocks);
    }

    /// <summary>�R�D����h���[</summary>
    /// <param name="index">�擾����u���b�N�X�ԍ�</param>
    /// <returns>�h���[���ꂽ�u���b�N�X</returns>
    public Blocks Draw()
    {
        // �R�D�Ƀu���b�N�X���Ȃ��ꍇ��null��Ԃ�
        if (deck.Count == 0) return null;

        // �R�D��0�Ԗڂ��폜����return
        var blocks = deck[0];           
        deck.RemoveAt(0);

        return blocks;
    }

    public void Shuffle() => deck.Shuffle();
}

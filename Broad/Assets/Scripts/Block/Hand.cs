using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>��D</summary>
public class Hand
{
    public const int m_default_max_hand_blocks = 4;

    /// <summary>��D�ɓ������u���b�N�X�̐�</summary>
    public int maxHandBlocks { get; private set; }

    /// <summary>��D�̔z����</summary>
    public Blocks[] hand { get; private set; }

    /// <summary>�f�b�L�N���X</summary>
    private readonly Deck m_Deck;

    /// <summary>�R���X�g���N�^</summary>
    /// <param name="deck">�h���[���邽�߂̃f�b�L</param>
    /// <param name="maxHandBlocks">��D�̍ő�u���b�N�X��</param>
    public Hand(Deck deck, int maxHandBlocks = m_default_max_hand_blocks)
    {
        this.maxHandBlocks = maxHandBlocks;

        // �f�b�L�̃R�s�[���쐬���Ď擾
        m_Deck = new Deck(deck.deck.ToArray());

        hand = new Blocks[maxHandBlocks];

        // ��D�̐������h���[����
        for (int i = 0; i < maxHandBlocks; ++i) DrawAt();
    }

    /// <summary>�R�D�����D�Ƀh���[</summary>
    /// <param name="index">�擾����u���b�N�X�ԍ�</param>
    /// <returns>�h���[���ꂽ�u���b�N�X</returns>
    public Blocks DrawAt()
    {
        // ��D����󂢂Ă���ꏊ��T��
        int[] blanks = FindBlank();

        // ��D�����t�Ȃ�G���[��f���ďI��
        Debug.Assert(blanks.Length > 0, $"��D�����t�ł��Bmax: {hand.Length}");

        // �R�D����h���[����return
        hand[blanks[0]] = m_Deck.Draw();

        return hand[blanks[0]];
    }

    /// <summary>��D�����ɏo��</summary>
    /// <param name="index">�擾����u���b�N�X�ԍ�</param>
    /// <returns>��ɏo���u���b�N�X</returns>
    public Blocks PlayAt(int index)
    {
        // �z��O�Q�Ƃ��Ă���΃G���[��f���ďI��
        Debug.Assert(!hand.IsProtrude(index), $"�z��O�Q��: hand[{index}] <- Length: {hand.Length}");

        // �Q�Ƃ�����D�Ƀu���b�N�X���Ȃ��ꍇ�̓G���[��f���ďI��
        Debug.Assert(!(hand[index] == null), $"{index}�Ԗڂ̃u���b�N�X��{null}�ł�");

        // ��D��index�Ԗڂ�Ԃ��Ď�D����폜
        var blocks = hand[index];
        hand[index] = null;
        return blocks;
    }

    /// <summary>��D����u���b�N�X�����擾(��ɂ͏o���Ȃ�)</summary>
    /// <param name="index">�擾����u���b�N�X�ԍ�</param>
    public Blocks GetBlocksAt(int index)
    {
        // �z��O�Q�Ƃ��Ă���΃G���[��f���ďI��
        Debug.Assert(!hand.IsProtrude(index), $"�z��O�Q��: hand[{index}] <- Length: {hand.Length}");

        return hand[index];
    }

    /// <summary>��D�Ƀu���b�N�X����ݒ�</summary>
    /// <param name="index">�ݒ肷��u���b�N�X�ԍ�</param>
    /// <param name="blocks">�ݒ肷��u���b�N�X���</param>
    public void SetBlocksAt(int index, Blocks blocks)
    {
        // �z��O�Q�Ƃ��Ă���΃G���[��f���ďI��
        Debug.Assert(!hand.IsProtrude(index), $"�z��O�Q��: hand[{index}] <- Length: {hand.Length}");

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

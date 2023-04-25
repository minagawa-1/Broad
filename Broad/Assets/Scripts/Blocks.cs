using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blocks
{
    // �u���b�N�̌`��
    public bool[,] shape;

    /// <summary>�R���X�g���N�^</summary>
    /// <param name="shape">�`��f�[�^</param>
    public Blocks(bool[,] shape)
    {
        this.shape = shape;
    }

    /// <summary>�u���b�N�̐�</summary>
    public int GetBlockNum()
    {
        int counter = 0;

        for (int y = 0; y < GetHeight(); ++y)
            for (int x = 0; x < GetWidth(); ++x)
                if (shape[x, y]) counter++;

        return counter;
    }

    /// <summary>�u���b�N�X�̉���</summary>
    public int GetWidth() => shape.GetLength(0);

    /// <summary>�u���b�N�X�̏c��</summary>
    public int GetHeight() => shape.GetLength(1);

    /// <summary>���̈ʒu�ɐݒu�\��</summary>
    /// <param name="board">�{�[�h���</param>
    /// <param name="blocks">�u���b�N���</param>
    /// <param name="x">�����W�i�u���b�N�X�̍����j</param>
    /// <param name="y">�����W�i�u���b�N�X�̍����j</param>
    /// <param name="player">�v���C���[�ԍ�</param>
    public bool IsSetable(GameManager.Square[,] board, Blocks blocks, int x, int y, int player) => IsSetable(board, blocks, new Vector2Int(x, y), player);

    /// <summary>���̈ʒu�ɐݒu�\��</summary>
    /// <param name="board">�{�[�h���</param>
    /// <param name="blocks">�u���b�N���</param>
    /// <param name="pos">���W�i�u���b�N�X�̍����j</param>
    /// <param name="player">�v���C���[�ԍ�</param>
    public bool IsSetable(GameManager.Square[,] board, Blocks blocks, Vector2Int position, int player)
    {
        for (int y = 0; y < blocks.GetHeight(); ++y)
        {
            for (int x = 0; x < blocks.GetWidth(); ++x)
            {
                // shape�̎Q�ƈʒu��false�Ȃ�A���ɐi��
                if (!blocks.shape[x, y]) continue;

                string boardLog = "board: (" + board.GetLength(0) + ", " + board.GetLength(1) + ")";
                string blockLog = "�Q��: (" + blocks.GetWidth() + ", " + blocks.GetHeight() + ")";

                Debug.Log(boardLog + "\n" + blockLog);

                // �n���ꂽ���W��state��-1�������͓����v���C���[�ԍ��Ȃ�false
                if (board[position.x + x, position.y + y].state == -1
                 || board[position.x + x, position.y + y].state == player) return false;
            }
        }

        return true;
    }
}
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blocks
{
    // �u���b�N�̌`��
    public bool[,] shape;

    public Vector2 center;

    /// <summary>�R���X�g���N�^</summary>
    /// <param name="shape">�`��f�[�^</param>
    public Blocks(bool[,] shape)
    {
        this.shape = shape;

        //Debug.Log(GetWidth() % 2 != GetHeight() % 2);

        // int�^�ňʒu���i�[���Ă��邽�߁A�K������������
        center = new Vector2(GetWidth() / 2, GetHeight() / 2);

        // �c���������Ȃ犮�S�ɐ^�񒆂ɂ���
        if (GetWidth() % 2 == 0 && GetHeight() % 2 == 0) center = center.Offset(-0.5f, -0.5f);

        //Debug.Log("size: " + new Vector2(GetWidth(), GetHeight()) + "\ncenter: " + center);
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
    public bool IsSetable(int[,] board, int x, int y, int player) => IsSetable(board, new Vector2Int(x, y), player);

    /// <summary>���̈ʒu�ɐݒu�\��</summary>
    /// <param name="board">�{�[�h���</param>
    /// <param name="blocks">�u���b�N���</param>
    /// <param name="pos">���W�i�u���b�N�X�̍����j</param>
    /// <param name="player">�v���C���[�ԍ�</param>
    public bool IsSetable(int[,] board, Vector2Int position, int player)
    {
        for (int y = 0; y < GetHeight(); ++y)
        {
            for (int x = 0; x < GetWidth(); ++x)
            {
                // shape�̎Q�ƈʒu��false�Ȃ�A���ɐi��
                if (!shape[x, y]) continue;

                // �z��O�Q��
                if (position.x + x < 0 || board.GetLength(0) <= position.x + x) return false;
                if (position.y + y < 0 || board.GetLength(1) <= position.y + y) return false;

                // �n���ꂽ���W��state��-1�������͓����v���C���[�ԍ��Ȃ�false
                if (board[position.x + x, position.y + y] == -1
                 || board[position.x + x, position.y + y] == player) return false;
            }
        }

        return true;
    }

    /// <summary>����]</summary>
    public bool[,] RotateLeft()
    {
        // �z��̒��g������]������
        bool[,] newShape = new bool[GetHeight(), GetWidth()];
        for (int y = 0; y < GetHeight(); y++)
            for (int x = 0; x < GetWidth(); x++)
                newShape[y, GetWidth() - x - 1] = shape[x, y];

        // center����]��̒��S�ʒu�ɂ���
        center = new Vector2(center.y, GetWidth() - center.x - 1);

        return shape = newShape;
    }

    /// <summary>�E��]</summary>
    public bool[,] RotateRight()
    {
        // �z��̒��g���E��]������
        bool[,] newShape = new bool[GetHeight(), GetWidth()];
        for (int y = 0; y < GetHeight(); y++)
            for (int x = 0; x < GetWidth(); x++)
                newShape[GetHeight() - y - 1, x] = shape[x, y];

        // center����]��̒��S�ʒu�ɂ���
        center = new Vector2(GetHeight() - center.y - 1, center.x);

        return shape = newShape;
    }
}
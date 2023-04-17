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
        int counter = 0; ;

        for(int y = 0; y < GetHeight(); ++y)
            for (int x = 0; x < GetWidth(); ++x)
                if(shape[x, y]) counter++;

        return counter;
    }

    /// <summary>�u���b�N�X�̉���</summary>
    public int GetWidth() => shape.GetLength(1);

    /// <summary>�u���b�N�X�̏c��</summary>
    public int GetHeight() => shape.GetLength(0);

    /// <summary>���̈ʒu�ɐݒu�\��</summary>
    /// <param name="board">�{�[�h���</param>
    /// <param name="x">�����W�i�u���b�N�X�̍����j</param>
    /// <param name="y">�����W�i�u���b�N�X�̍����j</param>
    bool IsSetable(GameManager.Square[,] board, int x, int y) => IsSetable(board, new Vector2Int(x, y));

    /// <summary>���̈ʒu�ɐݒu�\��</summary>
    /// <param name="board">�{�[�h���</param>
    /// <param name="pos">���W�i�u���b�N�X�̍����j</param>
    bool IsSetable(GameManager.Square[,] board, Vector2Int pos)
    {
        // �ݒu�\���撣���Ē��ׂ鏈�������ɋL�q���Ă�


        return true;
    }
}

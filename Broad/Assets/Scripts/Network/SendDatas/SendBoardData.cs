using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public struct BoardData : NetworkMessage
{
    public Board board;   // �{�[�h���

    public BoardData(Board board)
    {
        this.board = board;
    }
}

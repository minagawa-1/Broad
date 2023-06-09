using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public struct BoardData : NetworkMessage
{
    public Board board;   // ƒ{[ƒhî•ñ

    public BoardData(Board board)
    {
        this.board = board;
    }
}

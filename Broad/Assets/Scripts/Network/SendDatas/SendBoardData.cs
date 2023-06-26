using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public struct BoardData : NetworkMessage
{
    public Board board;   // ボード情報
    public int   index;   // プレイヤー番号

    public BoardData(Board board, int index)
    {
        this.board = board;
        this.index = index;
    }
}

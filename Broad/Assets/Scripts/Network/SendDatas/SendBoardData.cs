using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public struct BoardData : NetworkMessage
{
    public Board board;   // ボード情報
    public int   player;   // プレイヤー番号

    public BoardData(Board board, int player = -1)
    {
        this.board = board;
        this.player = player;
    }
}

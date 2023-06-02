using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

/// <summary>プレイヤーデータ</summary>
public enum PlayerColor
{
    Red,
    Green,
    Blue,
    Yellow
}

[System.Serializable]
public struct PlayerData : NetworkMessage
{
    public int          index;          // プレイヤー番号
    //public Blocks       blocks;       // ブロック情報
    public Vector3      position;
    
    public PlayerData(int index, Vector3 position)
    {
        this.index = index;
        //this.blocks = blocks;
        this.position = position;
    }
}

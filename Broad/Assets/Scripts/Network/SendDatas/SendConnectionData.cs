using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[System.Serializable]
public struct ConnectionData : NetworkMessage
{
    public int playerCount;     // 接続しているプレイヤー数

    public ConnectionData(int count)
    {
        this.playerCount = count;
    }
}
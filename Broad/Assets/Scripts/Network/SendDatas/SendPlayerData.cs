using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[System.Serializable]
public struct PlayerData : NetworkMessage
{
    public int selfIndex;          // プレイヤー番号

    public PlayerData(int selfIndex)
    {
        this.selfIndex = selfIndex;
    }
}

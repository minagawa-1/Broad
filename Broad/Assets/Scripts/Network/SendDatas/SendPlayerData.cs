using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[System.Serializable]
public struct PlayerData : NetworkMessage
{
    public int selfIndex;          // プレイヤー番号
    public string name;            // プレイヤー名

    public PlayerData(int selfIndex, string name = "")
    {
        this.selfIndex = selfIndex;
        this.name = name;
    }
}

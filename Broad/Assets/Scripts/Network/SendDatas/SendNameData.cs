using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public struct NameData : NetworkMessage
{
    public string[] name;   // プレイヤー名配列

    public NameData(string[] name)
    {
        this.name = name;
    }
}

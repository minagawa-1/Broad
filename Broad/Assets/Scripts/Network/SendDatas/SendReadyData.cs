using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[System.Serializable]
public struct ReadyData : NetworkMessage
{
    public bool isReady;        // 準備完了フラグ(基本的にtrueを入れる)

    public ReadyData(bool isReady)
    {
        this.isReady = isReady;
    }
}

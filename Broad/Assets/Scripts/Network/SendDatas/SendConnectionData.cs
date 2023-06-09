using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[System.Serializable]
public struct ConnectionData : NetworkMessage
{
    public int playerCount;     // �ڑ����Ă���v���C���[��

    public ConnectionData(int count)
    {
        this.playerCount = count;
    }
}
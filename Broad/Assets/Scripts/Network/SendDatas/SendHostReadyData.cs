using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[System.Serializable]
public struct HostReadyData:NetworkMessage
{
    public bool isHostReady;    // �z�X�g�����t���O

    public HostReadyData(bool ready)
    {
        this.isHostReady = ready;
    }
}

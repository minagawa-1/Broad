using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[System.Serializable]
public struct ConnectionData : NetworkMessage
{
    public int playerCount;     // Ú‘±‚µ‚Ä‚¢‚éƒvƒŒƒCƒ„[”

    public ConnectionData(int count)
    {
        this.playerCount = count;
    }
}
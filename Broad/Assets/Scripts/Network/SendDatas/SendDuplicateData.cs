using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public struct DuplicateData : NetworkMessage
{
    public Vector2Int[] position;

    public DuplicateData(Vector2Int[] position)
    {
        this.position = position;
    }
}

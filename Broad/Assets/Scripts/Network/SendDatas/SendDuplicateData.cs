using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public struct DuplicateData : NetworkMessage
{
    public GameObject[] duplicates;

    public DuplicateData(GameObject[] duplicates)
    {
        this.duplicates = duplicates;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public struct ColorData : NetworkMessage
{
    public Color[] color;   // カラー

    public ColorData(Color[] color)
    {
        this.color = color;
    }
}

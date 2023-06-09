using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[System.Serializable]
public struct PlayerData : NetworkMessage
{
    public int   index;          // プレイヤー番号
    public Color color;          // プレイヤーカラー
    
    public PlayerData(int index, Color color)
    {
        this.index = index;
        this.color = color;
    }

    /// <summary>プレイヤー番号設定</summary>
    /// <param name="index">プレイヤー番号</param>
    public void SetIndex(int index) => this.index = index;

    /// <summary>プレイヤーカラー設定</summary>
    /// <param name="color">カラー</param>
    public void SetColor(Color color) => this.color = color;
}

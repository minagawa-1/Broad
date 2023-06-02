using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

/// <summary>�v���C���[�f�[�^</summary>
public enum PlayerColor
{
    Red,
    Green,
    Blue,
    Yellow
}

[System.Serializable]
public struct PlayerData : NetworkMessage
{
    public int          index;          // �v���C���[�ԍ�
    //public Blocks       blocks;       // �u���b�N���
    public Vector3      position;
    
    public PlayerData(int index, Vector3 position)
    {
        this.index = index;
        //this.blocks = blocks;
        this.position = position;
    }
}

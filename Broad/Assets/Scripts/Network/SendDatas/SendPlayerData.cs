using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public struct PlayerData : NetworkMessage
{
    public int   index;          // �v���C���[�ԍ�
    public Color color;          // �v���C���[�J���[
    
    public PlayerData(int index, Color color)
    {
        this.index = index;
        this.color = color;
    }

    /// <summary>�v���C���[�ԍ��ݒ�</summary>
    /// <param name="index">�v���C���[�ԍ�</param>
    public void SetIndex(int index) => this.index = index;

    /// <summary>�v���C���[�J���[�ݒ�</summary>
    /// <param name="color">�J���[</param>
    public void SetColor(Color color) => this.color = color;
}

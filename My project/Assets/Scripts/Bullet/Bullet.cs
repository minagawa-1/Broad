#if false
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet// : MonoBehaviour
{
    /// <summary>�R���X�g���N�^</summary>
    /// <param name="gameObject">�Q�[���I�u�W�F�N�g</param>
    /// <param name="moveSpeed">�ړ����x</param>
    /// <param name="direction">�ˏo����</param>
    public Bullet(GameObject gameObject, float moveSpeed, float direction)
    {
        this.gameObject = gameObject;
        transform = gameObject.transform;
        this.moveSpeed = moveSpeed;
        this.direction = direction;
    }

    public GameObject gameObject;
    public Transform transform;
    public float moveSpeed;
    public float direction;
    public woskni.Timer timer;
}
#endif
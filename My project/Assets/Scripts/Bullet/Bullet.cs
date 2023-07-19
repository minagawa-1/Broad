#if false
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet// : MonoBehaviour
{
    /// <summary>コンストラクタ</summary>
    /// <param name="gameObject">ゲームオブジェクト</param>
    /// <param name="moveSpeed">移動速度</param>
    /// <param name="direction">射出方向</param>
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
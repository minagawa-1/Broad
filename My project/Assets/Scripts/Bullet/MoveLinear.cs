using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveLinear : MoveBullet
{
    protected override void Move()
    {
        Vector3 v = Vector3.zero;

        v.x = Mathf.Cos(Mathf.Deg2Rad * m_Direction) * m_Speed;
        v.y = Mathf.Sin(Mathf.Deg2Rad * m_Direction) * m_Speed;

        m_MyTransform.position += v * Time.deltaTime;

        base.Move();
    }
}

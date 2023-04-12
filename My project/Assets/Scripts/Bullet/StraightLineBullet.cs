using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StraightLineBullet : MoveBullet
{
    protected override void Move()
    {
        Vector3 verocity = Vector3.zero;

        float x = Mathf.Cos(Mathf.Deg2Rad * m_Direction) * m_Speed;
        float y = Mathf.Sin(Mathf.Deg2Rad * m_Direction) * m_Speed;

        verocity.x = Mathf.Atan2(x, y);
        verocity.y = Mathf.Atan2(y, x);

        m_MyTransform.position += verocity * Time.deltaTime;

        base.Move();
    }
}

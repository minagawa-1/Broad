using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuckShot : MoveBullet
{
    private float m_ShakeAngle;

    public override void Initialize(BulletData.Bullet bullet, BulletPreset bullet_preset, BulletProcess bullet_process, Unit target, string layer_name, float direction = 0f)
    {
        base.Initialize(bullet, bullet_preset, bullet_process, target, layer_name, direction);

        // type‚ªˆá‚Á‚½‚ç‰½‚à‚µ‚È‚¢
        if (bulletPreset.type != BulletType.BuckShot) return;

        // shakePower‚ð“ü‚ê‚é
        m_ShakeAngle = bulletPreset.shakeAngle;

        float shake = Random.Range(-m_ShakeAngle, m_ShakeAngle);

        m_Direction += shake;
    }

    protected override void Move()
    {
        Vector3 vector  = Vector3.zero;
        vector.x        = Mathf.Cos(Mathf.Deg2Rad * m_Direction) * m_Speed;
        vector.y        = Mathf.Sin(Mathf.Deg2Rad * m_Direction) * m_Speed;

        m_MyTransform.position += vector * Time.deltaTime;

        base.Move();
    }
}

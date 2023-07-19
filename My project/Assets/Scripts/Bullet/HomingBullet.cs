using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingBullet : MoveBullet
{
    private Vector3 m_Velocity    = Vector3.zero;
    private float   m_HomingPower = 0.0f;


    public override void Initialize(BulletData.Bullet bullet, BulletPreset bullet_preset, BulletProcess bullet_process, Unit target, string layer_name, float direction = 0f)
    {
        base.Initialize(bullet, bullet_preset, bullet_process, target, layer_name, direction);

        // BulletData��PresetList����status����
        if (bulletPreset.type != BulletType.Homing) return;

        // status����presetIndex�ɑΉ�����homingPower��m_HomingPower�ɓ����
        m_HomingPower = bulletPreset.homingPower;
    }

    protected override void Move()
    {
        if (!m_Target) return;

        if (!m_Target.invincibleFlag)
        {
            // �ΏۂƂ̋������v�Z
            Vector3 targetVector = m_Target.transform.position - m_MyTransform.position;

            // ���g�̃x�N�g��
            Vector3 vector = Vector3.zero;
            vector.x = Mathf.Cos(m_Direction * Mathf.Deg2Rad) * m_Speed;
            vector.y = Mathf.Sin(m_Direction * Mathf.Deg2Rad) * m_Speed;

            // 2�̃x�N�g�����g���A�O�ς����߂�
            Vector3 cross = Vector3.Cross(vector, targetVector);

            // �ʒu�֌W�����]������������߂�
            if (cross.z < 0f) m_Direction -= m_HomingPower * Time.deltaTime;
            else              m_Direction += m_HomingPower * Time.deltaTime;

        }

        // �ړ��v�Z
        m_Velocity.x = Mathf.Cos(Mathf.Deg2Rad * m_Direction) * m_Speed;
        m_Velocity.y = Mathf.Sin(Mathf.Deg2Rad * m_Direction) * m_Speed;

        m_MyTransform.position += m_Velocity * Time.deltaTime;

        base.Move();
    }
}
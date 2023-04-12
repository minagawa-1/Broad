using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pendulum : MoveBullet
{
    // ��]�^�C�}�[
    private woskni.Timer    m_RotationRightTimer;
    private woskni.Timer    m_RotationLefghtTimer;
    
    // ��]�^�C��
    private float           m_RotationTime = 1.5f;

    // �p�x�ɉ��Z����l
    private float           m_AddRotation = 60f;

    public override void Initialize(BulletData.Bullet bullet, BulletPreset bullet_preset, BulletProcess bullet_process, Unit target, string layer_name, float direction = 0)
    {
        base.Initialize(bullet, bullet_preset, bullet_process, target, layer_name, direction);

        // type��Pendulum�Ŗ�����΃��^�[��
        if (bulletPreset.type != BulletType.Pendulum) return;

        //Timer�̃Z�b�g�A�b�v
        m_RotationRightTimer.Setup(m_RotationTime);
        m_RotationLefghtTimer.Setup(m_RotationTime);
    }

    protected override void Move()
    {
        if (m_RotationLefghtTimer.IsFinished())
        {
            m_RotationRightTimer.Update();

            // �ړ�����
            Vector3 vector = Vector3.zero;
            vector.x = Mathf.Cos(Mathf.Deg2Rad * woskni.Easing.InSine(m_RotationRightTimer.time, m_RotationRightTimer.limit, m_Direction, m_Direction + m_AddRotation)) * m_Speed;
            vector.y = Mathf.Sin(Mathf.Deg2Rad * m_Direction) * m_Speed;

            m_MyTransform.position += vector * Time.deltaTime;

            // Timer���I��������
            if (m_RotationRightTimer.IsFinished())
                // Timer�̃��Z�b�g
                m_RotationLefghtTimer.Reset();
        }
        else
        {
            m_RotationLefghtTimer.Update();

            // �ړ�����
            Vector3 vector = Vector3.zero;
            vector.x = Mathf.Cos(Mathf.Deg2Rad * woskni.Easing.InSine(m_RotationLefghtTimer.time, m_RotationLefghtTimer.limit, m_Direction, m_Direction - m_AddRotation)) * m_Speed;
            vector.y = Mathf.Sin(Mathf.Deg2Rad * m_Direction) * m_Speed;

            m_MyTransform.position += vector * Time.deltaTime;

            // Timer���I��������
            if (m_RotationLefghtTimer.IsFinished())
                // Timer�̃��Z�b�g
                m_RotationRightTimer.Reset();
        }

        base.Move();
    }
}

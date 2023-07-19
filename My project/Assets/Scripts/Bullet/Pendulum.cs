using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pendulum : MoveBullet
{
    // 回転タイマー
    private woskni.Timer    m_RotationRightTimer;
    private woskni.Timer    m_RotationLefghtTimer;
    
    // 回転タイム
    private float           m_RotationTime = 1.5f;

    // 角度に加算する値
    private float           m_AddRotation = 60f;

    public override void Initialize(BulletData.Bullet bullet, BulletPreset bullet_preset, BulletProcess bullet_process, Unit target, string layer_name, float direction = 0)
    {
        base.Initialize(bullet, bullet_preset, bullet_process, target, layer_name, direction);

        // typeがPendulumで無ければリターン
        if (bulletPreset.type != BulletType.Pendulum) return;

        //Timerのセットアップ
        m_RotationRightTimer.Setup(m_RotationTime);
        m_RotationLefghtTimer.Setup(m_RotationTime);
    }

    protected override void Move()
    {
        if (m_RotationLefghtTimer.IsFinished())
        {
            m_RotationRightTimer.Update();

            // 移動処理
            Vector3 vector = Vector3.zero;
            vector.x = Mathf.Cos(Mathf.Deg2Rad * woskni.Easing.InSine(m_RotationRightTimer.time, m_RotationRightTimer.limit, m_Direction, m_Direction + m_AddRotation)) * m_Speed;
            vector.y = Mathf.Sin(Mathf.Deg2Rad * m_Direction) * m_Speed;

            m_MyTransform.position += vector * Time.deltaTime;

            // Timerが終了したら
            if (m_RotationRightTimer.IsFinished())
                // Timerのリセット
                m_RotationLefghtTimer.Reset();
        }
        else
        {
            m_RotationLefghtTimer.Update();

            // 移動処理
            Vector3 vector = Vector3.zero;
            vector.x = Mathf.Cos(Mathf.Deg2Rad * woskni.Easing.InSine(m_RotationLefghtTimer.time, m_RotationLefghtTimer.limit, m_Direction, m_Direction - m_AddRotation)) * m_Speed;
            vector.y = Mathf.Sin(Mathf.Deg2Rad * m_Direction) * m_Speed;

            m_MyTransform.position += vector * Time.deltaTime;

            // Timerが終了したら
            if (m_RotationLefghtTimer.IsFinished())
                // Timerのリセット
                m_RotationRightTimer.Reset();
        }

        base.Move();
    }
}

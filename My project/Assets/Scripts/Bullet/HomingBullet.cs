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

        // BulletDataのPresetListからstatusを代入
        if (bulletPreset.type != BulletType.Homing) return;

        // statusからpresetIndexに対応したhomingPowerをm_HomingPowerに入れる
        m_HomingPower = bulletPreset.homingPower;
    }

    protected override void Move()
    {
        if (!m_Target) return;

        if (!m_Target.invincibleFlag)
        {
            // 対象との距離を計算
            Vector3 targetVector = m_Target.transform.position - m_MyTransform.position;

            // 自身のベクトル
            Vector3 vector = Vector3.zero;
            vector.x = Mathf.Cos(m_Direction * Mathf.Deg2Rad) * m_Speed;
            vector.y = Mathf.Sin(m_Direction * Mathf.Deg2Rad) * m_Speed;

            // 2つのベクトルを使い、外積を求める
            Vector3 cross = Vector3.Cross(vector, targetVector);

            // 位置関係から回転する向きを決める
            if (cross.z < 0f) m_Direction -= m_HomingPower * Time.deltaTime;
            else              m_Direction += m_HomingPower * Time.deltaTime;

        }

        // 移動計算
        m_Velocity.x = Mathf.Cos(Mathf.Deg2Rad * m_Direction) * m_Speed;
        m_Velocity.y = Mathf.Sin(Mathf.Deg2Rad * m_Direction) * m_Speed;

        m_MyTransform.position += m_Velocity * Time.deltaTime;

        base.Move();
    }
}
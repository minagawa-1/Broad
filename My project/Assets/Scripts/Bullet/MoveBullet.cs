using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBullet : MonoBehaviour
{
    public virtual void Initialize(BulletData.Bullet bullet, BulletPreset bulletPreset, BulletProcess bulletProcess, Unit target, string layerName, float direction = 0f)
    {
        m_BulletName                = bulletPreset.bullet;
        this.bulletPreset           = bulletPreset;
        this.bulletProcess          = bulletProcess;
        m_Target                    = target;
        m_LayerName                 = layerName;
        m_Speed                     = bulletProcess.speed;
        m_Direction                 = direction;
        m_RotateSpeed               = bullet.rotateSpeed;
        m_HitFlag                   = false;
        m_DeadTimer.Setup(bulletProcess.deadTime);
        m_MyTransform               = gameObject.transform;
    }

    // 弾の名前
    public BulletName   m_BulletName { get; private set; }
    // ターゲット
    public Unit         m_Target        = null;
    // レイヤー名
    public string       m_LayerName;
    // 速度
    public float        m_Speed         = 0f;
    // 角度
    public float        m_Direction     = 0f;

    public float        m_RotateSpeed   = 0f;
    // 死亡タイマー
    public woskni.Timer m_DeadTimer;
    // 当たり判定
    public bool         m_HitFlag;
    // bulletStatus用リスト
    protected BulletPreset    bulletPreset    = new BulletPreset();
    protected BulletProcess   bulletProcess   = new BulletProcess();
    protected Transform       m_MyTransform   = null;

    // Update is called once per frame
    void Update() => Move();

    protected virtual void Move()
    {
        // 回転しない場合は、m_Directionの方向を向く。
        if (m_RotateSpeed == 0f) m_MyTransform.rotation = Quaternion.Euler(0f, 0f, m_Direction - 90f);
        else m_MyTransform.rotation = Rotate(m_MyTransform.rotation, Quaternion.Euler(new Vector3 (0f, 0f, m_RotateSpeed * Time.deltaTime)));

        m_DeadTimer.Update();

        Hit();

        // 消滅時間に達したら初期位置に戻す
        if (m_DeadTimer.IsFinished() || m_HitFlag)
        {
            m_HitFlag = false;
            BulletManager.BulletUnactive(this);
        }
    }

    private void Hit()
    {
        //衝突相手が無敵時はreturn
        if (m_Target.invincibleFlag || m_Target.deadFlag) return;

        float distance = (m_Target.transform.position - m_MyTransform.position).magnitude;

        // ターゲットのサイズを半径で捉えた長さを取得
        float targetRadius = Mathf.Min(m_Target.m_TargetRenderer.bounds.size.x,
                                       m_Target.m_TargetRenderer.bounds.size.y) * 0.5f;
        float myRadius     = Mathf.Min(m_MyTransform.localScale.x,
                                       m_MyTransform.localScale.y) * 0.5f;

        if (distance <= targetRadius + myRadius)
        {
            // 衝突
            m_HitFlag = true;

            // 衝突相手にダメージ処理
            m_Target.Damage(1, m_Target.invincibleTime);
        }
    }

    Quaternion Rotate(Quaternion rotA, Quaternion rotB) => Quaternion.Euler(rotA.eulerAngles + rotB.eulerAngles);
}

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

    // �e�̖��O
    public BulletName   m_BulletName { get; private set; }
    // �^�[�Q�b�g
    public Unit         m_Target        = null;
    // ���C���[��
    public string       m_LayerName;
    // ���x
    public float        m_Speed         = 0f;
    // �p�x
    public float        m_Direction     = 0f;

    public float        m_RotateSpeed   = 0f;
    // ���S�^�C�}�[
    public woskni.Timer m_DeadTimer;
    // �����蔻��
    public bool         m_HitFlag;
    // bulletStatus�p���X�g
    protected BulletPreset    bulletPreset    = new BulletPreset();
    protected BulletProcess   bulletProcess   = new BulletProcess();
    protected Transform       m_MyTransform   = null;

    // Update is called once per frame
    void Update() => Move();

    protected virtual void Move()
    {
        // ��]���Ȃ��ꍇ�́Am_Direction�̕����������B
        if (m_RotateSpeed == 0f) m_MyTransform.rotation = Quaternion.Euler(0f, 0f, m_Direction - 90f);
        else m_MyTransform.rotation = Rotate(m_MyTransform.rotation, Quaternion.Euler(new Vector3 (0f, 0f, m_RotateSpeed * Time.deltaTime)));

        m_DeadTimer.Update();

        Hit();

        // ���Ŏ��ԂɒB�����珉���ʒu�ɖ߂�
        if (m_DeadTimer.IsFinished() || m_HitFlag)
        {
            m_HitFlag = false;
            BulletManager.BulletUnactive(this);
        }
    }

    private void Hit()
    {
        //�Փˑ��肪���G����return
        if (m_Target.invincibleFlag || m_Target.deadFlag) return;

        float distance = (m_Target.transform.position - m_MyTransform.position).magnitude;

        // �^�[�Q�b�g�̃T�C�Y�𔼌a�ő������������擾
        float targetRadius = Mathf.Min(m_Target.m_TargetRenderer.bounds.size.x,
                                       m_Target.m_TargetRenderer.bounds.size.y) * 0.5f;
        float myRadius     = Mathf.Min(m_MyTransform.localScale.x,
                                       m_MyTransform.localScale.y) * 0.5f;

        if (distance <= targetRadius + myRadius)
        {
            // �Փ�
            m_HitFlag = true;

            // �Փˑ���Ƀ_���[�W����
            m_Target.Damage(1, m_Target.invincibleTime);
        }
    }

    Quaternion Rotate(Quaternion rotA, Quaternion rotB) => Quaternion.Euler(rotA.eulerAngles + rotB.eulerAngles);
}

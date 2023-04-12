using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    [SerializeField] GameObject     m_BulletPrefab;
    [SerializeField] int            m_CreateBulletCount;
    static private List<BulletPool> m_BulletPools   = new List<BulletPool>();
    [SerializeField] BulletData     m_BulletData    = null;

    private void Awake()
    {
        // BulletPool������
        m_BulletPools.Clear();

        List<BulletData.Bullet> bulletData = m_BulletData.GetBulletDataList();

        // bulletData�̗v�f�����Q�b�g�R���|�[�l���g����
        for (int i = 0; i < bulletData.Count; ++i)
        {
            //�q�I�u�W�F�N�g�����
            GameObject child = new GameObject();

            child.name = "BulletPool[" + ((BulletName)i).ToString() + "]";

            //�e��ݒ肷��
            child.transform.SetParent(transform);

            // BulletPool��������悤�ɂ���
            BulletPool pool = child.AddComponent<BulletPool>();
            pool.Setup();

            // �v�[���ƃI�u�W�F�N�g�̐���
            pool.CreatePool((BulletName)i, m_BulletPrefab, bulletData[i].sprite, bulletData[i].scale, m_CreateBulletCount);

            //���X�g�ɓ����
            m_BulletPools.Add(pool);
        }
    }

    /// <summary>�e���ˏo</summary>
    /// <param name="target">�����蔻���T���Ώ�</param>
    /// <param name="layerName">�ˏo���ɐݒ肳��郌�C���[</param>
    /// <param name="position">�ʒu���W</param>
    /// <param name="direction">�ˏo����(0.0 to 360.0,---- 0.0: �^��, 90.0: ��)</param>
    /// <param name="presetIndex">�v���Z�b�g�ԍ�</param>
    public void Create( Unit target, string layerName, Vector3 position, float direction, BulletData.Bullet bullet, BulletPreset bulletPreset, BulletProcess bulletProcess)
    {
        // ���C���[���v���C���[�ł��G�ł��Ȃ�
        if (layerName != "Player" && layerName != "Enemy") return;

        // m_BulletData��null
        if (!m_BulletData) return;

        // presetIndex��type������
        BulletType bulletType = bulletPreset.type;

        // bulletStatus��name�ƈ�v����I�u�W�F�N�g������
        GameObject gameObject = m_BulletPools[(int)bulletPreset.bullet].GetPoolObject();

        gameObject.transform.position       = position;

        // �e�̃��C���[��ݒ�
        gameObject.SetLayer(layerName + "Bullet", true);

        // �O���f�[�V�������̃����_���ȐF���擾
        Color randomColor = bulletPreset.colorGradation.Evaluate(Random.value);

        gameObject.GetComponent<Renderer>().material.color = randomColor;

        MoveBullet moveBullet = null;

        // bulletType�ɑΉ�����script���A�^�b�`
        switch(bulletType)
        {
            case BulletType.Normal:
                moveBullet = gameObject.AddComponent<MoveLinear>();
                break;
            case BulletType.Homing:
                moveBullet = gameObject.AddComponent<HomingBullet>();
                break;
            case BulletType.Curve:
                moveBullet = gameObject.AddComponent<MoveLinear>();
                break;
            case BulletType.BuckShot:
                moveBullet = gameObject.AddComponent<BuckShot>();
                break;
            case BulletType.Pendulum:
                moveBullet = gameObject.AddComponent<Pendulum>();
                break;
        }

        // MoveBullet�Ɉ�����n�� (direction����������ɕς���)
        moveBullet.Initialize(bullet, bulletPreset, bulletProcess, target, layerName, direction + (bulletProcess.isLookTarget ? 0f : 90f));
    }

    // �e���A�N�e�B�u�ɂ���
    public static void BulletUnactive(MoveBullet bullet)
    {
        m_BulletPools[(int)bullet.m_BulletName].SetUnActive(bullet.gameObject);
    }
}

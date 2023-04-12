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
        // BulletPoolを消す
        m_BulletPools.Clear();

        List<BulletData.Bullet> bulletData = m_BulletData.GetBulletDataList();

        // bulletDataの要素数分ゲットコンポーネントする
        for (int i = 0; i < bulletData.Count; ++i)
        {
            //子オブジェクトを作る
            GameObject child = new GameObject();

            child.name = "BulletPool[" + ((BulletName)i).ToString() + "]";

            //親を設定する
            child.transform.SetParent(transform);

            // BulletPoolを入れられるようにする
            BulletPool pool = child.AddComponent<BulletPool>();
            pool.Setup();

            // プールとオブジェクトの生成
            pool.CreatePool((BulletName)i, m_BulletPrefab, bulletData[i].sprite, bulletData[i].scale, m_CreateBulletCount);

            //リストに入れる
            m_BulletPools.Add(pool);
        }
    }

    /// <summary>弾を射出</summary>
    /// <param name="target">当たり判定を探す対象</param>
    /// <param name="layerName">射出時に設定されるレイヤー</param>
    /// <param name="position">位置座標</param>
    /// <param name="direction">射出方向(0.0 to 360.0,---- 0.0: 真上, 90.0: 左)</param>
    /// <param name="presetIndex">プリセット番号</param>
    public void Create( Unit target, string layerName, Vector3 position, float direction, BulletData.Bullet bullet, BulletPreset bulletPreset, BulletProcess bulletProcess)
    {
        // レイヤーがプレイヤーでも敵でもない
        if (layerName != "Player" && layerName != "Enemy") return;

        // m_BulletDataがnull
        if (!m_BulletData) return;

        // presetIndexのtypeを入れる
        BulletType bulletType = bulletPreset.type;

        // bulletStatusのnameと一致するオブジェクトを入れる
        GameObject gameObject = m_BulletPools[(int)bulletPreset.bullet].GetPoolObject();

        gameObject.transform.position       = position;

        // 弾のレイヤーを設定
        gameObject.SetLayer(layerName + "Bullet", true);

        // グラデーション内のランダムな色を取得
        Color randomColor = bulletPreset.colorGradation.Evaluate(Random.value);

        gameObject.GetComponent<Renderer>().material.color = randomColor;

        MoveBullet moveBullet = null;

        // bulletTypeに対応したscriptをアタッチ
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

        // MoveBulletに引数を渡す (directionを左から上基準に変える)
        moveBullet.Initialize(bullet, bulletPreset, bulletProcess, target, layerName, direction + (bulletProcess.isLookTarget ? 0f : 90f));
    }

    // 弾を非アクティブにする
    public static void BulletUnactive(MoveBullet bullet)
    {
        m_BulletPools[(int)bullet.m_BulletName].SetUnActive(bullet.gameObject);
    }
}

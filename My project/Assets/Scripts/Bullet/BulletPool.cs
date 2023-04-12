using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    //! プールに入れるプレハブ
    private GameObject   m_PoolPrefab;
    //アクティブなオブジェクトの親
    private GameObject   m_ActiveObject             = null;
    //非アクティブなオブジェクトの親
    private GameObject   m_UnactiveObject           = null;
    //アクティブなオブジェクトの親のトランスフォーム
    private Transform    m_ActiveObjectTransform    = null;
    //非アクティブなオブジェクトの親のトランスフォーム
    private Transform    m_UnactiveObjectTransform  = null;
    //弾幕画像
    private Sprite       m_BulletSprite             = null;
    //弾幕拡大率
    private Vector2      m_BulletScale              = Vector2.one;
    // 弾の種類
    private BulletName   m_BulletName               = BulletName.Small;

    // セットアップ
    public void Setup()
    {
        // 非待機オブジェクトを設定
        m_ActiveObject = new GameObject();
        m_ActiveObject.transform.SetParent(transform);
        m_ActiveObject.name = "ActiveObjects";
        m_ActiveObjectTransform = m_ActiveObject.transform;

        // 待機オブジェクトを設定
        m_UnactiveObject = new GameObject();
        m_UnactiveObject.transform.SetParent(transform);
        m_UnactiveObject.name = "UnactiveObjects";
        m_UnactiveObjectTransform = m_UnactiveObject.transform;
    }

    // オブジェクトプールの作成
    public void CreatePool(BulletName bulletName, GameObject poolPrefab, Sprite sprite, Vector2 scale, int poolSize)
    {
        m_BulletName        = bulletName;
        m_BulletScale       = scale;
        m_BulletSprite      = sprite;
        m_PoolPrefab        = poolPrefab;

        // poolSize分オブジェクトを生成
        for(int i = 0; i < poolSize; ++i)
            CreateNewPoolObject();
    }

    public GameObject GetPoolObject()
    {
        //待機オブジェクトがあるか調べる
        if (m_UnactiveObjectTransform.childCount > 0)
        {
            Transform child = m_UnactiveObjectTransform.GetChild(0);
            child.GetComponent<SpriteRenderer>().sprite = m_BulletSprite;
            child.gameObject.SetActive(true);
            child.SetParent(m_ActiveObjectTransform);
            return child.gameObject;
        }

        // 全部使っていたら、新しく生成して返す
        GameObject newObject = CreateNewPoolObject();

        newObject.SetActive(true);
        newObject.transform.SetParent(m_ActiveObjectTransform);
        return newObject;
    }

    public GameObject SetUnActive(GameObject poolPrefab)
    {
        // UnActiveObjectの子供にする
        poolPrefab.transform.SetParent(m_UnactiveObjectTransform);

        //アタッチされているコンポーネントを削除する
        Destroy(poolPrefab.GetComponent<MoveBullet>());

        // poolPrefabのマテリアルカラーをWhiteにする
        poolPrefab.GetComponent<Renderer>().material.color = new Color32(255, 255, 255, 255);

        // poolPrefabを非アクティブにする
        poolPrefab.SetActive(false);

        return poolPrefab;
    }

    // プールに入れるオブジェクトの生成
    private GameObject CreateNewPoolObject()
    {
        GameObject newObject = Instantiate(m_PoolPrefab, m_UnactiveObjectTransform);

        newObject.transform.localScale = m_BulletScale;
        newObject.GetComponent<SpriteRenderer>().sprite = m_BulletSprite;
        newObject.SetActive(false);

        // Bulletが小弾
        if (m_BulletName == BulletName.Small)
            newObject.GetComponent<SpriteRenderer>().material.SetFloat("_OutlineSize", 8f);

        return newObject;
    }
}

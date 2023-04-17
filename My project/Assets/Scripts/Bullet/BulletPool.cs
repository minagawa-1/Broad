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

    /// <summary> セットアップ </summary>
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

    /// <summary>                   プールの作成 </summary>
    /// <param name="bulletName">   弾幕名 </param>
    /// <param name="poolPrefab">   生成したいプレファブ </param>
    /// <param name="sprite">       アタッチしたい画像 </param>
    /// <param name="scale">        拡大率 </param>
    /// <param name="poolSize">     プールの大きさ </param>
    public void CreatePool(BulletName bulletName, GameObject poolPrefab, Sprite sprite, Vector2 scale, int poolSize)
    {
        // 弾幕名・プレファブ・画像・拡大率をそれぞれ設定
        m_BulletName        = bulletName;
        m_PoolPrefab        = poolPrefab;
        m_BulletSprite      = sprite;
        m_BulletScale       = scale;

        // poolSize分オブジェクトを生成
        for(int i = 0; i < poolSize; ++i)
            CreateNewPoolObject();
    }

    /// <summary> プール内のオブジェクト取得 </summary>
    /// <returns> プールオブジェクト </returns>
    public GameObject GetPoolObject()
    {
        //待機オブジェクトがあるか調べる
        if (m_UnactiveObjectTransform.childCount > 0)
        {
            // 待機オブジェクトの0番を取得
            Transform child = m_UnactiveObjectTransform.GetChild(0);

            // 画像・アクティブ・親の設定をする
            child.GetComponent<SpriteRenderer>().sprite = m_BulletSprite;
            child.gameObject.SetActive(true);
            child.SetParent(m_ActiveObjectTransform);

            // オブジェクトを返す
            return child.gameObject;
        }

        // 待機オブジェクトがいなかったら、新しく生成
        GameObject newObject = CreateNewPoolObject();

        // アクティブ・親の設定
        newObject.SetActive(true);
        newObject.transform.SetParent(m_ActiveObjectTransform);

        // 生成したオブジェクトを返す
        return newObject;
    }

    /// <summary>                   使い終わったオブジェクトを非アクティブ化する </summary>
    /// <param name="poolPrefab">   非アクティブ化したいオブジェクト </param>
    /// <returns>                   オブジェクト </returns>
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

        // プレファブを返す
        return poolPrefab;
    }

    /// <summary> オブジェクト生成 </summary>
    /// <returns> 生成したオブジェクト </returns>
    private GameObject CreateNewPoolObject()
    {
        // オブジェクトの生成
        GameObject newObject = Instantiate(m_PoolPrefab, m_UnactiveObjectTransform);

        // 拡大率・画像・アクティブの設定
        newObject.transform.localScale = m_BulletScale;
        newObject.GetComponent<SpriteRenderer>().sprite = m_BulletSprite;
        newObject.SetActive(false);

        // Bulletが小弾
        if (m_BulletName == BulletName.Small)
            // マテリアルの設定
            newObject.GetComponent<SpriteRenderer>().material.SetFloat("_OutlineSize", 8f);

        // 生成したオブジェクトを返す
        return newObject;
    }
}

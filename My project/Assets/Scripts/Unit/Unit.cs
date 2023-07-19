using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public  class Unit : MonoBehaviour
{
    virtual public string unitLayer => "Enemy";

    [woskni.Name("HP")                                ] public       int                 stock;                     //残機

    [woskni.Name("死亡フラグ")                        ] public       bool                deadFlag;                  //死亡フラグ

    [woskni.Name("弾発射間隔")        , SerializeField] protected    float               m_CreateInterval = 0.25f;  //発射間隔

    [woskni.Name("ターゲットプレハブ"), SerializeField] protected    Unit                m_Target = null;           //相手ユニット

    [woskni.Name("無敵フラグ")                        ] public       bool                invincibleFlag;            //無敵フラグ

    [woskni.Name("無敵時間")                          ] public       float               invincibleTime;            // 無敵時間

    [woskni.Name("弾データ")          , SerializeField] protected    BulletData          m_BulletData;

    protected List<woskni.Timer>    m_CreateTimerList       = new List<woskni.Timer>();         // 発射タイマー
    protected List<woskni.Timer>    m_GimmickTimerList      = new List<woskni.Timer>();         // ギミック用タイマー

    public SpriteRenderer           m_SpriteRenderer;                                           //スプライトレンダラー
    public SpriteRenderer           m_ParentRenderer;                                           //スプライトレンダラー
    public SpriteRenderer           m_TargetRenderer;                                           //スプライトレンダラー
    protected BulletManager         m_BulletManager         = null;

    protected List<float>           m_DirectionList         = new List<float>();                // 放出角度リスト

    // Start is called before the first frame update
    protected virtual void Start()
    {
        m_SpriteRenderer    = GetComponent<SpriteRenderer>();
        m_ParentRenderer    = transform.parent.GetComponent<SpriteRenderer>();
        m_BulletManager     = GameObject.Find("BulletManager").GetComponent<BulletManager>();

        SetRendererColorAlpha(0.75f);

        invincibleFlag = false;

        deadFlag = false;
    }

    public virtual void Damage(int damage, float invincibleTime)
    {
        const float deadCoverTime = 2f;

        stock -= damage;

        // 死亡判定
        if(stock <= 0 && !deadFlag) {
            stock = 0;
            deadFlag = true;
        }

        // 生きていればコルーチン
        if (!deadFlag && invincibleTime > 0f)
            StartCoroutine(Invincible(invincibleTime));

        // 死亡処理
        if (deadFlag) StartCoroutine(Dead(deadCoverTime));
    }

    protected IEnumerator Invincible(float invincibleTime)
    {
        // 普通に処理/無敵にする処理
        invincibleFlag = true;
        SetRendererColorAlpha(0.25f);

        yield return new WaitForSeconds(invincibleTime);

        // 3秒後の処理/無敵解除
        invincibleFlag = false;
        SetRendererColorAlpha(0.75f);
    }

    protected IEnumerator Dead(float waitTime)
    {
        // 同フレームで敵が先に倒れていたら何もせず終了
        if (m_Target.deadFlag) yield break;

        yield return new WaitForSeconds(waitTime);

        SceneManager.LoadScene(woskni.Scenes.Result.ToString(), LoadSceneMode.Additive);
    }

    /// <summary>透明度設定</summary>
    /// <param name="alpha">透明度</param>
    void SetRendererColorAlpha(float alpha)
    {
        m_SpriteRenderer.color = GetAlphaColor(m_SpriteRenderer, alpha);
        m_ParentRenderer.color = GetAlphaColor(m_ParentRenderer, alpha);
    }

    Color GetAlphaColor(SpriteRenderer renderer, float alpha) => new Color(renderer.color.r, renderer.color.g, renderer.color.b, alpha);
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public  class Unit : MonoBehaviour
{
    virtual public string unitLayer => "Enemy";

    [woskni.Name("HP")                                ] public       int                 stock;                     //�c�@

    [woskni.Name("���S�t���O")                        ] public       bool                deadFlag;                  //���S�t���O

    [woskni.Name("�e���ˊԊu")        , SerializeField] protected    float               m_CreateInterval = 0.25f;  //���ˊԊu

    [woskni.Name("�^�[�Q�b�g�v���n�u"), SerializeField] protected    Unit                m_Target = null;           //���胆�j�b�g

    [woskni.Name("���G�t���O")                        ] public       bool                invincibleFlag;            //���G�t���O

    [woskni.Name("���G����")                          ] public       float               invincibleTime;            // ���G����

    [woskni.Name("�e�f�[�^")          , SerializeField] protected    BulletData          m_BulletData;

    protected List<woskni.Timer>    m_CreateTimerList       = new List<woskni.Timer>();         // ���˃^�C�}�[
    protected List<woskni.Timer>    m_GimmickTimerList      = new List<woskni.Timer>();         // �M�~�b�N�p�^�C�}�[

    public SpriteRenderer           m_SpriteRenderer;                                           //�X�v���C�g�����_���[
    public SpriteRenderer           m_ParentRenderer;                                           //�X�v���C�g�����_���[
    public SpriteRenderer           m_TargetRenderer;                                           //�X�v���C�g�����_���[
    protected BulletManager         m_BulletManager         = null;

    protected List<float>           m_DirectionList         = new List<float>();                // ���o�p�x���X�g

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

        // ���S����
        if(stock <= 0 && !deadFlag) {
            stock = 0;
            deadFlag = true;
        }

        // �����Ă���΃R���[�`��
        if (!deadFlag && invincibleTime > 0f)
            StartCoroutine(Invincible(invincibleTime));

        // ���S����
        if (deadFlag) StartCoroutine(Dead(deadCoverTime));
    }

    protected IEnumerator Invincible(float invincibleTime)
    {
        // ���ʂɏ���/���G�ɂ��鏈��
        invincibleFlag = true;
        SetRendererColorAlpha(0.25f);

        yield return new WaitForSeconds(invincibleTime);

        // 3�b��̏���/���G����
        invincibleFlag = false;
        SetRendererColorAlpha(0.75f);
    }

    protected IEnumerator Dead(float waitTime)
    {
        // ���t���[���œG����ɓ|��Ă����牽�������I��
        if (m_Target.deadFlag) yield break;

        yield return new WaitForSeconds(waitTime);

        SceneManager.LoadScene(woskni.Scenes.Result.ToString(), LoadSceneMode.Additive);
    }

    /// <summary>�����x�ݒ�</summary>
    /// <param name="alpha">�����x</param>
    void SetRendererColorAlpha(float alpha)
    {
        m_SpriteRenderer.color = GetAlphaColor(m_SpriteRenderer, alpha);
        m_ParentRenderer.color = GetAlphaColor(m_ParentRenderer, alpha);
    }

    Color GetAlphaColor(SpriteRenderer renderer, float alpha) => new Color(renderer.color.r, renderer.color.g, renderer.color.b, alpha);
}

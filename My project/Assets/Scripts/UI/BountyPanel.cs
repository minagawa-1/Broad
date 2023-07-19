using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//�o�E���e�B�p�l���N���X
public class BountyPanel : MonoBehaviour
{
    /// <summary>�t�B�[���h�摜�\����</summary>
    [System.Serializable]
    public struct StageImage
    {
        public Image BackGround;  //�w�i(�t�B�[���h)
        public Image EnemyImage;  //�G�C���[�W
    }

    [Header("�R���|�[�l���g")]
    [SerializeField] TextMeshProUGUI    m_BountyNameText        = null;             //�o�E���e�B��
    [SerializeField] StageImage         m_StageImage            = new StageImage(); //�t�B�[���h�摜
    [SerializeField] TextMeshProUGUI    m_FieldNameText         = null;             //�t�B�[���h��
    [SerializeField] TextMeshProUGUI    m_BonusWeaponText       = null;             //�{�[�i�X����
    [SerializeField] TextMeshProUGUI    m_PointAmmountText      = null;             //�|�C���g��
    [SerializeField] TextMeshProUGUI    m_LifeAmmountText       = null;             //���C�t��
    [SerializeField] Color              m_IncompletionColor     = Color.white;      //�|���Ă��Ȃ��G�̐F
    [SerializeField] RectTransform      m_CompletionImage       = null;             //�o�E���e�B�B���摜
    [SerializeField] Image              m_MaskImage             = null;             //�}�X�N�摜
    [SerializeField, Range(0.01f, 1.0f)]
                     float              m_MaxMaskAlpha          = 0.4f;             //�}�X�N�摜�̍ő哧���x
    [SerializeField, Range(0.01f, 0.1f)]
                     float              m_MaxAlphaDelta         = 0.05f;            //�����x�̍ő�ω���
    [SerializeField, Range(1.0f , 1.5f)]
                     float              m_MaxScale              = 1.1f;             //�ő�X�P�[���l
    [SerializeField, Range(0.01f, 0.1f)]
                     float              m_MaxScaleDelta         = 0.01f;            //�X�P�[���l�̍ő�ω���
    [Space(12)]
    [SerializeField] BountyNameData     m_BountyNameData        = null;             //�o�E���e�B���f�[�^
    [SerializeField] FieldData          m_FieldData             = null;             //�t�B�[���h�f�[�^
    [SerializeField] EnemyData          m_EnemyData             = null;             //�G�l�~�[�f�[�^
    [SerializeField] WeaponData         m_WeaponData            = null;             //����f�[�^
    Bounty                              m_Bounty                = null;             //�o�E���e�B���
    FieldSelect                         m_FieldSelect           = null;             //�t�B�[���h�I���N���X
    Image                               m_EnemyImage            = null;             //��Փx�I�����̓G�摜
    Transform                           m_MyTransform           = null;             //���g�̃g�����X�t�H�[��

    [HideInInspector]
    public int                          m_BountyID              = 0;                //�o�E���e�BID
    [HideInInspector]
    public bool                         m_IsGriped              = false;            //�͂܂�Ă���t���O

    public void Initialize(int id)
    {
        //�R���|�[�l���g����Ȃ�R���|�[�l���g�擾
        m_EnemyImage  ??= GameObject.Find("Difficulty/EnemyImage").GetComponent<Image>();
        m_FieldSelect ??= GameObject.Find("FieldSelect").GetComponent<FieldSelect>();
        m_MyTransform ??= transform;

        //ID�ݒ�
        m_BountyID = id;

        //�Z�[�u�f�[�^����o�E���e�B�����擾�A�ҏW�ς݂�����
        m_Bounty = SaveSystem.m_SaveData.bounties[m_BountyID];

        // Bounty����Ȃ�V�K����
        m_Bounty ??= new Bounty();

        //�ҏW�ς݂ł͂Ȃ�������V�������
        if (!m_Bounty.editedFlag)
        {
            m_Bounty.CreateBounty(m_FieldData, m_EnemyData, m_BountyNameData, m_WeaponData);
            SaveSystem.m_SaveData.bounties[m_BountyID] = m_Bounty;
            SaveSystem.Save();
        }

        //�ԍ������ƂɃG�l�~�[�ƃt�B�[���h�̏����擾����
        FieldData.Field field = m_FieldData.GetFieldAt(m_Bounty.fieldIndex);
        EnemyData.Enemy enemy = m_EnemyData.GetEnemyAt(m_Bounty.enemyIndex);

        // * �o�E���e�B�p�l���̏��ݒ� * //
        //�摜�ݒ�
        m_StageImage.BackGround.sprite = field.sprite;
        m_StageImage.EnemyImage.sprite = enemy.sprite;

        //�摜�F����(�|�������Ƃ̂Ȃ��G�͍�������)
        if (!SaveSystem.m_SaveData.completedEnemies[m_Bounty.enemyIndex])
            m_StageImage.EnemyImage.color = m_IncompletionColor;

        //��`�T�C�Y����
        Texture2D texture = enemy.sprite.texture;
        m_StageImage.EnemyImage.rectTransform.sizeDelta = new Vector2(texture.width, texture.height);

        //������ݒ�
        m_BountyNameText.text   = m_Bounty.bountyName;
        m_FieldNameText.text    = m_FieldData.GetFieldAt(m_Bounty.fieldIndex).name;
        m_BonusWeaponText.text  = m_WeaponData.GetWeaponAt(m_Bounty.weaponIndex).weaponName;
        m_PointAmmountText.text = m_Bounty.point.ToString();

        //�����T�C�Y����
        float largeSize = m_LifeAmmountText.fontSize * 1.25f;
        m_LifeAmmountText.text = "<size=" + largeSize + ">" + m_Bounty.life.ToString() + "</size> ��͐s�����玸�s";

        //�o�E���e�B�����łɒB���ς݂Ȃ班���҂��Ă���X�V����
        if (m_Bounty.completionFlag && woskni.Scene.m_LastScene == woskni.Scenes.GameMain)
            Invoke(nameof(RenewalBounty), 0.25f);
    }

    private void Update()
    {
        //�}�X�N�摜�̃t�F�[�h����(�͂܂�Ă���t���O�ŔZ�����邩�������邩���߂�j
        Color color = m_MaskImage.color;
        color.a = Mathf.MoveTowards(color.a, m_IsGriped ? m_MaxMaskAlpha : 0.0f, m_MaxAlphaDelta);
        m_MaskImage.color = color;

        //���g�̊g�k����(�͂܂�Ă���t���O�ő傫�����邩���������邩���߂�j
        Vector3 scale = m_MyTransform.localScale;
        scale.x = scale.y = Mathf.MoveTowards(scale.x, m_IsGriped ? m_MaxScale : 1.0f, m_MaxScaleDelta);
        m_MyTransform.localScale = scale;
    }

    //�o�E���e�B�X�V
    public void RenewalBounty()
    {
        //�R���[�`���X�^�[�g
        StartCoroutine(UpdateBounty());
    }

    IEnumerator UpdateBounty()
    {
        //�o�E���e�B�X�V���t���O���グ��
        BountyScrollView bountyScrollView = GameObject.Find("BountyScrollView").GetComponent<BountyScrollView>();
        bountyScrollView.m_BountyUpdatingFlag = true;

        //�L�����o�X�O���[�v�擾
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();

        //�^�C�}�[����
        woskni.Timer timer = new woskni.Timer();
        timer.Setup(0.4f);

        //�o�E���e�B�B���̉摜�̃R���|�[�l���g�擾
        Image image = m_CompletionImage.GetComponent<Image>();

        while (true)
        {
            //�^�C�}�[�X�V
            timer.Update();

            //�C�[�W���O�Ŋg�k
            float size = woskni.Easing.OutBounce(timer.time, timer.limit, 1.0f, 0.55f);
            m_CompletionImage.localScale = new Vector2(size, size);

            //�C�[�W���O�Ńt�F�[�h������
            float alpha = woskni.Easing.OutSine(timer.time, timer.limit, 0.0f, 1.0f);
            image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);

            //�P�t���[���ҋ@
            yield return null;

            //�^�C�}�[�I���Ń��[�v�E�o
            if (timer.IsFinished())
            {
                //�덷�C��
                m_CompletionImage.localScale = new Vector2(0.55f, 0.55f);
                image.color = new Color(image.color.r, image.color.g, image.color.b, 1.0f);
                break;
            }
        }

        //�^�C�}�[���Z�b�g
        timer.Reset();

        //�����ҋ@
        yield return new WaitForSeconds(0.1f);

        // �ړ��n�_��ݒ�
        Vector3 firstPos = transform.localPosition;
        Vector3 lastPos = firstPos + new Vector3(0, 300, 0);

        while (true)
        {
            //�^�C�}�[�X�V
            timer.Update();

            //�C�[�W���O�œ�����
            Vector3 pos = transform.localPosition;
            pos.y = woskni.Easing.OutQuintic(timer.time, timer.limit, firstPos.y, lastPos.y);
            transform.localPosition = pos;

            //�C�[�W���O�Ńt�F�[�h������
            canvasGroup.alpha = woskni.Easing.InQuart(timer.time, timer.limit, 1.0f, 0.0f);

            //�P�t���[���ҋ@
            yield return null;

            //�^�C�}�[�I���Ń��[�v�E�o
            if (timer.IsFinished())
            {
                //�덷�C��
                transform.localPosition = lastPos;
                canvasGroup.alpha = 0.0f;
                break;
            }
        }

        //�^�C�}�[���Z�b�g
        timer.Reset();

        //�ҏW�ς݃t���O����
        m_Bounty.editedFlag = false;

        //�o�E���e�B�����Z�b�g
        Initialize(m_BountyID);

        //�����ҋ@
        yield return new WaitForSeconds(0.1f);

        //�B���摜�������Ȃ�����
        image.color = new Color(image.color.r, image.color.g, image.color.b, 0.0f);

        // �ړ��n�_���Đݒ�
        lastPos = firstPos;
        firstPos = transform.localPosition;

        while (true)
        {
            //�^�C�}�[�X�V
            timer.Update();

            //�C�[�W���O�œ�����
            Vector3 pos = transform.localPosition;
            pos.y = woskni.Easing.OutQuintic(timer.time, timer.limit, firstPos.y, lastPos.y);
            transform.localPosition = pos;

            //�C�[�W���O�Ńt�F�[�h������
            canvasGroup.alpha = woskni.Easing.OutQuart(timer.time, timer.limit, 0.0f, 1.0f);

            //�P�t���[���ҋ@
            yield return null;

            //�^�C�}�[�I���Ń��[�v�E�o
            if (timer.IsFinished())
            {
                //�덷�C��
                transform.localPosition = lastPos;
                canvasGroup.alpha = 1.0f;
                break;
            }
        }

        //�o�E���e�B�X�V���t���O��������
        bountyScrollView.m_BountyUpdatingFlag = false;

        //�R���[�`���I��
        yield break;
    }

    public void ButtonDown()
    {
        //���ݒ�
        TemporarySavingBounty.field        = m_FieldData.GetFieldAt(m_Bounty.fieldIndex);
        TemporarySavingBounty.enemy        = m_EnemyData.GetEnemyAt(m_Bounty.enemyIndex);
        TemporarySavingBounty.bonusWeapon  = m_WeaponData.GetWeaponAt(m_Bounty.weaponIndex);
        TemporarySavingBounty.bountyName   = m_Bounty.bountyName;
        TemporarySavingBounty.point        = m_Bounty.point;
        TemporarySavingBounty.life         = m_Bounty.life;
        TemporarySavingBounty.bountyIndex  = m_BountyID;

        //�摜�̐ݒ�
        m_EnemyImage.sprite = m_StageImage.EnemyImage.sprite;
        m_EnemyImage.color  = m_StageImage.EnemyImage.color;

        //�摜�T�C�Y�ύX
        Vector2 size = (TemporarySavingBounty.enemy.sprite.rect.size + Vector2.one * 100f) / 2f;
        m_EnemyImage.rectTransform.sizeDelta = size;

        //��Փx�I����ʂ�
        m_FieldSelect.ShowDifficulty();
    }
}

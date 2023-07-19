using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Result : MonoBehaviour
{
    [Header("�摜�E�e�L�X�g�R���|�[�l���g")]
    [woskni.Name("�t�B�[���h�w�i"),       SerializeField] Image                 m_FieldImage;
    [woskni.Name("�G�摜"),               SerializeField] Image                 m_EnemyImage;
    [woskni.Name("�|���Ă��Ȃ��G�̐F"),   SerializeField] Color                 m_IncompletionColor;

    [woskni.Name("�w�i�摜"),             SerializeField] Image                 m_BackImage;
    [woskni.Name("�o�E���e�B�摜"),       SerializeField] Image                 m_BountyImage;
    [woskni.Name("�͂񂱉摜"),           SerializeField] Image                 m_StampImage;
    [woskni.Name("�o�E���e�B��"),         SerializeField] TextMeshProUGUI       m_BountyName;
    [woskni.Name("�����̃e�L�X�g"),       SerializeField] List<TextMeshProUGUI> m_LeftTextList;
    [woskni.Name("�E���̃e�L�X�g"),       SerializeField] List<TextMeshProUGUI> m_RightTextList;
    [woskni.Name("�����̋��z�̃e�L�X�g"), SerializeField] TextMeshProUGUI       m_BottomYenText;
    [woskni.Name("���v"),                 SerializeField] Text                  m_ClockText;

    [Header("�͂񂱉摜")]
    [woskni.Name("�B���X�v���C�g"),   SerializeField] Sprite m_CompletionSprite;
    [woskni.Name("���B���X�v���C�g"), SerializeField] Sprite m_IncompletionSprite;

    [Header("�^�C�}�[�p����")]
    [woskni.Name("�t�F�[�h����"),     SerializeField] float m_FadeTime;
    [woskni.Name("�͂񂱎���"),       SerializeField] float m_StampTime;
    [woskni.Name("�\������(��)"),     SerializeField] float m_AppearBountyflyerTime;
    [woskni.Name("�\������(����)"),   SerializeField] float m_AppearBountytextTime;
    [woskni.Name("��������������"),   SerializeField] float m_YenGainTime;
    [woskni.Name("�^�C�g���J�ڎ���"), SerializeField] float m_TransitionTime;

    [Header("�ŏI���z�\��")]
    [woskni.Name("���z�w�i"), SerializeField] Image           m_YenFrame;
    [woskni.Name("�ǉ����z"), SerializeField] TextMeshProUGUI m_YenGainText;
    [woskni.Name("�ŏI���z"), SerializeField] TextMeshProUGUI m_FinallyYenText;

    Vector3 m_BasisPosition;
    Vector3 m_FieldBasisPosition;
    Vector3 m_EnemyBasisPosition;

    woskni.Timer m_Timer;
    woskni.Timer m_TransitionTimer;

    // ����{�[�i�X�̃t���O
    bool m_BonusFlag = false;

    bool m_SceneChangeCompletedFlag = false;

    // ���Z��̕�V��
    int m_FinallyReward = 0;

    // �C�[�W���O�J�n���̋��z
    int m_EasingStartYen;

    enum State
    {
          FadeOut       // �t�F�[�h����
        , ShowBounty    // �o�E���e�B�\��
        , ShowStamp     // �X�^���v�\��
        , ShowLeft      // �����e�L�X�g�\��
        , ShowRight     // �E���e�L�X�g�\��
        , ShowBottom    // �����e�L�X�g�\��
        , AddYen        // �����ǉ�����&�\��
        , Finish        // �I������
    }

    State m_State;

    int m_CurrentAppearNum;

    void Start()
    {
        // �o�E���e�B�N���X�̒B���t���O�����āA�͂񂱉摜�ɔ��f������
        int bounty_id = TemporarySavingBounty.bountyIndex;
        Bounty bounty = SaveSystem.m_SaveData.bounties[bounty_id];
        m_StampImage.sprite = bounty.completionFlag ? m_CompletionSprite : m_IncompletionSprite;

        // �t�B�[���h�E�G�̉摜
        {
            // �摜�ݒ�
            m_FieldImage.sprite = TemporarySavingBounty.field.sprite;
            m_EnemyImage.sprite = TemporarySavingBounty.enemy.sprite;

            // �A���t�@�l�ݒ�
            m_FieldImage.color = GetAlphaColor(m_FieldImage.color, 0f);
            m_EnemyImage.color = GetAlphaColor(m_EnemyImage.color, 0f);

            // �ʒu�ݒ�
            m_FieldBasisPosition = m_FieldImage.rectTransform.position;
            m_EnemyBasisPosition = m_EnemyImage.rectTransform.position;

            // �摜�F����(�|�������Ƃ̂Ȃ��G�͍�������)
            bool flag = SaveSystem.m_SaveData.completedEnemies[bounty.enemyIndex];
            Color color = flag ? Color.white : m_IncompletionColor;
            color.a = 0;
            m_EnemyImage.color = color;

            // ��`�T�C�Y����
            Texture2D texture = m_EnemyImage.sprite.texture;
            m_EnemyImage.rectTransform.sizeDelta = new Vector2(texture.width, texture.height);
        }

        // BGM�̉��ʂ�������
        woskni.SoundManager.ChangeVolume(TemporarySavingBounty.field.BGM, SaveSystem.m_SaveData.BGMvolume * 0.3f, m_FadeTime);

        // �^�C�}�[��ݒ�
        m_Timer.Setup(m_FadeTime);

        m_BackImage.color   = GetAlphaColor(m_BackImage.color  , 0f);
        m_BountyImage.color = GetAlphaColor(m_BountyImage.color, 0f);
        m_BountyName.color  = GetAlphaColor(m_BountyName.color , 0f);
        m_StampImage.color  = GetAlphaColor(m_StampImage.color , 0f);

        m_BasisPosition = m_BountyImage.rectTransform.position;

        // �o�E���e�B���ݒ�
        m_BountyName.text = bounty.bountyName;

        for (int i = 0; i < m_LeftTextList.Count; ++i) {
            m_LeftTextList[i].color  = GetAlphaColor(m_LeftTextList[i].color, 0f);
            m_RightTextList[i].color = GetAlphaColor(m_RightTextList[i].color, 0f);
        }

        // �{�[�i�X�t���O�𔻒�
        m_BonusFlag = TemporarySavingBounty.bonusWeapon.weaponName == TemporarySavingBounty.equipWeapon.weaponName;

        float difficulty = woskni.Math.Round(TemporarySavingBounty.difficulty * 2.00f + 1.00f, -2);
        difficulty -= 0.01f;

        // �E���̃e�L�X�g�̒l���
        TemporarySavingBounty.
               point = bounty.completionFlag ? bounty.point : 0;                     // �s�k��������0�|�C���g
        m_RightTextList[0].text = TemporarySavingBounty.point.ToString() + " �~";    // ��V��
        m_RightTextList[1].text = m_BonusFlag ? "�B���I�@x1.5" : "���B��";           // �{�[�i�X����
        m_RightTextList[2].text = "x" + difficulty.ToString("F2");       // ��Փx�{�[�i�X

        // �ŏI���z
        ////m_FinallyReward = (int)(TemporarySavingBounty.point * (m_BonusFlag ? 1.5f : 1f) * (difficulty * 2f + 1f));

        m_FinallyReward = woskni.Math.Round((int)(TemporarySavingBounty.point * (m_BonusFlag ? 1.5f : 1f) * difficulty), 2);

        // ���Z�z
        m_BottomYenText.color = GetAlphaColor(m_BottomYenText.color, 0f);
        m_BottomYenText.text  = m_FinallyReward.ToString() + " �~";

        // �ǉ����z
        // �s�k�������當���F�͔�
        m_YenGainText.color = m_FinallyReward == 0 ? Color.white : m_YenGainText.color;
        m_YenGainText.color = GetAlphaColor(m_YenGainText.color, 0f);
        m_YenGainText.text  = "+" + m_FinallyReward.ToString();

        // �ŏI���z
        m_FinallyYenText.color = GetAlphaColor(m_FinallyYenText.color, 0f);
        m_FinallyYenText.text  = SaveSystem.m_SaveData.money.ToString();

        m_YenFrame.color = GetAlphaColor(m_YenFrame.color, 0f);

        m_State = State.FadeOut;

        m_CurrentAppearNum = 0;

        // �C�[�W���O�J�n���̋��z��ݒ�
        m_EasingStartYen = SaveSystem.m_SaveData.money;

        m_ClockText.text = "";
    }

    void Update()
    {
        switch (m_State)
        {
            case State.FadeOut:    FadeOut();                           break;
            case State.ShowBounty: ShowBounty();                        break;
            case State.ShowStamp:  ShowStamp();                         break;
            case State.ShowLeft:   ShowLeftRight(ref m_LeftTextList, m_RightTextList);                                  break;
            case State.ShowRight:  ShowLeftRight(ref m_RightTextList, new List<TextMeshProUGUI>() { m_BottomYenText });    break;
            case State.ShowBottom: ShowBottom();                        break;
            case State.AddYen:     AddYen();                            break;
            case State.Finish:     Finish();                            break;
        }
    }

    /// <summary>�A���t�@�l��ύX�����F��Ԃ�</summary>
    /// <param name="color">���̐F</param>
    /// <param name="alpha">�A���t�@�l (0 to 1)</param>
    Color GetAlphaColor(Color color, float alpha) => new Color(color.r, color.g, color.b, alpha);

    /// <summary>�t�F�[�h����</summary>
    /// <remarks>�t�F�[�h�������s���A�w�i���Â����鏈��</remarks>
    void FadeOut()
    {
        m_Timer.Update();

        // �t�F�[�h���Ԃ��I������܂œ����x��M�鏈�����s��
        float alpha = woskni.Easing.Linear(m_Timer.time, m_Timer.limit, 0f, 0.5f);

        m_BackImage.color = GetAlphaColor(m_BackImage.color, alpha);

        if (m_Timer.IsFinished())
        {
            m_BackImage.color = GetAlphaColor(m_BackImage.color, 0.5f);

            m_State = State.ShowBounty;

            m_Timer.Setup(m_AppearBountyflyerTime);
        }
    }

    /// <summary>�o�E���e�B�\��</summary>
    /// <remarks>�o�E���e�B�摜��\�����邽�߂̏���</remarks>
    void ShowBounty()
    {
        m_Timer.Update();

        // �t�F�[�h���Ԃ��I������܂ŃC�[�W���O���s��
        {
            float time = m_Timer.time;
            float limit = m_Timer.limit;

            float alpha = woskni.Easing.Linear(time, limit, 0f, 1f);

            const float dif = -120f;
            float easingDif = woskni.Easing.OutQuart(time, limit, dif, 0f);

            // �ʒu
            m_BountyImage.rectTransform.position = m_BasisPosition     .Difference(y: easingDif);
            m_FieldImage .rectTransform.position = m_FieldBasisPosition.Difference(y: easingDif);
            m_EnemyImage .rectTransform.position = m_EnemyBasisPosition.Difference(y: easingDif);

            // �A���t�@�l
            m_BountyImage   .color  = GetAlphaColor(m_BountyImage   .color, alpha);
            m_BountyName    .color  = GetAlphaColor(m_BountyName    .color, alpha);

            m_FieldImage    .color  = GetAlphaColor(m_FieldImage    .color, alpha);
            m_EnemyImage    .color  = GetAlphaColor(m_EnemyImage    .color, alpha);

            m_FinallyYenText.color  = GetAlphaColor(m_FinallyYenText.color, alpha);
            m_YenFrame      .color  = GetAlphaColor(m_YenFrame      .color, alpha);
        }

        // �^�C�}�[�I��
        if (m_Timer.IsFinished())
        {
            m_BountyImage.rectTransform.position = m_BasisPosition;
            m_FieldImage .rectTransform.position = m_FieldBasisPosition;
            m_EnemyImage .rectTransform.position = m_EnemyBasisPosition;

            m_BountyImage   .color = GetAlphaColor(m_BountyImage.color   , 1f);
            m_FieldImage    .color = GetAlphaColor(m_FieldImage.color    , 1f);
            m_EnemyImage    .color = GetAlphaColor(m_EnemyImage.color    , 1f);
            m_BountyName    .color = GetAlphaColor(m_BountyName.color    , 1f);
            m_FinallyYenText.color = GetAlphaColor(m_FinallyYenText.color, 1f);
            m_YenFrame      .color = GetAlphaColor(m_YenFrame.color      , 1f);

            m_State = State.ShowStamp;

            m_Timer.Setup(m_StampTime);
        }
    }
    /// <summary>�X�^���v�\��</summary>
    /// <remarks>�B���E���B���̃X�^���v����������</remarks>
    void ShowStamp()
    {
        m_Timer.Update();
        float time = m_Timer.time;
        float limit = m_Timer.limit;

        float alpha = woskni.Easing.Linear (time, limit, 0f, 1f);
        float scale = woskni.Easing.OutBack(time, limit, 1.5f, 1f, 2f);

        m_StampImage.color = GetAlphaColor(m_StampImage.color, alpha);
        m_StampImage.rectTransform.localScale = new Vector3(scale, scale, 1f);

        if(m_Timer.IsFinished())
        {
            m_StampImage.color = GetAlphaColor(m_StampImage.color, 1f);
            m_StampImage.rectTransform.localScale = Vector3.one;

            m_CurrentAppearNum = 0;

            m_BasisPosition = m_LeftTextList[m_CurrentAppearNum].rectTransform.position;

            m_State = State.ShowLeft;

            m_Timer.Setup(m_AppearBountytextTime);
        }
    }

    /// <summary>�e�L�X�g�\��</summary>
    /// <param name="textList">��������e�L�X�g</param>
    /// <param name="nextTextList">���ɏ�������e�L�X�g</param>
    /// <remarks>�����̃e�L�X�g�����X�g���ɕ\�������鏈��</remarks>
    void ShowLeftRight(ref List<TextMeshProUGUI> textList, List<TextMeshProUGUI> nextTextList)
    {
        m_Timer.Update();

        float time = m_Timer.time;
        float limit = m_Timer.limit;

        float alpha = woskni.Easing.Linear(time, limit, 0f, 1f);
        float dif = woskni.Easing.OutQuart(time, limit, -64f, 0f);

        textList[m_CurrentAppearNum].color = GetAlphaColor(textList[m_CurrentAppearNum].color, alpha);
        textList[m_CurrentAppearNum].rectTransform.position = m_BasisPosition + new Vector3(0f, dif, 0f);

        if(m_Timer.IsFinished())
        {
            m_Timer.Reset();

            textList[m_CurrentAppearNum].color = GetAlphaColor(textList[m_CurrentAppearNum].color, 1f);
            textList[m_CurrentAppearNum].rectTransform.position = m_BasisPosition;

            // ���e�L�X�g�����X�g�Ō���𒴉߂�����
            if (++m_CurrentAppearNum >= textList.Count)
            {
                m_CurrentAppearNum = 0;

                Debug.Log(nextTextList[m_CurrentAppearNum]);

                m_BasisPosition = nextTextList[m_CurrentAppearNum].rectTransform.position;

                switch (m_State) {
                    case State.ShowLeft:  m_State = State.ShowRight;  break;
                    case State.ShowRight: m_State = State.ShowBottom; break;

                    default: Debug.LogError("State����O�ł�: " + m_State.ToString()); break;
                }
            }
            else
                m_BasisPosition = textList[m_CurrentAppearNum].rectTransform.position;
        }
    }

    /// <summary>�e�L�X�g�\��</summary>
    /// <remarks>�ŏI�I�ȕ�V���z��\������</remarks>
    void ShowBottom()
    {
        m_Timer.Update();
        float time = m_Timer.time;
        float limit = m_Timer.limit;

        float alpha = woskni.Easing.Linear(time, limit, 0f, 1f);
        float dif = woskni.Easing.OutQuart(time, limit, -64f, 0f);

        m_BottomYenText.color = GetAlphaColor(m_BottomYenText.color, alpha);
        m_BottomYenText.rectTransform.position = m_BasisPosition + new Vector3(0f, dif);

        if (m_Timer.IsFinished())
        {
            m_Timer.Setup(m_YenGainTime);
            m_State = State.AddYen;

            m_BottomYenText.color = GetAlphaColor(m_BottomYenText.color, 1f);
            m_BottomYenText.rectTransform.position = m_BasisPosition;

            m_BasisPosition = m_YenGainText.rectTransform.position;
        }
    }

    /// <summary>�������̑��z����</summary>
    void AddYen()
    {
        m_Timer.Update();
        float time = m_Timer.time;
        float limit = m_Timer.limit;

        float alpha = woskni.Easing.OutExp(time, limit, 0f, 1f);
        float dif = woskni.Easing.OutExp(time, limit, -16f, 0f);

        // ������ǉ����ꂽ���\������
        m_YenGainText.color = GetAlphaColor(m_YenGainText.color, alpha);
        m_YenGainText.rectTransform.position = m_BasisPosition + new Vector3(0f, dif);

        SaveSystem.m_SaveData.money = 
            (int)woskni.Easing.OutQuart(time, limit, m_EasingStartYen, Mathf.Min(9999999, m_EasingStartYen + m_FinallyReward));

        m_FinallyYenText.text = SaveSystem.m_SaveData.money.ToString();

        if (m_Timer.IsFinished())
        {
            // �Z�[�u����
            SaveSystem.m_SaveData.money = Mathf.Min(99999999, m_EasingStartYen + m_FinallyReward);

            m_YenGainText.color = GetAlphaColor(m_YenGainText.color, 1f);
            m_YenGainText.rectTransform.position = m_BasisPosition;

            m_FinallyYenText.text = SaveSystem.m_SaveData.money.ToString();

            m_Timer.Reset();
            m_TransitionTimer.Setup(m_TransitionTime);
            m_State = State.Finish;
        }
    }

    /// <summary>�I��</summary>
    /// <remarks>�V�[���J�ړ��̏���</remarks>
    void Finish()
    {
        if(!m_TransitionTimer.IsFinished())
            m_TransitionTimer.Update();
        else
        {
            FinishChangeScene(true);
            return;
        }

        m_ClockText.text = (m_TransitionTimer.TimeLeft() + 0.5f).ToString("F0");

        // 14.2567�b => 0.256
        float time01 = woskni.Easing.OutCubic(new woskni.Range(0f, 1f).GetAround(m_TransitionTimer.time), 1f, 0f, 1f);

        m_ClockText.color = m_ClockText.color.GetAlphaColor(1f - time01);

        //�V�[���`�F���W���m�肵���炱���ŏI��
        if (m_SceneChangeCompletedFlag) return;

        if (woskni.InputManager.IsButtonUp())
            FinishChangeScene(false);
    }

    void FinishChangeScene(bool afkFlag)
    {
        m_TransitionTimer.Fin();

        m_ClockText.text = "";

        SaveSystem.m_SaveData.AFKflag = afkFlag;
        SaveSystem.Save();

        woskni.SoundManager.ChangeVolume(TemporarySavingBounty.field.BGM, 0f, 1f);

        woskni.Scene.Change(woskni.Scenes.Title);

        m_SceneChangeCompletedFlag = true;
    }
}

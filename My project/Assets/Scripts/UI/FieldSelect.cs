using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�t�B�[���h�I���N���X
public class FieldSelect : MonoBehaviour
{
    //�I�����
    enum SelectState
    {
        Bounty,
        Difficulty,
    }

    [SerializeField] DifficultyFire m_DifficultyFire            = null;                 //��Փx�G�t�F�N�g
    [SerializeField] Title          m_Title                     = null;                 //�^�C�g���X�N���v�g
    [SerializeField] Transform      m_Difficulty                = null;                 //��Փx�p�l��
    [SerializeField] Transform      m_BountySelect              = null;                 //�o�E���e�B�I���p�l��
    [SerializeField] float          m_MoveTime                  = 0.25f;                //�ړ�����
    SelectState                     m_SelectState               = SelectState.Bounty;   //��ʏ��
    Vector3                         m_FirstDifficultyPosition   = Vector3.zero;         //��Փx�p�l���̏����ʒu
    Vector3                         m_FirstBountySelectPosition = Vector3.zero;         //�o�E���e�B�I���p�l���̏����ʒu
    bool                            m_ChangeFlag                = false;                //��ʕύX�t���O
    woskni.Timer                    m_ChangeTimer;                                      //��ʕύX�^�C�}�[
    float                           m_HomePositionX             = 0.0f;                 //�C�[�W���O�J�n�ʒuX
    float                           m_DestinationPositionX      = 0.0f;                 //�C�[�W���O�I���ʒuX

    void Start()
    {
        //�����ʒu�ۑ�
        m_FirstDifficultyPosition   = m_Difficulty.localPosition;
        m_FirstBountySelectPosition = m_BountySelect.localPosition;

        //�^�C�}�[�Z�b�g�A�b�v
        m_ChangeTimer.Setup(m_MoveTime);
    }

    void Update()
    {
        m_DifficultyFire.m_IsShake = m_SelectState == SelectState.Difficulty;

        //�ύX���Ȃ��Ȃ牽�����Ȃ�
        if (!m_ChangeFlag) return;

        //�^�C�}�[�X�V
        m_ChangeTimer.Update();

        //�ϐ��p��
        Vector3 pos = m_Difficulty.localPosition;
        float time  = m_ChangeTimer.time;
        float limit = m_ChangeTimer.limit;

        //�C�[�W���O
        pos.x = woskni.Easing.OutSine(time, limit, m_HomePositionX, m_DestinationPositionX);
        m_Difficulty.localPosition = pos;

        //�^�C�}�[�I���Ńt���O��������
        if(m_ChangeTimer.IsFinished())
            m_ChangeFlag = false;
    }

    /// <summary>��Փx�̕\���ύX</summary>
    /// <param name="selectState">�I�����</param>
    void ChangeDifficulty(SelectState selectState)
    {
        m_SelectState = selectState;

        //�t���O���グ��
        m_ChangeFlag = true;

        //�^�C�}�[���Z�b�g
        m_ChangeTimer.Reset();

        //�����l�E�ړI�l�ۑ�
        switch (m_SelectState)
        {
            case SelectState.Bounty:
                m_HomePositionX         = m_FirstBountySelectPosition.x;
                m_DestinationPositionX  = m_FirstDifficultyPosition.x;
                break;
            case SelectState.Difficulty:
                m_HomePositionX         = m_FirstDifficultyPosition.x;
                m_DestinationPositionX  = m_FirstBountySelectPosition.x;
                break;
        }
    }

    /// <summary>��Փx�\��</summary>
    public void ShowDifficulty()
    {
        //�\���ύX
        ChangeDifficulty(SelectState.Difficulty);

        //�X���C�_�[�̈ʒu���Z�b�g
        m_Difficulty.GetComponent<Difficulty>().ResetSlider();
    }

    /// <summary>��Փx��\��</summary>
    public void HideDifficulty()
    {
        //�\���ύX
        ChangeDifficulty(SelectState.Bounty);

        //�X���C�_�[�̈ʒu�ۑ�
        SaveSystem.m_SaveData.difficulty = m_Difficulty.GetComponent<Difficulty>().GetDifficulty();
        SaveSystem.Save();
    }

    /// <summary>�߂�{�^������</summary>
    public void PressReturnButton()
    {
        //��Ԃ����Ăǂ��ɖ߂������߂�
        switch (m_SelectState)
        {
            case SelectState.Bounty:        m_Title.UndoWindowState();  break;
            case SelectState.Difficulty:    HideDifficulty();           break;
        }
    }

    /// <summary>�V�[���ύX</summary>
    public void ChangeScene()
    {
        //��Փx�ݒ�
        Difficulty difficulty = m_Difficulty.GetComponent<Difficulty>();
        TemporarySavingBounty.difficulty = difficulty.GetDifficulty();

        //�t���O���Z�b�g
        int bounty_id = TemporarySavingBounty.bountyIndex;
        SaveSystem.m_SaveData.bounties[bounty_id].completionFlag = false;

        //�f�[�^�ۑ�
        SaveSystem.m_SaveData.difficulty = difficulty.GetDifficulty();
        SaveSystem.Save();

        woskni.SoundManager.ChangeVolume("�������ƃo�E���e�B", 0f, 1f);
        woskni.SoundManager.ChangeVolume("�������ƃo�E���e�B", 0f, 1f);

        woskni.Scene.Change(woskni.Scenes.GameMain);
    }
}

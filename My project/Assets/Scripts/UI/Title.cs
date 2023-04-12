using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//�^�C�g����ʃN���X
public class Title : MonoBehaviour
{
    //��ʏ��
    [System.Serializable]
    public enum WINDOW_STATE
    {
        Title,          //�^�C�g��
        FieldSelect,    //�X�e�[�W�I��
        Settings,       //�ݒ�
    }

    [Header("�R���|�[�l���g")]
    [SerializeField] Transform  m_Title         = null; // �^�C�g��
    [SerializeField] Transform  m_FieldSelect   = null; // �X�e�[�W�I��
    [SerializeField] Transform  m_Settings      = null; // �ݒ�

    [Header("�ړ�����")]
    [SerializeField] float      m_Movetime      = 1f;   // �ړ�����
    [woskni.Name("�f�[�^�폜�܂ł̎���"), SerializeField] float m_Deletetime = 180f;

    [Header("�ȃ��X�g")]
    [SerializeField] string[]   m_Musics        = null; //�y�ȃ��X�g

    Transform       myTransform                 = null;                 // ���g�̃g�����X�t�H�[��
    Vector3         m_FirstTitlePosition        = Vector3.zero;         // �^�C�g�������ʒu
    Vector3         m_FirstFieldSelectPosition  = Vector3.zero;         // �X�e�[�W�I�������ʒu
    Vector3         m_FirstSettingsPosition     = Vector3.zero;         // �ݒ菉���ʒu
    Vector3         m_StartPosition             = Vector3.zero;         // �C�[�W���O�J�n�ʒu
    Vector3         m_EndPosition               = Vector3.zero;         // �C�[�W���O�I���ʒu
    WINDOW_STATE    m_WindowState               = WINDOW_STATE.Title;   // ��ʏ��
    WINDOW_STATE    m_BeforeWindowState         = WINDOW_STATE.Title;   // ��ʏ��(��O)
    woskni.Timer    m_MoveTimer;                                        // �ړ��^�C�}�[
    woskni.Timer    m_DeleteTimer;                                      // �f�[�^�폜�^�C�}�[
    int             m_CurrentMusicID            = 0;                    //���݂�BGM�̔ԍ�

    void Start()
    {
        woskni.SoundManager.StopAll();
        PlayTitleBGM(System.DateTime.Now);

        //�g�����X�t�H�[���ۑ�
        myTransform = transform;

        //�����ʒu�ۑ�
        m_FirstTitlePosition         = m_Title.localPosition;
        m_FirstFieldSelectPosition   = -m_FieldSelect.localPosition;
        m_FirstSettingsPosition      = -m_Settings.localPosition;

        //�^�C�}�[�Z�b�g�A�b�v
        m_MoveTimer.Setup(m_Movetime);
        m_DeleteTimer.Setup(m_Deletetime);

        //�O�̃V�[���ɂ���ʏ�Ԃ����߂�
        if((woskni.Scene.m_LastScene == woskni.Scenes.GameMain ||
            woskni.Scene.m_LastScene == woskni.Scenes.Result)  &&
           !SaveSystem.m_SaveData.AFKflag)
        {
            ChangeWindowState_FieldSelect();
            m_MoveTimer.Fin();
        }
    }

    void Update()
    {
        //�^�C�}�[�ғ����Ȃ�
        if (!m_MoveTimer.IsFinished())
        {
            //�^�C�}�[�X�V
            m_MoveTimer.Update();

            //�C�[�W���O�ňړ�
            float limit = m_MoveTimer.limit;
            float time = m_MoveTimer.time;
            Vector3 pos = myTransform.localPosition;
            pos.x = woskni.Easing.OutSine(time, limit, m_StartPosition.x, m_EndPosition.x);
            pos.y = woskni.Easing.OutSine(time, limit, m_StartPosition.y, m_EndPosition.y);
            myTransform.localPosition = pos;
        }
        //�^�C�}�[�I���Ȃ�
        else
            //�덷�␳
            myTransform.localPosition = m_EndPosition;

        //�^�C�g����� ���� �^�C�}�[�I�� ���� ���͂Ȃ�
        if (m_WindowState == WINDOW_STATE.Title)
        {
            m_DeleteTimer.Update();
            if (m_DeleteTimer.IsFinished())
            {
                SaveSystem.TrialReset();
                m_DeleteTimer.Reset();
            }
            else if (woskni.InputManager.IsButton())
                m_DeleteTimer.Reset();
        }

#if true
        if (Input.GetKeyDown(KeyCode.H))
        {
            Debug.Log("�o�E���e�B�X�V");
            GameObject.Find("BountyScrollView").GetComponent<BountyScrollView>().BountiesReset();
            if (m_Musics.Length > 0)
            {
                int num = Random.Range(0, m_Musics.Length);
                while (m_CurrentMusicID == num)
                    num = Random.Range(0, m_Musics.Length);
                CrossFade(m_Musics[num], 1.5f, false);
                m_CurrentMusicID = num;
            }
        }
#else
        //�w�莞�Ԃ��߂�����X�V����
        if (SaveSystem.IsUpdatedDay(System.DateTime.Now,SaveSystem.m_SaveData.GetLastSaveTime()))
        {
            Debug.Log("�o�E���e�B�X�V");
            GameObject.Find("BountyScrollView").GetComponent<BountyScrollView>().BountiesReset();
            if (m_Musics.Length > 0)
            {
                int num = Random.Range(0, m_Musics.Length);
                while (m_CurrentMusicID == num)
                    num = Random.Range(0, m_Musics.Length);
                CrossFade(m_Musics[num]);
                m_CurrentMusicID = num;
            }
        }
#endif
    }

    void CrossFade(string nextMusicName, float fadeTime, bool isBeginning)
    {
        woskni.SoundManager.SoundPlayer currentMusic = woskni.SoundManager.FindSoundPlayer(m_Musics[m_CurrentMusicID]);
        float volume = currentMusic.source.volume;
        float time = 0.0f;
        if (!isBeginning) time = currentMusic.source.time;
        woskni.SoundManager.Play(nextMusicName, true, 0.01f, playTime: time);
        woskni.SoundManager.SoundPlayer nextMusic = woskni.SoundManager.FindSoundPlayer(nextMusicName);

        woskni.SoundManager.ChangeVolume(currentMusic.audio.clip, 0f, fadeTime);
        woskni.SoundManager.ChangeVolume(nextMusic.audio.clip, volume, fadeTime);
    }

    void PlayTitleBGM(System.DateTime time)
    {
        //  6�� �` 18�� �͏��
        if (new woskni.RangeInt(6, 18 - 1).IsIn(time.Hour))
            woskni.SoundManager.Play("�������ƃo�E���e�B", true, SaveSystem.m_SaveData.BGMvolume);
        // 18�� �`  6�� �͉���
        else
            woskni.SoundManager.Play("�������ƃo�E���e�B", true, SaveSystem.m_SaveData.BGMvolume);
    }

    /// <summary>��ʕύX</summary>
    /// <param name="state">�ύX���������</param>
    public void ChangeWindowState(WINDOW_STATE state)
    {
        m_BeforeWindowState = m_WindowState;
        m_WindowState = state;

        m_MoveTimer.Reset();

        //�ЂƂO�̃X�e�[�g�̈ʒu���擾
        switch (m_BeforeWindowState)
        {
            case WINDOW_STATE.Title:        m_StartPosition = m_FirstTitlePosition;       break;
            case WINDOW_STATE.FieldSelect:  m_StartPosition = m_FirstFieldSelectPosition; break;
            case WINDOW_STATE.Settings:     m_StartPosition = m_FirstSettingsPosition;    break;
        }

        //���̃X�e�[�g�̈ʒu���擾
        switch (m_WindowState)
        {
            case WINDOW_STATE.Title:        m_EndPosition = m_FirstTitlePosition;        break;
            case WINDOW_STATE.FieldSelect:  m_EndPosition = m_FirstFieldSelectPosition;  break;
            case WINDOW_STATE.Settings:     m_EndPosition = m_FirstSettingsPosition;     break;
        }
    }

    /// <summary>��ʕύX(�t�B�[���h�I��)</summary>
    public void ChangeWindowState_FieldSelect()
    {
        ChangeWindowState(WINDOW_STATE.FieldSelect);
    }

    /// <summary>��ʕύX(�ݒ���)</summary>
    public void ChangeWindowState_Settings()
    {
        ChangeWindowState(WINDOW_STATE.Settings);
    }

    /// <summary>��ʂ���O�ɖ߂�</summary>
    public void UndoWindowState()
    {
        ChangeWindowState(m_BeforeWindowState);
        SaveSystem.Save();
    }
}

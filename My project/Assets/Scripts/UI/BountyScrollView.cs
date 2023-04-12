using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//�o�E���e�B�X�N���[���r���[�N���X
public class BountyScrollView : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI m_BountyIndexText       = null;         //�o�E���e�B�ԍ�������
    [SerializeField] GameObject      m_BountyPrefab          = null;         //�o�E���e�B�̃v���n�u
    [SerializeField] Transform       m_Contents              = null;         //�R���e���c
    [SerializeField] float           m_DragLength            = 1.25f;        //�h���b�O����
    [SerializeField] float           m_ScrollTime            = 0.25f;        //�ړ�����
    [SerializeField] float           m_FlickTime             = 0.75f;        //�t���b�N����
    [SerializeField] float           m_ScrollAngle           = 10.0f;        //�X�N���[���p�x
    [SerializeField] float           m_MaxScale              = 1.1f;         //�X�P�[���ő�l
    [SerializeField] float           m_MinScale              = 0.8f;         //�X�P�[���ŏ��l
    RectTransform[]                  m_BountyArray           = null;         //�o�E���e�B�z��
    Vector3                          m_PointerDownPosition   = Vector3.zero; //�|�C���^�[�����ʒu
    Vector3                          m_PointerUpPosition     = Vector3.zero; //�|�C���^�[�ŏI�ʒu
    float                            m_ContentsStartAngle    = 0f;           //�R���e���c�����p�x
    float                            m_ContentsLastAngle     = 0f;           //�R���e���c�ŏI�p�x
    bool                             m_ScrollFlag            = false;        //�X�N���[�����t���O
    bool                             m_PointerDown           = false;        //�|�C���^�[�����t���O
    int                              m_ContentsIndex         = 0;            //�R���e���c�̔ԍ�
    int                              m_BountyIndex           = 0;            //�o�E���e�B�̔ԍ�
    int                              m_BeforeBountyIndex     = 0;            //1�t���[���O�̃o�E���e�B�̔ԍ�
    woskni.Timer                     m_ScrollTimer;                          //�X�N���[���^�C�}�[
    woskni.Timer                     m_FlickTimer;                           //�t���b�N�^�C�}�[

    [HideInInspector]
    public bool                      m_BountyUpdatingFlag    = false;        //�o�E���e�B�X�V���t���O

    void Start()
    {
        //�q���̑傫����ς���
        foreach (Transform child in m_Contents)
        {
            Vector3 scale = new Vector3(m_MinScale, m_MinScale, 1.0f);
            child.localScale = scale;
        }

        //�^�C�}�[�ݒ�
        m_ScrollTimer.Setup(m_ScrollTime);
        m_FlickTimer.Setup(m_FlickTime);

        //�v���n�u����
        int bountyCount = SaveData.m_bounty_count;
        m_BountyArray = new RectTransform[bountyCount];
        for (int i = 0; i < bountyCount; ++i)
        {
            m_BountyArray[i] = Instantiate(m_BountyPrefab).GetComponent<RectTransform>();
            m_BountyArray[i].gameObject.SetActive(false);
            m_BountyArray[i].name = "Bounty_" + (i + 1).ToString();
            m_BountyArray[i].SetParent(transform);
            m_BountyArray[i].localPosition = Vector3.zero;
            m_BountyArray[i].localScale = new Vector3(m_MinScale, m_MinScale, 1.0f);
            BountyPanel bountyPanel = m_BountyArray[i].GetComponent<BountyPanel>();
            bountyPanel.Initialize(i);
        }

        //�ŏ��̈ʒu�Ɛe����
        FirstSetBountyPosition_and_Parent(TemporarySavingBounty.bountyIndex);
    }

    void Update()
    {
        //�|�C���^�[�������Ȃ�^�C�}�[��i�߂�
        if (m_PointerDown)
            m_FlickTimer.Update();

        //�X�N���[�����Ȃ�X�N���[�������Ɗg�k����������
        if (m_ScrollFlag)
        {
            Scaling();
            Scroll();
        }

        //�o�E���e�B�ԍ��𕶎���ɕϊ�����(0����n�܂�̂�+1)
        int bountyCount = SaveData.m_bounty_count;
        m_BountyIndexText.text = (m_BountyIndex + 1).ToString() + "/" + bountyCount;
    }

    //�X�N���[������
    void Scroll()
    {
        m_ScrollTimer.Update();

        //�C�[�W���O�Ŋp�x�X�V
        float limit = m_ScrollTimer.limit;
        float time  = m_ScrollTimer.time;
        Vector3 rotate = m_Contents.localEulerAngles;
        rotate.z = woskni.Easing.OutSine(time, limit, m_ContentsStartAngle, m_ContentsLastAngle);
        m_Contents.localEulerAngles = rotate;

        //�^�C�}�[�I��
        if (m_ScrollTimer.IsFinished())
        {
            //��ԃ��Z�b�g
            m_ScrollTimer.Reset();
            m_ScrollFlag = false;
            m_Contents.localEulerAngles = new Vector3(0, 0, m_ContentsLastAngle);
        }
    }

    //�X�P�[�����O����
    void Scaling()
    {
        //�ϐ��擾
        float limit = m_ScrollTimer.limit;
        float time  = m_ScrollTimer.time;

        //�C�[�W���O�Ŋg�k
        Vector3 scale = Vector3.one;
        scale.x = scale.y = woskni.Easing.OutSine(time, limit, m_MaxScale, m_MinScale);
        m_BountyArray[m_BeforeBountyIndex].parent.localScale = scale;
        scale.x = scale.y = woskni.Easing.OutSine(time, limit, m_MinScale, m_MaxScale);
        m_BountyArray[m_BountyIndex].parent.localScale = scale;
    }

    //�|�C���^�[�������ꂽ
    public void PointerDown()
    {
        //�^�b�`�ʒu�ۑ�
        m_PointerDownPosition = woskni.InputManager.GetInputPosition(Application.platform);

        //�^�C�}�[���Z�b�g
        m_FlickTimer.Reset();

        m_PointerDown = true;

        //�o�E���e�B�p�l���̃T�C�Y���擾
        Vector2 size = m_BountyArray[m_BountyIndex].sizeDelta;
        //�}�E�X�J�[�\���Ƃ̈ʒu�֌W���v�Z
        Vector3 pos = m_PointerDownPosition - m_BountyArray[m_BountyIndex].position;
        //�p�l���̓��������肷��
        bool isInside = true;
        isInside &= Mathf.Abs(pos.x) < size.x * m_MaxScale / 2;
        isInside &= Mathf.Abs(pos.y) < size.y * m_MaxScale / 2;

        m_BountyArray[m_BountyIndex].GetComponent<BountyPanel>().m_IsGriped = isInside;
    }

    //�|�C���^�[�����ꂽ
    public void PointerUp()
    {
        m_PointerDown = false;

        m_BountyArray[m_BountyIndex].GetComponent<BountyPanel>().m_IsGriped = m_PointerDown;

        //�X�N���[�����͉������Ȃ�
        if (m_ScrollFlag) return;

        //��莞�Ԉȏ�^�b�`���Ă�����I��
        if (m_FlickTimer.IsFinished()) return;

        //�o�E���e�B�X�V���Ȃ�I��
        if (m_BountyUpdatingFlag) return;

        //�^�b�`�ʒu�ۑ�
        m_PointerUpPosition = woskni.InputManager.GetInputPosition(Application.platform);

        //�������v�Z
        Vector3 increment = m_PointerUpPosition - m_PointerDownPosition;

        //�������̍������K�v�����ɖ����Ȃ�������I��
        if (Mathf.Abs(increment.x) < m_DragLength)
        {
            //�o�E���e�B�p�l���̃T�C�Y���擾
            Vector2 size = m_BountyArray[m_BountyIndex].sizeDelta;
            //�}�E�X�J�[�\���Ƃ̈ʒu�֌W���v�Z
            Vector3 pos = m_PointerUpPosition - m_BountyArray[m_BountyIndex].position;
            //�p�l���̓��������肷��
            bool isInside = true;
            isInside &= Mathf.Abs(pos.x) < size.x * m_MaxScale / 2;
            isInside &= Mathf.Abs(pos.y) < size.y * m_MaxScale / 2;

            //�p�l�����N���b�N�����Ɣ��肷��
            if (isInside)
                m_BountyArray[m_BountyIndex].GetComponent<BountyPanel>().ButtonDown();

            return;
        }

        //�����p�x���
        m_ContentsStartAngle = m_Contents.localEulerAngles.z;

        //�ԍ��̕ۑ�
        m_BeforeBountyIndex = m_BountyIndex;

        //�ԍ��̉��Z
        m_BountyIndex   += increment.x < 0 ? +1 : -1;
        m_ContentsIndex += increment.x < 0 ? +1 : -1;

        //�ԍ��̕␳(�o�E���e�B�ԍ�)
        int bountyCount = SaveData.m_bounty_count;
        if (m_BountyIndex >= bountyCount)   m_BountyIndex = 0;
        else if (m_BountyIndex < 0)         m_BountyIndex = bountyCount - 1;

        //�ԍ��̕␳(�R���e���c�ԍ�)
        int child_count = m_Contents.childCount;
        if (m_ContentsIndex >= child_count) m_ContentsIndex = 0;
        else if (m_ContentsIndex < 0)       m_ContentsIndex = child_count - 1;

        //�ŏI�p�x�v�Z
        m_ContentsLastAngle = m_ContentsStartAngle + m_ScrollAngle * (increment.x < 0 ? +1 : -1);

        //�t���O���グ��
        m_ScrollFlag = true;

        //�ʒu�Ɛe����
        SetBountyPosition_and_Parent(increment.x < 0);
    }

    //�ŏ��̈ʒu�Ɛe����
    void FirstSetBountyPosition_and_Parent(int firstIndex)
    {
        //�o�E���e�B�ƃR���e���c�̗v�f�ԍ����v�Z����
        int firstContentsIndex  = m_Contents.childCount - 2;

        //��ʂɎʂ�3�Ƃ��ׂ̗�2�����߂�
        for (int i = 0; i < 5; i++)
        {
            //�v�f�ԍ����Z�o
            int index = firstIndex - 2 < 0 ? SaveData.m_bounty_count + (firstIndex - 2) : firstIndex - 2;
            int b_index = (index + i) % SaveData.m_bounty_count;
            int c_index = (firstContentsIndex + i) % m_Contents.childCount;

            //�e�ƃ��[�J���ʒu�E�p�x�E�g�嗦��ݒ肷��
            m_BountyArray[b_index].SetParent(m_Contents.GetChild(c_index));
            m_BountyArray[b_index].localPosition = Vector3.zero;
            m_BountyArray[b_index].localEulerAngles = Vector3.zero;
            m_BountyArray[b_index].parent.localScale =
                            i == 2 ? new Vector3(m_MaxScale, m_MaxScale, 1.0f) :
                                     new Vector3(m_MinScale, m_MinScale, 1.0f);

            //��Ԃ��A�N�e�B�u�ɂ���
            m_BountyArray[b_index].gameObject.SetActive(true);
        }

        //�ԍ���ݒ�
        m_BountyIndex = firstIndex;
    }

    //�ʒu�Ɛe����
    void SetBountyPosition_and_Parent(bool leftMove)
    {
        int bountyCount = SaveData.m_bounty_count;
        int sub;
        int firstBountyIndex;
        int lastBountyIndex;
        int firstContentsIndex;
        int lastContentsIndex;

        //������
        if (leftMove)
        {
            //���O���o�E���e�B�̏ꏊ�����
            sub = m_BountyIndex - 3;
            firstBountyIndex = sub >= 0 ? sub : bountyCount + sub;

            //�e�q�֌W���������Ĉʒu���Z�b�g
            m_BountyArray[firstBountyIndex].SetParent(transform);
            m_BountyArray[firstBountyIndex].localPosition = Vector3.zero;
            m_BountyArray[firstBountyIndex].gameObject.SetActive(false);

            //���t����o�E���e�B�̏ꏊ�����
            sub = m_ContentsIndex + 2;
            lastContentsIndex = sub % m_Contents.childCount;
            sub = m_BountyIndex + 2;
            lastBountyIndex = sub % bountyCount;

            //�e�q�֌W������Ĉʒu�ݒ�
            m_BountyArray[lastBountyIndex].SetParent(m_Contents.GetChild(lastContentsIndex));
            m_BountyArray[lastBountyIndex].localPosition = Vector3.zero;
            m_BountyArray[lastBountyIndex].localEulerAngles = Vector3.zero;
            m_BountyArray[lastBountyIndex].gameObject.SetActive(true);
        }
        //�E����
        else
        {
            //���O���o�E���e�B�̏ꏊ�����
            sub = m_BountyIndex + 3;
            lastBountyIndex = sub % bountyCount;

            //�e�q�֌W���������Ĉʒu���Z�b�g
            m_BountyArray[lastBountyIndex].SetParent(transform);
            m_BountyArray[lastBountyIndex].localPosition = Vector3.zero;
            m_BountyArray[lastBountyIndex].gameObject.SetActive(false);

            //���t����o�E���e�B�̏ꏊ�����
            sub = m_ContentsIndex - 2;
            firstContentsIndex = sub >= 0 ? sub : m_Contents.childCount + sub;
            sub = m_BountyIndex - 2;
            firstBountyIndex = sub >= 0 ? sub : bountyCount + sub;

            //�e�q�֌W������Ĉʒu�ݒ�
            m_BountyArray[firstBountyIndex].SetParent(m_Contents.GetChild(firstContentsIndex));
            m_BountyArray[firstBountyIndex].localPosition = Vector3.zero;
            m_BountyArray[firstBountyIndex].localEulerAngles = Vector3.zero;
            m_BountyArray[firstBountyIndex].gameObject.SetActive(true);
        }
    }

    /// <summary>�o�E���e�B���Z�b�g</summary>
    public void BountiesReset()
    {
        //�o�E���e�B����S�ă��Z�b�g
        for (int i = 0; i < SaveSystem.m_SaveData.bounties.Length; ++i)
        {
            SaveSystem.m_SaveData.bounties[i] = new Bounty();
            m_BountyArray[i].GetComponent<BountyPanel>().Initialize(i);
        }
    }
}

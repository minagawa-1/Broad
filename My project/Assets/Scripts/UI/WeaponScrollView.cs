using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//����X�N���[���r���[�N���X
public class WeaponScrollView : MonoBehaviour
{
    [SerializeField] NotAllowedBuyDialog
                                m_CanNotBuyDialog       = null;             //�w���s�_�C�A���O
    [SerializeField] GameObject m_WeaponBuyDialog       = null;             //����w���_�C�A���O
    [SerializeField] GameObject m_WeaponPrefab          = null;             //����v���n�u
    [SerializeField] Transform  m_Contents              = null;             //�R���e���c
    [SerializeField] WeaponInfo m_WeaponInfo            = null;             //������N���X
    [SerializeField] float      m_ScrollSpeed           = 0.25f;            //�ړ����x
    [SerializeField] float      m_ScalingSpeed          = 0.25f;            //�g�k���x
    [SerializeField] float      m_WeaponSize_X          = 250.0f;           //���퉡�T�C�Y
    [SerializeField] float      m_CotentInterval        = 50.0f;            //�R���e���c�̐���Ԋu
    [SerializeField] float      m_MaxScale              = 1.1f;             //�ő�X�P�[��
    [SerializeField] float      m_MinScale              = 0.8f;             //�ŏ��X�P�[��
    [SerializeField] float      m_SpeedDamping          = 0.98f;            //���x����
    [SerializeField] float      m_MinScrollVelocity     = 0.5f;             //�ŏ��X�N���[�����x
    [SerializeField] float      m_MinFlickLength        = 10.0f;            //�ŏ��t���b�N����
    [SerializeField] float      m_ClickMoveTime         = 1.0f;             //�N���b�N���̈ړ�����
    [SerializeField] float      m_BaseScreenWidth       = 1080f;            //��ʉ�����l
    [SerializeField] int        m_MaxWeaponCount        = 8;                //����ő��
    [Space(12)]
    public           WeaponData m_WeaponData            = null;             //����f�[�^
    Vector3                     m_PointerDownPosition   = Vector3.zero;     //�|�C���^�[�����ʒu
    Vector3                     m_PointerDragPosition   = Vector3.zero;     //���݃|�C���^�[�ʒu
    Vector3                     m_ContentsFirstPosition = Vector3.zero;     //�R���e���c�����ʒu(�Q�[���J�n��)
    Vector3                     m_ContentsStartPosition = Vector3.zero;     //�R���e���c�����ʒu
    Vector3                     m_ContentsLastPosition  = Vector3.zero;     //�R���e���c�ŏI�ʒu
    float                       m_ContentsEndPositionX  = 0.0f;             //�R���e���c�ړ����E(X��)
    float                       m_ScrollVelocity        = 0.0f;             //�R���e���c�X�N���[�����x
    float                       m_DragTime              = 0.0f;             //�h���b�O����
    bool                        m_PointerDownFlag       = false;            //�|�C���^�[�����t���O
    int                         m_CenterWeaponIndex     = 0;                //���S�ɗ��镐��̔ԍ�
    int                         m_BeforeWeaponIndex     = 0;                //1�t���[���O�ɒ��S�ɂ�������̔ԍ�
    woskni.Timer                m_ClickMoveTimer;                           //�N���b�N�ړ��^�C�}�[

    void Start()
    {
        //�^�C�}�[�ݒ�
        m_ClickMoveTimer.Setup(m_ClickMoveTime);

        //�w���_�C�A���O�͕\���������Ă���
        m_WeaponBuyDialog.SetActive(false);

        //�����ʒu�ۑ�
        m_ContentsFirstPosition = m_Contents.localPosition;
        m_ContentsStartPosition = m_ContentsFirstPosition;
        m_ContentsLastPosition  = m_ContentsFirstPosition;

        //���E�ʒu�v�Z
        m_ContentsEndPositionX = m_ContentsFirstPosition.x -
                                    ((m_WeaponSize_X + m_CotentInterval) * (m_MaxWeaponCount - 1));

        //����p�l������
        List<WeaponData.Weapon> weaponList = m_WeaponData.GetWeaponList();
        for (int i = 0; i < m_MaxWeaponCount; ++i)
        {
            GameObject obj = Instantiate(m_WeaponPrefab, m_Contents);
            obj.name = "Weapon_" + (i + 1).ToString();
            obj.transform.localScale = new Vector3(m_MinScale, m_MinScale, 1.0f);
            obj.GetComponent<WeaponPanel>().Setup(i, weaponList[i].weaponImage, this);
        }

        //�擪�̗v�f�����傫������
        m_Contents.GetChild(0).localScale = new Vector3(m_MaxScale, m_MaxScale, 1.0f);

        //������N���X�ɏ����l������
        m_WeaponInfo.SetWeaponIndex(0);

        //�ŏ��̕�������炩���ߑI��ł���(�����t���O���グ��)
        TemporarySavingBounty.equipWeapon = m_WeaponData.GetWeaponAt(SaveSystem.m_SaveData.eqipWeaponID);

        //���S�̕���̔ԍ����擾
        m_CenterWeaponIndex = SaveSystem.m_SaveData.eqipWeaponID;
        Vector3 pos = m_Contents.localPosition;
        pos.x = m_ContentsFirstPosition.x - (m_WeaponSize_X + m_CotentInterval) * SaveSystem.m_SaveData.eqipWeaponID;
        m_Contents.localPosition = pos;
        m_ContentsStartPosition = pos;
        m_ContentsLastPosition = pos;
    }

    void Update()
    {
        //�q�v�f�̃X�P�[�����O
        ChildScaling();

        //�^�C�}�[���ғ����Ȃ�C�[�W���O�ňړ�
        if (!m_ClickMoveTimer.IsFinished())
        {
            m_ClickMoveTimer.Update();
            Vector3 pos = m_Contents.localPosition;
            float time  = m_ClickMoveTimer.time;
            float limit = m_ClickMoveTimer.limit;
            pos.x = woskni.Easing.OutSine(time, limit, m_ContentsStartPosition.x, m_ContentsLastPosition.x);
            m_Contents.localPosition = pos;
            return;
        }

        //�������͉������Ȃ�
        if (m_PointerDownFlag)
        {
            //�|�C���^�[�ʒu�擾
            m_PointerDragPosition = woskni.InputManager.GetInputPosition(Application.platform);
            return;
        }

        //�X�N���[�����x�����ȏ゠��
        if (Mathf.Abs(m_ScrollVelocity) > m_MinScrollVelocity)
        {
            //���x�����݈ʒu�ɉ��Z(���x������������)
            m_Contents.localPosition += new Vector3(m_ScrollVelocity * Time.deltaTime, 0f, 0f);

            //�͈͂��O��Ȃ��悤�ɕ␳
            Vector3 pos = m_Contents.localPosition;
            pos.x = Mathf.Clamp(pos.x, m_ContentsEndPositionX, m_ContentsFirstPosition.x);
            m_Contents.localPosition = pos;
        }
        //�X�N���[�����x���قƂ�ǂȂ�
        else
        {
            //�ۑ�����X�ʒu�܂Ŋ��炩�ɓ���
            Vector3 pos = m_Contents.localPosition;
            pos.x = Mathf.MoveTowards(pos.x, m_ContentsLastPosition.x, m_ScrollSpeed * Time.deltaTime);
            m_Contents.localPosition = pos;

            //�����ʒu�ۑ�
            m_ContentsStartPosition = m_Contents.localPosition;

            //���x�폜
            m_ScrollVelocity = 0.0f;
        }
    }

    private void FixedUpdate()
    {
        //�^�C�}�[���ғ����Ȃ牽�����Ȃ�
        if (!m_ClickMoveTimer.IsFinished()) return;

        //�������͉������Ȃ�
        if (m_PointerDownFlag)
        {
            //���ԉ��Z
            m_DragTime += Time.fixedDeltaTime;
            return;
        }

        //�X�N���[�����x��������x���鎞�͌���������
        if (Mathf.Abs(m_ScrollVelocity) > m_MinScrollVelocity)
        {
            m_ScrollVelocity *= m_SpeedDamping;
            //���x�`�F�b�N
            CheckVelocity();
        }
    }

    /// <summary>���݂̑��x�`�F�b�N</summary>
    void CheckVelocity()
    {
        //���x���܂���������I��
        if (Mathf.Abs(m_ScrollVelocity) > m_MinScrollVelocity) return;

        //���x���Ȃ��Ȃ����畐��̔ԍ����擾����
        for (int i = 0; i < m_MaxWeaponCount; ++i)
        {
            //�����ʒu���瓙�Ԋu�̈ʒu���o��
            float increment_x = (m_WeaponSize_X + m_CotentInterval) * i;
            Vector3 point = m_ContentsFirstPosition - new Vector3(increment_x, 0f, 0f);

            //�u�p�l���ʒu�v���傫��
            bool min = point.x - (m_WeaponSize_X + m_CotentInterval) / 2 <= m_Contents.localPosition.x;
            //�u�p�l���ʒu + �T�C�Y�v��菬����
            bool max = point.x + (m_WeaponSize_X + m_CotentInterval) / 2 >= m_Contents.localPosition.x;

            //�ʒu������p�l���͈͓̔�
            if (min && max)
            {
                //��ԋ߂����ڂ̈ʒu��ۑ����ďI��
                Vector3 pos = m_Contents.localPosition;
                pos.x = m_ContentsFirstPosition.x - (m_WeaponSize_X + m_CotentInterval) * i;
                m_ContentsLastPosition = pos;
                m_CenterWeaponIndex = i;
                return;
            }
        }
    }

    /// <summary>����p�l�����N���b�N���ꂽ</summary>
    /// <param name="weapon_id">����ID</param>
    public void PanelClick(int weapon_id)
    {
        //�^�C�}�[���ғ����Ȃ牽�����Ȃ�
        if (!m_ClickMoveTimer.IsFinished()) return;

        //�N���b�N���ꂽ����p�l���̔ԍ��ƍ����S�ɂ��镐��p�l���̔ԍ����Ⴄ
        if (weapon_id != m_CenterWeaponIndex)
        {
            //�N���b�N���ꂽ���ڂ̈ʒu��ۑ�
            Vector3 pos = m_Contents.localPosition;
            pos.x = m_ContentsFirstPosition.x - (m_WeaponSize_X + m_CotentInterval) * weapon_id;
            m_ContentsLastPosition = pos;
            //���x������
            m_ScrollVelocity = 0.0f;
            //�^�C�}�[���Z�b�g
            m_ClickMoveTimer.Reset();
            //�N���b�N���ꂽ����̔ԍ���ۑ�
            m_CenterWeaponIndex = weapon_id;
        }
        //�N���b�N���ꂽ����p�l���̔ԍ��ƍ����S�ɂ��镐��p�l���̔ԍ�������
        else
        {
            //���łɍw���ς݂Ȃ炱���ŏI��
            if (SaveSystem.m_SaveData.hasWeapons[weapon_id])
            {
                //�I�񂾕���𑕔�����(�����t���O���グ��)
                TemporarySavingBounty.equipWeapon = m_WeaponData.GetWeaponAt(weapon_id);
                SaveSystem.m_SaveData.eqipWeaponID = weapon_id;
                SaveSystem.Save();

                return;
            }

#if true
            if (m_WeaponData.GetWeaponAt(weapon_id).presetIndex.Count <= 0)
            {
                m_CanNotBuyDialog.ShowDialog();
                return;
            }
#endif

            //����w���_�C�A���O�\��
            m_WeaponBuyDialog.SetActive(true);
            m_WeaponBuyDialog.GetComponent<WeaponBuyDialog>().ShowDialog(weapon_id);
        }
    }

    /// <summary>�q�v�f�̃X�P�[�����O</summary>
    void ChildScaling()
    {
        int num = 0;
        for (int i = 0; i < m_MaxWeaponCount; ++i)
        {
            //�����ʒu���瓙�Ԋu�̈ʒu���o��
            float increment_x = (m_WeaponSize_X + m_CotentInterval) * i;
            Vector3 point = m_ContentsFirstPosition - new Vector3(increment_x, 0f, 0f);

            //�����ʒu���傫��
            bool min = point.x - (m_WeaponSize_X + m_CotentInterval) / 2 <= m_Contents.localPosition.x;
            //�����ʒu+�T�C�Y��菬����
            bool max = point.x + (m_WeaponSize_X + m_CotentInterval) / 2 >= m_Contents.localPosition.x;

            //�Y������q�����擾
            Transform child = m_Contents.GetChild(i);

            //�͈͓�
            if (min && max)
            {
                //���炩�ɑ傫������
                Vector3 scale = child.localScale;
                scale.x = scale.y = Mathf.MoveTowards(scale.x, m_MaxScale, m_ScalingSpeed * Time.deltaTime);
                child.localScale = scale;

                //�ԍ��̕ۑ�
                num = i;
            }
            //�͈͊O
            else
            {
                //���炩�ɏ���������
                Vector3 scale = child.localScale;
                scale.x = scale.y = Mathf.MoveTowards(scale.x, m_MinScale, m_ScalingSpeed * Time.deltaTime);
                child.localScale = scale;
            }
        }

        //����̔ԍ����ς�����u��
        if (m_BeforeWeaponIndex != num)
            //������N���X�ɒ��S�ɂ��镐��̔ԍ���n��
            m_WeaponInfo.SetWeaponIndex(num);

        //1�t���[���O�̔ԍ���ۑ�
        m_BeforeWeaponIndex = num;
    }

    /// <summary>�|�C���^�[�������ꂽ</summary>
    public void PointerDown()
    {
        //�^�b�`�ʒu�ۑ�
        m_PointerDownPosition = woskni.InputManager.GetInputPosition(Application.platform);
        m_PointerDragPosition = m_PointerDownPosition;

        //���x����
        m_ScrollVelocity = 0.0f;

        //���ԏ���
        m_DragTime = 0.0f;

        //�t���O���グ��
        m_PointerDownFlag = true;

        //�����ʒu�ۑ�
        m_ContentsStartPosition = m_Contents.localPosition;
    }

    /// <summary>�|�C���^�[�����ꂽ</summary>
    public void PointerUp()
    {
        //���x�Z�o
        Vector3 nowPointerPos = woskni.InputManager.GetInputPosition(Application.platform);
        float increment = (nowPointerPos - m_PointerDownPosition).x / (Screen.width / m_BaseScreenWidth);

        //�t���b�N���������ȏ゠��
        if (Mathf.Abs(increment) > m_MinFlickLength)
            m_ScrollVelocity = increment / m_DragTime;

        //�t���O��������
        m_PointerDownFlag = false;
    }

    /// <summary>�|�C���^�[���h���b�O</summary>
    public void PointerDrag()
    {
        //�����v�Z
        Vector3 increment = Vector3.zero;
        increment.x = (m_PointerDragPosition - m_PointerDownPosition).x / (Screen.width / m_BaseScreenWidth);

        //�ʒu���
        m_Contents.localPosition = m_ContentsStartPosition + increment;

        //�͈͂��O��Ȃ��悤�ɕ␳
        Vector3 pos = m_Contents.localPosition;
        pos.x = Mathf.Clamp(pos.x, m_ContentsEndPositionX, m_ContentsFirstPosition.x);

        //�␳�l���
        m_Contents.localPosition = pos;
    }

    /// <summary>�X�N���[���ʒu���Z�b�g</summary>
    public void PositionReset()
    {
        m_Contents.localPosition = m_ContentsFirstPosition;
    }
}

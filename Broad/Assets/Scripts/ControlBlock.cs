using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ControlBlock : MonoBehaviour
{
    bool[,] sampleBlocks = new bool[,] {
            {false, true},
            {false, true},
            {true , true}
        };

    [Header("�R���|�[�l���g")]
    [SerializeField] GameManager m_GameManager = null;

    [Header("�ړ��Ԋu�E�ړ�����")]
    [SerializeField] float m_MoveInterval = 0.1f;
    [SerializeField] float m_MoveTime = 0.05f;

    // ����\��
    bool m_IsOperatable = true;

    woskni.Timer m_KeyRepeatTimer;

    // �u���b�N���
    Blocks m_Blocks;

    // Start is called before the first frame update
    void Start()
    {
        m_Blocks = new Blocks(sampleBlocks);

        m_KeyRepeatTimer.Setup(m_MoveInterval);

        // �����ʒu�̐ݒ�(�{�[�h�̃T�C�Y�������������0.5f���炷�����߂�)
        transform.position = transform.position.Difference(x: m_GameManager.m_BoardSize.x % 2 == 0 ? 0.5f : 0f
                                                         , z: m_GameManager.m_BoardSize.y % 2 == 0 ? 0.5f : 0f);
    }

    // Update is called once per frame
    void Update()
    {
        // �㉺���E�̈ړ������iEase.OutCubic�E���Έړ�)
        if (KeyRepeat(KeyCode.UpArrow    , KeyCode.W)) Move( 0,  1, m_MoveTime);
        if (KeyRepeat(KeyCode.LeftArrow  , KeyCode.A)) Move(-1,  0, m_MoveTime);
        if (KeyRepeat(KeyCode.DownArrow  , KeyCode.S)) Move( 0, -1, m_MoveTime);
        if (KeyRepeat(KeyCode.RightArrow , KeyCode.D)) Move( 1,  0, m_MoveTime);
    }

    bool KeyRepeat(params KeyCode[] key)
    {
        // �L�[�������Ă��Ȃ����false��Ԃ�
        if (!woskni.KeyBoard.GetOrKey(key)) return false;

        // �^�C�}�[�X�V
        m_KeyRepeatTimer.Update();

        // ���s�[�g�Ԋu�ɒB�����Ƃ��^�C�}�[�����Z�b�g����true��Ԃ��B
        if (m_KeyRepeatTimer.IsFinished()) {
            m_KeyRepeatTimer.Reset();
            return true;
        }

        // �ǂ���ł��Ȃ���΁A�������u�Ԃ����m
        return woskni.KeyBoard.GetOrKeyDown(key);
    }

    /// <summary>�ړ�����</summary>
    /// <param name="move">�ړ��ʁi�}�X���j</param>
    void Move(Vector2Int move = new Vector2Int(), float moveTime = 0.1f)
    {
        // �ړ�
        bool[] isCollision = IsCollisionMoveLimit();

        // �㉺���E�̕ǌ��m
        if (isCollision[0] && move.y > 0) return;
        if (isCollision[1] && move.x < 0) return;
        if (isCollision[2] && move.y < 0) return;
        if (isCollision[3] && move.x > 0) return;

        // ����s�Ȃ�Ԃ�
        if (!m_IsOperatable) return;

        // ����s�\�ɂ���
        m_IsOperatable = false;

        // Vector3�ɕϊ�
        Vector3 moveVector3 = new Vector3(m_GameManager.m_SquareSize.x * move.x, 0f, m_GameManager.m_SquareSize.y * move.y);

        transform.DOLocalMove(moveVector3, moveTime).SetEase(Ease.OutCubic).SetRelative().OnComplete(() => m_IsOperatable = true);
    }

    /// <summary>�ړ�����</summary>
    /// <param name="x">���ړ��ʁi�}�X���j</param>
    /// <param name="y">���ړ��ʁi�}�X���j></param>
    private void Move(int x = 0, int y = 0, float moveTime = 0.1f) => Move(new Vector2Int(x, y), moveTime);

    /// <summary> �ǌ��m </summary>
    /// <returns>�ǂ̗L���@[0]: ��, [1]: ��, [2]: ��, [3]: �E</returns>
    private bool[] IsCollisionMoveLimit()
    {
        bool[] isCollision = new bool[4];

        Vector2 blocksSize = new Vector2((float)m_Blocks.GetWidth(), (float)m_Blocks.GetHeight());
        Vector2 maxDinstance = ((Vector2)m_GameManager.m_BoardSize / 2f) - blocksSize / 2f;

        Debug.Log(maxDinstance);

        isCollision[0] = transform.position.x >= maxDinstance.x;    // ��
        isCollision[1] = transform.position.y < -maxDinstance.y;    // ��
        isCollision[2] = transform.position.y < -maxDinstance.x;    // ��
        isCollision[3] = transform.position.y >= maxDinstance.y;    // �E

        return isCollision;
    }
}

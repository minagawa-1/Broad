using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ControlBlock : MonoBehaviour
{
    const float m_move_interval = 0.2f;
    const float m_move_time = 0.05f;

    /// <summary> �u���b�N�̏��</summary>
    enum BlocksState
    {
        /// <summary> �ҋ@ </summary>
        Wait,
        /// <summary> �ړ� </summary>
        Move,
        /// <summary> �U�� </summary>
        Vibrate,
        /// <summary> �ݒu </summary>
        Set,
        /// <summary> ��] </summary>
        Rotate,
    }

    // �u���b�N�̏��
    BlocksState m_BlocksState = BlocksState.Wait;

    // GameManager�R���|�[�l���g
    GameManager m_GameManager = null;

    // ����\��
    bool m_IsOperatable = true;

    woskni.Timer m_KeyRepeatTimer;

    // �u���b�N���
    public Blocks m_Blocks;

    // �v���C���[�ԍ�
    public int playerIndex;

    ///<summary>�ݒu��̃I�u�W�F�N�g</summary>
    public GameObject m_AfterSetParent;

    // �ړ�����
    Vector2Int m_MoveDirection;



    // Start is called before the first frame update
    void Start()
    {
        m_GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        m_KeyRepeatTimer.Setup(m_move_interval);
        m_BlocksState = BlocksState.Wait;

        m_MoveDirection = Vector2Int.zero;
    }

    // Update is called once per frame
    void Update()
    {
        // BlockState�ɉ������֐����Ă�
        switch (m_BlocksState)
        {
            case BlocksState.Wait:      WaitState(); break;
            case BlocksState.Move:      MoveState(); break;
            case BlocksState.Vibrate:   VibrateState(); break;
            case BlocksState.Set:       SetState(); break;
        }
    }

    void WaitState()
    {
        // Z�L�[ �� Enter�L�[ �������ꂽ�� �u���b�N�X��ݒu
        if (woskni.KeyBoard.GetOrKeyDown(KeyCode.Z, KeyCode.Return))
        {
            // �ݒu���������true�Ȃ�BlocksState��Set�ɕύX
            if ( m_Blocks.IsSetable( m_GameManager.m_Board, m_Blocks, GetBoardPosition(transform.position), playerIndex))
                m_BlocksState = BlocksState.Set;

            // �ݒu���ł��Ȃ��ꍇ�͐U��
            else
            {
                m_BlocksState = BlocksState.Vibrate;
                m_MoveDirection = Vector2Int.one;
            }
        }

        // �㉺���E�̈ړ������iEase.OutCubic�E���Έړ�)
        if (KeyRepeat(KeyCode.UpArrow   , KeyCode.W)) ChangeState(Vector2Int.up);
        if (KeyRepeat(KeyCode.LeftArrow , KeyCode.A)) ChangeState(Vector2Int.left);
        if (KeyRepeat(KeyCode.DownArrow , KeyCode.S)) ChangeState(Vector2Int.down);
        if (KeyRepeat(KeyCode.RightArrow, KeyCode.D)) ChangeState(Vector2Int.right);


        // ��Ԃ�ύX
        void ChangeState(Vector2Int move)
        {
            m_MoveDirection = move;

            int num = -1;
            if (move == Vector2Int.up) num = 0;
            if (move == Vector2Int.left) num = 1;
            if (move == Vector2Int.down) num = 2;
            if (move == Vector2Int.right) num = 3;

            // ���͂��ꂽ�����̕ǌ��m�����ď�Ԃ�ς���
            m_BlocksState = IsCollisionMoveLimit()[num] ? BlocksState.Vibrate : BlocksState.Move;
        }
    }

    /// <summary> �ړ���� </summary>
    void MoveState()
    {
        // �ړ�����
        Move(m_MoveDirection, m_move_time);
    }

    /// <summary> �U����� </summary>
    void VibrateState()
    {
        Vector3 vibrateRate = new Vector3(m_MoveDirection.x, 0.2f, m_MoveDirection.y);

        // �U������
        if (m_IsOperatable) transform.Vibrate(0.2f, vibrateRate, m_move_interval);

        // ����s�\�ɂ���
        m_IsOperatable = false;

        // �U���I�����m
        if (!transform.IsVibrating())
        {
            // �ړ��\�ɂ��A�ړ��������[���ɂ���
            m_IsOperatable = true;
            m_MoveDirection = Vector2Int.zero;

            // State��Wait�ɖ߂���return
            m_BlocksState = BlocksState.Wait;
            return;
        }
    }

    /// <summary> �ݒu��� </summary>
    void SetState()
    {
        // ����s�\�Ȃ�I��
        if (!m_IsOperatable) return;

        // ����s�\�ɂ���
        m_IsOperatable = false;

        // �C�[�W���O (�I����A�Ֆʂɔ��f�����ďI��)
        transform.DOMoveY(0f, m_move_interval).SetEase(Ease.InQuart).OnComplete(() =>
            {
                // �ݒu��Ԃɂ���
                m_IsOperatable = true;

                // �ݒu����
                Set();
            });
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
        // �ړ����͉��������I��
        if (!m_IsOperatable) return;

        // �ړ�
        bool[] isCollision = IsCollisionMoveLimit();

        // Vector3�ɕϊ�
        Vector3 moveVector3 = new Vector3(m_GameManager.m_SquareSize.x * move.x, 0f, m_GameManager.m_SquareSize.y * move.y);

        // ����s�\�ɂ���
        m_IsOperatable = false;

        transform.DOLocalMove(moveVector3, moveTime).SetEase(Ease.OutCubic).SetRelative()
            .OnComplete(() =>
            {
                // �ړ��\�ɂ��A�ړ��������[���ɂ���
                m_IsOperatable = true;
                m_MoveDirection = Vector2Int.zero;

                // State��Wait�ɖ߂�
                m_BlocksState = BlocksState.Wait;
            });
    }

    /// <summary>�ړ�����</summary>
    /// <param name="x">���ړ��ʁi�}�X���j</param>
    /// <param name="y">���ړ��ʁi�}�X���j></param>
    private void Move(int x = 0, int y = 0, float moveTime = 0.1f) => Move(new Vector2Int(x, y), moveTime);

    Vector2Int GetBoardPosition(Vector3 position)
    {
        var pos = new Vector2Int((int)transform.position.x, (int)-transform.position.z);
        return pos - new Vector2Int(m_Blocks.GetWidth() / 2, m_Blocks.GetHeight() / 2);
    }

    void OutputDebugText()
    {
        string debugText = "";

        for (int y = 0; y < m_GameManager.m_BoardSize.y; ++y) {
            for (int x = 0; x < m_GameManager.m_BoardSize.x; ++x)
                debugText += m_GameManager.m_Board[x, y].state < 0 ? "-1" : " " + m_GameManager.m_Board[x, y].state;

            debugText += "\n";
        }

        Debug.Log(debugText);

        TextOperate.WriteFile("debugText.txt", debugText);
    }

    /// <summary> �ǌ��m </summary>
    /// <returns>�ǂ̗L���@[0]: ��, [1]: ��, [2]: ��, [3]: �E</returns>
    private bool[] IsCollisionMoveLimit()
    {
        // �����蔻��p�z��
        bool[] isCollision = new bool[4];

        // �u���b�N�T�C�Y
        Vector2 blocksSize = new Vector2(m_Blocks.GetWidth(), m_Blocks.GetHeight());

        Vector2 limitLeftTop = Vector2.zero - (blocksSize / 2f);                    // ����̈ړ����E
        Vector2 limitRightBottom = m_GameManager.m_BoardSize - (blocksSize / 2f); // �E���̈ړ����E

        isCollision[0] = transform.position.z >= limitLeftTop.y;        // ��
        isCollision[1] = transform.position.x <= -limitLeftTop.x;       // ��
        isCollision[2] = transform.position.z <= -limitRightBottom.y;   // ��
        isCollision[3] = transform.position.x >= limitRightBottom.x;    // �E

        return isCollision;
    }

    /// <summary> �ݒu���� </summary>
    private void Set()
    {
        var pos = GetBoardPosition(transform.position);

        for (int y = 0; y < m_Blocks.GetHeight(); ++y)
            for (int x = 0; x < m_Blocks.GetWidth(); ++x)
                if (m_Blocks.shape[x, y])
                    m_GameManager.m_Board[pos.x + x, pos.y + y].state = playerIndex;

        // �e��ς���
        GameObject[] children = gameObject.GetChildren();
        for (int i = 0; i < children.Length; ++i)
            children[i].transform.SetParent(m_AfterSetParent.transform);
    }
}

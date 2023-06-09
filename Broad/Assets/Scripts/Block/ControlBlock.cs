using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ControlBlock : MonoBehaviour
{
    const float m_move_interval = 0.2f;
    const float m_move_time = 0.05f;

// public:

    // �u���b�N���
    public Blocks blocks;

    // �v���C���[�ԍ�
    public int playerIndex;

    ///<summary>�ݒu��̃I�u�W�F�N�g</summary>
    public GameObject afterSetParent;

    ///<summary>�ݒu��̃}�e���A��</summary>
    public Material afterSetMaterial;

// private:

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
    BlockManager m_BlockManager = null;

    // ����\��
    bool m_IsOperatable = true;

    woskni.Timer m_KeyRepeatTimer;

    // �ړ�����
    Vector2Int m_MoveDirection;
    float      m_RotateDirection;

    // Start is called before the first frame update
    void Start()
    {
        m_GameManager = GameObject.Find(nameof(GameManager)).GetComponent<GameManager>();
        m_BlockManager = GameObject.Find(nameof(BlockManager)).GetComponent<BlockManager>();

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
            case BlocksState.Wait:      WaitState();    break;
            case BlocksState.Move:      MoveState();    break;
            case BlocksState.Vibrate:   VibrateState(); break;
            case BlocksState.Set:       SetState();     break;
            case BlocksState.Rotate:    RotateState();  break;
        }
    }

    void WaitState()
    {
        // X�L�[ �ŁA�u���b�N�X�𔼓����ɂ���
        if (Input.GetKeyDown(KeyCode.X))
        {
            var children = transform.GetChildren();
            GetComponent<ChangeTransparency>().Change(ref children);
        }

        // Z�L�[ �� Enter�L�[ �ŁA�u���b�N�X��ݒu
        if (woskni.KeyBoard.GetOrKeyDown(KeyCode.Z, KeyCode.Return))
        {
            // �ݒu���������true�Ȃ�BlocksState��Set�ɕύX
            if (blocks.IsSetable( m_GameManager.board, playerIndex))
                m_BlocksState = BlocksState.Set;

            // �ݒu���ł��Ȃ��ꍇ�͐U��
            else
            {
                m_BlocksState = BlocksState.Vibrate;
                m_MoveDirection = Vector2Int.one;
            }
        }

        if (KeyRepeat(KeyCode.Q)) ChangeRotateState(-90f);
        if (KeyRepeat(KeyCode.E)) ChangeRotateState( 90f);

        // �㉺���E�̈ړ������iEase.OutCubic�E���Έړ�)
        if (KeyRepeat(KeyCode.UpArrow   , KeyCode.W)) ChangeMoveState(Vector2Int.up);
        if (KeyRepeat(KeyCode.LeftArrow , KeyCode.A)) ChangeMoveState(Vector2Int.left);
        if (KeyRepeat(KeyCode.DownArrow , KeyCode.S)) ChangeMoveState(Vector2Int.down);
        if (KeyRepeat(KeyCode.RightArrow, KeyCode.D)) ChangeMoveState(Vector2Int.right);


        // ��Ԃ�ύX (BlocksState.Rotate)
        void ChangeRotateState(float rotate)
        {
            m_RotateDirection = rotate;

            m_BlocksState = BlocksState.Rotate;
        }

        // ��Ԃ�ύX (BlocksState.Move)
        void ChangeMoveState(Vector2Int move)
        {
            m_MoveDirection = move;

            int num = -1;
            if (move == Vector2Int.up   ) num = 0;
            if (move == Vector2Int.left ) num = 1;
            if (move == Vector2Int.down ) num = 2;
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
                // �ݒu����
                Set();

                // �ݒu��Ԃɂ���
                m_IsOperatable = true;
            });
    }

    /// <summary>��]���</summary>
    void RotateState()
    {
        if (!m_IsOperatable) return;

        m_IsOperatable = false;

        var rot = Vector3.zero.Offset(y: m_RotateDirection);

        transform.DOLocalRotate(rot, m_move_interval, RotateMode.FastBeyond360).SetEase(Ease.OutCubic)
            .SetRelative().OnComplete(() =>
            {
                // �z��̉�]����
                if (m_RotateDirection < 0f) blocks.RotateLeft();
                if (m_RotateDirection > 0f) blocks.RotateRight();

                blocks.position = GetBoardPosition(transform.position);

                OutputDebugText(false, "blockShape[,].txt");

                // �ړ��\�ɂ��A�ړ��������[���ɂ���
                m_IsOperatable = true;
                m_RotateDirection = 0f;

                // State��Wait�ɖ߂�
                m_BlocksState = BlocksState.Wait;
            });
    }

    /// <summary>�ړ�����</summary>
    /// <param name="move">�ړ��ʁi�}�X���j</param>
    void Move(Vector2Int move = new Vector2Int(), float moveTime = 0.1f)
    {
        // �ړ����͉��������I��
        if (!m_IsOperatable) return;

        // �ړ�
        bool[] isCollision = IsCollisionMoveLimit();

        // ����s�\�ɂ���
        m_IsOperatable = false;

        transform.DOLocalMove(GetVector3Board(move), moveTime).SetEase(Ease.OutCubic).SetRelative()
            .OnComplete(() =>
            {
                blocks.position = GetBoardPosition(transform.position);

                // �ړ��\�ɂ��A�ړ��������[���ɂ���
                m_IsOperatable = true;
                m_MoveDirection = Vector2Int.zero;

                // State��Wait�ɖ߂�
                m_BlocksState = BlocksState.Wait;
            });
    }

    /// <summary> �ݒu���� </summary>
    private void Set()
    {
        var pos = GetBoardPosition(transform.position);

        for (int y = 0; y < blocks.GetHeight(); ++y)
            for (int x = 0; x < blocks.GetWidth(); ++x)
                if (blocks.shape[x, y]) m_GameManager.board.SetBoardData(playerIndex, pos.x + x, pos.y + y);

        Transform[] children = transform.GetChildren();

        // �u���b�N�̔�������������
        GetComponent<ChangeTransparency>().Set(ref children, false);

        for (int i = 0; i < children.Length; ++i)
        {
            Vector3 local = children[i].transform.position - transform.position;

            int x = (int)( local.x + blocks.center.x + 0.51f);
            int y = (int)(-local.z + blocks.center.y + 0.51f);

            var boardPos = pos + new Vector2Int(x, y);

            string name = "Block[" + boardPos.x + ", " + boardPos.y + "]";

            // ���X���̔Ֆʂɂ����u���b�N��j������
            GameObject oldBlock = GameObject.Find(name);
            if (oldBlock) Destroy(oldBlock);

            // �I�u�W�F�N�g����Ֆʍ��W�̖��O�ɕύX����
            children[i].name = name;

            // �e��ς���
            children[i].transform.SetParent(afterSetParent.transform);

            // �}�e���A����ݒu��̂��̂ɕύX
            children[i].GetComponent<Renderer>().material = afterSetMaterial;
        }

        m_BlockManager.SortBlocks();

        OutputDebugText(true, "board[,].txt");

        // �R���g���[���p�I�u�W�F�N�g��j��
        Destroy(gameObject);
    }

    /// <summary>Vector3��Ֆʍ��W�ɕϊ�</summary>
    /// <param name="position">�R�������W</param>
    /// <returns>�Ֆʂ̂Q�������W</returns>
    Vector2Int GetBoardPosition(Vector3 pos)
    {
        var posBoard = new Vector2Int((int)pos.x, (int)-pos.z);
        int offsetX = (int)(-blocks.center.x * m_GameManager.m_SquareSize.x);
        int offsetY = (int)(-blocks.center.y * m_GameManager.m_SquareSize.y);

        return posBoard.Offset(offsetX, offsetY);
    }

    /// <summary>�Ֆʍ��W��Vector3�ɕϊ�</summary>
    /// <param name="pos">�Ֆʂ̂Q�������W</param>
    /// <returns>�R�������W</returns>
    Vector3 GetVector3Board(Vector2Int pos)
    {
        return new Vector3(m_GameManager.m_SquareSize.x * pos.x, 0f, m_GameManager.m_SquareSize.y * pos.y);
    }

    /// <summary>�L�[���s�[�g����</summary>
    /// <param name="key">���肷��L�[</param>
    /// <returns>�������u�ԂƉ����Ă���Ԃ̈��Ԋu��true</returns>
    bool KeyRepeat(params KeyCode[] key)
    {
        // �L�[�������Ă��Ȃ���΃^�C�}�[�����Z�b�g����false��Ԃ�
        if (!woskni.KeyBoard.GetOrKey(key)) {
            m_KeyRepeatTimer.Reset();
            return false;
        }

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

    void OutputDebugText(bool boardDebug, string filePath = "debugText.txt")
    {
        string debugText = "";

        if (boardDebug)
        {
            for (int y = 0; y < m_GameManager.boardSize.y; ++y)
            {
                for (int x = 0; x < m_GameManager.boardSize.x; ++x)
                {
                    switch (m_GameManager.board.GetBoardData(x, y))
                    {
                        case -1: debugText += "�@"; break;
                        case  0: debugText += "��"; break;
                        default: debugText += "��"; break;
                    }
                }

                debugText += "\n";
            }
        }
        else
        {
            bool isEven = blocks.GetWidth() % 2 == 0 && blocks.GetHeight() % 2 == 0;

            for (int y = 0; y < blocks.GetHeight(); ++y)
            {
                for (int x = 0; x < blocks.GetWidth(); ++x)
                {
                    // center�\��
                    if (!isEven && new Vector2(x, y) == blocks.center) { 
                        debugText += blocks.shape[x, y] ? "��" : "��";
                        continue;
                    }

                    debugText += blocks.shape[x, y] ? "��" : "�E";
                }

                debugText += "\n";
            }
        }

        TextOperate.WriteFile(filePath, debugText);
    }

    /// <summary> �ǌ��m </summary>
    /// <returns>�ǂ̗L���@[0]: ��, [1]: ��, [2]: ��, [3]: �E</returns>
    private bool[] IsCollisionMoveLimit()
    {
        // �����蔻��p�z��
        bool[] isCollision = new bool[4];

        // �ړ����E
        Vector2 limitLeftTop = Vector2.zero - blocks.center;                  // ����
        Vector2 limitRightBottom = m_GameManager.boardSize - blocks.center;   // �E��

        if (blocks.GetWidth()  % 2 == 1) limitRightBottom.x -= 1;
        if (blocks.GetHeight() % 2 == 1) limitRightBottom.y -= 1;

        if (blocks.GetWidth() % 2 == 0 && blocks.GetHeight() % 2 == 0) limitRightBottom -= Vector2.one;

        isCollision[0] = transform.position.z >=  limitLeftTop.y;       // ��
        isCollision[1] = transform.position.x <= -limitLeftTop.x;       // ��
        isCollision[2] = transform.position.z <= -limitRightBottom.y;   // ��
        isCollision[3] = transform.position.x >=  limitRightBottom.x;   // �E

        return isCollision;
    }
}

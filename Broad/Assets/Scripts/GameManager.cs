using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(MeshCombiner))]
public partial class GameManager : MonoBehaviour
{
    [Chapter("�R���|�[�l���g")]
    [SerializeField] BlockManager m_BlockManager;

    [Chapter]
    [Header("�}�X�ڗp�v���t�@�u")]
    [SerializeField] GameObject m_SquarePrefab = null;

    [Header("�ݒu�s�}�X�p�v���t�@�u")]
    [SerializeField] GameObject m_UnsetbleSquarePrefab = null;

    [Header("�J�����̍ŉ��̂Ƃ���Offset�l")]
    [SerializeField] Vector3 m_FurthestCameraOffset;

    // �ՖʊǗ��I�u�W�F�N�g
    GameObject m_BoardManagerObject = null;
    GameObject m_UnsetableParent = null;
    GameObject m_SetableParent = null;

    // �Ֆ�
    [HideInInspector] public Board board;

    // �Ֆʂ̃T�C�Y
    [HideInInspector] public Vector2Int boardSize;

    // �}�X�ڃT�C�Y
    public Vector2 m_SquareSize;

    private void Awake()
    {
        m_BoardManagerObject = new GameObject("BoardManager");
        m_SetableParent      = new GameObject("SetableSquares");
        m_UnsetableParent    = new GameObject("UnsetableSquares");

        // �ՖʊǗ��I�u�W�F�N�g��e�ɂ���
        m_SetableParent.transform.SetParent(m_BoardManagerObject.transform);
        m_UnsetableParent.transform.SetParent(m_BoardManagerObject.transform);

        // �z�X�g�̂݃{�[�h�������߂�
        if (NetworkClient.activeHost)
        {
            // �{�[�h�T�C�Y�������_���ɐݒ�
            boardSize = RandomVector2Int(GameSetting.instance.minBoardSize, GameSetting.instance.maxBoardSize);

            // �Ֆʂ̃T�C�Y��n��
            board = new Board(boardSize.x, boardSize.y);

            // �ݒu�s�}�X�̌���
            board.ShaveBoard();

            // �Ֆʏ���S�N���C�A���g�ɑ��M
            BoardData sendData = new BoardData(board);
            NetworkServer.SendToAll(sendData);
        }

        // BoardData����M������AReceivedBoardData�����s����悤�ɓo�^
        NetworkClient.RegisterHandler<BoardData>(ReceivedBoardData);

        // �͈͓��Ń����_���ȍ��W��Ԃ��֐����̊֐�
        Vector2Int RandomVector2Int(Vector2Int min, Vector2Int max)
            => new Vector2Int(Random.Range(min.x, max.x), Random.Range(min.y, max.y));
    }

    // Start is called before the first frame update
    void Start()
    {
        m_State = State.Placement;

        // �Ֆʂ̐ݒ�
        SetupBoard().Forget();
    }

    // �Ֆʂ̐ݒ�
    async UniTask SetupBoard()
    {
        // �{�[�h�̏�񂪓���܂őҋ@
        await UniTask.WaitUntil(() => board.data != null);

        Debug.Log(board.data);

        // �{�[�h�̍쐬
        LayOutSquare(m_SquarePrefab, m_UnsetbleSquarePrefab, boardSize, m_SquareSize);

        // �J�����̈ʒu���ړ�
        float rate = Mathf.InverseLerp(GameSetting.instance.minBoardSize.y, GameSetting.instance.maxBoardSize.y, boardSize.y);
        Vector3 offset = new Vector3(boardSize.x / 2f, rate * m_FurthestCameraOffset.y, rate * m_FurthestCameraOffset.z);
        Camera.main.transform.position = Camera.main.transform.position.Offset(offset);

        // �w�i�̍쐬
        CreateBackGround(m_UnsetbleSquarePrefab, boardSize);

        // �v���C���[�S���̏�񂪑����܂őҋ@
        await UniTask.WaitUntil(() => GameSetting.instance.playerColors.Length == GameSetting.instance.playerNum);
        m_BlockManager.CreateMaterials();

        // �ݒu�\�}�X�̃��b�V���̌�������
        //CombineMeshes().Forget();
    }

    // ���b�V���̌�������
    async UniTaskVoid CombineMeshes()
    {
        await UniTask.DelayFrame(1);

        // �ݒu�\�}�X�̃��b�V������
        MeshCombiner.Combine(m_SetableParent.transform.GetChildren().ToGameObjects(), "SetableBoard", m_BoardManagerObject.transform);

        // �ݒu�s�}�X�Ƙg�O�w�i�̃��b�V���̌���
        GameObject[] background = GameObject.Find("Background").transform.GetChildren().ToGameObjects();
        GameObject[] unsetable = m_UnsetableParent.transform.GetChildren().ToGameObjects().Concat(background).ToArray();
        var us = MeshCombiner.Combine(unsetable, "UnsetableBoard", m_BoardManagerObject.transform);

        // ��������Ȃ��̂ŏ�����΂�
        Destroy(m_SetableParent);
        Destroy(m_UnsetableParent);
    }

    /// <summary>�}�X�ڂ�~���l�߂�</summary>
    /// <param name="squarePrefab">�v���t�@�u</param>
    /// <param name="unsetablePrefab">�v���t�@�u</param>
    /// <param name="boardSize">�Ֆʂ̏c����</param>
    /// <param name="squareSize">�}�X�̃T�C�Y</param>
    /// <param name="parentTransform">�e�̃g�����X�t�H�[��</param>
    void LayOutSquare(GameObject setablePrefab, GameObject unsetablePrefab, Vector2Int boardSize, Vector2 squareSize)
    {
        // �I�u�W�F�N�g�̐���
        // y�������̐���
        for (int y = 0; y < boardSize.y; y++)
        {
            // x�������̐���
            for (int x = 0; x < boardSize.x; x++)
            {
                // �ݒu�󋵂̏�����
                if (board.GetBoardData(x, y) > 0) board.SetBoardData(0, x, y);

                // �}�X�ڐ���
                GameObject newSquare = Instantiate(board.GetBoardData(x, y) == 0 ? setablePrefab : unsetablePrefab);

                // �V�������������I�u�W�F�N�g�̖��O�E�e�E���W�E�X�P�[����ݒ肷��
                newSquare.gameObject.name       = "Square[" + x + "," + y + "]";
                newSquare.transform.parent      = board.GetBoardData(x, y) == 0 ? m_SetableParent.transform : m_UnsetableParent.transform;
                newSquare.transform.position    = new Vector3( x, -0.1f, -y);
                newSquare.transform.position    = Multi(newSquare.transform.position, new Vector3(squareSize.x, 1f, squareSize.y));
                newSquare.transform.localScale  = Multi(newSquare.transform.localScale, new Vector3(squareSize.x, 1f, squareSize.y));
            }
        }
    }

    /// <summary> �w�i���� </summary>
    /// <param name="prefab">�v���t�@�u</param>
    /// <param name="boardSize">�{�[�h�̃T�C�Y</param>
    void CreateBackGround(GameObject prefab, Vector2Int boardSize)
    {
        // �e�̐���
        GameObject parent           = new GameObject("Background");

        // �Ֆʂ̍ő�T�C�Y + �]�� �̒���
        const float size = 48f;

        // ��
        GameObject left             = Instantiate(prefab, parent.transform);
        left.name                   = "BackgroundLeft";
        left.transform.localScale   = new Vector3(size / 2f - boardSize.x / 2f, left.transform.localScale.y, size);
        left.transform.position     = new Vector3(-left.transform.localScale.x / 2f - 0.5f, -0.1f, -boardSize.y / 2f + 0.5f);

        // �E
        GameObject right            = Instantiate(left, parent.transform);
        right.name                  = "BackgroundRight";
        right.transform.position    = right.transform.position.Offset(x: boardSize.x + left.transform.localScale.x);

        // ��
        GameObject top              = Instantiate(prefab, parent.transform);
        top.name                    = "BackgroundTop";
        top.transform.localScale    = new Vector3(boardSize.x, top.transform.localScale.y, size / 2f - boardSize.y / 2f);
        top.transform.position      = new Vector3(boardSize.x / 2f - 0.5f, -0.1f, top.transform.localScale.z / 2 + 0.5f);

        // ��
        GameObject bottom           = Instantiate(top, parent.transform);
        bottom.name                 = "BackgroundBottom";
        top.transform.position      = bottom.transform.position.Offset(z: -boardSize.y - top.transform.localScale.z);
    }

        /// <summary>�x�N�g�����m�̏�Z</summary>
        /// <param name="v">�x�N�g���i�����ݒ�\�j</param>
        /// <returns>��Z�����z�Ɍ��܂��Ă񂾂�jk</returns>
        Vector3 Multi(params Vector3[] v)
    {
        for (int i = 1; i < v.Length; ++i)
            v[0] = new Vector3(v[0].x * v[i].x, v[0].y * v[i].y, v[0].z * v[i].z);

        return v[0];
    }

    /// <summary>�{�[�h�f�[�^��M</summary>
    /// <param name="receivedData">��M�f�[�^</param>
    void ReceivedBoardData(BoardData receivedData)
    {
        // ��M�f�[�^�𔽉f
        board = receivedData.board;

        boardSize = new Vector2Int(receivedData.board.width, receivedData.board.height);
    }
}

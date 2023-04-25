using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("�}�X�ڗp�v���t�@�u")]
    [SerializeField] GameObject m_SquarePrefab = null;

    [Header("�ݒu�s�}�X�p�v���t�@�u")]
    [SerializeField] GameObject m_UnsetBleSquarePrefab = null;

    [Header("�}�X�ڂ̐�����")]
    [SerializeField, Range(0f, 1f)] float m_BoardViability = 0.8f;

    [Header("�p�[�����m�C�Y�̑傫��")]
    [SerializeField] float m_PerlinScale = 0.1f;

    [Header("�Ֆʂ̍ŏ��A�ő�T�C�Y")]
    [SerializeField] Vector2Int m_MinBoardSize;
    [SerializeField] Vector2Int m_MaxBoardSize;

    // �ՖʊǗ��I�u�W�F�N�g
    GameObject m_BoardManagerObject = null;
    GameObject m_UnsetableParent = null;
    GameObject m_SetableParent = null;

    // �}�X�ڂ̎����
    public struct Square
    {
        // ���W�F���オVector2Int( 0, 0)
        public Vector2Int       position;
        // �󋵁i-1: �ݒu�s��, 0�F���ݒu, n: nP���ݒu�ς�)
        public int              state;

        // �R���X�g���N�^
        public Square(Vector2Int position, int state)
        {
            this.position = position;
            this.state = state;
        }
    }

    // �Ֆ�
    [HideInInspector] public Square[,] m_Board;

    // �Ֆʂ̃T�C�Y
    [HideInInspector] public Vector2Int m_BoardSize;

    // �}�X�ڃT�C�Y
    public Vector2 m_SquareSize;

    // Start is called before the first frame update
    void Awake()
    {
        m_BoardManagerObject = new GameObject("BoardManager");
        m_SetableParent =  new GameObject("SetableSquares");
        m_UnsetableParent = new GameObject("UnsetableSquares");

        // �ՖʊǗ��I�u�W�F�N�g��e�ɂ���
        m_SetableParent.transform.parent = m_BoardManagerObject.transform;
        m_UnsetableParent.transform.parent = m_BoardManagerObject.transform;

        // �Ֆʂ̐ݒ�
        SetupBoard();

        // ��������Ȃ��̂ŏ�����΂�
        Destroy(m_SetableParent);
        Destroy(m_UnsetableParent);
    }

    // �Ֆʂ̐ݒ�
    void SetupBoard()
    {
        // �{�[�h�T�C�Y�������_���ɐݒ�
        m_BoardSize = RandomVector2Int(m_MinBoardSize, m_MaxBoardSize);

        // �Ֆʂ̃T�C�Y��n��
        m_Board = new Square[m_BoardSize.x, m_BoardSize.y];

        // �ݒu�s�}�X�̌���
        ShaveBoard(m_BoardSize);

        // �{�[�h�̍쐬
        LayOutSquare(m_SquarePrefab, m_UnsetBleSquarePrefab, m_BoardSize, m_SquareSize);

        // �J�����̈ʒu���ړ�
        Camera.main.transform.position = Camera.main.transform.position.Difference(x: m_BoardSize.x / 2f);

        // �w�i�̍쐬
        CreateBackGround(m_UnsetBleSquarePrefab, m_BoardSize);

        // �ݒu�\�}�X�̃��b�V���̌�������
        GetComponent<MeshCombiner>().Combine(m_SetableParent.GetChildren(), "SetableBoard", m_BoardManagerObject.transform);

        // �ݒu�s�}�X�̃��b�V���̌�������
        GameObject[] background = GameObject.Find("Background").GetChildren();
        GameObject[] unsetable = m_UnsetableParent.GetChildren().Concat(background).ToArray();
        GetComponent<MeshCombiner>().Combine(unsetable, "UnSetableBoard", m_BoardManagerObject.transform);

        // �͈͓��Ń����_���ȍ��W��Ԃ��֐����̊֐�
        Vector2Int RandomVector2Int(Vector2Int min, Vector2Int max)
            => new Vector2Int(Random.Range(min.x, max.x), Random.Range(min.y, max.y));
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
                if (m_Board[x, y].state > 0) m_Board[x, y].state = 0;

                // �}�X�ڐ���
                GameObject newSquare = Instantiate(m_Board[x, y].state == 0 ? setablePrefab : unsetablePrefab);

                // �V�������������I�u�W�F�N�g�̖��O�E�e�E���W�E�X�P�[����ݒ肷��
                newSquare.gameObject.name       = "Square[" + x + "," + y + "]";
                newSquare.transform.parent      = m_Board[x, y].state == 0 ? m_SetableParent.transform : m_UnsetableParent.transform;
                newSquare.transform.position    = new Vector3( x, -0.1f, -y);
                newSquare.transform.position    = Multi(newSquare.transform.position, new Vector3(squareSize.x, 1f, squareSize.y));
                newSquare.transform.localScale  = Multi(newSquare.transform.localScale, new Vector3(squareSize.x, 1f, squareSize.y));

                // �{�[�h�Ƀ}�X�ڏ����i�[
                m_Board[x, y] = new Square(new Vector2Int(x, y), m_Board[x, y].state);
            }
        }
    }

    /// <summary> �ݒu�s�}�X�ɑ���̃I�u�W�F�N�g��ݒu���� </summary>
    /// <param name="squarePrefab">�v���t�@�u</param>
    /// <param name="boardSize">�{�[�h�̃T�C�Y</param>
    /// <param name="squareSize">�I�u�W�F�N�g�T�C�Y</param>
    /// <param name="parentTransform">�e�̃g�����X�t�H�[��</param>
    void ShaveBoard(Vector2Int boardSize)
    {
        // �p�[�����m�C�Y�̃V�[�h�l
        Vector2 seed = new Vector2(Random.value, Random.value) * 100f;

        for(int y = 0; y < boardSize.y; ++y)
        {
            for(int x = 0; x < boardSize.x; ++x)
            {
                // �p�[�����m�C�Y�̃T���v�����O�����Đݒu�s�}�X�ɂ���m�������߂�
                Vector2 value = new Vector2( x, y) * m_PerlinScale + seed;
                float perlinValue = Mathf.PerlinNoise(value.x, value.y);

                if (perlinValue >= m_BoardViability)
                {
                    // �ݒu�s�ɂ���
                    m_Board[x, y].state = -1;
                }
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
        left.transform.localScale   = new Vector3(size / 2f - m_BoardSize.x / 2f, left.transform.localScale.y, size);
        left.transform.position     = new Vector3(-left.transform.localScale.x / 2f - 0.5f, -0.1f, -m_BoardSize.y / 2f + 0.5f);

        // �E
        GameObject right            = Instantiate(left, parent.transform);
        right.name                  = "BackgroundRight";
        right.transform.position    = right.transform.position.Difference(x: m_BoardSize.x + left.transform.localScale.x);


        // ��
        GameObject top              = Instantiate(prefab, parent.transform);
        top.name                    = "BackgroundTop";
        top.transform.localScale    = new Vector3(m_BoardSize.x, top.transform.localScale.y, size / 2f - m_BoardSize.y / 2f);
        top.transform.position      = new Vector3(m_BoardSize.x / 2f - 0.5f, -0.1f, top.transform.localScale.z / 2 + 0.5f);

        // ��
        GameObject bottom           = Instantiate(top, parent.transform);
        bottom.name                 = "BackgroundBottom";
        top.transform.position      = bottom.transform.position.Difference(z: -m_BoardSize.y - top.transform.localScale.z);
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


}

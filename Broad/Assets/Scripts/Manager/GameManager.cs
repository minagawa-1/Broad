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

    // �}�X�ڃT�C�Y
    [SerializeField] Vector2 m_SquareSize;

    // �ՖʊǗ��I�u�W�F�N�g
    GameObject m_BoardManagerObject = null;
    GameObject m_UnsetableParent = null;
    GameObject m_SetableParent = null;

    // �}�X�ڂ̎����
    public struct Square
    {
        // �Q�[���I�u�W�F�N�g
        public GameObject       gameObject;
        // �X�v���C�g�����_���[
        public MeshRenderer     meshRenderer;
        // ���W�F���オVector2Int( 0, 0)
        public Vector2Int       position;
        // �󋵁i-1: �ݒu�s��, 0�F���ݒu, n: nP���ݒu�ς�)
        public int              state;

        // �R���X�g���N�^
        public Square(GameObject gameObject, MeshRenderer meshRenderer, Vector2Int position, int state)
        {
            this.gameObject = gameObject;
            this.meshRenderer = meshRenderer;
            this.position = position;
            this.state = state;
        }
    }

    // �Ֆ�
    Square[,] m_Board;

    // �Ֆʂ̃T�C�Y
    Vector2Int m_BoardSize;

    // Start is called before the first frame update
    void Start()
    {
        m_BoardManagerObject = new GameObject("BoardManager");
        m_SetableParent =  new GameObject("SetableSquares");
        m_UnsetableParent = new GameObject("SetableSquares");

        // �ՖʊǗ��I�u�W�F�N�g��e�ɂ���
        m_SetableParent.transform.parent = m_BoardManagerObject.transform;
        m_UnsetableParent.transform.parent = m_BoardManagerObject.transform;

        // �Ֆʂ̐ݒ�
        SetupBoard();
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

        // �w�i�̍쐬
        CreateBackGround(m_UnsetBleSquarePrefab, m_BoardSize);
        
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
                newSquare.gameObject.name = "Square[" + x + "," + y + "]";
                newSquare.transform.parent = m_Board[x, y].state == 0 ? m_SetableParent.transform : m_UnsetableParent.transform;
                newSquare.transform.position = new Vector3((float)x - (float)boardSize.x / 2f + 0.5f, -0.1f, (float)y - (float)boardSize.y / 2f + 0.5f);
                newSquare.transform.position = Multi(newSquare.transform.position, new Vector3(squareSize.x, 1f, squareSize.y));
                newSquare.transform.localScale = Multi(newSquare.transform.localScale, new Vector3(squareSize.x, 1f, squareSize.y));

                // �{�[�h�Ƀ}�X�ڏ����i�[
                m_Board[x, y] = new Square(newSquare, newSquare.GetComponent<MeshRenderer>(), new Vector2Int(x, y), m_Board[x, y].state);
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
        GameObject parent = new GameObject("Backgrounds");
        parent.transform.parent = m_BoardManagerObject.transform;

        // �Ֆʂ̍ő�T�C�Y + �]�� �̒���
        const float size = 48f;

        // ��
        GameObject left             = Instantiate(prefab, parent.transform);
        left.name = "BackgroundLeft";
        left.transform.localScale = new Vector3(size / 2f - m_BoardSize.x / 2f, left.transform.localScale.y, size);
        left.transform.position     = new Vector3(-m_BoardSize.x / 2f - left.transform.localScale.x / 2f, -0.1f, 0f);

        // �E
        GameObject right            = Instantiate(left, parent.transform);
        right.name = "BackgroundRight";
        right.transform.position    = new Vector3(-right.transform.position.x, right.transform.position.y, right.transform.position.z);

        // ��
        GameObject top              = Instantiate(prefab, parent.transform);
        top.name = "BackgroundTop";
        top.transform.localScale    = new Vector3(m_BoardSize.x, top.transform.localScale.y, size / 2f - m_BoardSize.y / 2f);
        top.transform.position      = new Vector3(0f, -0.1f, -m_BoardSize.y / 2f - top.transform.localScale.z / 2f);

        // ��
        GameObject bottom            = Instantiate(top, parent.transform);
        bottom.name = "BackgroundBottom";
        top.transform.position = new Vector3(bottom.transform.position.x, bottom.transform.position.y, -bottom.transform.position.z);
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

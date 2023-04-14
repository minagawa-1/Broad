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

    [Header("�{�[�h�̐e�ɐݒ肵�����I�u�W�F�N�g�̃g�����X�t�H�[��")]
    [SerializeField] Transform m_SetableParentTransform = null;

    [Header("�{�[�h�̐e�ɐݒ肵�����I�u�W�F�N�g�̃g�����X�t�H�[��")]
    [SerializeField] Transform m_UnsetableParentTransform = null;

    // �}�X�ڃT�C�Y
    [SerializeField] Vector2 m_SquareSize;


    // �}�X�ڂ̎����
    public struct Square
    {
        // �Q�[���I�u�W�F�N�g
        public GameObject       gameObject;
        // �X�v���C�g�����_���[
        public MeshRenderer   meshRenderer;
        // ���W�F���オVector2Int( 0, 0)
        public Vector2Int       position;
        // �ݒu�ł��邩
        public bool             setable;

        // �R���X�g���N�^
        public Square(GameObject gameObject, MeshRenderer meshRenderer, Vector2Int position, bool setable)
        {
            this.gameObject = gameObject;
            this.meshRenderer = meshRenderer;
            this.position = position;
            this.setable = setable;
        }
    }

    // �Ֆ�
    Square[,] m_Board;

    // �Ֆʂ̃T�C�Y
    Vector2Int m_BoardSize;

    // �{�[�h�}�X�̃X�e�[�^�X
    int[] m_SquareState = { -1, 0, 1, 2, 3, 4 };

    // �f�t�H���g�̃{�[�h�ݒ�
    int m_DefaultSquare;

    // Start is called before the first frame update
    void Start()
    {
        // �Ֆʂ̐ݒ�
        SetupBoard();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // �Ֆʂ̐ݒ�
    void SetupBoard()
    {
        // �{�[�h�T�C�Y�������_���ɐݒ�
        m_BoardSize = RandomVector2Int(m_MinBoardSize, m_MaxBoardSize);

        // �Ֆʂ̃T�C�Y��n��
        m_Board = new Square[m_BoardSize.x, m_BoardSize.y];

        // �f�t�H���g�̃{�[�h�}�X��ݒ�
        m_DefaultSquare = m_SquareState[1];

        // �ݒu�s�}�X�̌���
        ShaveBoard(m_BoardSize);

        // �{�[�h�̍쐬
        LayOutSquare(m_SquarePrefab, m_UnsetBleSquarePrefab, m_BoardSize, m_SquareSize, m_SetableParentTransform);
        
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
    void LayOutSquare(GameObject setablePrefab, GameObject unsetablePrefab, Vector2Int boardSize, Vector2 squareSize, Transform parentTransform)
    {
        // �I�u�W�F�N�g�̐���
        // y�������̐���
        for (int y = 0; y < boardSize.y; y++)
        {
            // x�������̐���
            for (int x = 0; x < boardSize.x; x++)
            {
                // �ݒu�s�̏ꍇ
                if (m_Board[x, y].setable)
                {
                    // �}�X�ڐ���
                    GameObject newSquare = Instantiate(unsetablePrefab);

                    // �V�������������I�u�W�F�N�g�̖��O�A�e�A���W��ݒ肷��
                    newSquare.gameObject.name = "Square[" + x + "," + y + "]";
                    newSquare.transform.parent = parentTransform;
                    newSquare.transform.position = new Vector3((float)x - (float)boardSize.x / 2f + 0.5f, -0.1f, (float)y - (float)boardSize.y / 2f + 0.5f);
                    newSquare.transform.position = Multi(newSquare.transform.position, new Vector3(squareSize.x, 1f, squareSize.y));
                    newSquare.transform.localScale = Multi(newSquare.transform.localScale, new Vector3(squareSize.x, 1f, squareSize.y));

                    // �{�[�h�Ƀ}�X�ڏ����i�[
                    m_Board[x, y] = new Square(newSquare, newSquare.GetComponent<MeshRenderer>(), new Vector2Int(x, y), true);
                }
                // �ݒu�\�̏ꍇ
                else
                {
                    // �}�X�ڐ���
                    GameObject newSquare = Instantiate(setablePrefab);

                    // �V�������������I�u�W�F�N�g�̖��O�A�e�A���W��ݒ肷��
                    newSquare.gameObject.name = "Square[" + x + "," + y + "]";
                    newSquare.transform.parent = parentTransform;
                    newSquare.transform.position = new Vector3((float)x - (float)boardSize.x / 2f + 0.5f, -0.1f, (float)y - (float)boardSize.y / 2f + 0.5f);
                    newSquare.transform.position = Multi(newSquare.transform.position, new Vector3(squareSize.x, 1f, squareSize.y));
                    newSquare.transform.localScale = Multi(newSquare.transform.localScale, new Vector3(squareSize.x, 1f, squareSize.y));

                    // �{�[�h�Ƀ}�X�ڏ����i�[
                    m_Board[x, y] = new Square(newSquare, newSquare.GetComponent<MeshRenderer>(), new Vector2Int(x, y), true);
                }
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
                    m_Board[ x, y].setable = true;
                }
            }
        }
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

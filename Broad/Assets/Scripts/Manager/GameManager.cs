using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // �t�B�[���h�p�v���t�@�u
    [SerializeField] GameObject m_FieldPrefab;

    // �ŏ��t�B�[���h�T�C�Y
    [SerializeField] Vector2Int m_MinFieldSize;
    // �ő�t�B�[���h�T�C�Y
    [SerializeField] Vector2Int m_MaxFieldSize;

    // �t�B�[���h�T�C�Y
    int[,] m_FieldSize;

    // �t�B�[���h�}�X�̃X�e�[�^�X
    int[] m_SquareState = { -1, 0, 1, 2, 3, 4 };

    // �f�t�H���g�̃t�B�[���h�ݒ�
    int m_DefaultSquare;

    // Start is called before the first frame update
    void Start()
    {
        // �t�B�[���h�T�C�Y�������_���ɐݒ�
        m_FieldSize = new int[Random.Range(m_MinFieldSize.x, m_MaxFieldSize.x), Random.Range(m_MinFieldSize.y, m_MaxFieldSize.y)];

        // �f�t�H���g�̃t�B�[���h�}�X��ݒ�
        m_DefaultSquare = m_SquareState[1];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // フィールド用プレファブ
    [SerializeField] GameObject m_FieldPrefab;

    // 最小フィールドサイズ
    [SerializeField] Vector2Int m_MinFieldSize;
    // 最大フィールドサイズ
    [SerializeField] Vector2Int m_MaxFieldSize;

    // フィールドサイズ
    int[,] m_FieldSize;

    // フィールドマスのステータス
    int[] m_SquareState = { -1, 0, 1, 2, 3, 4 };

    // デフォルトのフィールド設定
    int m_DefaultSquare;

    // Start is called before the first frame update
    void Start()
    {
        // フィールドサイズをランダムに設定
        m_FieldSize = new int[Random.Range(m_MinFieldSize.x, m_MaxFieldSize.x), Random.Range(m_MinFieldSize.y, m_MaxFieldSize.y)];

        // デフォルトのフィールドマスを設定
        m_DefaultSquare = m_SquareState[1];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
}

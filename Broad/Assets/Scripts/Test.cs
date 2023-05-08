using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] BlockManager m_BlockManager;
    [SerializeField] GameManager m_GameManager;

    [Header("ブロック数")]
    [SerializeField, Min(1)] int m_MinBlockUnits;
    [SerializeField, Min(1)] int m_MaxBlockUnits;

    [Header("密度")]
    [SerializeField, Range(0f, 1f)] float m_MinDensity;
    [SerializeField, Range(0f, 1f)] float m_MaxDensity;

    [Header("プレイヤー番号")]
    [SerializeField, Range(0, 4)] int m_PlayerNum;

    // Update is called once per frame
    void Update()
    {
        // スペースキーでブロックスを生成
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector2Int pos = new Vector2Int( m_GameManager.m_BoardSize.x / 2, m_GameManager.m_BoardSize.y / 2);

            int blockUnits = Random.Range(m_MinBlockUnits, m_MaxBlockUnits + 1);
            float density = Random.Range(m_MinDensity, m_MaxDensity);

            bool[,] blocks = LotteryBlocks.Lottery(blockUnits, density);

            //Debug.Log((float)sampleBlocks.GetLength(1) / 2f + ", " + (float)sampleBlocks.GetLength(0) / 2f);

            int player = m_PlayerNum == 0 ? Random.Range(1, 5) : m_PlayerNum;
            m_BlockManager.CreateBlock(player, blocks, pos);
        }
    }
}

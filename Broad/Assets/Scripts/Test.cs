using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] BlockManager m_BlockManager;
    [SerializeField] GameManager m_GameManager;

    [Header("プレイヤー番号")]
    [SerializeField, Min(0)] int m_PlayerNum;

    // Update is called once per frame
    void Update()
    {
        // スペースキーでブロックスを生成
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector2Int pos = new Vector2Int( m_GameManager.boardSize.x / 2, m_GameManager.boardSize.y / 2);

            int blockUnits = Random.Range(GameSetting.instance.minBlockUnits, GameSetting.instance.maxBlockUnits + 1);
            float density = Random.Range(GameSetting.instance.minDensity, GameSetting.instance.maxDensity);

            bool[,] blocks = LotteryBlocks.Lottery(blockUnits, density);

            //Debug.Log((float)sampleBlocks.GetLength(1) / 2f + ", " + (float)sampleBlocks.GetLength(0) / 2f);

            int player = m_PlayerNum == 0 ? Random.Range(1, GameSetting.instance.playerNum + 1)
                                          : Mathf.Min(m_PlayerNum, GameSetting.instance.playerNum);
            m_BlockManager.CreateBlock(player, blocks, pos);
        }
    }
}

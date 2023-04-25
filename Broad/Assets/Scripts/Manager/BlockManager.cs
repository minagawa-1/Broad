using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour
{
    [Header("生成したいブロックのプレファブ")]
    [SerializeField] GameObject m_BlockPrefab = null;

    // 生成したブロックの親
    GameObject m_BlockParent = null;

    private void Start()
    {
        m_BlockParent = new GameObject("Blocks");
    }

    /// <summary> ブロックの生成 </summary>
    /// <param name="player"> プレイヤー番号 </param>
    /// <param name="shape"> 形状データ</param>
    /// <param name="position"> 座標 </param>
    public void CreateBlock( int player, bool[,] shape, Vector2Int position)
    {
        Blocks blocks = new Blocks(shape);

        // 親設定
        GameObject parent = new GameObject("ControlBlocks");
        ControlBlock cb = parent.AddComponent<ControlBlock>();
        cb.m_AfterSetParent = m_BlockParent;
        cb.m_Blocks = blocks;
        cb.playerIndex = player;

        parent.transform.parent = transform;

        // ControlBlocksを1m上に押し上げる。
        parent.transform.position = new Vector3(position.x, 1f, -position.y);

        int blockCount = 0;

        for (int y = 0; y < blocks.GetHeight(); ++y) {
            for (int x = 0; x < blocks.GetWidth(); ++x)
            {
                // 生成しない座標だったらcontinue
                if (blocks.shape[x, y] == false) continue;

                // ブロックの生成
                GameObject newBlock = Instantiate(m_BlockPrefab);

                // 親の設定
                newBlock.transform.parent = parent.transform;

                // 生成したブロックの名前・形状内での座標を設定
                newBlock.name = "ControledBlock[" + (blockCount++) + "]";

                

                // 位置
                newBlock.transform.localPosition = new Vector3(
                    x:   x - (float)blocks.GetWidth()  / 2f,
                    y: 0f,
                    z: -(y - (float)blocks.GetHeight() / 2f));
            }
        }

        // 偶数奇数に応じて半マスずらす
        parent.transform.position = parent.transform.position.Difference(x: blocks.GetWidth()  % 2 == 0 ? 0f :  0.5f,
                                                                         z: blocks.GetHeight() % 2 == 0 ? 0f : -0.5f);
    }
}

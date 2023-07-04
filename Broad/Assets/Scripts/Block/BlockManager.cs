using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour
{
    [Header("生成したいブロックのプレファブ")]
    [SerializeField] GameObject m_BlockPrefab = null;

    // プレファブのマテリアル
    public Material[] m_ControlBlockMaterials = null;
    public Material[] m_SetBlockMaterials = null;

    // 生成したブロックの親
    public GameObject m_BlockParent = null;

    private void Start()
    {
        m_BlockParent = new GameObject("Blocks");
        m_BlockParent.transform.SetParent(transform);
    }

    public void CreateMaterials()
    {
        // プレイヤーの色を取得
        Color[] colors = GameSetting.instance.playersColor;

        m_ControlBlockMaterials = new Material[colors.Length];
        m_SetBlockMaterials     = new Material[colors.Length];

        for (int i = 0; i < colors.Length; ++i)
        {
            m_ControlBlockMaterials[i] = new Material(m_BlockPrefab.GetComponent<MeshRenderer>().sharedMaterial);
            m_SetBlockMaterials[i]     = new Material(m_BlockPrefab.GetComponent<MeshRenderer>().sharedMaterial);

            m_ControlBlockMaterials[i].color = colors[i];
            m_SetBlockMaterials[i].color     = colors[i];
        }
    }

    /// <summary> ブロックの生成 </summary>
    /// <param name="player"> プレイヤー番号 </param>
    /// <param name="shape"> 形状データ</param>
    /// <param name="position"> 座標 </param>
    public void CreateBlock(int player, bool[,] shape, Vector2Int position, float density)
    {
        Blocks blocks = new Blocks(shape, position, density);

        // 親設定
        GameObject parent = new GameObject("ControlBlocks");
        var cb = parent.AddComponent<ControlBlock>();
        cb.afterSetParent = m_BlockParent.transform;
        cb.afterSetMaterial = m_SetBlockMaterials[player - 1];
        cb.blocks = blocks;
        cb.playerIndex = player;

        var ct = parent.AddComponent<ChangeTransparency>();
        ct.blockMaterials = m_ControlBlockMaterials;
        ct.player = player;

        parent.transform.parent = transform;

        // ControlBlocksを1m上に押し上げる。
        parent.transform.position = new Vector3(position.x, 1f, -position.y);

        if (blocks.width % 2 == 0 && blocks.height % 2 == 0)
            parent.transform.position = parent.transform.position.Offset(-0.5f, 0f, -0.5f);

        int blockCount = 0;

        for (int y = 0; y < blocks.height; ++y) {
            for (int x = 0; x < blocks.width; ++x)
            {
                // 生成しない座標だったらcontinue
                if (blocks.shape[x, y] == false) continue;

                // ブロックの生成
                GameObject newBlock = Instantiate(m_BlockPrefab);

                // 親の設定
                newBlock.transform.parent = parent.transform;

                // 生成したブロックの名前・形状内での座標を設定
                newBlock.name = "Block[" + (blockCount++) + "]";

                // 位置
                newBlock.transform.localPosition = GetBlockPosition(blocks, new Vector2Int(x, y));

                // マテリアルの設定
                newBlock.GetComponent<Renderer>().material = m_ControlBlockMaterials[player - 1];
            }
        }
    }

    public static Vector3 GetBlockPosition(Blocks blocks, Vector2Int pos) => new Vector3(pos.x - blocks.center.x, 0f
                                                                                     , -(pos.y - blocks.center.y));

    public void SortBlocks()
    {
        var blockList = m_BlockParent.transform.GetChildren().ToList();

        // オブジェクトを座標で昇順ソート
        blockList.Sort((a, b) => {
            int zComparison = -a.transform.position.z.CompareTo(b.transform.position.z);
            if (zComparison != 0) return zComparison;
            else return a.transform.position.x.CompareTo(b.transform.position.x);
        });

        // ソート結果順にGameObjectの順序を反映
        foreach (var obj in blockList)
            obj.transform.SetSiblingIndex(blockList.Count - 1);
    }
}
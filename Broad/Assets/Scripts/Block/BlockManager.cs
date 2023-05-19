using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour
{
    [Header("�����������u���b�N�̃v���t�@�u")]
    [SerializeField] GameObject m_BlockPrefab = null;

    // �v���t�@�u�̃}�e���A��
    Material[] m_ControlBlockMaterials = null;
    Material[] m_SetBlockMaterials = null;

    // ���������u���b�N�̐e
    GameObject m_BlockParent = null;

    private void Start()
    {
        // �v���C���[�̐l������F������
        SetupPlayerColor();

        m_BlockParent = new GameObject("Blocks");
        m_BlockParent.transform.SetParent(transform);
    }

    void SetupPlayerColor()
    {
        for (int i = 0; i < GameSetting.instance.playerNum; ++i)
        {
            float h = Random.value;
            float s = Random.Range(0.2f, 0.5f);
            float v = Random.Range(0.9f, 1f);
            Color color1P = Color.HSVToRGB(h, s, v);

            GameSetting.instance.playerColors = ColorUtils.GetRelativeColor(color1P, GameSetting.instance.playerNum);

            CreateMaterials(GameSetting.instance.playerColors);
        }

        void CreateMaterials(Color[] colors)
        {
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
    }

    /// <summary> �u���b�N�̐��� </summary>
    /// <param name="player"> �v���C���[�ԍ� </param>
    /// <param name="shape"> �`��f�[�^</param>
    /// <param name="position"> ���W </param>
    public void CreateBlock(int player, bool[,] shape, Vector2Int position)
    {
        Blocks blocks = new Blocks(shape);

        // �e�ݒ�
        GameObject parent = new GameObject("ControlBlocks");
        var cb = parent.AddComponent<ControlBlock>();
        cb.afterSetParent = m_BlockParent;
        cb.afterSetMaterial = m_SetBlockMaterials[player - 1];
        cb.blocks = blocks;
        cb.playerIndex = player;

        var ct = parent.AddComponent<ChangeTransparency>();
        ct.blockMaterials = m_ControlBlockMaterials;
        ct.player = player;

        parent.transform.parent = transform;

        // ControlBlocks��1m��ɉ����グ��B
        parent.transform.position = new Vector3(position.x, 1f, -position.y);

        if (blocks.GetWidth() % 2 == 0 && blocks.GetHeight() % 2 == 0)
            parent.transform.position = parent.transform.position.Offset(-0.5f, 0f, -0.5f);

        int blockCount = 0;

        for (int y = 0; y < blocks.GetHeight(); ++y) {
            for (int x = 0; x < blocks.GetWidth(); ++x)
            {
                // �������Ȃ����W��������continue
                if (blocks.shape[x, y] == false) continue;

                // �u���b�N�̐���
                GameObject newBlock = Instantiate(m_BlockPrefab);

                // �e�̐ݒ�
                newBlock.transform.parent = parent.transform;

                // ���������u���b�N�̖��O�E�`����ł̍��W��ݒ�
                newBlock.name = "Block[" + (blockCount++) + "]";

                // �ʒu
                newBlock.transform.localPosition = GetBlockPosition(blocks, new Vector2Int(x, y));

                // �}�e���A���̐ݒ�
                newBlock.GetComponent<Renderer>().material = m_ControlBlockMaterials[player - 1];
            }
        }
    }

    public static Vector3 GetBlockPosition(Blocks blocks, Vector2Int pos) => new Vector3(pos.x - blocks.center.x, 0f
                                                                                     , -(pos.y - blocks.center.y));

    public void SortBlocks()
    {
        var blockList = m_BlockParent.GetChildren().ToList();

        // �I�u�W�F�N�g�����W�ŏ����\�[�g
        blockList.Sort((a, b) => {
            int zComparison = -a.transform.position.z.CompareTo(b.transform.position.z);
            if (zComparison != 0) return zComparison;
            else return a.transform.position.x.CompareTo(b.transform.position.x);
        });

        // �\�[�g���ʏ���GameObject�̏����𔽉f
        foreach (var obj in blockList)
            obj.transform.SetSiblingIndex(blockList.Count - 1);
    }
}
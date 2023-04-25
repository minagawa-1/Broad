using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour
{
    [Header("�����������u���b�N�̃v���t�@�u")]
    [SerializeField] GameObject m_BlockPrefab = null;

    // ���������u���b�N�̐e
    GameObject m_BlockParent = null;

    private void Start()
    {
        m_BlockParent = new GameObject("Blocks");
    }

    /// <summary> �u���b�N�̐��� </summary>
    /// <param name="player"> �v���C���[�ԍ� </param>
    /// <param name="shape"> �`��f�[�^</param>
    /// <param name="position"> ���W </param>
    public void CreateBlock( int player, bool[,] shape, Vector2Int position)
    {
        Blocks blocks = new Blocks(shape);

        // �e�ݒ�
        GameObject parent = new GameObject("ControlBlocks");
        ControlBlock cb = parent.AddComponent<ControlBlock>();
        cb.m_AfterSetParent = m_BlockParent;
        cb.m_Blocks = blocks;
        cb.playerIndex = player;

        parent.transform.parent = transform;

        // ControlBlocks��1m��ɉ����グ��B
        parent.transform.position = new Vector3(position.x, 1f, -position.y);

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
                newBlock.name = "ControledBlock[" + (blockCount++) + "]";

                

                // �ʒu
                newBlock.transform.localPosition = new Vector3(
                    x:   x - (float)blocks.GetWidth()  / 2f,
                    y: 0f,
                    z: -(y - (float)blocks.GetHeight() / 2f));
            }
        }

        // ������ɉ����Ĕ��}�X���炷
        parent.transform.position = parent.transform.position.Difference(x: blocks.GetWidth()  % 2 == 0 ? 0f :  0.5f,
                                                                         z: blocks.GetHeight() % 2 == 0 ? 0f : -0.5f);
    }
}

using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class LotteryBlocks
{
    // �݌v�p�L�����o�X�̕�
    public const int m_max_width = 25;

    /// <summary>�u���b�N�X�̒��I</summary>
    /// <param name="blockUnits">�u���b�N��</param>
    /// <param name="obliqueRate">���W�x</param>
    /// <returns>�u���b�N�X�̗L�����w���z��</returns>
    public static bool[,] Lottery(int blockUnits, float density = 0.5f)
    {
        bool[,] blocks = new bool[m_max_width, m_max_width];

        // ���S�Ƀu���b�N�𐶐�
        blocks[m_max_width / 2, m_max_width / 2] = true;

        // �u���b�N�z�u�̌��菈��
        for (int i = 1; i < blockUnits; ++i)
        {
            Vector2Int pos = GetNeighborPositions(blocks, density).AtRandom();
            blocks[pos.x, pos.y] = true;
        }

        // �O���̗]���ȍs����g���~���O����return
        return TrimmingBlocks.Trimming(blocks, false);
    }

    /// <summary>�z�u�\�ȍ��W���X�g�̎擾</summary>
    /// <param name="blocks">�u���b�N�X���</param>
    /// <param name="density">���W��</param>
    static List<Vector2Int> GetNeighborPositions(bool[,] blocks, float density)
    {
        List<Vector2Int> neighborPositions = new List<Vector2Int>();

        // �{�[�h�̕��ƍ������擾
        int width = blocks.GetLength(0);
        int height = blocks.GetLength(1);

        // �{�[�h�̊e�}�X�ɑ΂��ėאڔ�����s��
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // true�łȂ��ꍇ�̓X�L�b�v
                if (!blocks[x, y]) continue;

                // �a�̍��W�i�΂߁j
                if (Random.value >= density)
                {
                    // �ȂȂ߂𑖍�
                    for (int i = -1; i <= 1; i += 2)
                    {
                        for (int j = -1; j <= 1; j += 2)
                        {
                            int neighborX = x + i;
                            int neighborY = y + j;

                            // �{�[�h�O�̃}�X�̓X�L�b�v
                            if (neighborX < 0 || neighborX >= width || neighborY < 0 || neighborY >= height) continue;

                            // true���łɃu���b�N������ꍇ�̓X�L�b�v
                            if (blocks[neighborX, neighborY]) continue;

                            // �אڂ���false�����o�����ꍇ�̓��X�g�ɒǉ�
                            Vector2Int neighborPosition = new Vector2Int(neighborX, neighborY);
                            if (!neighborPositions.Contains(neighborPosition)) neighborPositions.Add(neighborPosition);
                        }
                    }
                }

                // ���̍��W�i�㉺���E�j
                else
                {
                    // �㉺���E�𑖍�
                    for (int i = -1; i <= 1; i++)
                    {
                        for (int j = i % 2 == 0 ? -1 : 0; j <= 1; j += 2)
                        {
                            int neighborX = x + i;
                            int neighborY = y + j;

                            // �{�[�h�O�̃}�X�̓X�L�b�v
                            if (neighborX < 0 || neighborX >= width || neighborY < 0 || neighborY >= height) continue;

                            // true���łɃu���b�N������ꍇ�̓X�L�b�v
                            if (blocks[neighborX, neighborY]) continue;

                            // �אڂ���false�����o�����ꍇ�̓��X�g�ɒǉ�
                            Vector2Int neighborPosition = new Vector2Int(neighborX, neighborY);
                            if (!neighborPositions.Contains(neighborPosition)) neighborPositions.Add(neighborPosition);
                        }
                    }
                }
            }
        }

        return neighborPositions;
    }

    /// <summary>�g���~���O</summary>
    public class TrimmingBlocks
    {
        /// <summary>�z����g���~���O</summary>
        /// <param name="array">�g���~���O����z��</param>
        /// <param name="empty">�g���~���O�����i�����A�C�e���j</param>
        /// <returns>�]�����������z��</returns>
        public static T[,] Trimming<T>(T[,] array, T empty)
        {
            // �s�̌����i�ЂƂ�true�������Ă��Ȃ��s��true�j
            bool[] isEmptyRow = Enumerable.Range(0, array.GetLength(0))
                 .Select(row => Enumerable.Range(0, array.GetLength(1)).All(col => array[row, col].Equals(empty))).ToArray();
            // ��̌����i�ЂƂ�true�������Ă��Ȃ����true�j
            bool[] isEmptyCol = Enumerable.Range(0, array.GetLength(1))
                 .Select(col => Enumerable.Range(0, array.GetLength(0)).All(row => array[row, col].Equals(empty))).ToArray();

            // �폜��̔z��̃T�C�Y���擾
            int newRows = isEmptyRow.Count(row => !row);
            int newCols = isEmptyCol.Count(col => !col);

            // �폜��̔z����쐬
            T[,] newArray = new T[newRows, newCols];

            // �폜��z���ݒ�
            int xCounter = 0;
            foreach (int x in Enumerable.Range(0, array.GetLength(0)).Where(r => !isEmptyRow[r]))
            {
                int yCounter = 0;

                foreach (int y in Enumerable.Range(0, array.GetLength(1)).Where(c => !isEmptyCol[c])) {
                    newArray[xCounter, yCounter] = array[x, y];
                    yCounter++;
                }
                xCounter++;
            }

            return newArray;
        }
    }
}

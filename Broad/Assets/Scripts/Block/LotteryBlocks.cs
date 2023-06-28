using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class LotteryBlocks
{
    // 設計用キャンバスの幅
    public const int m_max_width = 25;

    /// <summary>ブロックスの抽選（引数なしの場合はGameSettingに準拠）</summary>
    /// <param name="blockUnits">ブロック数</param>
    /// <param name="obliqueRate">密集度</param>
    /// <returns>ブロックスの有無を指す配列</returns>
    public static bool[,] Lottery(int? blockUnits = null, float? density = null)
    {
        blockUnits ??= Random.Range(GameSetting.instance.minBlockUnits, GameSetting.instance.maxBlockUnits + 1);
        float newDensity = density ?? Random.Range(GameSetting.instance.minDensity   , GameSetting.instance.maxDensity);

        bool[,] blocks = new bool[m_max_width, m_max_width];

        // 中心にブロックを生成
        blocks[m_max_width / 2, m_max_width / 2] = true;

        // ブロック配置の決定処理
        for (int i = 1; i < blockUnits; ++i)
        {
            Vector2Int pos = GetNeighborPositions(blocks, newDensity).AtRandom();
            blocks[pos.x, pos.y] = true;
        }

        // 外側の余分な行列をトリミングしてreturn
        return TrimmingBlocks.Trimming(blocks, false);
    }

    /// <summary>配置可能な座標リストの取得</summary>
    /// <param name="blocks">ブロックス情報</param>
    /// <param name="density">密集率</param>
    static List<Vector2Int> GetNeighborPositions(bool[,] blocks, float density)
    {
        List<Vector2Int> neighborPositions = new List<Vector2Int>();

        // ボードの幅と高さを取得
        int width = blocks.GetLength(0);
        int height = blocks.GetLength(1);

        // ボードの各マスに対して隣接判定を行う
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // trueでない場合はスキップ
                if (!blocks[x, y]) continue;

                // 疎の座標（斜め）
                if (Random.value >= density)
                {
                    // ななめを走査
                    for (int i = -1; i <= 1; i += 2)
                    {
                        for (int j = -1; j <= 1; j += 2)
                        {
                            int neighborX = x + i;
                            int neighborY = y + j;

                            // ボード外のマスはスキップ
                            if (neighborX < 0 || neighborX >= width || neighborY < 0 || neighborY >= height) continue;

                            // trueすでにブロックがある場合はスキップ
                            if (blocks[neighborX, neighborY]) continue;

                            // 隣接するfalseを検出した場合はリストに追加
                            Vector2Int neighborPosition = new Vector2Int(neighborX, neighborY);
                            if (!neighborPositions.Contains(neighborPosition)) neighborPositions.Add(neighborPosition);
                        }
                    }
                }

                // 密の座標（上下左右）
                else
                {
                    // 上下左右を走査
                    for (int i = -1; i <= 1; i++)
                    {
                        for (int j = i % 2 == 0 ? -1 : 0; j <= 1; j += 2)
                        {
                            int neighborX = x + i;
                            int neighborY = y + j;

                            // ボード外のマスはスキップ
                            if (neighborX < 0 || neighborX >= width || neighborY < 0 || neighborY >= height) continue;

                            // trueすでにブロックがある場合はスキップ
                            if (blocks[neighborX, neighborY]) continue;

                            // 隣接するfalseを検出した場合はリストに追加
                            Vector2Int neighborPosition = new Vector2Int(neighborX, neighborY);
                            if (!neighborPositions.Contains(neighborPosition)) neighborPositions.Add(neighborPosition);
                        }
                    }
                }
            }
        }

        return neighborPositions;
    }

    /// <summary>トリミング</summary>
    public class TrimmingBlocks
    {
        /// <summary>配列をトリミング</summary>
        /// <param name="array">トリミングする配列</param>
        /// <param name="empty">トリミング条件（消すアイテム）</param>
        /// <returns>余白を消した配列</returns>
        public static T[,] Trimming<T>(T[,] array, T empty)
        {
            // 行の検査（ひとつもtrueが入っていない行はtrue）
            bool[] isEmptyRow = Enumerable.Range(0, array.GetLength(0))
                 .Select(row => Enumerable.Range(0, array.GetLength(1)).All(col => array[row, col].Equals(empty))).ToArray();
            // 列の検査（ひとつもtrueが入っていない列はtrue）
            bool[] isEmptyCol = Enumerable.Range(0, array.GetLength(1))
                 .Select(col => Enumerable.Range(0, array.GetLength(0)).All(row => array[row, col].Equals(empty))).ToArray();

            // 削除後の配列のサイズを取得
            int newRows = isEmptyRow.Count(row => !row);
            int newCols = isEmptyCol.Count(col => !col);

            // 削除後の配列を作成
            T[,] newArray = new T[newRows, newCols];

            // 削除後配列を設定
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

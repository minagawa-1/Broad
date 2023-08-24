using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace woskni
{
    public class CSVLoader
    {
        /// <summary>データのリスト</summary>
        public List<string> data { get; }

        /// <summary>横に並んでいるデータの個数</summary>
        public int cols { get; }

        /// <summary>縦に並んでいるデータの個数</summary>
        public int rows { get; }

        // データをキーと位置を対応付けるハッシュテーブル
        private Dictionary<string, Vector2Int> dataPositions;

        /// <summary>CSVファイル読み込み
        /// <param name="filePath"> 読み込むCSVファイルパス </param>
        public CSVLoader(string filePath)
        {
            // データ初期化
            data = new List<string>();
            cols = rows = 0;
            dataPositions = new Dictionary<string, Vector2Int>();

            // ファイルを読み込む
            var csv = Resources.Load<TextAsset>(filePath);

            // 読み込めなかった場合はエラーを出力して終了
            if (csv == null) {
                Debug.LogError(filePath + "の読み込み失敗");
                return;
            }

            // 行で分割
            var lines = csv.text.Split('\n');
            
            // 末尾まで繰り返す
            for(int line = 0; line < lines.Length; ++line)
            {
                var data = lines[line].Split(',');

                for (int i = 0; i < data.Length; ++i)
                {
                    this.data.Add(data[i]);

                    // 位置情報を格納
                    if (dataPositions.ContainsKey(data[i])) dataPositions[data[i]] = new Vector2Int(i, rows);
                    else dataPositions.Add(data[i], new Vector2Int(i, rows));

                    // 行数を加算
                    ++rows;
                }
            }

            // 列数を算出
            cols = this.data.Count / rows;
        }

        /// <summary>指定されたデータを文字列で取得</summary>
        /// <param name="rows">行</param>
        /// <param name="cols">列</param>
        public string GetString(int row, int col) => data[(row * cols) + col];

        /// <summary>指定されたデータを整数に変換して取得</summary>
        /// <param name="rows">行</param>
        /// <param name="cols">列</param>
        public int GetInteger(int row, int col) => int.Parse(data[(row * cols) + col]);

        /// <summary>指定されたデータを実数に変換して取得</summary>
        /// <param name="rows">行</param>
        /// <param name="cols">列</param>
        public float GetFloat(int row, int col) => float.Parse(data[(row * cols) + col]);

        /// <summary>データの場所を検索</summary>
        /// <param name="search">検索文字列</param>
        /// <returns>見つかった場所の行列。見つからなかった場合は(-1, -1)</returns>
        public Vector2Int Find(string search)
        {
            Vector2Int position;
            return dataPositions.TryGetValue(search, out position) ? position : new Vector2Int(-1, -1);
        }
    }
}
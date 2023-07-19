using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace woskni
{
    public class CSVLoader
    {
        // CSVデータリスト
        public static List<string> m_Data = new List<string>();

        public static int m_Cols; // 横に並んでいるデータの個数
        public static int m_Rows; // 縦に並んでいるデータの個数

        /// <summary>
        /// CSVファイル読み込み
        /// </summary>
        /// <param name="file_path"> 読み込むCSVファイルパス </param>
        public void Load(string file_path)
        {
            // 念のため解放する
            Unload();

            StreamReader reader = new StreamReader(file_path);

            // 読み込めなかった場合はエラーを出力して終了
            if (reader == null) { Debug.LogError(file_path + "の読み込み失敗"); return; }

            // 末尾まで繰り返す
            while (!reader.EndOfStream)
            {
                // CSVファイルの一行を読み込む
                string line = reader.ReadLine();

                // 読み込んだ一行をカンマ毎に分けて配列に格納する
                for (int i = 0; i < line.Split(',').Length; ++i)
                    m_Data.Add(line.Split(',')[i]);

                // 縦に並んでいる数をカウント
                ++m_Rows;
            }
            // 横に並んでいる数を算出
            m_Cols = m_Data.Count / m_Rows;
        }

        /// <summary>
        /// 解放
        /// </summary>
        public void Unload()
        {
            m_Data.Clear();

            m_Cols = m_Rows = 0;
        }

        /// <summary>
        /// 横に並んでいる数を取得
        /// </summary>
        /// <returns>列数</returns>
        public int GetCols()
        {
            return m_Cols;
        }

        /// <summary>
        /// 縦に並んでいる数を取得
        /// </summary>
        /// <returns>行数</returns>
        public int GetRows()
        {
            return m_Rows;
        }

        /// <summary>
        /// データ数を取得
        /// </summary>
        /// <returns>データ数</returns>
        public int GetDataCount()
        {
            return m_Data.Count;
        }

        /// <summary>
        /// 指定されたデータを文字列で取得
        /// </summary>
        /// <param name="rows">行</param>
        /// <param name="cols">列</param>
        /// <returns>行列に該当する文字列</returns>
        public string GetString(int rows, int cols)
        {
            return m_Data[(rows * m_Cols) + cols];
        }

        /// <summary>
        /// 指定されたデータを整数に変換して取得
        /// </summary>
        /// <param name="rows">行</param>
        /// <param name="cols">列</param>
        /// <returns>行列に該当する整数</returns>
        public int GetInteger(int rows, int cols)
        {
            return int.Parse(m_Data[(rows * m_Cols) + cols]);
        }

        /// <summary>
        /// 指定されたデータを実数に変換して取得
        /// </summary>
        /// <param name="rows">行</param>
        /// <param name="cols">列</param>
        /// <returns>行列に該当する実数</returns>
        public float GetFloat(int rows, int cols)
        {
            return float.Parse(m_Data[(rows * m_Cols) + cols]);
        }
    }
}
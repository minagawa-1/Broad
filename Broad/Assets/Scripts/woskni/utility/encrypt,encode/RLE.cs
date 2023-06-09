using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace woskni
{
    public class RLE : MonoBehaviour
    {
        public const string zero = "0";
        public const string one = "1";

        /// <summary>ランレングス符号化</summary>
        /// <param name="plaintext">平文 (例: "0001110110")</param>
        /// <returns>符号文 (例: "033121")</returns>
        public static string Encode(string plaintext)
        {
            string encodedText = "";

            // 数値の連続回数
            int counter = 1;

            // 1から始まる前提なので、0が最初の場合は先頭に1が0個であることを明記する
            if (plaintext.Substring(0, 1) == zero) encodedText += "0";


            for (int i = 1; i < plaintext.Length; ++i)
            {
                // 前の数値と同じ場合はカウンターを増やす
                if (plaintext.Substring(i, 1) == plaintext.Substring(i - 1, 1))
                    ++counter;
                // 違う場合は連続数を文字列に格納してカウンターリセット
                else
                {
                    encodedText += counter.ToString();
                    counter = 1;
                }
            }

            // 最後のカウンターを格納してreturn
            return encodedText + counter.ToString();
        }

        /// <summary>ランレングス復号化</summary>
        /// <param name="ciphertext">符号文 (例: "24151")</param>
        /// /// <returns>復号文 (例: "1100001000001")</returns>
        public static string Decode(string ciphertext)
        {
            string decodedText = "";

            for (int i = 0; i < ciphertext.Length; ++i)
            {
                int repeat = int.Parse(ciphertext.Substring(i, 1));

                // 奇数番目の数値は1,偶数番目の数値は0 の連続数
                decodedText += i % 2 == 0 ? one.Repeat(repeat) : zero.Repeat(repeat);
            }

            return decodedText;
        }
    }
}
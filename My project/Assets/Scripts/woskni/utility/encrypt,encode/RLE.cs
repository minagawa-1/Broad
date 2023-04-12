using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace woskni
{
    public class RLE : MonoBehaviour
    {
        public const string zero = "0";
        public const string one = "1";

        /// <summary>���������O�X������</summary>
        /// <param name="plaintext">���� (��: "0001110110")</param>
        /// <returns>������ (��: "033121")</returns>
        public static string Encode(string plaintext)
        {
            string encodedText = "";

            // ���l�̘A����
            int counter = 1;

            // 1����n�܂�O��Ȃ̂ŁA0���ŏ��̏ꍇ�͐擪��1��0�ł��邱�Ƃ𖾋L����
            if (plaintext.Substring(0, 1) == zero) encodedText += "0";


            for (int i = 1; i < plaintext.Length; ++i)
            {
                // �O�̐��l�Ɠ����ꍇ�̓J�E���^�[�𑝂₷
                if (plaintext.Substring(i, 1) == plaintext.Substring(i - 1, 1))
                    ++counter;
                // �Ⴄ�ꍇ�͘A�����𕶎���Ɋi�[���ăJ�E���^�[���Z�b�g
                else
                {
                    encodedText += counter.ToString();
                    counter = 1;
                }
            }

            // �Ō�̃J�E���^�[���i�[����return
            return encodedText + counter.ToString();
        }

        /// <summary>���������O�X������</summary>
        /// <param name="ciphertext">������ (��: "24151")</param>
        /// /// <returns>������ (��: "1100001000001")</returns>
        public static string Decode(string ciphertext)
        {
            string decodedText = "";

            for (int i = 0; i < ciphertext.Length; ++i)
            {
                int repeat = int.Parse(ciphertext.Substring(i, 1));

                // ��Ԗڂ̐��l��1,�����Ԗڂ̐��l��0 �̘A����
                decodedText += i % 2 == 0 ? one.Repeat(repeat) : zero.Repeat(repeat);
            }

            return decodedText;
        }
    }
}
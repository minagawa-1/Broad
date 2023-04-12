using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace woskni
{
    public static class Stringer
    {
        /// <summary>�����^���J���}��؂�ɂ���</summary>
        /// <param name="num">�J���}��؂�ɂ��鐔�l</param>
        /// <returns>�J���}��؂肳�ꂽ������</returns>
        public static string Camma(this object num) => string.Format("{0:#,##0}", num);

        public static string Repeat(this string text, int count)
        {
            // �A���񐔂�1�񖢖��Ȃ���Ԃ�
            if (1 > count) return null;

            string repeatText = text;

            for (int i = 1; i < count; ++i)
                text += repeatText;

            return text;
        }

        /// <summary>�w�茅���܂ŕ�������(������̑O�ɖ��߂�)</summary>
        /// <param name="str">�������߂����Ώە�����</param>
        /// <param name="fillStr">�������߂Ɏg�p���镶����(��F"�@", "0")</param>
        /// <param name="maxDigit">�������߂���ۂ̍ő啶����</param>
        /// <returns></returns>
        public static string FillFront(string str, string fillStr, int maxDigit)
        {
            // �ő啶�����𕶎����ߐ��ɕϊ�
            maxDigit -= str.Length;

            string fill = "";

            // �������ߐ�������fillStr��fillStr�����Z
            for (int i = 0; i < maxDigit; ++i) fill += fillStr;

            return fill + str;
        }

        /// <summary>�w�茅���܂ŕ�������(������̌�ɖ��߂�)</summary>
        /// <param name="str">�������߂����Ώە�����</param>
        /// <param name="filStr">�������߂Ɏg�p���镶����(��F"�@", "0")</param>
        /// <param name="maxDigit">�������߂���ۂ̍ő啶����</param>
        /// <returns></returns>
        public static string FillBack(string str, string fillStr, int maxDigit)
        {
            // �ő啶�����𕶎����ߐ��ɕϊ�
            maxDigit -= str.Length;

            string fill = "";

            // �������ߐ�������fillStr��fillStr�����Z
            for (int i = 0; i < maxDigit; ++i) fill += fillStr;

            return str + fillStr;
        }
    }
}
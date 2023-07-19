using System.Collections;
using System.Collections.Generic;

namespace woskni
{
    public partial class Math
    {
        private static List<int> fMemo = new List<int>();

        /// <summary>�w��Ԗڂ̃t�B�{�i�b�`����Ԃ�</summary>
        /// <remarks>�t�B�{�i�b�`��: n-1�̒l��n-2�̒l�����Z�����l</remarks>
        /// 
        /// <param name="num">���Ԗڂ̃t�B�{�i�b�`�����擾���邩 (0 to)</param>
        public static int Fibonacci(int num)
        {
            // ���R���łȂ����return
            if (num < 0) return -1;

            StoreFibonacciMemo(num);

            return fMemo[num];
        }

        /// <summary>�t�B�{�i�b�`���������Ɋi�[</summary>
        /// <param name="num">�i�[����ő�̓Y��</param>
        private static void StoreFibonacciMemo(int num)
        {
            // num�܂Ń����Ɋi�[����Ă��Ȃ����num�܂Ŋi�[����
            if (fMemo.Count - 1 < num)
            {
                for (int i = fMemo.Count; i <= num; ++i)
                    fMemo.Add(i < 2 ? 1 : fMemo[i - 1] + fMemo[i - 2]);
            }
        }
    }
}
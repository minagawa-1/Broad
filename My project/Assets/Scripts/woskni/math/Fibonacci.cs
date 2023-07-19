using System.Collections;
using System.Collections.Generic;

namespace woskni
{
    public partial class Math
    {
        private static List<int> fMemo = new List<int>();

        /// <summary>指定番目のフィボナッチ数を返す</summary>
        /// <remarks>フィボナッチ数: n-1の値とn-2の値を加算した値</remarks>
        /// 
        /// <param name="num">何番目のフィボナッチ数を取得するか (0 to)</param>
        public static int Fibonacci(int num)
        {
            // 自然数でなければreturn
            if (num < 0) return -1;

            StoreFibonacciMemo(num);

            return fMemo[num];
        }

        /// <summary>フィボナッチ数をメモに格納</summary>
        /// <param name="num">格納する最大の添字</param>
        private static void StoreFibonacciMemo(int num)
        {
            // numまでメモに格納されていなければnumまで格納する
            if (fMemo.Count - 1 < num)
            {
                for (int i = fMemo.Count; i <= num; ++i)
                    fMemo.Add(i < 2 ? 1 : fMemo[i - 1] + fMemo[i - 2]);
            }
        }
    }
}
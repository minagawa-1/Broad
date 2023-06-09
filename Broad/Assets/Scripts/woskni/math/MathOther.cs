using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace woskni
{
    public partial class Math
    {
        /// <summary>素数判定</summary>
        /// <param name="num">検索値</param>
        /// <returns>素数か否か</returns>
        public static bool IsPrime(long num)
        {
            // ３以下・偶数は先に判断
            if (num < 2) return false;
            if (num == 2 || num == 3) return true;
            if (num % 2 == 0 || num % 3 == 0) return false;

            // 5以上の素数は全て6n±1
            int sqrtNum = (int)System.Math.Sqrt(num);
            for (int i = 6; i <= sqrtNum; i += 6)
                if (num % i - 1 == 0 || num % (i + 1) == 0)
                    return false;

            return true;
        }

        /// <summary>階乗</summary>
        /// <param name="num">何番目の階乗か(0 to)</param>
        /// <returns>num!</returns>
        public static int Factorial(int num) => num <= 1 ? 1 : num * Factorial(num - 1);

        /// <summary>指定番目のメルセンヌ数を返す</summary>
        /// <remarks>メルセンヌ数: 2のn乗 - 1</remarks>
        /// <param name="num">何番目のメルセンヌ数を取得するか (0 to)</param>
        public static int Mersenne(int num) => num < 0 ? -1 : (int)System.Math.Pow(2, num) - 1;
    }
}
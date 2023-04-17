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
        public static bool IsPrime(int num)
        {
            // ２未満・２・偶数は先に判断
            if (num < 2) return false;
            else if (num == 2) return true;
            else if (num % 2 == 0) return false;

            // 5以上の素数はすべて 6n-1 か 6n+1 である
            if (num >= 5 && num % 6 != 1 && num % 6 != 5) return false;

            // ３以上の奇数値
            // 正の平方根numを超えるまで走査(numの平方根以降は計算式を判定させただけなため)
            float sqrtNum = (float)System.Math.Sqrt(num);
            for (int i = 3; i <= sqrtNum; i += 2)
                if (num % i == 0)
                    return false;

            // いずれも割り切れなければ素数が確定
            return true;
        }

        /// <summary>階乗</summary>
        /// <param name="num">何番目の階乗か(0 to)</param>
        /// <returns>num! の値</returns>
        public static int Factorial(int num)
        {
            int ans = 1;

            for (int i = 2; i <= num; ++i)
                ans *= i;

            return ans;
        }

        /// <summary>指定番目のメルセンヌ数を返す</summary>
        /// <remarks>メルセンヌ数: 2のn乗 - 1</remarks>
        /// 
        /// <param name="num">何番目のメルセンヌ数を取得するか (0 to)</param>
        public static int Mersenne(int num) => num < 0 ? -1 : (int)System.Math.Pow(2, num) - 1;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace woskni
{
    public partial class Math
    {
        /// <summary>‘f””»’è</summary>
        /// <param name="num">ŒŸõ’l</param>
        /// <returns>‘f”‚©”Û‚©</returns>
        public static bool IsPrime(long num)
        {
            // ‚RˆÈ‰ºE‹ô”‚Íæ‚É”»’f
            if (num < 2) return false;
            if (num == 2 || num == 3) return true;
            if (num % 2 == 0 || num % 3 == 0) return false;

            // 5ˆÈã‚Ì‘f”‚Í‘S‚Ä6n}1
            int sqrtNum = (int)System.Math.Sqrt(num);
            for (int i = 6; i <= sqrtNum; i += 6)
                if (num % i - 1 == 0 || num % (i + 1) == 0)
                    return false;

            return true;
        }

        /// <summary>ŠKæ</summary>
        /// <param name="num">‰½”Ô–Ú‚ÌŠKæ‚©(0 to)</param>
        /// <returns>num!</returns>
        public static int Factorial(int num) => num <= 1 ? 1 : num * Factorial(num - 1);

        /// <summary>w’è”Ô–Ú‚Ìƒƒ‹ƒZƒ“ƒk”‚ğ•Ô‚·</summary>
        /// <remarks>ƒƒ‹ƒZƒ“ƒk”: 2‚Ìnæ - 1</remarks>
        /// <param name="num">‰½”Ô–Ú‚Ìƒƒ‹ƒZƒ“ƒk”‚ğæ“¾‚·‚é‚© (0 to)</param>
        public static int Mersenne(int num) => num < 0 ? -1 : (int)System.Math.Pow(2, num) - 1;
    }
}
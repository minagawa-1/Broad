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
        public static bool IsPrime(int num)
        {
            // ‚Q–¢–E‚QE‹ô”‚Íæ‚É”»’f
            if (num < 2) return false;
            else if (num == 2) return true;
            else if (num % 2 == 0) return false;

            // 5ˆÈã‚Ì‘f”‚Í‚·‚×‚Ä 6n-1 ‚© 6n+1 ‚Å‚ ‚é
            if (num >= 5 && num % 6 != 1 && num % 6 != 5) return false;

            // ‚RˆÈã‚ÌŠï”’l
            // ³‚Ì•½•ûªnum‚ğ’´‚¦‚é‚Ü‚Å‘–¸(num‚Ì•½•ûªˆÈ~‚ÍŒvZ®‚ğ”»’è‚³‚¹‚½‚¾‚¯‚È‚½‚ß)
            float sqrtNum = (float)System.Math.Sqrt(num);
            for (int i = 3; i <= sqrtNum; i += 2)
                if (num % i == 0)
                    return false;

            // ‚¢‚¸‚ê‚àŠ„‚èØ‚ê‚È‚¯‚ê‚Î‘f”‚ªŠm’è
            return true;
        }

        /// <summary>ŠKæ</summary>
        /// <param name="num">‰½”Ô–Ú‚ÌŠKæ‚©(0 to)</param>
        /// <returns>num! ‚Ì’l</returns>
        public static int Factorial(int num)
        {
            int ans = 1;

            for (int i = 2; i <= num; ++i)
                ans *= i;

            return ans;
        }

        /// <summary>w’è”Ô–Ú‚Ìƒƒ‹ƒZƒ“ƒk”‚ğ•Ô‚·</summary>
        /// <remarks>ƒƒ‹ƒZƒ“ƒk”: 2‚Ìnæ - 1</remarks>
        /// 
        /// <param name="num">‰½”Ô–Ú‚Ìƒƒ‹ƒZƒ“ƒk”‚ğæ“¾‚·‚é‚© (0 to)</param>
        public static int Mersenne(int num) => num < 0 ? -1 : (int)System.Math.Pow(2, num) - 1;
    }
}
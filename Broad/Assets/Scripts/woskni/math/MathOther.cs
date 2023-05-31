using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace woskni
{
    public partial class Math
    {
        /// <summary>�f������</summary>
        /// <param name="num">�����l</param>
        /// <returns>�f�����ۂ�</returns>
        public static bool IsPrime(long num)
        {
            // �R�ȉ��E�����͐�ɔ��f
            if (num < 2) return false;
            if (num == 2 || num == 3) return true;
            if (num % 2 == 0 || num % 3 == 0) return false;

            // 5�ȏ�̑f���͑S��6n�}1
            int sqrtNum = (int)System.Math.Sqrt(num);
            for (int i = 6; i <= sqrtNum; i += 6)
                if (num % i - 1 == 0 || num % (i + 1) == 0)
                    return false;

            return true;
        }

        /// <summary>�K��</summary>
        /// <param name="num">���Ԗڂ̊K�悩(0 to)</param>
        /// <returns>num!</returns>
        public static int Factorial(int num) => num <= 1 ? 1 : num * Factorial(num - 1);

        /// <summary>�w��Ԗڂ̃����Z���k����Ԃ�</summary>
        /// <remarks>�����Z���k��: 2��n�� - 1</remarks>
        /// <param name="num">���Ԗڂ̃����Z���k�����擾���邩 (0 to)</param>
        public static int Mersenne(int num) => num < 0 ? -1 : (int)System.Math.Pow(2, num) - 1;
    }
}
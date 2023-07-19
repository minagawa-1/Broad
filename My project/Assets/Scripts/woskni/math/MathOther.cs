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
        public static bool IsPrime(int num)
        {
            // �Q�����E�Q�E�����͐�ɔ��f
            if (num < 2) return false;
            else if (num == 2) return true;
            else if (num % 2 == 0) return false;

            // 5�ȏ�̑f���͂��ׂ� 6n-1 �� 6n+1 �ł���
            if (num >= 5 && num % 6 != 1 && num % 6 != 5) return false;

            // �R�ȏ�̊�l
            // ���̕�����num�𒴂���܂ő���(num�̕������ȍ~�͌v�Z���𔻒肳���������Ȃ���)
            float sqrtNum = (float)System.Math.Sqrt(num);
            for (int i = 3; i <= sqrtNum; i += 2)
                if (num % i == 0)
                    return false;

            // �����������؂�Ȃ���Αf�����m��
            return true;
        }

        /// <summary>�K��</summary>
        /// <param name="num">���Ԗڂ̊K�悩(0 to)</param>
        /// <returns>num! �̒l</returns>
        public static int Factorial(int num)
        {
            int ans = 1;

            for (int i = 2; i <= num; ++i)
                ans *= i;

            return ans;
        }

        /// <summary>�w��Ԗڂ̃����Z���k����Ԃ�</summary>
        /// <remarks>�����Z���k��: 2��n�� - 1</remarks>
        /// 
        /// <param name="num">���Ԗڂ̃����Z���k�����擾���邩 (0 to)</param>
        public static int Mersenne(int num) => num < 0 ? -1 : (int)System.Math.Pow(2, num) - 1;
    }
}
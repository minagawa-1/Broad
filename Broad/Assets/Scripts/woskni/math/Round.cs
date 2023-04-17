using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace woskni
{
    public partial class Math
    {
        /// <summary>�l�̌ܓ�</summary>
        /// <param name="value">�l</param>
        /// <param name="place">����</param>
        public static int Round(int value, int place) => (value / (int)Mathf.Pow(10, place - 1)) % 10 >= 5 ? RoundUp(value, place) : RoundDown(value, place);

        /// <summary>�l�̌ܓ�</summary>
        /// <param name="value">�l</param>
        /// <param name="place">����</param>
        public static float Round(float value, int place) => (value / Mathf.Pow(10, place - 1)) % 10 >= 5 ? RoundUp(value, place) : RoundDown(value, place);


        /// <summary>�؂�̂�</summary>
        /// <param name="value">�l</param>
        /// <param name="place">����</param>
        public static int RoundDown(int value, int place) => value / (int)Mathf.Pow(10, place) * (int)Mathf.Pow(10, place);

        /// <summary>�؂�̂�</summary>
        /// <param name="value">�l</param>
        /// <param name="place">����</param>
        public static float RoundDown(float value, int place) => value / Mathf.Pow(10, place) * Mathf.Pow(10, place);

        /// <summary>�؂�グ</summary>
        /// <param name="value">�l</param>
        /// <param name="place">����</param>
        public static int RoundUp(int value, int place) => (value / (int)Mathf.Pow(10, place) + 1) * (int)Mathf.Pow(10, place);

        /// <summary>�؂�グ</summary>
        /// <param name="value">�l</param>
        /// <param name="place">����</param>
        public static float RoundUp(float value, int place) => (value / Mathf.Pow(10, place) + 1) * Mathf.Pow(10, place);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace woskni
{
    public partial class Math
    {
        /// <summary>四捨五入</summary>
        /// <param name="value">値</param>
        /// <param name="place">桁数</param>
        public static int Round(int value, int place) => (value / (int)Mathf.Pow(10, place - 1)) % 10 >= 5 ? RoundUp(value, place) : RoundDown(value, place);

        /// <summary>四捨五入</summary>
        /// <param name="value">値</param>
        /// <param name="place">桁数</param>
        public static float Round(float value, int place) => (value / Mathf.Pow(10, place - 1)) % 10 >= 5 ? RoundUp(value, place) : RoundDown(value, place);


        /// <summary>切り捨て</summary>
        /// <param name="value">値</param>
        /// <param name="place">桁数</param>
        public static int RoundDown(int value, int place) => value / (int)Mathf.Pow(10, place) * (int)Mathf.Pow(10, place);

        /// <summary>切り捨て</summary>
        /// <param name="value">値</param>
        /// <param name="place">桁数</param>
        public static float RoundDown(float value, int place) => value / Mathf.Pow(10, place) * Mathf.Pow(10, place);

        /// <summary>切り上げ</summary>
        /// <param name="value">値</param>
        /// <param name="place">桁数</param>
        public static int RoundUp(int value, int place) => (value / (int)Mathf.Pow(10, place) + 1) * (int)Mathf.Pow(10, place);

        /// <summary>切り上げ</summary>
        /// <param name="value">値</param>
        /// <param name="place">桁数</param>
        public static float RoundUp(float value, int place) => (value / Mathf.Pow(10, place) + 1) * Mathf.Pow(10, place);
    }
}
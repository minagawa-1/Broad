using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace woskni
{
    /// <summary>入力管理</summary>
    public class InputManager
    {
        /// <summary>入力位置を取得</summary>
        /// <param name="platform">入力デバイスの種類 (推奨: Application.platform)</param>
        /// <returns>入力位置</returns>
        public static Vector3 GetInputPosition(RuntimePlatform? platform = null)
        {
            platform ??= Application.platform;

            // Windows
            if (platform == RuntimePlatform.WindowsPlayer || platform == RuntimePlatform.WindowsEditor)
                return Input.mousePosition;

            // iPhone, Android
            if (platform == RuntimePlatform.IPhonePlayer || platform == RuntimePlatform.Android)
                return Input.GetTouch(0).position;

            return Vector3.zero;
        }

        /// <summary>入力されている間trueを返す</summary>
        /// <param name="platform">入力デバイスの種類 (推奨: Application.platform)</param>
        public static bool IsButton(RuntimePlatform? platform = null)
        {
            platform ??= Application.platform;

            // Windows
            if (platform == RuntimePlatform.WindowsPlayer || platform == RuntimePlatform.WindowsEditor)
                return Input.GetMouseButton(0);

            // iPhone, Android
            if (platform == RuntimePlatform.IPhonePlayer || platform == RuntimePlatform.Android)
                return Input.GetTouch(0).phase == TouchPhase.Moved ||
                       Input.GetTouch(0).phase == TouchPhase.Stationary;

            return false;
        }

        /// <summary>入力された瞬間にtrueを返す</summary>
        /// <param name="platform">入力デバイスの種類 (推奨: Application.platform)</param>
        public static bool IsButtonDown(RuntimePlatform? platform = null)
        {
            platform ??= Application.platform;

            // Windows
            if (platform == RuntimePlatform.WindowsPlayer || platform == RuntimePlatform.WindowsEditor)
                return Input.GetMouseButtonDown(0);

            // iPhone, Android
            if (platform == RuntimePlatform.IPhonePlayer || platform == RuntimePlatform.Android)
                return Input.GetTouch(0).phase == TouchPhase.Began;

            return false;
        }

        /// <summary>入力が解除された瞬間にtrueを返す</summary>
        /// <param name="platform">入力デバイスの種類 (推奨: Application.platform)</param>
        public static bool IsButtonUp(RuntimePlatform? platform = null)
        {
            platform ??= Application.platform;

            // Windows
            if (platform == RuntimePlatform.WindowsPlayer || platform == RuntimePlatform.WindowsEditor)
                return Input.GetMouseButtonUp(0);

            // iPhone, Android
            if (platform == RuntimePlatform.IPhonePlayer || platform == RuntimePlatform.Android)
                return Input.GetTouch(0).phase == TouchPhase.Ended;

            return false;
        }
    }
}
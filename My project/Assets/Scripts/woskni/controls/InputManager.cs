using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace woskni
{
    /// <summary>���͊Ǘ�</summary>
    public class InputManager
    {
        /// <summary>���͈ʒu���擾</summary>
        /// <param name="platform">���̓f�o�C�X�̎�� (����: Application.platform)</param>
        /// <returns>���͈ʒu</returns>
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

        /// <summary>���͂���Ă����true��Ԃ�</summary>
        /// <param name="platform">���̓f�o�C�X�̎�� (����: Application.platform)</param>
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

        /// <summary>���͂��ꂽ�u�Ԃ�true��Ԃ�</summary>
        /// <param name="platform">���̓f�o�C�X�̎�� (����: Application.platform)</param>
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

        /// <summary>���͂��������ꂽ�u�Ԃ�true��Ԃ�</summary>
        /// <param name="platform">���̓f�o�C�X�̎�� (����: Application.platform)</param>
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
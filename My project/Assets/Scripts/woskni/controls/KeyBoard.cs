using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace woskni
{
    public class KeyBoard : MonoBehaviour
    {
        /// <summary>�i�[�����</summary>
        private const int m_max_store_command = 16;

        /// <summary>���͂��ꂽ�R�}���h���i�[���郊�X�g</summary>
        public static List<KeyCode> commandList;

        /// <summary>�����L�[��OR���m</summary>
        /// <param name="keys">�������L�[</param>
        /// <returns>�����̃L�[�̂����ꂩ���������܂�Ă����</returns>
        public static bool GetOrKey(params KeyCode[] keys)
        {
            // �������Ĉ�v������̂������true��Ԃ�
            foreach (KeyCode kc in keys)
                if (Input.GetKey(kc)) return true;

            return false;
        }

        /// <summary>�����L�[��OR���m</summary>
        /// <param name="keys">�������L�[</param>
        /// <returns>�����̃L�[�̂����ꂩ���������܂ꂽ�u��</returns>
        public static bool GetOrKeyDown(params KeyCode[] keys)
        {
            // �������Ĉ�v������̂������true��Ԃ�
            foreach (KeyCode kc in keys)
                if (Input.GetKeyDown(kc)) return true;

            return false;
        }

        /// <summary>�����L�[��OR���m</summary>
        /// <param name="keys">�������L�[</param>
        /// <returns>�����̃L�[�̂����ꂩ�̉������݂��������ꂽ�u��</returns>
        public static bool GetOrKeyUp(params KeyCode[] keys)
        {
            // �������Ĉ�v������̂������true��Ԃ�
            foreach (KeyCode kc in keys)
                if (Input.GetKeyUp(kc)) return true;

            return false;
        }

        /// <summary>�����L�[��OR���m</summary>
        /// <param name="keys">�������L�[</param>
        /// <returns>�����̃L�[�̑S�Ă��������܂�Ă����</returns>
        public static bool GetAndKey(params KeyCode[] keys)
        {
            // �������Ĉ�v���Ȃ����̂������false��Ԃ�
            foreach (KeyCode kc in keys)
                if (!Input.GetKey(kc)) return false;

            return true;
        }

        /// <summary>�����L�[��AND���m</summary>
        /// <param name="key">�g���K�[�ƂȂ�L�[</param>
        /// <param name="keys">�������L�[</param>
        /// <returns>�����̃L�[�̑S�Ă��������܂ꂽ�u��</returns>
        public static bool GetAndKeyDown(KeyCode key, params KeyCode[] keys)
        {
            // �g���K�[�ƂȂ�L�[��������Ă��Ȃ����false��Ԃ�
            if (!Input.GetKeyDown(key)) return false;

            // �������Ĉ�v���Ȃ����̂������false��Ԃ�
            foreach (KeyCode kc in keys)
                if (!Input.GetKey(kc)) return false;

            return true;
        }

        /// <summary>�����L�[��AND���m</summary>
        /// <param name="key">�g���K�[�ƂȂ�L�[</param>
        /// <param name="keys">�������L�[</param>
        /// <returns>�����̃L�[�̑S�Ẳ������݂��������ꂽ�u��</returns>
        public static bool GetAndKeyUp(KeyCode key, params KeyCode[] keys)
        {
            // �g���K�[�ƂȂ�L�[��������Ă��Ȃ����false��Ԃ�
            if (!Input.GetKeyDown(key)) return false;

            // �������Ĉ�v���Ȃ����̂������false��Ԃ�
            foreach (KeyCode kc in keys)
                if (!Input.GetKey(kc)) return false;

            return true;
        }

        /// <summary>�R�}���h���͌��m</summary>
        /// <param name="keys">�R�}���h�̏���</param>
        /// <returns>�R�}���h���������Ă��邩</returns>
        public static bool IsCommandedKey(params KeyCode[] keys)
        {
            // �Ō�̃R�}���h�L�[���s���Ă��Ȃ����false��Ԃ�
            if (!Input.GetKey(keys[keys.Length - 1])) return false;

            // �������Ĉ�v���Ȃ����̂������false��Ԃ�
            for (int i = 0; i < keys.Length - 1; ++i)
                if (commandList[keys.Length - i] != keys[i]) return false;

            return true;
        }

        /// <summary>�R�}���h���͌��m</summary>
        /// <param name="keys">�R�}���h�̏���</param>
        /// <returns>�R�}���h���������Ă��邩</returns>
        public static bool IsCommandedKeyDown(params KeyCode[] keys)
        {
            // �Ō�̃R�}���h�L�[���s���Ă��Ȃ����false��Ԃ�
            if (!Input.GetKeyDown(keys[keys.Length - 1])) return false;

            if (commandList.Count >= 5)
                Debug.Log("");

            // �������Ĉ�v���Ȃ����̂������false��Ԃ�
            for (int i = 0; i < keys.Length - 1; ++i)
                if (commandList[keys.Length - i] != keys[i]) return false;

            return true;
        }

        /// <summary>�R�}���h���͌��m</summary>
        /// <param name="keys">�R�}���h�̏���</param>
        /// <returns>�R�}���h���������Ă��邩</returns>
        public static bool IsCommandedKeyUp(params KeyCode[] keys)
        {
            // �Ō�̃R�}���h�L�[���s���Ă��Ȃ����false��Ԃ�
            if (!Input.GetKeyUp(keys[keys.Length - 1])) return false;

            // �������Ĉ�v���Ȃ����̂������false��Ԃ�
            for (int i = 0; i < keys.Length - 1; ++i)
                if (commandList[keys.Length - i] != keys[i]) return false;

            return true;
        }

        public static void LogCommandList(int num)
        {
            string arrow = " �� ";
            string text = "Command: ";
            
            for (int i = 0; i < Mathf.Min(num, commandList.Count); ++i)
                text += commandList[i].ToString() + arrow;

            Debug.Log(text.Substring(0, text.Length - arrow.Length));
        }

        private void Start()
        {
            commandList = new List<KeyCode>();
            commandList.Clear();
        }

        private void Update()
        {
            // KeyCode�̑���
            if(Input.anyKeyDown){
                foreach(KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
                {
                    if (!Input.GetKeyDown(key)) continue;

                    // �擪�Ɋi�[
                    commandList.Insert(0, key);

                    // �i�[������𒴂�����ŌÂ̊i�[�L�[���폜
                    if (commandList.Count > m_max_store_command)
                        commandList.RemoveAt(m_max_store_command - 1);

                    // LogCommandList(15);

                    break;
                }
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace woskni
{
    public class KeyBoard : MonoBehaviour
    {
        /// <summary>格納上限数</summary>
        private const int m_max_store_command = 16;

        /// <summary>入力されたコマンドを格納するリスト</summary>
        public static List<KeyCode> commandList;

        /// <summary>複数キーのOR検知</summary>
        /// <param name="keys">押込中キー</param>
        /// <returns>引数のキーのいずれかが押し込まれている間</returns>
        public static bool GetOrKey(params KeyCode[] keys)
        {
            // 走査して一致するものがあればtrueを返す
            foreach (KeyCode kc in keys)
                if (Input.GetKey(kc)) return true;

            return false;
        }

        /// <summary>複数キーのOR検知</summary>
        /// <param name="keys">押込中キー</param>
        /// <returns>引数のキーのいずれかが押し込まれた瞬間</returns>
        public static bool GetOrKeyDown(params KeyCode[] keys)
        {
            // 走査して一致するものがあればtrueを返す
            foreach (KeyCode kc in keys)
                if (Input.GetKeyDown(kc)) return true;

            return false;
        }

        /// <summary>複数キーのOR検知</summary>
        /// <param name="keys">押込中キー</param>
        /// <returns>引数のキーのいずれかの押し込みが解除された瞬間</returns>
        public static bool GetOrKeyUp(params KeyCode[] keys)
        {
            // 走査して一致するものがあればtrueを返す
            foreach (KeyCode kc in keys)
                if (Input.GetKeyUp(kc)) return true;

            return false;
        }

        /// <summary>複数キーのOR検知</summary>
        /// <param name="keys">押込中キー</param>
        /// <returns>引数のキーの全てが押し込まれている間</returns>
        public static bool GetAndKey(params KeyCode[] keys)
        {
            // 走査して一致しないものがあればfalseを返す
            foreach (KeyCode kc in keys)
                if (!Input.GetKey(kc)) return false;

            return true;
        }

        /// <summary>複数キーのAND検知</summary>
        /// <param name="key">トリガーとなるキー</param>
        /// <param name="keys">押込中キー</param>
        /// <returns>引数のキーの全てが押し込まれた瞬間</returns>
        public static bool GetAndKeyDown(KeyCode key, params KeyCode[] keys)
        {
            // トリガーとなるキーが押されていなければfalseを返す
            if (!Input.GetKeyDown(key)) return false;

            // 走査して一致しないものがあればfalseを返す
            foreach (KeyCode kc in keys)
                if (!Input.GetKey(kc)) return false;

            return true;
        }

        /// <summary>複数キーのAND検知</summary>
        /// <param name="key">トリガーとなるキー</param>
        /// <param name="keys">押込中キー</param>
        /// <returns>引数のキーの全ての押し込みが解除された瞬間</returns>
        public static bool GetAndKeyUp(KeyCode key, params KeyCode[] keys)
        {
            // トリガーとなるキーが離されていなければfalseを返す
            if (!Input.GetKeyDown(key)) return false;

            // 走査して一致しないものがあればfalseを返す
            foreach (KeyCode kc in keys)
                if (!Input.GetKey(kc)) return false;

            return true;
        }

        /// <summary>コマンド入力検知</summary>
        /// <param name="keys">コマンドの順番</param>
        /// <returns>コマンドが成立しているか</returns>
        public static bool IsCommandedKey(params KeyCode[] keys)
        {
            // 最後のコマンドキーが行われていなければfalseを返す
            if (!Input.GetKey(keys[keys.Length - 1])) return false;

            // 走査して一致しないものがあればfalseを返す
            for (int i = 0; i < keys.Length - 1; ++i)
                if (commandList[keys.Length - i] != keys[i]) return false;

            return true;
        }

        /// <summary>コマンド入力検知</summary>
        /// <param name="keys">コマンドの順番</param>
        /// <returns>コマンドが成立しているか</returns>
        public static bool IsCommandedKeyDown(params KeyCode[] keys)
        {
            // 最後のコマンドキーが行われていなければfalseを返す
            if (!Input.GetKeyDown(keys[keys.Length - 1])) return false;

            if (commandList.Count >= 5)
                Debug.Log("");

            // 走査して一致しないものがあればfalseを返す
            for (int i = 0; i < keys.Length - 1; ++i)
                if (commandList[keys.Length - i] != keys[i]) return false;

            return true;
        }

        /// <summary>コマンド入力検知</summary>
        /// <param name="keys">コマンドの順番</param>
        /// <returns>コマンドが成立しているか</returns>
        public static bool IsCommandedKeyUp(params KeyCode[] keys)
        {
            // 最後のコマンドキーが行われていなければfalseを返す
            if (!Input.GetKeyUp(keys[keys.Length - 1])) return false;

            // 走査して一致しないものがあればfalseを返す
            for (int i = 0; i < keys.Length - 1; ++i)
                if (commandList[keys.Length - i] != keys[i]) return false;

            return true;
        }

        public static void LogCommandList(int num)
        {
            string arrow = " ← ";
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
            // KeyCodeの走査
            if(Input.anyKeyDown){
                foreach(KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
                {
                    if (!Input.GetKeyDown(key)) continue;

                    // 先頭に格納
                    commandList.Insert(0, key);

                    // 格納上限数を超えたら最古の格納キーを削除
                    if (commandList.Count > m_max_store_command)
                        commandList.RemoveAt(m_max_store_command - 1);

                    // LogCommandList(15);

                    break;
                }
            }
        }
    }
}
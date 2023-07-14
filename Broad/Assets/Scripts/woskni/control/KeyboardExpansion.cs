using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public static class KeyBoardExpansion
{
    /// <summary>複数キーのOR検知</summary>
    /// <param name="keys">押込中キー</param>
    /// <returns>引数のキーのいずれかが押し込まれている間</returns>
    public static bool GetOrKey(this Keyboard keyboard, params Key[] keys)
    {
        // 走査して一致するものがあればtrueを返す
        foreach (Key key in keys)
            if (keyboard[key].isPressed) return true;

        return false;
    }

    /// <summary>複数キーのOR検知</summary>
    /// <param name="keys">押込中キー</param>
    /// <returns>引数のキーのいずれかが押し込まれた瞬間</returns>
    public static bool GetOrKeyDown(this Keyboard keyboard, params Key[] keys)
    {
        // 走査して一致するものがあればtrueを返す
        foreach (Key key in keys)
            if (keyboard[key].wasPressedThisFrame) return true;

        return false;
    }

    /// <summary>複数キーのOR検知</summary>
    /// <param name="keys">押込中キー</param>
    /// <returns>引数のキーのいずれかの押し込みが解除された瞬間</returns>
    public static bool GetOrKeyUp(this Keyboard keyboard, params Key[] keys)
    {
        // 走査して一致するものがあればtrueを返す
        foreach (Key key in keys)
            if (keyboard[key].wasReleasedThisFrame) return true;

        return false;
    }

    /// <summary>複数キーのOR検知</summary>
    /// <param name="keys">押込中キー</param>
    /// <returns>引数のキーの全てが押し込まれている間</returns>
    public static bool GetAndKey(this Keyboard keyboard, params Key[] keys)
    {
        // 走査して一致しないものがあればfalseを返す
        foreach (Key key in keys)
            if (!keyboard[key].isPressed) return false;

        return true;
    }

    /// <summary>複数キーのAND検知</summary>
    /// <param name="key">トリガーとなるキー</param>
    /// <param name="keys">押込中キー</param>
    /// <returns>引数のキーの全てが押し込まれた瞬間</returns>
    public static bool GetAndKeyDown(this Keyboard keyboard, Key key, params Key[] keys)
    {
        // トリガーとなるキーが押されていなければfalseを返す
        if (!keyboard[key].wasPressedThisFrame) return false;

        // 走査して一致しないものがあればfalseを返す
        foreach (Key k in keys)
            if (!keyboard[k].isPressed) return false;

        return true;
    }

    /// <summary>複数キーのAND検知</summary>
    /// <param name="key">トリガーとなるキー</param>
    /// <param name="keys">押込中キー</param>
    /// <returns>引数のキーの全ての押し込みが解除された瞬間</returns>
    public static bool GetAndKeyUp(this Keyboard keyboard, Key key, params Key[] keys)
    {
        // トリガーとなるキーが離されていなければfalseを返す
        if (!keyboard[key].wasPressedThisFrame) return false;

        // 走査して一致しないものがあればfalseを返す
        foreach (Key k in keys)
            if (!keyboard[k].isPressed) return false;

        return true;
    }
}

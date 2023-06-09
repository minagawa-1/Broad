using System;
using System.IO;
using System.Text;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

public class ScriptCreator : MonoBehaviour
{
    /// <summary>無効文字の一覧</summary>
    public static readonly string[] invalid_chars =
    {
        " ", "!", "\"", "#", "$",
        "%", "&", "\'", "(", ")",
        "-", "=", "^",  "~", "\\",
        "|", "[", "{",  "@", "`",
        "]", "}", ":",  "*", ";",
        "+", "/", "?",  ".", ">",
        ",", "<"
    };

    /// <summary>スクリプトを作成</summary>
    /// <param name="content">スクリプト内の文字列</param>
    /// <param name="path">作成するファイルパス(Assets/～)</param>
    public static void Create(string content, string path = "Assets/Example.cs")
    {
        var directoryName = Path.GetDirectoryName(path);
        if (!Directory.Exists(directoryName)) Directory.CreateDirectory(directoryName);

        File.WriteAllText(path, content, Encoding.UTF8);
        AssetDatabase.Refresh(ImportAssetOptions.ImportRecursive);
    }

    /// <summary>スクリプトを作成</summary>
    /// <param name="content">スクリプト内の文字列</param>
    /// <param name="path">作成するファイルパス(Assets/～)</param>
    public static void Create(StringBuilder content, string path = "Assets/Example.cs") =>  Create(content.ToString(), path);

    /// <summary>クラスを作成できるか</summary>
    public static bool CanCreate() => !EditorApplication.isPlaying && !Application.isPlaying && !EditorApplication.isCompiling;

    /// <summary>無効文字を削除</summary>
    public static string RemoveInvalidChars(string str)
    {
        Array.ForEach(invalid_chars, c => str = str.Replace(c, string.Empty));
        return str;
    }
}
#endif
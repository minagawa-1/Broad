using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class ScriptCreator : MonoBehaviour
{
    /// <summary>���������̈ꗗ</summary>
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

    /// <summary>�X�N���v�g���쐬</summary>
    /// <param name="content">�X�N���v�g���̕�����</param>
    /// <param name="path">�쐬����t�@�C���p�X(Assets/�`)</param>
    public static void Create(string content, string path = "Assets/Example.cs")
    {
        var directoryName = Path.GetDirectoryName(path);
        if (!Directory.Exists(directoryName)) Directory.CreateDirectory(directoryName);

        File.WriteAllText(path, content, Encoding.UTF8);
        AssetDatabase.Refresh(ImportAssetOptions.ImportRecursive);
    }

    /// <summary>�X�N���v�g���쐬</summary>
    /// <param name="content">�X�N���v�g���̕�����</param>
    /// <param name="path">�쐬����t�@�C���p�X(Assets/�`)</param>
    public static void Create(StringBuilder content, string path = "Assets/Example.cs") =>  Create(content.ToString(), path);

    /// <summary>�N���X���쐬�ł��邩</summary>
    public static bool CanCreate() => !EditorApplication.isPlaying && !Application.isPlaying && !EditorApplication.isCompiling;

    /// <summary>�����������폜</summary>
    public static string RemoveInvalidChars(string str)
    {
        Array.ForEach(invalid_chars, c => str = str.Replace(c, string.Empty));
        return str;
    }
}

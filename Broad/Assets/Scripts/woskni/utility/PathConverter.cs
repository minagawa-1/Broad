using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>�t�@�C���p�X�̊J�n�ʒu</summary>
[System.Serializable]
public enum FilePathType
{
    /// <summary>���[�g�f�B���N�g������̃t�@�C���p�X(��FC:/�`/example.txt)</summary>
    RootDirectoryPath,

    /// <summary>�v���W�F�N�g��Assets�t�H���_����̃t�@�C���p�X(��FAssets/�`/example.txt)</summary>
    AssetsPath,

    /// <summary>�t�@�C�����̂���(��Fexample.txt)</summary>
    CurrentDirectoryPath,
}

public class PathConverter : MonoBehaviour
{
    public static string Convert(string path, FilePathType pathType)
    {
        switch (pathType)
        {
            case FilePathType.RootDirectoryPath:      return path;
            case FilePathType.AssetsPath:             return path.Substring(path.IndexOf("Assets/"));
            case FilePathType.CurrentDirectoryPath:   return path.Substring(path.LastIndexOf("/") + 1);
            default:                                                    return "";
        }
    }

    public static FilePathType Evaluate(string path)
    {
        if (path.StartsWith("c:")) return FilePathType.RootDirectoryPath;
        if (path.StartsWith("Assets")) return FilePathType.AssetsPath;

        return FilePathType.CurrentDirectoryPath;
    }
}

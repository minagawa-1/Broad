using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>ファイルパスの開始位置</summary>
[System.Serializable]
public enum FilePathType
{
    /// <summary>ルートディレクトリからのファイルパス(例：C:/～/example.txt)</summary>
    RootDirectoryPath,

    /// <summary>プロジェクトのAssetsフォルダからのファイルパス(例：Assets/～/example.txt)</summary>
    AssetsPath,

    /// <summary>ファイルそのもの(例：example.txt)</summary>
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

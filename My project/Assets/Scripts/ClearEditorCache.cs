using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ClearEditorCache : MonoBehaviour
{
    const string m_local_path = @"\AppData\Local\Unity\cache";

    /// <summary>エディタでの起動時にキャッシュを削除する</summary>
    [RuntimeInitializeOnLoadMethod]
    static void InitializeClearCache()
    {
#if DEBUG
        ClearCache();
#endif
    }

    /// <summary>キャッシュが溜まっているかを取得</summary>
    /// <returns>溜まっているか</returns>
    public static bool ExistsCache() => Directory.Exists(@"C:\Users\" + System.Environment.UserName + m_local_path);

    /// <summary>Editorのキャッシュを削除</summary>
    public static void ClearCache()
    {
        string path = @"C:\Users\" + System.Environment.UserName + m_local_path;

        DirectoryInfo folder = new DirectoryInfo(path);

        // キャッシュがなければreturn
        if (!ExistsCache()) return;

        // 1GB 以上なら削除 (サブディレクトリ含め、空でないフォルダも削除する)
        if (GetDirectorySize(folder) > Mathf.Pow(1024, 3)) folder.Delete(true);
    }

    /// <summary>指定したフォルダ・ファイルのサイズを取得</summary>
    /// <param name="directoryInfo">フォルダ・ファイルを指定</param>
    /// <returns>サイズ(byte)</returns>
    public static long GetDirectorySize(DirectoryInfo directoryInfo)
    {
        long size = 0;

        // 全ファイルの合計サイズを計算
        foreach (FileInfo fi in directoryInfo.GetFiles())
            size += fi.Length;

        // サブフォルダのサイズを合計
        foreach (DirectoryInfo di in directoryInfo.GetDirectories())
            size += GetDirectorySize(di);

        return size;
    }
}

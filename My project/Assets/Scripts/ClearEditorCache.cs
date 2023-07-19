using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ClearEditorCache : MonoBehaviour
{
    const string m_local_path = @"\AppData\Local\Unity\cache";

    /// <summary>�G�f�B�^�ł̋N�����ɃL���b�V�����폜����</summary>
    [RuntimeInitializeOnLoadMethod]
    static void InitializeClearCache()
    {
#if DEBUG
        ClearCache();
#endif
    }

    /// <summary>�L���b�V�������܂��Ă��邩���擾</summary>
    /// <returns>���܂��Ă��邩</returns>
    public static bool ExistsCache() => Directory.Exists(@"C:\Users\" + System.Environment.UserName + m_local_path);

    /// <summary>Editor�̃L���b�V�����폜</summary>
    public static void ClearCache()
    {
        string path = @"C:\Users\" + System.Environment.UserName + m_local_path;

        DirectoryInfo folder = new DirectoryInfo(path);

        // �L���b�V�����Ȃ����return
        if (!ExistsCache()) return;

        // 1GB �ȏ�Ȃ�폜 (�T�u�f�B���N�g���܂߁A��łȂ��t�H���_���폜����)
        if (GetDirectorySize(folder) > Mathf.Pow(1024, 3)) folder.Delete(true);
    }

    /// <summary>�w�肵���t�H���_�E�t�@�C���̃T�C�Y���擾</summary>
    /// <param name="directoryInfo">�t�H���_�E�t�@�C�����w��</param>
    /// <returns>�T�C�Y(byte)</returns>
    public static long GetDirectorySize(DirectoryInfo directoryInfo)
    {
        long size = 0;

        // �S�t�@�C���̍��v�T�C�Y���v�Z
        foreach (FileInfo fi in directoryInfo.GetFiles())
            size += fi.Length;

        // �T�u�t�H���_�̃T�C�Y�����v
        foreach (DirectoryInfo di in directoryInfo.GetDirectories())
            size += GetDirectorySize(di);

        return size;
    }
}

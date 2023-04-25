using UnityEngine;
using System;
using System.IO;
using System.Text;

public class TextOperate : MonoBehaviour
{
    /// <summary>�e�L�X�g�t�@�C���̏�������</summary>
    /// <param name="filePath">�t�@�C���p�X (Asset/�`)</param>
    /// <param name="text">�e�L�X�g�t�@�C���ɏ������ޕ�����</param>
    public static void WriteFile(string filePath, string text)
    {
        FileInfo file = new FileInfo(Application.dataPath + "/" + filePath);

        // �㏑�����[�h��StreamWriter���쐬
        var writer = new StreamWriter(file.FullName, false);

        // �e�L�X�g����������
        writer.WriteLine(text);

        // �t���b�V���I
        writer.Flush();

        // �X�g���[�������
        writer.Close();
    }

    /// <summary>�e�L�X�g�t�@�C���̓ǂݍ���</summary>
    /// <param name="filePath">�t�@�C���p�X (Asset/�`)</param>
    /// <returns>�e�L�X�g�t�@�C������ǂݍ��񂾕�����</returns>
    public static string ReadFile(string filePath)
    {
        FileInfo file = new FileInfo(Application.dataPath + "/" + filePath);

        try
        {
            using (StreamReader sr = new StreamReader(file.OpenRead(), Encoding.UTF8))
            {
                return sr.ReadToEnd();
            }
        }
        catch (Exception e) {
            return e.ToString();
        }
    }
}
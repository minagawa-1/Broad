using UnityEngine;
using System;
using System.IO;
using System.Text;

public class TextOperate : MonoBehaviour
{
    /// <summary>テキストファイルの書き込み</summary>
    /// <param name="filePath">ファイルパス (Asset/～)</param>
    /// <param name="text">テキストファイルに書き込む文字列</param>
    public static void WriteFile(string filePath, string text)
    {
        FileInfo file = new FileInfo(Application.dataPath + "/" + filePath);

        // 上書きモードでStreamWriterを作成
        var writer = new StreamWriter(file.FullName, false);

        // テキストを書き込む
        writer.WriteLine(text);

        // フラッシュ！
        writer.Flush();

        // ストリームを閉じる
        writer.Close();
    }

    /// <summary>テキストファイルの読み込み</summary>
    /// <param name="filePath">ファイルパス (Asset/～)</param>
    /// <returns>テキストファイルから読み込んだ文字列</returns>
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
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public static class StringExpansion
{
    /// <summary>文字列を繰り返す</summary>
    /// <param name="count">繰り返す回数</param>
    public static string Repeat(this string text, int count)
    {
        // 連続回数が1回未満なら空を返す
        if (count < 1) return null;

        string repeatText = text;

        for (int i = 1; i < count; ++i)
            text += repeatText;

        return text;
    }

    /// <summary>ヒット数を取得</summary>
    /// <param name="search">検索文字列</param>
    public static int HitCount(this string text, string search) => new Regex(Regex.Escape(search)).Matches(text).Count;

    /// <summary>数値を抽出</summary>
    /// <returns>見つからなかった場合、0を返す</returns>
    public static double ExtractNumerics(this string text)
    {
        // 正規表現を適用してマッチした文字列を取得
        // [-+]?    ：- or + が 0～1回現れる
        // \d+      ：数値(\d)が1回以上現れる
        // (\.\d+)? ：小数点以下の数値(\.\d+)が1回以上現れる
        Match match = Regex.Match(text, @"[-+]?\d+(\.\d+)?");

        // マッチが成功している場合は、double型に変換して返す
        return match.Success && double.TryParse(match.Value, out double result) ? result : 0;
    }

    /// <summary>指定行目の文字列を抽出</summary>
    /// <param name="line">行</param>
    public static string GetLine(this string text, int line)
    {
        return text.Split(new[] { System.Environment.NewLine }, System.StringSplitOptions.None)[line];
    }

    /// <summary>指定行範囲の文字列を抽出</summary>
    /// <param name="startLine">開始行</param>
    /// <param name="endLine">終了行</param>
    public static string GetLine(this string text, int startLine, int endLine)
    {
        var result = text.Split(new[] { System.Environment.NewLine }, System.StringSplitOptions.None);

        return string.Join(System.Environment.NewLine, result.Skip(startLine).Take(endLine));
    }

    /// <summary>全角に変換</summary>
    /// <param name="halfText">半角の文字列</param>
    public static string ToFullWidth(this string halfText)
    {
        string fullWidthStr = null;

        // 半角の文字コードに加算して全角にする
        for (int i = 0; i < halfText.Length; i++) fullWidthStr += (char)(halfText[i] + 65248);

        return fullWidthStr;
    }

    /// <summary>半角に変換</summary>
    /// <param name="fullText">全角の文字列</param>
    static public string ToHalfWidth(this string fullText)
    {
        string halfWidthStr = null;

        // 全角の文字コードに減算して半角にする
        for (int i = 0; i < fullText.Length; i++) halfWidthStr += (char)(fullText[i] - 65248);

        return halfWidthStr;
    }

    /// <summary>指定桁数まで文字埋め(文字列の前に埋める)</summary>
    /// <param name="str">文字埋めされる対象文字列</param>
    /// <param name="fillStr">文字埋めに使用する文字列(例："　", "0")</param>
    /// <param name="maxDigit">文字埋めする際の最大文字数</param>
    /// <returns></returns>
    public static string FillFront(string str, string fillStr, int maxDigit)
    {
        // 最大文字数を文字埋め数に変換
        maxDigit -= str.Length;

        string fill = "";

        // 文字埋め数分だけfillStrにfillStrを加算
        for (int i = 0; i < maxDigit; ++i) fill += fillStr;

        return fill + str;
    }

    /// <summary>指定桁数まで文字埋め(文字列の後に埋める)</summary>
    /// <param name="str">文字埋めされる対象文字列</param>
    /// <param name="filStr">文字埋めに使用する文字列(例："　", "0")</param>
    /// <param name="maxDigit">文字埋めする際の最大文字数</param>
    /// <returns></returns>
    public static string FillBack(string str, string fillStr, int maxDigit)
    {
        // 最大文字数を文字埋め数に変換
        maxDigit -= str.Length;

        string fill = "";

        // 文字埋め数分だけfillStrにfillStrを加算
        for (int i = 0; i < maxDigit; ++i) fill += fillStr;

        return str + fillStr;
    }
}

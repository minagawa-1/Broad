using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static  class StringExpansion
{
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

    /// <summary>指定桁数まで文字埋め(文字列の前に埋める)</summary>
    /// <param name="str">文字埋めされる対象文字列</param>
    /// <param name="fillStr">文字埋めに使用する文字列(例："　", "0")</param>
    /// <param name="maxDigit">文字埋めする際の最大文字数</param>
    /// <returns></returns>
    public static string ZeroFillFront(string str, string fillStr, int maxDigit)
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

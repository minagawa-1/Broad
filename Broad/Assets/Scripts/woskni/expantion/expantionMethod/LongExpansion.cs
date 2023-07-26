using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LongExpansion
{
    const int unit = 1024;

    /// <summary>値をフォーマット</summary>
    /// <param name="fileSize">元のサイズ</param>
    /// <returns>例: 1263000 => 1.26M</returns>
    public static string FormatSize(this long sourceSize, bool isUpper = true)
    {
        string[] sizeSuffixes = { "", "K", "M", "G", "T", "P" };

        // ゼロ算を回避
        if (sourceSize == 0) return "0";

        sourceSize = (long)Mathf.Abs(sourceSize);
        var mag = (int)Mathf.Log(sourceSize, unit);
        var adjustedSize = sourceSize / Mathf.Pow(unit, mag);

        var sizeString = Mathf.Sign(sourceSize) * adjustedSize;
        string formated = isUpper ? sizeSuffixes[mag] : sizeSuffixes[mag].ToLower();

        return $"{sizeString:0.##} {formated}";
    }
}
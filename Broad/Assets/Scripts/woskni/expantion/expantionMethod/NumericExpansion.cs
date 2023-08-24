using System;

public static class LongExpansion
{
    /// <summary>バイトの桁区切り</summary>
    const int unit = 1024;

    /// <summary>桁数を取得</summary>
    public static int Digit(this long num) => num == 0 ? 1 : (int)Math.Log10(num) + 1;

    /// <summary>値をフォーマット</summary>
    /// <param name="fileSize">元のサイズ</param>
    /// <returns>例: 1263000 => 1.26M</returns>
    public static string FormatSize(this long sourceSize, bool isUpper = true)
    {
        string[] sizeSuffixes = { "", "K", "M", "G", "T", "P" };

        // ゼロ算を回避
        if (sourceSize == 0) return "0";

        sourceSize = (long)Math.Abs(sourceSize);
        var mag = (int)Math.Log(sourceSize, unit);
        var adjustedSize = sourceSize / Math.Pow(unit, mag);

        var sizeString = Math.Sign(sourceSize) * adjustedSize;
        string formated = isUpper ? sizeSuffixes[mag] : sizeSuffixes[mag].ToLower();

        return $"{sizeString:0.##} {formated}";
    }
}

public static class IntegerExpansion
{
    /// <summary>桁数を取得</summary>
    public static int Digit(this int num) => num == 0 ? 1 : (int)Math.Log10(num) + 1;
}
using System.Linq;
using System.Text;

/// <summary>漢数字の種類</summary>
public enum KansujiType
{
    /// <summary>通常の漢数字表記（例: 千二百五）</summary>
    Normal,

    /// <summary>桁表記がなくn桁目が0の場合は"〇"と表記（例: 一二〇五）</summary>
    NormalDigitZero,

    /// <summary>大字（例: 阡弐佰伍）</summary>
    Old,

    /// <summary>大字かつ、桁表記がなくn桁目が0の場合は"〇"と表記（例: 壱弐〇伍）</summary>
    OldDigitZero
}

public class TextConverter
{
    static readonly string[] m_NormalNumbers = {
        "〇", "一", "二", "三", "四", "五", "六", "七", "八", "九"
    };

    static readonly string[] m_OldNumbers = {
        "零", "壱", "弐", "参", "肆", "伍", "陸", "漆", "捌", "玖"
    };

    static readonly string[] m_NormalUnits = {
        "", "十", "百", "千", "万", "億", "兆"
    };

    static readonly string[] m_OldUnits = {
        "", "拾", "佰", "阡", "萬", "億", "兆"
    };

    /// <summary>数値を漢数字の文字列に変換</summary>
    /// <param name="num">漢数字にする数値</param>
    /// <param name="oji">大字（古字を用いた漢数字表記）</param>
    /// <param name="ignoreOne">中間桁の一を省略（一万一千一百一 => 一万千百一）</param>
    public static string ToKanji(int num, KansujiType type, bool ignoreOne = true)
    {
        bool old       = type == KansujiType.Old             || type == KansujiType.OldDigitZero;
        bool digitZero = type == KansujiType.NormalDigitZero || type == KansujiType.OldDigitZero;

        if (num == 0) return old ? m_OldNumbers[0] : m_NormalNumbers[0];

        int[] digits = new int[num.Digit()];

        for(int i = 0; i < digits.Length; ++i)
        {
            digits[i] = num % 10;
            num /= 10;
        }

        StringBuilder kanjiBuilder = new StringBuilder();

        for (int i = digits.Length - 1; i >= 0; i--)
        {
            int digit = digits[i];

            if (digitZero)
            {
                kanjiBuilder.Append(old ? m_OldNumbers[digit] : m_NormalNumbers[digit]);
            }
            else
            {
                string kanjiNumber = old ? m_OldNumbers[digit] : m_NormalNumbers[digit];
                string kanjiUnit   = old ? m_OldUnits  [i % 4] : m_NormalUnits  [i % 4];
                
                // 万, 億, 萬 などの追加
                if (i % 4 == 0 && i > 0) kanjiUnit += old ? m_OldUnits[i / 4 + 3] : m_NormalUnits[i / 4 + 3];

                // 一万一千一百一 を 一万千百一 にするためのスキップ処理
                if (ignoreOne && IgnoreOne(digit, i)) kanjiNumber = "";

                if (digit != 0) kanjiBuilder.Append(kanjiNumber);
                kanjiBuilder.Append(kanjiUnit);
            }

            
        }
        

        return kanjiBuilder.ToString();
    }

    static bool IgnoreOne(int digit, int i) => digit == 1 && i % 4 != 0;
}

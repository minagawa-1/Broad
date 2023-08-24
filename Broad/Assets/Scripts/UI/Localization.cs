using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Localization : MonoBehaviour
{
    // テキストコンポーネントと開始時のフォントサイズの辞書
    static Dictionary<Text, int> m_BasisFontSizes;

    // 言語情報のCSV
    static woskni.CSVLoader m_CSVLoader = null;

    /// <summary>辞書のセットアップ</summary>
    public static void Setup()
    {
        // 辞書とCSVの初期化
        if (m_CSVLoader == null)
        {
            m_CSVLoader = new woskni.CSVLoader("LanguageInfo");
            m_BasisFontSizes = new Dictionary<Text, int>();
        }

        var texts = FindObjectsOfType<Text>();

        // フォントサイズ辞書に各要素を設定
        foreach (Text text in texts) if (!m_BasisFontSizes.ContainsKey(text)) m_BasisFontSizes.Add(text, text.fontSize);
    }

    /// <summary>フォントサイズを補正</summary>
    public static void Correct()
    {
        var texts = FindObjectsOfType<Text>();

        var language = Config.data.language;

        // 辞書を参照して各テキストに補正を行う
        foreach (Text text in texts)
        {
            if (m_BasisFontSizes.ContainsKey(text))
                text.fontSize = Mathf.RoundToInt((float)m_BasisFontSizes[text] * Config.data.fontSizeScale);

            if (m_CSVLoader.Find(text.text).y == -1) continue;

            text.text = Translate(text.text, language);
        }
    }

    /// <summary>英語テキストを各言語に翻訳</summary>
    /// <param name="source">英語テキスト</param>
    /// <param name="afterLanguage">翻訳先の言語</param>
    /// <returns>設定言語に翻訳されたテキスト</returns>
    public static string Translate(string source, SystemLanguage? afterLanguage = null)
    {
        // 翻訳言語が未設定の場合、設定言語を使用する
        afterLanguage ??= Config.data.language;

        // CSVから該当する単語を探して行数を返す
        int row = m_CSVLoader.Find(source).y;

        // 見つからなかったらエラー文を返す
        if (row == -1) return $"#can not translate:\"{source}\"#";

        switch (Config.data.language)
        {
            case SystemLanguage.English: return m_CSVLoader.GetString(row, 0);
            case SystemLanguage.Japanese: return m_CSVLoader.GetString(row, 1);
            case SystemLanguage.ChineseSimplified: return m_CSVLoader.GetString(row, 2);
            default: return $"#invalid language: {afterLanguage}#";
        }
    }

    /// <summary>数値をローカライズして文字列化</summary>
    /// <param name="num">自然数</param>
    public static string LocalizeInt(int num)
    {
        switch(Config.data.language)
        {
            case SystemLanguage.English:           return num.ToString();
            case SystemLanguage.Japanese:          return num.ToString();
            case SystemLanguage.ChineseSimplified: return TextConverter.ToKanji(num, KansujiType.Normal);
            default:                               return num.ToString();
        }
    }
}

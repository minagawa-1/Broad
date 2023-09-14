using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class RankingUI : MonoBehaviour
{
    [Header("背景の幅（min: 1位以外, max: 1位）")]
    [SerializeField] woskni.Range m_BackgroundWidth;
    [SerializeField] woskni.Range m_BackgroundHeight;
    [SerializeField] woskni.Range m_RankBackgroundSize;
    [SerializeField] woskni.Range m_RankBackgroundOffset;

    [Header("文字（min: 1位以外, max: 1位）")]
    [SerializeField] woskni.RangeInt m_PlayerNameFontSize;
    [SerializeField] woskni.RangeInt m_PlayerNameOffsetX;
    [SerializeField] woskni.RangeInt m_PlayerNameOffsetY;
    [SerializeField] woskni.RangeInt m_BroadFontSize;
    [SerializeField] woskni.RangeInt m_RankFontSize;

    [Header("リザルト移行時の移動")]
    [SerializeField] Vector3 m_MoveDistance;
    [SerializeField] float   m_MoveDuration;

    [Header("リザルト移行時に選択するボタン")]
    [SerializeField] Button m_FirstSelectButton;

    [System.Serializable]
    public class ResultContent
    {
        [ReadOnly] public Text name;
        [ReadOnly] public (Text text, int value) broad;
        [ReadOnly] public (Text text, int value) rank;
        [ReadOnly] public Image background;
        [ReadOnly] public Image rankBackground;

        public ResultContent(Image background)
        {
            this.background = background;

            name            =  GameObject.Find($"{background.name}/Name")                .GetComponent<Text>();
            broad           = (GameObject.Find($"{background.name}/Broad")               .GetComponent<Text>(), -1);
            rank            = (GameObject.Find($"{background.name}/Rank Background/Rank").GetComponent<Text>(), -1);
            rankBackground  =  GameObject.Find($"{background.name}/Rank Background")     .GetComponent<Image>();
        }
    }

    [Chapter("ReadOnly")]
    public List<ResultContent> contents;

    // Start is called before the first frame update
    void Start()
    {
        m_FirstSelectButton.Select();

        var rectTransform = transform.parent.GetComponent<RectTransform>();

        rectTransform.anchoredPosition3D -= m_MoveDistance;
        rectTransform.DOMove(m_MoveDistance, m_MoveDuration).SetEase(Ease.OutQuart).SetRelative();

        // contentsの初期化
        foreach (var child in transform.GetChildren()) contents.Add(new ResultContent(child.GetComponent<Image>()));

        // ゲームメインシーンの盤面情報から各プレイヤーの得点を算出
        var broads = CalcBroad.Calc(GameManager.board);

        for (int i = 0; i < contents.Count; ++i)
        {
            FixColor(i, GameSetting.instance.playerColors[i]);
            contents[i].broad.value = broads[i];
            contents[i].broad.text.text = broads[i].ToString();

            int rank = GetRank(broads.ToArray(), i);

            contents[i].rank.value = rank;
            contents[i].rank.text.text = (rank + 1).ToString();

            // サイズの修正
            FixSize(i);
        }

        UpdateSibling();
    }

    /// <summary>ランキングの背景色を修正</summary>
    /// <param name="index">修正する背景の番号</param>
    /// <param name="color">修正後の色</param>
    void FixColor(int index, Color color)
    {
        // 背景色
        contents[index].background.color = color;
        contents[index].rankBackground.color = color;

        // 順位の光彩色
        var outlines = contents[index].rank.text.GetComponents<Outline>();

        for (int i = 0; i < outlines.Length; ++i)
            outlines[i].effectColor = color.SetHSV(s: 1f).GetAlphaColor(1f - (float)i / (float)outlines.Length);

        // ブロック数
        contents[index].broad.text.color = color.SetHSV(s: 0.5f, v: 0.4f);
    }

    /// <summary>サイズを修正</summary>
    /// <param name="index">何番目のコンテンツを修正するか</param>
    void FixSize(int index)
    {
        bool is1st = contents[index].rank.value == 0;

        // 背景
        contents[index].background.rectTransform.sizeDelta
            = new Vector2(GetRankedRange(m_BackgroundWidth, is1st), GetRankedRange(m_BackgroundHeight, is1st));

        // 順位アイコン背景
        float size = GetRankedRange(m_RankBackgroundSize, is1st);
        contents[index].rankBackground.rectTransform.sizeDelta = new Vector2(size, size);

        float offset = GetRankedRange(m_RankBackgroundOffset, is1st);
        contents[index].rankBackground.rectTransform.anchoredPosition = new Vector3(offset, -offset);

        // 各テキスト
        contents[index].name      .fontSize = GetRankedRange(m_PlayerNameFontSize, is1st);
        contents[index].broad.text.fontSize = GetRankedRange(m_BroadFontSize     , is1st);
        contents[index].rank .text.fontSize = GetRankedRange(m_RankFontSize      , is1st);

        // テキストのオフセット
        contents[index].name.rectTransform.anchoredPosition
            = new Vector3(GetRankedRange(m_PlayerNameOffsetX, is1st), GetRankedRange(m_PlayerNameOffsetY, is1st));
    }

    /// <summary>ランキングに合わせて兄弟関係を更新</summary>
    void UpdateSibling()
    {
        // 子オブジェクトをint値で降順にソート
        contents.Sort((a, b) =>
        {
            // 同じ値を持つ場合はもともとの兄弟関係の昇順でソート
            if (a.rank.value == b.rank.value)
                return -a.background.transform.GetSiblingIndex().CompareTo(b.background.transform.GetSiblingIndex());

            // 値が異なる場合は降順にソート
            return b.rank.value.CompareTo(a.rank.value);
        });

        foreach (var content in contents) content.background.transform.SetAsFirstSibling();
    }

    float GetRankedRange(woskni.Range range, bool is1st) => is1st ? range.max : range.min;
    int GetRankedRange(woskni.RangeInt range, bool is1st) => is1st ? range.max : range.min;

    int GetRank(int[] broads, int index)
    {
        // dataのコピーを作成
        int[] sortedData = new int[broads.Length];
        broads.CopyTo(sortedData, 0);

        // 降順に並べ替え（昇順ソートののち配列を反転）
        System.Array.Sort(sortedData);
        System.Array.Reverse(sortedData);

        // ソートされた配列内でのindexの位置を求める
        return System.Array.IndexOf(sortedData, broads[index]);
    }
}

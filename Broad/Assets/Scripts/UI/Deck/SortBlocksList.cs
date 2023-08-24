using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class BlocksListUI
{
    [SerializeField] Dropdown m_Dropdown;
    [SerializeField] Button m_OrderButton;

    public enum SortType
    {
        /// <summary>番号 順</summary>
        Index,

        /// <summary>ブロック数 順</summary>
        BlockNum,

        /// <summary>縦横幅 順</summary>
        AreaWidth,

        /// <summary>密度 順</summary>
        Density,
    }

    [ReadOnly, Name("昇順")] public bool asc = true;

    public void ChangeOrder(Text text)
    {
        asc = !asc;

        text.rectTransform.DoHighlight();
        text.text = Localization.Translate(asc ? "Asc" : "Desc");

        // 降順設定の場合は並びを反転させる
        blocksList.Reverse();

        // リストの順番に合わせて並べ替える
        for (int i = 0; i < blocksList.Count; ++i)
        {
            blocksList[i].transform.parent.SetSiblingIndex(i);

            int index = i;

            // クリック時コールバックのインデックスを再設定
            blocksList[i].button.onClick.RemoveAllListeners();
            blocksList[i].button.onClick.AddListener(() => m_DeckListUI.OnDecisionBlocks(index));
        }

        // カーソル移動先の再設定
        SetupNavigation();
    }

    /// <summary>ブロックスリストを並べ替え</summary>
    /// <param name="dropdown">ドロップダウン</param>
    public void Sort()
    {
        m_Dropdown.captionText.rectTransform.DoHighlight();

        switch ((SortType)m_Dropdown.value)
        {
            case SortType.Index:     SortIndex();     break;
            case SortType.BlockNum:  SortBlockNum();  break;
            case SortType.AreaWidth: SortAreaWidth(); break;
            case SortType.Density:   SortDensity();   break;
        }

        // 降順設定の場合は並びを反転させる
        if (!asc) blocksList.Reverse();

        // リストの順番に合わせて並べ替える
        for (int i = 0; i < blocksList.Count; ++i)
        {
            blocksList[i].transform.parent.SetSiblingIndex(i);

            int index = i;

            // クリック時コールバックのインデックスを再設定
            blocksList[i].button.onClick.RemoveAllListeners();
            blocksList[i].button.onClick.AddListener(() => m_DeckListUI.OnDecisionBlocks(index));
        }

            // カーソル移動先の再設定
            SetupNavigation();
    }

    /// <summary>番号 順にソート</summary>
    /// <param name="asc">昇順か否か</param>
    public void SortIndex() => blocksList.Sort((a, b) => a.blocks.index - b.blocks.index);

    /// <summary>ブロック数 順にソート</summary>
    /// <param name="asc">昇順か否か</param>
    public void SortBlockNum() => blocksList.Sort((a, b) => a.blocks.GetBlockNum() - b.blocks.GetBlockNum());

    /// <summary>縦横幅 順にソート</summary>
    /// <param name="asc">昇順か否か</param>
    public void SortAreaWidth() => blocksList.Sort((a, b) => (a.blocks.width * a.blocks.height) - (b.blocks.width * b.blocks.height));

    /// <summary>密度 順にソート</summary>
    /// <param name="asc">昇順か否か</param>
    public void SortDensity() => blocksList.Sort((a, b) => (int)(a.blocks.density * 100f) - (int)(b.blocks.density * 100f));





    void SetupNavigation()
    {
        int n = m_Content.constraintCount;

        int max = blocksList.Count;

        // ブロックスのNavigationをリセット
        for (int i = 0; i < max; ++i) blocksList[i].button.navigation = new Navigation();

        // ブロックスとそれ以外とのNavigationを設定
        m_Dropdown.navigation           = Nav(m_Dropdown          , down: blocksList[0].button);
        m_OrderButton.navigation        = Nav(m_OrderButton       , down: blocksList[1].button);
        blocksList[0].button.navigation = Nav(blocksList[0].button, up  : m_Dropdown);

        // 上下のNavigation（最上列）
        for (int i = 0; i < n; ++i)
        {
            // 上へのNavigationを設定
            if (i != 0) blocksList[i].button.navigation = Nav(blocksList[i].button, up: m_OrderButton);

            // 下へのNavigationを設定
            if (i + n < max)
                blocksList[i].button.navigation = Nav(blocksList[i].button, down: blocksList[i + n].button);
        }

        // 上下のNavigation（中間・最下列）
        for (int i = n; i < max; ++i)
        {
            // 上へのNavigationを設定
            blocksList[i].button.navigation = Nav(blocksList[i].button, up: blocksList[i - n].button);

            // 下へのNavigationを設定
            if (i + n < max)
                blocksList[i].button.navigation = Nav(blocksList[i].button, down: blocksList[i + n].button);
        }

        // 左右のNavigation
        for (int i = 0; i < max; ++i)
        {
            // 左（最左のボタン以外）
            if (i % n != 0)
                blocksList[i].button.navigation = Nav(blocksList[i].button, left : blocksList[i - 1].button);

            // 右（最右と最後のボタン以外）
            if (i % n != n - 1 && i < max - 1)
                blocksList[i].button.navigation = Nav(blocksList[i].button, right: blocksList[i + 1].button);
        }
    }
}

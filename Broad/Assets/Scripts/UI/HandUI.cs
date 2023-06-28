using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;

[RequireComponent(typeof(RectTransform))]
public class HandUI : MonoBehaviour
{
    [Chapter("UI")]
    [SerializeField] MoveButton m_ButtonPrefab;
    [SerializeField] Sprite m_BlockSprite;
    [SerializeField] float m_BlocksScale = 1f;

    [Header("ブロックスの影")]
    [SerializeField] Color m_ShadowColor = new Color(0f, 0f, 0f, 0.5f);
    [SerializeField] Vector2 m_ShadowDistance = Vector2.zero;

    [Header("手札全体の横幅")]
    public float buttonGroupWidth = Screen.width * 0.8f;


    [Chapter("デッキ・手札情報")]
    public int maxDeckBlocks;
    public int maxHandBlocks;

    [Chapter("コンポーネント")]
    [SerializeField] BlockManager m_BlockManager;

    Deck m_Deck;
    Hand m_Hand;

    MoveButton[] m_MoveButtons;

    // Start is called before the first frame update
    void Awake()
    {
        Blocks[] deck = new Blocks[maxDeckBlocks];
        m_MoveButtons = new MoveButton[maxHandBlocks];

        for (int i = 0; i < maxDeckBlocks; ++i) deck[i] = Lottery();

        m_Deck = new Deck(deck);
        m_Hand = new Hand(m_Deck, maxHandBlocks);

        BuildButton().Forget();
    }

    Blocks Lottery()
    {
        int units = Random.Range(GameSetting.instance.minBlockUnits, GameSetting.instance.maxBlockUnits + 1);
        float density = Random.Range(GameSetting.instance.minDensity, GameSetting.instance.maxDensity);

        Vector2Int pos = new Vector2Int(5, 5);

        return new Blocks(LotteryBlocks.Lottery(units, density), pos);
    }

    async UniTask BuildButton()
    {
        // playersColorの中身が入るまで待機
        await UniTask.WaitUntil(() => GameSetting.instance.playersColor.Length > 0);

        Color playerColor = GameSetting.instance.playersColor[GameSetting.instance.selfIndex - 1];

        for (int i = 0; i < maxHandBlocks; ++i)
        {
            var button = Instantiate(m_ButtonPrefab.gameObject).GetComponent<MoveButton>();

            button.gameObject.name = $"Button[{i}]";
            button.transform.SetParent(transform);

            var rectTransform = button.GetComponent<RectTransform>();

            float width = buttonGroupWidth / maxHandBlocks;
            float x = i * width;

            rectTransform.offsetMin = rectTransform.offsetMin.Setter(x: x);
            rectTransform.offsetMax = rectTransform.offsetMax.Setter(x: x + width);

            button.transform.GetChild(0).GetComponent<Text>().text = $"Blocks {i + 1}";

            // BlocksUI
            BlocksUI blocksUI = new GameObject("Blocks").AddComponent<BlocksUI>();
            var blocksUITrandform = blocksUI.gameObject.GetComponent<RectTransform>();

            blocksUI.transform.SetParent(button.transform);
            blocksUI.Initialize(m_BlockSprite, this, m_BlocksScale);
            blocksUI.SetupShadow(m_ShadowColor, m_ShadowDistance);
            blocksUI.SetupUI(m_Hand.GetBlocksAt(i), playerColor, rectTransform.position);

            // BlocksUIの位置調整
            blocksUITrandform.position = blocksUITrandform.position.Offset
                (rectTransform.sizeDelta.x / 2f, rectTransform.sizeDelta.y / 2f);

            m_MoveButtons[i] = button;
        }

        // OnClick時の挙動設定
        for(int i = 0; i < maxHandBlocks; ++i) {
            int handIndex = i;
            m_MoveButtons[i].onClick.AddListener(() => DrawHandToBoard(handIndex));
        }
    }

    /// <summary>盤面にブロックスを生成するUI処理</summary>
    public void DrawHandToBoard(int handIndex)
    {
        Vector2Int pos = GameManager.boardSize / 2;

        Debug.Log("山札の数: " + m_Deck.deck.Count);
        Debug.Log("手札の数: " + m_Hand.hand.Length);

        m_BlockManager.CreateBlock(GameSetting.instance.selfIndex, m_Hand.PlayAt(handIndex).shape, pos);

        // すべてのボタンを操作不能にする
        for(int i = 0; i < maxHandBlocks; ++i) m_MoveButtons[i].Uninteractate();
    }

    /// <summary>山札からドローするUI処理</summary>
    /// <param name="num">ドロー先の手札UIの番号</param>
    public void DrawDeckToHand(int handIndex)
    {
        m_MoveButtons[handIndex].DoMove(new Vector2(m_MoveButtons[handIndex].basisPosition.x, 0f), Ease.InOutCubic, Replace);

        void Replace()
        {
            m_Hand.SetBlocksAt(handIndex, m_Deck.Draw());

            m_MoveButtons[handIndex].DoMove(m_MoveButtons[handIndex].basisPosition, Ease.InOutCubic);
        }
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(1))
        {
            // すべてのボタンを操作可能にする
            for (int i = 0; i < maxHandBlocks; ++i) m_MoveButtons[i].Interactate();
        }
    }

    Blocks Lottery() => new Blocks(LotteryBlocks.Lottery(), new Vector2Int(5, 5));
}

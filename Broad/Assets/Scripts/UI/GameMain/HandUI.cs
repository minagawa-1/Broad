using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Cysharp.Threading.Tasks;
using DG.Tweening;

[RequireComponent(typeof(RectTransform))]
public class HandUI : MonoBehaviour
{
    [Chapter("UI")]
    [SerializeField] MoveButton m_ButtonPrefab;
    [SerializeField] Sprite m_BlockSprite;

    [Header("ブロックスの影")]
    [SerializeField] Vector2 m_ShadowDistance = Vector2.zero;

    [Header("手札全体の横幅")]
    public float buttonGroupWidth = Screen.width * 0.8f;

    [Header("初期の高さ")]
    public float initHeight = 80f;

    [Chapter("コンポーネント")]
    [SerializeField] BlockManager m_BlockManager;

    public Hand hand;

    public List<MoveButton> moveButtonList;

    [ReadOnly] public int m_AddListenerCounter;

    // Start is called before the first frame update
    void Awake()
    {
        Blocks[] deckBlocks = new Blocks[SaveSystem.data.deck.Length];
        moveButtonList = new List<MoveButton>();

        for (int i = 0; i < deckBlocks.Length; ++i) deckBlocks[i] = SaveSystem.data.deck[i];

        deckBlocks.Shuffle();

        hand = new Hand(new Deck(deckBlocks), GameSetting.hand_blocks);

        for (int i = 0; i < GameSetting.hand_blocks; ++i) BuildButton(i, true);
    }

    public void BuildButton(int handIndex, bool firstBuild = false)
    {
<<<<<<< HEAD
        if (Gamepad.current == null) return;

        // スキップ（ Yボタン | □ボタン ）
        if (Gamepad.current.buttonWest.wasPressedThisFrame)
        {
            if (!m_PopupUI.isShowing) ShowSkip(PopupUI.PopupType.Skip);
            else HideSkip();
        }

        // キャンセル（ Bボタン | ×ボタン ）
        if (m_PopupUI.isShowing && Gamepad.current.buttonSouth.wasPressedThisFrame) HideSkip(); 
    }

    public void BuildButton(int handIndex)
    {
        bool firstBuild = moveButtonList.Count < GameSetting.hand_blocks;

=======
>>>>>>> 7770864a5be58c031180ca8880d2646eb9644104
        // ボタン単体の横幅
        float width = buttonGroupWidth / GameSetting.hand_blocks;

        // ボタンの生成
        var button = Instantiate(m_ButtonPrefab.gameObject).GetComponent<MoveButton>();
        button.transform.SetParent(transform);

        // ボタンの横幅と位置を演算
        float x = handIndex * width;

        // ボタンの横幅を設定
        var rectTransform = button.GetComponent<RectTransform>();
        rectTransform.offsetMin = rectTransform.offsetMin.Setter(x: x);
        rectTransform.offsetMax = rectTransform.offsetMax.Setter(x: x + width);

        // 初期位置の設定
        bool selected = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == button.gameObject;

        // ボタンの名前を設定
        button.transform.GetChild(0).GetComponent<Text>().text = $"Blocks";

        // BlocksUI
        ButtonBlocksUI blocksUI = button.GetComponentInChildren<ButtonBlocksUI>();

        blocksUI.Setup(hand.GetBlocksAt(handIndex), m_BlockSprite, false, false);
        blocksUI.SetupShadow(m_ShadowDistance);

<<<<<<< HEAD
        if (firstBuild) {
            moveButtonList.Add(button);
            moveButtonTitleList.Add(button.transform.GetChild(0).GetComponent<Text>());
        }

        // ボタンのタイトルを設定
        moveButtonTitleList[handIndex].text = $"Blocks";

        moveButtonList[handIndex].name = $"Button[{handIndex}]";
        moveButtonList[handIndex].basisPosition = moveButtonList[handIndex].rectTransform.position;

        // 開始時の移動表示DOTween
        moveButtonList[handIndex].rectTransform.DOMove(moveButtonList[handIndex].basisPosition.Setter(y: -40f), moveButtonList[handIndex].moveTime)
            .SetEase(Ease.InOutBack).SetDelay(firstBuild ? 0.06f * handIndex : 0f).OnComplete(ValidateSetable);
=======
        moveButtonList.Add(button);
        
>>>>>>> 7770864a5be58c031180ca8880d2646eb9644104

        // ボタンを押したときの処理
        {
            moveButtonList[handIndex].name = $"Button[{handIndex}]";

            moveButtonList[handIndex].basisPosition = moveButtonList[handIndex].rectTransform.position;

            // 開始時の移動表示DOTween
            moveButtonList[handIndex].rectTransform.DOMove(moveButtonList[handIndex].basisPosition.Setter(y: -40f), moveButtonList[handIndex].moveTime)
                .SetEase(Ease.InOutBack).SetDelay(firstBuild ? 0.06f * handIndex : 0f);

            moveButtonList[handIndex].onClick.AddListener(() => PlayAt(handIndex));
        }

        // ゲーム開始時の生成後は左端の手札ボタンを選択
        if(firstBuild) UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(moveButtonList[0].gameObject);
    }

    /// <summary>手札から盤面にブロックスを生成するUI処理</summary>
    public void PlayAt(int handIndex)
    {
        // いずれかのボタンが操作不能・いずれかのボタンが移動処理中
        foreach (var mb in moveButtonList) if (!mb.interactable || DOTween.IsTweening(mb.gameObject)) return;

        Vector2Int pos = GameManager.boardSize / 2;

        var blocks = hand.GetBlocksAt(handIndex);

        // ブロックの生成
        m_BlockManager.CreateBlock(this, handIndex, blocks.shape, pos, blocks.density);

        // すべてのボタンを操作不能にする
<<<<<<< HEAD
        for(int i = 0; i < GameSetting.hand_blocks; ++i) Uninteractate(i);
    }

    /// <summary>設置スキップ</summary>
    public void ShowSkip(PopupUI.PopupType type)
    {
        // 設置不可の時のポップアップ
        m_PopupUI.ShowPopup(type);

        // 各手札ボタンの処理内容を「手札交換処理」に変更
        for (int i = 0; i < GameSetting.hand_blocks; ++i)
        {
            moveButtonList[i].onClick.RemoveAllListeners();

            // COLUMN: AddListenerの引数インデックスはループ変数への対応力がないため、
            //         一度ローカル変数を介して引数を提示する必要がある
            int index = i;
            moveButtonList[i].onClick.AddListener(() => ChangeAt(index));
        }
    }

    public void HideSkip()
    {
        // 設置不可の時のポップアップ
        m_PopupUI.HidePopup();

        // 各手札ボタンの処理内容を「手札交換処理」に変更
        for (int i = 0; i < GameSetting.hand_blocks; ++i) 
        {
            moveButtonList[i].onClick.RemoveAllListeners();

            // QUIZ: AddListenerの関数内引数に対して i ではなく index を用いている理由を述べよ
            int index = i;
            moveButtonList[i].onClick.AddListener(() => PlayAt(index));
        }
=======
        Uninteractate();
>>>>>>> 7770864a5be58c031180ca8880d2646eb9644104
    }

    /// <summary>山札から手札にドローするUI処理</summary>
    /// <param name="num">ドロー先の手札UIの番号</param>
    public void DrawAt(int handIndex)
    {
        // ① handIndexと一致するUIを下方向に移動
        // ② 手札UIのドロー処理
        Vector3 spos = moveButtonList[handIndex].basisPosition;
        Vector3 fpos = moveButtonList[handIndex].basisPosition.Setter(y: -40f);

        moveButtonList[handIndex].rectTransform.DOMove(spos, moveButtonList[handIndex].moveTime).SetEase(Ease.InCubic)
            .OnComplete(() => Rebuild(handIndex));
    }

    public void OnSet(int handIndex)
    {
        hand.hand[handIndex] = null;
    }

    public void Rebuild(int handIndex)
    {
        if (hand.deck.deck.Count == 0) return;

        Blocks blocks = hand.deck.Draw();

        hand.hand[handIndex] = blocks;

        // ボタンのブロックス画像を更新
        moveButtonList[handIndex].GetComponentInChildren<ButtonBlocksUI>().ResetBlocks(blocks);

        BuildButton(handIndex);
    }

    /// <summary>選択可能にする</summary>
    /// <param name="selectHandIndex">選択する手札番号</param>
    public void Interactate(int selectHandIndex)
    {
        // すべてのボタンを操作可能にする
        for (int i = 0; i < GameSetting.hand_blocks; ++i) moveButtonList[i].interactable = true;

        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(moveButtonList[selectHandIndex].gameObject);
    }

    /// <summary>選択不可能にする</summary>
    public void Uninteractate()
    {
<<<<<<< HEAD
        moveButtonList[handIndex].interactable = false;

        var image = moveButtonList[handIndex].GetComponentInChildren<ButtonBlocksUI>().image;
        image.DOFade(0.25f, moveButtonList[handIndex].colors.fadeDuration).SetEase(Ease.Unset);

        moveButtonTitleList[handIndex] .DOFade(0.25f, moveButtonList[handIndex].colors.fadeDuration).SetEase(Ease.Unset);
    }

    /// <summary>設置できるか検証し、設置不可能なブロックスのボタンを操作不可能にする</summary>
    /// <remarks>全て設置不可能な場合、ボタンを操作可能にしてスキップ処理に移行する</remarks>
    void ValidateSetable()
    {
        List<int> unsetables = new List<int>();

        for (int i = 0; i < GameSetting.hand_blocks; ++i)
            if (!hand.hand[i].ValidateSetable(GameManager.board, GameSetting.instance.selfIndex))
                unsetables.Add(i);

        Debug.Log($"unsetables count: {unsetables.Count}");

        // 全て設置可能なら何もせず終了
        if (unsetables.Count == 0) return;

        // 全て操作不可能な場合はスキップ処理
        if (unsetables.Count == GameSetting.hand_blocks) ShowSkip(PopupUI.PopupType.Unsetable);

        // 一部が操作できない場合は設置不可手札を操作不可能にする
        else foreach (int unsetable in unsetables) Uninteractate(unsetable);

        // 最後に操作可能な手札UIにカーソルを合わせる
        for (int i = 0; i < GameSetting.hand_blocks; ++i) {
            if (moveButtonList[i].interactable) {
                UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(moveButtonList[i].gameObject);
                break;
            }
        }
=======
        // すべてのボタンを操作可能にする
        for (int i = 0; i < GameSetting.hand_blocks; ++i) moveButtonList[i].interactable = false;
>>>>>>> 7770864a5be58c031180ca8880d2646eb9644104
    }
}

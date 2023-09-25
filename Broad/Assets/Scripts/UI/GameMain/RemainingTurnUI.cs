using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Mirror;

[RequireComponent(typeof(Text))]
public class RemainingTurnUI : MonoBehaviour
{
    [SerializeField] woskni.RangeInt m_FontSizeRange;
    [SerializeField] Color m_TenseColor;

    [Header("残り何ターン目から緊迫状態を開始するか")]
    [SerializeField] int m_StartTenseTurn;

    [Header("\"のこり\"のフォントサイズ")]
    [SerializeField] int m_RemainingFontSize = 25;

    [Chapter("ゲーム終了時情報")]
    [Header("UI・カメラの移動時間")]
    [SerializeField] float m_MoveDuration;

    [Header("UIの移動距離")]
    [SerializeField] Vector3 m_MoveDirection;

    [Header("移動するUI")]
    [SerializeField] List<RectTransform> m_MoveUIs;

    Text m_TurnText;

    Color m_BasisColor;

    // Start is called before the first frame update
    void Start()
    {
        m_TurnText = GetComponent<Text>();
        m_BasisColor = m_TurnText.color;
    }

    /// <summary>のこりターンUIを更新</summary>
    /// <param name="hand"></param>
    public void UpdateTurnUI(Hand hand)
    {
        var tenseRange = new woskni.RangeInt(1, m_StartTenseTurn);

        int turn = GetRemainingBlocks(hand);

        float rate = Mathf.InverseLerp(tenseRange.min, tenseRange.max, turn);

        m_TurnText.fontSize = (int)Mathf.Lerp(m_FontSizeRange.max, m_FontSizeRange.min, rate);

        m_TurnText.color = Color.Lerp(m_TenseColor, m_BasisColor, rate);

        m_TurnText.text = $"<size={m_RemainingFontSize}>{Localization.Translate("remain")}</size>\n{turn}";

        if (turn == 0) OnFinish();
    }

    void OnFinish()
    {
        // ゲームが終了したら通信を切る
        NetworkManager.singleton.StopHost();

        Move(m_MoveUIs.ToArray());

        Camera.main.transform.DOMoveX(GameManager.boardSize.x * 0.25f, m_MoveDuration).SetRelative().SetEase(Ease.InOutCirc)
            .OnComplete(() => SceneManager.LoadScene((int)Scene.ResultScene, LoadSceneMode.Additive));
    }

    /// <summary>デッキと手札の合計ブロックス保有数を取得</summary>
    /// <param name="hand">調べる手札</param>
    int GetRemainingBlocks(Hand hand) => hand.deck.deck.Count + (GameSetting.hand_blocks - hand.FindBlank().Length);

    /// <summary>UIを移動</summary>
    /// <param name="moveUIs">移動するUI</param>
    void Move(params RectTransform[] moveUIs)
    {
        foreach(var moveUI in moveUIs)
            moveUI.DOMove(m_MoveDirection, m_MoveDuration).SetRelative().SetEase(Ease.InCirc);
    }
}

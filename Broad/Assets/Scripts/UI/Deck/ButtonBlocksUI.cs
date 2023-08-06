using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Image))]
public class ButtonBlocksUI : MonoBehaviour
{
    [Name("ブロックスの拡大率")]
    [SerializeField] float m_Scale = 1f;

    [Header("コンポーネント")]
    [SerializeField] RectTransform m_ButtonRectTransform;
    public Text blockIndexText;
    public Text blockNumText;

    [HideInInspector] public RectTransform rectTransform;
    [HideInInspector] public RectTransform buttonTransform;
    [HideInInspector] public Button        button;
    [HideInInspector] public Image         image;

    public Blocks blocks;

    public void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        buttonTransform = transform.parent.GetComponent<RectTransform>();
        button = transform.parent.GetComponent<Button>();
        image = GetComponent<Image>();
    }

    public void Setup(Blocks blocks, Sprite blockSprite, bool hasNumText = true, bool hasIndexText = true)
    {
        if (hasIndexText) blockIndexText.text = Localization.LocalizeInt(blocks.index + 1);

        // ブロックスが未設定なら終了
        if (blocks == null) return;
        if (blocks.shape.Length == 0) return;

        this.blocks = blocks;

        if (hasNumText)
        {
            // "コ"のフォントサイズを取得
            int fontSize = (int)blockNumText.text.ExtractNumerics();

            // 設定の文字サイズを反映
            fontSize = (int)((float)fontSize * Config.data.fontSizeScale);

            // 個数表記をローカライズ
            string pieces = Localization.Translate("pcs");

            // ブロック数テキストの設定
            blockNumText.text = $"{Localization.LocalizeInt(blocks.GetBlockNum())}<size={fontSize}> {pieces}</size>";
        }

        // サイズ設定
        float limitSize = Mathf.Max(3, this.blocks.width, this.blocks.height);

        float dispBlocksSize = m_ButtonRectTransform.sizeDelta.x / limitSize;

        image.rectTransform.sizeDelta = new Vector2(this.blocks.width * dispBlocksSize, this.blocks.height * dispBlocksSize);

        // ブロックスの画像設定
        Texture2D texture = this.blocks.GetBlocksTexture(blockSprite);
        image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);

        // 拡大率と色を設定
        image.rectTransform.localScale = new Vector2(m_Scale, m_Scale);

        image.color = GameSetting.instance.playerColors[GameSetting.instance.selfIndex];
    }

    public void SetupShadow(Vector2 distance, Color? color = null)
    {
        Shadow shadow = image.gameObject.GetOrAddComponent<Shadow>();

        // 影のオフセット値
        Vector2 offset = image.rectTransform.sizeDelta / new Vector2(blocks.width, blocks.height);
        shadow.effectDistance = new Vector2(offset.x * distance.x, offset.y * distance.y);

        // 影の色
        shadow.effectColor = color ?? Color.black.GetAlphaColor(0.1f);
    }
}

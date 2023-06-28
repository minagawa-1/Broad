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

    [HideInInspector] public RectTransform rectTransform;
    [HideInInspector] public Image image;

    Blocks m_Blocks;

    public void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
    }
    public void Setup(Blocks blocks, Sprite blockSprite)
    {
        m_Blocks = blocks;

        // 単ブロックの画像サイズ
        var blockSize = new Vector2Int(blockSprite.texture.width, blockSprite.texture.height);

        // Rawm_Imageに設定する画像
        var glowTexture = new Texture2D(m_Blocks.width * blockSize.x, m_Blocks.height * blockSize.y);

        // 背景色は透明にする
        Color32[] pixels = glowTexture.GetPixels32();
        for (int i = 0; i < pixels.Length; i++) pixels[i] = Color.clear;
        glowTexture.SetPixels32(pixels);

        // フィルターモードの設定
        glowTexture.filterMode = FilterMode.Point;

        // サイズ設定
        float limitSize = Mathf.Max(3, m_Blocks.width, m_Blocks.height);
        float dispBlocksSize = m_ButtonRectTransform.sizeDelta.x / limitSize;

        image.rectTransform.sizeDelta = new Vector2(m_Blocks.width * dispBlocksSize, m_Blocks.height * dispBlocksSize);


        // 位置
        image.rectTransform.position = m_ButtonRectTransform.rect.center;

        // 画像情報
        Color[] spritePixels = blockSprite.texture.GetPixels();



        int cnt = 0;

        for (int y = 0; y < m_Blocks.height; ++y)
        {
            for (int x = 0; x < m_Blocks.width; ++x)
            {
                if (!m_Blocks.shape[x, y]) continue;

                glowTexture.SetPixels(x * blockSize.x, y * blockSize.y, blockSize.x, blockSize.x, spritePixels);

                if (++cnt > m_Blocks.GetBlockNum()) break;
            }
        }

        // 画像情報を決定して格納
        glowTexture.Apply();
        image.sprite = Sprite.Create(glowTexture, new Rect(0, 0, glowTexture.width, glowTexture.height), Vector2.zero);

        // 拡大率と色を設定
        image.rectTransform.localScale = new Vector2(m_Scale, m_Scale);
        image.color = GameSetting.instance.playersColor[GameSetting.instance.selfIndex];
    }

    public void SetupShadow(Vector2 distance, Color? color = null)
    {
        Shadow shadow = image.gameObject.GetOrAddComponent<Shadow>();

        // 影のオフセット値
        Vector2 offset = image.rectTransform.sizeDelta / new Vector2(m_Blocks.width, m_Blocks.height);
        shadow.effectDistance = new Vector2(offset.x * distance.x, offset.y * distance.y);

        // 影の色
        shadow.effectColor = color ?? Color.black.GetAlphaColor(0.1f);
    }
}

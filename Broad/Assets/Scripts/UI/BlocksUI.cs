using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class BlocksUI : MonoBehaviour
{
    public float scale = 1f;

    public HandUI handUI;


    Sprite m_BlockSprite;
    Image m_Image;
    Shadow m_Shadow;

    Vector2 m_ShadowDistance;

    public void Initialize(Sprite sprite, HandUI handUI, float scale)
    {
        this.m_BlockSprite = sprite;
        this.scale = scale;
        this.handUI = handUI;

        m_Image = gameObject.AddComponent<Image>();
    }

    public void SetupShadow(Color shadowColor, Vector2 shadowDistance)
    {
        m_Shadow = gameObject.AddComponent<Shadow>();
        m_ShadowDistance = shadowDistance;
        m_Shadow.effectColor = shadowColor;
    }

    public void SetupUI(Blocks blocks, Color playerColor, Vector3 buttonPosition)
    {
        // 単ブロックの画像サイズ
        var blockSize = new Vector2Int(m_BlockSprite.texture.width, m_BlockSprite.texture.height);

        // Rawm_Imageに設定する画像
        var texture = new Texture2D(blocks.width * blockSize.x, blocks.height * blockSize.y);

        // 背景色は透明にする
        Color32[] pixels = texture.GetPixels32();
        for (int i = 0; i < pixels.Length; i++) pixels[i] = Color.clear;
        texture.SetPixels32(pixels);

        // フィルターモードの設定
        texture.filterMode = FilterMode.Point;

        // サイズ設定
        float limitSize = Mathf.Max(3, blocks.width, blocks.height);
        float dispBlocksSize = (handUI.buttonGroupWidth / (float)handUI.maxHandBlocks) / limitSize;

        m_Image.rectTransform.sizeDelta = new Vector2(blocks.width * dispBlocksSize, blocks.height * dispBlocksSize);
        

        // 位置
        m_Image.rectTransform.position = buttonPosition;

        // 画像情報
        Color[] spritePixels = m_BlockSprite.texture.GetPixels();

        int cnt = 0;

        for (int y = 0; y < blocks.height; ++y)
        {
            for (int x = 0; x < blocks.width; ++x)
            {

                if (!blocks.shape[x, y]) continue;

                texture.SetPixels(x * blockSize.x
                                , (blocks.height - y - 1) * blockSize.y
                                , blockSize.x
                                , blockSize.y
                                , spritePixels);

                if (++cnt > blocks.GetBlockNum()) break;
            }
        }

        // 画像情報を決定して格納
        texture.Apply();
        m_Image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);

        // 拡大率と色を設定
        m_Image.rectTransform.localScale = new Vector2(scale, scale);
        m_Image.color = playerColor;

        // 影のオフセット値
        Vector2 offset = m_Image.rectTransform.sizeDelta / new Vector2(blocks.width, blocks.height);
        m_Shadow.effectDistance = new Vector2(offset.x * m_ShadowDistance.x, offset.y * m_ShadowDistance.y);
    }
}

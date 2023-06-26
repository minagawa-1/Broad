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
        // �P�u���b�N�̉摜�T�C�Y
        var blockSize = new Vector2Int(m_BlockSprite.texture.width, m_BlockSprite.texture.height);

        // Rawm_Image�ɐݒ肷��摜
        var glowTexture = new Texture2D(blocks.width * blockSize.x, blocks.height * blockSize.y);

        // �w�i�F�͓����ɂ���
        Color32[] pixels = glowTexture.GetPixels32();
        for (int i = 0; i < pixels.Length; i++) pixels[i] = Color.clear;
        glowTexture.SetPixels32(pixels);

        // �t�B���^�[���[�h�̐ݒ�
        glowTexture.filterMode = FilterMode.Point;

        // �T�C�Y�ݒ�
        float limitSize = Mathf.Max(3, blocks.width, blocks.height);
        float dispBlocksSize = (handUI.buttonGroupWidth / (float)handUI.maxHandBlocks) / limitSize;

        m_Image.rectTransform.sizeDelta = new Vector2(blocks.width * dispBlocksSize, blocks.height * dispBlocksSize);
        

        // �ʒu
        m_Image.rectTransform.position = buttonPosition;

        // �摜���
        Color[] spritePixels = m_BlockSprite.texture.GetPixels();



        int cnt = 0;

        for (int y = 0; y < blocks.height; ++y)
        {
            for (int x = 0; x < blocks.width; ++x)
            {
                if (!blocks.shape[x, y]) continue;

                glowTexture.SetPixels(x * blockSize.x, y * blockSize.y, blockSize.x, blockSize.x, spritePixels);

                if (++cnt > blocks.GetBlockNum()) break;
            }
        }

        // �摜�������肵�Ċi�[
        glowTexture.Apply();
        m_Image.sprite = Sprite.Create(glowTexture, new Rect(0, 0, glowTexture.width, glowTexture.height), Vector2.zero);

        // �g�嗦�ƐF��ݒ�
        m_Image.rectTransform.localScale = new Vector2(scale, scale);
        m_Image.color = playerColor;

        // �e�̃I�t�Z�b�g�l
        Vector2 offset = m_Image.rectTransform.sizeDelta / new Vector2(blocks.width, blocks.height);
        m_Shadow.effectDistance = new Vector2(offset.x * m_ShadowDistance.x, offset.y * m_ShadowDistance.y);
    }
}

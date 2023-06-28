using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlocksListUI : MonoBehaviour
{
    [Header("生成するブロックスの数")]
    [SerializeField] int m_CreateNum;

    [Header("ブロックスの画像")]
    [SerializeField] Sprite m_BlockSprite;

    [Header("影の距離")]
    [SerializeField] Vector2 m_ShadowDistance;

    [Space(20)]
    [Header("コンポーネント・プレファブ")]
    [SerializeField] GameObject m_ButtonPrefab;
    [SerializeField] GridLayoutGroup m_Content;

    List<ButtonBlocksUI> m_BlocksList;

    // Start is called before the first frame update
    void Start()
    {
        m_BlocksList = new List<ButtonBlocksUI>();

        for (int i = 0; i < m_CreateNum; ++i)
        {
            m_BlocksList.Add(AddContent());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    ButtonBlocksUI AddContent()
    {
        var blocksUI = Instantiate(m_ButtonPrefab).GetComponentInChildren<ButtonBlocksUI>();
        blocksUI.transform.parent.name = $"Blocks[{m_BlocksList.Count}]";
        blocksUI.transform.parent.SetParent(m_Content.transform);

        blocksUI.Setup(Lottery(), m_BlockSprite);
        blocksUI.SetupShadow(m_ShadowDistance);

        blocksUI.rectTransform.anchoredPosition = new Vector2(0f, 25f);

        return blocksUI;
    }

    Blocks Lottery() => new Blocks(LotteryBlocks.Lottery(), new Vector2Int(5, 5));
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour
{
    [Header("生成したいブロックのプレファブ")]
    [SerializeField] GameObject m_BlockPrefab = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CreateBlock(GameObject blockPrefab, Vector2Int position)
    {
        // ブロックの生成
        GameObject newBlock = Instantiate(blockPrefab);
    }
}

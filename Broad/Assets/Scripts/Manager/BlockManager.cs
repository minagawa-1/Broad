using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour
{
    [Header("�����������u���b�N�̃v���t�@�u")]
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
        // �u���b�N�̐���
        GameObject newBlock = Instantiate(blockPrefab);
    }
}

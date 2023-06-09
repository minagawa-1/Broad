using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PlayerPrefabsList")]
public class PlayerPrefabsList : ScriptableObject
{
    // プレイヤープレハブリスト
    public GameObject[] m_PlayerPrefabsList = null;
}

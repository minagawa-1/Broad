using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GameSetting : ScriptableObject
{
    public static GameSetting instance;

    // ScriptableObjectの初期化時にインスタンスを設定
    private void OnEnable() => instance = this;

    [Chapter("ゲーム情報")]

    [Header("プレイヤー人数")]
    [Min(1)] public int playerNum;

    [Header("プレイヤーの色"), ReadOnly]
    public Color[] playerColors;

    [Chapter("盤面情報")]

    [Header("盤面サイズ")]
    [Min(1)] public Vector2Int minBoardSize;
    [Min(1)] public Vector2Int maxBoardSize;

    [Header("マス目の生存率")]
    [Range(0f, 1f)] public float boardViability = 0.8f;

    [Header("パーリンノイズ拡大率")]
    [Comment("値が0に近いほど大きい穴になり、値が遠いほど斑点のような小さい穴になる", -8)]
    public float perlinScale = 0.1f;

    [Chapter("ブロックス情報")]

    [Header("ブロック数")]
    [Min(1)] public int minBlockUnits;
    [Min(1)] public int maxBlockUnits;

    [Header("密度")]
    [Range(0f, 1f)] public float minDensity;
    [Range(0f, 1f)] public float maxDensity;

    /// <summary>プレイヤー色の初期設定</summary>
    public void SetupPlayerColor()
    {
        for (int i = 0; i < instance.playerNum; ++i)
        {
            float h = Random.value;
            float s = Random.Range(0.2f, 0.5f);
            float v = Random.Range(0.9f, 1f);
            Color color1P = Color.HSVToRGB(h, s, v);

            instance.playerColors = color1P.GetRelativeColor(instance.playerNum);
        }
    }
}

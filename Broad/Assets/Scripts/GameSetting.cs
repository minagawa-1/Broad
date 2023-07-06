﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[CreateAssetMenu]
public class GameSetting : ScriptableObject
{
    public static GameSetting instance;

    // GameSetting がアクティブになった瞬間
    private void OnEnable()  { if (!instance) instance = this; }

    // GameSetting が非アクティブになった瞬間・ゲームの実行時
    private void OnDisable() { if (!instance) instance = this; }

// 定数

    // デッキに設定できるブロックスの数
    public const int deck_blocks = 16;



// 変数

    [Chapter("ゲーム情報")]

    [Header("プレイヤーの情報")]
    public int selfIndex = 0;
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

    [Header("ブロックスの彩度")]
    [Range(0f, 1f)] public float minSaturation;
    [Range(0f, 1f)] public float maxSaturation;

    [Header("ブロックスの明度")]
    [Range(0f, 1f)] public float minValue;
    [Range(0f, 1f)] public float maxValue;

    /// <summary>ブロックスのカラーをランダムに取得</summary>
    /// <returns>ランダムなブロックスの色</returns>
    public Color GetRandomColor()
    {
        float s = Random.Range(minSaturation, maxSaturation);
        float v = Random.Range(minValue, maxValue);

        return Color.HSVToRGB(Random.value, s, v);
    }
}

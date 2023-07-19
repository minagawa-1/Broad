using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public const float levelMin = 0f;
    public const float levelMax = 1f;

    public static woskni.Range levelRange = new woskni.Range(levelMin, levelMax);

    /// <summary>最小値から最大値までの、レベルでの割合を取得</summary>
    /// <param name="min">最小値</param>
    /// <param name="max">最大値</param>
    public static float GetLevelRange(float min, float max) => new woskni.Range(min, max).GetCompress(SaveSystem.m_SaveData.difficulty, levelRange);
}

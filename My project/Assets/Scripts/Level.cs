using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public const float levelMin = 0f;
    public const float levelMax = 1f;

    public static woskni.Range levelRange = new woskni.Range(levelMin, levelMax);

    /// <summary>�ŏ��l����ő�l�܂ł́A���x���ł̊������擾</summary>
    /// <param name="min">�ŏ��l</param>
    /// <param name="max">�ő�l</param>
    public static float GetLevelRange(float min, float max) => new woskni.Range(min, max).GetCompress(SaveSystem.m_SaveData.difficulty, levelRange);
}

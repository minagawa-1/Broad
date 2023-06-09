using System.Collections;
using System.Collections.Generic;

public static class ListExpansion
{
    /// <summary>ランダムな要素を抽出</summary>
	public static T AtRandom<T>(this List<T> list) => list[UnityEngine.Random.Range(0, list.Count)];

    /// <summary>リスト内をシャッフルする</summary>
    public static void Shuffle<T>(this IList<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = UnityEngine.Random.Range(i, list.Count);
            T temp = list[randomIndex];
            list[randomIndex] = list[i];
            list[i] = temp;
        }
    }

    /// <summary>配列外参照検知</summary>
    /// <param name="index">参照する番号</param>
    public static bool IsProtrude<T>(this IList<T> list, int index) => index >= list.Count || index < 0;
}

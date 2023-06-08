using System.Collections;
using System.Collections.Generic;

public static class ListExpansion
{
    /// <summary>�����_���ȗv�f�𒊏o</summary>
	public static T AtRandom<T>(this List<T> list) => list[UnityEngine.Random.Range(0, list.Count)];

    /// <summary>���X�g�����V���b�t������</summary>
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

    /// <summary>�z��O�Q�ƌ��m</summary>
    /// <param name="index">�Q�Ƃ���ԍ�</param>
    public static bool IsProtrude<T>(this IList<T> list, int index) => index >= list.Count || index < 0;
}

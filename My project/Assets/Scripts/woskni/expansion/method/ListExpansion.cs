using System.Collections;
using System.Collections.Generic;

public static class ListExpansion
{
	public static T AtRandom<T>(this List<T> list) => list[UnityEngine.Random.Range(0, list.Count)];
}

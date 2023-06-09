using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TranformExpansion
{
	/// <summary>子オブジェクトを取得（非再帰的）</summary>
	/// <remarks>孫以降は取得しない。取得したい場合はGetComponentsInChildren</remarks>
	public static Transform[] GetChildren(this Transform transform)
	{
		var list = new List<Transform>();

		for (int i = 0; i < transform.childCount; ++i)
			list.Add(transform.GetChild(i));
		return list.ToArray();
	}

	/// <summary>最上位の親を取得</summary>
	/// <remarks>再起関数によって親を取得し続ける</remarks>
	public static Transform GetRootParent(this Transform transform)
		=> transform.parent == null ? transform : transform.parent.GetRootParent();

	/// <summary>Transform[]をGameObject[]に変換</summary>
	public static GameObject[] ToGameObjects(this Transform[] transforms)
    {
		var gameObjects = new GameObject[transforms.Length];

		for (int i = 0; i < transforms.Length; ++i)
			gameObjects[i] = transforms[i].gameObject;

		return gameObjects;
	}
}

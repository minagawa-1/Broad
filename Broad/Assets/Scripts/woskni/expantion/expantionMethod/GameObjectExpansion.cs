using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameObjectExpansion
{
	/// <summary>子オブジェクトを取得（非再帰的）</summary>
	/// <remarks>孫以降は取得しない。取得したい場合はGetComponentsInChildrenでTransformを指定してね</remarks>
	public static GameObject[] GetChildren(this GameObject gameObject)
    {
		List<GameObject> objs = new List<GameObject>();

		for (int i = 0; i < gameObject.transform.childCount; ++i)
			objs.Add(gameObject.transform.GetChild(i).gameObject);

		return objs.ToArray();
	}

	/// <summary>レイヤーを変更</summary>
	/// <param name="gameObject">お前</param>
	/// <param name="layer">レイヤー番号</param>
	/// <param name="needSetChildrens">子オブジェクトのレイヤーも変更するか</param>
	public static void SetLayer(this GameObject gameObject, int layer, bool needSetChildrens = true)
	{
		if (!gameObject) return;

		gameObject.layer = layer;

		if (!needSetChildrens) return;

		foreach (Transform childTransform in gameObject.transform)
			SetLayer(childTransform.gameObject, layer, needSetChildrens);
	}

	/// <summary>レイヤーを変更</summary>
	/// <param name="gameObject">お前</param>
	/// <param name="layerName">レイヤー名</param>
	/// <param name="needSetChildrens">子オブジェクトのレイヤーも変更するか</param>
	public static void SetLayer(this GameObject gameObject, string layerName, bool needSetChildrens = true)
	{
		SetLayer(gameObject, LayerMask.NameToLayer(layerName), needSetChildrens);
	}
}
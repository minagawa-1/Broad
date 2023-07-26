using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameObjectExpansion
{
	/// <summary>GameObject[]をTransform[]に変換</summary>
	public static Transform[] ToTransforms(this GameObject[] gameObjects)
	{
		var transforms = new Transform[gameObjects.Length];

		for (int i = 0; i < gameObjects.Length; ++i)
			transforms[i] = gameObjects[i].transform;

		return transforms;
	}

	/// <summary>コンポーネントを取得（ない場合はアタッチして返す）</summary>
	public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
	{
		T component = gameObject.GetComponent<T>();

		return component != null ? component : gameObject.AddComponent<T>();
    }

	/// <summary>レイヤーを変更</summary>
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
	/// <param name="layerName">レイヤー名</param>
	/// <param name="needSetChildrens">子オブジェクトのレイヤーも変更するか</param>
	public static void SetLayer(this GameObject gameObject, string layerName, bool needSetChildrens = true)
	{
		SetLayer(gameObject, LayerMask.NameToLayer(layerName), needSetChildrens);
	}
}
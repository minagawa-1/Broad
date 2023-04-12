using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameObjectExpansion
{
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
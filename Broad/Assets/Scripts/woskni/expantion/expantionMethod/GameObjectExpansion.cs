using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameObjectExpansion
{
	/// <summary>GameObject[]��Transform[]�ɕϊ�</summary>
	public static Transform[] ToTransforms(this GameObject[] gameObjects)
	{
		var transforms = new Transform[gameObjects.Length];

		for (int i = 0; i < gameObjects.Length; ++i)
			transforms[i] = gameObjects[i].transform;

		return transforms;
	}

	/// <summary>���C���[��ύX</summary>
	/// <param name="gameObject">���O</param>
	/// <param name="layer">���C���[�ԍ�</param>
	/// <param name="needSetChildrens">�q�I�u�W�F�N�g�̃��C���[���ύX���邩</param>
	public static void SetLayer(this GameObject gameObject, int layer, bool needSetChildrens = true)
	{
		if (!gameObject) return;

		gameObject.layer = layer;

		if (!needSetChildrens) return;

		foreach (Transform childTransform in gameObject.transform)
			SetLayer(childTransform.gameObject, layer, needSetChildrens);
	}

	/// <summary>���C���[��ύX</summary>
	/// <param name="gameObject">���O</param>
	/// <param name="layerName">���C���[��</param>
	/// <param name="needSetChildrens">�q�I�u�W�F�N�g�̃��C���[���ύX���邩</param>
	public static void SetLayer(this GameObject gameObject, string layerName, bool needSetChildrens = true)
	{
		SetLayer(gameObject, LayerMask.NameToLayer(layerName), needSetChildrens);
	}
}
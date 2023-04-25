using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameObjectExpansion
{
	/// <summary>�q�I�u�W�F�N�g���擾�i��ċA�I�j</summary>
	/// <remarks>���ȍ~�͎擾���Ȃ��B�擾�������ꍇ��GetComponentsInChildren��Transform���w�肵�Ă�</remarks>
	public static GameObject[] GetChildren(this GameObject gameObject)
    {
		List<GameObject> objs = new List<GameObject>();

		for (int i = 0; i < gameObject.transform.childCount; ++i)
			objs.Add(gameObject.transform.GetChild(i).gameObject);

		return objs.ToArray();
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
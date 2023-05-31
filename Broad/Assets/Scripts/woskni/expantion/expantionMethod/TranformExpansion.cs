using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TranformExpansion
{
	/// <summary>�q�I�u�W�F�N�g���擾�i��ċA�I�j</summary>
	/// <remarks>���ȍ~�͎擾���Ȃ��B�擾�������ꍇ��GetComponentsInChildren</remarks>
	public static Transform[] GetChildren(this Transform transform)
	{
		var list = new List<Transform>();

		for (int i = 0; i < transform.childCount; ++i)
			list.Add(transform.GetChild(i));
		return list.ToArray();
	}

	/// <summary>�ŏ�ʂ̐e���擾</summary>
	/// <remarks>�ċN�֐��ɂ���Đe���擾��������</remarks>
	public static Transform GetRootParent(this Transform transform)
		=> transform.parent == null ? transform : transform.parent.GetRootParent();

	/// <summary>Transform[]��GameObject[]�ɕϊ�</summary>
	public static GameObject[] ToGameObjects(this Transform[] transforms)
    {
		var gameObjects = new GameObject[transforms.Length];

		for (int i = 0; i < transforms.Length; ++i)
			gameObjects[i] = transforms[i].gameObject;

		return gameObjects;
	}
}

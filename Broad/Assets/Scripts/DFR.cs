using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DFR : MonoBehaviour
{
	[SerializeField] float angle = 0f;
	[SerializeField] float velocity = 0f;

	[SerializeField, ReadOnly] float result;


    // Update is called once per frame
    void Update()
    {
		result = calcDistance(angle, velocity);

	}

	/// <summary>飛距離計算</summary>
	/// <param name="a">角度</param>
	/// <param name="v">速度</param>
	/// <returns>飛距離</returns>
	float calcDistance(float a, float v)
	{
		float distance = 0f;   // 飛距離

		// 上昇時・下降時の速度をあわせて考える（v^2）
		v = Mathf.Pow(v, 2);

		// 上昇時・下降時の角度をあわせて考える（a*2）
		a = (a * (3.14f / 180f)) * 2;

		// 速度 * 正弦の角度 を重力加速度で除算して距離を算出する
		distance = (v * Mathf.Sin(a)) / 0.98f;

		return distance;
	}
}

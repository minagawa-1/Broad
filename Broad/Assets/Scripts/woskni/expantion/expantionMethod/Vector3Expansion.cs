using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector3Expansion
{
    public static Vector3 Difference(this Vector3 v, float x = 0f, float y = 0f, float z = 0f) => Difference(v, new Vector3(x, y, z));

    public static Vector3 Difference(this Vector3 v, Vector3 dif) => v + dif;

    /// <summary>値の設定</summary>
    /// <remarks>個々の値のみの設定が可能</remarks>
    /// <param name="x">x座標の設定</param>
    /// <param name="y">y座標の設定</param>
    /// <param name="z">z座標の設定</param>
    /// <returns>設定された値</returns>
    public static Vector3 Assign(this Vector3 v, float? x = null, float? y = null, float? z = null) => new Vector3(x ?? v.x, y ?? v.y, z ?? v.z);

    public static Vector3 Assign(this Vector3 v, Vector3 set) => v = set;
}

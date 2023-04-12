using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector3Expansion
{
    public static Vector3 Difference(this Vector3 pos, float x = 0f, float y = 0f, float z = 0f) => pos + new Vector3(x, y, z);

    public static Vector3 Difference(this Vector3 pos, Vector3 dif) => pos + dif;
}

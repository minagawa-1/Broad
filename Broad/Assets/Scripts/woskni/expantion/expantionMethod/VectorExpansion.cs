using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Vector3
public static class Vector3Expansion
{
    /// <summary>一定値離れた座標を返す</summary>
    public static Vector3 Offset(this Vector3 v, float x = 0f, float y = 0f, float z = 0f) => Offset(v, new Vector3(x, y, z));

    /// <summary>一定値離れた座標を返す</summary>
    public static Vector3 Offset(this Vector3 v, Vector3 offset) => v + offset;

    /// <summary>値の設定</summary>
    /// <remarks>個々の値のみの設定が可能</remarks>
    public static Vector3 Setter(this Vector3 v, float? x = null, float? y = null, float? z = null) => new Vector3(x ?? v.x, y ?? v.y, z ?? v.z);



    /// <summary>xとyの座標を入れ替えたベクトルを返す</summary>
    public static Vector3 GetSwapXY(this Vector3 v) => new Vector3(v.y, v.x, v.z);
    /// <summary>xとzの座標を入れ替えたベクトルを返す</summary>
    public static Vector3 GetSwapXZ(this Vector3 v) => new Vector3(v.z, v.y, v.x);
    /// <summary>yとzの座標を入れ替えたベクトルを返す</summary>
    public static Vector3 GetSwapYZ(this Vector3 v) => new Vector3(v.x, v.z, v.y);

}

// Vector3Int
public static class Vector3IntExpansion
{
    /// <summary>一定値離れた座標を返す</summary>
    public static Vector3Int Offset(this Vector3Int v, int x = 0, int y = 0, int z = 0) => Offset(v, new Vector3Int(x, y, z));

    /// <summary>一定値離れた座標を返す</summary>
    public static Vector3Int Offset(this Vector3Int v, Vector3Int offset) => v + offset;

    /// <summary>値の設定</summary>
    /// <remarks>個々の値のみの設定が可能</remarks>
    public static Vector3Int Setter(this Vector3Int v, int? x = null, int? y = null, int? z = null) => new Vector3Int(x ?? v.x, y ?? v.y, z ?? v.z);



    /// <summary>xとyの座標を入れ替えたベクトルを返す</summary>
    public static Vector3Int GetSwapXY(this Vector3Int v) => new Vector3Int(v.y, v.x, v.z);
    /// <summary>xとzの座標を入れ替えたベクトルを返す</summary>
    public static Vector3Int GetSwapXZ(this Vector3Int v) => new Vector3Int(v.z, v.y, v.x);
    /// <summary>yとzの座標を入れ替えたベクトルを返す</summary>
    public static Vector3Int GetSwapYZ(this Vector3Int v) => new Vector3Int(v.x, v.z, v.y);
}



// Vector2
public static class Vector2Expansion
{
    /// <summary>一定値離れた座標を返す</summary>
    public static Vector2 Offset(this Vector2 v, float x = 0f, float y = 0f) => Offset(v, new Vector2(x, y));

    /// <summary>一定値離れた座標を返す</summary>
    public static Vector2 Offset(this Vector2 v, Vector2 offset) => v + offset;

    /// <summary>値の設定</summary>
    /// <remarks>個々の値のみの設定が可能</remarks>
    public static Vector2 Setter(this Vector2 v, float? x = null, float? y = null) => new Vector2(x ?? v.x, y ?? v.y);



    /// <summary>xとyの座標を入れ替えたベクトルを返す</summary>
    public static Vector2 GetSwapXY(this Vector2 v) => new Vector2(v.y, v.x);
}

// Vector2Int
public static class Vector2IntExpansion
{
    /// <summary>一定値離れた座標を返す</summary>
    public static Vector2Int Offset(this Vector2Int v, int x = 0, int y = 0) => Offset(v, new Vector2Int(x, y));

    /// <summary>一定値離れた座標を返す</summary>
    public static Vector2Int Offset(this Vector2Int v, Vector2Int offset) => v + offset;

    /// <summary>値の設定</summary>
    /// <remarks>個々の値のみの設定が可能</remarks>
    public static Vector2Int Setter(this Vector2Int v, int? x = null, int? y = null) => new Vector2Int(x ?? v.x, y ?? v.y);



    /// <summary>xとyの座標を入れ替えたベクトルを返す</summary>
    public static Vector2Int GetSwapXY(this Vector2Int v) => new Vector2Int(v.y, v.x);
}
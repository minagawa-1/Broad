using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshCombiner : MonoBehaviour
{
    /// <summary><b>メッシュの結合</b></summary>
    /// <remarks>複数のGameObjectのメッシュを結合して、ひとつのGameObjectにする</remarks>
    /// <param name="gameObjects">結合するGameObject</param>
    /// <param name="name">結合されたGameObjectの名前</param>
    /// <param name="parent">結合されたGameObjectの親</param>
    /// <returns>結合されたGameObject</returns>
    public static GameObject Combine(GameObject[] gameObjects, string name = "NewObject", Transform parent = null)
    {
        // 結合するGameObjectの配列が空ならreturn
        if (gameObjects.Length <= 0) return null;

        MeshFilter     [] meshFilters = new MeshFilter     [gameObjects.Length];    // 結合前オブジェクト達のメッシュ等情報
        CombineInstance[] combine     = new CombineInstance[gameObjects.Length];    // 結合後オブジェクト　のメッシュ等情報

        // 結合に向けてオブジェクトの情報を格納する
        for (int i = 0; i < gameObjects.Length; ++i)
        {
            meshFilters[i] = gameObjects[i].GetComponent<MeshFilter>();

            if (meshFilters[i] == null) continue;

            combine[i].mesh      = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
        }

        GameObject newObj = new GameObject(name);
        newObj.transform.SetParent(parent);
        
        // メッシュを結合したメッシュとして作り直す
        MeshFilter meshFilter = newObj.AddComponent<MeshFilter>();
        meshFilter.sharedMesh = new Mesh();
        meshFilter.sharedMesh.CombineMeshes(combine);

        // 作り直したメッシュを設定する
        newObj.AddComponent<MeshRenderer>().material = meshFilters[0].GetComponent<MeshRenderer>().sharedMaterial;

        // 元々のオブジェクトは破棄する
        foreach (var obj in gameObjects) Destroy(obj);

        return newObj;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshCombiner : MonoBehaviour
{
    /// <summary><b>���b�V���̌���</b></summary>
    /// <remarks>������GameObject�̃��b�V�����������āA�ЂƂ�GameObject�ɂ���</remarks>
    /// <param name="gameObjects">��������GameObject</param>
    /// <param name="name">�������ꂽGameObject�̖��O</param>
    /// <param name="parent">�������ꂽGameObject�̐e</param>
    /// <returns>�������ꂽGameObject</returns>
    public static GameObject Combine(GameObject[] gameObjects, string name = "NewObject", Transform parent = null)
    {
        // ��������GameObject�̔z�񂪋�Ȃ�return
        if (gameObjects.Length <= 0) return null;

        MeshFilter     [] meshFilters = new MeshFilter     [gameObjects.Length];    // �����O�I�u�W�F�N�g�B�̃��b�V�������
        CombineInstance[] combine     = new CombineInstance[gameObjects.Length];    // ������I�u�W�F�N�g�@�̃��b�V�������

        // �����Ɍ����ăI�u�W�F�N�g�̏����i�[����
        for (int i = 0; i < gameObjects.Length; ++i)
        {
            meshFilters[i] = gameObjects[i].GetComponent<MeshFilter>();

            if (meshFilters[i] == null) continue;

            combine[i].mesh      = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
        }

        GameObject newObj = new GameObject(name);
        newObj.transform.SetParent(parent);
        
        // ���b�V���������������b�V���Ƃ��č�蒼��
        MeshFilter meshFilter = newObj.AddComponent<MeshFilter>();
        meshFilter.sharedMesh = new Mesh();
        meshFilter.sharedMesh.CombineMeshes(combine);

        // ��蒼�������b�V����ݒ肷��
        newObj.AddComponent<MeshRenderer>().material = meshFilters[0].GetComponent<MeshRenderer>().sharedMaterial;

        // ���X�̃I�u�W�F�N�g�͔j������
        foreach (var obj in gameObjects) Destroy(obj);

        return newObj;
    }
}

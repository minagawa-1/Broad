using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

public class ChangeActiveGameObject
{
    private const string m_Key = "Woskni/ChangeActive _,";

    [MenuItem(m_Key, false)]
    public static void Invert()
    {
        foreach (var n in Selection.gameObjects)
            n.SetActive(!n.activeSelf);
    }

    [MenuItem(m_Key, true)]
    public static bool CanInvert()
    {
        GameObject[] obj = Selection.gameObjects;
        return obj != null && 0 < obj.Length;
    }
}
#endif
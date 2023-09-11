using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;

[InitializeOnLoad]
public class HierarchySymbols : MonoBehaviour
{
    public static bool drawing = true;

    static HierarchySymbols()
    {
        if (drawing) EditorApplication.hierarchyWindowItemOnGUI += DrawHierarchySymbols;
    }

    private static void DrawHierarchySymbols(int instanceID, Rect selectionRect)
    {
        GameObject gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;

        if (gameObject != null && gameObject.transform.parent != null)
                SymbolsField(selectionRect, GetSymbols(gameObject.transform));
    }

    /// <summary>Transformの親子関係から罫線素片を取得</summary>
    /// <param name="transform"></param>
    /// <returns>表示用の罫線素片の配列</returns>
    private static string[] GetSymbols(Transform transform)
    {
        int depth = GetHierarchyDepth(transform);

        List<string> symbols = new List<string>();

        // ("│")
        for (int i = 0; i < depth - 1; ++i) symbols.Add(!IsLast(GetAncestor(transform, depth - i - 1)) ? "│" : "　");


        // ("└", "├")
        if (transform.childCount == 0) symbols.Add(IsLast(transform) ? "└" : "├");

        return symbols.ToArray();
    }

    /// <summary>ヒエラルキー上の深さ（先祖の長さ）を取得</summary>
    private static int GetHierarchyDepth(Transform transform)
    {
        int depth = 0;
        while (transform.parent != null)
        {
            depth++;
            transform = transform.parent;
        }
        return depth;
    }

    /// <summary>複数の罫線素片をひとつの文字列に変換する</summary>
    private static void SymbolsField(Rect selectionRect, params string[] symbols)
    {
        const int indent = 59;
        const int interval = 14;

        for(int i = 0; i < symbols.Length; ++i)
        {
            float x = i * interval + indent;

            Rect labelRect = new Rect(x, selectionRect.y, interval, selectionRect.height);

            GUIStyle style = new GUIStyle();
            style.normal.textColor = EditorTheme.GetIconColor().GetAlphaColor(0.5f);
            style.fontSize = 15;

            EditorGUI.LabelField(labelRect, symbols[i], style);
        }
    }

    /// <summary>そのTransformの先祖を調べる</summary>
    /// <param name="degree">何親等さかのぼるか（1: 親, 2: 祖親, 3: 曾祖親 …)</param>
    /// <returns></returns>
    private static Transform GetAncestor(Transform transform, int degree)
    {
        for (int i = 0; i < degree; ++i) transform = transform.parent;

        return transform;
    }

    /// <summary>そのTransformが兄弟関係において末弟かどうか</summary>
    private static bool IsLast(Transform transform) => transform.GetSiblingIndex() == transform.parent.childCount - 1;
}
#endif
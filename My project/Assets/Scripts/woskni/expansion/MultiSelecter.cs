using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

public static class MultiSelecter
{
    private const int m_width = 16;

    [InitializeOnLoadMethod]
    private static void GUIUpdate()
    {
        EditorApplication.hierarchyWindowItemOnGUI += OnGUI;
    }

    private static void OnGUI(int instanceID, Rect selectionRect)
    {
        Rect rect = selectionRect;
        rect.x = rect.xMax - m_width;
        rect.width = m_width;

        bool oldSelected = Selection.Contains(instanceID);
        bool newSelected = GUI.Toggle(rect, oldSelected, string.Empty);

        // 追記・削除箇所がなければreturn
        if (newSelected == oldSelected) return;

        // 変更前のinstanceID情報
        int[] instanceIDs = Selection.instanceIDs;

        // 取得したinstanceID情報の追記・削除を行う
        if (newSelected) ArrayUtility.Add   (ref instanceIDs, instanceID);
        else             ArrayUtility.Remove(ref instanceIDs, instanceID);

        // 本来のInstanceID情報に反映させる
        Selection.instanceIDs = instanceIDs;
    }
}
#endif
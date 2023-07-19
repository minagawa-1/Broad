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

        // �ǋL�E�폜�ӏ����Ȃ����return
        if (newSelected == oldSelected) return;

        // �ύX�O��instanceID���
        int[] instanceIDs = Selection.instanceIDs;

        // �擾����instanceID���̒ǋL�E�폜���s��
        if (newSelected) ArrayUtility.Add   (ref instanceIDs, instanceID);
        else             ArrayUtility.Remove(ref instanceIDs, instanceID);

        // �{����InstanceID���ɔ��f������
        Selection.instanceIDs = instanceIDs;
    }
}
#endif
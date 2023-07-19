using UnityEditor;
using System.Linq;

#if UNITY_EDITOR
using UnityEngine;

public class SetParentWindow : ScriptableWizard
{
    [Header("設定する親オブジェクト名")]
    [SerializeField] string m_ParentName = "";

    [MenuItem("Woskni/Grouping %G", false)]
    public static void CreateWizard()
	{
		// ウィンドウを開く
		ScriptableWizard.DisplayWizard("Group name", typeof(SetParentWindow), "決定");
	}

    /// <summary>置換して終了</summary>
    void OnWizardCreate()
    {
        Grouping();
    }

    // 置換処理
    void Grouping()
    {
        // 何も選択されていなければreturn
        if (!Selection.activeTransform) return;

        // 親オブジェクトの作成
        GameObject obj = new GameObject(m_ParentName);

        // 親オブジェクトのレジスタ設定
        Undo.RegisterCreatedObjectUndo(obj, "オブジェクトのグループ化");

        // 選択しているオブジェクトの親情報を削除して
        // 作成したオブジェクトを親にする
        obj.transform.SetParent(Selection.activeTransform.parent, false);
        foreach (var transform in Selection.transforms.OrderBy(t => t.GetSiblingIndex()))
            Undo.SetTransformParent(transform, obj.transform, "オブジェクトのグループ化");

        // 親を選択する
        Selection.activeGameObject = obj;
    }

    /// <summary>ウィンドウの更新</summary>
    void OnWizardUpdate()
    {
        Transform[] transforms = Selection.GetTransforms(SelectionMode.Unfiltered);
        helpString = "選択オブジェクト数: " + transforms.Length;
        errorString = "";
        isValid = true;

        if (transforms.Length < 1)
            errorString += "オブジェクトが選択されておりません";

        isValid = string.IsNullOrEmpty(errorString);
    }

    /// <summary>選択オブジェクトに変更があったとき</summary>
    void OnSelectionChange()
    {
        OnWizardUpdate();
    }
}
#endif
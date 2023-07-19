using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

public class RenameSelection : ScriptableWizard
{
    [Header("変換前の文字列(空ならばafterTextを先頭に挿入")]
    [SerializeField] string m_ReplaceText = "";

    [Header("変換後の文字列")]
    [SerializeField] string m_AfterText = "";

    /// <summary>ウィンドウを開けるか</summary>
    [MenuItem("Woskni/Renamer", true)]
    static bool CanDispWizard()
    {
        // 選択中のオブジェクト数を取得して
        // １つ以上選択されていればウィンドウを開く
        Transform[] transforms = Selection.GetTransforms(SelectionMode.Unfiltered);
        return transforms.Length >= 1;
    }

    /// <summary>ウィンドウを開く</summary>
    [MenuItem("Woskni/Renamer", false)]
    static void CreateWizard()
    {
        // ウィンドウを開く
        ScriptableWizard.DisplayWizard("Renamer", typeof(RenameSelection), "決定");
    }

    /// <summary>置換して終了</summary>
    void OnWizardCreate()
    {
        Rename();
    }

    // 置換処理
    void Rename()
    {
        GameObject[] gos = Selection.gameObjects;

        // 置換前文字が空
        if (string.IsNullOrEmpty(m_ReplaceText)) {
            // 先頭に文字列を挿入
            foreach (GameObject go in gos)
                go.name = m_AfterText + go.name;
        }

        // 置換前文字変数に文字列が入っている
        else {
            // 置換前文字列が見つかったら置換を行う
            foreach (GameObject go in gos)
                if (go.name.IndexOf(m_ReplaceText) >= 0)
                    go.name = go.name.Replace(m_ReplaceText, m_AfterText);
        }
        
    }

    /// <summary>置換</summary>
    void OnWizardOtherButton()
    {
        Rename();
    }

    /// <summary>ウィンドウの更新</summary>
    void OnWizardUpdate()
    {
        Transform[] transforms = Selection.GetTransforms(SelectionMode.Unfiltered);
        helpString = "Objects selected: " + transforms.Length;
        errorString = "";
        isValid = true;

        if (transforms.Length < 1)
        {
            errorString += "No object selected to rename";
        }
        isValid = string.IsNullOrEmpty(errorString);

    }

    /// <summary>選択オブジェクトに変更があったとき</summary>
    void OnSelectionChange()
    {
        OnWizardUpdate();
    }
}
#endif
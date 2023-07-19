using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

public class RenameSelection : ScriptableWizard
{
    [Header("�ϊ��O�̕�����(��Ȃ��afterText��擪�ɑ}��")]
    [SerializeField] string m_ReplaceText = "";

    [Header("�ϊ���̕�����")]
    [SerializeField] string m_AfterText = "";

    /// <summary>�E�B���h�E���J���邩</summary>
    [MenuItem("Woskni/Renamer", true)]
    static bool CanDispWizard()
    {
        // �I�𒆂̃I�u�W�F�N�g�����擾����
        // �P�ȏ�I������Ă���΃E�B���h�E���J��
        Transform[] transforms = Selection.GetTransforms(SelectionMode.Unfiltered);
        return transforms.Length >= 1;
    }

    /// <summary>�E�B���h�E���J��</summary>
    [MenuItem("Woskni/Renamer", false)]
    static void CreateWizard()
    {
        // �E�B���h�E���J��
        ScriptableWizard.DisplayWizard("Renamer", typeof(RenameSelection), "����");
    }

    /// <summary>�u�����ďI��</summary>
    void OnWizardCreate()
    {
        Rename();
    }

    // �u������
    void Rename()
    {
        GameObject[] gos = Selection.gameObjects;

        // �u���O��������
        if (string.IsNullOrEmpty(m_ReplaceText)) {
            // �擪�ɕ������}��
            foreach (GameObject go in gos)
                go.name = m_AfterText + go.name;
        }

        // �u���O�����ϐ��ɕ����񂪓����Ă���
        else {
            // �u���O�����񂪌���������u�����s��
            foreach (GameObject go in gos)
                if (go.name.IndexOf(m_ReplaceText) >= 0)
                    go.name = go.name.Replace(m_ReplaceText, m_AfterText);
        }
        
    }

    /// <summary>�u��</summary>
    void OnWizardOtherButton()
    {
        Rename();
    }

    /// <summary>�E�B���h�E�̍X�V</summary>
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

    /// <summary>�I���I�u�W�F�N�g�ɕύX���������Ƃ�</summary>
    void OnSelectionChange()
    {
        OnWizardUpdate();
    }
}
#endif
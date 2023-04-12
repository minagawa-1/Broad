using UnityEditor;
using System.Linq;

#if UNITY_EDITOR
using UnityEngine;

public class SetParentWindow : ScriptableWizard
{
    [Header("�ݒ肷��e�I�u�W�F�N�g��")]
    [SerializeField] string m_ParentName = "";

    [MenuItem("Woskni/Grouping %G", false)]
    public static void CreateWizard()
	{
		// �E�B���h�E���J��
		ScriptableWizard.DisplayWizard("Group name", typeof(SetParentWindow), "����");
	}

    /// <summary>�u�����ďI��</summary>
    void OnWizardCreate()
    {
        Grouping();
    }

    // �u������
    void Grouping()
    {
        // �����I������Ă��Ȃ����return
        if (!Selection.activeTransform) return;

        // �e�I�u�W�F�N�g�̍쐬
        GameObject obj = new GameObject(m_ParentName);

        // �e�I�u�W�F�N�g�̃��W�X�^�ݒ�
        Undo.RegisterCreatedObjectUndo(obj, "�I�u�W�F�N�g�̃O���[�v��");

        // �I�����Ă���I�u�W�F�N�g�̐e�����폜����
        // �쐬�����I�u�W�F�N�g��e�ɂ���
        obj.transform.SetParent(Selection.activeTransform.parent, false);
        foreach (var transform in Selection.transforms.OrderBy(t => t.GetSiblingIndex()))
            Undo.SetTransformParent(transform, obj.transform, "�I�u�W�F�N�g�̃O���[�v��");

        // �e��I������
        Selection.activeGameObject = obj;
    }

    /// <summary>�E�B���h�E�̍X�V</summary>
    void OnWizardUpdate()
    {
        Transform[] transforms = Selection.GetTransforms(SelectionMode.Unfiltered);
        helpString = "�I���I�u�W�F�N�g��: " + transforms.Length;
        errorString = "";
        isValid = true;

        if (transforms.Length < 1)
            errorString += "�I�u�W�F�N�g���I������Ă���܂���";

        isValid = string.IsNullOrEmpty(errorString);
    }

    /// <summary>�I���I�u�W�F�N�g�ɕύX���������Ƃ�</summary>
    void OnSelectionChange()
    {
        OnWizardUpdate();
    }
}
#endif
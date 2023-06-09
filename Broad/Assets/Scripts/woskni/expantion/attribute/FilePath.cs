using System.IO;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>�t�@�C����I�����ăt�@�C���p�X���w��</summary>
public class FilePathAttribute : PropertyAttribute
{
    public FilePathType pathType { get; set; }
    public string delimiter { get; set; }

    /// <summary>�t�@�C����I�����ăt�@�C���p�X���w��</summary>
    /// <param name="pathType">�t�@�C���p�X�̊J�n�ʒu</param>
    /// <param name="delimiter">�t�@�C���p�X�̋�؂蕶��</param>
    public FilePathAttribute(FilePathType pathType = FilePathType.RootDirectoryPath, string delimiter = "/")
    {
        this.pathType = pathType;
        this.delimiter = delimiter;
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(FilePathAttribute))]
public class FilePathDrawer : PropertyDrawer
{
    const float button_width = 20f;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        FilePathAttribute filePathAttribute = attribute as FilePathAttribute;
        EditorGUI.BeginProperty(position, label, property);

        EditorGUI.BeginChangeCheck();
        string path = DrawFilePathField(position, property.stringValue, filePathAttribute.pathType, filePathAttribute.delimiter, label);

        if (EditorGUI.EndChangeCheck()) property.stringValue = path;

        EditorGUI.EndProperty();
    }

    private string DrawFilePathField(Rect position, string path, FilePathType pathType, string delimiter, GUIContent label)
    {
        Rect textFieldRect = new Rect(position.x, position.y, position.width - button_width, position.height);

        //EditorGUI.BeginDisabledGroup(true);
        EditorGUI.TextField(textFieldRect, label, path);
        //EditorGUI.EndDisabledGroup();

        Rect buttonRect = new Rect(position.xMax - button_width, position.y, button_width, position.height);

        if (GUI.Button(buttonRect, "�c"))
        {
            string initialPath = GetInitialPath(pathType);
            string selectedPath = EditorUtility.OpenFilePanel("Select File", initialPath, string.Empty);
            if (!string.IsNullOrEmpty(selectedPath))
            {
                path = PathConverter.Convert(selectedPath, pathType);

                // ��؂蕶�����f�t�H���g�łȂ��ꍇ�͋�؂蕶����u������
                if(delimiter != "/") path = path.Replace("/", delimiter);
            }
        }

        return path;
    }

    string GetInitialPath(FilePathType pathType)
    {
        switch (pathType)
        {
            case FilePathType.RootDirectoryPath:    return "";
            case FilePathType.AssetsPath:           return "Assets/";
            case FilePathType.CurrentDirectoryPath: return Path.GetDirectoryName(Application.dataPath).Replace("\\", "/") + "/";
            default:                                return "";
        }
    }
}
#endif
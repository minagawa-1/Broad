using System.IO;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>ファイルを選択してファイルパスを指定</summary>
public class FilePathAttribute : PropertyAttribute
{
    public FilePathType pathType { get; set; }
    public string delimiter { get; set; }

    /// <summary>ファイルを選択してファイルパスを指定</summary>
    /// <param name="pathType">ファイルパスの開始位置</param>
    /// <param name="delimiter">ファイルパスの区切り文字</param>
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

        if (GUI.Button(buttonRect, "…"))
        {
            string initialPath = GetInitialPath(pathType);
            string selectedPath = EditorUtility.OpenFilePanel("Select File", initialPath, string.Empty);
            if (!string.IsNullOrEmpty(selectedPath))
            {
                path = PathConverter.Convert(selectedPath, pathType);

                // 区切り文字がデフォルトでない場合は区切り文字を置換する
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
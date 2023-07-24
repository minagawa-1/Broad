using System.Linq;
using System.Text;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;

/// <summary>タグ名を列挙型で管理するクラスを作成するスクリプト</summary>
public static class EnumTagCreator
{
    private const string class_name = "Tag";
    private const string command = "Tools/Create/EnumTag";  // コマンド名
    private const string path = "Assets/Scripts/woskni/utility/enumCreator/EnumTag.cs"; // ファイルパス

    /// <summary>タグ名を定数で管理するクラスを作成します</summary>
    [MenuItem(command)]
    public static void Create()
    {
        if (!ScriptCreator.CanCreate()) return;

        CreateScript();

        EditorUtility.DisplayDialog(command, $"{class_name}クラスの作成完了：\n({path})", "OK");
    }

    /// <summary>スクリプトの構築処理</summary>
    public static void CreateScript()
    {
        var builder = new StringBuilder();

        builder.AppendLine("/// <summary>レイヤー(列挙型)</summary>");
        builder.AppendLine("public enum " + class_name);
        builder.AppendLine("{");

        var tags = InternalEditorUtility.tags.Select(c => new { enumeration = ScriptCreator.RemoveInvalidChars(c), name = c });
        for (int i = 0; i < tags.Count(); ++i)
        {
            builder.AppendLine($"\t/// <summary>{tags.ElementAt(i).enumeration}</summary>");

            string fin = i < tags.Count() - 1 ? "," : "";

            string enumeration = tags.ElementAt(i).enumeration;

            builder.AppendLine($"\t{enumeration}" + fin);
        }

        builder.AppendLine("}");

        ScriptCreator.Create(builder, path);
    }    
}
#endif
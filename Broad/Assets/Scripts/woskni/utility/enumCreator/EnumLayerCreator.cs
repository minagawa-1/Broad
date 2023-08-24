using System.Linq;
using System.Text;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;

/// <summary>タグ名を列挙型で管理するクラスを作成するスクリプト</summary>
public static class EnumLayerCreator
{
    private const string class_name = "Layer";
    private const string command = "Tools/Create/EnumLayer";  // コマンド名
    private const string path = "Assets/Scripts/woskni/utility/enumCreator/EnumLayer.cs"; // ファイルパス

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

        var layers = InternalEditorUtility.layers.Select(c => new { enumeration = ScriptCreator.RemoveInvalidChars(c), name = c });
        for (int i = 0; i < layers.Count(); ++i)
        {
            builder.AppendLine($"\t/// <summary>{layers.ElementAt(i).enumeration}</summary>");

            string fin = i < layers.Count() - 1 ? "," : "";

            string enumeration = layers.ElementAt(i).enumeration;

            builder.AppendLine($"\t{enumeration} = {UnityEngine.LayerMask.NameToLayer(enumeration)}" + fin);
        }

        builder.AppendLine("}");

        ScriptCreator.Create(builder, path);
    }
}
#endif
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditorInternal;

/// <summary>タグ名を列挙型で管理するクラスを作成するスクリプト</summary>
public static class EnumSceneCreator
{
    private const string class_name = "Scene";
    private const string command = "Tools/Create/EnumScene";  // コマンド名
    private const string path = "Assets/Scripts/woskni/utility/enumCreator/EnumScene.cs"; // ファイルパス

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
        builder.AppendLine("public class " + class_name);
        builder.AppendLine("{");

        var layers = GetAllScenes().Select(c => new { enumeration = ScriptCreator.RemoveInvalidChars(c), name = c });
        
        for (int i = 0; i < layers.Count(); ++i)
        {
            builder.AppendLine($"\t/// <summary>{layers.ElementAt(i).enumeration}</summary>");

            string fin = i < layers.Count() - 1 ? "\n" : "";

            builder.AppendLine($"\tpublic const string {layers.ElementAt(i).enumeration} = \"{layers.ElementAt(i).name}\";" + fin);
        }

        builder.AppendLine("}");

        ScriptCreator.Create(builder, path);
    }

    private static string[] GetAllScenes()
    {
        string[] guids = AssetDatabase.FindAssets("t:Scene");
        string[] scenes = new string[guids.Length];

        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(path);
            scenes[i] = sceneName;
        }

        return scenes;
    }
}
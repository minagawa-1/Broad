using System.Linq;
using System.Text;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
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
        builder.AppendLine("public enum " + class_name);
        builder.AppendLine("{");
        
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; ++i)
        {
            string scene = System.IO.Path.GetFileNameWithoutExtension(EditorBuildSettings.scenes[i].path);

            builder.AppendLine($"\t/// <summary>{scene}</summary>");

            string fin = i < SceneManager.sceneCountInBuildSettings - 1 ? "," : "";

            builder.AppendLine($"\t{scene}" + fin);
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
#endif
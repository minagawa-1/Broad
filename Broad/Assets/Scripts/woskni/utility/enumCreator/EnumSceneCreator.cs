using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditorInternal;

/// <summary>�^�O����񋓌^�ŊǗ�����N���X���쐬����X�N���v�g</summary>
public static class EnumSceneCreator
{
    private const string class_name = "Scene";
    private const string command = "Tools/Create/EnumScene";  // �R�}���h��
    private const string path = "Assets/Scripts/woskni/utility/enumCreator/EnumScene.cs"; // �t�@�C���p�X

    /// <summary>�^�O����萔�ŊǗ�����N���X���쐬���܂�</summary>
    [MenuItem(command)]
    public static void Create()
    {
        if (!ScriptCreator.CanCreate()) return;

        CreateScript();

        EditorUtility.DisplayDialog(command, $"{class_name}�N���X�̍쐬�����F\n({path})", "OK");
    }

    /// <summary>�X�N���v�g�̍\�z����</summary>
    public static void CreateScript()
    {
        var builder = new StringBuilder();

        builder.AppendLine("/// <summary>���C���[(�񋓌^)</summary>");
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
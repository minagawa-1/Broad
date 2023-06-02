using System.Linq;
using System.Text;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;

/// <summary>�^�O����񋓌^�ŊǗ�����N���X���쐬����X�N���v�g</summary>
public static class EnumLayerCreator
{
    private const string class_name = "Layer";
    private const string command = "Tools/Create/EnumLayer";  // �R�}���h��
    private const string path = "Assets/Scripts/woskni/utility/enumCreator/EnumLayer.cs"; // �t�@�C���p�X

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

        builder.AppendLine("/// <summary>�V�[��(�񋓌^)</summary>");
        builder.AppendLine("public class " + class_name);
        builder.AppendLine("{");

        var layers = InternalEditorUtility.layers.Select(c => new { enumeration = ScriptCreator.RemoveInvalidChars(c), name = c });
        for (int i = 0; i < layers.Count(); ++i)
        {
            builder.AppendLine($"\t/// <summary>{layers.ElementAt(i).enumeration}</summary>");

            string fin = i < layers.Count() - 1 ? "\n" : "";

            builder.AppendLine($"\tpublic const string {layers.ElementAt(i).enumeration} = \"{layers.ElementAt(i).name}\";" + fin);
        }

        builder.AppendLine("}");

        ScriptCreator.Create(builder, path);
    }
}
#endif
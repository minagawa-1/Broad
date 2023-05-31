using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditorInternal;

/// <summary>�^�O����񋓌^�ŊǗ�����N���X���쐬����X�N���v�g</summary>
public static class EnumTagCreator
{
    private const string class_name = "Tag";
    private const string command = "Tools/Create/EnumTag";  // �R�}���h��
    private const string path = "Assets/Scripts/woskni/utility/enumCreator/EnumTag.cs"; // �t�@�C���p�X

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

        builder.AppendLine("/// <summary>�^�O(�񋓌^)</summary>");
        builder.AppendLine("public class " + class_name);
        builder.AppendLine("{");

        var tags = InternalEditorUtility.tags.Select(c => new { enumeration = ScriptCreator.RemoveInvalidChars(c), name = c });
        for (int i = 0; i < tags.Count(); ++i)
        {
            builder.AppendLine($"\t/// <summary>{tags.ElementAt(i).enumeration}</summary>");

            string fin = i < tags.Count() - 1 ? "\n" : "";

            builder.AppendLine($"\tpublic const string {tags.ElementAt(i).enumeration} = \"{tags.ElementAt(i).name}\";" + fin);
        }

        builder.AppendLine("}");

        ScriptCreator.Create(builder, path);
    }    
}
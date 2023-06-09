using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ChapterAttribute : PropertyAttribute
{
    public readonly string chapterName;
    public readonly float space;

    /// <summary>区切り線の付加</summary>
    /// <param name="comment">区切り線に乗せる文字</param>
    /// <param name="space">上のフィールドとコメントの余白幅</param>
    public ChapterAttribute(string chapterName = "", float space = 20f)
    {
        this.chapterName = chapterName;
        this.space = space;
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ChapterAttribute))]
public class ChapterAttributeDrawer : DecoratorDrawer
{
    private const float line_height = 1f;
    private const float line_space = 4f;
    private const float label_indent = 24f;
    private static readonly Color color = Color.gray;

    public override float GetHeight()
    {
        ChapterAttribute chapterAttribute = attribute as ChapterAttribute;
        return EditorGUIUtility.singleLineHeight + chapterAttribute.space + line_height + line_space;
    }

    public override void OnGUI(Rect position)
    {
        ChapterAttribute chapterAttribute = attribute as ChapterAttribute;

        Rect contentPosition = EditorGUI.PrefixLabel(position, GUIContent.none);
        contentPosition.height = EditorGUIUtility.singleLineHeight;

        // 文字列の位置とサイズを計算し、描画する
        Rect labelPosition = new Rect(contentPosition.x, contentPosition.y, contentPosition.width, contentPosition.height);

        if (!string.IsNullOrEmpty(chapterAttribute.chapterName))
        {
            GUIStyle chapterStyle = new GUIStyle(EditorStyles.boldLabel);
            chapterStyle.normal.textColor = color.SetHSV(v: 0.45f);
            Vector2 chapterLabelSize = chapterStyle.CalcSize(new GUIContent(chapterAttribute.chapterName));
            Rect chapterLabelPosition = new Rect(contentPosition.x + label_indent, labelPosition.y + chapterAttribute.space, chapterLabelSize.x, labelPosition.height);
            EditorGUI.LabelField(chapterLabelPosition, chapterAttribute.chapterName, chapterStyle);

            // 線を文字列の両脇に描画
            float lineY = labelPosition.y + labelPosition.height * 0.5f - line_height * 0.5f + chapterAttribute.space;

            Rect leftLinePosition = new Rect(contentPosition.x, lineY, chapterLabelPosition.x - contentPosition.x, line_height);
            Rect rightLinePosition = new Rect(chapterLabelPosition.xMax, lineY, contentPosition.xMax - chapterLabelPosition.xMax, line_height);

            EditorGUI.DrawRect(leftLinePosition, color.SetHSV(v: 0.6f));
            EditorGUI.DrawRect(rightLinePosition, color.SetHSV(v: 0.6f));
        }
        else
        {
            // 文字列が存在しない場合は単一の線を描画
            Rect linePosition = new Rect(contentPosition.x, labelPosition.y + labelPosition.height + chapterAttribute.space, contentPosition.width, line_height);
            EditorGUI.DrawRect(linePosition, color.SetHSV(v: 0.6f));
        }
    }
}
#endif
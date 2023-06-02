using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
# endif

/// <summary>�t�B�[���h��ɃR�����g��t��</summary>
public class CommentAttribute : PropertyAttribute
{
    public readonly string comment;
    public readonly float space;

    /// <summary>�t�B�[���h��ɃR�����g��t��</summary>
    /// <param name="comment">�R�����g�e�L�X�g</param>
    /// <param name="space">��̃t�B�[���h�ƃR�����g�̗]����</param>
    public CommentAttribute(string comment = "", float space = 20f)
    {
        this.comment = comment;
        this.space = space;
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(CommentAttribute))]
public class CommentAttributeDrawer : DecoratorDrawer
{
    private const float line_width = 1f;
    private const float background_indent = 8f;
    private const float text_indent = 8f;
    private const float label_margin = 8f;

    public override float GetHeight()
    {
        var commentAttribute = attribute as CommentAttribute;

        // �Ȃɂ��܂܂�Ă��Ȃ���΁A��s���̍���
        if (commentAttribute == null) return base.GetHeight() + EditorGUIUtility.singleLineHeight;

        // ���s���l�����āA�R�����g�̍������v�Z
        var commentLines = commentAttribute.comment.Split('\n');
        var lineCount = commentLines.Length;

        // (�P�s���̍��� * �s����ǉ�) + space
        return base.GetHeight() + EditorGUIUtility.singleLineHeight * lineCount + commentAttribute.space;
    }

    public override void OnGUI(Rect position)
    {
        var commentAttribute = attribute as CommentAttribute;
        if (commentAttribute == null) return;

        // �R�����g�̘g����`��
        var commentRect = position;
        commentRect.yMin += commentAttribute.space;

        commentRect.yMin += label_margin;
        commentRect.yMax -= label_margin;
        commentRect.xMin += background_indent;
        commentRect.xMax -= background_indent;
        DrawCommentBackground(commentRect);

        // �R�����g�̃e�L�X�g��`��
        commentRect.xMin += label_margin + text_indent;
        commentRect.xMax -= label_margin + text_indent;
        EditorGUI.LabelField(commentRect, commentAttribute.comment);
    }

    private void DrawCommentBackground(Rect position)
    {
        Color editorColor = EditorTheme.GetThemeColor();

        Color backgroundColor = editorColor.SetHSV(v: (editorColor.GetValue() - 0.02f));
        Color exposedColor    = editorColor.SetHSV(v: (editorColor.GetValue() + 0.05f));
        Color shadedColor     = editorColor.SetHSV(v: (editorColor.GetValue() - 0.10f));

        // �w�i�̕`��
        EditorGUI.DrawRect(position, backgroundColor);

        // �g���̕`��
        var borderRect = position;
        borderRect.width += line_width;  // �����𒲐�
        borderRect.height += line_width; // �����𒲐�

        EditorGUI.DrawRect(new Rect(borderRect.x                , borderRect.y                  , borderRect.width  , line_width        ), shadedColor ); // ��ӂ̘g��
        EditorGUI.DrawRect(new Rect(borderRect.x                , borderRect.yMax - line_width  , borderRect.width  , line_width        ), exposedColor); // ���ӂ̘g��
        EditorGUI.DrawRect(new Rect(borderRect.x    - line_width, borderRect.y                  , line_width        , borderRect.height ), shadedColor ); // ���ӂ̘g��
        EditorGUI.DrawRect(new Rect(borderRect.xMax - line_width, borderRect.y                  , line_width        , borderRect.height ), exposedColor); // �E�ӂ̘g��
    }
}
#endif
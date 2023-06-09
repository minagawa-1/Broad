using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>フィールド上にコメントを付加</summary>
public class CommentAttribute : PropertyAttribute
{
    public readonly string comment;
    public readonly float space;

    /// <summary>フィールド上にコメントを付加</summary>
    /// <param name="comment">コメントテキスト</param>
    /// <param name="space">上のフィールドとコメントの余白幅</param>
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

        // なにも含まれていなければ、一行分の高さ
        if (commentAttribute == null) return base.GetHeight() + EditorGUIUtility.singleLineHeight;

        // 改行を考慮して、コメントの高さを計算
        var commentLines = commentAttribute.comment.Split('\n');
        var lineCount = commentLines.Length;

        // (１行分の高さ * 行数を追加) + space
        return base.GetHeight() + EditorGUIUtility.singleLineHeight * lineCount + commentAttribute.space;
    }

    public override void OnGUI(Rect position)
    {
        var commentAttribute = attribute as CommentAttribute;
        if (commentAttribute == null) return;

        // コメントの枠線を描画
        var commentRect = position;
        commentRect.yMin += commentAttribute.space;

        commentRect.yMin += label_margin;
        commentRect.yMax -= label_margin;
        commentRect.xMin += background_indent;
        commentRect.xMax -= background_indent;
        DrawCommentBackground(commentRect);

        // コメントのテキストを描画
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

        // 背景の描画
        EditorGUI.DrawRect(position, backgroundColor);

        // 枠線の描画
        var borderRect = position;
        borderRect.width += line_width;  // 横幅を調整
        borderRect.height += line_width; // 高さを調整

        EditorGUI.DrawRect(new Rect(borderRect.x                , borderRect.y                  , borderRect.width  , line_width        ), shadedColor ); // 上辺の枠線
        EditorGUI.DrawRect(new Rect(borderRect.x                , borderRect.yMax - line_width  , borderRect.width  , line_width        ), exposedColor); // 下辺の枠線
        EditorGUI.DrawRect(new Rect(borderRect.x    - line_width, borderRect.y                  , line_width        , borderRect.height ), shadedColor ); // 左辺の枠線
        EditorGUI.DrawRect(new Rect(borderRect.xMax - line_width, borderRect.y                  , line_width        , borderRect.height ), exposedColor); // 右辺の枠線
    }
}
#endif
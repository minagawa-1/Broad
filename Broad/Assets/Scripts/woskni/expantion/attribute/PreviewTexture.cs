using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>アスペクト比率の設定種別</summary>
public enum AspectRatioType
{
    /// <summary>完全に枠内にする</summary>
    None,

    /// <summary>枠の中でアスペクト比を固定する</summary>
    FixedInFrame,

    /// <summary>枠ごとアスペクト比を固定する</summary>
    AllFixed
}

/// <summary>画像をプレビュー表示</summary>
public class PreviewTextureAttribute : PropertyAttribute
{
    public Vector2 scale;
    public AspectRatioType aspectType;

    /// <summary>画像をプレビュー表示</summary>
    /// <param name="width">プレビューする画像の横幅</param>
    /// <param name="height">プレビューする画像の縦幅</param>
    /// <param name="fixAspect">アスペクト比率の保持</param>
    public PreviewTextureAttribute(float scaleX = 1f, float scaleY = 1f, AspectRatioType aspectType = AspectRatioType.AllFixed)
    {
        scale = new Vector2(scaleX, scaleY);
        this.aspectType = aspectType;
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(PreviewTextureAttribute))]
public class PreviewTextureDrawer : PropertyDrawer
{
    const float spacing = -10f;

    readonly float field_height = EditorGUIUtility.singleLineHeight * 2f;

    static Vector2 default_size = new Vector2(100f, 100f);

    (float distance, Color color)[] line = new (float distance, Color color)[] { 
        (0f, (EditorTheme.GetThemeColor() * 0.95f).GetAlphaColor(1f)), 
        (1f, (EditorTheme.GetThemeColor() * 0.65f).GetAlphaColor(1f)), 
        (2f, (EditorTheme.GetThemeColor() * 0.6f ).GetAlphaColor(1f)), 
        (3f, (EditorTheme.GetThemeColor() * 0.5f ).GetAlphaColor(1f)), 
    };

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (!ShouldShowPreview())
        {
            string warning = $"{property.propertyType}は未対応です。\n対応: Texture, Texture2D, Sprite";

            EditorGUI.LabelField(position, warning);
            position.y += EditorGUIUtility.singleLineHeight * 0.5f * warning.HitCount("\n");
            EditorGUI.PropertyField(position, property, true);
            return;
        }

        var ptAttribute = attribute as PreviewTextureAttribute;

        EditorGUI.BeginProperty(position, label, property);

        // プレビューするテクスチャの取得
        Texture2D texture = GetPreviewTexture(property);

        // プレビューを描画
        if (texture != null)
        {
            Vector2 scale = new Vector2(ptAttribute.scale.x * default_size.x, ptAttribute.scale.y * default_size.y);

            float x = EditorGUIUtility.labelWidth / 2f;
            float y = position.yMin + GetPropertyHeight(property, label) / 2f + EditorGUIUtility.singleLineHeight + spacing;

            // アスペクト比
            ScaleMode scaleMode;
            switch (ptAttribute.aspectType)
            {
                case AspectRatioType.None:          scaleMode = ScaleMode.StretchToFill; break;
                case AspectRatioType.FixedInFrame:  scaleMode = ScaleMode.ScaleToFit;    break;
                case AspectRatioType.AllFixed:      scaleMode = ScaleMode.ScaleToFit;    break;
                default:                            scaleMode = ScaleMode.StretchToFill; break;
            }

            // AllFixedなら、枠ごとscale.yを比率に合わせる
            if (ptAttribute.aspectType == AspectRatioType.AllFixed)
            {
                if     (texture.width > texture.height) scale.y *= (float)texture.height / (float)texture.width;
                else if(texture.height > texture.width) scale.x *= (float)texture.width / (float)texture.height;
            }
                

            // プレビュー表示のサイズを設定
            Rect previewRect = new Rect(x - scale.x / 2f, y - scale.y / 2f, scale.x, scale.y);

            // 外枠の作成
            for (int i = line.Length - 1; i >= 0; --i)
                EditorGUI.DrawRect(AddScaleRect(previewRect, line[i].distance), line[i].color);

            EditorGUI.DrawTextureTransparent(previewRect, texture, scaleMode);

            
            EditorGUI.PrefixLabel(position, label);

            float inputFieldY = position.yMin + EditorGUIUtility.singleLineHeight;

            // 入力フィールドの描画
            EditorGUI.PropertyField(new Rect(position.x + EditorGUIUtility.labelWidth, inputFieldY
                                           , position.width - EditorGUIUtility.labelWidth
                                           , field_height), property, GUIContent.none);

            inputFieldY = AddLabelField(position, position.yMin, $"File path: {GetFilePath(property)}");
            inputFieldY = AddLabelField(position, inputFieldY, $"({texture.width}x{texture.height})");
                          AddLabelField(position, inputFieldY, $"File size: {GetFileSize(property).FormatSize()}B");
        }
        else
        {
            var rect = position;

            rect.yMin = rect.center.y - field_height / 2f;
            rect.height = field_height;

            EditorGUI.PropertyField(rect, property, label);
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var ptAttribute = attribute as PreviewTextureAttribute;

        bool shouldShowPreview = ShouldShowPreview() && GetPreviewTexture(property) != null;

        // 描画できる場合は描画縦幅を、できない場合は一行幅を返す
        return shouldShowPreview ? default_size.y * ptAttribute.scale.y + field_height + spacing
                                   : field_height;
    }

    /// <summary>プレビューする画像を取得</summary>
    /// <param name="property">プロパティ</param>
    Texture2D GetPreviewTexture(SerializedProperty property)
    {
        if (!property.objectReferenceValue) return null;

        // ObjectReferenceValue を Texture2D型 に変換して返す
        switch (fieldInfo.FieldType)
        {
            case System.Type textureType when textureType == typeof(Texture) || textureType == typeof(Texture[]):
            case System.Type texture2DType when texture2DType == typeof(Texture2D) || texture2DType == typeof(Texture2D[]):
                return (Texture2D)property.objectReferenceValue;
            case System.Type spriteType when spriteType == typeof(Sprite) || spriteType == typeof(Sprite[]):
                return ((Sprite)property.objectReferenceValue).texture;
            default:
                return null;
        }
    }

    /// <summary>プレビュー可能か</summary>
    /// <param name="property">プロパティ</param>
    bool ShouldShowPreview()
    {
        switch (fieldInfo.FieldType)
        {
            case System.Type textureType when textureType == typeof(Texture) || textureType == typeof(Texture[]):
            case System.Type texture2DType when texture2DType == typeof(Texture2D) || texture2DType == typeof(Texture2D[]):
            case System.Type spriteType when spriteType == typeof(Sprite) || spriteType == typeof(Sprite[]):
                return true;
            default:
                return false;
        }
    }

    /// <summary>均等に拡大した範囲を返す</summary>
    /// <param name="oldRect">拡大前の範囲</param>
    /// <param name="addWidth">拡大する距離</param>
    Rect AddScaleRect(Rect oldRect, float addWidth)
    {
        float x      = oldRect.xMin   - addWidth;
        float y      = oldRect.yMin   - addWidth;
        float width  = oldRect.width  + addWidth * 2f;
        float height = oldRect.height + addWidth * 2f;

        return new Rect(x, y, width, height);
    }

    float AddLabelField(Rect position, float currentY, string text)
    {
        position.x += EditorGUIUtility.labelWidth;
        position.y = currentY;

        EditorGUI.LabelField(position, text);

        return currentY + EditorGUIUtility.singleLineHeight;
    }

    /// <summary>ファイルサイズを取得</summary>
    long GetFileSize(SerializedProperty property) => new System.IO.FileInfo(GetFilePath(property)).Length;

    /// <summary>ファイルパスを取得</summary>
    string GetFilePath(SerializedProperty property) => AssetDatabase.GetAssetPath(property.objectReferenceValue);
}
#endif
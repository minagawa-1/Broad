using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

public class ComponentGUI : MonoBehaviour
{
    // アイコンのサイズ
    public const int icon_size = 16;

    // アイコンのアルファ値
    public const float icon_transparency = 0.6f;

    // 表示しないコンポーネント
    public static System.Type[] ignore_components = { 
              typeof(Transform)
            , typeof(RectTransform)
            , typeof(CanvasRenderer)
        };

    static Dictionary<System.Type, Texture2D> m_IconDictionary = new Dictionary<System.Type, Texture2D>();

    private void OnEnable() => m_IconDictionary = new Dictionary<System.Type, Texture2D>();

    [InitializeOnLoadMethod]
    private static void Initialize()
    {
        EditorApplication.hierarchyWindowItemOnGUI += OnGUI;
    }

    private static void OnGUI(int instanceID, Rect selectionRect)
    {
        // instanceID をオブジェクト参照に変換
        var go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
        if (go == null) return;

        // オブジェクトが所持しているコンポーネント一覧を取得
        var components = go.GetComponents<Component>();
        if (components.Length == 0) return;

        // 表示しないコンポーネントの数
        int ignoreCount = components.Count(component => Ignore(component));

        // 描画範囲の設定
        selectionRect.x = selectionRect.xMax - icon_size * (components.Length - ignoreCount);
        selectionRect.width = icon_size;

        foreach (var component in components)
        {
            // 表示しないコンポーネントの場合はcontinue
            if (Ignore(component)) continue;

            // コンポーネントからアイコンを取得
            GUI.DrawTexture(selectionRect, FindTexure(component));

            // 次の画像の表示位置に移動する
            selectionRect.x += icon_size;
        }
    }

    /// <summary>そのコンポーネントは非表示対象か</summary>
    /// <param name="component">調べるコンポーネント</param>
    static bool Ignore(Component component) => ignore_components.Contains(component.GetType());

    /// <summary>コンポーネントからアイコンを辞書参照によって取得</summary>
    /// <param name="component">コンポーネント</param>
    /// <returns>未登録の場合は登録してから返す</returns>
    static Texture2D FindTexure(Component component)
    {
        // 未定義のコンポーネントかどうかを調べる
        bool undefined = !m_IconDictionary.ContainsKey(component.GetType());

        // 定義済みだったとしても、コンポーネント内容がNullだった場合は再定義を行う
        if (!undefined) {
            if (m_IconDictionary[component.GetType()] == null) {
                undefined = true;
                m_IconDictionary.Remove(component.GetType());
            }
        }

        // 未定義のコンポーネントを定義する
        if (undefined) {
            // コンポーネントからアイコン画像を取得
            Texture2D texture = Copy(AssetPreview.GetMiniThumbnail(component));

            // そのままでは主張が激しいため、透明度を下げる
            texture = ReduceAlpha(texture, 1f - icon_transparency);

            // リストに追加
            m_IconDictionary.Add(component.GetType(), texture);
        }

        return m_IconDictionary[component.GetType()];
    }

    /// <summary>画像の透明度を下げる</summary>
    /// <param name="texture">ソース画像</param>
    /// <param name="a">下げるアルファ値（0 to 1）</param>
    /// <returns>透明度を下げた画像</returns>
    static Texture2D ReduceAlpha(Texture2D source, float a)
    {
        Color32[] colors = source.GetPixels32();

        for (int i = 0; i < colors.Length; ++i)
        {
            byte reducedAlpha = (byte)Mathf.Max(0, colors[i].a - Mathf.RoundToInt(a * 255f));

            colors[i] = colors[i].GetAlphaColor(reducedAlpha);
        }

        source.SetPixels32(colors);
        source.Apply();

        return source;
    }

    public static Texture2D Copy(Texture2D originalTexture)
    {
        if (originalTexture == null) return null;

        int width = originalTexture.width;
        int height = originalTexture.height;

        // 一時的なRenderTextureを作成
        RenderTexture renderTexture = new RenderTexture(width, height, 0);
        Graphics.Blit(originalTexture, renderTexture);

        // 新しいTexture2Dを作成
        Texture2D newTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);

        // レンダリングテクスチャから新しいTexture2Dにデータを読み込む
        RenderTexture.active = renderTexture;
        newTexture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        newTexture.Apply();

        // 一時的なRenderTextureを解放
        RenderTexture.active = null;
        renderTexture.Release();

        return newTexture;
    }
}
#endif
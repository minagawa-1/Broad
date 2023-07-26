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

    // 表示しないコンポーネント
    public static System.Type[] ignore_components = { 
              typeof(Transform)
            , typeof(RectTransform)
            , typeof(CanvasRenderer)
        };

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
            if (Ignore(component)) continue;

            // コンポーネントからアイコン画像を取得
            var texture2D = AssetPreview.GetMiniThumbnail(component);

            GUI.DrawTexture(selectionRect, texture2D);
            selectionRect.x += icon_size;
        }
    }

    /// <summary>そのコンポーネントは非表示対象か</summary>
    /// <param name="component">調べるコンポーネント</param>
    static bool Ignore(Component component) => ignore_components.Contains(component.GetType());
}
#endif
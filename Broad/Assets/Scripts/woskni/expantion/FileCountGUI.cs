using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

public class FileCountGUI : MonoBehaviour
{
    // 最小幅
    public const int min_width = 8;

    // 削除するファイルパスの位置
    private const string removed_path = "Assets";

    [InitializeOnLoadMethod]
    private static void Initialize()
    {
        EditorApplication.projectWindowItemOnGUI += OnGUI;
    }

    private static void OnGUI(string guid, Rect selectionRect)
    {
        if (string.IsNullOrEmpty(guid)) return;

        // Assetsフォルダの位置
        int    startIndex = Application.dataPath.LastIndexOf(removed_path);
        string dir        = Application.dataPath.Remove(startIndex, removed_path.Length);
        string path       = dir + AssetDatabase.GUIDToAssetPath(guid);

        // パスが見つからない場合はreturn
        if (!Directory.Exists(path)) return;

        // ファイル数とフォルダ数を求める
        int fileCount = Directory.GetFiles(path).Where(c => !c.EndsWith(".meta")).Count();
        int dirCount = Directory.GetDirectories(path).Length;

        // ファイル数が0ならreturn
        if (fileCount + dirCount == 0) return;

        string text = (fileCount + dirCount).ToString();

        float width = Mathf.Max(min_width, EditorStyles.label.CalcSize(new GUIContent(text)).x) + GUI.skin.label.fontSize * 2;

        Rect pos = selectionRect;
        pos.x = pos.xMax - width;
        pos.width = width - 15;

        float centerY = pos.center.y;

        pos.yMin = centerY - GUI.skin.label.fontSize / 2;
        pos.yMax = centerY + GUI.skin.label.fontSize / 2;


        // 背景
        {
            // 円の半径
            var radius = pos.height;

            // 背景色
            Color back = EditorTheme.GetIconColor().GetAlphaColor(0.5f);

            // 描画
            GUI.DrawTexture(pos, EditorGUIUtility.whiteTexture, ScaleMode.StretchToFill, true, 0, back, 0f, radius);
        }

        // ファイル数テキスト
        {
            GUIStyle gui = GUIStyle.none;

            // フォントサイズ（標準サイズ）
            gui.fontSize = GUI.skin.label.fontSize;

            // 文字色
            gui.normal.textColor = EditorTheme.GetThemeColor();

            // 右揃え
            gui.alignment = TextAnchor.MiddleRight;

            // テキストを左にずらす
            pos.xMin -= gui.fontSize / 2;
            pos.xMax -= gui.fontSize / 2;

            // 描画
            GUI.Label(pos, text, gui);
        }
    }
}
#endif
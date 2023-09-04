using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace woskni
{
    [System.Serializable]
    public struct RangeInt
    {
        /// <summary>最小値</summary>
        public int min;

        /// <summary>最大値</summary>
        public int max;

        /// <summary>コンストラクタ</summary>
        /// <param name="min">最小値</param>
        /// <param name="max">最大値</param>
        public RangeInt(int min, int max) { this.max = Mathf.Max(max, min); this.min = Mathf.Min(min, max); }

        /// <summary>範囲内検知</summary>
        /// <param name="value">整数型の検索値</param>
        /// <returns>引数値が範囲内に含まれているか</returns>
        public bool IsIn(int value) => min <= value && value <= max;

        /// <summary>範囲内の収束値取得</summary>
        /// <param name="value">範囲内に収束させる数値</param>
        /// <returns>収束された数値</returns>
        public int Clamp(int value) => Mathf.Max(min, Mathf.Min(value, max));

        /// <summary>範囲外の発散値取得</summary>
        /// <param name="value">範囲外に発散させる数値</param>
        /// <returns>発散された数値</returns>
        public int ClampOut(int value) => !IsIn(value) ? value : (value - min >= (max - min) / 2f ? max : min);

        /// <summary>範囲拡縮した値の取得</summary>
        /// <param name="beforeValue">旧範囲の値</param>
        /// <param name="beforeRange">新しい範囲</param>
        /// <returns>拡縮した範囲の値</returns>
        public int Lerp(int beforeValue, RangeInt newRange) => Lerp(beforeValue, newRange.min, newRange.max);

        /// <summary>範囲拡縮した値の取得</summary>
        /// <param name="beforeValue">旧範囲の値</param>
        /// <param name="min">新しい範囲の最小値</param>
        /// <param name="max">新しい範囲の最大値</param>
        /// <returns>拡縮した範囲の値</returns>
        public int Lerp(int beforeValue, int min, int max) => (max - min) * ((beforeValue - this.min) / (this.max - this.min)) + this.min;

        /// <summary>範囲内の収束値取得(min, maxからの差分を残す)</summary>
        /// <param name="value">範囲内に収束させる数値</param>
        /// <returns>収束された数値</returns>
        public int Repeat(int value) => IsIn(value) ? value : Repeat((value > max ? value - (max - min) : value + (max - min)));

        /// <summary>範囲内のランダムな値取得</summary>
        /// <returns>乱数値</returns>
        public int Random() => UnityEngine.Random.Range(min, max + 1);

        /// <summary>デバッグログ</summary>
        public void DebugLog() => Debug.Log("min: " + min.ToString() + ", max: " + max.ToString());

        public string ToString(string format = "F") => $"({min.ToString(format)} ～ {max.ToString(format)})";
    }
}
#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(woskni.RangeInt))]
public class RangeIntDrawer : PropertyDrawer
{
    Rect                wholeRect;  // 全体の幅
    float               partialSum; // 参照中、今までのプロパティの横幅の合計
    SerializedProperty  property;   // プロパティ

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // 幅の設定
        EditorGUIUtility.labelWidth = 80f + EditorGUI.indentLevel * 20f;

        // プロパティ表示部分の作成
        label = EditorGUI.BeginProperty(position, label, property);
        {
            Initialize(position, property, label);

            int lastIndentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            DivideField(0.3f, "min", "min", 32f);
            DivideField(0.3f, "max", "max", 32f);

            EditorGUI.indentLevel = lastIndentLevel;
        }
        EditorGUI.EndProperty();
    }

    void Initialize(Rect position, SerializedProperty property, GUIContent label)
    {
        partialSum      = 40f;
        this.property   = property;
        wholeRect       = EditorGUI.PrefixLabel(position, label);
    }

    /// <summary>幅を割合で指定してプロパティを１行で表示</summary>
    /// <param name="widthRate">入力プロパティの幅(0 to 1)</param>
    /// <param name="propertyName">表示させるプロパティ名</param>
    /// <param name="label">ラベル名</param>
    /// <param name="labelWidth">ラベル幅</param>
    void DivideField(float widthRate, string propertyName, string label = "", float labelWidth = 0)
    {
        // 引数が規定に従っているかの監査
        Debug.Assert(0f < widthRate && widthRate <= 1f);
        Debug.Assert(!string.IsNullOrEmpty(propertyName));
        Debug.Assert(label != null);
        Debug.Assert(labelWidth >= 0f);

        float width = wholeRect.width * widthRate;
        Rect rect = new Rect(wholeRect.x + partialSum + 80f, wholeRect.y, width - 20f, wholeRect.height);
        partialSum += width;

        EditorGUIUtility.labelWidth = Mathf.Clamp(labelWidth, 0, rect.width - 20f);
        var item = property.FindPropertyRelative(propertyName);

        if (item != null) EditorGUI.PropertyField(rect, item, new GUIContent(label));
        else Debug.LogWarningFormat("プロパティが見つかりませんでした:  '{0}' in '{1}'", propertyName, this.GetType());
    }
}
#endif
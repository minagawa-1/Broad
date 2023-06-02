using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace woskni
{
    [System.Serializable]
    public struct Range : IEquatable<Range>, IFormattable
    {
        /// <summary>最小値</summary>
        public float min;

        /// <summary>最大値</summary>
        public float max;

        /// <summary>コンストラクタ</summary>
        /// <param name="min">最小値</param>
        /// <param name="max">最大値</param>
        public Range(float min, float max) { this.max = max; this.min = min; }

        /// <summary>範囲内検知</summary>
        /// <param name="value">浮動小数点型の検索値</param>
        /// <returns>引数値が範囲内に含まれているか</returns>
        public bool IsIn(float value) => min <= value && value <= max;

        /// <summary>範囲内の収束値取得</summary>
        /// <param name="value">範囲内に収束させる数値</param>
        /// <returns>収束された数値</returns>
        public float GetIn(float value) => Mathf.Max(min, Mathf.Min(value, max));

        /// <summary>範囲外の発散値取得</summary>
        /// <param name="value">範囲外に発散させる数値</param>
        /// <returns>発散された数値</returns>
        public float GetOut(float value) => !IsIn(value) ? value : ( value - min >= (max - min) / 2f ? max : min );

        /// <summary>範囲拡縮した値の取得</summary>
        /// <param name="beforeValue">旧範囲の値</param>
        /// <param name="beforeRange">旧範囲</param>
        /// <returns>拡縮した範囲の値</returns>
        public float GetCompress(float beforeValue, Range beforeRange) => GetCompress(beforeValue, beforeRange.min, beforeRange.max);

        /// <summary>範囲拡縮した値の取得</summary>
        /// <param name="beforeValue">旧範囲の値</param>
        /// <param name="min">旧範囲の最小値</param>
        /// <param name="max">旧範囲の最大値</param>
        /// <returns>拡縮した範囲の値</returns>
        public float GetCompress(float beforeValue, float min, float max) => (this.max - this.min) * ((beforeValue - min) / (max - min)) + this.min;

        /// <summary>範囲内の収束値取得(min, maxからの差分を残す)</summary>
        /// <param name="value">範囲内に収束させる数値</param>
        /// <returns>収束された数値</returns>
        public float GetAround(float value) => IsIn(value) ? value : GetAround((value > max ? value - (max - min) : value + (max - min)));

        /// <summary>範囲内のランダムな値取得</summary>
        /// <returns>乱数値</returns>
        public float Random() => UnityEngine.Random.Range(min, max);

        /// <summary>デバッグログ</summary>
        public void DebugLog() => Debug.Log("min: " + min.ToString() + ", max: " + max.ToString());

        bool IEquatable<Range>.Equals(Range other) { return Equals(this, other); }
        public string ToString(string format, IFormatProvider formatProvider) { return min.ToString(format, formatProvider); }
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(woskni.Range))]
public class RangeDrawer : PropertyDrawer
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

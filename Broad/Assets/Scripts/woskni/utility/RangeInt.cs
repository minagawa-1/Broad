using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace woskni
{
    [System.Serializable]
    public struct RangeInt : IEquatable<RangeInt>, IFormattable
    {
        /// <summary>�ŏ��l</summary>
        public int min;

        /// <summary>�ő�l</summary>
        public int max;

        /// <summary>�R���X�g���N�^</summary>
        /// <param name="min">�ŏ��l</param>
        /// <param name="max">�ő�l</param>
        public RangeInt(int min, int max) { this.max = Mathf.Max(max, min); this.min = Mathf.Min(min, max); }

        /// <summary>�͈͓����m</summary>
        /// <param name="value">�����^�̌����l</param>
        /// <returns>�����l���͈͓��Ɋ܂܂�Ă��邩</returns>
        public bool IsIn(int value) => min <= value && value <= max;

        /// <summary>�͈͓��̎����l�擾</summary>
        /// <param name="value">�͈͓��Ɏ��������鐔�l</param>
        /// <returns>�������ꂽ���l</returns>
        public int GetIn(int value) => Mathf.Max(min, Mathf.Min(value, max));

        /// <summary>�͈͊O�̔��U�l�擾</summary>
        /// <param name="value">�͈͊O�ɔ��U�����鐔�l</param>
        /// <returns>���U���ꂽ���l</returns>
        public int GetOut(int value) => IsIn(value) ? (value - min >= (max - min) / 2f ? max : min) : value;

        /// <summary>�͈͊g�k�����l�̎擾</summary>
        /// <param name="beforeValue">���͈͂̒l</param>
        /// <param name="beforeRange">���͈�</param>
        /// <returns>�g�k�����͈͂̒l</returns>
        public int GetCompress(int beforeValue, RangeInt beforeRange) => (int)((float)this.max * ((float)beforeValue / (float)(beforeRange.max - beforeRange.min)));

        /// <summary>�͈͊g�k�����l�̎擾</summary>
        /// <param name="beforeValue">���͈͂̒l</param>
        /// <param name="min">���͈͂̍ŏ��l</param>
        /// <param name="max">���͈͂̍ő�l</param>
        public int GetCompress(int beforeValue, int min, int max) => (int)((float)this.max * ((float)beforeValue / (float)(max - min)));

        /// <summary>�͈͓��̎����l�擾(min, max����̍������c��)</summary>
        /// <param name="value">�͈͓��Ɏ��������鐔�l</param>
        /// <returns>�������ꂽ���l</returns>
        public int GetAround(int value) => IsIn(value) ? value : GetAround((value > max ? value - (max - min) : value + (max - min)));

        /// <summary>�͈͓��̃����_���Ȓl�擾</summary>
        /// <returns>�����l</returns>
        public float Random() => UnityEngine.Random.Range(min, max);

        /// <summary>�f�o�b�O���O</summary>
        public void DebugLog() => Debug.Log("min: " + min.ToString() + ", max: " + max.ToString());

        bool IEquatable<RangeInt>.Equals(RangeInt other) { return Equals(this, other); }
        public string ToString(string format, IFormatProvider formatProvider) { return min.ToString(format, formatProvider); }
    }
}
#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(woskni.RangeInt))]
public class RangeIntDrawer : PropertyDrawer
{
    Rect                wholeRect;  // �S�̂̕�
    float               partialSum; // �Q�ƒ��A���܂ł̃v���p�e�B�̉����̍��v
    SerializedProperty  property;   // �v���p�e�B

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // ���̐ݒ�
        EditorGUIUtility.labelWidth = 80f + EditorGUI.indentLevel * 20f;

        // �v���p�e�B�\�������̍쐬
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

    /// <summary>���������Ŏw�肵�ăv���p�e�B���P�s�ŕ\��</summary>
    /// <param name="widthRate">���̓v���p�e�B�̕�(0 to 1)</param>
    /// <param name="propertyName">�\��������v���p�e�B��</param>
    /// <param name="label">���x����</param>
    /// <param name="labelWidth">���x����</param>
    void DivideField(float widthRate, string propertyName, string label = "", float labelWidth = 0)
    {
        // �������K��ɏ]���Ă��邩�̊č�
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
        else Debug.LogWarningFormat("�v���p�e�B��������܂���ł���:  '{0}' in '{1}'", propertyName, this.GetType());
    }
}
#endif
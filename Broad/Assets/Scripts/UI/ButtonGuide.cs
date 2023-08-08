using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem.LowLevel;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

[AddComponentMenu("UI/ButtonGuide", 35)]
[RequireComponent(typeof(RectTransform))]
public class ButtonGuide : MonoBehaviour
{
    protected internal class ButtonGuideItem : MonoBehaviour
    {
        [SerializeField] Text  m_Text;
        [SerializeField] Image m_Icon;
        [SerializeField] RectTransform m_RectTransform;

        public Text  text { get { return m_Text; } set { m_Text = value; } }
        public Image icon { get { return m_Icon; } set { m_Icon = value; } }
        public RectTransform rectTransform { get { return m_RectTransform; } set { m_RectTransform = value; } }
    }

    [Serializable]
    public class GuideData
    {
        [SerializeField] GamepadButton m_Key;
        [SerializeField] string m_Text;

        public GamepadButton key  { get { return m_Key; } set { m_Key = value; } }
        public string        text { get { return m_Text; } set { m_Text = value; } }

        public GuideData()                               { key = GamepadButton.A; text = ""; }
        public GuideData(GamepadButton key)              { this.key = key;        text = ""; }
        public GuideData(string text)                    { key = GamepadButton.A; this.text = text; }
        public GuideData(GamepadButton key, string text) { this.key = key;        this.text = text; }
        public GuideData(string text, GamepadButton key) { this.key = key;        this.text = text; }
    }

    [Serializable]
    public class GuideDataList
    {
        [SerializeField] List<GuideData> m_Guides;

        public List<GuideData> guides { get { return m_Guides; } set { m_Guides = value; } }

        public GuideDataList()
        {
            guides = new List<GuideData>();
        }
    }

    [SerializeField] int m_Size;

    public int size
    {
        get { return m_Size; }
        set { m_Size = Mathf.Max(0, value); OnChangedSize(); }
    }

    [Space]

    [SerializeField] GuideDataList m_Guides = new GuideDataList();

    public List<GuideData> guides
    {
        get { return m_Guides.guides; }
        set { m_Guides.guides = value; }
    }



    void OnChangedSize()
    {

    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ButtonGuide), true)]
[CanEditMultipleObjects]
public class ButtonGuideEditor : Editor
{
    SerializedProperty m_Size;
    SerializedProperty m_Guides;

    protected void OnEnable()
    {
        m_Size   = serializedObject.FindProperty("m_Size");
        m_Guides = serializedObject.FindProperty("m_Guides");
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.Space();

        serializedObject.Update();
        EditorGUILayout.PropertyField(m_Size);
        EditorGUILayout.PropertyField(m_Guides);
        serializedObject.ApplyModifiedProperties();
    }
}

[CustomPropertyDrawer(typeof(ButtonGuide.GuideDataList), true)]
class GuideDataListDrawer : PropertyDrawer
{
    ReorderableList m_ReorderableList = null;

    void Init(SerializedProperty property)
    {
        if (m_ReorderableList != null) return;

        SerializedProperty array = property.FindPropertyRelative("m_Guides");

        m_ReorderableList = new ReorderableList(property.serializedObject, array);
        m_ReorderableList.drawElementCallback = DrawGuideData;
        m_ReorderableList.drawHeaderCallback = DrawHeader;
        //m_ReorderableList.elementHeight += 16;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Init(property);

        m_ReorderableList.DoList(position);
    }

    void DrawHeader(Rect rect)
    {
        GUI.Label(rect, "Guides");
    }

    void DrawGuideData(Rect rect, int index, bool isActive, bool isFocused)
    {
        SerializedProperty itemData = m_ReorderableList.serializedProperty.GetArrayElementAtIndex(index);
        SerializedProperty itemKey = itemData.FindPropertyRelative("m_Key");
        SerializedProperty itemText = itemData.FindPropertyRelative("m_Text");

        rect.height = EditorGUIUtility.singleLineHeight;

        float cutline = rect.width * 0.33f;

        Rect keyRect  = new Rect(rect.x, rect.y, cutline, rect.height);

        Rect iconRect = new Rect(rect.x + cutline, rect.y, EditorGUIUtility.singleLineHeight, rect.y);

        Rect textRect = new Rect(rect.x     + cutline + rect.height, rect.y
                               , rect.width - cutline - rect.height, rect.height);

        // 描画処理（横一列にプロパティを並べる））
        EditorGUILayout.BeginHorizontal();

        EditorGUI.PropertyField(keyRect , itemKey , GUIContent.none);
        EditorGUI.DrawTextureTransparent(iconRect, GamepadButtonIconLoader.Load((GamepadButton)itemKey.enumValueIndex));
        EditorGUI.PropertyField(textRect, itemText, GUIContent.none);

        // 横一列の終了
        EditorGUILayout.EndHorizontal();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        Init(property);

        return m_ReorderableList.GetHeight();
    }
}
#endif
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AnimatedValues;
#endif

[RequireComponent(typeof(CanvasRenderer))]
[AddComponentMenu("UI/Donut Chip", 12)]
public class DonutChip : MaskableGraphic
{
    [SerializeField] Sprite m_Sprite;

    public Sprite sprite
    {
        get { return m_Sprite; }
        set
        {
            if (m_Sprite != null)
            {
                if (m_Sprite != value)
                {
                    m_SkipLayoutUpdate = m_Sprite.rect.size.Equals(value ? value.rect.size : Vector2.zero);
                    m_SkipMaterialUpdate = m_Sprite.texture == (value ? value.texture : null);
                    m_Sprite = value;

                    SetAllDirty();
                }
            }
            else if (value != null)
            {
                m_SkipLayoutUpdate = value.rect.size == Vector2.zero;
                m_SkipMaterialUpdate = value.texture == null;
                m_Sprite = value;

                SetAllDirty();
            }
        }
    }

    [Chapter("�h�[�i�c���")]
    [Min(0f)] public float  thickness = 20f;  // �h�[�i�c�̑���
    [Min(3)]  public int    resolution = 100;    // �𑜓x

    [Header("�h�[�i�c�͈̔�")]
    [Range(0f, 1f)] public float  areaMin = 0f;
    [Range(0f, 1f)] public float  areaMax = 1f;

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        float minArea = Mathf.Min(areaMin, areaMax);
        areaMin = minArea;
        areaMax = Mathf.Max(areaMin, areaMax);

        vh.Clear();

        // ���S���W�̌v�Z
        Vector3 center = rectTransform.rect.center;

        // �Z���a
        float radius = rectTransform.rect.height * 0.5f;

        int startAngle = (int)(areaMin * (float)resolution);
        int endAngle = (int)(areaMax * (float)resolution);

        int vertexCount = 0;

        // Color�ɔ�ׂ�Color32�͌^�ϊ������܂Ȃ����ߍ���
        Color32 color32 = color;

        var uv = (sprite != null) ? UnityEngine.Sprites.DataUtility.GetOuterUV(sprite) : Vector4.zero;

        // ���_�̍��W���v�Z���Ē��_���X�g�ɒǉ�
        for (int i = startAngle; i <= endAngle; i++)
        {
            float angle = -(Mathf.PI * 2f * i) / resolution;

            Vector3 rot = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

            // ���~�̒��_���W
            Vector3 pos = rot * radius;

            // ���_�����ݒ�
            UIVertex innerVertex = UIVertex.simpleVert;
            UIVertex outerVertex = UIVertex.simpleVert;

            // ���_���W
            innerVertex.position = center + pos.normalized * (pos.magnitude - thickness);
            outerVertex.position = center + pos.normalized * (pos.magnitude + thickness);

            // ���_�J���[
            innerVertex.color = color32;
            outerVertex.color = color32;

            // UV���W
            innerVertex.uv0 = new Vector2((float)(i - startAngle) / (float)(endAngle - startAngle), 0f);
            outerVertex.uv0 = new Vector2((float)(i - startAngle) / (float)(endAngle - startAngle), 1f);

            vh.AddVert(innerVertex);
            vh.AddVert(outerVertex);

            // ���~�E�O�~�𓯎��ݒ肵������ �{�Q
            vertexCount += 2;
        }

        // �O�p�`���\�����钸�_�C���f�b�N�X��ǉ�
        for (int i = 0; i < vertexCount - 2; i += 2)
        {
            int innerIndex = i;
            int outerIndex = i + 1;

            vh.AddTriangle(innerIndex, outerIndex, outerIndex + 2);
            vh.AddTriangle(innerIndex, outerIndex + 2, innerIndex + 2);
        }
    }

    public void DoChangeArea(float min, float max, float time, Ease ease)
    {
        DOTween.To(() => areaMin, x => areaMin = x, min, time).SetEase(ease);
        DOTween.To(() => areaMax, x => areaMax = x, max, time).SetEase(ease).OnUpdate(SetVerticesDirty);
    }

    protected override void UpdateMaterial()
    {
        base.UpdateMaterial();

        if (sprite == null) { canvasRenderer.SetAlphaTexture(null); return; }

        Texture2D alphaTex = sprite.associatedAlphaSplitTexture;

        if (alphaTex != null) canvasRenderer.SetAlphaTexture(alphaTex);
    }
}


#if UNITY_EDITOR

[CustomEditor(typeof(DonutChip), true)]
[CanEditMultipleObjects]
public class DonutChipEditor : UnityEditor.UI.GraphicEditor
{
    SerializedProperty m_Sprite;

    SerializedProperty m_Thickness;
    SerializedProperty m_Resolution;

    [Header("�h�[�i�c�͈̔�")]
    SerializedProperty m_AreaMin;
    SerializedProperty m_AreaMax;

    protected override void OnEnable()
    {
        base.OnEnable();

        m_Sprite     = serializedObject.FindProperty("m_Sprite");
        m_Thickness  = serializedObject.FindProperty("thickness");
        m_Resolution = serializedObject.FindProperty("resolution");
        m_AreaMin    = serializedObject.FindProperty("areaMin");
        m_AreaMax    = serializedObject.FindProperty("areaMax");
    }


    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(m_Sprite, EditorGUIUtility.TrTextContent("Source Image"));

        AppearanceControlsGUI();
        RaycastControlsGUI();
        MaskableControlsGUI();

        DonutControlGUI();

        serializedObject.ApplyModifiedProperties();
    }

    void DonutControlGUI()
    {
        EditorGUILayout.PropertyField(m_Thickness, EditorGUIUtility.TrTextContent("Thickness"));
        EditorGUILayout.PropertyField(m_Resolution, EditorGUIUtility.TrTextContent("Resolution"));

        EditorGUILayout.PropertyField(m_AreaMin, EditorGUIUtility.TrTextContent("Area Min"));
        EditorGUILayout.PropertyField(m_AreaMax, EditorGUIUtility.TrTextContent("Area Max"));
    }
}
#endif
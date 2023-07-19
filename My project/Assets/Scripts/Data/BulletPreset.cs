using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class BulletPreset
{
    public string           explanation;

    public BulletType       type;
    public BulletName       bullet;
    public Gradient         colorGradation;
    public int              processIndex;
    public woskni.Range     injectionInterval;
    public float            homingPower;
    public AnimationCurve   moveCurve;
    public float            shakeAngle;
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(BulletPreset))]
public class BulletStatusDrawer : PropertyDrawer
{
    private class PropertyData
    {
        // Bullet��explanation
        public SerializedProperty explanationProperty;
        // Bullet��type
        public SerializedProperty typeProperty;
        // Bullet��data
        public SerializedProperty bulletProperty;
        // Bullet��colorGradation
        public SerializedProperty colorGradationProperty;
        // Bullet��process
        public SerializedProperty processIndexProperty;
        // �ˏo�Ԋu
        public SerializedProperty injectionIntervalProperty;
        // �^�[�Q�b�g��Ǐ]�����
        public SerializedProperty homingPowerProperty;
        // �A�j���[�V�����J�[�u
        public SerializedProperty moveCurveProperty;
        // �e�v��
        public SerializedProperty shakeAngleProperty;
    }

    private Dictionary<string, PropertyData> _propertyDataPerPropertyPath = new Dictionary<string, PropertyData>();
    private PropertyData _property;

    private float LineHeight { get { return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing; } }

    private void Init(SerializedProperty property)
    {
        if (_propertyDataPerPropertyPath.TryGetValue(property.propertyPath, out _property))
        {
            return;
        }

        _property                           = new PropertyData();
        _property.explanationProperty       = property.FindPropertyRelative("explanation");
        _property.typeProperty              = property.FindPropertyRelative("type");
        _property.bulletProperty            = property.FindPropertyRelative("bullet");
        _property.colorGradationProperty    = property.FindPropertyRelative("colorGradation");
        _property.processIndexProperty      = property.FindPropertyRelative("processIndex");
        _property.injectionIntervalProperty = property.FindPropertyRelative("injectionInterval");
        _property.homingPowerProperty       = property.FindPropertyRelative("homingPower");
        _property.moveCurveProperty         = property.FindPropertyRelative("moveCurve");
        _property.shakeAngleProperty        = property.FindPropertyRelative("shakeAngle");
        _propertyDataPerPropertyPath.Add(property.propertyPath, _property);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Init(property);
        var fieldRect = position;
        // �C���f���g���ꂽ�ʒu��Rect���~������΂��������g��
        var indentedFieldRect = EditorGUI.IndentedRect(fieldRect);
        fieldRect.height = LineHeight;

        // Prefab��������v���p�e�B�ɕύX���������ۂɑ����ɂ����肷��@�\�������邽��PropertyScope���g��
        using (new EditorGUI.PropertyScope(fieldRect, label, property))
        {
            // �v���p�e�B����\�����Đ܂��ݏ�Ԃ𓾂�
            property.isExpanded = EditorGUI.Foldout(new Rect(fieldRect), property.isExpanded, label);

            //�܂肽����ł�����I��
            if (!property.isExpanded) return;

            using (new EditorGUI.IndentLevelScope())
            {
                // Explanation��`��
                fieldRect.y += LineHeight;
                EditorGUI.PropertyField(new Rect(fieldRect), _property.explanationProperty);

                // Type��`��
                fieldRect.y += LineHeight * 1.5f;
                EditorGUI.PropertyField(new Rect(fieldRect), _property.typeProperty);

                // Data��`��
                fieldRect.y += LineHeight;
                EditorGUI.PropertyField(new Rect(fieldRect), _property.bulletProperty);

                // Color��`��
                fieldRect.y += LineHeight;
                EditorGUI.PropertyField(new Rect(fieldRect), _property.colorGradationProperty);

                // ProcessIndex��`��
                fieldRect.y += LineHeight;
                EditorGUI.PropertyField(new Rect(fieldRect), _property.processIndexProperty);

                // injectionInterval��`��
                fieldRect.y += LineHeight;
                EditorGUI.PropertyField(new Rect(fieldRect), _property.injectionIntervalProperty);

                //Homing�^�C�v�Ȃ�
                if (_property.typeProperty.enumValueIndex == (int)BulletType.Homing)
                {
                    // HomingPower��`��
                    fieldRect.y += LineHeight;
                    EditorGUI.PropertyField(new Rect(fieldRect), _property.homingPowerProperty);
                }

                //Curve�^�C�v�Ȃ�
                if (_property.typeProperty.enumValueIndex == (int)BulletType.Curve)
                {
                    // MoveCurve��`��
                    fieldRect.y += LineHeight;
                    EditorGUI.PropertyField(new Rect(fieldRect), _property.moveCurveProperty);
                }

                // BuckShot�^�C�v�Ȃ�
                if (_property.typeProperty.enumValueIndex == (int)BulletType.BuckShot)
                {
                    // shakePower��`��
                    fieldRect.y += LineHeight;
                    EditorGUI.PropertyField(new Rect(fieldRect), _property.shakeAngleProperty);
                }
            }
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        Init(property);

        // (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) x �s�� �ŕ`��̈�̍��������߂�
        // Normal�^�C�v�ȊO�Ȃ�
        if (_property.typeProperty.enumValueIndex == (int)BulletType.Homing ||
            _property.typeProperty.enumValueIndex == (int)BulletType.Curve ||
            _property.typeProperty.enumValueIndex == (int)BulletType.BuckShot)
        {
                return LineHeight * (property.isExpanded ? 8.5f : 1f);
        }
        else
        {
                return LineHeight * (property.isExpanded ? 7.5f : 1f);
        }
    }
}
#endif
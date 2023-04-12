using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public struct BulletProcess
{
    [woskni.Name("����")]public string           explanation;
    public woskni.RangeInt  wayCountRange;
    [woskni.Name("��������Ɏˏo")]public bool             isLookTarget;
    [woskni.Name("�ˏo����")]public float            direction;
    [woskni.Name("��ʊO����ˏo")]public bool             isSniping;
    [woskni.Name("�S�����ˏo")]public bool             isAllRange;
    public woskni.Range     rotateDirection;
    public woskni.Range     angleRange;
    [woskni.Name("�e��")]public float            speed;
    [woskni.Name("���Ŏ���")]public float            deadTime;
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(BulletProcess))]
public class BulletProcessDrawer : PropertyDrawer
{
    private class PropertyData
    {
        // ���
        public SerializedProperty explanationProperty;
        // ���E�F�C��邩
        public SerializedProperty wayCountRangeProperty;
        // �^�[�Q�b�g�����邩
        public SerializedProperty isLookTargetProperty;
        // ������邩
        public SerializedProperty directionProterty;
        // �_�����邩
        public SerializedProperty isSnipingProperty;
        // �S���ʂɂ��邩
        public SerializedProperty isAllRangeProperty;
        // �ˏo�p�x�̉�]�l
        public SerializedProperty rotateDirectionProperty;
        // ���ˊp�x
        public SerializedProperty angleRangeProperty;
        // �e��
        public SerializedProperty speedProperty;
        // �e�̎��S����
        public SerializedProperty deadTimeProperty;
    }

    // �L�[�l��PropertyData���������ł���悤�ɂ���
    private Dictionary<string, PropertyData> _propertyDataPerPropertyPath = new Dictionary<string, PropertyData>();
    private PropertyData _property;

    // �C���X�y�N�^��Ō��₷������ׂɊԊu�����߂�
    private float LineHeight { get { return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing; } }

    private void Init(SerializedProperty property)
    {
        // �w�肵���L�[�̒l���Ȃ��ꍇ�Areturn
        if (_propertyDataPerPropertyPath.TryGetValue(property.propertyPath, out _property))
        {
            return;
        }
        _property                           = new PropertyData();
        _property.explanationProperty       = property.FindPropertyRelative("explanation");
        _property.wayCountRangeProperty     = property.FindPropertyRelative("wayCountRange");
        _property.isLookTargetProperty      = property.FindPropertyRelative("isLookTarget");
        _property.directionProterty         = property.FindPropertyRelative("direction");
        _property.isSnipingProperty         = property.FindPropertyRelative("isSniping");
        _property.isAllRangeProperty        = property.FindPropertyRelative("isAllRange");
        _property.rotateDirectionProperty   = property.FindPropertyRelative("rotateDirection");
        _property.angleRangeProperty        = property.FindPropertyRelative("angleRange");
        _property.speedProperty             = property.FindPropertyRelative("speed");
        _property.deadTimeProperty          = property.FindPropertyRelative("deadTime");
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // ������
        Init(property);
        // �\���ʒu
        var fieldRect = position;
        // �C���f���g���ꂽ�ʒu��Rect���~������΂��������g��
        var indentedFieldRect = EditorGUI.IndentedRect(fieldRect);
        // LineHeight����
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
                // explanation��`��
                fieldRect.y += LineHeight;
                EditorGUI.PropertyField(new Rect(fieldRect), _property.explanationProperty);

                // wayCountRange��`��
                fieldRect.y += LineHeight * 1.5f;
                EditorGUI.PropertyField(new Rect(fieldRect), _property.wayCountRangeProperty);

                // isLookTarget��`��
                fieldRect.y += LineHeight;
                EditorGUI.PropertyField(new Rect(fieldRect), _property.isLookTargetProperty);

                // isLookTarget��false�Ȃ�\��
                if (!_property.isLookTargetProperty.boolValue)
                {
                    // direction��`��
                    fieldRect.y += LineHeight;
                    EditorGUI.PropertyField(new Rect(fieldRect), _property.directionProterty);

                    // rotateDirection��`��
                    fieldRect.y += LineHeight;
                    EditorGUI.PropertyField(new Rect(fieldRect), _property.rotateDirectionProperty);

                    // isSniping��`��
                    fieldRect.y += LineHeight;
                    EditorGUI.PropertyField(new Rect(fieldRect), _property.isSnipingProperty);
                }

                // isAllRange��`��
                fieldRect.y += LineHeight;
                EditorGUI.PropertyField(new Rect(fieldRect), _property.isAllRangeProperty);

                // isAllRange��false�Ȃ�\��
                if (!_property.isAllRangeProperty.boolValue)
                {
                    // angleRange��`��
                    fieldRect.y += LineHeight;
                    EditorGUI.PropertyField(new Rect(fieldRect), _property.angleRangeProperty);
                }

                // speed��`��
                fieldRect.y += LineHeight;
                EditorGUI.PropertyField(new Rect(fieldRect), _property.speedProperty);

                // deadTime��`��
                fieldRect.y += LineHeight;
                EditorGUI.PropertyField(new Rect(fieldRect), _property.deadTimeProperty);
            }
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        Init(property);

        // (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) x �s�� �ŕ`��̈�̍��������߂�
        if (_property.isLookTargetProperty.boolValue)
            return LineHeight * (property.isExpanded ? 11.5f : 1);
        else
            return LineHeight * (property.isExpanded ? 13.5f : 1);
    }
}
#endif

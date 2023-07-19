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
        // Bulletのexplanation
        public SerializedProperty explanationProperty;
        // Bulletのtype
        public SerializedProperty typeProperty;
        // Bulletのdata
        public SerializedProperty bulletProperty;
        // BulletのcolorGradation
        public SerializedProperty colorGradationProperty;
        // Bulletのprocess
        public SerializedProperty processIndexProperty;
        // 射出間隔
        public SerializedProperty injectionIntervalProperty;
        // ターゲットを追従する力
        public SerializedProperty homingPowerProperty;
        // アニメーションカーブ
        public SerializedProperty moveCurveProperty;
        // 弾プレ
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
        // インデントされた位置のRectが欲しければこっちを使う
        var indentedFieldRect = EditorGUI.IndentedRect(fieldRect);
        fieldRect.height = LineHeight;

        // Prefab化した後プロパティに変更を加えた際に太字にしたりする機能を加えるためPropertyScopeを使う
        using (new EditorGUI.PropertyScope(fieldRect, label, property))
        {
            // プロパティ名を表示して折り畳み状態を得る
            property.isExpanded = EditorGUI.Foldout(new Rect(fieldRect), property.isExpanded, label);

            //折りたたんでいたら終了
            if (!property.isExpanded) return;

            using (new EditorGUI.IndentLevelScope())
            {
                // Explanationを描画
                fieldRect.y += LineHeight;
                EditorGUI.PropertyField(new Rect(fieldRect), _property.explanationProperty);

                // Typeを描画
                fieldRect.y += LineHeight * 1.5f;
                EditorGUI.PropertyField(new Rect(fieldRect), _property.typeProperty);

                // Dataを描画
                fieldRect.y += LineHeight;
                EditorGUI.PropertyField(new Rect(fieldRect), _property.bulletProperty);

                // Colorを描画
                fieldRect.y += LineHeight;
                EditorGUI.PropertyField(new Rect(fieldRect), _property.colorGradationProperty);

                // ProcessIndexを描画
                fieldRect.y += LineHeight;
                EditorGUI.PropertyField(new Rect(fieldRect), _property.processIndexProperty);

                // injectionIntervalを描画
                fieldRect.y += LineHeight;
                EditorGUI.PropertyField(new Rect(fieldRect), _property.injectionIntervalProperty);

                //Homingタイプなら
                if (_property.typeProperty.enumValueIndex == (int)BulletType.Homing)
                {
                    // HomingPowerを描画
                    fieldRect.y += LineHeight;
                    EditorGUI.PropertyField(new Rect(fieldRect), _property.homingPowerProperty);
                }

                //Curveタイプなら
                if (_property.typeProperty.enumValueIndex == (int)BulletType.Curve)
                {
                    // MoveCurveを描画
                    fieldRect.y += LineHeight;
                    EditorGUI.PropertyField(new Rect(fieldRect), _property.moveCurveProperty);
                }

                // BuckShotタイプなら
                if (_property.typeProperty.enumValueIndex == (int)BulletType.BuckShot)
                {
                    // shakePowerを描画
                    fieldRect.y += LineHeight;
                    EditorGUI.PropertyField(new Rect(fieldRect), _property.shakeAngleProperty);
                }
            }
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        Init(property);

        // (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) x 行数 で描画領域の高さを求める
        // Normalタイプ以外なら
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
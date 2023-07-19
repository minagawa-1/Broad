using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public struct BulletProcess
{
    [woskni.Name("説明")]public string           explanation;
    public woskni.RangeInt  wayCountRange;
    [woskni.Name("相手方向に射出")]public bool             isLookTarget;
    [woskni.Name("射出方向")]public float            direction;
    [woskni.Name("画面外から射出")]public bool             isSniping;
    [woskni.Name("全方向射出")]public bool             isAllRange;
    public woskni.Range     rotateDirection;
    public woskni.Range     angleRange;
    [woskni.Name("弾速")]public float            speed;
    [woskni.Name("消滅時間")]public float            deadTime;
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(BulletProcess))]
public class BulletProcessDrawer : PropertyDrawer
{
    private class PropertyData
    {
        // 解説
        public SerializedProperty explanationProperty;
        // 何ウェイ作るか
        public SerializedProperty wayCountRangeProperty;
        // ターゲットを見るか
        public SerializedProperty isLookTargetProperty;
        // 上を見るか
        public SerializedProperty directionProterty;
        // 狙撃するか
        public SerializedProperty isSnipingProperty;
        // 全方位にするか
        public SerializedProperty isAllRangeProperty;
        // 射出角度の回転値
        public SerializedProperty rotateDirectionProperty;
        // 発射角度
        public SerializedProperty angleRangeProperty;
        // 弾速
        public SerializedProperty speedProperty;
        // 弾の死亡時間
        public SerializedProperty deadTimeProperty;
    }

    // キー値でPropertyData内を検索できるようにする
    private Dictionary<string, PropertyData> _propertyDataPerPropertyPath = new Dictionary<string, PropertyData>();
    private PropertyData _property;

    // インスペクタ上で見やすくする為に間隔を決める
    private float LineHeight { get { return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing; } }

    private void Init(SerializedProperty property)
    {
        // 指定したキーの値がない場合、return
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
        // 初期化
        Init(property);
        // 表示位置
        var fieldRect = position;
        // インデントされた位置のRectが欲しければこっちを使う
        var indentedFieldRect = EditorGUI.IndentedRect(fieldRect);
        // LineHeightを代入
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
                // explanationを描画
                fieldRect.y += LineHeight;
                EditorGUI.PropertyField(new Rect(fieldRect), _property.explanationProperty);

                // wayCountRangeを描画
                fieldRect.y += LineHeight * 1.5f;
                EditorGUI.PropertyField(new Rect(fieldRect), _property.wayCountRangeProperty);

                // isLookTargetを描画
                fieldRect.y += LineHeight;
                EditorGUI.PropertyField(new Rect(fieldRect), _property.isLookTargetProperty);

                // isLookTargetがfalseなら表示
                if (!_property.isLookTargetProperty.boolValue)
                {
                    // directionを描画
                    fieldRect.y += LineHeight;
                    EditorGUI.PropertyField(new Rect(fieldRect), _property.directionProterty);

                    // rotateDirectionを描画
                    fieldRect.y += LineHeight;
                    EditorGUI.PropertyField(new Rect(fieldRect), _property.rotateDirectionProperty);

                    // isSnipingを描画
                    fieldRect.y += LineHeight;
                    EditorGUI.PropertyField(new Rect(fieldRect), _property.isSnipingProperty);
                }

                // isAllRangeを描画
                fieldRect.y += LineHeight;
                EditorGUI.PropertyField(new Rect(fieldRect), _property.isAllRangeProperty);

                // isAllRangeがfalseなら表示
                if (!_property.isAllRangeProperty.boolValue)
                {
                    // angleRangeを描画
                    fieldRect.y += LineHeight;
                    EditorGUI.PropertyField(new Rect(fieldRect), _property.angleRangeProperty);
                }

                // speedを描画
                fieldRect.y += LineHeight;
                EditorGUI.PropertyField(new Rect(fieldRect), _property.speedProperty);

                // deadTimeを描画
                fieldRect.y += LineHeight;
                EditorGUI.PropertyField(new Rect(fieldRect), _property.deadTimeProperty);
            }
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        Init(property);

        // (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) x 行数 で描画領域の高さを求める
        if (_property.isLookTargetProperty.boolValue)
            return LineHeight * (property.isExpanded ? 11.5f : 1);
        else
            return LineHeight * (property.isExpanded ? 13.5f : 1);
    }
}
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "Scriptable/Create EnemyData")]
public class EnemyData : ScriptableObject
{
    //エネミー情報構造体
    [System.Serializable]
    public struct Enemy
    {
        public string       name;               // エネミー名
        public Sprite       sprite;             // エネミー画像
        public List<int>    weaponIndex;        // 装備したい武器番号
        public int          HP;                 // 体力
        [woskni.Name("基礎報酬")]
        public int      basePoint;          // 基礎報酬

        [Header("行動情報")]
        [woskni.Name("無敵時間")] public float          invincibleTime;
        [woskni.Name("移動距離")] public float          moveLength;
        public woskni.Range                             moveIntervalRange;
        [woskni.Name("移動時間")] public float          moveTime;

        /// [追加願い] どんな弾射出を行うのかのスクリプト・情報



    }

    [SerializeField] List<Enemy> enemyList; //エネミーリスト

    /// <summary>エネミーリスト取得</summary>
    /// <returns>エネミーリスト</returns>
    public List<Enemy> GetEnemyList() { return enemyList; }

    /// <summary>番号を指定してエネミー情報を取得</summary>
    /// <param name="index">要素番号</param>
    /// <returns>指定エネミー情報</returns>
    public Enemy GetEnemyAt(int index) { return enemyList[index]; }
}

//#if UNITY_EDITOR
//[CustomPropertyDrawer(typeof(EnemyData.Enemy))]
//public class EnemyDataDrawer : PropertyDrawer
//{
//    Rect wholeRect;  // 全体の幅
//    float partialSum; // 参照中、今までのプロパティの横幅の合計
//    SerializedProperty property;   // プロパティ

//    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//    {
//        // 幅の設定
//        EditorGUIUtility.labelWidth = 60f + EditorGUI.indentLevel * 20f;

//        // プロパティ表示部分の作成
//        label = EditorGUI.BeginProperty(position, label, property);
//        {
//            Initialize(position, property, label);

//            int lastIndentLevel = EditorGUI.indentLevel;
//            EditorGUI.indentLevel = 0;

//            DivideField(0.4f, "name"    , "名前", 48f);
//            DivideField(0.0f, "image"   , "画像", 48f);

//            EditorGUI.indentLevel = lastIndentLevel;
//        }
//        EditorGUI.EndProperty();
//    }

//    void Initialize(Rect position, SerializedProperty property, GUIContent label)
//    {
//        partialSum = 40f;
//        this.property = property;
//        wholeRect = EditorGUI.PrefixLabel(position, label);
//    }

//    /// <summary>幅を割合で指定してプロパティを１行で表示</summary>
//    /// <param name="widthRate">入力プロパティの幅(0 to 1)</param>
//    /// <param name="propertyName">表示させるプロパティ名</param>
//    /// <param name="label">ラベル名</param>
//    /// <param name="labelWidth">ラベル幅</param>
//    void DivideField(float widthRate, string propertyName, string label = "", float labelWidth = 0)
//    {
//        // 引数が規定に従っているかの監査
//        Debug.Assert(0f < widthRate && widthRate <= 1f);
//        Debug.Assert(!string.IsNullOrEmpty(propertyName));
//        Debug.Assert(label != null);
//        Debug.Assert(labelWidth >= 0f);

//        float width = wholeRect.width * widthRate;
//        Rect rect = new Rect(wholeRect.x + partialSum + 80f, wholeRect.y, width - 20f, wholeRect.height);
//        partialSum += width;

//        EditorGUIUtility.labelWidth = Mathf.Clamp(labelWidth, 0, rect.width - 20f);
//        var item = property.FindPropertyRelative(propertyName);

//        if (item != null) EditorGUI.PropertyField(rect, item, new GUIContent(label));
//        else Debug.LogWarningFormat("プロパティが見つかりませんでした:  '{0}' in '{1}'", propertyName, this.GetType());
//    }
//}
//#endif
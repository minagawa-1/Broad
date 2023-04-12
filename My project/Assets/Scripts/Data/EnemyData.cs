using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "Scriptable/Create EnemyData")]
public class EnemyData : ScriptableObject
{
    //�G�l�~�[���\����
    [System.Serializable]
    public struct Enemy
    {
        public string       name;               // �G�l�~�[��
        public Sprite       sprite;             // �G�l�~�[�摜
        public List<int>    weaponIndex;        // ��������������ԍ�
        public int          HP;                 // �̗�
        [woskni.Name("��b��V")]
        public int      basePoint;          // ��b��V

        [Header("�s�����")]
        [woskni.Name("���G����")] public float          invincibleTime;
        [woskni.Name("�ړ�����")] public float          moveLength;
        public woskni.Range                             moveIntervalRange;
        [woskni.Name("�ړ�����")] public float          moveTime;

        /// [�ǉ��肢] �ǂ�Ȓe�ˏo���s���̂��̃X�N���v�g�E���



    }

    [SerializeField] List<Enemy> enemyList; //�G�l�~�[���X�g

    /// <summary>�G�l�~�[���X�g�擾</summary>
    /// <returns>�G�l�~�[���X�g</returns>
    public List<Enemy> GetEnemyList() { return enemyList; }

    /// <summary>�ԍ����w�肵�ăG�l�~�[�����擾</summary>
    /// <param name="index">�v�f�ԍ�</param>
    /// <returns>�w��G�l�~�[���</returns>
    public Enemy GetEnemyAt(int index) { return enemyList[index]; }
}

//#if UNITY_EDITOR
//[CustomPropertyDrawer(typeof(EnemyData.Enemy))]
//public class EnemyDataDrawer : PropertyDrawer
//{
//    Rect wholeRect;  // �S�̂̕�
//    float partialSum; // �Q�ƒ��A���܂ł̃v���p�e�B�̉����̍��v
//    SerializedProperty property;   // �v���p�e�B

//    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//    {
//        // ���̐ݒ�
//        EditorGUIUtility.labelWidth = 60f + EditorGUI.indentLevel * 20f;

//        // �v���p�e�B�\�������̍쐬
//        label = EditorGUI.BeginProperty(position, label, property);
//        {
//            Initialize(position, property, label);

//            int lastIndentLevel = EditorGUI.indentLevel;
//            EditorGUI.indentLevel = 0;

//            DivideField(0.4f, "name"    , "���O", 48f);
//            DivideField(0.0f, "image"   , "�摜", 48f);

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

//    /// <summary>���������Ŏw�肵�ăv���p�e�B���P�s�ŕ\��</summary>
//    /// <param name="widthRate">���̓v���p�e�B�̕�(0 to 1)</param>
//    /// <param name="propertyName">�\��������v���p�e�B��</param>
//    /// <param name="label">���x����</param>
//    /// <param name="labelWidth">���x����</param>
//    void DivideField(float widthRate, string propertyName, string label = "", float labelWidth = 0)
//    {
//        // �������K��ɏ]���Ă��邩�̊č�
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
//        else Debug.LogWarningFormat("�v���p�e�B��������܂���ł���:  '{0}' in '{1}'", propertyName, this.GetType());
//    }
//}
//#endif
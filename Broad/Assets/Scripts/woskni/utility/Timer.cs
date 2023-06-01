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
    public struct Timer : IEquatable<Timer>
    {
        /// <summary>タイマーの速度倍率</summary>
        public float timeScale;

        /// <summary>現在の経過秒数</summary>
        public float time;

        /// <summary>終了時間(秒)</summary>
        public float limit;

        /// <summary>終了時間の初期設定</summary>
        /// <param name="limit">終了時間</param>
        public void Setup(float limit) { time = 0.0f; this.limit = limit; timeScale = 1f; }

        /// <summary>経過時間の初期化</summary>
        public void Reset() { time = 0.0f; timeScale = 1f; }

        /// <summary>タイマー更新</summary>
        /// <param name="affectScale">Time.timeScaleの影響を受けるか</param>
        public void Update(bool affectScale = true) => time += affectScale ? Time.deltaTime * timeScale : Time.unscaledDeltaTime * timeScale;

        /// <summary>タイマーを終了させる</summary>
        public void Fin() => time = limit;

        /// <summary>終了検知</summary>
        public bool IsFinished() => time >= limit;

        /// <summary>デバッグログ</summary>
        public string DebugLog(bool isOutputLog = true, string name = "")
        {
            if(name == "") name =  ToString();

            string logText = name + " : time: " + time.ToString("F2") + " / limit: " + limit.ToString("F2");

            if (isOutputLog) {
                Debug.Log(logText);
                return string.Empty;
            }

            return logText;
        }

        public float TimeLeft() => limit - time;


        bool IEquatable<Timer>.Equals(Timer other){ return Equals(this, other); }
    }


#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(Timer))]
    public class TimerDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // limitプロパティのSerializedPropertyを取得
            SerializedProperty limitProperty = property.FindPropertyRelative("limit");

            // ラベルの表示
            label.text += ".limit";
            EditorGUI.LabelField(position, label);

            // limitプロパティのみ表示
            Rect prefix = EditorGUI.PrefixLabel(position, label);
            limitProperty.floatValue = EditorGUI.FloatField(prefix, limitProperty.floatValue);

            EditorGUI.EndProperty();
        }
    }
#endif
}
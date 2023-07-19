using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleBackground : MonoBehaviour
{

    [SerializeField] SpriteRenderer m_SpriteRenderer = null;

    [SerializeField] FieldData m_FieldData = null;

    [Header("フィールド番号")]
    [SerializeField, Range(-1, 10)] int m_FieldNum;

    

    void Start()
    {
        int num = m_FieldNum < 0
            ? Random.Range(0, m_FieldData.GetFieldList().Count)
            : Mathf.Min(m_FieldNum, m_FieldData.GetFieldList().Count - 1);

        // 指定 or ランダム なフィールドを設定する
        FieldData.Field randField = m_FieldData.GetFieldList()[num];

        m_SpriteRenderer.sprite = randField.sprite;

        m_SpriteRenderer.material.SetFloat  ("_Glossiness"  , randField.smoothness      );
        m_SpriteRenderer.material.SetFloat  ("_Metallic"    , randField.metallic        );
        m_SpriteRenderer.material.SetTexture("_BumpMap"     , randField.normal          );
        m_SpriteRenderer.material.SetFloat  ("_BumpScale"   , randField.normalIntensity );
    }
}

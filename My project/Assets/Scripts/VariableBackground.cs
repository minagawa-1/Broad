using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class VariableBackground : MonoBehaviour
{
    [SerializeField] Player m_Player;

    [SerializeField, woskni.Name("ˆÚ“®”{—¦")] float m_MoveRate;

    SpriteRenderer m_SpriteRenderer = null;

    void Start()
    {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();

        FieldData.Field field = TemporarySavingBounty.field;

        woskni.SoundManager.Play(field.BGM, volume: SaveSystem.m_SaveData.BGMvolume);

        m_SpriteRenderer.sprite = field.sprite;

        m_SpriteRenderer.material.SetFloat  ("_Glossiness"  , field.smoothness       );
        m_SpriteRenderer.material.SetFloat  ("_Metallic"    , field.metallic         );
        m_SpriteRenderer.material.SetTexture("_BumpMap"     , field.normal           );
        m_SpriteRenderer.material.SetFloat  ("_BumpScale"   , field.normalIntensity / 2f);
    }

    private void Update()
    {
        Vector3 pos = Vector3.zero;

        pos = CalcDifferencePlayerPosition  (pos, m_MoveRate);
        pos = CalcRandomMove                (pos, m_MoveRate);

        m_SpriteRenderer.transform.position = pos;
    }

    Vector3 CalcDifferencePlayerPosition(Vector3 pos, float moveRate = 0f)
    {
        Vector3 dif = m_Player.transform.position;

        dif.x *= moveRate;
        dif.y *= moveRate;

        return pos + dif;
    }

    Vector3 CalcRandomMove(Vector3 pos, float moveLength = 0f)
    {
        return pos;
    }
}

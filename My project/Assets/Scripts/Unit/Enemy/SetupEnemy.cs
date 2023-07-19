using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SetupEnemy : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SetupSprite();
    }

    void SetupSprite()
    {
        var sr = GetComponent<SpriteRenderer>();

        sr.sprite = TemporarySavingBounty.enemy.sprite;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GameSetting : ScriptableObject
{
    public static GameSetting instance;

    // ScriptableObject�������������^�C�~���O�ŃC���X�^���X��ݒ肷��
    private void OnEnable() => instance = this;

    [Chapter("�Q�[�����")]

    [Header("�v���C���[�l��")]
    [Min(1)] public int playerNum;

    [Header("�v���C���[�̐F"), ReadOnly]
    public Color[] playerColors;

    [Chapter("�Ֆʏ��")]

    [Header("�ՖʃT�C�Y")]
    [Min(1)] public Vector2Int minBoardSize;
    [Min(1)] public Vector2Int maxBoardSize;

    [Header("�}�X�ڂ̐�����")]
    [Range(0f, 1f)] public float boardViability = 0.8f;

    [Header("�p�[�����m�C�Y�g�嗦")]
    [Comment("�l��0�ɋ߂��قǑ傫�����ɂȂ�A�l�������قǔ��_�̂悤�ȏ��������ɂȂ�", -8)]
    public float perlinScale = 0.1f;

    [Chapter("�u���b�N�X���")]

    [Header("�u���b�N��")]
    [Min(1)] public int minBlockUnits;
    [Min(1)] public int maxBlockUnits;

    [Header("���x")]
    [Range(0f, 1f)] public float minDensity;
    [Range(0f, 1f)] public float maxDensity;
}

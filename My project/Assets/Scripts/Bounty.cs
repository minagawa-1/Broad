#define DEBUG_MODE


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>�o�E���e�B���N���X</summary>
[System.Serializable]
public class Bounty
{

    public string   bountyName      = "";       //�o�E���e�B��
    public int      point           = 0;        //�|�C���g
    public int      life            = 1;        //���C�t
    public int      fieldIndex      = 0;        //�t�B�[���h�ԍ�
    public int      enemyIndex      = 0;        //�G�l�~�[�ԍ�
    public int      weaponIndex     = 0;        //����ԍ�
    public bool     editedFlag      = false;    //�ҏW�ς݃t���O
    public bool     completionFlag  = false;    //�B���t���O

    /// <summary>�o�E���e�B�쐬</summary>
    /// <param name="fieldData">�t�B�[���h�f�[�^</param>
    /// <param name="enemyData">�G�f�[�^</param>
    /// <param name="weaponData">����f�[�^</param>
    /// <param name="bountyNameData">�o�E���e�B���f�[�^</param>
    public void CreateBounty(FieldData fieldData, EnemyData enemyData,
                             BountyNameData bountyNameData, WeaponData weaponData)
    {
        //�t�B�[���h����
        ChoiceField(fieldData);

        //�G�l�~�[����
        ChoiceEnemy(enemyData);

        //�o�E���e�B������
        ChoiseBountyName(bountyNameData, enemyData);

        //���팈��
        ChoiceWeapon(weaponData);

        //�c�@�ݒ�
        life = Random.Range(SaveData.m_min_life, SaveData.m_max_life + 1);

        //��V�ݒ�
        DecidePoint(enemyData);

        //�ҏW�ς݃t���O���グ��
        editedFlag = true;

        //�B���ς݃t���O��������
        completionFlag = false;
    }

    //�t�B�[���h���I
    void ChoiceField(FieldData fieldData)
    {
        //�t�B�[���h�f�[�^����t�B�[���h��񃊃X�g���擾
        List<FieldData.Field> fieldList = fieldData.GetFieldList();

        //�����_���Ƀt�B�[���h�𒊑I����
        fieldIndex = Random.Range(0, fieldList.Count);

#if false
        fieldIndex = (int)FieldName.JapanCastle;
#endif
    }

    //�G�l�~�[���I
    void ChoiceEnemy(EnemyData enemyData)
    {
        //�G�l�~�[�f�[�^����G�l�~�[���X�g���擾
        List<EnemyData.Enemy> enemyList = enemyData.GetEnemyList();

        //�����_���ɃG�l�~�[�𒊑I����
        enemyIndex = Random.Range(0, enemyList.Count);

#if false
        enemyIndex = (int)EnemyName.Spider;
#endif
    }

    //���풊�I
    void ChoiceWeapon(WeaponData weaponData)
    {
        //����f�[�^���畐�탊�X�g���擾
        List<WeaponData.Weapon> weaponList = weaponData.GetWeaponList();

        //�����_���ɕ���𒊑I����
        weaponIndex = Random.Range(0, weaponList.Count);
    }

    //�o�E���e�B�����I
    void ChoiseBountyName(BountyNameData bountyNameData, EnemyData enemyData)
    {
        //�S�ʃo�E���e�B���ɂ��邩�ǂ������߂�
        bool generalBountyFlag = Random.Range(0, 2) == 1 ? true : false;

        //�S�ʃo�E���e�B�ɂ���ꍇ
        if (generalBountyFlag)
        {
            //�S�ʃo�E���e�B�����X�g���擾
            List<string> generalBountyNameList = bountyNameData.GetGeneralBounty();

            //�����_���ɕ�����𒊑I����
            bountyName = generalBountyNameList.AtRandom();
        }
        //�S�ʃo�E���e�B�ɂ��Ȃ��ꍇ
        else
        {
            //�f�[�^����o�E���e�B�����X�g���擾
            List<BountyNameData.BountyName> bountyNameList = bountyNameData.GetBountyNameList();

            //�t�B�[���h�ɕR�Â��������_���ȃo�E���e�B�����擾
            List<string> names = bountyNameList[fieldIndex].bountyNames;
            bountyName = names.AtRandom();
        }

        //�o�E���e�B���̎w�蕶������G�l�~�[���ɒu��
        List<string> replaceNameList = bountyNameData.GetReplaceNameList();
        foreach (string replaceName in replaceNameList)
            bountyName = bountyName.Replace(replaceName, enemyData.GetEnemyAt(enemyIndex).name);
    }

    //��V�ݒ�
    void DecidePoint(EnemyData enemyData)
    {
        //���C�t�̕��ς����߂�
        float avg = woskni.Easing.Linear(0.5f, 1.0f, SaveData.m_min_life, SaveData.m_max_life);

        //��{���V * ((�ő僉�C�t + 1 - ���C�t) / ����)�̎l�̌ܓ�(100�̈�)
        point = Round((int)(enemyData.GetEnemyAt(enemyIndex).basePoint * ((SaveData.m_max_life + 1 - life) / avg)), 3);
    }

    /// <summary>�l�̌ܓ�</summary>
    /// <param name="value">���l</param>
    /// <param name="digit">����</param>
    /// <returns>�l�̌ܓ������l</returns>
    int Round(int value, int digit)
    {
        float temp = value;
        //�����𐔒l�ɕϊ�
        digit = (int)Mathf.Pow(10.0f, digit - 1);

        // �l�̌ܓ�����
        System.MidpointRounding roundType = System.MidpointRounding.AwayFromZero;
        temp = (float)System.Math.Round(temp / digit, roundType) * digit;

        //�l��Ԃ�
        return (int)temp;
    }
}

/// <summary>�o�E���e�B���ꎞ�ۑ��N���X</summary>
public class TemporarySavingBounty
{
    public static FieldData.Field   field;                  //�t�B�[���h���
    public static EnemyData.Enemy   enemy;                  //�G�l�~�[���
    public static WeaponData.Weapon bonusWeapon;            //�{�[�i�X����
    public static WeaponData.Weapon equipWeapon;            //��������
    public static string            bountyName  = "";       //�o�E���e�B��
    public static float             difficulty  = 0.0f;     //��Փx
    public static int               point       = 0;        //�|�C���g
    public static int               life        = 1;        //���C�t
    public static int               bountyIndex = 0;        //�o�E���e�B�ԍ�
}

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
// �����o�萔

    // �����̏�����
    public const int    m_initial_money = 10000;

    // �����������ς݂̕��퐔
    public const int    m_initial_has_weapons = 3;

    // ������Փx
    public const float  m_initial_difficulty = 0.2f;

    // ��������
    public const float m_initial_volume = woskni.Sound.default_volume;

    // �o�E���e�B��
    public const int    m_bounty_count = 8;

    // �ŏ����C�t
    public const int    m_min_life = 1;

    // �ő僉�C�t
    public const int    m_max_life = 5;

// �Z�[�u�p�����o�ϐ�

    // ���݂̏�����
    public int          money = m_initial_money;

    //��������ID
    public int          eqipWeaponID = 0;

    // �����ς݂̕�����
    public bool[]       hasWeapons = 
                new bool[woskni.Enum.GetLength<WeaponName>()];

    // �����ς݂̓G���
    public bool[]       completedEnemies = 
                new bool[woskni.Enum.GetLength<EnemyName>()];

    // �o�E���e�B�f�[�^
    public Bounty[]     bounties = new Bounty[m_bounty_count];

    // ��Փx
    public float        difficulty = 0f;

    // �ŏI�Z�[�u����(�o�C�i���f�[�^)
    public long         lastSaveTimeBinary;

    // ����
    public float        BGMvolume = m_initial_volume;

    // ����
    public float        SEvolume = m_initial_volume;

    /// <remarks>�ҋ@���t���O(Away from keyboard)</remarks>
    public bool         AFKflag = false;

// �����o�֐�

    // �ŏI�Z�[�u�����擾
    public DateTime GetLastSaveTime() => DateTime.FromBinary(Convert.ToInt64(lastSaveTimeBinary));
}

public class SaveSystem
{
    //�o�E���e�B�X�V����
    public const int m_update_hour = 6;

    // �Z�[�u�f�[�^�̐���
    public static SaveData m_SaveData = new SaveData();

    // �Z�[�u�f�[�^�ۑ�����
    static string m_FilePath = Application.persistentDataPath + "/.savedata.json";

    // �Z�[�u
    static public void Save()
    {
        //���������o�C�i���f�[�^�ɂ��ĕۑ�
        m_SaveData.lastSaveTimeBinary = DateTime.Now.ToBinary();

        // �f�[�^�̊m�F(�f�o�b�O�@�\)
        ConfirmData(m_SaveData);

        //StreamWriter�N���X�𐶐�
        StreamWriter streamWriter = new StreamWriter(m_FilePath);

        // �Í������ꂽ������
        string encrypt = woskni.AESEncrypt.Encrypt(JsonUtility.ToJson(m_SaveData));

        // �f�[�^��������
        streamWriter.Write(encrypt);

        // �f�[�^��Flush�֐��ŕۑ�
        streamWriter.Flush();

        // �f�[�^�����
        streamWriter.Close();
    }

    // ���[�h
    static public void Load()
    {
        //�t�@�C���̕ۑ��悪��������
        if (File.Exists(m_FilePath))
        {
            //StreamReader�N���X�𐶐�
            StreamReader streamReader = new StreamReader(m_FilePath);

            // �f�[�^�̍Ō�܂œǂݍ���
            string data = streamReader.ReadToEnd();

            // �f�[�^�����
            streamReader.Close();

            // ���������ꂽ������
            string decrypt = woskni.AESEncrypt.Decrypt(data);

            // �f�[�^�̒l�𕜍��ς݂̂��̂ɒu������
            m_SaveData = JsonUtility.FromJson<SaveData>(decrypt);

            //���[�h�������擾���āA�����X�V�������߂��Ă�����o�E���e�B���X�V����
            if (IsUpdatedDay(DateTime.Now, m_SaveData.GetLastSaveTime()))
            {
                //�o�E���e�B����S�ă��Z�b�g
                for (int i = 0; i < m_SaveData.bounties.Length; ++i)
                    m_SaveData.bounties[i] = new Bounty();
            }
        }
        //�t�@�C���̕ۑ��悪������Ȃ�����
        else
        {
            Debug.Log("�f�[�^�������ĂȂ����");
            Reset();
        }
    }

    //���Z�b�g
    static public void Reset()
    {
        // ��������������̏�������
        m_SaveData.money = SaveData.m_initial_money;

        // ��x�ݒ��0��
        m_SaveData.difficulty = SaveData.m_initial_difficulty;

        m_SaveData.BGMvolume = SaveData.m_initial_volume;
        m_SaveData.SEvolume = SaveData.m_initial_volume;

        // ����̍w���L�^���ŏ���3�݂̂�
        for (int i = 0; i < m_SaveData.hasWeapons.Length; ++i)
            m_SaveData.hasWeapons[i] = i < SaveData.m_initial_has_weapons ? true : false;

        // �G�̓����������ׂĂȂ��������Ƃ�
        for (int i = 0; i < m_SaveData.completedEnemies.Length; ++i)
            m_SaveData.completedEnemies[i] = false;

        //�o�E���e�B����S�ă��Z�b�g
        for (int i = 0; i < m_SaveData.bounties.Length; ++i)
            m_SaveData.bounties[i] = new Bounty();

        //�������탊�Z�b�g
        m_SaveData.eqipWeaponID = 0;

        Debug.Log("�ǂ����Ă����̂���");
        //Debug.Log("�f�[�^�����Z�b�g����܂���");
    }

    /// <remarks>�W���p�̃��Z�b�g����</remarks>
    static public void TrialReset()
    {
        // ��x�ݒ��0��
        m_SaveData.difficulty = SaveData.m_initial_difficulty;

        m_SaveData.BGMvolume = SaveData.m_initial_volume;
        m_SaveData.SEvolume = SaveData.m_initial_volume;

        // ����̍w���L�^���ŏ���3�݂̂�
        for (int i = 0; i < m_SaveData.hasWeapons.Length; ++i)
            m_SaveData.hasWeapons[i] = i < SaveData.m_initial_has_weapons ? true : false;

        // �G�̓����������ׂĂȂ��������Ƃ�
        for (int i = 0; i < m_SaveData.completedEnemies.Length; ++i)
            m_SaveData.completedEnemies[i] = false;

        //�o�E���e�B����S�ă��Z�b�g
        for (int i = 0; i < m_SaveData.bounties.Length; ++i)
            m_SaveData.bounties[i] = new Bounty();

        Debug.Log("�ǂ����Ă����̂���");
        //Debug.Log("�f�[�^�����Z�b�g����܂���");
    }

    // �f�[�^�m�F
    static public void ConfirmData(SaveData data)
    {
        string json = JsonUtility.ToJson(data);

        if (!string.IsNullOrEmpty(json))
        {
            const string space = ": ";
            string text = "";

            text = "Money" + space + data.money;
            text += "\nHasWeapons" + space;
            foreach (bool flag in data.hasWeapons)
                text += flag ? "1" : "0";
            text += "\nCompletedEnemies" + space;
            foreach (bool flag in data.completedEnemies)
                text += flag ? "1" : "0";
            text += "\nBounties" + space + "{\n";
            for (int i = 0; i < data.bounties.Length; i++)
            {
                Bounty bounty = data.bounties[i];
                text += "   Bounty"       + i + space;
                text += "[ Name"          + space + bounty.bountyName;
                text += " , Point"        + space + bounty.point;
                text += " , Life"         + space + bounty.life;
                text += " , FieldIndex"   + space + bounty.fieldIndex;
                text += " , EnemyIndex"   + space + bounty.enemyIndex;
                text += " , WeaponIndex"  + space + bounty.weaponIndex;
                text += " ]\n";
            }
            text += " }";
            text += "\nDifficulty" + space + data.difficulty;
            text += "\nLastSaveTimeBinary" + space + data.lastSaveTimeBinary;
            text += "\nBGMvolume" + space + data.BGMvolume;
            text += "\nSEvolume"  + space + data.SEvolume;

            text += "\n";
            Debug.Log(text);
        }
        else Debug.Log("�f�[�^���Ȃ���");
    }

    /// <summary>�o�E���e�B�̍X�V���Ԃ��߂��Ă��邩</summary>
    /// <param name="loadDay">���[�h����</param>
    /// <param name="lastSaveDay">�ŏI�Z�[�u����</param>
    /// <returns>�X�V���Ԃ��߂��ē����True</returns>
    public static bool IsUpdatedDay(DateTime loadDay, DateTime lastSaveDay)
    {
        // ����
        loadDay = loadDay.AddHours(-m_update_hour);
        lastSaveDay = lastSaveDay.AddHours(-m_update_hour);

        // �N����
        if (loadDay.Date > lastSaveDay.Date) return true;
        if (loadDay.Month > lastSaveDay.Month) return true;
        if (loadDay.Year > lastSaveDay.Year) return true;

        return false;
    }
}

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
// メンバ定数

    // 初期の所持金
    public const int    m_initial_money = 10000;

    // 初期から解放済みの武器数
    public const int    m_initial_has_weapons = 3;

    // 初期難易度
    public const float  m_initial_difficulty = 0.2f;

    // 初期音量
    public const float m_initial_volume = woskni.Sound.default_volume;

    // バウンティ数
    public const int    m_bounty_count = 8;

    // 最少ライフ
    public const int    m_min_life = 1;

    // 最大ライフ
    public const int    m_max_life = 5;

// セーブ用メンバ変数

    // 現在の所持金
    public int          money = m_initial_money;

    //装備武器ID
    public int          eqipWeaponID = 0;

    // 所持済みの武器情報
    public bool[]       hasWeapons = 
                new bool[woskni.Enum.GetLength<WeaponName>()];

    // 討伐済みの敵情報
    public bool[]       completedEnemies = 
                new bool[woskni.Enum.GetLength<EnemyName>()];

    // バウンティデータ
    public Bounty[]     bounties = new Bounty[m_bounty_count];

    // 難易度
    public float        difficulty = 0f;

    // 最終セーブ日時(バイナリデータ)
    public long         lastSaveTimeBinary;

    // 音量
    public float        BGMvolume = m_initial_volume;

    // 音量
    public float        SEvolume = m_initial_volume;

    /// <remarks>待機中フラグ(Away from keyboard)</remarks>
    public bool         AFKflag = false;

// メンバ関数

    // 最終セーブ日時取得
    public DateTime GetLastSaveTime() => DateTime.FromBinary(Convert.ToInt64(lastSaveTimeBinary));
}

public class SaveSystem
{
    //バウンティ更新時間
    public const int m_update_hour = 6;

    // セーブデータの生成
    public static SaveData m_SaveData = new SaveData();

    // セーブデータ保存先情報
    static string m_FilePath = Application.persistentDataPath + "/.savedata.json";

    // セーブ
    static public void Save()
    {
        //現時刻をバイナリデータにして保存
        m_SaveData.lastSaveTimeBinary = DateTime.Now.ToBinary();

        // データの確認(デバッグ機能)
        ConfirmData(m_SaveData);

        //StreamWriterクラスを生成
        StreamWriter streamWriter = new StreamWriter(m_FilePath);

        // 暗号化された文字列
        string encrypt = woskni.AESEncrypt.Encrypt(JsonUtility.ToJson(m_SaveData));

        // データ書き込み
        streamWriter.Write(encrypt);

        // データをFlush関数で保存
        streamWriter.Flush();

        // データを閉じる
        streamWriter.Close();
    }

    // ロード
    static public void Load()
    {
        //ファイルの保存先が見つかった
        if (File.Exists(m_FilePath))
        {
            //StreamReaderクラスを生成
            StreamReader streamReader = new StreamReader(m_FilePath);

            // データの最後まで読み込む
            string data = streamReader.ReadToEnd();

            // データを閉じる
            streamReader.Close();

            // 復号化された文字列
            string decrypt = woskni.AESEncrypt.Decrypt(data);

            // データの値を復号済みのものに置換する
            m_SaveData = JsonUtility.FromJson<SaveData>(decrypt);

            //ロード日時を取得して、自動更新時刻を過ぎていたらバウンティを更新する
            if (IsUpdatedDay(DateTime.Now, m_SaveData.GetLastSaveTime()))
            {
                //バウンティ情報を全てリセット
                for (int i = 0; i < m_SaveData.bounties.Length; ++i)
                    m_SaveData.bounties[i] = new Bounty();
            }
        }
        //ファイルの保存先が見つからなかった
        else
        {
            Debug.Log("データが入ってないやん");
            Reset();
        }
    }

    //リセット
    static public void Reset()
    {
        // お金を初期からの所持金に
        m_SaveData.money = SaveData.m_initial_money;

        // 難度設定を0に
        m_SaveData.difficulty = SaveData.m_initial_difficulty;

        m_SaveData.BGMvolume = SaveData.m_initial_volume;
        m_SaveData.SEvolume = SaveData.m_initial_volume;

        // 武器の購入記録を最初の3つのみに
        for (int i = 0; i < m_SaveData.hasWeapons.Length; ++i)
            m_SaveData.hasWeapons[i] = i < SaveData.m_initial_has_weapons ? true : false;

        // 敵の討伐情報をすべてなかったことに
        for (int i = 0; i < m_SaveData.completedEnemies.Length; ++i)
            m_SaveData.completedEnemies[i] = false;

        //バウンティ情報を全てリセット
        for (int i = 0; i < m_SaveData.bounties.Length; ++i)
            m_SaveData.bounties[i] = new Bounty();

        //装備武器リセット
        m_SaveData.eqipWeaponID = 0;

        Debug.Log("どうしてくれんのこれ");
        //Debug.Log("データがリセットされました");
    }

    /// <remarks>展示用のリセット処理</remarks>
    static public void TrialReset()
    {
        // 難度設定を0に
        m_SaveData.difficulty = SaveData.m_initial_difficulty;

        m_SaveData.BGMvolume = SaveData.m_initial_volume;
        m_SaveData.SEvolume = SaveData.m_initial_volume;

        // 武器の購入記録を最初の3つのみに
        for (int i = 0; i < m_SaveData.hasWeapons.Length; ++i)
            m_SaveData.hasWeapons[i] = i < SaveData.m_initial_has_weapons ? true : false;

        // 敵の討伐情報をすべてなかったことに
        for (int i = 0; i < m_SaveData.completedEnemies.Length; ++i)
            m_SaveData.completedEnemies[i] = false;

        //バウンティ情報を全てリセット
        for (int i = 0; i < m_SaveData.bounties.Length; ++i)
            m_SaveData.bounties[i] = new Bounty();

        Debug.Log("どうしてくれんのこれ");
        //Debug.Log("データがリセットされました");
    }

    // データ確認
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
        else Debug.Log("データがないぞ");
    }

    /// <summary>バウンティの更新時間を過ぎているか</summary>
    /// <param name="loadDay">ロード時刻</param>
    /// <param name="lastSaveDay">最終セーブ時刻</param>
    /// <returns>更新時間を過ぎて入ればTrue</returns>
    public static bool IsUpdatedDay(DateTime loadDay, DateTime lastSaveDay)
    {
        // 時間
        loadDay = loadDay.AddHours(-m_update_hour);
        lastSaveDay = lastSaveDay.AddHours(-m_update_hour);

        // 年月日
        if (loadDay.Date > lastSaveDay.Date) return true;
        if (loadDay.Month > lastSaveDay.Month) return true;
        if (loadDay.Year > lastSaveDay.Year) return true;

        return false;
    }
}

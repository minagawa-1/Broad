#define DEBUG_MODE


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>バウンティ情報クラス</summary>
[System.Serializable]
public class Bounty
{

    public string   bountyName      = "";       //バウンティ名
    public int      point           = 0;        //ポイント
    public int      life            = 1;        //ライフ
    public int      fieldIndex      = 0;        //フィールド番号
    public int      enemyIndex      = 0;        //エネミー番号
    public int      weaponIndex     = 0;        //武器番号
    public bool     editedFlag      = false;    //編集済みフラグ
    public bool     completionFlag  = false;    //達成フラグ

    /// <summary>バウンティ作成</summary>
    /// <param name="fieldData">フィールドデータ</param>
    /// <param name="enemyData">敵データ</param>
    /// <param name="weaponData">武器データ</param>
    /// <param name="bountyNameData">バウンティ名データ</param>
    public void CreateBounty(FieldData fieldData, EnemyData enemyData,
                             BountyNameData bountyNameData, WeaponData weaponData)
    {
        //フィールド決定
        ChoiceField(fieldData);

        //エネミー決定
        ChoiceEnemy(enemyData);

        //バウンティ名決定
        ChoiseBountyName(bountyNameData, enemyData);

        //武器決定
        ChoiceWeapon(weaponData);

        //残機設定
        life = Random.Range(SaveData.m_min_life, SaveData.m_max_life + 1);

        //報酬設定
        DecidePoint(enemyData);

        //編集済みフラグを上げる
        editedFlag = true;

        //達成済みフラグを下げる
        completionFlag = false;
    }

    //フィールド抽選
    void ChoiceField(FieldData fieldData)
    {
        //フィールドデータからフィールド情報リストを取得
        List<FieldData.Field> fieldList = fieldData.GetFieldList();

        //ランダムにフィールドを抽選する
        fieldIndex = Random.Range(0, fieldList.Count);

#if false
        fieldIndex = (int)FieldName.JapanCastle;
#endif
    }

    //エネミー抽選
    void ChoiceEnemy(EnemyData enemyData)
    {
        //エネミーデータからエネミーリストを取得
        List<EnemyData.Enemy> enemyList = enemyData.GetEnemyList();

        //ランダムにエネミーを抽選する
        enemyIndex = Random.Range(0, enemyList.Count);

#if false
        enemyIndex = (int)EnemyName.Spider;
#endif
    }

    //武器抽選
    void ChoiceWeapon(WeaponData weaponData)
    {
        //武器データから武器リストを取得
        List<WeaponData.Weapon> weaponList = weaponData.GetWeaponList();

        //ランダムに武器を抽選する
        weaponIndex = Random.Range(0, weaponList.Count);
    }

    //バウンティ名抽選
    void ChoiseBountyName(BountyNameData bountyNameData, EnemyData enemyData)
    {
        //全般バウンティ名にするかどうか決める
        bool generalBountyFlag = Random.Range(0, 2) == 1 ? true : false;

        //全般バウンティにする場合
        if (generalBountyFlag)
        {
            //全般バウンティ名リストを取得
            List<string> generalBountyNameList = bountyNameData.GetGeneralBounty();

            //ランダムに文字列を抽選する
            bountyName = generalBountyNameList.AtRandom();
        }
        //全般バウンティにしない場合
        else
        {
            //データからバウンティ名リストを取得
            List<BountyNameData.BountyName> bountyNameList = bountyNameData.GetBountyNameList();

            //フィールドに紐づいたランダムなバウンティ名を取得
            List<string> names = bountyNameList[fieldIndex].bountyNames;
            bountyName = names.AtRandom();
        }

        //バウンティ名の指定文字列をエネミー名に置換
        List<string> replaceNameList = bountyNameData.GetReplaceNameList();
        foreach (string replaceName in replaceNameList)
            bountyName = bountyName.Replace(replaceName, enemyData.GetEnemyAt(enemyIndex).name);
    }

    //報酬設定
    void DecidePoint(EnemyData enemyData)
    {
        //ライフの平均を求める
        float avg = woskni.Easing.Linear(0.5f, 1.0f, SaveData.m_min_life, SaveData.m_max_life);

        //基本報報酬 * ((最大ライフ + 1 - ライフ) / 平均)の四捨五入(100の位)
        point = Round((int)(enemyData.GetEnemyAt(enemyIndex).basePoint * ((SaveData.m_max_life + 1 - life) / avg)), 3);
    }

    /// <summary>四捨五入</summary>
    /// <param name="value">数値</param>
    /// <param name="digit">桁数</param>
    /// <returns>四捨五入した値</returns>
    int Round(int value, int digit)
    {
        float temp = value;
        //桁数を数値に変換
        digit = (int)Mathf.Pow(10.0f, digit - 1);

        // 四捨五入処理
        System.MidpointRounding roundType = System.MidpointRounding.AwayFromZero;
        temp = (float)System.Math.Round(temp / digit, roundType) * digit;

        //値を返す
        return (int)temp;
    }
}

/// <summary>バウンティ情報一時保存クラス</summary>
public class TemporarySavingBounty
{
    public static FieldData.Field   field;                  //フィールド情報
    public static EnemyData.Enemy   enemy;                  //エネミー情報
    public static WeaponData.Weapon bonusWeapon;            //ボーナス武器
    public static WeaponData.Weapon equipWeapon;            //装備武器
    public static string            bountyName  = "";       //バウンティ名
    public static float             difficulty  = 0.0f;     //難易度
    public static int               point       = 0;        //ポイント
    public static int               life        = 1;        //ライフ
    public static int               bountyIndex = 0;        //バウンティ番号
}

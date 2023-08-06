using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

/// <summary>セーブデータ情報クラス（セーブデータの参照はSaveSystem.data</summary>
[Serializable]
public class SaveData
{
    /// <summary>所持しているブロックスのリスト</summary>
    public List<Blocks> blocksList = new List<Blocks>();

    /// <summary>デッキ情報</summary>
    public Blocks[]     deck = new Blocks[GameSetting.deck_blocks];

    /// <summary>前回プレイしたときのカラー</summary>
    public Color        lastColor = Color.white;
}

public class SaveSystem
{
    /// <summary>セーブデータ情報</summary>
    public static SaveData data = new SaveData();

    /// <summary>セーブデータの保存先（ファイルパス）</summary>
    static readonly string m_FilePath = Application.persistentDataPath + "/.savedata.json";

    /// <summary>セーブデータの書込み</summary>
    public static void Save()
    {
        SerializeBlocks();

        // データの確認(デバッグ用)
        ConfirmData();

        //StreamWriterクラスを生成
        StreamWriter streamWriter = new StreamWriter(m_FilePath);

        // 暗号化された文字列
        string encrypt = woskni.AESEncrypt.Encrypt(JsonUtility.ToJson(data));

        // データ書き込み
        streamWriter.Write(encrypt);

        // データをFlush関数で保存
        streamWriter.Flush();

        // データを閉じる
        streamWriter.Close();
    }

    /// <summary>セーブデータの読込み</summary>
    public static void Load()
    {
        // セーブデータが見つかった
        if (File.Exists(m_FilePath))
        {
            //StreamReaderクラスを生成
            StreamReader streamReader = new StreamReader(m_FilePath);

            // データの最後まで読み込む
            string dataText = streamReader.ReadToEnd();

            // データを閉じる
            streamReader.Close();

            // 復号化された文字列
            string decrypt = woskni.AESEncrypt.Decrypt(dataText);

            // データの値を復号済みのものに置換する
            data = JsonUtility.FromJson<SaveData>(decrypt);

            // Shape情報は二次元配列のため、デシリアライズする
            for (int i = 0; i < data.deck.Length;      ++i) data.deck[i].DeserializeShape();
            for (int i = 0; i < data.blocksList.Count; ++i) data.blocksList[i].DeserializeShape();
        }
        // セーブデータが見つからなかった
        else
        {
            // セーブデータを初期化
            Reset();
        }
    }

    /// <summary>セーブデータの初期化</summary>
    public static void Reset()
    {
        data = new SaveData();
        data.blocksList.Clear();

        int blocksNum = UnityEngine.Random.Range(17, 35);
        for (int i = 0; i < blocksNum; ++i)
        {
            data.blocksList.Add(LotteryBlocks.Lottery());
            data.blocksList.Last().index = i;
        }

        // デッキ情報の再設定
        data.deck = new Blocks[GameSetting.deck_blocks];

        // ブロックスリストのブロックスをデッキに設定しておく
        for (int i = 0; i < GameSetting.deck_blocks; ++i)
            data.deck[i] = i < blocksNum ? data.blocksList[i] : null;

        // 前回のカラーはランダムに設定
        data.lastColor = GameSetting.instance.GetRandomColor();

        Save();
    }

    /// <summary>セーブデータのデバッグログ出力</summary>
    public static void ConfirmData()
        => Debug.Log($"{m_FilePath} ({DateTime.Now.ToShortTimeString()}): \n" + JsonUtility.ToJson(data));


    /// <summary>ブロックスのshape情報をserializedShape情報に移す</summary>
    static void SerializeBlocks()
    {
        // Shape情報は二次元配列のため、シリアライズする

        for (int i = 0; i < data.deck.Length; ++i)
            if (data.deck[i] != null) {
                data.deck[i].serializedShape = JsonConvert.SerializeObject(data.deck[i].shape);
                data.deck[i].index = i;
            }
        
        for (int i = 0; i < data.blocksList.Count; ++i)
            if (data.blocksList[i] != null) {
                data.blocksList[i].serializedShape = JsonConvert.SerializeObject(data.blocksList[i].shape);
                data.blocksList[i].index = i;
            }
    }
}

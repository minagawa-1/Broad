using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

/// <summary>コンフィグ情報クラス（コンフィグデータの参照はConfig.data</summary>
[Serializable]
public class ConfigData
{
    public const float basis_deadzone = 0.5f;
    public const float basis_move_speed = 0.25f;
    public const float basis_volume = 0.7f;

// General

    /// <summary>プレイヤー名</summary>
    public string   playerName      = "";

    /// <summary>自動セーブ</summary>
    public bool     autosave        = true;

// Operation

    /// <summary>デッドゾーン (左スティック)</summary>
    public float    deadzoneLeft    = basis_deadzone;

    /// <summary>デッドゾーン (右スティック)</summary>
    public float    deadzoneRight   = basis_deadzone;

    /// <summary>移動速度</summary>
    public float    moveSpeed       = basis_move_speed;

    /// <summary>ボタンガイドの表示</summary>
    public bool     buttonGuide     = true;

    // Volume

    /// <summary>マスター音量</summary>
    public float masterVolume       = basis_volume;

    /// <summary>BGM音量</summary>
    public float bgmVolume          = basis_volume;

    /// <summary>SE音量</summary>
    public float seVolume           = basis_volume;

// Language

    /// <summary>言語</summary>
    public SystemLanguage language  = SystemLanguage.Japanese;

// Screen

    /// <summary>明るさ</summary>
    public float brightness         = 1f;

    /// <summary>文字サイズ</summary>
    public float fontSizeScale      = 1f;

    /// <summary>画面サイズ</summary>
    public Vector2Int screenSize    = new Vector2Int(Screen.width, Screen.height);

    public ConfigData Copy() => (ConfigData)MemberwiseClone();
}

public class Config
{
    /// <summary>セーブデータ情報</summary>
    public static ConfigData data = new ConfigData();

    /// <summary>セーブデータの保存先（ファイルパス）</summary>
    static readonly string m_FilePath = Application.persistentDataPath + "/.config.json";

    /// <summary>セーブデータの書込み</summary>
    public static void Save()
    {
        // データの確認(デバッグ用)
        //ConfirmData();

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
            data = JsonUtility.FromJson<ConfigData>(decrypt);
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
        data = new ConfigData();

        // General
        data.playerName     = "";
        data.autosave       = true;

        // Operation
        data.deadzoneLeft   = ConfigData.basis_deadzone;
        data.deadzoneRight  = ConfigData.basis_deadzone;
        data.moveSpeed      = ConfigData.basis_move_speed;
        data.buttonGuide    = true;

        // Volume
        data.masterVolume   = ConfigData.basis_volume;
        data.bgmVolume      = ConfigData.basis_volume;
        data.seVolume       = ConfigData.basis_volume;

        // Language
        data.language       = SystemLanguage.Japanese;

        // Screen
        data.brightness     = 1f;
        data.fontSizeScale  = 1f;
        data.screenSize     = new Vector2Int(Screen.width, Screen.height);

        Save();
    }

    /// <summary>セーブデータのデバッグログ出力</summary>
    public static void ConfirmData()
        => Debug.Log($"{m_FilePath} ({DateTime.Now.ToShortTimeString()}): \n" + JsonUtility.ToJson(data));
}
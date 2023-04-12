using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable/Create BulletData")]
public class BulletData : ScriptableObject
{
    //プレイヤー弾幕構造体
    [System.Serializable]
    public struct Bullet
    {
        [woskni.Name("弾幕名")]   public string  name;

        [woskni.Name("画像")]     public Sprite  sprite;

        [woskni.Name("拡大率")]   public Vector2 scale;

        [woskni.Name("回転速度")] public float   rotateSpeed;
    }

    [SerializeField, Space(32)] List<Bullet>               bulletDataList;     // 弾幕データリスト
    [SerializeField, Space(32)] List<BulletPreset>         bulletPresetList;   // ステータスプリセットリスト
    [SerializeField, Space(32)] List<BulletProcess>        bulletProcessList;  // 弾幕設定リスト

    public List<Bullet> GetBulletDataList() { return bulletDataList; }

    public Bullet GetBulletAt(int index) { return bulletDataList[index]; }

    public List<BulletPreset> GetBulletPresetList() { return bulletPresetList; }

    public List<BulletProcess> GetBulletProcessList() { return bulletProcessList; }
}

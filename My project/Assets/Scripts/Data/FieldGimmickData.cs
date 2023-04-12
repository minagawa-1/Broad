using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// スクリプタブルオブジェクトを作れるようにする
[CreateAssetMenu(menuName = "Scriptable/Create FieldGimmickData")]
public class FieldGimmickData : ScriptableObject
{
    [SerializeField, Space(32)] List<BulletPreset> weaponPresetList;
    [SerializeField, Space(32)] List<BulletProcess> weaponProcessList;

    public List<BulletPreset> GetWeaponPresetList() { return weaponPresetList; }
    public List<BulletProcess> GetWeaponProcessList() { return weaponProcessList; }
}

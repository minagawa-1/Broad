using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable/Create BountyNameData")]
public class BountyNameData : ScriptableObject
{
    //バウンティ名構造体
    [System.Serializable]
    public struct BountyName
    {
        public string       fieldName;      //フィールド名
        public List<string> bountyNames;    //バウンティ名リスト
    }

    [SerializeField] List<string>       replaceNameList;    //置換文字列リスト
    [Space(24)]
    [SerializeField] List<string>       generalBounty;      //全般バウンティ名
    [Space(24)]
    [SerializeField] List<BountyName>   bountyNameList;     //バウンティ名リスト

    public List<string> GetReplaceNameList() { return replaceNameList; }
    public List<string> GetGeneralBounty() { return generalBounty; }
    public List<BountyName> GetBountyNameList() { return bountyNameList; }
}

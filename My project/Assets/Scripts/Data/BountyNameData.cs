using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable/Create BountyNameData")]
public class BountyNameData : ScriptableObject
{
    //�o�E���e�B���\����
    [System.Serializable]
    public struct BountyName
    {
        public string       fieldName;      //�t�B�[���h��
        public List<string> bountyNames;    //�o�E���e�B�����X�g
    }

    [SerializeField] List<string>       replaceNameList;    //�u�������񃊃X�g
    [Space(24)]
    [SerializeField] List<string>       generalBounty;      //�S�ʃo�E���e�B��
    [Space(24)]
    [SerializeField] List<BountyName>   bountyNameList;     //�o�E���e�B�����X�g

    public List<string> GetReplaceNameList() { return replaceNameList; }
    public List<string> GetGeneralBounty() { return generalBounty; }
    public List<BountyName> GetBountyNameList() { return bountyNameList; }
}

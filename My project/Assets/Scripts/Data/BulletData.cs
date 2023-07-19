using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable/Create BulletData")]
public class BulletData : ScriptableObject
{
    //�v���C���[�e���\����
    [System.Serializable]
    public struct Bullet
    {
        [woskni.Name("�e����")]   public string  name;

        [woskni.Name("�摜")]     public Sprite  sprite;

        [woskni.Name("�g�嗦")]   public Vector2 scale;

        [woskni.Name("��]���x")] public float   rotateSpeed;
    }

    [SerializeField, Space(32)] List<Bullet>               bulletDataList;     // �e���f�[�^���X�g
    [SerializeField, Space(32)] List<BulletPreset>         bulletPresetList;   // �X�e�[�^�X�v���Z�b�g���X�g
    [SerializeField, Space(32)] List<BulletProcess>        bulletProcessList;  // �e���ݒ胊�X�g

    public List<Bullet> GetBulletDataList() { return bulletDataList; }

    public Bullet GetBulletAt(int index) { return bulletDataList[index]; }

    public List<BulletPreset> GetBulletPresetList() { return bulletPresetList; }

    public List<BulletProcess> GetBulletProcessList() { return bulletProcessList; }
}

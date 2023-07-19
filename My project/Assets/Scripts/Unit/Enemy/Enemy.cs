using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Unit
{
    [woskni.Name("Enemy�e���f�[�^")] public EnemyWeaponData m_EnemyWeaponData = null;  // enemy�e���f�[�^

    [woskni.Name("FieldGimmick�e���f�[�^")] public FieldGimmickData m_FieldGimmickData = null; // fieldGimmick�e���f�[�^

    [woskni.Name("�Ē��܊Ԋu"), SerializeField] float m_LotteryInterval = 10f;

    List<BulletPreset> m_EnemyWeaponPreset    = new List<BulletPreset>();         // EnemyWeaponPreset�p���X�g
    List<BulletProcess> m_EnemyWeaponProcess  = new List<BulletProcess>();        // EnemyWeaponProcess�p���X�g

    List<BulletPreset> m_FieldGimmickPreset   = new List<BulletPreset>();         // FieldGimmickPreset�p���X�g
    List<BulletProcess> m_FieldGimmickProcess = new List<BulletProcess>();        // FieldGimmickProcess�p���X�g

    List<float> m_FieldDirectionList          = new List<float>();                // ���o�p�x���X�g

    // �����_����Index�����߂��悤�ɂ��邽�߂̃^�C�}�[
    woskni.Timer m_IndexTimer;

    // Start is called before the first frame update
    protected override void Start()
    {
        invincibleTime = TemporarySavingBounty.enemy.invincibleTime;

        base.Start();
        m_TargetRenderer = m_ParentRenderer;

        stock = TemporarySavingBounty.enemy.HP;

        // indexTimer�̃Z�b�g�A�b�v
        m_IndexTimer.Setup(m_LotteryInterval);

        SetupWeapon();

        SetupFieldGimmck();

        // Debug.Log("Level: " + SaveSystem.m_SaveData.difficulty.ToString("F2"));
        // Debug.Log("16 �� " + ((int)Level.GetLevelRange(16f, 64f)).ToString() + " �� 64");
    }

    void SetupWeapon()
    {
        // * �G�l�~�[�̒e���������� * //

        int enemyIndex = SaveSystem.m_SaveData.bounties[TemporarySavingBounty.bountyIndex].enemyIndex;

        // enemyIndex��Joker�Ȃ�
        if (enemyIndex == (int) EnemyName.Joker)
        {
            // Joker�ȊO�ɂȂ�܂Ń��[�v
            while(true)
            {
                enemyIndex = (int)woskni.Enum.Random<EnemyName>();

                if (enemyIndex != (int)EnemyName.Joker)
                    break;
            }
        }

        List<BulletPreset> bulletPresets = m_EnemyWeaponData.GetWeaponPresetList();

        foreach (BulletPreset preset in bulletPresets)
        {
            if (preset.explanation.Contains(((EnemyName)enemyIndex).ToString()))
                m_EnemyWeaponPreset.Add(preset);
        }
        List<BulletProcess> bulletProcesses = m_EnemyWeaponData.GetWeaponProcessList();
        foreach (BulletPreset preset in m_EnemyWeaponPreset)
            m_EnemyWeaponProcess.Add(bulletProcesses[preset.processIndex]);

        for (int i = 0; i < m_EnemyWeaponProcess.Count; i++)
            m_DirectionList.Add(m_EnemyWeaponProcess[i].direction);

        //����̃^�C�}�[�ݒ�
        for (int i = 0; i < m_EnemyWeaponProcess.Count; ++i)
        {
            woskni.Timer timer = new woskni.Timer();

            // minInterval��maxInterval�Ń����_���l���Ƃ�
            float time = Level.GetLevelRange(m_EnemyWeaponPreset[i].injectionInterval.max, m_EnemyWeaponPreset[i].injectionInterval.min);
            // �^�C�}�[�̃Z�b�g�A�b�v
            timer.Setup(time);
            timer.Fin();

            m_CreateTimerList.Add(timer);
        }

        
    }
    void SetupFieldGimmck()
    {
        // * �t�B�[���h�M�~�b�N�̒e���������� * //

        // SaveData�Ŏw�肳�ꂽField�ԍ�������
        int fieldIndex = SaveSystem.m_SaveData.bounties[TemporarySavingBounty.bountyIndex].fieldIndex;

        // fieldIndex��UnendingDungeon�������ꍇ
        if (fieldIndex == (int)FieldName.UnendingDungeon)
        {
            // UnendingDungeon�ȊO�ɂȂ�܂Ń��[�v
            while (true)
            {
                fieldIndex = (int)woskni.Enum.Random<FieldName>();

                if (fieldIndex != (int)FieldName.UnendingDungeon)
                    break;
            }
        }

        // Preset���X�g���쐬
        List<BulletPreset> bulletPresets = m_FieldGimmickData.GetWeaponPresetList();
        foreach (BulletPreset preset in bulletPresets)
        {
            // preset��explanation��FieldName���܂܂�Ă�����̂�����Ȃ�AFieldGimmickPreset�ɒǉ�
            if (preset.explanation.Contains(((FieldName)fieldIndex).ToString()))
                m_FieldGimmickPreset.Add(preset);
        }
        List<BulletProcess> bulletProcesses = m_FieldGimmickData.GetWeaponProcessList();
        // FieldGimmickProcess��FieldGimmickPreset�Ŏw�肵��Process�𓖂Ă�
        foreach (BulletPreset preset in m_FieldGimmickPreset)
            m_FieldGimmickProcess.Add(bulletProcesses[preset.processIndex]);

        // FieldGimmickProcess�̐����A���o�p�����X�g�ɒǉ�
        for (int i = 0; i < m_FieldGimmickProcess.Count; i++)
            m_FieldDirectionList.Add(m_FieldGimmickProcess[i].direction);

        //�t�B�[���h�M�~�b�N�̃^�C�}�[�ݒ�
        for (int i = 0; i < m_FieldGimmickProcess.Count; ++i)
        {
            woskni.Timer gimmick_timer = new woskni.Timer();

            // minInterval��maxInterval�Ń����_���l���Ƃ�
            float time = Level.GetLevelRange(m_FieldGimmickPreset[i].injectionInterval.max, m_FieldGimmickPreset[i].injectionInterval.min);
            // �^�C�}�[�̃Z�b�g�A�b�v
            gimmick_timer.Setup(time);

            // �^�C�}�[�̒ǉ�
            m_GimmickTimerList.Add(gimmick_timer);
        }
    }


    public override void Damage(int damage, float invincibleTime)
    {
        base.Damage(damage, invincibleTime);

        // ���S����
        if (stock <= 0)
        {
            int boutny_id = TemporarySavingBounty.bountyIndex;
            Bounty bounty = SaveSystem.m_SaveData.bounties[boutny_id];
            bounty.completionFlag = true;
            SaveSystem.m_SaveData.completedEnemies[bounty.enemyIndex] = true;

            SaveSystem.Save();
        }
    }

    public void Fire()
    {
        m_IndexTimer.Update();

        /*
         *  Enemy�p�e���̐���
         */
        GenerateBullet(m_EnemyWeaponPreset, m_EnemyWeaponProcess, m_CreateTimerList, m_DirectionList);

        /*
         *  FieldGimmick�p�̒e������
         */
        GenerateBullet(m_FieldGimmickPreset, m_FieldGimmickProcess, m_GimmickTimerList, m_FieldDirectionList);

        if (m_IndexTimer.IsFinished())
        {
            // enemyIndex��Joker�Ȃ�DataList�ȊO��Clear���ASetup������
            if(SaveSystem.m_SaveData.bounties[TemporarySavingBounty.bountyIndex].enemyIndex == (int) EnemyName.Joker)
            {
                m_EnemyWeaponPreset.Clear();
                m_EnemyWeaponProcess.Clear();
                m_CreateTimerList.Clear();
                SetupWeapon();
            }
            // fieldIndex��UnendingDungeon�Ȃ�DataList�ȊO��Clear���ASetup������
            if (SaveSystem.m_SaveData.bounties[TemporarySavingBounty.bountyIndex].fieldIndex == (int)FieldName.UnendingDungeon)
            {
                m_FieldGimmickPreset.Clear();
                m_FieldGimmickProcess.Clear();
                m_GimmickTimerList.Clear();
                SetupFieldGimmck();
            }


            m_IndexTimer.Reset();
        }
    }

    void GenerateBullet(List<BulletPreset> presetList, List<BulletProcess> processList, List<woskni.Timer> timerList, List<float> directionList)
    {
        for (int i = 0; i < presetList.Count; ++i)
        {
            woskni.Timer timer = timerList[i];
            timer.Update();

            // ���E�F�C�o����
            int wayCount = (int)Level.GetLevelRange(processList[i].wayCountRange.min, processList[i].wayCountRange.max);
            // �ˏo�p�x
            float angle = (float)Level.GetLevelRange(processList[i].angleRange.max, processList[i].angleRange.min);

            // �^�C�}�[���I��������
            if (timer.IsFinished())
            {
                // �ˏo�ʒu
                Vector3 pos = transform.position;

                
                // Target�����邩
                if (processList[i].isLookTarget)
                {
                    //* �p�x�̌v�Z *//
                    // Target�ւ̌���
                    Vector2 direction_to_target = m_Target.transform.position - pos;

                    // �����_���ȍ��W���猂��
                    if (!processList[i].isSniping)
                    {
                        directionList[i] = Mathf.Atan2(direction_to_target.y, direction_to_target.x);
                        directionList[i] *= Mathf.Rad2Deg;

                        // �S���ʂɒe�����Ȃ�
                        if (processList[i].isAllRange)
                        {
                            for (int k = 0; k < wayCount; ++k)
                            {
                                m_BulletManager.Create(m_Target, unitLayer, pos, directionList[i] + (360f / wayCount) * k, m_BulletData.GetBulletAt((int)presetList[i].bullet), presetList[i], processList[i]);
                            }
                        }
                        // �w�肵�������ɒe������
                        else
                        {
                            float firstAngle = directionList[i] - ((wayCount - 1) * angle) / 2;
                            for (int n = 0; n < wayCount; ++n)
                            {
                                m_BulletManager.Create(m_Target, unitLayer, pos, firstAngle + n * angle, m_BulletData.GetBulletAt((int)presetList[i].bullet), presetList[i], processList[i]);
                            }
                        }
                    }
                    else
                    {
                        // �_���ʒu
                        Vector3 snipingPosition = Vector2.zero;

                        // ��ʂ̏�[�E���[�ŁA�v���C���[�̑Ίp�����x���W����o��
                        snipingPosition.x = -m_Target.transform.position.x + Random.Range(-2f, 2f);
                        snipingPosition.y = Camera.main.orthographicSize * (m_Target.transform.position.y < 0f ? 1f : -1f);

                        // Target�ւ̌������Čv�Z
                        Vector2 toTarget = m_Target.transform.position - snipingPosition;
                        directionList[i] = Mathf.Atan2(toTarget.y, toTarget.x);
                        directionList[i] *= Mathf.Rad2Deg;

                        // �S���ʂɒe�����Ȃ�
                        if (processList[i].isAllRange)
                        {
                            for (int k = 0; k < wayCount; ++k)
                            {
                                m_BulletManager.Create(m_Target, unitLayer, snipingPosition, directionList[i] + (360f / wayCount) * k, m_BulletData.GetBulletAt((int)presetList[i].bullet), presetList[i], processList[i]);
                            }
                        }
                        else
                        {
                            float firstAngle = directionList[i] - ((wayCount - 1) * angle) / 2;
                            for (int n = 0; n < wayCount; ++n)
                            {
                                m_BulletManager.Create(m_Target, unitLayer, snipingPosition, firstAngle + n * angle, m_BulletData.GetBulletAt((int)presetList[i].bullet), presetList[i], processList[i]);
                            }
                        }
                    }
                }
                // Target�����Ȃ�
                else
                {
                    // �S���ʂɒe�����Ȃ�
                    if (processList[i].isAllRange)
                    {
                        for (int k = 0; k < wayCount; ++k)
                        {
                            m_BulletManager.Create(m_Target, unitLayer, pos, directionList[i] + (360f / wayCount) * k, m_BulletData.GetBulletAt((int)presetList[i].bullet), presetList[i], processList[i]);
                        }
                    }
                    // �w�肵�������ɒe������
                    else
                    {
                        float firstAngle = directionList[i] - ((wayCount - 1) * angle) / 2;
                        for (int m = 0; m < wayCount; ++m)
                        {
                            m_BulletManager.Create(m_Target, unitLayer, pos, firstAngle + m * angle, m_BulletData.GetBulletAt((int)presetList[i].bullet), presetList[i], processList[i]);
                        }
                    }

                    //�ˏo�p�x�𑝂₷
                    directionList[i] += Level.GetLevelRange(processList[i].rotateDirection.max, processList[i].rotateDirection.min);
                }

                timer.Reset();

                timer.limit = Level.GetLevelRange(presetList[i].injectionInterval.max, presetList[i].injectionInterval.min);
            }

            timerList[i] = timer;
        }
    }

    Color GetAlphaColor(float alpha) => new Color(m_SpriteRenderer.color.r, m_SpriteRenderer.color.g, m_SpriteRenderer.color.b, alpha);
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Unit
{
    [woskni.Name("Enemy弾幕データ")] public EnemyWeaponData m_EnemyWeaponData = null;  // enemy弾幕データ

    [woskni.Name("FieldGimmick弾幕データ")] public FieldGimmickData m_FieldGimmickData = null; // fieldGimmick弾幕データ

    [woskni.Name("再抽籤間隔"), SerializeField] float m_LotteryInterval = 10f;

    List<BulletPreset> m_EnemyWeaponPreset    = new List<BulletPreset>();         // EnemyWeaponPreset用リスト
    List<BulletProcess> m_EnemyWeaponProcess  = new List<BulletProcess>();        // EnemyWeaponProcess用リスト

    List<BulletPreset> m_FieldGimmickPreset   = new List<BulletPreset>();         // FieldGimmickPreset用リスト
    List<BulletProcess> m_FieldGimmickProcess = new List<BulletProcess>();        // FieldGimmickProcess用リスト

    List<float> m_FieldDirectionList          = new List<float>();                // 放出角度リスト

    // ランダムにIndexを決めれるようにするためのタイマー
    woskni.Timer m_IndexTimer;

    // Start is called before the first frame update
    protected override void Start()
    {
        invincibleTime = TemporarySavingBounty.enemy.invincibleTime;

        base.Start();
        m_TargetRenderer = m_ParentRenderer;

        stock = TemporarySavingBounty.enemy.HP;

        // indexTimerのセットアップ
        m_IndexTimer.Setup(m_LotteryInterval);

        SetupWeapon();

        SetupFieldGimmck();

        // Debug.Log("Level: " + SaveSystem.m_SaveData.difficulty.ToString("F2"));
        // Debug.Log("16 ≦ " + ((int)Level.GetLevelRange(16f, 64f)).ToString() + " ≦ 64");
    }

    void SetupWeapon()
    {
        // * エネミーの弾幕処理準備 * //

        int enemyIndex = SaveSystem.m_SaveData.bounties[TemporarySavingBounty.bountyIndex].enemyIndex;

        // enemyIndexがJokerなら
        if (enemyIndex == (int) EnemyName.Joker)
        {
            // Joker以外になるまでループ
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

        //武器のタイマー設定
        for (int i = 0; i < m_EnemyWeaponProcess.Count; ++i)
        {
            woskni.Timer timer = new woskni.Timer();

            // minIntervalとmaxIntervalでランダム値をとる
            float time = Level.GetLevelRange(m_EnemyWeaponPreset[i].injectionInterval.max, m_EnemyWeaponPreset[i].injectionInterval.min);
            // タイマーのセットアップ
            timer.Setup(time);
            timer.Fin();

            m_CreateTimerList.Add(timer);
        }

        
    }
    void SetupFieldGimmck()
    {
        // * フィールドギミックの弾幕処理準備 * //

        // SaveDataで指定されたField番号を入れる
        int fieldIndex = SaveSystem.m_SaveData.bounties[TemporarySavingBounty.bountyIndex].fieldIndex;

        // fieldIndexがUnendingDungeonだった場合
        if (fieldIndex == (int)FieldName.UnendingDungeon)
        {
            // UnendingDungeon以外になるまでループ
            while (true)
            {
                fieldIndex = (int)woskni.Enum.Random<FieldName>();

                if (fieldIndex != (int)FieldName.UnendingDungeon)
                    break;
            }
        }

        // Presetリストを作成
        List<BulletPreset> bulletPresets = m_FieldGimmickData.GetWeaponPresetList();
        foreach (BulletPreset preset in bulletPresets)
        {
            // presetのexplanationにFieldNameが含まれているものがあるなら、FieldGimmickPresetに追加
            if (preset.explanation.Contains(((FieldName)fieldIndex).ToString()))
                m_FieldGimmickPreset.Add(preset);
        }
        List<BulletProcess> bulletProcesses = m_FieldGimmickData.GetWeaponProcessList();
        // FieldGimmickProcessにFieldGimmickPresetで指定したProcessを当てる
        foreach (BulletPreset preset in m_FieldGimmickPreset)
            m_FieldGimmickProcess.Add(bulletProcesses[preset.processIndex]);

        // FieldGimmickProcessの数分、放出角をリストに追加
        for (int i = 0; i < m_FieldGimmickProcess.Count; i++)
            m_FieldDirectionList.Add(m_FieldGimmickProcess[i].direction);

        //フィールドギミックのタイマー設定
        for (int i = 0; i < m_FieldGimmickProcess.Count; ++i)
        {
            woskni.Timer gimmick_timer = new woskni.Timer();

            // minIntervalとmaxIntervalでランダム値をとる
            float time = Level.GetLevelRange(m_FieldGimmickPreset[i].injectionInterval.max, m_FieldGimmickPreset[i].injectionInterval.min);
            // タイマーのセットアップ
            gimmick_timer.Setup(time);

            // タイマーの追加
            m_GimmickTimerList.Add(gimmick_timer);
        }
    }


    public override void Damage(int damage, float invincibleTime)
    {
        base.Damage(damage, invincibleTime);

        // 死亡判定
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
         *  Enemy用弾幕の生成
         */
        GenerateBullet(m_EnemyWeaponPreset, m_EnemyWeaponProcess, m_CreateTimerList, m_DirectionList);

        /*
         *  FieldGimmick用の弾幕生成
         */
        GenerateBullet(m_FieldGimmickPreset, m_FieldGimmickProcess, m_GimmickTimerList, m_FieldDirectionList);

        if (m_IndexTimer.IsFinished())
        {
            // enemyIndexがJokerならDataList以外をClearし、Setupし直す
            if(SaveSystem.m_SaveData.bounties[TemporarySavingBounty.bountyIndex].enemyIndex == (int) EnemyName.Joker)
            {
                m_EnemyWeaponPreset.Clear();
                m_EnemyWeaponProcess.Clear();
                m_CreateTimerList.Clear();
                SetupWeapon();
            }
            // fieldIndexがUnendingDungeonならDataList以外をClearし、Setupし直す
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

            // 何ウェイ出すか
            int wayCount = (int)Level.GetLevelRange(processList[i].wayCountRange.min, processList[i].wayCountRange.max);
            // 射出角度
            float angle = (float)Level.GetLevelRange(processList[i].angleRange.max, processList[i].angleRange.min);

            // タイマーが終了したら
            if (timer.IsFinished())
            {
                // 射出位置
                Vector3 pos = transform.position;

                
                // Targetを見るか
                if (processList[i].isLookTarget)
                {
                    //* 角度の計算 *//
                    // Targetへの向き
                    Vector2 direction_to_target = m_Target.transform.position - pos;

                    // ランダムな座標から撃つか
                    if (!processList[i].isSniping)
                    {
                        directionList[i] = Mathf.Atan2(direction_to_target.y, direction_to_target.x);
                        directionList[i] *= Mathf.Rad2Deg;

                        // 全方位に弾を撃つなら
                        if (processList[i].isAllRange)
                        {
                            for (int k = 0; k < wayCount; ++k)
                            {
                                m_BulletManager.Create(m_Target, unitLayer, pos, directionList[i] + (360f / wayCount) * k, m_BulletData.GetBulletAt((int)presetList[i].bullet), presetList[i], processList[i]);
                            }
                        }
                        // 指定した方向に弾を撃つ
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
                        // 狙撃位置
                        Vector3 snipingPosition = Vector2.zero;

                        // 画面の上端・下端で、プレイヤーの対角線上のx座標から出現
                        snipingPosition.x = -m_Target.transform.position.x + Random.Range(-2f, 2f);
                        snipingPosition.y = Camera.main.orthographicSize * (m_Target.transform.position.y < 0f ? 1f : -1f);

                        // Targetへの向きを再計算
                        Vector2 toTarget = m_Target.transform.position - snipingPosition;
                        directionList[i] = Mathf.Atan2(toTarget.y, toTarget.x);
                        directionList[i] *= Mathf.Rad2Deg;

                        // 全方位に弾を撃つなら
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
                // Targetを見ない
                else
                {
                    // 全方位に弾を撃つなら
                    if (processList[i].isAllRange)
                    {
                        for (int k = 0; k < wayCount; ++k)
                        {
                            m_BulletManager.Create(m_Target, unitLayer, pos, directionList[i] + (360f / wayCount) * k, m_BulletData.GetBulletAt((int)presetList[i].bullet), presetList[i], processList[i]);
                        }
                    }
                    // 指定した方向に弾を撃つ
                    else
                    {
                        float firstAngle = directionList[i] - ((wayCount - 1) * angle) / 2;
                        for (int m = 0; m < wayCount; ++m)
                        {
                            m_BulletManager.Create(m_Target, unitLayer, pos, firstAngle + m * angle, m_BulletData.GetBulletAt((int)presetList[i].bullet), presetList[i], processList[i]);
                        }
                    }

                    //射出角度を増やす
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

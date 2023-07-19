using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Unit
{
    public override string unitLayer => "Player";

    [SerializeField] Vector3 m_MuzzleOffset;

    protected List<int> m_PresetIndex = new List<int>();                  // Preset用インデックス 
    protected List<int> m_ProcessIndex = new List<int>();                  // Process用インデックス

    List<BulletPreset> m_BulletPreset = new List<BulletPreset>();         // BulletPreset用リスト
    List<BulletProcess> m_BulletProcess = new List<BulletProcess>();        // BulletProcess用リスト

    protected override void Start()
    {
        base.Start();
        m_TargetRenderer = m_SpriteRenderer;

        // StatusとProcessをそれぞれのリストに代入
        m_BulletPreset = m_BulletData.GetBulletPresetList();
        m_BulletProcess = m_BulletData.GetBulletProcessList();

        for (int i = 0; i < m_BulletProcess.Count; i++)
            m_DirectionList.Add(m_BulletProcess[i].direction);

        stock = TemporarySavingBounty.life;
        m_PresetIndex = TemporarySavingBounty.equipWeapon.presetIndex;

        // Debug.Log(nameof(m_PresetIndex.Count) + ": " + m_PresetIndex.Count);

        for( int i = 0; i < m_PresetIndex.Count; ++i)
        {
            m_ProcessIndex.Add(m_BulletPreset[m_PresetIndex[i]].processIndex);

            woskni.Timer timer = new woskni.Timer();

            // minIntervalとmaxIntervalでランダム値をとる
            float time = Level.GetLevelRange(m_BulletPreset[m_PresetIndex[i]].injectionInterval.max, m_BulletPreset[m_PresetIndex[i]].injectionInterval.min);
            // タイマーのセットアップ
            timer.Setup(time);
            timer.Fin();

            // タイマーの追加
            m_CreateTimerList.Add(timer);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //非死亡時のみ弾発射
        if (!deadFlag && woskni.InputManager.IsButton() && !DebugPlayer.isDebug)
            Fire();
    }
    public override void Damage(int damage, float invincibleTime)
    {
        base.Damage(damage, invincibleTime);

        if (!deadFlag)
            woskni.ShakeCamera.Shake(0.5f, 0.5f);
    }

    void Fire()
    {
        for (int i = 0; i < m_CreateTimerList.Count; ++i)
        {
            woskni.Timer timer = m_CreateTimerList[i];
            timer.Update();

            // 射出処理
            if (timer.IsFinished())
            {
                timer.Reset();

                // 射出音再生
                woskni.SoundManager.Play("銃撃", false, SaveSystem.m_SaveData.SEvolume, Random.Range(0.8f, 1.2f));

                // 何ウェイ出すか
                int wayCount = (int)Level.GetLevelRange(m_BulletProcess[m_ProcessIndex[i]].wayCountRange.min, m_BulletProcess[m_ProcessIndex[i]].wayCountRange.max);
                // 射出角度
                float angle = (float)Level.GetLevelRange(m_BulletProcess[m_ProcessIndex[i]].angleRange.max, m_BulletProcess[m_ProcessIndex[i]].angleRange.min);

                int preset_index  = m_PresetIndex[i];
                int process_index = m_ProcessIndex[i];

                // 射出位置
                Vector3 pos = transform.position;

                if (m_BulletProcess[m_ProcessIndex[i]].isLookTarget)
                {
                    // ターゲットの向き
                    Vector2 directionToTarget = m_Target.transform.position - pos;
                    float dir = Mathf.Atan2(directionToTarget.y, directionToTarget.x);

                    dir *= Mathf.Rad2Deg;

                    // 全方位に弾を出す
                    if (m_BulletProcess[m_ProcessIndex[i]].isAllRange)
                    {
                        for (int k = 0; k < wayCount; ++k)
                        {
                            m_BulletManager.Create(m_Target, unitLayer, pos, dir + (360f / wayCount) * k, m_BulletData.GetBulletAt((int)m_BulletPreset[preset_index].bullet), m_BulletPreset[preset_index], m_BulletProcess[process_index]);
                        }
                    }
                    else
                    {
                        float basisAngle = dir - ((float)(wayCount - 1) * angle) / 2f;

                        for (int k = 0; k < wayCount; ++k)
                        {
                            m_BulletManager.Create(m_Target, unitLayer, pos, basisAngle + k * angle, m_BulletData.GetBulletAt((int)m_BulletPreset[preset_index].bullet), m_BulletPreset[preset_index], m_BulletProcess[process_index]);
                        }
                    }
                }
                else
                {
                    // bulletProcessで指定した角度を入れる
                    float dir = m_DirectionList[i];

                    // 全方位に弾を出す
                    if (m_BulletProcess[m_ProcessIndex[i]].isAllRange)
                    {
                        for (int m = 0; m < wayCount; ++m)
                        {
                            m_BulletManager.Create(m_Target, unitLayer, pos, dir + (360f / wayCount) * m, m_BulletData.GetBulletAt((int)m_BulletPreset[preset_index].bullet), m_BulletPreset[preset_index], m_BulletProcess[process_index]);
                        }
                    }
                    // 全方位に弾を出さない
                    else
                    {
                        float basisAngle = dir - ((float)(wayCount - 1) * angle) / 2f;

                        for (int n = 0; n < wayCount; ++n)
                        {
                            m_BulletManager.Create(m_Target, unitLayer, pos, basisAngle + n * angle, m_BulletData.GetBulletAt((int)m_BulletPreset[preset_index].bullet), m_BulletPreset[preset_index], m_BulletProcess[process_index]);
                        }
                    }
                }

                // 射出方向の回転処理
                for (int k = 0; k < m_BulletProcess.Count; k++)
                {
                    if (m_BulletProcess[m_ProcessIndex[i]].rotateDirection.min != 0f && m_BulletProcess[m_ProcessIndex[i]].rotateDirection.max != 0f)
                        m_DirectionList[k] += Level.GetLevelRange(m_BulletProcess[m_ProcessIndex[i]].rotateDirection.max, m_BulletProcess[m_ProcessIndex[i]].rotateDirection.min) / 2f;
                }
            }

            m_CreateTimerList[i] = timer;
        }
    }
}

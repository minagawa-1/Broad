using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlYen : MonoBehaviour
{
    [SerializeField, woskni.Name("�㉺�L�[�ł̑����z")] int m_AddYenStrength = 1000;
    [SerializeField, woskni.Name("���E�L�[�ł̑����z")] int m_AddYenWeakness = 1000;

    [SerializeField] float m_AddInterval = 0f;

    woskni.Timer m_AddTimer;

    private void Start()
    {
        m_AddTimer.Setup(m_AddInterval);
    }

    // Update is called once per frame
    void Update()
    {
        /*if (woskni.KeyBoard.GetAndKey(KeyCode.UpArrow   , KeyCode.LeftControl))
            AddYen(m_AddYenStrength);

        if (woskni.KeyBoard.GetAndKey(KeyCode.DownArrow , KeyCode.LeftControl))
            AddYen(-m_AddYenStrength);

        if (woskni.KeyBoard.GetAndKey(KeyCode.RightArrow, KeyCode.LeftControl))
            AddYen(m_AddYenWeakness);

        if (woskni.KeyBoard.GetAndKey(KeyCode.LeftArrow, KeyCode.LeftControl))
            AddYen(-m_AddYenWeakness);*/
    }

    void AddYen(int add)
    {
        m_AddTimer.Update(false);

        if (m_AddTimer.IsFinished())
        {
            m_AddTimer.Reset();

            SaveSystem.m_SaveData.money += add;

            // 0�����ɂȂ�Ȃ��悤�ɂ���
            SaveSystem.m_SaveData.money = Mathf.Max(0, SaveSystem.m_SaveData.money);

            // �I�[�o�[�t���[�h�~
            SaveSystem.m_SaveData.money = Mathf.Min(999999999, SaveSystem.m_SaveData.money);

            SaveSystem.Save();
        }
    }

}

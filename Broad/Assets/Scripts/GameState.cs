using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public partial class GameManager
{
    enum State
    {
        Draw,           // ドロー
        Placement,      // 配置
        Wait,           // 待機
        Set,            // 設置
        SortHierarchy,  // ソート
        CalcBroad,      // 計算
        Finish          // 終了
    }
    State m_State;

    public void Update()
    {
        switch (m_State)
        {
            case State.Draw:            Draw();             break;
            case State.Placement:       Placement();        break;
            case State.Wait:            Wait();             break;
            case State.Set:             Set();              break;
            case State.SortHierarchy:   SortHierarchy();    break;
            case State.CalcBroad:       CalcBroad();        break;
            case State.Finish:          Finish();           break;
        }
        
    }

    void Draw()
    {

    }

    /// <summary>配置</summary>
    void Placement()
    {
    }

    /// <summary>待機</summary>
    void Wait()
    {
    }

    void Set()
    {
    }

    void SortHierarchy()
    {
        
    }

    void CalcBroad()
    {

    }

    void Finish()
    {

    }
}

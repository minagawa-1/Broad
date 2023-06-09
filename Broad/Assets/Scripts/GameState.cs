using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameManager
{
    enum State
    {
        HandOutBlocks,
        Placement,
        Wait,
        Set,
        SortHierarchy,
        CalcBroad,
        Finish
    }
    State m_State;

    public void Update()
    {
        switch (m_State)
        {
            case State.HandOutBlocks:   HandOutBlocks();    break;
            case State.Placement:       Placement();        break;
            case State.Wait:            Wait();             break;
            case State.Set:             Set();              break;
            case State.SortHierarchy:   SortHierarchy();    break;
            case State.CalcBroad:       CalcBroad();        break;
            case State.Finish:          Finish();           break;
        }
        
    }

    void HandOutBlocks()
    {

    }

    void Placement()
    {
        
    }

    void Wait()
    {

    }

    void Set()
    {

    }

    void SortHierarchy()
    {
        //bool lastTurn = false;
        //m_State = lastTurn ? State.CalcBroad : State.Placement;
    }

    void CalcBroad()
    {

    }

    void Finish()
    {

    }
}

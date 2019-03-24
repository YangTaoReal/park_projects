using QTFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState_ReadyComponent : StateBase
{
    public override void Init()
    {
        base.Init();
        Log.Info("LC_GameState_Ready", "初始化");
    }

    public override void StateStart(StateMachineBase kStateMachine)
    {
        base.StateStart(kStateMachine);
    }

    public override StateProcessResult StateProcess(StateMachineBase kStateMachine)
    {
        return StateProcessResult.Hold;
    }

    public override void StateEnd(StateMachineBase kStateMachine)
    {
        base.StateEnd(kStateMachine);
    }
}

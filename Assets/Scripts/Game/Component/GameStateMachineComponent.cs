/***********************************************
 * 游戏状态机
 * 启动 登陆 创建角色 选择服务器 加载 大厅 战斗等状态的管理
 * author:SmartCoder
 **********************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QTFramework
{
    [ObjectEventSystem]
    public class GameStateMachineComponentAwakeSystem : AAwake<GameStateMachineComponent>
    {
        public override void Awake(GameStateMachineComponent self)
        {
            self.Awake();
        }
    }

    [ObjectEventSystem]
    public class GameStateMachineComponentStartSystem : AStart<GameStateMachineComponent>
    {
        public override void Start(GameStateMachineComponent self)
        {
            self.Start();
        }
    }

    [ObjectEventSystem]
    public class GameStateMachineComponentFixedUpdateSystem : AFixedUpdate<GameStateMachineComponent>
    {
        public override void FixedUpdate(GameStateMachineComponent self)
        {
            self.FixedUpdate();
        }
    }

    public class GameStateMachineComponent : StateMachineBase
    {
        public StateBase m_eNextStateWhenLoadingFinish { get; set; }

        public void Awake()
        {
            Log.Info("GameStateMachineComponent", "游戏状态机组件挂载");
            AddComponent<GameState_ReadyComponent>();
            AddComponent<GameState_LoadingComponent>();
            AddComponent<GameState_PlayingComponent>();

            InitStateMachine(GetComponent<GameState_ReadyComponent>());
        }

        public void Start()
        {
            StartMachine();
        }

        public void FixedUpdate()
        {
            UpdateLogic();
        }

        public override void Reset()
        {
            base.Reset();
            m_eNextStateWhenLoadingFinish = null;
        }

        //public void ChangeState(StateBase _nextStateBase, StateBase _LoadingFinishGameState = null)
        //{
        //    m_eNextStateWhenLoadingFinish = _LoadingFinishGameState;
        //    ForcedToChangeState(_nextStateBase);
        //}

        public void SetLoadingFinishState<T>() where T : StateBase
        {
            m_eNextStateWhenLoadingFinish = GetComponent<T>();
        }
        public void ChangeState<T>() where T: StateBase
        {
            ForcedToChangeState(GetComponent<T>());
        }
    }
}
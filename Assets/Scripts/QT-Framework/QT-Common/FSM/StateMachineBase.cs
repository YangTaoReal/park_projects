/***********************************************
 * 状态机基类
 * 抽象状态的基础
 * author:SmartCoder
 **********************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QTFramework
{
    /// <summary>
    /// 状态机进程状态
    /// </summary>
    public enum StateMachineSateStage
    {
        /// <summary>
        /// 状态机默认值
        /// </summary>
        None = -1,
        /// <summary>
        /// 初始化
        /// </summary>
        Init,
        /// <summary>
        /// 开始
        /// </summary>
        Start,
        /// <summary>
        /// 结束
        /// </summary>
        End,
    }

    /// <summary>
    /// 状态机基类
    /// </summary>
    public abstract class StateMachineBase : QTEntity
    {
        /// <summary>
        ///状态机默认启动时执行的状态元类型
        /// </summary>
        public StateBase m_eDefaultState{ get; set; }

        /// <summary>
        /// 下一个状态元（当前状态元结束后执行的）
        /// </summary>
        public StateBase m_eNextState{ get; set; }

        /// <summary>
        /// 当前状态机执行的状态元
        /// </summary>
        public StateBase m_eCurrentState { get; set; }


        /// <summary>
        /// 初始化状态机
        /// </summary>
        public virtual void InitStateMachine(StateBase _GameState)
        {
            Reset();
            m_eDefaultState = _GameState;
 
        }

        /// <summary>
        /// 重置
        /// </summary>
        public virtual void Reset()
        {
            m_eNextState = null;
            m_eCurrentState = null;
        }

        public virtual void StartMachine()
        {
            m_eCurrentState = m_eDefaultState;
            m_eCurrentState.StateStart(this);
        }

        /// <summary>
        /// 逻辑帧更新
        /// </summary>
        /// <param name="_uiCrrentTime"></param>
        /// <param name="_uiDeltaTime"></param>
        public override void UpdateLogic()
        {
            base.UpdateLogic();

            if (CheckStateProcessCanFinish())
            {
                TransferState();
            }
        }

        /// <summary>
        /// 设置下个状态(等待当前状态元结束时执行)
        /// </summary>
        /// <param name="iStateType"></param>
        public void SetNextState(StateBase _GameState)
        {
            if (m_eNextState == _GameState)
            {
                return;
            }
            m_eNextState = _GameState;
        }


        /// <summary>
        /// 强行进入一个状态
        /// </summary>
        /// <param name="_eStateType"></param>
        /// <param name="_uiCrrentTime"></param>
        /// <param name="_uiDeltaTime"></param>
        protected void ForcedToChangeState(StateBase _eStateBase)
        {

            EndCurState();

            StartNewState(_eStateBase);

        }

        /// <summary>
        /// 开始新的状态元
        /// </summary>
        /// <param name="kState"></param>
        /// <param name="_uiCrrentTime"></param>
        /// <param name="_uiDeltaTime"></param>
        protected  void StartNewState(StateBase kState)
        {
            m_eCurrentState = kState;
            m_eCurrentState.StateStart(this);
        }

        /// <summary>
        /// 检查状态元的进程状态
        /// </summary>
        /// <param name="_uiCrrentTime"></param>
        /// <param name="_uiDeltaTime"></param>
        /// <returns></returns>
        protected  bool CheckStateProcessCanFinish()
        {
            StateProcessResult _eCurrentStateProcessResult = StateProcessResult.CanFinish;

            if (null != m_eCurrentState)
            {
                _eCurrentStateProcessResult = m_eCurrentState.StateProcess(this);
            }

            switch (_eCurrentStateProcessResult)
            {
                case StateProcessResult.Finish:
                    return true;
                case StateProcessResult.CanFinish:
                case StateProcessResult.CanBreak:
                    if (m_eNextState!= null && m_eNextState!= m_eCurrentState)
                    {
                        return true;
                    }
                    break;
            }
            return false;
        }

        /// <summary>
        /// 改变状态
        /// </summary>
        /// <param name="_uiCrrentTime"></param>
        /// <param name="_uiDeltaTime"></param>
        protected  void TransferState()
        {
            EndCurState();

            StartNewState(m_eNextState);
        }

        /// <summary>
        /// 结束状态元
        /// </summary>
        /// <param name="fCurrentTime"></param>
        /// <param name="fDeltaTime"></param>
        protected  void EndCurState()
        {
            m_eCurrentState.StateEnd(this);
        }
    }
}
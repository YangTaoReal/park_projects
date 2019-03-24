/***********************************************
 * 状态元基类
 * 抽象每种状态的基础流程
 * author:SmartCoder
 **********************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QTFramework
{

    /// <summary>
    /// 状态元执行进程
    /// </summary>
    public enum StateStage : int
    {
        /// <summary>
        /// 默认值
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
        /// 持续执行
        /// </summary>
        Process,
        /// <summary>
        /// 结束
        /// </summary>
        End,
    }

    /// <summary>
    /// 状态元持续过程的返回值
    /// </summary>
    public enum StateProcessResult : int
    {
        /// <summary>
        /// 默认值
        /// </summary>
        None = -1,
        /// <summary>
        /// 持续不结束
        /// </summary>
        Hold,
        /// <summary>
        /// 可结束（如果有下一个状态）
        /// </summary>
        CanFinish,
        /// <summary>
        /// 立即结束
        /// </summary>
        Finish,
        /// <summary>
        /// 可被打断
        /// </summary>
        CanBreak,
    }



    public class StateBase:QTComponent
    {
        /// <summary>
        /// 当前状态元阶段
        /// </summary>
        public StateStage m_eStateStage { get; set; }

        /// <summary>
        /// 状态元初始化
        /// </summary>
        public virtual void Init()
        {
            m_eStateStage = StateStage.Init;
        }

        /// <summary>
        /// 状态元开始执行
        /// </summary>
        /// <param name="kStateMachine"></param>
        /// <param name="fCurrentTime"></param>
        /// <param name="fDeltaTime"></param>
        public virtual void StateStart(StateMachineBase kStateMachine)
        {
            m_eStateStage = StateStage.Start;
        }

        /// <summary>
        /// 状态元持续执行
        /// </summary>
        /// <param name="kStateMachine"></param>
        /// <param name="fCurrentTime"></param>
        /// <param name="fDeltaTime"></param>
        /// <param name="fStateProcessTime"></param>
        /// <returns></returns>
        public virtual StateProcessResult StateProcess(StateMachineBase kStateMachine)
        {
            m_eStateStage = StateStage.Process;
            return StateProcessResult.CanFinish;
        }

        /// <summary>
        /// 状态元结束
        /// </summary>
        /// <param name="kStateMachine"></param>
        /// <param name="fCurrenTime"></param>
        /// <param name="fDeltaTime"></param>
        public virtual void StateEnd(StateMachineBase kStateMachine)
        {
            m_eStateStage = StateStage.End;
        }

    }
}
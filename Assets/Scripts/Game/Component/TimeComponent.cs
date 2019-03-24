using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


 

namespace QTFramework
{
 
    [ObjectEventSystem]
    public class TimeComponentAwakeSystem : AAwake<TimeComponent>
    {
        public override void Awake(TimeComponent _self)
        {
            _self.Awake();
        }
    }

    [ObjectEventSystem]
    public class TimeComponentFixedUpdateSystem : AFixedUpdate<TimeComponent>
    {
        public override void FixedUpdate(TimeComponent _self)
        {
            _self.FixedUpdate();
        }
    }

 
    //public delegate void AnimEvent(AnimData animationData);
    //public event AnimEvent OnAnimationBegin;

 
    public class TimerClass
    {
        public Guid guid;
        public TimeComponent.TimerCallback callback;
        public int loop;//-1是无限循环
        public int delayStart;//延迟开始时间
        public int delayTime;//间隔时间(毫秒)
        public int saveTime;//存在时间(毫秒)
        public int count;//已经循环了多少次
    }

    public class TimeComponent : QTComponent
    {
        List<TimerClass> m_lTimer = new List<TimerClass>();
 

        public delegate void TimerCallback();
        DateTime startTime;
        int lCount = 1;


        public void Awake()
        {
            Log.Info("TimeComponent", "时间组件挂载");
            m_lTimer.Clear();
            startTime = DateTime.Now;
            lCount = 1;
        }

        public void FixedUpdate()
        {
            if (m_lTimer.Count == 0)
                return;

            TimeSpan ts = DateTime.Now.Subtract(startTime);
            if (ts.TotalSeconds * 10 >= lCount)//100毫秒执行一次
            {
                lCount++;
                for (int i = m_lTimer.Count - 1; i >= 0; i--)
                {
                    if(lCount * 100 >= m_lTimer[i].delayStart) 
                    {
                        m_lTimer[i].saveTime = m_lTimer[i].saveTime + 100;
                        if (m_lTimer[i].saveTime >= m_lTimer[i].delayTime)
                        {
                            if (m_lTimer[i].callback != null)
                                m_lTimer[i].callback();

                            m_lTimer[i].count++;
                            m_lTimer[i].saveTime = 0;
                            if (m_lTimer[i].loop == -1)//无限循环
                            {

                            }
                            else
                            {
                                if (m_lTimer[i].count >= m_lTimer[i].loop)
                                {
                                    RemoveTimer(m_lTimer[i].guid);
                                }
                            }
 

                        } 
                    }


    

                }

 
            }

        
        }

    

        /// <summary>
        /// 客户端和服务器时间偏移
        /// </summary>
        private ulong ulTimeOffset = 0;

        public float RenderProcessTime
        {
            get { return Time.realtimeSinceStartup; }
        }
        /// <summary>
        /// 逻辑进程时间
        /// </summary>
        public float LogicProcessTime
        {
            get { return Time.fixedTime; }
        }
        /// <summary>
        /// 逻辑帧间隔
        /// </summary>
        public float LogicProcessDeltaTime
        {
            get;
            set;
        }

        /// <summary>
        /// 服务器同步时的服务器时间(标准格林尼治时间)
        /// </summary>
        private ulong serverTime = 0;
        public ulong ServerTime
        {
            get
            {
                return GetClientTime() + ulTimeOffset;
            }
            set
            {
                serverTime = value;
                ulTimeOffset = serverTime - GetClientTime();
            }
        }

        private ulong GetClientTime()
        {
            DateTime startTime = System.TimeZoneInfo.ConvertTime(new System.DateTime(1970, 1, 1),TimeZoneInfo.Local);
            ulong timeStamp = (ulong)(DateTime.Now - startTime).TotalSeconds;
            return timeStamp;
        }


        public Double DiffTimeSeconds(DateTime t1, DateTime t2)
        {
            TimeSpan ts = t1.Subtract(t2);
            return ts.TotalSeconds;
        }

        //delaystart 延迟开始时间
        public Guid CreateTimer(int delaystart, int delay, int loop = 1, TimerCallback cb = null)
        {
            TimerClass timerClass = new TimerClass();
            timerClass.guid = GenerateID.ID;
            timerClass.delayTime = delay;
            timerClass.delayStart = delaystart;
            timerClass.loop = loop;
            timerClass.callback = cb;
            timerClass.saveTime = 0;//存在时间(毫秒)
            timerClass.count = 0;//已经循环了多少次
 



            m_lTimer.Add(timerClass);

            return timerClass.guid;
        }

        public void RemoveTimer(Guid guid)
        {
            for (int i = 0; i < m_lTimer.Count; i++)
            {
                if(m_lTimer[i].guid == guid)
                {
                    m_lTimer.Remove(m_lTimer[i]);
                    return;
                }
            }
        }

    }
}
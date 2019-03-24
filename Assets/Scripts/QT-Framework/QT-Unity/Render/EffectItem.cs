
using System.Collections.Generic;
using UnityEngine;
using System;

namespace QTFramework
{
    /// <summary>
    /// 管理一个完整特效(可能有很多子特效组件)
    /// </summary>
    public class EffectItem : MonoBehaviour
    {
        private Guid effectID;
        private float durationTime;     // 特效持续时间 固有属性
        private float currTime;         // 播放了多久
        private bool isPlay;            // 是否播放
        private bool isLoop;            // 循环的话  就不计较时间了
        private List<ParticleSystem> particleList = new List<ParticleSystem>();

        public float DurationTime
        {
            get { return durationTime; }
            set
            {
                durationTime = value;
            }
        }

        public Guid EffectID
        {
            get { return effectID; }
            set { effectID = value; }
        }

        private void Awake()
        {
            gameObject.SetActive(false);
            GetComponentsInChildren<ParticleSystem>(true, particleList);
            Init();
        }

        private void Init()
        {
            if(particleList.Count == 0)
            {
                Debug.LogError("no particleSystem");
                return;
            }

            CalcDurationTime();
            AutoScale();
        }

        private void Update()
        {
            if(isPlay)
            {
                currTime += Time.deltaTime;
                //Debug.LogFormat("非循环特效，curr时间:{0},PlayTime:{1}",currTime,DurationTime);
                if(currTime > DurationTime && !isLoop)
                {
                    // 时间到了
                    //Debug.Log("非循环特效，时间到了，关闭特效");
                    Stop();
                }
            }
        }

        /// <summary>
        /// 根据ui类型不同 场景特效和UI特效 的缩放不同
        /// </summary>
        private void AutoScale()
        {
            //if(gameObject.layer == 5)
            //{
            //    Debug.Log("ui层，需要缩放大小");
            //    for (int i = 0; i < particleList.Count; i++)
            //    {
            //        particleList[i].transform.localScale = particleList[i].transform.localScale * 100;
            //    }
            //}
        }

        /// <summary>
        /// 遍历所有particleSystem 计算出整个特效的有效时间
        /// </summary>
        private void CalcDurationTime()
        {
            for (int i = 0; i < particleList.Count; i++)
            {
                ParticleSystem p = particleList[i];
                float time = 0;
                if (!p.main.loop)
                {
                    if (p.emission.enabled)
                        time = p.main.duration + p.main.startDelay.constantMax + p.main.startLifetime.constantMax;
                    else
                        time = p.main.startDelay.constantMax + p.main.startLifetime.constantMax;
                }
                else
                {
                    isLoop = true;
                    break;
                }
                if (time > DurationTime)
                    DurationTime = time;
            }
            //Debug.Log("DurationTIme = " + DurationTime);
        }

        /// <summary>
        /// 播放特效
        /// </summary>
        public void Play()
        {
            isPlay = true;
            gameObject.SetActive(true);
        }

        /// <summary>
        /// 特效播放完毕
        /// </summary>
        public void Stop()
        {
            //Debug.LogFormat("关闭特效{0}，回收到池里",EffectID);
            StopAllParticles();
            currTime = 0;
            isPlay = false;
            gameObject.SetActive(false);
            // 通知manager removem item
            World.Scene.GetComponent<EffectManagerComponent>().RemoveItemById(EffectID);
            // 回收利用
            AssetPoolManager.Instance.Recycle(gameObject);
        }

        /// <summary>
        /// 关闭列表中所有的particle特效
        /// </summary>
        private void StopAllParticles()
        {
            for (int i = 0; i < particleList.Count; i++)
            {
                particleList[i].Stop();
            }
        }
    }
}

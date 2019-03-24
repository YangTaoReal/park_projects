/*************************************
 * 音效单体载体
 * 功能：AudioSource 的播放和关闭
 * author:SmartCoder
**************************************/

using UnityEngine;

namespace QTFramework
{

    [RequireComponent(typeof(AudioSource))]
    public class AudioItem : MonoBehaviour
    {
        /// <summary>
        /// 音效播放源
        /// </summary>
        public AudioSource m_kSource
        {
            get;
            set;
        }
        /// <summary>
        /// 音效资源
        /// </summary>
        public string m_kAudioRes
        {
            get;
            set;
        }
        /// <summary>
        /// 音频结束时间
        /// </summary>
        public float EndTime
        {
            get;
            set;
        }
        /// <summary>
        /// 是否循环
        /// </summary>
        public bool Loop
        {
            get;
            set;
        }
        /// <summary>
        /// 音频文件
        /// </summary>
        private AudioClip m_kClip = null;
        /// <summary>
        /// 音频开始播放时间
        /// </summary>
        private float m_fStartTime = 0;
        /// <summary>
        /// 播放次数
        /// </summary>
        private int m_iLoopPlayCount = 1;
        /// <summary>
        /// 该音频属于哪个频道
        /// </summary>
        private AudioChannel.AudioChannelType audioChannelType;


        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="_audioChannelType"></param>
        /// <param name="_audioName"></param>
        /// <param name="_loop"></param>
        /// <param name="_loopPlayCount"></param>
        public void Init(AudioChannel.AudioChannelType _audioChannelType, string _audioName,float _volume, bool _loop = false, int _loopPlayCount = 1)
        {
            m_kSource = gameObject.GetComponent<AudioSource>();
            m_kAudioRes = _audioName;
            m_kClip = AssetPoolManager.Instance.Fetch(m_kAudioRes) as AudioClip;
            audioChannelType = _audioChannelType;

            Loop = _loop;
            m_iLoopPlayCount = _loopPlayCount;
            m_kSource.playOnAwake = false;
            m_kSource.volume = _volume;
            m_kSource.loop = Loop;
        }

        /// <summary>
        /// 播放
        /// </summary>
        /// <param name="currentTime"></param>
        /// <returns></returns>
        public void Play()
        {
            TimeComponent timeComponent = World.Scene.GetComponent<TimeComponent>();
            m_fStartTime = timeComponent.LogicProcessTime;
            EndTime = timeComponent.LogicProcessTime;
            if (m_kClip != null)
            {
                EndTime = m_fStartTime + (m_iLoopPlayCount * m_kClip.length) + 0.5f;
                m_kSource.clip = m_kClip;
                m_kSource.Play();
            }
        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            if (m_kSource != null)
            {
                m_kSource.Stop();
            }

            if (m_kClip != null)
            {
                AssetPoolManager.Instance.Recycle(m_kClip);
                m_kClip = null;
            }
            AssetPoolManager.Instance.Recycle(gameObject);
        }

        public void SetAudioVolume(float _volume)
        {
            m_kSource.volume = _volume;
        }



    }

}

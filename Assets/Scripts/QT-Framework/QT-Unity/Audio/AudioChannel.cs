/*************************************
 * 音效频道
 * 功能：频道的声音大小和播放停止
 * author:SmartCoder
**************************************/

using QTFramework;
using System.Collections.Generic;
using UnityEngine;


public class AudioChannel
{
    /// <summary>
    /// 声音通道类型
    /// </summary>
    public enum AudioChannelType
    {
        BGM = 1,//背景音乐
        Voice = 2,//语音通道
        SoundEffect = 3,//战斗相关的音效比如技能音效、脚步声、战斗场景中的
    }

    /// <summary>
    /// 音频通道
    /// </summary>
    private AudioChannelType m_kAudioChannelType;
    /// <summary>
    /// 音频
    /// </summary>
    private List<AudioItem> m_kAudioItemList = new List<AudioItem>(20);
    /// <summary>
    /// 零时公用
    /// </summary>
    private AudioItem tempAudio = null;


    public AudioChannel(AudioChannelType _audioChannelType)
    {
        m_kAudioChannelType = _audioChannelType;
    }

    /// <summary>
    /// 更新音效
    /// </summary>
    public void UpdateLogic()
    {
        TimeComponent timeComponent = World.Scene.GetComponent<TimeComponent>();
        for (int i = m_kAudioItemList.Count - 1; i >= 0; i--)
        {
            tempAudio = m_kAudioItemList[i];
            if (!tempAudio.Loop && tempAudio.EndTime < timeComponent.LogicProcessTime)
            {
                tempAudio.Stop();
                m_kAudioItemList.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// 播放
    /// </summary>
    /// <param name="audioName"></param>
    /// <param name="_loop"></param>
    /// <param name="_loopPlayCount"></param>
    /// <returns></returns>
    public void Play(string _sourceAudioPath,Transform _prentTransform, float _volume, bool _loop = false, int _loopPlayCount = 1)
    {
        GameObject _tempGameObject = AssetPoolManager.Instance.Fetch<GameObject>("Assets/Data/Model/Audio/AudioItem.prefab");
        if (_tempGameObject != null)
        {
            _tempGameObject.transform.SetParent(_prentTransform);
            tempAudio = _tempGameObject.GetComponent<AudioItem>();
            if (tempAudio == null)
            {
                tempAudio = _tempGameObject.AddComponent<AudioItem>();
            }
            tempAudio.Init(m_kAudioChannelType, _sourceAudioPath, _volume, _loop, _loopPlayCount);
            tempAudio.Play();
            m_kAudioItemList.Add(tempAudio);
        }
    }

    /// <summary>
    /// 停止
    /// </summary>
    /// <param name="kResName"></param>
    public void Stop(string kResName =null)
    {
        if (kResName == null)
        {
            for (int i = 0, iCount = m_kAudioItemList.Count; i < iCount; i++)
            {
                tempAudio = m_kAudioItemList[i];
                tempAudio.Stop();
                m_kAudioItemList.Clear();
            }
        }
        else
        {
            for (int i = m_kAudioItemList.Count-1; i >= 0;  i--)
            {
                tempAudio = m_kAudioItemList[i];
                if (tempAudio.m_kAudioRes == kResName)
                {
                    tempAudio.Stop();
                    m_kAudioItemList.Remove(tempAudio);
                }
            }
        } 
    }


    public void SetAudioVolume(float _volume)
    {
        for (int i = m_kAudioItemList.Count - 1; i >= 0; i--)
        {
            m_kAudioItemList[i].SetAudioVolume(_volume);
        }
        
    }
}
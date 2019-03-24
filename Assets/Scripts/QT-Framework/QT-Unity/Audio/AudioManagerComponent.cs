/*************************************
 * 音效管理
 * 功能：分类音效 播放
 * author:SmartCoder
**************************************/

using QTFramework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[ObjectEventSystem]
public class AudioManagerComponentAwakeSystem : AAwake<AudioManagerComponent>
{
    public override void Awake(AudioManagerComponent _self)
    {
        _self.Awake();
    }
}
[ObjectEventSystem]
public class AudioManagerComponentFixedUpdateSystem : AFixedUpdate<AudioManagerComponent>
{
    public override void FixedUpdate(AudioManagerComponent _self)
    {
        _self.FixedUpdate();
    }
}

public class AudioManagerComponent : QTComponent
{
    /// <summary>
    /// 日志TGA
    /// </summary>
    public static String TGA = "音效管理";
    /// <summary>
    /// 各个频道的音量
    /// </summary>
    public readonly Dictionary<AudioChannel.AudioChannelType, float> m_kDictionaryRate = new Dictionary<AudioChannel.AudioChannelType, float>();
    /// <summary>
    /// 通道
    /// </summary>
    private readonly Dictionary<AudioChannel.AudioChannelType, AudioChannel> m_kDictionaryAudioChannel = new Dictionary<AudioChannel.AudioChannelType, AudioChannel>();
    /// <summary>
    /// 初始化
    /// </summary>
    public void Awake()
    {
        Log.Info("AudioManagerComponent", "音效组件挂载");

        m_kDictionaryAudioChannel.Clear();
        m_kDictionaryRate.Clear();

  ;

        m_kDictionaryRate[AudioChannel.AudioChannelType.BGM] = PlayerPrefs.GetFloat("ToggleMusic",1);
        m_kDictionaryRate[AudioChannel.AudioChannelType.Voice] = PlayerPrefs.GetFloat("ToggleSound", 1);
        m_kDictionaryRate[AudioChannel.AudioChannelType.SoundEffect] = PlayerPrefs.GetFloat("ToggleSound", 1);

        m_kDictionaryAudioChannel[AudioChannel.AudioChannelType.BGM] = new AudioChannel(AudioChannel.AudioChannelType.BGM);
        m_kDictionaryAudioChannel[AudioChannel.AudioChannelType.Voice] = new AudioChannel(AudioChannel.AudioChannelType.Voice);
        m_kDictionaryAudioChannel[AudioChannel.AudioChannelType.SoundEffect] = new AudioChannel(AudioChannel.AudioChannelType.SoundEffect);
    }

    /// <summary>
    /// 更新
    /// </summary>
    public void FixedUpdate()
    {
        var enumChannel = m_kDictionaryAudioChannel.GetEnumerator();
        while (enumChannel.MoveNext())
        {
            enumChannel.Current.Value.UpdateLogic();
        }
    }

    /// <summary>
    /// 播放声音
    /// </summary>
    /// <param name="_audioChannelType">播放通道</param>
    /// <param name="_sourceAudioName">资源名</param>
    /// <param name="_loop">是否循环</param>
    /// <param name="_loopPlayCount">循环播放次数</param>
    public void PlayAudio(AudioChannel.AudioChannelType _audioChannelType, string _sourceAudioPath, bool _loop = false, int _loopPlayCount = 1)
    {
        m_kDictionaryAudioChannel[_audioChannelType].Play(_sourceAudioPath, World.Scene.GetComponent<WorldManagerComponent>().m_kAudioNode.transform, m_kDictionaryRate[_audioChannelType], _loop, _loopPlayCount);
    }

    /// <summary>
    /// 停止音效
    /// </summary>
    /// <param name="_audioChannelType"></param>
    /// <param name="_sourceAudioName"></param>
    public void StopAudio(AudioChannel.AudioChannelType _audioChannelType, string _sourceAudioName = null)
    {
        m_kDictionaryAudioChannel[_audioChannelType].Stop(_sourceAudioName);
    }

    public async void ChangeAudio(AudioChannel.AudioChannelType _audioChannelType, string _sourceAudioPath, bool _loop = false, int _loopPlayCount = 1)
    {
        float oldVolume = m_kDictionaryRate[_audioChannelType];
        for (int i = 1; i <= 10; i++)
        {
            await Task.Delay(70);
            m_kDictionaryAudioChannel[_audioChannelType].SetAudioVolume(oldVolume * (1 - i * 0.1f));
        }
        m_kDictionaryAudioChannel[_audioChannelType].Stop();
        m_kDictionaryAudioChannel[_audioChannelType].Play(_sourceAudioPath, World.Scene.GetComponent<WorldManagerComponent>().m_kAudioNode.transform, 0.1f, _loop, _loopPlayCount);
        for (int i = 1; i <= 10; i++)
        {
            await Task.Delay(70);
            m_kDictionaryAudioChannel[_audioChannelType].SetAudioVolume(oldVolume * (i * 0.1f));
        }

    }

    /// <summary>
    /// 全部停止
    /// </summary>
    public void StopAll()
    {
        var enumChannel = m_kDictionaryAudioChannel.GetEnumerator();
        while (enumChannel.MoveNext())
        {
            enumChannel.Current.Value.Stop();
        }
    }


    public async void SetMusicVolume(float _Volume)
    {

        float oldVolume = m_kDictionaryRate[AudioChannel.AudioChannelType.BGM];
        if (_Volume == 0)
        {
            for (int i = 1; i <= 10; i++)
            {
                await Task.Delay(100);
                m_kDictionaryAudioChannel[AudioChannel.AudioChannelType.BGM].SetAudioVolume(oldVolume * (1 - i * 0.05f));
            }

        }
        else
        {
            for (int i = 1; i <= 10; i++)
            {
                await Task.Delay(100);
                m_kDictionaryAudioChannel[AudioChannel.AudioChannelType.BGM].SetAudioVolume(i * 0.1f);
            }
        }
        m_kDictionaryRate[AudioChannel.AudioChannelType.BGM] = _Volume;
        m_kDictionaryAudioChannel[AudioChannel.AudioChannelType.BGM].SetAudioVolume(_Volume);
    }

    public async void SetVoiceVolume(float _Volume)
    {

        float oldVoiceVolume = m_kDictionaryRate[AudioChannel.AudioChannelType.BGM];
        float oldSoundEffectVolume = m_kDictionaryRate[AudioChannel.AudioChannelType.SoundEffect];
        if (_Volume == 0)
        {
            for (int i = 1; i <= 10; i++)
            {
                await Task.Delay(100);
                m_kDictionaryAudioChannel[AudioChannel.AudioChannelType.Voice].SetAudioVolume(oldVoiceVolume * (1 - i * 0.05f));
                m_kDictionaryAudioChannel[AudioChannel.AudioChannelType.SoundEffect].SetAudioVolume(oldSoundEffectVolume * (1 - i * 0.05f));
            }

        }
        else
        {
            for (int i = 1; i <= 10; i++)
            {
                await Task.Delay(100);
                m_kDictionaryAudioChannel[AudioChannel.AudioChannelType.Voice].SetAudioVolume((i * 0.1f));
                m_kDictionaryAudioChannel[AudioChannel.AudioChannelType.SoundEffect].SetAudioVolume(i * 0.1f);
            }
        }


        m_kDictionaryRate[AudioChannel.AudioChannelType.Voice] = _Volume;
        m_kDictionaryRate[AudioChannel.AudioChannelType.SoundEffect] = _Volume;

        m_kDictionaryAudioChannel[AudioChannel.AudioChannelType.Voice].SetAudioVolume(_Volume);
        m_kDictionaryAudioChannel[AudioChannel.AudioChannelType.SoundEffect].SetAudioVolume(_Volume);
    }
}

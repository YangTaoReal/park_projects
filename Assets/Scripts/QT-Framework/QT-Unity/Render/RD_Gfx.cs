using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RD_Gfx : MonoBehaviour
{
    private ParticleSystem[] m_kParticleSystemList;
    public bool m_kLoop;
    public float m_kParticleLifeTime;

    public float m_kStartPlayTime;

    private void Awake()
    {
        m_kParticleSystemList = GetComponentsInChildren<ParticleSystem>();

        for (int i = 0; i < m_kParticleSystemList.Length; i++)
        {
            float time = 0;
            m_kLoop = m_kParticleSystemList[i].main.loop;
            if (m_kParticleSystemList[i].main.loop)
            {

            }
            else
            {
                if (m_kParticleSystemList[i].emission.enabled)
                {
                    time = m_kParticleSystemList[i].main.duration + m_kParticleSystemList[i].main.startDelay.constant + m_kParticleSystemList[i].main.startLifetime.constantMax;
                }
                else
                {
                    time = m_kParticleSystemList[i].main.startDelay.constant + m_kParticleSystemList[i].main.startLifetime.constantMax;
                }
            }
            if (time > m_kParticleLifeTime)
            {
                m_kParticleLifeTime = time;
            }
            
        }

    }

    //激活特效
    public void Active()
    {
        this.gameObject.SetActive(true);
    }

    //关闭特效
    public void Deactive()
    {
        this.gameObject.SetActive(false);
        for (int i = 0; i < m_kParticleSystemList.Length; i++)
        {
            m_kParticleSystemList[i].Stop();
        }
    }

    // Update is called once per frame
    public void Play(float _renderTime)
    {
        Active();
        m_kStartPlayTime = _renderTime;
        for (int i = 0; i < m_kParticleSystemList.Length; i++)
        {
            m_kParticleSystemList[i].Play();
        }
    }

}

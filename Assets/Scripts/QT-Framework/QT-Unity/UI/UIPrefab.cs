/*************************************
 * view基类
 * 功能：界面基类
 * author:SmartCoder
 **************************************/
using QTFramework;
using System.Collections.Generic;
using UnityEngine;


public class UIPrefab : MonoBehaviour
{
    public string m_kOpenAudioClipPath = "";
    public string m_kCloseAudioClipPath = "";
    /// <summary>
    /// UI类型
    /// </summary>
    public UILayer m_kUIType = UILayer.Page;
    /// <summary>
    /// UI锚点
    /// </summary>
    public UIAnchorType m_kUIAnchorType = UIAnchorType.Anchor_Center;
    /// <summary>
    /// 节点列表
    /// </summary>
    public List<Component> m_kElements = new List<Component>();
    /// <summary>
    /// 当本UI打开的时候关闭其他打开界面，并且恢复以前的UI
    /// </summary>
    public bool m_kHideOtherUI_OnShow { get; private set; } = false;
    /// <summary>
    /// 界面层级排序
    /// </summary>
    public int m_kSortOrderLayer { get; private set; } = 0;
    /// <summary>
    /// 完整路径名称
    /// </summary>
    [HideInInspector]
    public string m_kUIFullPath { get; set; } = "";
    /// <summary>
    /// 完整路径名称
    /// </summary>
    [HideInInspector]
    public string m_kUIName { get; set; } = "";
    /// <summary>
    /// 界面动画
    /// </summary>
    [HideInInspector]
    public Animation m_kUEAnimation { get; set; } = null;
    /// <summary>
    /// 界面Canvas
    /// </summary>
    private Canvas m_kCanvas { get; set; } = null;
    /// <summary>
    /// RectTransform
    /// </summary>
    [HideInInspector]
    public RectTransform m_kGameObjectRectTransform { get { return transform.gameObject.GetComponent<RectTransform>(); } } 

    private void Awake()
    {
         m_kCanvas = transform.gameObject.GetComponent<Canvas>();
        m_kUEAnimation = transform.GetComponent<Animation>();
        m_kHideOtherUI_OnShow = m_kUIType == UILayer.Page;
    }

    public void OpenUI()
    {
        if (!string.IsNullOrEmpty(m_kOpenAudioClipPath))
        {
            World.Scene.GetComponent<AudioManagerComponent>().PlayAudio(AudioChannel.AudioChannelType.SoundEffect, m_kOpenAudioClipPath);
        }
    }

    public void CloseUI()
    {
        if (!string.IsNullOrEmpty(m_kCloseAudioClipPath))
        {
            World.Scene.GetComponent<AudioManagerComponent>().PlayAudio(AudioChannel.AudioChannelType.SoundEffect, m_kCloseAudioClipPath);
        }
    }

    /// <summary>
    /// 设置界面层级
    /// </summary>
    /// <param name="_SortOrderLayer"></param>
    public void UpdateSortOrderLayer(int _SortOrderLayer)
    {
        if (m_kCanvas != null)
        {
            m_kCanvas.sortingOrder = _SortOrderLayer;
        }
    }

    /// <summary>
    /// 获取缓存的GameObject
    /// </summary>
    /// <param name="index">索引</param>
    /// <returns>获取到的GameObject</returns>
    public GameObject GetCacheGameObject(int index)
    {
        if (m_kElements != null && m_kElements.Count > 0)
        {
            if (index < 0|| index >= m_kElements.Count || m_kElements[index] == null)
            {
                return null;
            }

            return m_kElements[index].gameObject;
        }

        return null;
    }
    /// <summary>
    /// 获取缓存的Transform
    /// </summary>
    /// <param name="index">索引</param>
    /// <returns>Transform</returns>
    public Transform GetCacheTransform(int index)
    {
        if (m_kElements != null && m_kElements.Count > 0)
        {
            if (index < 0 || index >= m_kElements.Count || m_kElements[index] == null)
            {
                return null;
            }
            GameObject go = m_kElements[index].gameObject;
            return go != null ? go.transform : null;
        }

        return null;
    }

    /// <summary>
    /// 通过index取Component
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public Component GetCacheComponent(int index)
    {
        if (m_kElements == null
            || index < 0
            || index >= m_kElements.Count
            || m_kElements[index] == null)
        {
            return null;
        }

        return m_kElements[index];
    }

    /// <summary>
    /// 通过index取Component
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public T GetCacheComponent<T>(int index) where T:Component
    {
        if (m_kElements == null
            || index < 0
            || index >= m_kElements.Count
            || m_kElements[index] == null)
        {
            return null;
        }

        return m_kElements[index] as T;
    }
}

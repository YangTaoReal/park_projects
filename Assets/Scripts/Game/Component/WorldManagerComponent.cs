/***********************************************************
 * 游戏世界管理
 * 负责游戏对象在场景中的添加删除
 * author:SmartCoder
 * *********************************************************/

using QTFramework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[ObjectEventSystem]
public class WorldManagerComponentAwakeSystem : AAwake<WorldManagerComponent, GameObject>
{
    public override void Awake(WorldManagerComponent _self,GameObject _parentGameObject)
    {
        _self.Awake(_parentGameObject);
    }
}

public class WorldManagerComponent : QTFramework.QTComponent
{
    /// <summary>
    /// 游戏场景根节点节点
    /// </summary>
    public GameObject m_kSceneRootNode;

    /// <summary>
    /// 游戏对象挂点
    /// </summary>
    public GameObject m_kActorNode { get; set; }
    public GameObject m_kAudioNode { get; set; }
    public GameObject m_kGfxNode { get; set; }
    public GameObject m_kSceneLogic { get; set; }

    public  Camera m_kGameCamera { get; set; }
    public  Camera m_kUICamera { get; set; }
    public  Transform m_kPoolNode { get; set; }
    public  Transform m_kUIRootNode { get; set; }
    public UnityEngine.EventSystems.EventSystem m_kEventSystem { get; set; }

    private readonly Dictionary<uint, LogicObject> m_DicLogicalObject = new Dictionary<uint, LogicObject>();

    public void Awake(GameObject _parentGameObject)
    {
        Log.Info("WorldManagerComponent", "世界管理组件挂载");

        m_kSceneRootNode    =   new GameObject("SceneRootNode");
        m_kActorNode        =   new GameObject("ActorNode");
        m_kGfxNode          =   new GameObject("GfxNode");
        m_kAudioNode        =   new GameObject("AudioNode");
        m_kSceneLogic       =   new GameObject("SceneLogic");
        m_kUIRootNode       =   GameObject.Find("UINode").transform;


        m_kGameCamera       =   GameObject.Find("SceneCamera").GetComponent<Camera>();
        // 适配屏幕处理
        if (m_kGameCamera.aspect > (2 / 3f))
        {
            m_kGameCamera.orthographicSize = 4.8f * (2 / 3f) / m_kGameCamera.aspect;
        }


        m_kUICamera         =   GameObject.Find("UICamera").GetComponent<Camera>();
        m_kEventSystem = GameObject.Find("EventSystem").GetComponent<UnityEngine.EventSystems.EventSystem>();

        m_kSceneRootNode.transform.SetParent(_parentGameObject.transform);
        m_kSceneRootNode.transform.SetAsFirstSibling();
        m_kActorNode.transform.SetParent(m_kSceneRootNode.transform);
        m_kAudioNode.transform.SetParent(m_kSceneRootNode.transform);
        m_kSceneLogic.transform.SetParent(m_kSceneRootNode.transform);
        m_kSceneLogic.AddComponent<SceneLogic>();

        m_kGfxNode.transform.SetParent(m_kSceneRootNode.transform);
        m_DicLogicalObject.Clear();
    }
    public void ClearSence()
    {
        var logicObject = m_DicLogicalObject.GetEnumerator();
        while (logicObject.MoveNext())
        {
            logicObject.Current.Value.Dispose();
        }
        m_DicLogicalObject.Clear();
    }

    public LogicObject AddLogicalObjectToSence(LogicObject _logicalObject)
    {
        m_DicLogicalObject[(uint)_logicalObject.m_uID] = _logicalObject;
        return _logicalObject;
    }

    public void RemoveLogicalObjectToSence(LogicObject _logicalObject)
    { 
        m_DicLogicalObject.Remove(_logicalObject.m_uID);
    }

    public LogicObject FindLogicalObjectByID(uint _logicalObjectID)
    {
        LogicObject _logicalObject = null;
        m_DicLogicalObject.TryGetValue(_logicalObjectID, out _logicalObject);
        return _logicalObject;
    }


}

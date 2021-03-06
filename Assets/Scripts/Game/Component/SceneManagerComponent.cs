/***********************************************************
 * 地图场景加载
 * 负责游戏场景的加载
 * author:SmartCoder
 * *********************************************************/

using UnityEngine;
using UnityEngine.SceneManagement;
using QTFramework;
using System;
using System.Collections.Generic;


[ObjectEventSystem]
public class SceneManagerComponentAwakeSystem : AAwake<SceneManagerComponent>
{
    public override void Awake(SceneManagerComponent self)
    {
        self.Awake();
    }
}

public class SceneManagerComponent : QTComponent
{
    /// <summary>
    /// 异步对象
    /// </summary>
    public AsyncOperation m_kAsyncScene { get; private set; }

    public void Awake()
    {
        Log.Info("SceneManagerComponent", "场景组件挂载");
    }

    /// <summary>
    /// 地图场景加载
    /// </summary>
    /// <param name="_SceneName">场景名字</param>
    /// <param name="_Action"></param>
    public void BeginLoad(string _SceneName, Action<AsyncOperation> _Action)
    {
        m_kAsyncScene = SceneManager.LoadSceneAsync(_SceneName, LoadSceneMode.Additive);
        m_kAsyncScene.completed += _Action;
    }
}

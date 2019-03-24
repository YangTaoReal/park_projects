using System.Collections;
using System.Collections.Generic;
using QTFramework;
using UnityEngine;
using System;
using UnityEngine.Events;
using BE;

[ObjectEventSystem]
public class GuidanceManagerAwakeSystem : AAwake<GuidanceManager>
{
    public override void Awake(GuidanceManager _self)
    {
        _self.Awake();
    }
}

[ObjectEventSystem]
public class GuidanceManagerUpdateSystem : AUpdate<GuidanceManager>
{
    public override void Update(GuidanceManager _self)
    {
        _self.Update();
    }
}

public class GuidanceManager : QTComponent
{
    
    public static GuidanceManager _Instance;
    public static bool isGuidancing = false;    // 是否在新手引导
    public static bool needSlide = false;
    public static bool needClick = true;
    public static GuidanceStep currStep = GuidanceStep.None;
    public static GuidanceEvent currEvent = GuidanceEvent.GreenHandEvent;

    public bool IsGuidanceReady = false;    // 必须从startguidance 中走才算新手引导
    private  bool isCanStartNextStep = false;
    public Dictionary<GuidanceStep, UnityAction> guidanceEventDic = new Dictionary<GuidanceStep, UnityAction>();
    public List<CS_Guidance.DataEntry> dataList = new List<CS_Guidance.DataEntry>();

    private UIEntity currEntity = null; // 当前打开的ui实体
    private Type currType;  // 当前打开的ui的组件

    public UnityAction currClickCallBack;   // 当前点击事件回调
    public UnityAction currSlideCallBack;   // 当前点击事件回调

    public GreenHandsGuidance greenHandsGuidance;

    public bool IsCanStartNextStep
    {
        get { return isCanStartNextStep; }
        set{
            isCanStartNextStep = value;
        }
    }
    public void Awake()
    {
        _Instance = this;
        AddEvent();


        ObserverHelper<GuidanceData>.AddEventListener(MessageMonitorType.GuidanceClickEvent, OnGuidanceClickEvent);
        ObserverHelper<GuidanceData>.AddEventListener(MessageMonitorType.GuidanceSlideEvent, OnGuidanceSlideEvent);
    }

    public void Update()
    {
        //CheckIfStartNextStep();
    }

    //private void CheckIfStartNextStep()
    //{
    //    if(IsCanStartNextStep)
    //    {
    //        MyDebug.Log($"update中检测到响应了{currStep.ToString()}，开始下一步");
    //        IsCanStartNextStep = false;

    //        if(currClickCallBack != null)
    //        {
    //            currClickCallBack();
    //            currClickCallBack = null;
    //        }

    //        NextStep();
    //    }
    //}

    private void AddEvent()
    {
        greenHandsGuidance = QTComponentFactory.Instance.Create<GreenHandsGuidance>();


    }



    public void StartGuidance(GuidanceEvent guidanceEvent)
    {
        IsGuidanceReady = true;
        isGuidancing = true;
        MobileRTSCam.instance.UseYRotation = false;
        MobileRTSCam.instance.isCanPinch = false;
        PlayerPrefs.SetInt("isGuidancing", 1);
        dataList.Clear();
        guidanceEventDic.Clear();
        Init((int)guidanceEvent);
        if (dataList.Count == 0)
            return;
        //currEvent = guidanceEvent;
        //currStep = GuidanceStep.None;
        //NextStep();

    }

    public void CheckCurrStep()
    {
        int step = PlayerPrefs.GetInt("currGuidanceStep", 1);
        int curEvent = PlayerPrefs.GetInt("currGuidanceEvent", 100);
        currStep = (GuidanceStep)step;

        //switch(currStep)
        //{
        //    //case 
        //}
    }

    void Init(int EventType)
    {
        var _guidanceMove = DBManager.Instance.m_kGuidance.m_kDataEntryTable.GetEnumerator();
        while (_guidanceMove.MoveNext())
        {

            if (_guidanceMove.Current.Value._EventType == EventType)
            {
                dataList.Add(_guidanceMove.Current.Value);
            }

        }
        MyDebug.Log($"当前引导总步数 = {dataList.Count}");

        switch(EventType)
        {
            case 100:  // 新手引导
                
                greenHandsGuidance.Start();
                break;
        }
    }

    public void SetCurrEntity(UIEntity entity)
    {
        if (entity != null)
            MyDebug.Log($"给当前currEntity赋值：{entity.m_kUIPrefab}");
        else
            MyDebug.Log("当前步骤没有UIEntity");
        currEntity = entity;
    }

    void OnGuidanceClickEvent(System.Object sender, MessageArgs<GuidanceData> data)
    {
        MyDebug.Log("接收到新手指导点击事件回调");
        if (!needClick)
        {
            MyDebug.Log($"当前needClick为false  return了");
            return;
        }
        
        //World.Scene.GetComponent<UIManagerComponent>().Remove(UI_PrefabPath.m_sUIPopUpWindow_Guidance_Click);
        if(currClickCallBack != null)
        {
            currClickCallBack();
            currClickCallBack = null;
        }
        needClick = false;
        // 接收到点击按钮事件回调 一步一步往下走了
        currEntity = data.Item.entity;
        currType = data.Item.type;
        //MyDebug.LogError($"当前打开的entity:{currEntity},当前组件的type:{currType}");
        NextStep();
       
    }

    public void OnGuidanceSlideEvent(System.Object sender, MessageArgs<GuidanceData> data)
    {
        MyDebug.Log("接收到新手指导滑动事件回调");
        if(currSlideCallBack != null)
        {
            currSlideCallBack();
            currSlideCallBack = null;
        }
        needSlide = false;
        currEntity = data.Item.entity;
        currType = data.Item.type;
       
        NextStep();

    }

    /// <summary>
    /// 下一个step
    /// </summary>
    public void NextStep()
    {
        //MyDebug.Log("当前步数：" + currStep);
        int n = (int)currStep;
        n++;
        if(n > dataList.Count)
        {
            MyDebug.Log($"引导结束了,n = {n}");
            // 引导结束了
            GuidanceOver();
            return;
        }
        currStep = (GuidanceStep)n;
        //MyDebug.Log("当前开始的步数: " + currStep);

        if(guidanceEventDic.ContainsKey(currStep))
        {
            TimerUtil.SetTimeOut(0.1f, () =>
            {
                MyDebug.Log($"当前开始的步数{currStep},调用了{currStep}的引导函数");
                guidanceEventDic[currStep]();
                PlayerPrefs.SetInt("currGuidanceStep", (int)currStep);
                PlayerPrefs.SetInt("currGuidanceEvent", (int)currEvent);
                PlayerPrefs.SetInt("isGuidancing", 1);
            });
           
        }
        else
        {
            MyDebug.LogError("没有{0}的引导函数, 新手引导结束？还是没有添加函数？", currStep);
            CloseGuidanceClickPanel();
            //CloseGuidanceSlidePanel(0.1f);
        }
    }

    public void CheckGuidance(UIEntity entity,MessageMonitorType type = MessageMonitorType.GuidanceClickEvent)  // 默认发送点击的事件
    {
        if(isGuidancing && IsGuidanceReady)
        {
            //SetCurrEntity(entity);
            //IsCanStartNextStep = true;

            GuidanceData data = new GuidanceData();
            data.entity = entity;
            ObserverHelper<GuidanceData>.SendMessage(type, this, new MessageArgs<GuidanceData>(data));
        }
        //else
        //{
        //    MyDebug.LogError("当前的isGuidancing = False");
        //}
    }



    public void ClearAllUI()
    {
        World.Scene.GetComponent<UIManagerComponent>().ClearUIStack();
        World.Scene.GetComponent<UIManagerComponent>().RemoveAll(UI_PrefabPath.m_sUISystem_Hall);
    }

    public void SetHallPanelButtonsEnable(bool isEnable)
    {
        UIEntity entity = World.Scene.GetComponent<UIManagerComponent>().Get(UI_PrefabPath.m_sUISystem_Hall);
        if (entity == null)
        {
            MyDebug.LogError("hall = null");
            return;
        }
        UISystem_HallComponent hallComponent = entity.GetComponent<UISystem_HallComponent>();
        hallComponent.SetAllButtonsDisable(isEnable);
    }

    /// <summary>
    /// 引导结束了
    /// </summary>
    void GuidanceOver()
    {
        MyDebug.Log($"当前引导的事件 = {currEvent.ToString()},已经结束");
        CloseGuidanceClickPanel();
        //CloseGuidanceSlidePanel(0.1f);
        dataList.Clear();
        guidanceEventDic.Clear();
        MobileRTSCam.instance.UseYRotation = false; // 将摄像机旋转还原
        MobileRTSCam.instance.isCanPinch = true;
        isGuidancing = false;
        IsGuidanceReady = false;
        needSlide = false;
        currStep = GuidanceStep.None;
        PlayerPrefs.SetInt(currEvent.ToString(), 1);
        PlayerPrefs.SetInt("isGuidancing", 0);
        PlayerPrefs.SetInt("currGuidanceStep", 0);
        // 监测引导事件结束
        GameEventManager._Instance.onGuidanceEventOver();
    }

    public void CloseGuidanceClickPanel()
    {
        World.Scene.GetComponent<UIManagerComponent>().Remove(UI_PrefabPath.m_sUIPopUpWindow_Guidance_Click);
    }

    public void CloseGuidanceSlidePanel(float time)
    {
        World.Scene.GetComponent<UIManagerComponent>().Get(UI_PrefabPath.m_sUIPopUpWindow_Guidance_Slide).GetComponent<UI_Guidance_Slide>().ClosePanel(time);
    }
    // 从打开的UI面板上获取具体的物体
    public Transform GetTargetTransform(string buttonName)
    {
        if(currEntity == null)
        {
            MyDebug.LogError("当前打开的Entity = null");
            return null;
        }
        List<Component> componentList = currEntity.m_kUIPrefab.m_kElements;
        //Transform tr = null;
        for (int i = 0; i < componentList.Count; i++)
        {
            if(buttonName.Equals(componentList[i].name))
            {
                return componentList[i].transform;

            }
        }
        return null;
    }

}

public class GuidanceData
{
    //public GuidanceStep currStep = GuidanceStep.None;   // 当前是第几步的回调
    public UIEntity entity; // 打开的ui的实体
    public Type type;
}

public enum GuidanceEvent
{
    GreenHandEvent = 100,   // 新手引导事件
}

/// <summary>
/// 所有新手引导总步数
/// </summary>
public enum GuidanceStep
{
    None = 0,
    Step1 = 1,  //
    Step2 = 2,  //
    Step3,  //
    Step4,  //
    Step5,  //
    Step6,  //
    Step7,  //
    Step8,  //
    Step9,  //
    Step10,  //
    Step11,  //
    Step12,  //
    Step13,  //
    Step14,  //
    Step15,  //Step8,  //
    Step16,  //
    Step17,  //
    Step18,  //
    Step19,  //
    Step20,  //
    Step21,  //


}


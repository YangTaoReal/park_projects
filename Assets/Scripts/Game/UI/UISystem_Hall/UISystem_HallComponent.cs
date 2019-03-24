using PixelCrushers.DialogueSystem;
using QTFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[ObjectEventSystem]
public class UISystem_HallComponentAwakeSystem : AAwake<UISystem_HallComponent>
{
    public override void Awake(UISystem_HallComponent _self)
    {
        _self.Awake();
    }
}


[UIEntityComponent(UI_PrefabPath.m_sUISystem_Hall)]
public class UISystem_HallComponent : QTComponent
{
    void HandleUnityAction()
    {
    }

    private bool isHasShowAirPlane = false;

    public Button m_kButton_Information;
    public Button m_kButton_Setting;
    public Button m_kButton_Dashboard;

    public Button m_kButton_Build;
    public Button m_kButton_Bag;
    public Button m_kButton_Friend;
    public Button m_kButton_Event;  // 事件按钮

    public Button m_kButton_AddGold;
    public Button m_kButton_AddStone;
    public Button m_btnWasteAward;

    public Text m_kText_GoldNumber;
    public Text m_kText_StoneNumber;

    public Button m_kButton_Weather;
    public Image m_ImageWeatherCurrent;
    public Text m_kTextWeatherCurrent;
    public Image m_ImageWeatherNext;
    public Text m_TextName;


    public RectTransform m_RectWasteAward;

    // 测试按钮
    public Button testBtn;
    public Button investBtn;
    public Button m_kButton_Task;
    public Button m_kButton_Activity;
    public void Awake()
    {
        UIEntity uI_Entity = ParentEntity as UIEntity;
        m_kButton_Information = uI_Entity.m_kUIPrefab.GetCacheComponent(0) as Button;
        m_kButton_Setting = uI_Entity.m_kUIPrefab.GetCacheComponent(1) as Button;
        m_kButton_Dashboard = uI_Entity.m_kUIPrefab.GetCacheComponent(2) as Button;
        m_kButton_Build = uI_Entity.m_kUIPrefab.GetCacheComponent(3) as Button;
        m_kButton_Bag = uI_Entity.m_kUIPrefab.GetCacheComponent(4) as Button;
        m_kButton_Friend = uI_Entity.m_kUIPrefab.GetCacheComponent(5) as Button;
        m_kButton_AddGold = uI_Entity.m_kUIPrefab.GetCacheComponent(6) as Button;
        m_kButton_AddStone = uI_Entity.m_kUIPrefab.GetCacheComponent(7) as Button;

        m_kText_GoldNumber = uI_Entity.m_kUIPrefab.GetCacheComponent(8) as Text;
        m_kText_StoneNumber = uI_Entity.m_kUIPrefab.GetCacheComponent(9) as Text;

        // 测试按钮
        testBtn = uI_Entity.m_kUIPrefab.GetCacheComponent(10) as Button;

        m_kButton_Weather = uI_Entity.m_kUIPrefab.GetCacheComponent(11) as Button;
        m_ImageWeatherCurrent = uI_Entity.m_kUIPrefab.GetCacheComponent(12) as Image;
        m_kTextWeatherCurrent = uI_Entity.m_kUIPrefab.GetCacheComponent(13) as Text;
        m_ImageWeatherNext = uI_Entity.m_kUIPrefab.GetCacheComponent(14) as Image;
        m_TextName = uI_Entity.m_kUIPrefab.GetCacheComponent(15) as Text;
        m_kButton_Event = uI_Entity.m_kUIPrefab.GetCacheComponent(16) as Button;
        m_kButton_Task = uI_Entity.m_kUIPrefab.GetCacheComponent(17) as Button;
        m_kButton_Activity = uI_Entity.m_kUIPrefab.GetCacheComponent(18) as Button;
        m_RectWasteAward = uI_Entity.m_kUIPrefab.GetCacheComponent(19) as RectTransform;
        m_btnWasteAward = uI_Entity.m_kUIPrefab.GetCacheComponent(20) as Button;

        m_kButton_Information.onClick.AddListener(OnButtonClick_Information);
        m_kButton_Setting.onClick.AddListener(OnButtonClick_Setting);
        m_kButton_Dashboard.onClick.AddListener(OnButtonClick_Dashboard);
        m_kButton_Build.onClick.AddListener(OnButtonClick_Build);
        m_kButton_Bag.onClick.AddListener(OnButtonClick_Bag);
        m_kButton_Friend.onClick.AddListener(OnButtonClick_Firend);
        m_kButton_AddGold.onClick.AddListener(OnButtonClick_AddGold);
        m_kButton_AddStone.onClick.AddListener(OnButtonClick_AddStone);
        m_kButton_Weather.onClick.AddListener(OnButtonClick_Weather);
        m_kButton_Event.onClick.AddListener(OnButtonClick_Event);
        m_kButton_Task.onClick.AddListener(onClick_Task);
        m_kButton_Activity.onClick.AddListener(onClick_Activity);
        m_btnWasteAward.onClick.AddListener(onClick_WastedAward);
 
        m_RectWasteAward.gameObject.SetActive(false);

        // 测试按钮
        testBtn = uI_Entity.m_kUIPrefab.GetCacheComponent(10) as Button;
        testBtn.onClick.AddListener(() =>
        {
            Debug.Log($"点击了testButton");
            //GameEventManager._Instance.onGuidanceEventOver();   // 模拟引导事件完成

            World.Scene.GetComponent<NPCManager>().BornNPC(60005);
            //World.Scene.GetComponent<NPCManager>().BornNPC(60002);
            //World.Scene.GetComponent<NPCManager>().BornNPC(60003);
            //World.Scene.GetComponent<NPCManager>().BornNPC(60004);
            //World.Scene.GetComponent<NPCManager>().BornNPC(60006);
            //PlayerPrefs.SetInt("currGuidanceStep", 1);
            //PlayerPrefs.SetInt(GuidanceEvent.GreenHandEvent.ToString(), 0);

        });

        ObserverHelper<decimal>.AddEventListener(MessageMonitorType.GoldChange, OnGoldChange);
        ObserverHelper<decimal>.AddEventListener(MessageMonitorType.StoneChange, OnStoneChange);
        ObserverHelper<string>.AddEventListener(MessageMonitorType.NameChange, OnNameChange);
        m_kText_GoldNumber.text = World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.m_kPlayerBasicAsset.m_kGold.ToString();
        m_kText_StoneNumber.text = World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.m_kPlayerBasicAsset.m_kStone.ToString();
        m_TextName.text = World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.m_kPlayerBasicAsset.m_kName;


        OnWeatherChange();
        Weather.Instance.OnWeatherChange = OnWeatherChange;
        ObserverHelper<bool>.AddEventListener(MessageMonitorType.WeatherListOpen, WeatherListOpen);
    }

    void WeatherListOpen(object sender, MessageArgs<bool> args)
    {
        if (args.Item == true)
        {
            m_kButton_Weather.gameObject.SetActive(false);
        }
        else
        {
            m_kButton_Weather.gameObject.SetActive(true);
        }
    }
    
    private void OnNameChange(object sender, MessageArgs<string> args)
    {
        m_TextName.text = args.Item;


    }
    void onClick_WastedAward()
    {
        m_RectWasteAward.gameObject.SetActive(false);
        UIEntity entity = World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIWastedAward_Box);
        entity.GetComponent<UIWastedAwardBox>().Init(200000);
    }

    private void onClick_Activity()
    {
        RestState();
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            UI_Helper.ShowCommonTips(229);
            return;
        }
        World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIActivity_FirstRecharge);
    }

    /// <summary>
    /// 任务
    /// </summary>
    private void onClick_Task()
    {
        RestState();
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            UI_Helper.ShowCommonTips(229);
            return;
        }
        World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPopUpWindow_Task);
    }

    private void OnWeatherChange()
    {
        //current
        TimeRangWeather timeRangWeatherCurrent = Weather.Instance.GetCurWeather();
        if (timeRangWeatherCurrent == null)
            return;
        
        m_ImageWeatherCurrent.sprite = UI_Helper.GetSprite(UIPage_WeatherInfoItemComponent.GetWeatherSprite(timeRangWeatherCurrent.weaterType));
        m_kTextWeatherCurrent.text = $"{timeRangWeatherCurrent.from}:00-{timeRangWeatherCurrent.to}:00 ";


        //next
        TimeRangWeather timeRangWeatherNext = Weather.Instance.GetNextWeather();
        if (timeRangWeatherNext == null)
            return;
        
        m_ImageWeatherNext.sprite = UI_Helper.GetSprite(UIPage_WeatherInfoItemComponent.GetWeatherSprite(timeRangWeatherNext.weaterType));
    }

    private void OnGoldChange(object sender, MessageArgs<decimal> args)
    {
        m_kText_GoldNumber.text = args.Item.ToString();
    }
    private void OnStoneChange(object sender, MessageArgs<decimal> args)
    {
        m_kText_StoneNumber.text = args.Item.ToString();
    }

    public override void Dispose()
    {
        m_kButton_Information.onClick.RemoveListener(OnButtonClick_Information);
        m_kButton_Setting.onClick.RemoveListener(OnButtonClick_Setting);
        m_kButton_Dashboard.onClick.RemoveListener(OnButtonClick_Dashboard);
        m_kButton_Build.onClick.RemoveListener(OnButtonClick_Build);
        m_kButton_Bag.onClick.RemoveListener(OnButtonClick_Bag);
        m_kButton_Friend.onClick.RemoveListener(OnButtonClick_Firend);
        m_kButton_AddGold.onClick.RemoveListener(OnButtonClick_AddGold);
        m_kButton_AddStone.onClick.RemoveListener(OnButtonClick_AddStone);
        m_kButton_Weather.onClick.RemoveListener(OnButtonClick_Weather);
        m_kButton_Event.onClick.RemoveListener(OnButtonClick_Event);
    }

    public void SetAllButtonsDisable(bool isDisable)
    {
        //Debug.LogError($"isEnable = {isDisable}");
        UIEntity entity = ParentEntity as UIEntity;
        var list = entity.m_kUIPrefab.m_kElements;
        for (int i = 0; i < list.Count; i++)
        {
            //Debug.LogError($"组件：{list[i].GetType()}");
            if(list[i].GetType() == typeof(Button))
            {
                //Debug.LogError($"找到button了，{list[i]}");
                var b = list[i] as Button;
                b.interactable = isDisable;
            }
        }
    }

    /// <summary>
    /// 天气
    /// </summary>
    private void OnButtonClick_Weather()
    {
        RestState();
        World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPage_WeatherInfo);
    }

    /// <summary>
    /// 个人信息
    /// </summary>
    public void OnButtonClick_Information()
    {
        RestState();
        World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPage_Head);
    }

    /// <summary>
    /// 事件按钮
    /// </summary>
    public void OnButtonClick_Event()
    {
        World.Scene.GetComponent<DialogueManagerComponent>().ShowAirPlaneScene();
        //bool hasName = DialogueManager.instance.initialDatabase.GetVariable("hasUserName").InitialBoolValue;
        //bool hasGotInvest = DialogueManager.instance.initialDatabase.GetVariable("hasGotInvest").InitialBoolValue;
        //if(!hasName || !hasGotInvest)
        //{
        //    if(!isHasShowAirPlane)
        //    {
        //        World.Scene.GetComponent<DialogueManagerComponent>().ShowAirPlaneScene();
        //        isHasShowAirPlane = true;
        //    }
        //}
    }

    /// <summary>
    /// 设置
    /// </summary>
    public void OnButtonClick_Setting()
    {
        RestState();
        World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPopupWindow_Setting);
    }

    /// <summary>
    /// Dashboard
    /// </summary>
    public void OnButtonClick_Dashboard()
    {
        RestState();
    }

    /// <summary>
    /// 建造
    /// </summary>
    public void OnButtonClick_Build()
    {
        RestState();
        UIEntity entity = World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPage_Build);

        //CheckGuidance(entity);
        GuidanceManager._Instance.CheckGuidance(entity);
    }

    /// <summary>
    /// 新手引导检测
    /// </summary>
    //private void CheckGuidance(UIEntity entity)
    //{
    //    if(GuidanceManager.isGuidancing)
    //    {
    //        GuidanceData data = new GuidanceData();
    //        data.entity = entity;
    //        ObserverHelper<GuidanceData>.SendMessage(MessageMonitorType.GuidanceClickEvent, this, new MessageArgs<GuidanceData>(data));
    //    }
    //}

    /// <summary>
    /// 商店
    /// </summary>
    public void OnButtonClick_Bag()
    {
        RestState();
        World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPage_Shop);
        //World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPage_Pack);
    }

    /// <summary>
    /// 好友
    /// </summary>
    public void OnButtonClick_Firend()
    {
        RestState();
        World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPopUpWindow_Friend);
    }

    /// <summary>
    /// 添加金币
    /// </summary>
    public void OnButtonClick_AddGold()
    {
        RestState();
    }

    /// <summary>
    /// 添加钻石
    /// </summary>
    public void OnButtonClick_AddStone()
    {
        RestState();
    }


    private void RestState()
    {
        World.Scene.GetComponent<UIManagerComponent>().RemoveAll(UI_PrefabPath.m_sUISystem_Hall);
        MapGridMgr.Instance.EndFreeingWastedland();
        MapGridMgr.Instance.UnFoucs();
    }
}

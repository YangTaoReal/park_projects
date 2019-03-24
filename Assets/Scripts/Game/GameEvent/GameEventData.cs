using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ParkGameEvent
{
    /// <summary>
    /// 结果枚举
    /// </summary>
    public enum ResultType
    {
        None = 0,
        StartDialogueConversation = 1,  // 触发对话事件
        RegisterEvent = 2,              // 结果触发注册另一个事件

        //Insvestment =1,    
        //EventHelper=2,
        //Reporter=3,
        //VisitorEvent=4,           
        //BiologicalExperts=5,      // 动植物专家事件
    }
    /// <summary>
    /// 注册类型
    /// </summary>
    public enum RegisterType
    {
        None = 0,
        CreateAccount = 1,  // 注册账号时候注册
        LogInGame = 2,      // 登录游戏注册
        QuitGame = 3,       // 退出游戏注册
    }
    /// <summary>
    /// 检测类型
    /// </summary>
    public enum CheckType
    {
        None = 0,
        LogInGame =1,
        QuitGame=2,
        EveryMinute = 3,  // 时钟 每分钟检测
        GetGold = 4,        // 获得金币d
        GetAnimal = 5,      // 获得动物
        GetPlant = 6,       // 获得植物
        BuildFinish = 7,    // 建造完成
        UpgradeBuildSuccess = 8,  // 升级完
        WastelandFinish = 9,// 开荒完成
        EventFinish = 10,    // 事件完成
        CureAnimal = 11,     // 动物治疗完成
        PlaceAnimalOrPlant = 12,    // 放置动物或者植物
        DialogueOver = 13,      // 对话事件结束
        UpgradeAnimalSuccess = 14,  // 升级动物成功
        UpgradPlantSuccess = 15,    // 升级植物成功
        GuidanceEventOver = 16,     // 引导功能完成
    }

    /// <summary>
    /// 条件枚举
    /// </summary>
    public enum ConditionType
    {
        None = 0,
        TotalGameTime  =1,      // 总游戏时长
        CreateTime = 2,         // 账号创建时长，包含离线时间
        RegisterEventOnlineTime = 3,       // 注册事件的在线时长(不是注册账号的时长)
        AnimalNumber = 4,
        PlantNumber = 5,
        IllustratedAnimalNumber = 6,    // 图鉴动物数量
        IllustratedPlantNumber = 7,     // 图鉴植物数量
        HasWastelandNumber = 8,         // 已开荒地数量
        PickUpLitterNumber = 9,         // 捡垃圾次数
        DealShitNumber = 10,             // 处理粪便次数
        TotalVisitorNumber =11,         // 游客总数
        CureAnimalNumber = 12,          // 治疗动物数量
        AnimalAndPlantNumber = 13,      // 园区动植物总数
        FinishEvent = 14,               // 完成事件
        SingleLogInTime = 15,           // 单次游戏在线时长
        DialogueOver = 16,              // 对话事件结束

    }

    /// <summary>
    /// 销毁类型枚举
    /// </summary>
    public enum DestroyType
    {
        None = 0,           
        LogIn = 1,          // 登录时候销毁
        QuiteGame = 2,      // 退出游戏时候销毁
        ReturnZero = 3,     // 触发次数归零
        NeverDestroy = 4,   // 永不销毁
    }

    /// <summary>
    /// 记录事件相关数据的类
    /// </summary>
    public class RecordEventItem
    {
        public DateTime finishTime;     // 完成事件的时间
        public DateTime registerTime;   // 注册事件的时间
        public int registerOnlineTime;  // 注册事件后的在线时间
    }

    public class GameEventData
    {

        public Dictionary<int, OneGameEvent> GameEventDic = new Dictionary<int, OneGameEvent>();    // 已经注册了的事件字典
        public Dictionary<int, RecordEventItem> RecordDic = new Dictionary<int, RecordEventItem>(); // 已完成的事件字典

    }

    [System.Serializable]
    public class RecordEventItemServer
    {
        public string _ID;//ID
        public string finishTime;     // 完成事件的时间
        public string registerTime;   // 注册事件的时间
        public int registerOnlineTime;  // 注册事件后的在线时间
    }

    [System.Serializable]
    public class OneGameEventServer
    {
        public string _ID;//ID
        public int _ConditionType;    
        public string _ConditionValue;
        public int _ResultType;
        public string _ResultValue;
        public int _CheckType;
        public int _RegType;
        public int _Count;
        public int _DestoryType;
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using ParkGameEvent;
using QTFramework;
using UnityEngine;

[ObjectEventSystem]
public class GameEventManagerAwakeSysterm : AAwake<GameEventManager>
{
    // 事件系统需要根据gameevent表来决定什么时候注册 检测那些值
    // 达到指定值之后再完
    public override void Awake(GameEventManager self)
    {
        self.Awake();
    }
}

public class GameEventManager : QTComponent {

    public static GameEventManager _Instance;
 

    public GameEventData mGameEventData = new GameEventData();

    public delegate void OnRegisterEvent();
    public delegate void OnLogInGameEvent();
    public delegate void OnGetGold();
    public delegate void OnGetAnimal();
    public delegate void OnGetPlant();
    public delegate void OnRemoveWasteland();
    public delegate void OnEventFinished();
    public delegate void OnPlaceAnimalOrPlant();
    public delegate void OnDialogueEventOver();
    public delegate void OnUpgradeAnimal();
    public delegate void OnUpgradePlant();
    public delegate void OnUpgradeBuild();
    public delegate void OnPlacedBuildSuccess();
    public delegate void OnQuiteGame();
    public delegate void OnEveryMinute();   // 每分钟检测
    public delegate void OnGuidanceEventOver();



    // 事件
    public OnRegisterEvent onResgister;
    public OnLogInGameEvent onLogInGame;
    public OnGetGold onGetGold;
    public OnGetAnimal onGetAnimal;
    public OnGetPlant onGetPlant;
    public OnRemoveWasteland onRemoveWasteland;
    public OnEventFinished onEventFinished;
    public OnPlaceAnimalOrPlant onPlaceAnimalOrPlant;   // 放置动物或者植物
    public OnDialogueEventOver onDialogueEventOver;
    public OnUpgradeBuild onUpgradeBuild;
    public OnUpgradePlant onUpgradePlant;
    public OnUpgradeAnimal onUpgradeAnimal;
    public OnPlacedBuildSuccess onPlacedBuildSuccess;
    public OnQuiteGame onQuiteGame;
    public OnEveryMinute onEveryMinute;
    public OnGuidanceEventOver onGuidanceEventOver;

	public void Awake()
    {
        _Instance = this;
        Init();
    }

    public void Init()
    {
        // 所有的初始化都在这里进行  
        //var allEvent = DBManager.Instance.m_kGameEvent.m_kDataEntryTable.Count;
        //Debug.Log($"当前事件总数：{allEvent}");

        // 监测事件
        onResgister = OnResgisterEventHappened;
        onLogInGame = LogInGame;
        onPlaceAnimalOrPlant = PlaceAnimalOrPlant;
        onDialogueEventOver = DialogueEventOver;
        onGetGold = GetGold;
        onGetPlant = GetPlant;
        onGetAnimal = GetAnimal;
        onRemoveWasteland = RemoveWastlandSuccess;
        onEventFinished = EventFinished;
        onUpgradeBuild = UpgradeBuildSuccess;
        onUpgradePlant = UpgradePlant;
        onUpgradeAnimal = UpgradeAnimal;
        onPlacedBuildSuccess = PlaceBuildSuccess;
        onQuiteGame = QuiteGame;
        onEveryMinute = CheckEveryMinute;
        onGuidanceEventOver = CheckGuidanceEventOver;

        // 每分钟检测一次代码写在这里
        TimerUtil.SetTimeOut(60f, () =>
        {
            // 每60s触发一次
            onEveryMinute();
        },-1);
    }

    public List<OneGameEvent> GetTargetGameEvents(CheckType type)
    {
        List<OneGameEvent> list = new List<OneGameEvent>();
        foreach (var item in mGameEventData.GameEventDic)
        {
            if (item.Value.GetCheckType() == type)
            {
                // 满足检测条件了 将满足检测枚举的事件添加进列表
                list.Add(item.Value);
            }
        }
        return list;
    }
#region   检测相关
   
    // 检测到注册账号事件发生时候
    private void OnResgisterEventHappened()
    {
        Debug.Log($"检测到注册发生，在这里注册所有的注册类型事件");
        // 遍历事件表 注册所有的注册事件
        var enumerator = DBManager.Instance.m_kGameEvent.m_kDataEntryTable.GetEnumerator();
        while (enumerator.MoveNext())
        {
            if (enumerator.Current.Value._RegType == (int)RegisterType.CreateAccount)
            {
                RegisterEvent(enumerator.Current.Value);
            }
        }
    }


    public void SetGameEvent(Dictionary<int, OneGameEvent> dic)
    {
        mGameEventData.GameEventDic = dic;
        Debug.Log($"从本地恢复未完成事件字典,dic.Count = {mGameEventData.GameEventDic.Count}");
    }
    public void SetRecord(Dictionary<int, RecordEventItem> dic)
    {
        mGameEventData.RecordDic = dic;
        Debug.Log($"从本地恢复事件簿,dic.Count = {mGameEventData.RecordDic.Count}");
    }
     

    // 检测到登录游戏的时候
    private void LogInGame()
    {
        //检测到登录游戏，在这里注册登录事件，检测登录也是在这里;
        //Debug.Log($"检测到登录游戏,当前事件字典长度:{mGameEventData.GameEventDic.Count},事件簿长度:{mGameEventData.RecordDic.Count}");
        var enumerator = DBManager.Instance.m_kGameEvent.m_kDataEntryTable.GetEnumerator();
        while (enumerator.MoveNext())
        {
            if (enumerator.Current.Value._RegType == (int)RegisterType.LogInGame)
            {
                RegisterEvent(enumerator.Current.Value);
            }
        }

        // 登录销毁类型也在这里检测
        var enumerator1 = DBManager.Instance.m_kGameEvent.m_kDataEntryTable.GetEnumerator();
        while (enumerator1.MoveNext())
        {
            if (enumerator1.Current.Value._DestoryType == (int)DestroyType.LogIn)
            {
                RemoveEvent(enumerator1.Current.Key);
            }
        }

        // 在这里检测登录类型
        var list = GetTargetGameEvents(CheckType.LogInGame);
        for (int i = 0; i < list.Count; i++)
        {
            list[i].CheckSelfCondition();
        }
       
    }

    // 检测到放置了动物或者植物
    private void PlaceAnimalOrPlant()
    {
        // 在这里触发所有注册了的 检测类型为12 的事件 判断条件是否满足
        //Debug.Log($"放置了动物或者植物,动植物总数 = {count}");
        List<OneGameEvent> list = GetTargetGameEvents(CheckType.PlaceAnimalOrPlant);
        // 遍历开始核对检测条件
        for (int i = 0; i < list.Count; i++)
        {
            list[i].CheckSelfCondition();
        }
    }
   
    // 检测到对话事件结束
    private void DialogueEventOver()
    {
        var list = GetTargetGameEvents(CheckType.DialogueOver);
        for (int i = 0; i < list.Count; i++)
        {
            list[i].CheckSelfCondition();
        }
    }

    // 检测到得到金币
    private void GetGold()
    {
        var list = GetTargetGameEvents(CheckType.GetGold);
        for (int i = 0; i < list.Count; i++)
        {
            list[i].CheckSelfCondition();
        }
    }

    // 检测到得到动物
    private void GetAnimal()
    {
        var list = GetTargetGameEvents(CheckType.GetAnimal);
        for (int i = 0; i < list.Count; i++)
        {
            list[i].CheckSelfCondition();
        }
    }
    private void EventFinished()
    {
        var list = GetTargetGameEvents(CheckType.EventFinish);
        for (int i = 0; i < list.Count; i++)
        {
            list[i].CheckSelfCondition();
        }
    }

    // 检测到得到植物
    private void GetPlant()
    {
        var list = GetTargetGameEvents(CheckType.GetPlant);
        for (int i = 0; i < list.Count; i++)
        {
            list[i].CheckSelfCondition();
        }
    }

    // 检测到建筑建造完成
    private void PlaceBuildSuccess()
    {
        var list = GetTargetGameEvents(CheckType.BuildFinish);
        for (int i = 0; i < list.Count; i++)
        {
            list[i].CheckSelfCondition();
        }
    }
    // 检测到升级建筑成功
    private void UpgradeBuildSuccess()
    {
        var list = GetTargetGameEvents(CheckType.UpgradeBuildSuccess);
        for (int i = 0; i < list.Count; i++)
        {
            list[i].CheckSelfCondition();
        }
    }
    // 检测到开荒成功
    private void RemoveWastlandSuccess()
    {
        var list = GetTargetGameEvents(CheckType.WastelandFinish);
        for (int i = 0; i < list.Count; i++)
        {
            list[i].CheckSelfCondition();
        }
    }
    // 检测到治疗动物完成
    private void CureAnimatl()
    {
        var list = GetTargetGameEvents(CheckType.CureAnimal);
        for (int i = 0; i < list.Count; i++)
        {
            list[i].CheckSelfCondition();
        }
    }
    // 检测到动物升级
    private void UpgradeAnimal()
    {
        var list = GetTargetGameEvents(CheckType.UpgradeAnimalSuccess);
        for (int i = 0; i < list.Count; i++)
        {
            list[i].CheckSelfCondition();
        }
    }
    // 检测到植物升级
    private void UpgradePlant()
    {
        var list = GetTargetGameEvents(CheckType.UpgradPlantSuccess);
        for (int i = 0; i < list.Count; i++)
        {
            list[i].CheckSelfCondition();
        }
    }
    // 检测到退出游戏
    private void QuiteGame()
    {
        // 登录销毁类型也在这里检测
        var enumerator1 = DBManager.Instance.m_kGameEvent.m_kDataEntryTable.GetEnumerator();
        while (enumerator1.MoveNext())
        {
            if (enumerator1.Current.Value._DestoryType == (int)DestroyType.QuiteGame)
            {
                RemoveEvent(enumerator1.Current.Key);
            }
        }

        // 检测类型为退出游戏
        var list = GetTargetGameEvents(CheckType.QuitGame);
        for (int i = 0; i < list.Count; i++)
        {
            list[i].CheckSelfCondition();
        }
    }
    // 每分钟检测
    private void CheckEveryMinute()
    {
        var list = GetTargetGameEvents(CheckType.EveryMinute);
        for (int i = 0; i < list.Count; i++)
        {
            list[i].CheckSelfCondition();
        }
    }
    // 检测到引导功能结束
    private void CheckGuidanceEventOver()
    {
        var list = GetTargetGameEvents(CheckType.GuidanceEventOver);
        for (int i = 0; i < list.Count; i++)
        {
            list[i].CheckSelfCondition();
        }
    }

#endregion
    /// <summary>
    /// 注册事件
    /// </summary>
    public void RegisterEvent(CS_GameEvent.DataEntry dataEntry)
    {
        if(!mGameEventData.GameEventDic.ContainsKey(dataEntry._ID))
        {
            OneGameEvent item = new OneGameEvent(dataEntry);
            mGameEventData.GameEventDic.Add(dataEntry._ID, item);
            RecordEventItem record = new RecordEventItem();
            record.registerTime = DateTime.Now;
            mGameEventData.RecordDic[dataEntry._ID] = record;
            DataManager._instance.AddLocalData<OneGameEvent>(dataEntry._ID.ToString(), item);
            DataManager._instance.AddLocalData<RecordEventItem>(dataEntry._ID.ToString(), record);

            Debug.Log($"事件注册成功，当前注册事件id = {dataEntry._ID},事件簿长度 = {mGameEventData.GameEventDic.Count}");
        }
        else
        {
            Debug.Log($"代号为{dataEntry._ID}的事件已经被注册，无法再进行注册");
        }
    }

    /// <summary>
    /// 移除事件
    /// </summary>
    public void RemoveEvent(int id)
    {
        if (mGameEventData.GameEventDic.ContainsKey(id))
        {
            mGameEventData.GameEventDic.Remove(id);
            DataManager._instance.DelLocalData<OneGameEvent>(id.ToString());
            RecordEventItem item;
            if(mGameEventData.RecordDic.TryGetValue(id,out item))
            {
                // 记录事件完成的时间
                item.finishTime = DateTime.Now;
                mGameEventData.RecordDic[id] = item;
                DataManager._instance.FixLocalData<RecordEventItem>(id.ToString(), item);
            }
            Debug.Log($"事件:{id}完成移除该事件,完成的时间是:{item.finishTime}");
            // 在这里响应事件完成事件
            onEventFinished();

        
         
        }
    }

    public CS_GameEvent.DataEntry GetDateById(int eventID)
    {
        var enumerator = DBManager.Instance.m_kGameEvent.m_kDataEntryTable.GetEnumerator();
        while (enumerator.MoveNext())
        {
            if (enumerator.Current.Value._ID == eventID)
            {
                return enumerator.Current.Value;
            }
        }
        return null;
    }

    public void TriggerToRegisterNewEvent(int eventID)
    {
        var data = GetDateById(eventID);
        RegisterEvent(data);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum MessageMonitorType
{
    None = 0,
    GoldChange,
    StoneChange,
    WaterChange,
    SunChange,
    SpadeChange,
    StartBuilding,//建筑物开始建造
    RemoveBuilding,//删除建筑物
    StartWastedland,//开荒
    StartUpLv,//建筑物开始升级
    SelectedTileInfo,//选中的物体
    SelectedModelInfo,//选中的物体
    GuidanceClickEvent,  // 新手引导点击事件
    GuidanceSlideEvent, // 新手引导滑动事件
    NameChange,
    RefreshTaskUI,
    RefreshPackUI,
    RecoverData,
    BagChange,//背包变化
    WeatherListOpen,
}
public class ObserverHelper<T>
{

    public static void SendMessage(MessageMonitorType _messageMonitorType, object _sender, MessageArgs<T> _args)
    {
        MessageAggregator<T>.Instance.Publish(_messageMonitorType, _sender, _args);
    }

    public static void AddEventListener(MessageMonitorType _messageMonitorType, MessageHandler<T> _handler)
    {
        MessageAggregator<T>.Instance.Subscribe(_messageMonitorType, _handler);
    }
}

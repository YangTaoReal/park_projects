using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[System.Serializable]
public class PlayerBasicAssetServer
{
    public string ID;
    public string m_kName;
    public uint m_kExp;
    public uint m_kLevel;
    public uint m_kStone;
    public uint m_kSun;
    public uint m_kGold;
    public uint m_kWater;
    public uint m_nWastedLand;
    public string m_lWasteLand;
    public string m_beginWaste;
    public bool m_IsCanAssistant;
    public int m_ToatlGameTime;
    public string m_OnLineTime;
    public string m_OfflineTime;
    public string m_CreateAccountTime;
 
}

public class PlayerBasicAsset
{
    /// <summary>
    /// 全球唯一ID GUIG
    /// </summary>
    string _id;
    public string ID
    {
        get { return _id; }
        set { _id = value; }
    }
    /// <summary>
    /// 名字
    /// </summary>
    public String m_kName { get; set; } = "";

    /// <summary>
    /// 经验
    /// </summary>
    public uint m_kExp { get; set; }
    /// <summary>
    /// 等级
    /// </summary>
    public uint m_kLevel { get; set; }
    /// <summary>
    /// 钻石
    /// </summary>
    public decimal m_kStone { get; set; }
    /// <summary>
    /// 阳光
    /// </summary>
    public decimal m_kSun { get; set; }
    /// <summary>
    /// 金币
    /// </summary>
    public decimal m_kGold { get; set; }
    /// <summary>
    /// 蓄水
    /// </summary>
    public decimal m_kWater { get; set; }
    /// <summary>
    /// 开荒了多少地块
    /// </summary>
    /// <value>The m  n wasted land.</value>
    public uint m_nWastedLand { get; set; }
    /// <summary>
    /// 正在开荒的土地
    /// </summary>
    /// <value>The m  l waste land.</value>
    public List<Guid> m_lWasteLand { get; set; }
    /// <summary>
    /// 开始开荒时间
    /// </summary>
    /// <value>The m begin waste.</value>
    public DateTime m_beginWaste { get; set; }
    /// <summary>
    /// 活跃度
    /// </summary>
    public uint m_UIntVitality { get; set; }
    /// <summary>
    /// 总的游戏在线时间(单位：秒)
    /// </summary>
    public int m_ToatlGameTime { get; set; }
    /// <summary>
    /// 上线时刻
    /// </summary>
    /// <value>The m on line time.</value>
    public DateTime m_OnLineTime { get; set; }
    public bool m_IsCanAssistant { get; set; }
    /// <summary>
    /// 下线时刻
    /// </summary>
    /// <value>The m offline time.</value>
    public DateTime m_OfflineTime { get; set; }
    /// <summary>
    /// 创建账号的时间(第一次登陆游戏的时候)
    /// </summary>
    /// <value>The m offline time.</value>
    public DateTime m_CreateAccountTime { get; set; }
    public List<int> m_GetVitality { get; set; } = new List<int>();
    public Dictionary<QTFramework.TaskType, int> m_VitalityNumber { get; set; } = new Dictionary<QTFramework.TaskType, int>()
    { { QTFramework.TaskType.Login, 0 }, { QTFramework.TaskType.ShovelShit, 0 }, { QTFramework.TaskType.ClearTheRubbish, 0 }, { QTFramework.TaskType.CollectResources, 0 }, { QTFramework.TaskType.Item, 0 }, { QTFramework.TaskType.Gold, 0 }, { QTFramework.TaskType.Share, 0 }, { QTFramework.TaskType.Visit, 0 }, { QTFramework.TaskType.BuyItem, 0 }, { QTFramework.TaskType.Feed, 0 }, { QTFramework.TaskType.Stone, 0 }, { QTFramework.TaskType.OnLine, 0 }, { QTFramework.TaskType.BuyAnimal, 0 }, { QTFramework.TaskType.BuyPlant, 0 }, { QTFramework.TaskType.Usefeed, 0 }, { QTFramework.TaskType.Water, 0 }, { QTFramework.TaskType.UseSpeedUpItem, 0 }, { QTFramework.TaskType.assart, 0 }, { QTFramework.TaskType.SendGift, 0 }, { QTFramework.TaskType.BuildingUpgrade, 0 }, { QTFramework.TaskType.DealWithEvent, 0 },
    };
}
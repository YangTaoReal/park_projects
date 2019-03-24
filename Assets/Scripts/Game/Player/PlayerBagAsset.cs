using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[System.Serializable]
public class PlayerBagAssetServer
{
    public string ID;
    public int BagVolume;
    public string m_kBag;
}

[System.Serializable]
public class PlayerBagAsset
{
    public enum ItemType
    {
        All = 0,
        Gold, //金币
        Stone, //钻石
        Sun, //阳光
        Water, //水
        Animal, //动物
        Botany, //植物
        Nutrients, //饲料
        StageProperty, //道具
        Building, //建筑
        AnimaiNutrients, //动物饲料
        BotanyNutrients, //植物饲料
        Medicine, //药剂
        AnimaiMedicine, //动物药剂
        BotanyMedicine, //植物药剂
        Shovel,//铲子
        Gift,//礼包
    }

    public string ID { get; set; }
    public int BagVolume { get; set; }

    [System.Serializable]
    public class BagItem
    {
        public ItemType m_kItemType;
        public int m_kItemID;
        public int m_kCount;
    }

    public List<BagItem> m_kBag = new List<BagItem>();

}
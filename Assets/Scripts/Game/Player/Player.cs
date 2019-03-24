/*************************************
 * 玩家游戏数据
 * author:SmartCoder
 **************************************/

using System;
using System.Collections.Generic;
using System.Linq;

using QTFramework;

using UnityEngine;

namespace QTFramework
{
    public enum TaskType
    {
        None = 0, //无意义 
        Login, //1:登陆 
        ShovelShit, // 2动物铲屎 
        ClearTheRubbish, //3清理垃圾 
        CollectResources, //4收集资源 
        Item, //   5使用道具 
        Gold, //   6消耗金币
        Share, //  7 分享
        Visit, //  8拜访好友
        BuyItem, //9 购买任意道具 
        Feed, //   10喂养动物或植物 
        Stone, //  11消耗钻石"
        OnLine, // 12在线
        BuyAnimal, // 13买动物
        BuyPlant, //14 买植物
        Usefeed, //15 使用饲料
        Water, // 16使用水
        UseSpeedUpItem, //17 使用加速道具 
        assart, //18 开荒 
        SendGift, //19 给好友送礼 
        BuildingUpgrade, //20 升级建筑 
        DealWithEvent //21 处理事件
    }

    public class Player : QTEntity
    {
        public PlayerBasicAsset m_kPlayerBasicAsset = new PlayerBasicAsset(); //玩家资产条
        public PlayerBagAsset m_kPlayerBagAsset = new PlayerBagAsset();
        public PlayerBuildingAsset m_kPlayerBuildingAsset = new PlayerBuildingAsset();
        public PlayerShopAsset m_kPlayerShopAsset = new PlayerShopAsset();
        //public SettlementsAsset m_kSettlementsAsset {  get; private set; } = new SettlementsAsset();
        public SettlementsAsset m_kSettlementsAsset = new SettlementsAsset(); //安置点信息

        public void Init()
        {

            PlayerBasicAsset basicAssetServer = DataManager._instance.GetOnlyData<PlayerBasicAsset>();
            PlayerBagAsset bagAssetServer = DataManager._instance.GetOnlyData<PlayerBagAsset>();
            SettlementsAsset settlements = DataManager._instance.GetOnlyData<SettlementsAsset>();
            if (settlements == null)
            {
                m_kSettlementsAsset.ID = GenerateID.ID.ToString();
                DataManager._instance.AddLocalData<SettlementsAsset>(m_kSettlementsAsset.ID, m_kSettlementsAsset);
            }
            else
                m_kSettlementsAsset = settlements;

            if (basicAssetServer != null)
                m_kPlayerBasicAsset = basicAssetServer;
            else
            {
                m_kPlayerBasicAsset.ID = GenerateID.ID.ToString();
                m_kPlayerBasicAsset.m_lWasteLand = new List<Guid>();
                m_kPlayerBasicAsset.m_beginWaste = default(DateTime);
                m_kPlayerBasicAsset.m_OnLineTime = default(DateTime);
                m_kPlayerBasicAsset.m_OfflineTime = default(DateTime);
                m_kPlayerBasicAsset.m_CreateAccountTime = default(DateTime);
                m_kPlayerBasicAsset.m_IsCanAssistant = true;

                DataManager._instance.AddLocalData<PlayerBasicAsset>(m_kPlayerBasicAsset.ID, m_kPlayerBasicAsset);
                InitPlayerAsset(1);
            }

            if (bagAssetServer != null)
                m_kPlayerBagAsset = bagAssetServer;
            else
            {
                m_kPlayerBagAsset.ID = GenerateID.ID.ToString();
                m_kPlayerBagAsset.BagVolume = int.Parse(DBManager.Instance.m_kDisperse.GetEntryPtr(10003)._Val1);
                DataManager._instance.AddLocalData<PlayerBagAsset>(m_kPlayerBagAsset.ID, m_kPlayerBagAsset);
                InitPlayerAsset(2);
            }

        }

        //typ 1是货币类型,2是背包
        private void InitPlayerAsset(int typ)
        {
            var dataEntry = DBManager.Instance.m_kInitialize.m_kDataEntryTable.GetEnumerator();
            while (dataEntry.MoveNext())
            {
                if (typ == 1)
                {

                    if (dataEntry.Current.Value._Goods.x == (int) PlayerBagAsset.ItemType.Gold)
                    {
                        AddGold((decimal) dataEntry.Current.Value._Goods.z);
                    }
                    else if (dataEntry.Current.Value._Goods.x == (int) PlayerBagAsset.ItemType.Stone)
                    {
                        AddStone((decimal) dataEntry.Current.Value._Goods.z);
                    }
                    else if (dataEntry.Current.Value._Goods.x == (int) PlayerBagAsset.ItemType.Sun)
                    {
                        AddSun((decimal) dataEntry.Current.Value._Goods.z);
                    }
                    else if (dataEntry.Current.Value._Goods.x == (int) PlayerBagAsset.ItemType.Water)
                    {
                        AddWater((decimal) dataEntry.Current.Value._Goods.z);
                    }
                    // else if (dataEntry.Current.Value._Goods.x == (int) PlayerBagAsset.ItemType.Spade)
                    // {
                    //     AddSpade((decimal) dataEntry.Current.Value._Goods.z);
                    // }
                }
                else if (typ == 2)
                {
                    if (dataEntry.Current.Value._Goods.x != (int) PlayerBagAsset.ItemType.Gold &&
                        dataEntry.Current.Value._Goods.x != (int) PlayerBagAsset.ItemType.Stone &&
                        dataEntry.Current.Value._Goods.x != (int) PlayerBagAsset.ItemType.Sun &&
                        dataEntry.Current.Value._Goods.x != (int) PlayerBagAsset.ItemType.Water)
                    {
                        AddItemToBag((int) dataEntry.Current.Value._Goods.y, (PlayerBagAsset.ItemType) dataEntry.Current.Value._Goods.x, (int) dataEntry.Current.Value._Goods.z);
                    }
                }
            }
        }
  

        public void AddAsset(PlayerBagAsset.ItemType _ItemType, decimal num, Action<decimal> _successCallBack = null)
        {
   
            if (_ItemType == PlayerBagAsset.ItemType.Gold)
            {
                if (num > 0)
                    AddGold(num, _successCallBack);
                else
                    CostGold(-num, _successCallBack);
            }
            else if (_ItemType == PlayerBagAsset.ItemType.Stone)
            {
                if (num > 0)
                    AddStone(num, _successCallBack);
                else
                    CostStone(-num, _successCallBack);
            }
            else if (_ItemType == PlayerBagAsset.ItemType.Sun)
            {
                if (num > 0)
                    AddSun(num, _successCallBack);
                else
                    CostSun(-num, _successCallBack);
            }
            else if (_ItemType == PlayerBagAsset.ItemType.Water)
            {
                if (num > 0)
                    AddWater(num, _successCallBack);
                else
                    CostWater(-num, _successCallBack);
            }
        


        }
        public void AddGold(decimal _goldNumber, Action<decimal> _successCallBack = null)
        {
            if (_goldNumber <= 0)
            {
                Debug.Log("加钱必须是正数");
                return;
            }
            UI_Helper.ShowCommonTips(UI_Helper.GetTextByLanguageID(123, _goldNumber.ToString()));
            _successCallBack?.Invoke(_goldNumber);
            m_kPlayerBasicAsset.m_kGold += _goldNumber;
            DataManager._instance.FixLocalData<PlayerBasicAsset>(m_kPlayerBasicAsset.ID, m_kPlayerBasicAsset);
            ObserverHelper<decimal>.SendMessage(MessageMonitorType.GoldChange, this, new MessageArgs<decimal>(m_kPlayerBasicAsset.m_kGold));
            if(GameEventManager._Instance != null && GameEventManager._Instance.onGetGold != null) GameEventManager._Instance.onGetGold();
        }
        public void CostGold(decimal _goldNumber, Action<decimal> _successCallBack = null)
        {
            if (_goldNumber < 0)
            {
                Debug.Log("扣钱必须是正数");
                return;
            }

            if (m_kPlayerBasicAsset.m_kGold >= _goldNumber)
            {
                _successCallBack?.Invoke(_goldNumber);
            }
            else
            {
                UI_Helper.ShowCommonTips(131);
                return;
            }
            World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.SetTask(TaskType.Gold, (int) _goldNumber);
            UI_Helper.ShowCommonTips(UI_Helper.GetTextByLanguageID(127, _goldNumber.ToString()));
            m_kPlayerBasicAsset.m_kGold -= _goldNumber;
            DataManager._instance.FixLocalData<PlayerBasicAsset>(m_kPlayerBasicAsset.ID, m_kPlayerBasicAsset);
            ObserverHelper<decimal>.SendMessage(MessageMonitorType.GoldChange, this, new MessageArgs<decimal>(m_kPlayerBasicAsset.m_kGold));
        }
        public void AddStone(decimal _stoneNumber, Action<decimal> _successCallBack = null)
        {
            if (_stoneNumber <= 0)
            {
                Debug.Log("加钱必须是正数");
                return;
            }
            _successCallBack?.Invoke(_stoneNumber);
            UI_Helper.ShowCommonTips(UI_Helper.GetTextByLanguageID(124, _stoneNumber.ToString()));
            m_kPlayerBasicAsset.m_kStone += _stoneNumber;
            DataManager._instance.FixLocalData<PlayerBasicAsset>(m_kPlayerBasicAsset.ID, m_kPlayerBasicAsset);
            ObserverHelper<decimal>.SendMessage(MessageMonitorType.StoneChange, this, new MessageArgs<decimal>(m_kPlayerBasicAsset.m_kStone));
        }
        public void CostStone(decimal _stoneNumber, Action<decimal> _successCallBack = null)
        {

            if (_stoneNumber <= 0)
            {
                Debug.Log("扣钱必须是正数");
                return;
            }
            if (m_kPlayerBasicAsset.m_kStone >= _stoneNumber)
            {
                _successCallBack?.Invoke(_stoneNumber);
            }
            else
            {
                UI_Helper.ShowCommonTips(132);
                return;
            }

            World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.SetTask(TaskType.Stone, (int) _stoneNumber);
            UI_Helper.ShowCommonTips(UI_Helper.GetTextByLanguageID(128, _stoneNumber.ToString()));
            m_kPlayerBasicAsset.m_kStone -= _stoneNumber;
            DataManager._instance.FixLocalData<PlayerBasicAsset>(m_kPlayerBasicAsset.ID, m_kPlayerBasicAsset);
            ObserverHelper<decimal>.SendMessage(MessageMonitorType.StoneChange, this, new MessageArgs<decimal>(m_kPlayerBasicAsset.m_kStone));
        }

        public int GetPackLeftNumber()
        {
            int _bagVolume = m_kPlayerBagAsset.BagVolume;
            int LeftItemNumber = _bagVolume - m_kPlayerBagAsset.m_kBag.Count;
            for (int index = 0; index < m_kPlayerBagAsset.m_kBag.Count; index++)
            {
                CS_Items.DataEntry dataEntry = null;
                if (m_kPlayerBagAsset.m_kBag[index].m_kItemType == PlayerBagAsset.ItemType.Animal ||
                    m_kPlayerBagAsset.m_kBag[index].m_kItemType == PlayerBagAsset.ItemType.Building ||
                    m_kPlayerBagAsset.m_kBag[index].m_kItemType == PlayerBagAsset.ItemType.Botany)
                {
                    var ItemList = DBManager.Instance.m_kItems.m_kDataEntryTable.GetEnumerator();
                    while (ItemList.MoveNext())
                    {
                        if (ItemList.Current.Value._Use.Contains(m_kPlayerBagAsset.m_kBag[index].m_kItemID.ToString()))
                        {
                            dataEntry = ItemList.Current.Value;
                            break;
                        }
                    }
                }
                else
                {
                    dataEntry = DBManager.Instance.m_kItems.GetEntryPtr(m_kPlayerBagAsset.m_kBag[index].m_kItemID);
                    if (dataEntry == null)
                    {
                        continue;
                    }
                }
                int _max = int.Parse(DBManager.Instance.m_kDisperse.GetEntryPtr(20011)._Val1);
                int zu = m_kPlayerBagAsset.m_kBag[index].m_kCount / _max;
                if (m_kPlayerBagAsset.m_kBag[index].m_kCount % _max != 0)
                {
                    zu++;
                }

                LeftItemNumber -= (zu - 1);
            }
            return LeftItemNumber;

        }

        public bool CanPutInBag(int _ItemID, int _number)
        {
            int _bagVolume = World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.m_kPlayerBagAsset.BagVolume;
            int _max = int.Parse(DBManager.Instance.m_kDisperse.GetEntryPtr(20011)._Val1);
            for (int index = 0; index < World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.m_kPlayerBagAsset.m_kBag.Count; index++)
            {
                PlayerBagAsset.BagItem bagItem = World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.m_kPlayerBagAsset.m_kBag[index];
                CS_Items.DataEntry dataEntry = DBManager.Instance.m_kItems.GetEntryPtr(bagItem.m_kItemID);

                if (bagItem != null && dataEntry != null && _ItemID == bagItem.m_kItemID)
                {
                    int zu = (bagItem.m_kCount + _number) / _max;
                    if ((bagItem.m_kCount + _number) % _max != 0)
                    {
                        zu++;
                    }
                    _bagVolume -= (zu - 1);
                }

            }
            return _bagVolume >= 0;

        }

        public List<PlayerBagAsset.BagItem> GetBagsByType(PlayerBagAsset.ItemType _itemType)
        {
            List<PlayerBagAsset.BagItem> list = new List<PlayerBagAsset.BagItem>();
            for (int i = 0; i < m_kPlayerBagAsset.m_kBag.Count; i++)
            {
                if (m_kPlayerBagAsset.m_kBag[i].m_kItemType == _itemType)
                {
                    list.Add(m_kPlayerBagAsset.m_kBag[i]);
                }
            }
            return list;
        }
        public void AddItemToBag(int _id, PlayerBagAsset.ItemType _itemType, int _number)
        {
            bool have = false;

            for (int i = 0; i < m_kPlayerBagAsset.m_kBag.Count; i++)
            {
                if (m_kPlayerBagAsset.m_kBag[i].m_kItemID == _id)
                {
                    m_kPlayerBagAsset.m_kBag[i].m_kCount += _number;
                    have = true;
                    break;
                }
            }

            if (!have)
            {
                PlayerBagAsset.BagItem bagItem = new PlayerBagAsset.BagItem();
                bagItem.m_kItemID = _id;
                bagItem.m_kCount = _number;
                bagItem.m_kItemType = _itemType;
                m_kPlayerBagAsset.m_kBag.Add(bagItem);
            }
            DataManager._instance.FixLocalData<PlayerBagAsset>(m_kPlayerBagAsset.ID, m_kPlayerBagAsset);

            if (_itemType == PlayerBagAsset.ItemType.Animal || _itemType == PlayerBagAsset.ItemType.Building || _itemType == PlayerBagAsset.ItemType.Botany)
            {
                CS_Model.DataEntry dataEntry = DBManager.Instance.m_kModel.GetEntryPtr(_id);
                if (dataEntry != null)
                {
                    UI_Helper.ShowCommonTips(145, UI_Helper.GetTextByLanguageID(dataEntry._DisplayName), _number.ToString());
                }
                if (_itemType  == PlayerBagAsset.ItemType.Animal)
                if(GameEventManager._Instance != null && GameEventManager._Instance.onGetAnimal!= null) GameEventManager._Instance.onGetAnimal();
                else if (_itemType == PlayerBagAsset.ItemType.Botany)
                if (GameEventManager._Instance != null && GameEventManager._Instance.onGetPlant != null) GameEventManager._Instance.onGetPlant();
            }
            else if (_itemType == PlayerBagAsset.ItemType.Nutrients || _itemType == PlayerBagAsset.ItemType.StageProperty)
            {
                CS_Items.DataEntry dataEntry = DBManager.Instance.m_kItems.GetEntryPtr(_id);
                if (dataEntry != null)
                {
                    UI_Helper.ShowCommonTips(145, UI_Helper.GetTextByLanguageID(dataEntry._DisplayName), _number.ToString());
                }
            }
            if (_id == 900013)
            {
                ObserverHelper<decimal>.SendMessage(MessageMonitorType.SpadeChange, this, new MessageArgs<decimal>(1));
            }
            ObserverHelper<int>.SendMessage(MessageMonitorType.BagChange, this, new MessageArgs<int>(_number));
        }
        public PlayerBagAsset.BagItem SelectItem(int _id)
        {
            for (int i = 0; i < m_kPlayerBagAsset.m_kBag.Count; i++)
            {
                if (m_kPlayerBagAsset.m_kBag[i].m_kItemID == _id)
                {
                    return m_kPlayerBagAsset.m_kBag[i];
                }
            }
            return null;
        }
        public void CosetItem(int _id, PlayerBagAsset.ItemType itemType, int _number)
        {
            bool have = false;
            PlayerBagAsset.ItemType m_kItemType = PlayerBagAsset.ItemType.All;
            for (int i = 0; i < m_kPlayerBagAsset.m_kBag.Count; i++)
            {
                if (m_kPlayerBagAsset.m_kBag[i].m_kItemID == _id)
                {
                    have = true;
                    if (m_kPlayerBagAsset.m_kBag[i].m_kCount >= _number)
                    {
                        m_kPlayerBagAsset.m_kBag[i].m_kCount -= _number;
                        m_kItemType = m_kPlayerBagAsset.m_kBag[i].m_kItemType;
                        break;
                    }
                    if (m_kPlayerBagAsset.m_kBag[i].m_kCount == 0)
                    {
                        m_kPlayerBagAsset.m_kBag.RemoveAt(i);
                        break;
                    }
                }
            }
            if (_id == 900013)
            {
                ObserverHelper<decimal>.SendMessage(MessageMonitorType.SpadeChange, this, new MessageArgs<decimal>(1));
            }
            if (!have)
            {
                if (itemType == PlayerBagAsset.ItemType.Animal || itemType == PlayerBagAsset.ItemType.Botany)
                {
                    CS_Model.DataEntry dataEntry = DBManager.Instance.m_kModel.GetEntryPtr(_id);
                    if (dataEntry != null)
                    {
                        UI_Helper.ShowCommonTips(250, UI_Helper.GetTextByLanguageID(dataEntry._DisplayName));
                    }
                }
                else
                {
                    CS_Items.DataEntry dataEntry = DBManager.Instance.m_kItems.GetEntryPtr(_id);
                    if (dataEntry != null)
                    {
                        UI_Helper.ShowCommonTips(250, UI_Helper.GetTextByLanguageID(dataEntry._DisplayName));
                    }
                }

                return;
            }

            DataManager._instance.FixLocalData<PlayerBagAsset>(m_kPlayerBagAsset.ID, m_kPlayerBagAsset);
            World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.SetTask(TaskType.Item, (int) _number);
            if (m_kItemType == PlayerBagAsset.ItemType.Nutrients || m_kItemType == PlayerBagAsset.ItemType.AnimaiNutrients || m_kItemType == PlayerBagAsset.ItemType.BotanyNutrients)
            {
                World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.SetTask(TaskType.Usefeed, _number);
            }
            if (m_kItemType == PlayerBagAsset.ItemType.StageProperty)
            {
                World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.SetTask(TaskType.UseSpeedUpItem, _number);
            }

            ObserverHelper<int>.SendMessage(MessageMonitorType.BagChange, this, new MessageArgs<int>(_number));
        }
        public void UnlockGoods(int _shopId)
        {
            if (m_kPlayerShopAsset.m_kShop.ContainsKey(_shopId))
            {
                m_kPlayerShopAsset.m_kShop[_shopId] = true;
            }
        }

        decimal allWater = 99999999;
        public void AddWater(decimal _water, Action<decimal> _successCallBack = null)
        {
            if (_water <= 0)
            {
                Debug.Log("加水必须是正数");
                return;
            }
            if(ModelManager._instance != null && ModelManager._instance.GetModleByType(ModelCType.WaterPool).Count != 0)
                allWater = ModelManager._instance.GetModleByType(ModelCType.WaterPool)[0].cfg._Capacity;

            _successCallBack?.Invoke(_water);
            UI_Helper.ShowCommonTips(UI_Helper.GetTextByLanguageID(126, _water.ToString()));
            m_kPlayerBasicAsset.m_kWater += _water;
            if (m_kPlayerBasicAsset.m_kWater > allWater) m_kPlayerBasicAsset.m_kWater = allWater;

            DataManager._instance.FixLocalData<PlayerBasicAsset>(m_kPlayerBasicAsset.ID, m_kPlayerBasicAsset);
            ObserverHelper<decimal>.SendMessage(MessageMonitorType.WaterChange, this, new MessageArgs<decimal>(m_kPlayerBasicAsset.m_kWater));
           
            if(ModelManager._instance != null && ModelManager._instance.GetModleByType(ModelCType.WaterPool).Count != 0)
                SceneLogic._instance.SetWaterPool();
        }
        public void CostWater(decimal _water, Action<decimal> _successCallBack = null)
        {

            if (_water <= 0)
            {
                Debug.Log("消耗水必须是正数");
                return;
            }
            if (m_kPlayerBasicAsset.m_kWater >= _water)
            {
                _successCallBack?.Invoke(_water);
            }
            else
            {
                UI_Helper.ShowCommonTips(134);
                return;
            }

            UI_Helper.ShowCommonTips(UI_Helper.GetTextByLanguageID(130, _water.ToString()));
            m_kPlayerBasicAsset.m_kWater -= _water;
            if (m_kPlayerBasicAsset.m_kWater < 0) m_kPlayerBasicAsset.m_kWater = 0;

            DataManager._instance.FixLocalData<PlayerBasicAsset>(m_kPlayerBasicAsset.ID, m_kPlayerBasicAsset);
            ObserverHelper<decimal>.SendMessage(MessageMonitorType.WaterChange, this, new MessageArgs<decimal>(m_kPlayerBasicAsset.m_kWater));
            World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.SetTask(TaskType.Water, 1);

            SceneLogic._instance.SetWaterPool();
        }
        public void AddSun(decimal _sun, Action<decimal> _successCallBack = null)
        {
            if (_sun <= 0)
            {
                Debug.Log("加阳光必须是正数");
                return;
            }
            UI_Helper.ShowCommonTips(UI_Helper.GetTextByLanguageID(125, _sun.ToString()));
            _successCallBack?.Invoke(_sun);
            m_kPlayerBasicAsset.m_kSun += _sun;
            DataManager._instance.FixLocalData<PlayerBasicAsset>(m_kPlayerBasicAsset.ID, m_kPlayerBasicAsset);
            ObserverHelper<decimal>.SendMessage(MessageMonitorType.SunChange, this, new MessageArgs<decimal>(m_kPlayerBasicAsset.m_kSun));
        }
        public void CostSun(decimal _sun, Action<decimal> _successCallBack = null)
        {

            if (_sun <= 0)
            {
                Debug.Log("消耗水必须是正数");
                return;
            }
            if (m_kPlayerBasicAsset.m_kWater >= _sun)
            {
                _successCallBack?.Invoke(_sun);
            }
            else
            {
                UI_Helper.ShowCommonTips(133);
                return;
            }

            UI_Helper.ShowCommonTips(UI_Helper.GetTextByLanguageID(129, _sun.ToString()));
            m_kPlayerBasicAsset.m_kSun -= _sun;
            DataManager._instance.FixLocalData<PlayerBasicAsset>(m_kPlayerBasicAsset.ID, m_kPlayerBasicAsset);
            ObserverHelper<decimal>.SendMessage(MessageMonitorType.SunChange, this, new MessageArgs<decimal>(m_kPlayerBasicAsset.m_kSun));
        }
        /// <summary>
        /// 铲子
        /// </summary>
        /// <param name="_spade"></param>
        /// <param name="_successCallBack"></param>
        public void CostSpade(decimal _spade, Action<decimal> _successCallBack = null)
        {

            if (_spade <= 0)
            {
                return;
            }
            if (m_kSpade >= _spade)
            {
                _successCallBack?.Invoke(_spade);
            }
            else
            {
                UI_Helper.ShowCommonTips(133);
                return;
            }
            Debug.Log("消耗铲子" + _spade);
            CosetItem(900013, PlayerBagAsset.ItemType.StageProperty, (int) _spade);
            DataManager._instance.FixLocalData<PlayerBasicAsset>(m_kPlayerBasicAsset.ID, m_kPlayerBasicAsset);
        }
        public void AddToSettlements(Guid _guid)
        {
            if (!m_kSettlementsAsset.m_kListSettlement.Contains(_guid))
            {
                m_kSettlementsAsset.m_kListSettlement.Add(_guid);
                DataManager._instance.FixLocalData<SettlementsAsset>(m_kSettlementsAsset.ID, m_kSettlementsAsset);

            }
        }
        public void RemoveSettlements(Guid _guid)
        {
            m_kSettlementsAsset.m_kListSettlement.Remove(_guid);
            DataManager._instance.FixLocalData<SettlementsAsset>(m_kSettlementsAsset.ID, m_kSettlementsAsset);
        }
        public void ChangleName(string _name)
        {
            m_kPlayerBasicAsset.m_kName = _name;
            ObserverHelper<string>.SendMessage(MessageMonitorType.NameChange, this, new MessageArgs<string>(m_kPlayerBasicAsset.m_kName));
        }
        //typ 1是增加, 2是删除
        public void WasteLand(int typ, Guid guid)
        {
            bool isHave = m_kPlayerBasicAsset.m_lWasteLand.Contains(guid);
            if (typ == 1)
            {
                if (!isHave)
                {
                    m_kPlayerBasicAsset.m_lWasteLand.Add(guid);
                    DataManager._instance.FixLocalData<PlayerBasicAsset>(m_kPlayerBasicAsset.ID, m_kPlayerBasicAsset);
                    Debug.Log("___11~~" + guid + "___count__" + m_kPlayerBasicAsset.m_lWasteLand.Count);
                }

            }
            else if (typ == 2)
            {
                if (isHave)
                {
                    List<Guid> temp = new List<Guid>();
                    for (int i = 0; i < m_kPlayerBasicAsset.m_lWasteLand.Count; i++)
                    {
                        if (m_kPlayerBasicAsset.m_lWasteLand[i] != guid)
                        {
                            temp.Add(m_kPlayerBasicAsset.m_lWasteLand[i]);
                        }
                    }
                    m_kPlayerBasicAsset.m_lWasteLand = temp;

                    if (m_kPlayerBasicAsset.m_lWasteLand.Count == 0)
                    {
                        m_kPlayerBasicAsset.m_beginWaste = default(DateTime);
                    }
                    Debug.Log("____22222____" + guid + "__count__" + m_kPlayerBasicAsset.m_lWasteLand.Count);
                    DataManager._instance.FixLocalData<PlayerBasicAsset>(m_kPlayerBasicAsset.ID, m_kPlayerBasicAsset);

                }
            }

        }
        public void AddVitality(uint _vitality)
        {
            m_kPlayerBasicAsset.m_UIntVitality = _vitality;
        }
        public void GetVitalityReward(int _taskID)
        {
            if (!m_kPlayerBasicAsset.m_GetVitality.Contains(_taskID))
            {
                m_kPlayerBasicAsset.m_GetVitality.Add(_taskID);
            }
        }

        public void SetTaskNumber(QTFramework.TaskType _TaskType, int _Number = 1)
        {
            m_kPlayerBasicAsset.m_VitalityNumber[_TaskType] = _Number;
        }
        public void SetTask(QTFramework.TaskType _TaskType, int _Number = 1)
        {
            m_kPlayerBasicAsset.m_VitalityNumber[_TaskType] += _Number;
            if(_TaskType == TaskType.assart)//开荒
            {
                m_kPlayerBasicAsset.m_nWastedLand++;
                if (GameEventManager._Instance.onRemoveWasteland != null) GameEventManager._Instance.onRemoveWasteland();
            }
                
            

        }
        public void ResetTask()
        {
            List<TaskType> _type = m_kPlayerBasicAsset.m_VitalityNumber.Keys.ToList();

            foreach (var item in _type)
            {
                m_kPlayerBasicAsset.m_VitalityNumber[item] = 0;
            }
            m_kPlayerBasicAsset.m_GetVitality.Clear();
        }

        /// <summary>
        /// 铲子
        /// </summary>
        public decimal m_kSpade
        {
            get
            {
                PlayerBagAsset.BagItem bagItem = SelectItem(900013);
                if (bagItem == null)
                {
                    return 0;
                }
                else
                {
                    return bagItem.m_kCount;
                }

            }
        }
    }
}
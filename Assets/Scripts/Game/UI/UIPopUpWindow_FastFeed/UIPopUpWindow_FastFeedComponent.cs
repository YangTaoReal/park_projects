using QTFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ObjectEventSystem]
public class UIPopUpWindow_FastFeedComponentAwakeSystem : AAwake<UIPopUpWindow_FastFeedComponent>
{
    public override void Awake(UIPopUpWindow_FastFeedComponent _self)
    {
        _self.Awake();
    }
}


[UIEntityComponent(UI_PrefabPath.m_sUIPopUpWindow_FastFeed)]
public class UIPopUpWindow_FastFeedComponent : UIComponent
{
    public Text m_TextTitle;
    public Text m_TextTips;
    public Button m_Button_Close;
    public ScrollRect m_ScrollRect_Item;
    public Button m_Button_Ok;
    public Text m_Text_Ok;

    int costWater;
    bool isCan;//能否喂养
    List<Dictionary<int, int>> listCost;

    //private List<BaseData> baseDataList;
    List<PlayerBagAsset.BagItem> tempBag = new List<PlayerBagAsset.BagItem>();
    internal void Awake()
    {
        m_TextTitle = m_kParentEntity.m_kUIPrefab.GetCacheComponent(0) as Text;
        m_TextTips = m_kParentEntity.m_kUIPrefab.GetCacheComponent(1) as Text;
        m_Button_Close = m_kParentEntity.m_kUIPrefab.GetCacheComponent(2) as Button;
        m_ScrollRect_Item = m_kParentEntity.m_kUIPrefab.GetCacheComponent(3) as ScrollRect;
        m_Button_Ok = m_kParentEntity.m_kUIPrefab.GetCacheComponent(4) as Button;
        m_Text_Ok = m_kParentEntity.m_kUIPrefab.GetCacheComponent(5) as Text;

        m_Button_Ok.onClick.AddListener(onClick_Ok);
        m_Button_Close.onClick.AddListener(onClick_Close);
        ObserverHelper<int>.AddEventListener(MessageMonitorType.BagChange, notificationBagChange);
    }
    /// <summary>
    /// 界面多语言
    /// </summary>
    public override void TranslateUI()
    {
        base.TranslateUI();
        m_TextTitle.text = UI_Helper.GetTextByLanguageID(254);
        m_TextTips.text = UI_Helper.GetTextByLanguageID(255);
        m_Text_Ok.text = UI_Helper.GetTextByLanguageID(141);
    }
    /// <summary>
    /// 关闭界面
    /// </summary>
    private void onClick_Close()
    {
        World.Scene.GetComponent<UIManagerComponent>().Remove(UI_PrefabPath.m_sUIPopUpWindow_FastFeed);
    }
    /// <summary>
    /// 同意一键喂养
    /// </summary>
    private void onClick_Ok()
    {
        if (!isCan) return;

        Player player = World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer;
        player.AddAsset(PlayerBagAsset.ItemType.Water, -costWater);
        for (int i = 0; i < listCost.Count; i++) 
        {
            foreach(var cost in listCost[i])
            {
                var CS = DBManager.Instance.m_kItems.GetEntryPtr(cost.Key);
                player.CosetItem(cost.Key, (PlayerBagAsset.ItemType)CS._ItemType, cost.Value); 
            }
        }
 
        World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.SetTask(TaskType.Feed, 1);
            
    }

    /// <summary>
    /// 买了药品到背包后刷新界面
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void notificationBagChange(object sender, MessageArgs<int> args)
    {
        UpdateList();
    }


    /// <summary>
    /// 外部初始化界面
    /// </summary>
    /// <param name="lbaseData"></param>

    public void Init(int numWater, List<Dictionary<int, int>> _listCost)
    {
        costWater = numWater;
        listCost = _listCost;
        UpdateList();
    }


    public void UpdateList()
    {
        Player player = World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer;

        GetEntity<UIEntity>().ClearChildren();
        m_ScrollRect_Item.verticalNormalizedPosition = 1;
        UIEntity uIEntity = World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIFastFeed_Item);
        m_kParentEntity.AddChildren(uIEntity);
        UIFastFeed_ItemComponent fastFeed_Item = uIEntity.GetComponent<UIFastFeed_ItemComponent>();
        uIEntity.m_kUIPrefab.gameObject.transform.SetParent(m_ScrollRect_Item.content);
        uIEntity.m_kUIPrefab.gameObject.transform.localPosition = Vector3.zero;
        fastFeed_Item.Init(100003, costWater);
        int numCan = 0;
        int numNeed = 1;
        if (costWater <= (int)player.m_kPlayerBasicAsset.m_kWater) numCan++;
        for (int i = 0; i < listCost.Count; i++)
        {
            var listcost = listCost[i];
            foreach(var cost in listcost)
            {
                numNeed++;
                UIEntity _uIEntity = World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIFastFeed_Item);
                m_kParentEntity.AddChildren(_uIEntity);
                UIFastFeed_ItemComponent _fastFeed_Item = _uIEntity.GetComponent<UIFastFeed_ItemComponent>();
                _uIEntity.m_kUIPrefab.gameObject.transform.SetParent(m_ScrollRect_Item.content);
                _uIEntity.m_kUIPrefab.gameObject.transform.localPosition = Vector3.zero;
                _fastFeed_Item.Init(cost.Key, cost.Value);

                if (cost.Value <= player.SelectItem(cost.Key).m_kCount) numCan++;
            }
        }



        if (numCan == numNeed) isCan = true;
 

    }


    ///// <summary>
    ///// 计算需要的东西
    ///// </summary>
    //private void calculateNeedItem()
    //{
  


    //    Player player = World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer;
    //    player.SelectItem
      

    //    PlayerBagAsset playerBagAsset = World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.m_kPlayerBagAsset;
    //    for (int i = 0; i < playerBagAsset.m_kBag.Count; i++)
    //    {
    //        var data = playerBagAsset.m_kBag[i];
    //        if (data.m_kItemType == PlayerBagAsset.ItemType.Nutrients) lNutrients.Add(data);
    //        else if (data.m_kItemType == PlayerBagAsset.ItemType.AnimaiNutrients) lAnimalNutrients.Add(data);
    //        else if (data.m_kItemType == PlayerBagAsset.ItemType.BotanyNutrients) lBotanyNutrients.Add(data);
    //        else if (data.m_kItemType == PlayerBagAsset.ItemType.Medicine) lMedicine.Add(data);
    //        else if (data.m_kItemType == PlayerBagAsset.ItemType.AnimaiMedicine) lAnimalMedicine.Add(data);
    //        else if (data.m_kItemType == PlayerBagAsset.ItemType.BotanyMedicine) lBotanyMedicine.Add(data);
    //    }

    //    // 类型 |  配置表cid, 数量
    //    Dictionary<PlayerBagAsset.ItemType, Dictionary<int, decimal>> dicResult = new Dictionary<PlayerBagAsset.ItemType, Dictionary<int, decimal>>();
    //    dicResult[PlayerBagAsset.ItemType.Nutrients] = new Dictionary<int, decimal>();
    //    dicResult[PlayerBagAsset.ItemType.AnimaiNutrients] = new Dictionary<int, decimal>();
    //    dicResult[PlayerBagAsset.ItemType.BotanyNutrients] = new Dictionary<int, decimal>();
    //    dicResult[PlayerBagAsset.ItemType.Medicine] = new Dictionary<int, decimal>();
    //    dicResult[PlayerBagAsset.ItemType.AnimaiMedicine] = new Dictionary<int, decimal>();
    //    dicResult[PlayerBagAsset.ItemType.BotanyMedicine] = new Dictionary<int, decimal>();


    //    GetEntity<UIEntity>().ClearChildren();
    //    m_ScrollRect_Item.verticalNormalizedPosition = 1;

    //    UIEntity uIEntity = World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIFastFeed_Item);
    //    m_kParentEntity.AddChildren(uIEntity);
    //    UIFastFeed_ItemComponent _fastFeed_Item = uIEntity.GetComponent<UIFastFeed_ItemComponent>();
    //    uIEntity.m_kUIPrefab.gameObject.transform.SetParent(m_ScrollRect_Item.content);
    //    uIEntity.m_kUIPrefab.gameObject.transform.localPosition = Vector3.zero;
    //    _fastFeed_Item.Init(1111,111);
    //}
    ///// <summary>
    ///// 单独计算需要的数量
    ///// </summary>
    ///// <returns>The expend item.</returns>
    ///// <param name="bagItem">Bag item.</param>
    ///// <param name="ExpAllNum">Exp all number.</param>
    //List<decimal> CalcExpendItem(PlayerBagAsset.BagItem bagItem, decimal ExpAllNum, decimal fixNum = default(decimal))
    //{
    //    List<decimal> lresult = new List<decimal>();
    //    CS_Items.DataEntry CS_Items = DBManager.Instance.m_kItems.GetEntryPtr(bagItem.m_kItemID);
    //    string[] splits = CS_Items._Use.Split(' ');
    //    int addVal = int.Parse(splits[2]);

    //    int needNum = (int)Math.Ceiling(Convert.ToDecimal(ExpAllNum / addVal));
    //    decimal count = bagItem.m_kCount;
    //    if (fixNum != 0)
    //        count = fixNum;
    //    if (count >= needNum)
    //    {
    //        lresult.Add(needNum);
    //        lresult.Add(0);
    //    }
    //    else
    //    {
    //        lresult.Add(bagItem.m_kCount);
    //        lresult.Add(ExpAllNum - bagItem.m_kCount * addVal);
    //    }
    //    return lresult;
    //}

    //List<int> CalcFodderItem(List<BaseData> lbaseData)
    //{
    //    List<int> lFodder = new List<int>();
    //    int water = 0; //水
    //    int animalFood = 0; //动物饲料
    //    int plantFood = 0; //植物饲料
    //    int animalMedicine = 0; //动物药剂
    //    int plantMedicine = 0; //植物药剂

    //    for (int i = 0; i < lbaseData.Count; i++)
    //    {
    //        BaseData baseData = lbaseData[i];
    //        ModelBase _modelBase = baseData.GetComponent<ModelBase>();
    //        BaseServer server = _modelBase.GetbaseServer;
    //        List<List<int>> m_lStatePro = _modelBase.GetStatePro;

    //        for (int j = 0; j < m_lStatePro.Count; j++)
    //        {
    //            List<int> lstate = m_lStatePro[j];
    //            int curval = (int)server.proVal[j];
    //            var state = (StatePro)lstate[0];
    //            var maxval = lstate[2];
    //            if (baseData.cfg._Type == (int)ModeTyp.Animal)
    //            {
    //                if (state == StatePro.Thirst)
    //                    water += maxval - curval;
    //                else if (state == StatePro.Hunger)
    //                    animalFood += maxval - curval;
    //            }
    //            else if (baseData.cfg._Type == (int)ModeTyp.Plant)
    //            {
    //                if (state == StatePro.Thirst)
    //                    water += maxval - curval;
    //                else if (state == StatePro.Hunger)
    //                    plantFood += maxval - curval;
    //            }
    //        }
    //    }
    //    lFodder.Add(water);
    //    lFodder.Add(animalFood);
    //    lFodder.Add(plantFood);
    //    return lFodder;
    //}

    public delegate void Event();

    void OpenBox(string title, string content, string ensure, Event cbEvent = null)
    {
        //
        UIEntity _uien = UI_Helper.ShowConfirmPanel("TextChange",
            () =>
            {
                if (cbEvent != null)
                    cbEvent();
            },
            () => { });
        _uien.GetComponent<UI_Confirm>().TextChange(title, content, ensure);
    }
}

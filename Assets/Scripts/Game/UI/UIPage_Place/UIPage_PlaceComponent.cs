using System;
using System.Collections;
using System.Collections.Generic;

using QTFramework;

using UnityEngine;
using UnityEngine.UI;

[ObjectEventSystem]
public class UIPage_PlaceComponentAwakeSystem : AAwake<UIPage_PlaceComponent>
{
    public override void Awake(UIPage_PlaceComponent _self)
    {
        _self.Awake();
    }
}

[UIEntityComponent(UI_PrefabPath.m_sUIPage_Place)]
public class UIPage_PlaceComponent : UIComponent
{
    public Toggle m_kToggleWarehouse;
    public Toggle m_kToggleSettlements;
    public Text m_kTextWarehouse;
    public Text m_kTextSettlements;

    public Toggle m_kToggleAll;
    public Toggle m_kToggleAnimal;
    public Toggle m_kToggleBotany;

    public Text m_kTextAll;
    public Text m_kTextAnimal;
    public Text m_kTextBotany;

    public ScrollRect m_kScrollRectScrollSettlements;
    public ScrollRect m_kScrollRectScrollWarehouse;
    public Button m_kButtonClose;
    private PlayerBagAsset.ItemType m_kWarehouseType;
    internal void Awake()
    {
        m_kToggleWarehouse = m_kParentEntity.m_kUIPrefab.GetCacheComponent(0) as Toggle;
        m_kToggleSettlements = m_kParentEntity.m_kUIPrefab.GetCacheComponent(1) as Toggle;

        m_kToggleAll = m_kParentEntity.m_kUIPrefab.GetCacheComponent(2) as Toggle;
        m_kToggleAnimal = m_kParentEntity.m_kUIPrefab.GetCacheComponent(3) as Toggle;
        m_kToggleBotany = m_kParentEntity.m_kUIPrefab.GetCacheComponent(4) as Toggle;

        m_kTextWarehouse = m_kParentEntity.m_kUIPrefab.GetCacheComponent(5) as Text;
        m_kTextSettlements = m_kParentEntity.m_kUIPrefab.GetCacheComponent(6) as Text;

        m_kTextAll = m_kParentEntity.m_kUIPrefab.GetCacheComponent(7) as Text;
        m_kTextAnimal = m_kParentEntity.m_kUIPrefab.GetCacheComponent(8) as Text;
        m_kTextBotany = m_kParentEntity.m_kUIPrefab.GetCacheComponent(9) as Text;

        m_kScrollRectScrollWarehouse = m_kParentEntity.m_kUIPrefab.GetCacheComponent(10) as ScrollRect;
        m_kScrollRectScrollSettlements = m_kParentEntity.m_kUIPrefab.GetCacheComponent(11) as ScrollRect;

        m_kButtonClose = m_kParentEntity.m_kUIPrefab.GetCacheComponent(12) as Button;

        m_kToggleWarehouse.onValueChanged.AddListener(onValueChanged_Warehouse);
        m_kToggleSettlements.onValueChanged.AddListener(onValueChanged_Settlements);
        m_kToggleAll.onValueChanged.AddListener(onValueChanged_All);
        m_kToggleAnimal.onValueChanged.AddListener(onValueChanged_Animal);
        m_kToggleBotany.onValueChanged.AddListener(onValueChanged_Botany);

        m_kButtonClose.onClick.AddListener(onClick_Close);

        m_kToggleWarehouse.isOn = true;
        m_kToggleAll.isOn = true;
        m_kWarehouseType = PlayerBagAsset.ItemType.All;
        InitWarehouse();
    }

    private void onClick_Close()
    {
        World.Scene.GetComponent<UIManagerComponent>().Remove(UI_PrefabPath.m_sUIPage_Place);
    }

    public override void Dispose()
    {
        base.Dispose();
        m_kToggleWarehouse.onValueChanged.RemoveListener(onValueChanged_Warehouse);
        m_kToggleSettlements.onValueChanged.RemoveListener(onValueChanged_Settlements);
        m_kToggleAll.onValueChanged.RemoveListener(onValueChanged_All);
        m_kToggleAnimal.onValueChanged.RemoveListener(onValueChanged_Animal);
        m_kToggleBotany.onValueChanged.RemoveListener(onValueChanged_Botany);
        m_kButtonClose.onClick.RemoveListener(onClick_Close);
    }

    /// <summary>
    /// 植物
    /// </summary>
    /// <param name="arg0"></param>
    private void onValueChanged_Botany(bool arg0)
    {
        if (arg0)
        {
            m_kWarehouseType = PlayerBagAsset.ItemType.Botany;
            InitWarehouse();
        }
    }

    /// <summary>
    /// 动物
    /// </summary>
    /// <param name="arg0"></param>
    private void onValueChanged_Animal(bool arg0)
    {
        if (arg0)
        {
            m_kWarehouseType = PlayerBagAsset.ItemType.Animal;
            InitWarehouse();
        }
    }

    /// <summary>
    /// 所有
    /// </summary>
    /// <param name="arg0"></param>
    private void onValueChanged_All(bool arg0)
    {
        if (arg0)
        {
            m_kWarehouseType = PlayerBagAsset.ItemType.All;
            InitWarehouse();
        }
    }

    /// <summary>
    /// 仓库
    /// </summary>
    /// <param name="arg0"></param>
    private void onValueChanged_Warehouse(bool arg0)
    {
        if (arg0)
        {
            m_kWarehouseType = PlayerBagAsset.ItemType.All;
            InitWarehouse();
        }
    }

    /// <summary>
    /// 安置点
    /// </summary>
    /// <param name="arg0"></param>
    private void onValueChanged_Settlements(bool arg0)
    {
        InitSettlements();
    }

    /// <summary>
    /// 仓库
    /// </summary>
    private void InitWarehouse()
    {
        GetEntity<UIEntity>().ClearChildren();

        List<PlayerBagAsset.BagItem> bagItems = World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.m_kPlayerBagAsset.m_kBag;
        List<PlayerBagAsset.BagItem> SelectItem = new List<PlayerBagAsset.BagItem>();

        if (m_kWarehouseType == PlayerBagAsset.ItemType.All)
        {
            SelectItem = bagItems;
        }
        else
        {
            for (int i = 0; i < bagItems.Count; i++)
            {
                if (bagItems[i].m_kItemType == m_kWarehouseType)
                {
                    SelectItem.Add(bagItems[i]);
                }
            }
        }

        for (int index = 0; index < SelectItem.Count; index++)
        {
            CS_Items.DataEntry dataEntry = null;
            var ItemsMoveMent = DBManager.Instance.m_kItems.m_kDataEntryTable.GetEnumerator();
            while (ItemsMoveMent.MoveNext())
            {
                if (ItemsMoveMent.Current.Value._Use.Contains(SelectItem[index].m_kItemID.ToString()))
                {
                    dataEntry = ItemsMoveMent.Current.Value;
                    break;
                }
            }

            if (dataEntry == null)
            {
                continue;
            }
            int _max = int.Parse(DBManager.Instance.m_kDisperse.GetEntryPtr(20011)._Val1);
            int zu = SelectItem[index].m_kCount / _max;
            if (SelectItem[index].m_kCount % _max != 0)
            {
                zu++;
            }
            for (int k = 0; k < zu; k++)
            {
                UIEntity uIEntity = World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPage_Place_Item);
                m_kParentEntity.AddChildren(uIEntity);
                UIPage_Place_ItemComponent _UIPage_Place_Item = uIEntity.GetComponent<UIPage_Place_ItemComponent>();
                uIEntity.m_kUIPrefab.gameObject.transform.SetParent(m_kScrollRectScrollWarehouse.content);
                uIEntity.m_kUIPrefab.gameObject.transform.localPosition = Vector3.zero;
                _UIPage_Place_Item.ClickCallBack = ItemClickCallBack;
                if (k == zu - 1)
                {
                    _UIPage_Place_Item.InitItem(SelectItem[index].m_kItemID, SelectItem[index].m_kItemType, SelectItem[index].m_kCount - k * _max);
                }
                else
                {
                    _UIPage_Place_Item.InitItem(SelectItem[index].m_kItemID, SelectItem[index].m_kItemType, _max);
                }
            }
        }
    }

    private void ItemClickCallBack()
    {
        InitWarehouse();
    }

    private void InitSettlements()
    {
        GetEntity<UIEntity>().ClearChildren();
        List<BaseData> lBaseData = new List<BaseData>();
        List<BaseData> temp = new List<BaseData>();
        List<Guid> _settlements = World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.m_kSettlementsAsset.m_kListSettlement;
        if (m_kWarehouseType == PlayerBagAsset.ItemType.All)
        {
            for (int i = 0; i < _settlements.Count; i++)
            {
                temp.Add(ModelManager._instance.GetModelByGuid(_settlements[i]));
            }
            lBaseData = temp;
        }
        else if (m_kWarehouseType == PlayerBagAsset.ItemType.Animal)
        {
            for (int i = 0; i < _settlements.Count; i++)
            {
                BaseData baseData = ModelManager._instance.GetModelByGuid(_settlements[i]);
                if (baseData.cfg._Type == (int) ModeTyp.Animal) { temp.Add(baseData); }
            }
            lBaseData = temp;
        }
        else if (m_kWarehouseType == PlayerBagAsset.ItemType.Botany)
        {
            for (int i = 0; i < _settlements.Count; i++)
            {
                BaseData baseData = ModelManager._instance.GetModelByGuid(_settlements[i]);
                if (baseData.cfg._Type == (int) ModeTyp.Plant) { temp.Add(baseData); }
            }
            lBaseData = temp;
        }

        for (int i = 0; i < lBaseData.Count; i++)
        {
            BaseData baseData = lBaseData[i];

            UIEntity uIEntity = World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPage_Pack_TemporaryItem);
            m_kParentEntity.AddChildren(uIEntity);
            UIPage_Pack_TemporaryItem _UIPage_Pack_TemporaryItem = uIEntity.GetComponent<UIPage_Pack_TemporaryItem>();
            uIEntity.m_kUIPrefab.gameObject.transform.SetParent(m_kScrollRectScrollSettlements.content);
            uIEntity.m_kUIPrefab.gameObject.transform.localPosition = Vector3.zero;
            _UIPage_Pack_TemporaryItem.InitItem(baseData);
        }
    }
}
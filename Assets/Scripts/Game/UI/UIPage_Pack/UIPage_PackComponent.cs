using System;
using System.Collections;
using System.Collections.Generic;

using QTFramework;

using UnityEngine;
using UnityEngine.UI;

[ObjectEventSystem]
public class UIPage_PackComponentAwakeSystem : AAwake<UIPage_PackComponent>
{
    public override void Awake(UIPage_PackComponent _self)
    {
        _self.Awake();
    }
}

[UIEntityComponent(UI_PrefabPath.m_sUIPage_Pack)]
public class UIPage_PackComponent : UIComponent
{
    public Toggle m_kToggleWarehouse;
    public Toggle m_kToggleSettlements;
    public Text m_kTextWarehouse;
    public Text m_kTextSettlements;

    public Toggle m_kToggleAll;
    public Toggle m_kToggleAnimal;
    public Toggle m_kToggleBotany;
    public Toggle m_kToggleNutrients;
    public Toggle m_kToggleStageProperty;
    public Image m_kImageAll;
    public Image m_kImageAnimal;
    public Image m_kImageBotany;
    public Image m_kImageNutrients;
    public Image m_kImageStageProperty;
    public Text m_kTextAll;
    public Text m_kTextAnimal;
    public Text m_kTextBotany;
    public Text m_kTextNutrients;
    public Text m_kTextStageProperty;

    public RectTransform m_kRectTransformWarehousebottomNode;
    public RectTransform m_kRectTransformSettlementsbottomNode;

    public Slider m_kSliderCapacity;
    public Text m_kTextCapacity;

    public Button m_kButtonEnlarge;
    public Text m_kTextEnlarge;

    public ScrollRect m_kScrollRectScrollSettlements;
    public ScrollRect m_kScrollRectScrollWarehouse;

    public Button m_kButtonClose;
    public Button m_btnOneKeyFeed;

    private PlayerBagAsset.ItemType m_kWarehouseType;

    private bool isPlace;
    List<BaseData> nowBaseData = new List<BaseData>();

    public enum Category
    {
        Warehouse, //仓库
        Settlements //安置点
    }

    private Category category;
    internal void Awake()
    {
        m_kToggleWarehouse = m_kParentEntity.m_kUIPrefab.GetCacheComponent(0) as Toggle;
        m_kToggleSettlements = m_kParentEntity.m_kUIPrefab.GetCacheComponent(1) as Toggle;
        m_kTextWarehouse = m_kParentEntity.m_kUIPrefab.GetCacheComponent(2) as Text;
        m_kTextSettlements = m_kParentEntity.m_kUIPrefab.GetCacheComponent(3) as Text;

        m_kToggleAll = m_kParentEntity.m_kUIPrefab.GetCacheComponent(4) as Toggle;
        m_kToggleAnimal = m_kParentEntity.m_kUIPrefab.GetCacheComponent(5) as Toggle;
        m_kToggleBotany = m_kParentEntity.m_kUIPrefab.GetCacheComponent(6) as Toggle;
        m_kToggleNutrients = m_kParentEntity.m_kUIPrefab.GetCacheComponent(7) as Toggle;
        m_kToggleStageProperty = m_kParentEntity.m_kUIPrefab.GetCacheComponent(8) as Toggle;
        m_kImageAll = m_kParentEntity.m_kUIPrefab.GetCacheComponent(9) as Image;
        m_kImageAnimal = m_kParentEntity.m_kUIPrefab.GetCacheComponent(10) as Image;
        m_kImageBotany = m_kParentEntity.m_kUIPrefab.GetCacheComponent(11) as Image;
        m_kImageNutrients = m_kParentEntity.m_kUIPrefab.GetCacheComponent(12) as Image;
        m_kImageStageProperty = m_kParentEntity.m_kUIPrefab.GetCacheComponent(13) as Image;
        m_kTextAll = m_kParentEntity.m_kUIPrefab.GetCacheComponent(14) as Text;
        m_kTextAnimal = m_kParentEntity.m_kUIPrefab.GetCacheComponent(15) as Text;
        m_kTextBotany = m_kParentEntity.m_kUIPrefab.GetCacheComponent(16) as Text;
        m_kTextNutrients = m_kParentEntity.m_kUIPrefab.GetCacheComponent(17) as Text;
        m_kTextStageProperty = m_kParentEntity.m_kUIPrefab.GetCacheComponent(18) as Text;

        m_kRectTransformWarehousebottomNode = m_kParentEntity.m_kUIPrefab.GetCacheComponent(19) as RectTransform;
        m_kRectTransformSettlementsbottomNode = m_kParentEntity.m_kUIPrefab.GetCacheComponent(20) as RectTransform;

        m_kSliderCapacity = m_kParentEntity.m_kUIPrefab.GetCacheComponent(21) as Slider;
        m_kTextCapacity = m_kParentEntity.m_kUIPrefab.GetCacheComponent(22) as Text;

        m_kButtonEnlarge = m_kParentEntity.m_kUIPrefab.GetCacheComponent(23) as Button;
        m_kTextEnlarge = m_kParentEntity.m_kUIPrefab.GetCacheComponent(24) as Text;

        m_kScrollRectScrollSettlements = m_kParentEntity.m_kUIPrefab.GetCacheComponent(25) as ScrollRect;
        m_kScrollRectScrollWarehouse = m_kParentEntity.m_kUIPrefab.GetCacheComponent(26) as ScrollRect;

        m_kButtonClose = m_kParentEntity.m_kUIPrefab.GetCacheComponent(27) as Button;
        m_btnOneKeyFeed = m_kParentEntity.m_kUIPrefab.GetCacheComponent(28) as Button;

        m_kToggleWarehouse.onValueChanged.AddListener(onValueChanged_Warehouse);
        m_kToggleSettlements.onValueChanged.AddListener(onValueChanged_Settlements);
        m_kToggleAll.onValueChanged.AddListener(onValueChanged_All);
        m_kToggleAnimal.onValueChanged.AddListener(onValueChanged_Animal);
        m_kToggleBotany.onValueChanged.AddListener(onValueChanged_Botany);
        m_kToggleNutrients.onValueChanged.AddListener(onValueChanged_Nutrients);
        m_kToggleStageProperty.onValueChanged.AddListener(onValueChanged_StageProperty);

        m_kButtonClose.onClick.AddListener(OnButtonClick_Close);
        m_btnOneKeyFeed.onClick.AddListener(OnBtnOneKeyFeed);

        m_kButtonEnlarge.onClick.AddListener(OnButtonClick_Enlarge);

        m_kToggleWarehouse.isOn = false;
        m_kToggleAll.isOn = true;
        category = Category.Warehouse;
        nowBaseData.Clear();

        ObserverHelper<bool>.AddEventListener(MessageMonitorType.RefreshPackUI, RefreshPackUI);
    }

    private void OnButtonClick_Enlarge()
    {
        World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPopUpWindow_BackpackCapacity);
    }

    private void RefreshPackUI(object sender, MessageArgs<bool> args)
    {
        if (category == Category.Warehouse)
        {
            InitWarehouse();
        }
        else
        {
            InitSettlements();
        }
    }

    public override void TranslateUI()
    {
        base.TranslateUI();
        m_kTextAll.text = UI_Helper.GetTextByLanguageID(100);
        m_kTextAnimal.text = UI_Helper.GetTextByLanguageID(103);
        m_kTextBotany.text = UI_Helper.GetTextByLanguageID(101);
        m_kTextNutrients.text = UI_Helper.GetTextByLanguageID(102);
        m_kTextStageProperty.text = UI_Helper.GetTextByLanguageID(104);
        m_kTextWarehouse.text = UI_Helper.GetTextByLanguageID(119);
        m_kTextSettlements.text = UI_Helper.GetTextByLanguageID(118);
    }

    public override void Dispose()
    {
        base.Dispose();

        m_kToggleWarehouse.onValueChanged.RemoveListener(onValueChanged_Warehouse);
        m_kToggleSettlements.onValueChanged.RemoveListener(onValueChanged_Settlements);
        m_kToggleAll.onValueChanged.RemoveListener(onValueChanged_All);
        m_kToggleAnimal.onValueChanged.RemoveListener(onValueChanged_Animal);
        m_kToggleBotany.onValueChanged.RemoveListener(onValueChanged_Botany);
        m_kToggleNutrients.onValueChanged.RemoveListener(onValueChanged_Nutrients);
        m_kToggleStageProperty.onValueChanged.RemoveListener(onValueChanged_StageProperty);
        m_kButtonClose.onClick.RemoveListener(OnButtonClick_Close);
        m_btnOneKeyFeed.onClick.RemoveAllListeners();
    }
    private void OnButtonClick_Close()
    {
        World.Scene.GetComponent<UIManagerComponent>().Remove(UI_PrefabPath.m_sUIPage_Pack);
        CheckGuidance();
    }

    private void OnBtnOneKeyFeed()
    {
        if (nowBaseData.Count == 0) return;
        SceneLogic._instance.QuickFeed(nowBaseData);
    }

    void CheckGuidance(UIEntity entity = null)
    {
        if (GuidanceManager.isGuidancing)
        {
            GuidanceData data = new GuidanceData();
            data.entity = entity;
            ObserverHelper<GuidanceData>.SendMessage(MessageMonitorType.GuidanceClickEvent, this, new MessageArgs<GuidanceData>(data));
        }
    }

    /// <summary>
    /// 道具
    /// </summary>
    /// <param name="arg0"></param>
    private void onValueChanged_StageProperty(bool arg0)
    {
        if (arg0)
        {
            m_kWarehouseType = PlayerBagAsset.ItemType.StageProperty;
            if (category == Category.Warehouse)
            {
                InitWarehouse();
            }
            else
            {
                InitSettlements();
            }
        }

    }

    /// <summary>
    /// 养料
    /// </summary>
    /// <param name="arg0"></param>
    private void onValueChanged_Nutrients(bool arg0)
    {
        if (arg0)
        {
            m_kWarehouseType = PlayerBagAsset.ItemType.Nutrients;
            if (category == Category.Warehouse)
            {
                InitWarehouse();
            }
            else
            {
                InitSettlements();
            }
        }
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
            if (category == Category.Warehouse)
            {
                InitWarehouse();
            }
            else
            {
                InitSettlements();
            }
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
            if (category == Category.Warehouse)
            {
                InitWarehouse();
            }
            else
            {
                InitSettlements();
            }
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
            if (category == Category.Warehouse)
            {
                InitWarehouse();
            }
            else
            {
                InitSettlements();
            }
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
            category = Category.Warehouse;
            m_kScrollRectScrollWarehouse.gameObject.SetActive(true);
            m_kScrollRectScrollSettlements.gameObject.SetActive(false);

            m_kRectTransformWarehousebottomNode.gameObject.SetActive(true);
            m_kRectTransformSettlementsbottomNode.gameObject.SetActive(false);
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
        if (arg0)
        {
            category = Category.Settlements;
            m_kScrollRectScrollWarehouse.gameObject.SetActive(false);
            m_kScrollRectScrollSettlements.gameObject.SetActive(true);
            m_kRectTransformWarehousebottomNode.gameObject.SetActive(false);
            m_kRectTransformSettlementsbottomNode.gameObject.SetActive(true);
            InitSettlements();
        }
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

        if (isPlace)
        {
            for (int index = SelectItem.Count - 1; index >= 0; index--)
            {
                if (SelectItem[index].m_kItemType != PlayerBagAsset.ItemType.Animal && SelectItem[index].m_kItemType != PlayerBagAsset.ItemType.Botany)
                {
                    SelectItem.RemoveAt(index);
                }
            }
        }
        int _max = int.Parse(DBManager.Instance.m_kDisperse.GetEntryPtr(20011)._Val1);

        int _bagVolume = World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.m_kPlayerBagAsset.BagVolume;
        int LeftItemNumber = _bagVolume - SelectItem.Count;
        for (int index = 0; index < SelectItem.Count; index++)
        {
            int zu = SelectItem[index].m_kCount / _max;

            if (SelectItem[index].m_kCount % _max != 0)
            {
                zu++;
            }

            LeftItemNumber -= (zu - 1);
            for (int k = 0; k < zu; k++)
            {
                UIEntity uIEntity = World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPage_Pack_Item);
                m_kParentEntity.AddChildren(uIEntity);
                UIPage_Pack_ItemComponent _UIPage_Pack_Item = uIEntity.GetComponent<UIPage_Pack_ItemComponent>();
                uIEntity.m_kUIPrefab.gameObject.transform.SetParent(m_kScrollRectScrollWarehouse.content);
                uIEntity.m_kUIPrefab.gameObject.transform.localPosition = Vector3.zero;
                if (isPlace)
                {
                    _UIPage_Pack_Item.ClickCallBack = ItemClickCallBack;
                }
                else
                {
                    _UIPage_Pack_Item.ClickCallBack = null;
                }

                if (k == zu - 1)
                {
                    _UIPage_Pack_Item.InitItem(SelectItem[index], SelectItem[index].m_kCount - k * _max);

                }
                else
                {
                    _UIPage_Pack_Item.InitItem(SelectItem[index], _max);
                }
            }
        }

        m_kSliderCapacity.value = (_bagVolume - LeftItemNumber) / (float) _bagVolume;
        m_kTextCapacity.text = UI_Helper.GetTextByLanguageID(232, $"{_bagVolume - LeftItemNumber}/{_bagVolume}");

        while (LeftItemNumber > 0)
        {
            UIEntity uIEntity = World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPage_Pack_Item);
            m_kParentEntity.AddChildren(uIEntity);
            UIPage_Pack_ItemComponent _UIPage_Pack_Item = uIEntity.GetComponent<UIPage_Pack_ItemComponent>();
            uIEntity.m_kUIPrefab.gameObject.transform.SetParent(m_kScrollRectScrollWarehouse.content);
            uIEntity.m_kUIPrefab.gameObject.transform.localPosition = Vector3.zero;
            _UIPage_Pack_Item.InitItem();
            LeftItemNumber--;
        }

    }

    public void InitSettlements()
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

        nowBaseData = lBaseData;
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

    private void ItemClickCallBack()
    {
        InitWarehouse();
    }

    public void InitUI(bool _isPlace, Category _category)
    {
        category = _category;
        isPlace = _isPlace;
        m_kToggleNutrients.gameObject.SetActive(!isPlace);
        m_kToggleStageProperty.gameObject.SetActive(!isPlace);
        if (category == Category.Warehouse)
        {
            m_kToggleWarehouse.isOn = true;
            InitWarehouse();
        }
        else
        {
            m_kToggleSettlements.isOn = true;
            InitSettlements();
        }
    }
}
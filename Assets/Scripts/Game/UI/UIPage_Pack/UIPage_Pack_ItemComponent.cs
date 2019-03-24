using QTFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ObjectEventSystem]
public class UIPage_Pack_ItemComponentAwakeSystem : AAwake<UIPage_Pack_ItemComponent>
{
    public override void Awake(UIPage_Pack_ItemComponent _self)
    {
        _self.Awake();
    }
}


[UIEntityComponent(UI_PrefabPath.m_sUIPage_Pack_Item)]
public class UIPage_Pack_ItemComponent : UIComponent
{
    public Image m_kImageBg;
    public RawImage m_kRawImageIcon;
    public Text m_kTextNumber;
    public Text m_kTextName;

    public Button m_kButtonItem;
    public RectTransform m_kRectTransformContent;

    private PlayerBagAsset.BagItem Item;

    public Action ClickCallBack;
    internal void Awake()
    {
        m_kImageBg = m_kParentEntity.m_kUIPrefab.GetCacheComponent(0) as Image;
        m_kRawImageIcon = m_kParentEntity.m_kUIPrefab.GetCacheComponent(1) as RawImage;
        m_kTextNumber = m_kParentEntity.m_kUIPrefab.GetCacheComponent(2) as Text;
        m_kTextName = m_kParentEntity.m_kUIPrefab.GetCacheComponent(3) as Text;
        m_kButtonItem = m_kParentEntity.m_kUIPrefab.GetCacheComponent(4) as Button;
        m_kRectTransformContent = m_kParentEntity.m_kUIPrefab.GetCacheComponent(5) as RectTransform;
        m_kButtonItem.onClick.RemoveAllListeners();
        m_kButtonItem.onClick.AddListener(OnButtonClick_ButtonItem);
        Item = null;
    }
    public override void Dispose()
    {
        base.Dispose();
        ClickCallBack = null;
        m_kButtonItem.onClick.RemoveListener(OnButtonClick_ButtonItem);
    }

    private void OnButtonClick_ButtonItem()
    {
        // 判断新手引导
        //CheckGuidance(m_kParentEntity.ParentEntity as UIEntity);
        GuidanceManager._Instance.CheckGuidance(m_kParentEntity.ParentEntity as UIEntity);
        if (ClickCallBack == null && Item != null)
        {
           UIEntity uIEntity = World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPage_PackItemDetails);
            UIPage_PackItemDetailsComponent uIPage_PackItemDetailsComponent = uIEntity.GetComponent<UIPage_PackItemDetailsComponent>();
            uIPage_PackItemDetailsComponent.InitUI(Item);
            return;
        }

        if (Item == null)
        {
            World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPopUpWindow_BackpackCapacity);
            return;
        }

        CS_Model.DataEntry dataEntry = DBManager.Instance.m_kModel.GetEntryPtr(Item.m_kItemID);
        if (dataEntry == null)
        {
            return;
        }

        bool iSCreat = false;

        if (dataEntry._Type == (int)ModeTyp.Animal)
        {
            iSCreat = ModelManager._instance.CreateAnimalPark(dataEntry._ID);
            if(iSCreat) GameEventManager._Instance.onPlaceAnimalOrPlant();
        }
        else if (dataEntry._Type == (int)ModeTyp.Plant)
        {
            iSCreat = ModelManager._instance.CreatePlantPark(dataEntry._ID);
            if (iSCreat) GameEventManager._Instance.onPlaceAnimalOrPlant();
        }

        if (ClickCallBack != null)
        {
            if (iSCreat)
            {
                World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.CosetItem(Item.m_kItemID, Item.m_kItemType, 1);
            }
            else
            {
                UI_Helper.ShowCommonTips(UI_Helper.GetTextByLanguageID(218));
            }
            ClickCallBack?.Invoke();
            //// 判断新手引导
            //CheckGuidance(m_kParentEntity.ParentEntity as UIEntity);
        }

       
    }
    // 新手引导
    //private void CheckGuidance(UIEntity entity)
    //{
    //    if(GuidanceManager.isGuidancing)
    //    {
    //        GuidanceData data = new GuidanceData();
    //        data.entity = entity;
    //        ObserverHelper<GuidanceData>.SendMessage(MessageMonitorType.GuidanceClickEvent, this, new MessageArgs<GuidanceData>(data));
    //    }
    //}
    public void InitItem()
    {
        Item = null;
        m_kRectTransformContent.gameObject.SetActive(false);
        m_kImageBg.sprite = UI_Helper.GetSprite("bg003");
    }
    public void InitItem(PlayerBagAsset.BagItem _Item,int _Number)
    {
        m_kImageBg.sprite = UI_Helper.GetSprite("bg002");
        m_kRectTransformContent.gameObject.SetActive(true);
        Item = _Item;

        if (Item.m_kItemType == PlayerBagAsset.ItemType.Animal
          || Item.m_kItemType == PlayerBagAsset.ItemType.Building
          || Item.m_kItemType == PlayerBagAsset.ItemType.Botany)
        {
            CS_Model.DataEntry dataEntry = DBManager.Instance.m_kModel.GetEntryPtr(Item.m_kItemID);
            if (dataEntry == null)
            {
                return;
            }
            m_kRawImageIcon.texture = UI_Helper.AllocTexture(dataEntry._Icon);
            m_kTextName.text = UI_Helper.GetTextByLanguageID(dataEntry._DisplayName);
        }
        else
        {
            CS_Items.DataEntry dataEntry = DBManager.Instance.m_kItems.GetEntryPtr(Item.m_kItemID);
            if (dataEntry == null)
            {
                return;
            }
            m_kRawImageIcon.texture = UI_Helper.AllocTexture(dataEntry._Icon);
            m_kTextName.text = UI_Helper.GetTextByLanguageID(dataEntry._DisplayName);
        }


        m_kTextNumber.text = _Number.ToString();

    }
}

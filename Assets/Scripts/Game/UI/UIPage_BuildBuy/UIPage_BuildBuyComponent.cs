using System;
using System.Collections;
using System.Collections.Generic;

using QTFramework;

using UnityEngine;
using UnityEngine.UI;

[ObjectEventSystem]
public class UIPage_BuildBuyComponentAwakeSystem : AAwake<UIPage_BuildBuyComponent>
{
    public override void Awake(UIPage_BuildBuyComponent _self)
    {
        _self.Awake();
    }
}

[ObjectEventSystem]
public class UIPage_BuildBuyComponentFixedUpdateSystem : AFixedUpdate<UIPage_BuildBuyComponent>
{
    public override void FixedUpdate(UIPage_BuildBuyComponent _self)
    {
        _self.FixedUpdate();
    }
}

[UIEntityComponent(UI_PrefabPath.m_sUIPage_BuildBuy)]
public class UIPage_BuildBuyComponent : UIComponent
{
    public Button m_kButtonClose;
    public Button m_kButtonRotation;
    public Button m_kButtonGold;
    public Button m_kButtonStone;

    public RectTransform m_kRectTransformOr;
    public RectTransform m_kRectTransformGold;
    public RectTransform m_kRectTransformStone;

    public Text m_kTextGoldNumber;
    public Text m_kTextStoneNumber;

    private int m_kShopID;
    public void Awake()
    {
        m_kButtonClose = m_kParentEntity.m_kUIPrefab.GetCacheComponent(0) as Button;
        m_kButtonRotation = m_kParentEntity.m_kUIPrefab.GetCacheComponent(1) as Button;
        m_kButtonGold = m_kParentEntity.m_kUIPrefab.GetCacheComponent(2) as Button;
        m_kButtonStone = m_kParentEntity.m_kUIPrefab.GetCacheComponent(3) as Button;

        m_kRectTransformOr = m_kParentEntity.m_kUIPrefab.GetCacheComponent(4) as RectTransform;
        m_kRectTransformGold = m_kParentEntity.m_kUIPrefab.GetCacheComponent(5) as RectTransform;
        m_kRectTransformStone = m_kParentEntity.m_kUIPrefab.GetCacheComponent(6) as RectTransform;

        m_kTextGoldNumber = m_kParentEntity.m_kUIPrefab.GetCacheComponent(7) as Text;
        m_kTextStoneNumber = m_kParentEntity.m_kUIPrefab.GetCacheComponent(8) as Text;

        m_kButtonClose.onClick.AddListener(OnButtonClick_Close);
        m_kButtonRotation.onClick.AddListener(OnButtonClick_Rotation);
        m_kButtonGold.onClick.AddListener(OnButtonClick_GoldBuy);
        m_kButtonStone.onClick.AddListener(OnButtonClick_StoneBuy);

    }

    public void Init(int _shopID)
    {
        m_kShopID = _shopID;
        CS_Shop.DataEntry shopDataEntry = DBManager.Instance.m_kShop.GetEntryPtr(m_kShopID);
        if (shopDataEntry == null)
        {
            return;
        }
        Debug.Log($"m_kBuildingID:{shopDataEntry._Goods.y}");

        m_kTextGoldNumber.text = shopDataEntry._GoldPrice.z.ToString();
        m_kTextStoneNumber.text = shopDataEntry._StonePrice.z.ToString();

        bool ISdouble = true;
        if (shopDataEntry._GoldPrice == Vector3Int.zero || shopDataEntry._StonePrice == Vector3Int.zero)
        {
            ISdouble = false;
        }
        m_kRectTransformOr.gameObject.SetActive(ISdouble);

        m_kButtonGold.gameObject.SetActive(shopDataEntry._GoldPrice != Vector3Int.zero);
        m_kButtonStone.gameObject.SetActive(shopDataEntry._StonePrice != Vector3Int.zero);
    }

    private void OnButtonClick_StoneBuy()
    {

        if (MapGridMgr.Instance.CanEndEdit())
        {
            CS_Shop.DataEntry shopDataEntry = DBManager.Instance.m_kShop.GetEntryPtr(m_kShopID);
            if (shopDataEntry == null)
            {
                return;
            }

            World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.CostStone((decimal) shopDataEntry._StonePrice.z, CallBackStoneBuy);
        }
        else
        {
            UI_Helper.ShowCommonTips(247);
            MapGridMgr.Instance.CancelEdit();
        }
        CloseUI();
    }
    private void CallBackStoneBuy(decimal _price)
    {
        MapGridMgr.Instance.EndEdit();
        CheckGuidance(ParentEntity as UIEntity);
    }

    private void OnButtonClick_GoldBuy()
    {

        if (MapGridMgr.Instance.CanEndEdit())
        {

            CS_Shop.DataEntry shopDataEntry = DBManager.Instance.m_kShop.GetEntryPtr(m_kShopID);
            if (shopDataEntry == null)
            {
                return;
            }

            World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.CostGold((decimal) shopDataEntry._GoldPrice.z, CallBackGoldBuy);
        }
        else
        {
            UI_Helper.ShowCommonTips(247);
            MapGridMgr.Instance.CancelEdit();
        }
        CloseUI();
    }
    private void CallBackGoldBuy(decimal _price)
    {
        MapGridMgr.Instance.EndEdit();
        //CheckGuidance(ParentEntity as UIEntity);
    }
    private void CheckGuidance(UIEntity entity)
    {
        if (GuidanceManager.isGuidancing)
        {
            GuidanceData data = new GuidanceData();
            data.entity = entity;
            ObserverHelper<GuidanceData>.SendMessage(MessageMonitorType.GuidanceClickEvent, this, new MessageArgs<GuidanceData>(data));
        }
    }

    private void OnButtonClick_Rotation()
    {
        MapGridMgr.Instance.RotateEdit();
    }

    private void OnButtonClick_Close()
    {
        UI_Helper.ShowCommonTips(248);
        CloseUI();
    }

    private void CloseUI()
    {
        MapGridMgr.Instance.CancelEdit();
        World.Scene.GetComponent<UIManagerComponent>().ClearUIStack();
        World.Scene.GetComponent<UIManagerComponent>().Remove(UI_PrefabPath.m_sUIPage_BuildBuy);
    }

    public void FixedUpdate()
    {

    }
    public override void Dispose()
    {
        m_kButtonClose.onClick.RemoveListener(OnButtonClick_Close);
        m_kButtonRotation.onClick.RemoveListener(OnButtonClick_Rotation);
        m_kButtonGold.onClick.RemoveListener(OnButtonClick_GoldBuy);
        m_kButtonStone.onClick.RemoveListener(OnButtonClick_StoneBuy);
    }

}
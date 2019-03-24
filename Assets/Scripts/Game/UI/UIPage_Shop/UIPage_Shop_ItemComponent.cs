using System;
using System.Collections;
using System.Collections.Generic;

using QTFramework;

using UnityEngine;
using UnityEngine.UI;

[ObjectEventSystem]
public class UIPage_Shop_ItemComponentAwakeSystem : AAwake<UIPage_Shop_ItemComponent>
{
    public override void Awake(UIPage_Shop_ItemComponent _self)
    {
        _self.Awake();
    }
}

[ObjectEventSystem]
public class UIPage_Shop_ItemComponentFixedUpdateSystem : AFixedUpdate<UIPage_Shop_ItemComponent>
{
    public override void FixedUpdate(UIPage_Shop_ItemComponent _self)
    {
        _self.FixedUpdate();
    }
}

[UIEntityComponent(UI_PrefabPath.m_sUIPage_Shop_Item)]
public class UIPage_Shop_ItemComponent : UIComponent
{

    public RawImage m_kRawImageIcon;
    public Text m_kTextNumber;
    public Text m_kTextName;
    public Text m_kTextGoldNumber;
    public Text m_kTextStoneNumber;
    public Button m_kButtonItem;
    private int ShopID;
    internal void Awake()
    {
        m_kRawImageIcon = m_kParentEntity.m_kUIPrefab.GetCacheComponent(0) as RawImage;
        m_kTextNumber = m_kParentEntity.m_kUIPrefab.GetCacheComponent(1) as Text;
        m_kTextName = m_kParentEntity.m_kUIPrefab.GetCacheComponent(2) as Text;
        m_kTextGoldNumber = m_kParentEntity.m_kUIPrefab.GetCacheComponent(3) as Text;
        m_kTextStoneNumber = m_kParentEntity.m_kUIPrefab.GetCacheComponent(4) as Text;
        m_kButtonItem = m_kParentEntity.m_kUIPrefab.GetCacheComponent(5) as Button;

        m_kButtonItem.onClick.AddListener(OnButtonClick_ButtonItem);
    }

    private void OnButtonClick_ButtonItem()
    {
        UIEntity uIEntity = World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPage_ShopBuy);
        uIEntity.GetComponent<UIPage_ShopBuyComponent>().Init(ShopID);
    }

    internal void FixedUpdate()
    {

    }
    public void InitItem(int _ShopID)
    {
        ShopID = _ShopID;
        CS_Shop.DataEntry ShopItem = DBManager.Instance.m_kShop.GetEntryPtr(_ShopID);
        if (ShopItem == null)
        {
            return;
        }
        m_kRawImageIcon.texture = UI_Helper.AllocTexture(ShopItem._Icon);
        m_kTextName.text = UI_Helper.GetTextByLanguageID(ShopItem._DisplayName);
        m_kTextGoldNumber.text = ShopItem._GoldPrice.z.ToString();
        m_kTextStoneNumber.text = ShopItem._StonePrice.z.ToString();

        if (ShopItem._Category == (int)ShopCategory.Animal || ShopItem._Category == (int)ShopCategory.Botany || ShopItem._Category == (int)ShopCategory.Building)
        {
            CS_Model.DataEntry _mode = DBManager.Instance.m_kModel.GetEntryPtr((int)ShopItem._Goods.y);
            if (_mode == null)
            {
                return;
            }
            m_kButtonItem.GetComponent<Image>().sprite = UI_Helper.SetQualityBG(1,_mode._Quality);

        }
        else
        {
            CS_Items.DataEntry items = DBManager.Instance.m_kItems.GetEntryPtr((int)ShopItem._Goods.y);
            if (items == null)
            {
                return;
            }
            m_kButtonItem.GetComponent<Image>().sprite = UI_Helper.SetQualityBG(1,items._Quality);
        }
    }
}
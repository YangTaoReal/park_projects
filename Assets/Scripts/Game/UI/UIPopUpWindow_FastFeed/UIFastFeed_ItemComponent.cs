using QTFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ObjectEventSystem]
public class UIFastFeed_ItemComponentAwakeSystem : AAwake<UIFastFeed_ItemComponent>
{
    public override void Awake(UIFastFeed_ItemComponent _self)
    {
        _self.Awake();
    }
}


[UIEntityComponent(UI_PrefabPath.m_sUIFastFeed_Item)]
public class UIFastFeed_ItemComponent : UIComponent
{
    public RawImage m_RawImageIcon;
    public Text m_TextNumber;
    public Button m_ButtonAdd;

    int cidItem;
    int numItem;
    internal void Awake()
    {
        m_RawImageIcon = m_kParentEntity.m_kUIPrefab.GetCacheComponent(0) as RawImage;
        m_TextNumber = m_kParentEntity.m_kUIPrefab.GetCacheComponent(1) as Text;
        m_ButtonAdd = m_kParentEntity.m_kUIPrefab.GetCacheComponent(2) as Button;

        m_ButtonAdd.onClick.AddListener(onClick_Add);
        ObserverHelper<int>.AddEventListener(MessageMonitorType.BagChange, notificationBagChange);
    }

    private void onClick_Add()
    {
        UIEntity uIEntity = World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPage_ShopBuy);
        uIEntity.GetComponent<UIPage_ShopBuyComponent>().Init(cidItem);
    }

    private void notificationBagChange(object sender, MessageArgs<int> args)
    {
        UpdateUI();
    }

    public void Init(int _itemID, int needNumer)
    {
        cidItem = _itemID;
        numItem = needNumer;

        UpdateUI();
    }

    void UpdateUI()
    {
        Player player = World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer;
        CS_Items.DataEntry dataEntry = DBManager.Instance.m_kItems.GetEntryPtr(cidItem);
        if (dataEntry == null)
        {
            Debug.Log($"不存在ID为{cidItem}的物品");
            return;
        }
        m_RawImageIcon.texture = UI_Helper.AllocTexture(dataEntry._Icon);

        int haveCount = 0;
        if (dataEntry._ItemType == (int)PlayerBagAsset.ItemType.Water)
        {
            haveCount = (int)player.m_kPlayerBasicAsset.m_kWater;
        }
        else
        {
            PlayerBagAsset.BagItem bagItem = player.SelectItem(cidItem);
            if (bagItem != null)
            {
                haveCount = bagItem.m_kCount;
            }
        }
 
        if (numItem > haveCount)
        {
            m_TextNumber.text = $"<color=#a4ffff>{numItem}</color>/{haveCount}";
            if (dataEntry._ItemType == (int)PlayerBagAsset.ItemType.Water)
                m_ButtonAdd.gameObject.SetActive(false);
            else
                m_ButtonAdd.gameObject.SetActive(true);
        }
        else
        {
            m_TextNumber.text = $"<color=#FFFFFF>{numItem}</color>/{haveCount}";
            m_ButtonAdd.gameObject.SetActive(false);
        }
    }
}

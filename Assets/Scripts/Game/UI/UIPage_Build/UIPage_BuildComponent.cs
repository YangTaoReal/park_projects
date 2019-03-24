using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using QTFramework;

using UnityEngine;
using UnityEngine.UI;

[ObjectEventSystem]
public class UIPage_BuildComponentAwakeSystem : AAwake<UIPage_BuildComponent>
{
    public override void Awake(UIPage_BuildComponent _self)
    {
        _self.Awake();
    }
}

[UIEntityComponent(UI_PrefabPath.m_sUIPage_Build)]
public class UIPage_BuildComponent : UIComponent
{
    public ScrollRect m_kScrollRectNode
    {
        get;
        set;
    }
    public Button m_kButtonClose
    {
        get;
        set;
    }
    public void Awake()
    {
        m_kScrollRectNode = m_kParentEntity.m_kUIPrefab.GetCacheComponent(0) as ScrollRect;
        m_kButtonClose = m_kParentEntity.m_kUIPrefab.GetCacheComponent(1) as Button;
        m_kButtonClose.onClick.AddListener(OnButtonClick_Close);

        Init();
    }
    public override void TranslateUI()
    {
        base.TranslateUI();
    }
    public override void Dispose()
    {
        m_kButtonClose.onClick.RemoveListener(OnButtonClick_Close);
        m_kScrollRectNode.horizontal = true;
    }

    private void OnButtonClick_Close()
    {
        World.Scene.GetComponent<UIManagerComponent>().Remove(UI_PrefabPath.m_sUIPage_Build);

        if (GuidanceManager.isGuidancing)
        {
            GuidanceData data = new GuidanceData();
            data.entity = null;
            ObserverHelper<GuidanceData>.SendMessage(MessageMonitorType.GuidanceClickEvent, this, new MessageArgs<GuidanceData>(data));
        }
    }

    public void Init()
    {
        m_kScrollRectNode.content.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
        Dictionary<int, UIEntity> m_kUIEntity = new Dictionary<int, UIEntity>();

        PlayerShopAsset playerShopAsset = World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.m_kPlayerShopAsset;
        int count = 0;
        var ShopItem = playerShopAsset.m_kShop.GetEnumerator();
        while (ShopItem.MoveNext())
        {
            CS_Shop.DataEntry ShopItemDataEntry = DBManager.Instance.m_kShop.GetEntryPtr(ShopItem.Current.Key);
            if (ShopItemDataEntry == null)
            {
                continue;
            }
            if (ShopItem.Current.Value && ShopItemDataEntry._Category == (int) ShopCategory.Building)
            {
                UIEntity uIEntity = World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPage_Build_Item);
                m_kParentEntity.AddChildren(uIEntity);
                UIPage_BuildComponent_Item uIPage_BuildComponent_Item = uIEntity.GetComponent<UIPage_BuildComponent_Item>();
                uIEntity.m_kUIPrefab.gameObject.transform.SetParent(m_kScrollRectNode.content);
                uIEntity.m_kUIPrefab.gameObject.transform.localPosition = Vector3.zero;
                m_kUIEntity[uIEntity.m_kUIPrefab.gameObject.GetInstanceID()] = uIEntity;

                uIPage_BuildComponent_Item.InitItem(ShopItem.Current.Key);

                DragBuild _dragBuild = uIEntity.m_kUIPrefab.gameObject.GetComponent<DragBuild>();
                if (_dragBuild == null)
                {
                    _dragBuild = uIEntity.m_kUIPrefab.gameObject.AddComponent<DragBuild>();
                }
                _dragBuild.scrollRect = m_kScrollRectNode;
                _dragBuild.m_kCreatActionCallBack = uIPage_BuildComponent_Item.CreatBuild;
                count++;
                if (count == 8)
                {
                    break;
                }
            }
        }
        // m_kScrollRectNode.onValueChanged = OnValueChange;
    }

    private void OnValueChange(int arg1, UIEntity arg2)
    {
        PlayerBuildingAsset playerBuildingAsset = World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.m_kPlayerBuildingAsset;
        var modelItemList = playerBuildingAsset.m_kDictionaryBuilding.ToList();
        arg2.GetComponent<UIPage_BuildComponent_Item>().InitItem(modelItemList[arg1].Key);
    }
}
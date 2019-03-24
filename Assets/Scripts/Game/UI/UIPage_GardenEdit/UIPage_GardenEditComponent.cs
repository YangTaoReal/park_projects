using System;
using System.Collections;
using System.Collections.Generic;

using QTFramework;

using UnityEngine;
using UnityEngine.UI;

[ObjectEventSystem]
public class UIPage_GardenEditComponentAwakeSystem : AAwake<UIPage_GardenEditComponent>
{
    public override void Awake(UIPage_GardenEditComponent _self)
    {
        _self.Awake();
    }
}

[UIEntityComponent(UI_PrefabPath.m_sUIPage_GardenEdit)]
public class UIPage_GardenEditComponent : UIComponent
{
    public Button m_kButtonClose;
    public Button m_kButtonRecycle;
    public Toggle m_kToggleSmart;
    public Toggle m_kToggleBig;

    public Button m_kButtonStone;
    public Button m_kButtonGold;
    public Button m_kButtonok;

    public RectTransform m_kRectTransformOr;

    public Text m_kTextStone;
    public Text m_kTextGold;

    private decimal TotalGoldPrice;
    private decimal TotalStonePrice;
    public void Awake()
    {

        m_kButtonClose = m_kParentEntity.m_kUIPrefab.GetCacheComponent(0) as Button;
        m_kButtonRecycle = m_kParentEntity.m_kUIPrefab.GetCacheComponent(1) as Button;

        m_kToggleSmart = m_kParentEntity.m_kUIPrefab.GetCacheComponent(2) as Toggle;
        m_kToggleBig = m_kParentEntity.m_kUIPrefab.GetCacheComponent(3) as Toggle;

        m_kButtonStone = m_kParentEntity.m_kUIPrefab.GetCacheComponent(4) as Button;
        m_kButtonGold = m_kParentEntity.m_kUIPrefab.GetCacheComponent(5) as Button;
        m_kButtonok = m_kParentEntity.m_kUIPrefab.GetCacheComponent(6) as Button;

        m_kRectTransformOr = m_kParentEntity.m_kUIPrefab.GetCacheComponent(7) as RectTransform;

        m_kTextStone = m_kParentEntity.m_kUIPrefab.GetCacheComponent(8) as Text;
        m_kTextGold = m_kParentEntity.m_kUIPrefab.GetCacheComponent(9) as Text;

        m_kButtonClose.onClick.AddListener(OnButtonClick_Close);
        m_kButtonRecycle.onClick.AddListener(OnButtonClick_Recycle);

        m_kToggleSmart.onValueChanged.AddListener(onValueChanged_Smart);
        m_kToggleBig.onValueChanged.AddListener(onValueChanged_Big);

        m_kButtonStone.onClick.AddListener(OnButtonClick_Stone);
        m_kButtonGold.onClick.AddListener(OnButtonClick_Gold);
        m_kButtonok.onClick.AddListener(OnButtonClick_ok);

        m_kToggleBig.isOn = true;

        MapGridMgr.Instance.onReEdit = RefreshPrice;

        RefreshPrice(System.Guid.Empty);

    }
    private void OnButtonClick_Close()
    {
        MapGridMgr.Instance.CancelEdit();
        World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPage_Build);
        World.Scene.GetComponent<UIManagerComponent>().ClearUIStack();

    }
    private void OnButtonClick_Recycle()
    {
        World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPopUpWindow_Demolition);
    }

    private void OnButtonClick_Stone()
    {
        if (MapGridMgr.Instance.CanEndEdit())
        {
            World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.CostGold(TotalStonePrice, CallBackStoneBuy);
        }
        else
        {
            UI_Helper.ShowCommonTips(247);
        }
    }

    private void CallBackStoneBuy(decimal obj)
    {
        EndEdit();
    }

    private void OnButtonClick_Gold()
    {
        if (MapGridMgr.Instance.CanEndEdit())
        {
            World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.CostGold(TotalGoldPrice, CallBackGoldBuy);
        }
        else
        {
            UI_Helper.ShowCommonTips(247);
        }
    }

    private void CallBackGoldBuy(decimal obj)
    {
        EndEdit();
    }

    private void EndEdit()
    {
        MapGridMgr.Instance.EndEdit();

        World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPage_Build);
        World.Scene.GetComponent<UIManagerComponent>().ClearUIStack();
    }

    private void OnButtonClick_ok()
    {

        MapGridMgr.Instance.EndEdit();

        World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPage_Build);
        World.Scene.GetComponent<UIManagerComponent>().ClearUIStack();
    }
    private void onValueChanged_Big(bool arg0)
    {
        if (arg0)
        {
            MapGridMgr.Instance.ExpandEdit();
        }
    }

    private void onValueChanged_Smart(bool arg0)
    {
        if (arg0)
        {
            MapGridMgr.Instance.ReduceEdit();
        }

    }

    public override void Dispose()
    {
        m_kButtonClose.onClick.RemoveListener(OnButtonClick_Close);
        m_kButtonRecycle.onClick.RemoveListener(OnButtonClick_Recycle);
        m_kToggleSmart.onValueChanged.RemoveListener(onValueChanged_Smart);
        m_kToggleBig.onValueChanged.RemoveListener(onValueChanged_Big);
        m_kButtonStone.onClick.RemoveListener(OnButtonClick_Stone);
        m_kButtonGold.onClick.RemoveListener(OnButtonClick_Gold);
        m_kButtonok.onClick.RemoveListener(OnButtonClick_ok);
    }

    private void RefreshPrice(System.Guid guids)
    {
        //20501  20502  20503
        int index = 0;
        TileInfo tileInfo = MapGridMgr.Instance.GetEditoringInfo();
        if (tileInfo.baseData.cfg._ID == 20501)
        {
            index = 0;
        }
        else if (tileInfo.baseData.cfg._ID == 20502)
        {
            index = 1;
        }
        else if (tileInfo.baseData.cfg._ID == 20503)
        {
            index = 2;
        }
        decimal _GoldPrice = 0;
        decimal _StonePrice = 0;

        CS_MapInfo.DataEntry dataEntry = DBManager.Instance.m_kMapInfo.GetEntryPtr(1000000);
        if (dataEntry != null)
        {
            string[] MapArguments = dataEntry._Arguments.Split(';');
            for (int i = 0; i < MapArguments.Length; i++)
            {
                string[] par = MapArguments[i].Split('=');
                if (par[0] == "ZhaLanJinBi")
                {
                    string[] level = par[1].Split('|');
                    _GoldPrice = Convert.ToDecimal(level[index]);
                }
                if (par[0] == "ZhaLanZhuanShi")
                {
                    string[] level = par[1].Split('|');
                    _StonePrice = Convert.ToDecimal(level[index]);
                }
            }
        }

        TotalGoldPrice = _GoldPrice * MapGridMgr.Instance.ExpandCount;
        TotalStonePrice = _StonePrice * MapGridMgr.Instance.ExpandCount;

        m_kTextGold.text = TotalGoldPrice.ToString();
        m_kTextStone.text = TotalStonePrice.ToString();

    }

}
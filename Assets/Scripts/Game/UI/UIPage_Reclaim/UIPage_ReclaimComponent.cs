using System;
using System.Collections;
using System.Collections.Generic;

using QTFramework;

using UnityEngine;
using UnityEngine.UI;

[ObjectEventSystem]
public class UIPage_ReclaimComponentAwakeSystem : AAwake<UIPage_ReclaimComponent>
{
    public override void Awake(UIPage_ReclaimComponent _self)
    {
        _self.Awake();
    }
}

[UIEntityComponent(UI_PrefabPath.m_sUIPage_Reclaim)]
public class UIPage_ReclaimComponent : UIComponent
{
    public Button m_kButtonAutoReclaim;
    public Image m_kImageReclaim;
    public Text m_kReclaimNumber;
    public Text m_kTextAutoReclaim;

    public DragReclaim m_kDragReclaim;
    public Button m_btnLock;
    public Button m_btnReclaim;
    int nScoop;

    internal void Awake()
    {
        m_kButtonAutoReclaim = m_kParentEntity.m_kUIPrefab.GetCacheComponent(0) as Button;
        m_kTextAutoReclaim = m_kParentEntity.m_kUIPrefab.GetCacheComponent(1) as Text;
        m_kReclaimNumber = m_kParentEntity.m_kUIPrefab.GetCacheComponent(2) as Text;
        m_kImageReclaim = m_kParentEntity.m_kUIPrefab.GetCacheComponent(3) as Image;
        m_kDragReclaim = m_kParentEntity.m_kUIPrefab.GetCacheComponent(4) as DragReclaim;
        m_btnLock = m_kParentEntity.m_kUIPrefab.GetCacheComponent(5) as Button;
        m_btnReclaim = m_kParentEntity.m_kUIPrefab.GetCacheComponent(6) as Button;

        m_kButtonAutoReclaim.onClick.AddListener(OnButtonClick_AutoReclaim);
        m_kDragReclaim.BeginDragAction = BeginDragAction;
        m_kDragReclaim.OnEndDragAction = OnEndDragAction;

        m_btnLock.onClick.AddListener(OnBtnLock);
        ObserverHelper<decimal>.AddEventListener(MessageMonitorType.SpadeChange, OnStartWastedland);

        Refresh();
        Init();
        UpdateIcon();
    }

    public void UpdateIcon()
    {
        bool isCan = World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.m_kPlayerBasicAsset.m_IsCanAssistant;
        m_kButtonAutoReclaim.gameObject.SetActive(isCan);
     
    }

    void Init()
    {
        for (int i = 0; i < SceneLogic._instance.listWasteland.Count; i++) 
        {
            SceneLogic._instance.listWasteland[i].subgrade.go.SetActive(false);
        }
        for (int i = 0; i < SceneLogic._instance.listSelectWBoard.Count; i++)
        {
            SceneLogic._instance.listSelectWBoard[i].BoardbaseData.go.SetActive(true);
        }
    }

    private void Refresh()
    {
        nScoop = (int)World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.m_kSpade;
        m_kReclaimNumber.text = nScoop.ToString();
        if (nScoop > 0)
        {
            m_kDragReclaim.enabled = true;
            m_btnReclaim.enabled = true;
            m_btnLock.gameObject.SetActive(false);
        }
        else
        {
            //m_kDragReclaim.enabled = false;
            m_btnReclaim.enabled = false;
            m_btnLock.gameObject.SetActive(true);
        }
            
    }

    public override void TranslateUI()
    {
        base.TranslateUI();
        m_kTextAutoReclaim.text = UI_Helper.GetTextByLanguageID(157);
    }
    private void OnStartWastedland(object sender, MessageArgs<decimal> args)
    {
        Refresh();
    }
    void OnBtnLock()
    {
        UIEntity uIEntity = World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPage_ShopBuy);
        uIEntity.GetComponent<UIPage_ShopBuyComponent>().Init(900013);
    }

    private void OnEndDragAction()
    {
        if (GuidanceManager.isGuidancing)
        {
            if (World.Scene.GetComponent<GuidanceManager>().greenHandsGuidance.isKaiHuangSuccess)
            {
                MapGridMgr.Instance.EndFreeingWastedland();
                World.Scene.GetComponent<UIManagerComponent>().Remove(UI_PrefabPath.m_sUIPage_Reclaim);
            }
            else
            {
                // 没有拖到就继续显示开荒界面
            }
        }
        else
        {
            MapGridMgr.Instance.EndFreeingWastedland();
            World.Scene.GetComponent<UIManagerComponent>().Remove(UI_PrefabPath.m_sUIPage_Reclaim);
        }
    }

    private void BeginDragAction()
    {
        BE.MobileRTSCam.instance.CreatEdit();
        MapGridMgr.Instance.BeginFreeingOneByOne();
    }

    public override void Dispose()
    {
        base.Dispose();
        m_kButtonAutoReclaim.onClick.RemoveListener(OnButtonClick_AutoReclaim);
        m_btnLock.onClick.RemoveAllListeners();

        //var WastedBoards = MapGridMgr.Instance.WastedBoards;
        //for (int i = 0; i < WastedBoards.Count; i++)
        //{
        //    MapGridMgr.Instance.DestroyBoards(WastedBoards[i].WastedGuid);
        //}

        SceneLogic._instance.RemoveAllSelectWastedLand();
        for (int i = 0; i < SceneLogic._instance.listWasteland.Count; i++)
        {
            SceneLogic._instance.listWasteland[i].subgrade.go.SetActive(true);
        }
    }

    /// <summary>
    /// 自动开垦
    /// </summary>
    void OnButtonClick_AutoReclaim()
    {
        int limit_sele = int.Parse(DBManager.Instance.m_kDisperse.GetEntryPtr(20002)._Val1);
        //int nSelected = MapGridMgr.Instance._selectedWastedlandTile.Count;
        int nWasteland = SceneLogic._instance.listWasteland.Count;
        if (nWasteland >= limit_sele)
        {
            UI_Helper.ShowCommonTips(244);
            return;
        }
        World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPopUpWindow_AutoWasteland);
    }

}
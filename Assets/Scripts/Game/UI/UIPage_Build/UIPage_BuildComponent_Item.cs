using System;
using System.Collections;
using System.Collections.Generic;

using QTFramework;

using UnityEngine;
using UnityEngine.UI;

[ObjectEventSystem]
public class UIPage_BuildComponent_ItemAwakeSystem : AAwake<UIPage_BuildComponent_Item>
{
    public override void Awake(UIPage_BuildComponent_Item _self)
    {
        _self.Awake();
    }
}

[ObjectEventSystem]
public class UIPage_BuildComponent_ItemFixedUpdateSystem : AFixedUpdate<UIPage_BuildComponent_Item>
{
    public override void FixedUpdate(UIPage_BuildComponent_Item _self)
    {
        _self.FixedUpdate();
    }
}

[UIEntityComponent(UI_PrefabPath.m_sUIPage_Build_Item)]
public class UIPage_BuildComponent_Item : UIComponent
{
    public RawImage m_kRawImageIcon;
    public Text m_kTextName;
    public Text m_kTextDesc;

    public Button m_kButtonLookDesc;
    public Text m_kTextBuildNumberAndMax;

    public RectTransform m_kRectTransformCoin;
    public RectTransform m_kRectTransformStone;

    public Text m_kTextCoinNumber;
    public Text m_kTextStoneNumber;

    public RectTransform m_kRectTransforPrice;
    public RectTransform m_kRectTransformMax;

    public Image m_kImageBG;

    public RectTransform m_kRectTransforAvailable;
    public RectTransform m_kRectTransformNotApplicable;

    public Button m_kButtonBack;
    private bool m_kLimit;

    private int shopID = 0;
    public void Awake()
    {
        m_kRawImageIcon = m_kParentEntity.m_kUIPrefab.GetCacheComponent<RawImage>(0);
        m_kTextName = m_kParentEntity.m_kUIPrefab.GetCacheComponent<Text>(1);
        m_kTextDesc = m_kParentEntity.m_kUIPrefab.GetCacheComponent<Text>(2);

        m_kButtonLookDesc = m_kParentEntity.m_kUIPrefab.GetCacheComponent<Button>(3);
        m_kTextBuildNumberAndMax = m_kParentEntity.m_kUIPrefab.GetCacheComponent<Text>(4);

        m_kRectTransformCoin = m_kParentEntity.m_kUIPrefab.GetCacheComponent<RectTransform>(5);
        m_kRectTransformStone = m_kParentEntity.m_kUIPrefab.GetCacheComponent<RectTransform>(6);

        m_kTextCoinNumber = m_kParentEntity.m_kUIPrefab.GetCacheComponent<Text>(7);
        m_kTextStoneNumber = m_kParentEntity.m_kUIPrefab.GetCacheComponent<Text>(8);

        m_kRectTransforPrice = m_kParentEntity.m_kUIPrefab.GetCacheComponent<RectTransform>(9);
        m_kRectTransformMax = m_kParentEntity.m_kUIPrefab.GetCacheComponent<RectTransform>(10);

        m_kImageBG = m_kParentEntity.m_kUIPrefab.GetCacheComponent<Image>(11);

        m_kRectTransforAvailable = m_kParentEntity.m_kUIPrefab.GetCacheComponent<RectTransform>(12);
        m_kRectTransformNotApplicable = m_kParentEntity.m_kUIPrefab.GetCacheComponent<RectTransform>(13);

        m_kButtonBack = m_kParentEntity.m_kUIPrefab.GetCacheComponent<Button>(14);

        m_kButtonLookDesc.onClick.AddListener(OnButtonClick_LookDesc);
        m_kButtonBack.onClick.AddListener(OnButtonClick_Back);
    }

    private void OnButtonClick_Back()
    {
        m_kRectTransforAvailable.gameObject.SetActive(true);
        m_kRectTransformNotApplicable.gameObject.SetActive(false);

    }

    private void OnButtonClick_LookDesc()
    {
        m_kRectTransforAvailable.gameObject.SetActive(false);
        m_kRectTransformNotApplicable.gameObject.SetActive(true);
    }

    public void FixedUpdate()
    {

    }

    public void InitItem(int _shopID)
    {
        shopID = _shopID;
        CS_Shop.DataEntry shopDataEntry = DBManager.Instance.m_kShop.GetEntryPtr(_shopID);
        if (shopDataEntry == null)
        {
            return;
        }

        CS_Model.DataEntry dataEntry = DBManager.Instance.m_kModel.GetEntryPtr((int) shopDataEntry._Goods.y);
        if (dataEntry != null)
        {
            m_kTextName.text = UI_Helper.GetTextByLanguageID(dataEntry._DisplayName);
            m_kTextDesc.text = UI_Helper.GetTextByLanguageID(dataEntry._Desc);
            m_kRawImageIcon.texture = UI_Helper.AllocTexture(dataEntry._Icon);

            int number = DataManager._instance.GetNumByCtype(dataEntry._ID);
            m_kLimit = number == dataEntry._BuildMax;
            m_kTextBuildNumberAndMax.text = $"{number}/{dataEntry._BuildMax}";

            m_kRectTransforPrice.gameObject.SetActive(!m_kLimit);
            m_kRectTransformMax.gameObject.SetActive(m_kLimit);
            int count = DataManager._instance.GetNumByCtype(dataEntry._ID);
            m_kImageBG.sprite = World.Scene.GetComponent<UIManagerComponent>().m_kCommonSpriteAtlas.GetSprite(count < dataEntry._BuildMax ? "Layout_bg00" : "Layout_bg02");
        }

        m_kRectTransformCoin.gameObject.SetActive(shopDataEntry._GoldPrice != Vector3Int.zero);
        m_kRectTransformStone.gameObject.SetActive(shopDataEntry._StonePrice != Vector3Int.zero);
        m_kTextCoinNumber.text = shopDataEntry._GoldPrice.z.ToString();
        m_kTextStoneNumber.text = shopDataEntry._StonePrice.z.ToString();
    }

    public void CreatBuild()
    {

        if (m_kLimit)
        {
            UI_Helper.ShowCommonTips(UI_Helper.GetTextByLanguageID(114));
            return;
        }
        CS_Shop.DataEntry shopDataEntry = DBManager.Instance.m_kShop.GetEntryPtr(shopID);
        if (shopDataEntry == null)
        {
            return;
        }

        MapGridMgr.Instance.NewEdit((int) shopDataEntry._Goods.y);
        UIEntity uIBuildBuyEntity = World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPage_BuildBuy);
        UIPage_BuildBuyComponent uIPage_BuildBuyComponent = uIBuildBuyEntity.GetComponent<UIPage_BuildBuyComponent>();
        if (uIPage_BuildBuyComponent != null)
        {
            uIPage_BuildBuyComponent.Init(shopID);
        }

        Debug.Log("拖动item到场景中，开始下一步");
        //CheckGuidance(uIBuildBuyEntity);
        GuidanceManager._Instance.CheckGuidance(uIBuildBuyEntity);
    }

    // 新手引导
    //private void CheckGuidance(UIEntity entity)
    //{
    //    if(GuidanceManager.isGuidancing)
    //    {
    //        GuidanceData data = new GuidanceData();
    //        data.entity = entity;
    //        ObserverHelper<GuidanceData>.SendMessage(MessageMonitorType.GuidanceClickEvent, this, new MessageArgs<GuidanceData>(data));
    //        // 做一些 回调的操作在这
    //        World.Scene.GetComponent<UIManagerComponent>().Remove(UI_PrefabPath.m_sUIPopUpWindow_Guidance_Click);
    //        //var scroll = World.Scene.GetComponent<UIManagerComponent>().Get(UI_PrefabPath.m_sUIPage_Build).GetComponent<UIPage_BuildComponent>().m_kScrollRectNode;
    //        //scroll.horizontal = true;
    //        //Debug.Log("打开scroll");
    //    }
    //}
}
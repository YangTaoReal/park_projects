using QTFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ObjectEventSystem]
public class UIPage_Place_ItemComponentAwakeSystem : AAwake<UIPage_Place_ItemComponent>
{
    public override void Awake(UIPage_Place_ItemComponent _self)
    {
        _self.Awake();
    }
}

[ObjectEventSystem]
public class UIPage_Place_ItemComponentFixedUpdateSystem : AFixedUpdate<UIPage_Place_ItemComponent>
{
    public override void FixedUpdate(UIPage_Place_ItemComponent _self)
    {
        _self.FixedUpdate();
    }
}

[UIEntityComponent(UI_PrefabPath.m_sUIPage_Place_Item)]
public class UIPage_Place_ItemComponent : UIComponent
{
    public RawImage m_kRawImageIcon;
    public Text m_kTextNumber;
    public Text m_kTextName;

    public Button m_kButtonItem;
    public RectTransform m_kRectTransformContent;

    private int ItemID;
    private PlayerBagAsset.ItemType ItemType;
    public Action ClickCallBack;
    internal void Awake()
    {
        m_kRawImageIcon = m_kParentEntity.m_kUIPrefab.GetCacheComponent(0) as RawImage;
        m_kTextNumber = m_kParentEntity.m_kUIPrefab.GetCacheComponent(1) as Text;
        m_kTextName = m_kParentEntity.m_kUIPrefab.GetCacheComponent(2) as Text;
        m_kButtonItem = m_kParentEntity.m_kUIPrefab.GetCacheComponent(3) as Button;
        m_kRectTransformContent = m_kParentEntity.m_kUIPrefab.GetCacheComponent(4) as RectTransform;

        m_kButtonItem.onClick.AddListener(OnButtonClick_ButtonItem);
    }
    public override void Dispose()
    {
        base.Dispose();
        m_kButtonItem.onClick.RemoveListener(OnButtonClick_ButtonItem);
    }
    private void OnButtonClick_ButtonItem()
    {
        CS_Model.DataEntry dataEntry = DBManager.Instance.m_kModel.GetEntryPtr(ItemID);
        if (dataEntry == null)
        {
            return;
        }

        if (dataEntry._Type == (int)ModeTyp.Animal)
        {
            ModelManager._instance.CreateAnimalPark(dataEntry._ID);
        }
        else if (dataEntry._Type == (int)ModeTyp.Plant)
        {
            ModelManager._instance.CreatePlantPark(dataEntry._ID);
        }
        World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.CosetItem(ItemID, ItemType,1);
        ClickCallBack?.Invoke();
        CheckGuidance();
    }

    void CheckGuidance()
    {
        if(GuidanceManager.isGuidancing)
        {
            GuidanceData data = new GuidanceData();
            data.entity = World.Scene.GetComponent<UIManagerComponent>().Get(UI_PrefabPath.m_sUIPage_Place);
            ObserverHelper<GuidanceData>.SendMessage(MessageMonitorType.GuidanceClickEvent, this, new MessageArgs<GuidanceData>(data));
        }
    }

    internal void FixedUpdate()
    {

    }
    public void InitItem()
    {
        m_kRectTransformContent.gameObject.SetActive(false);
    }
    public void InitItem(int _ItemID, PlayerBagAsset.ItemType _itemType, int _Number)
    {
        m_kRectTransformContent.gameObject.SetActive(true);
        ItemID = _ItemID;
        ItemType = _itemType;
        CS_Model.DataEntry dataEntry = DBManager.Instance.m_kModel.GetEntryPtr(_ItemID);
        if (dataEntry == null)
        {
            return;
        }

        m_kRawImageIcon.texture = UI_Helper.AllocTexture(dataEntry._Icon);
        m_kTextName.text = UI_Helper.GetTextByLanguageID(dataEntry._DisplayName);
        m_kTextNumber.text = _Number.ToString();

    }
}

using QTFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ObjectEventSystem]
public class UIPopUpWindow_BackpackCapacityComponentAwakeSystem : AAwake<UIPopUpWindow_BackpackCapacityComponent>
{
    public override void Awake(UIPopUpWindow_BackpackCapacityComponent _self)
    {
        _self.Awake();
    }
}

[UIEntityComponent(UI_PrefabPath.m_sUIPopUpWindow_BackpackCapacity)]
public class UIPopUpWindow_BackpackCapacityComponent : UIComponent
{
    public Text m_kTextTitle;
    public Text m_kTextCurrentCapacity;
    public Text m_kTextCurrentCapacityNumber;
    public Text m_kTextMaxCapacity;
    public Text m_kTextMaxCapacityNumber;
    public Text m_kTextTips;

    public Button m_kButtonClose;
    public Button m_kButtonOk;

    public Text m_kTextok;
    public Text m_kTextStone;
    public Image m_kImageIcon;
    private Vector3 price;
    internal void Awake()
    {
        m_kTextTitle = m_kParentEntity.m_kUIPrefab.GetCacheComponent(0) as Text;
        m_kTextCurrentCapacity = m_kParentEntity.m_kUIPrefab.GetCacheComponent(1) as Text;
        m_kTextCurrentCapacityNumber = m_kParentEntity.m_kUIPrefab.GetCacheComponent(2) as Text;
        m_kTextMaxCapacity = m_kParentEntity.m_kUIPrefab.GetCacheComponent(3) as Text;
        m_kTextMaxCapacityNumber = m_kParentEntity.m_kUIPrefab.GetCacheComponent(4) as Text;
        m_kTextTips = m_kParentEntity.m_kUIPrefab.GetCacheComponent(5) as Text;
        m_kButtonClose = m_kParentEntity.m_kUIPrefab.GetCacheComponent(6) as Button;
        m_kButtonOk = m_kParentEntity.m_kUIPrefab.GetCacheComponent(7) as Button;
        m_kTextok = m_kParentEntity.m_kUIPrefab.GetCacheComponent(8) as Text;
        m_kTextStone = m_kParentEntity.m_kUIPrefab.GetCacheComponent(9) as Text;
        m_kImageIcon = m_kParentEntity.m_kUIPrefab.GetCacheComponent(10) as Image;
        m_kButtonClose.onClick.AddListener(OnButtonClick_Close);
        m_kButtonOk.onClick.AddListener(OnButtonClick_Ok);
        Init();

        string[] xyz = DBManager.Instance.m_kDisperse.GetEntryPtr(10002)._Val1.Split(' ');
        price = new Vector3(int.Parse(xyz[0]), int.Parse(xyz[1]), int.Parse(xyz[2]));
        m_kTextStone.text = price.z.ToString();

        if (price.x == 1)
        {
            m_kImageIcon.sprite = UI_Helper.GetSprite("gold");
        }
        else if(price.x == 2)
        {
            m_kImageIcon.sprite = UI_Helper.GetSprite("crystal");
        }
    }

    public override void TranslateUI()
    {
        base.TranslateUI();
        m_kTextTitle.text = UI_Helper.GetTextByLanguageID(158);
        m_kTextCurrentCapacity.text = UI_Helper.GetTextByLanguageID(159);
        m_kTextMaxCapacity.text = UI_Helper.GetTextByLanguageID(160);
        m_kTextTips.text = UI_Helper.GetTextByLanguageID(161);
        m_kTextok.text = UI_Helper.GetTextByLanguageID(141);
    }
    private void OnButtonClick_Ok()
    {
        if (price.x == 1)
        {
            World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.CostGold((int)price.z,BuyCallBack);
        }
        else if (price.x == 2)
        {
            World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.CostStone((int)price.z, BuyCallBack);
        }

    }

    private void BuyCallBack(decimal obj)
    {
        World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.m_kPlayerBagAsset.BagVolume += 5;
        DataManager._instance.FixLocalData<PlayerBagAsset>(World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.m_kPlayerBagAsset.ID, World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.m_kPlayerBagAsset);
        World.Scene.GetComponent<UIManagerComponent>().Remove(UI_PrefabPath.m_sUIPopUpWindow_BackpackCapacity);
        ObserverHelper<bool>.SendMessage(MessageMonitorType.RefreshPackUI, this, new MessageArgs<bool>(true));
    }

    private void OnButtonClick_Close()
    {
        World.Scene.GetComponent<UIManagerComponent>().Remove(UI_PrefabPath.m_sUIPopUpWindow_BackpackCapacity);

    }

    public override void Dispose()
    {
        m_kButtonClose.onClick.RemoveListener(OnButtonClick_Close);
        m_kButtonOk.onClick.RemoveListener(OnButtonClick_Ok);
    }


    public void Init()
    {
        m_kTextCurrentCapacityNumber.text = World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.m_kPlayerBagAsset.BagVolume.ToString();
        m_kTextMaxCapacityNumber.text = "999";
    }
}

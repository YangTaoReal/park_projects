using System;
using System.Collections;
using System.Collections.Generic;
using QTFramework;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[ObjectEventSystem]
public class UIWastedAwardBoxAwakeSystem : AAwake<UIWastedAwardBox>
{
    public override void Awake(UIWastedAwardBox _self)
    {
        _self.Awake();
    }
}


[UIEntityComponent(UI_PrefabPath.m_sUIWastedAward_Box)]
public class UIWastedAwardBox : UIComponent 
{
 
    public Text ui_titleText;
    public Text ui_abstractText;
    public Text ui_confirmBtnText;
    public Button ui_confirmBtn;
    public ScrollRect ui_propScrollRect;

    public List<UIWastedAwardBoxItem> itemList = new List<UIWastedAwardBoxItem>();
    Dictionary<int, int> dicDrop = new Dictionary<int, int>();
 
	public void Awake()
    {
        UIEntity uI_Entity = ParentEntity as UIEntity;
        ui_titleText = uI_Entity.m_kUIPrefab.GetCacheComponent(0) as Text;
        ui_abstractText = uI_Entity.m_kUIPrefab.GetCacheComponent(1) as Text;
        ui_confirmBtnText = uI_Entity.m_kUIPrefab.GetCacheComponent(2) as Text;
        ui_confirmBtn = uI_Entity.m_kUIPrefab.GetCacheComponent(3) as Button;
        ui_propScrollRect = uI_Entity.m_kUIPrefab.GetCacheComponent(4) as ScrollRect;
 

        ui_confirmBtn.onClick.AddListener(() =>
        {
            foreach(var data in dicDrop)
            {
                CS_Items.DataEntry dataEntry = DBManager.Instance.m_kItems.GetEntryPtr(data.Key);
                //UI_Helper.ShowCommonTips(145, UI_Helper.GetTextByLanguageID(dataEntry._DisplayName), data.Value.ToString());
                World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.AddItemToBag(data.Key, (PlayerBagAsset.ItemType)dataEntry._ItemType, data.Value);
            }

            World.Scene.GetComponent<UIManagerComponent>().Remove(UI_PrefabPath.m_sUIWastedAward_Box);
        });

        InitList();
    }

    public void Init(int drop_cid)
    {
        dicDrop = Drop.GetDrop(drop_cid);

        InitList();
    }

    public override void Dispose()
    {
        base.Dispose();
 
        ui_confirmBtn.onClick.RemoveAllListeners();
       
    }

  
 
    public override void TranslateUI()
    {
        base.TranslateUI();
        ui_titleText.text = UI_Helper.GetTextByLanguageID(1202);
        ui_abstractText.text = UI_Helper.GetTextByLanguageID(1203);
        ui_confirmBtnText.text = UI_Helper.GetTextByLanguageID(1201);

    }



    private void InitList()
    {
        itemList.Clear();
        GetEntity<UIEntity>().ClearChildren();
        foreach(var data in dicDrop)
        {
            CS_Items.DataEntry dataEntry = DBManager.Instance.m_kItems.GetEntryPtr(data.Key);
            UIEntity entity = World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIWastedward_Item);
            UIWastedAwardBoxItem ItemBox = entity.GetComponent<UIWastedAwardBoxItem>();
            ItemBox.Init(dataEntry, data.Value);
            entity.m_kUIPrefab.transform.SetParent(ui_propScrollRect.content);
            itemList.Add(ItemBox);
            m_kParentEntity.AddChildren(entity);
        }
 

    }

 
}

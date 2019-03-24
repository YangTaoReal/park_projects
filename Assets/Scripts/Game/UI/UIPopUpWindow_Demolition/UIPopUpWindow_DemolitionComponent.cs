using QTFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ObjectEventSystem]
public class UIPopUpWindow_DemolitionComponentAwakeSystem : AAwake<UIPopUpWindow_DemolitionComponent>
{
    public override void Awake(UIPopUpWindow_DemolitionComponent _self)
    {
        _self.Awake();
    }
}

[UIEntityComponent(UI_PrefabPath.m_sUIPopUpWindow_Demolition)]
public class UIPopUpWindow_DemolitionComponent : UIComponent
{

    public Text m_kTextTitle;
    public Text m_kTextName;
    public Text m_kTextLevel;
    public Text m_kTextTips1;
    public Text m_kTextTips2;
    public RawImage m_kRawImageIcon;
    public Button m_kButtonClose;
    public Button m_kButtonOk;
    public Text m_kTextOk;
    internal void Awake()
    {
        m_kTextTitle = m_kParentEntity.m_kUIPrefab.GetCacheComponent(0) as Text;
        m_kButtonClose = m_kParentEntity.m_kUIPrefab.GetCacheComponent(1) as Button;
        m_kTextName = m_kParentEntity.m_kUIPrefab.GetCacheComponent(2) as Text;
        m_kTextLevel = m_kParentEntity.m_kUIPrefab.GetCacheComponent(3) as Text;
        m_kRawImageIcon = m_kParentEntity.m_kUIPrefab.GetCacheComponent(4) as RawImage;
        m_kTextTips1 = m_kParentEntity.m_kUIPrefab.GetCacheComponent(5) as Text;
        m_kTextTips2 = m_kParentEntity.m_kUIPrefab.GetCacheComponent(6) as Text;
        m_kButtonOk = m_kParentEntity.m_kUIPrefab.GetCacheComponent(7) as Button;
        m_kTextOk = m_kParentEntity.m_kUIPrefab.GetCacheComponent(8) as Text;
        m_kButtonClose.onClick.AddListener(OnButtonClick_Close);
        m_kButtonOk.onClick.AddListener(OnButtonClick_Ok);
        Init();
    }

    public override void TranslateUI()
    {
        base.TranslateUI();
        m_kTextTitle.text = UI_Helper.GetTextByLanguageID(146);
        m_kTextTips1.text = UI_Helper.GetTextByLanguageID(147);
        m_kTextTips2.text = UI_Helper.GetTextByLanguageID(148);
        m_kTextOk.text = UI_Helper.GetTextByLanguageID(141);
    }
    private void OnButtonClick_Ok()
    {
        MapGridMgr.Instance.RemoveEdit();
        World.Scene.GetComponent<UIManagerComponent>().Remove(UI_PrefabPath.m_sUIPopUpWindow_Demolition);
        World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUISystem_Hall);
        World.Scene.GetComponent<UIManagerComponent>().ClearUIStack();
    }

    private void OnButtonClick_Close()
    {
        World.Scene.GetComponent<UIManagerComponent>().Remove(UI_PrefabPath.m_sUIPopUpWindow_Demolition);

    }

    public override void Dispose()
    {
        m_kButtonClose.onClick.RemoveListener(OnButtonClick_Close);
        m_kButtonOk.onClick.RemoveListener(OnButtonClick_Ok);
    }


    public void Init()
    {
        DataManager dataManager = World.Scene.GetComponent<DataManager>();
        ModelManager modelManager = World.Scene.GetComponent<ModelManager>();

   
        BaseData baseData = modelManager.GetModelByGuid(MapGridMgr.Instance.GetEditoringInfo().guid);
        BuildingServer Server = baseData.go.GetComponent<Building>().GetServer;

        m_kTextName.text = UI_Helper.GetTextByLanguageID(baseData.cfg._Name);
        m_kTextLevel.text = "LV." + Server.lv;
        m_kRawImageIcon.texture = UI_Helper.AllocTexture(baseData.cfg._Icon);
        //building.baseData.cfg._ID
    }
}

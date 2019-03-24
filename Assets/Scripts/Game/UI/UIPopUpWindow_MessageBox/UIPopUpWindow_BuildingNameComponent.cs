using QTFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ObjectEventSystem]
public class UIPopUpWindow_BuildingNameComponentAwakeSystem : AAwake<UIPopUpWindow_BuildingNameComponent>
{
    public override void Awake(UIPopUpWindow_BuildingNameComponent _self)
    {
        _self.Awake();
    }
}

[UIEntityComponent(UI_PrefabPath.m_sUIPopUpWindow_BuildingName)]
public class UIPopUpWindow_BuildingNameComponent : UIComponent
{
    public Text m_textTitle;
    public Text m_textDes;
    public Text m_textTips;
    public Text m_textBtn;
 
    public Button m_btnClose;
    public Button m_btnEnsure;
    public InputField m_inputField;
 
    Park park;


    internal void Awake()
    {
        m_textTitle = m_kParentEntity.m_kUIPrefab.GetCacheComponent(0) as Text;
        m_textDes = m_kParentEntity.m_kUIPrefab.GetCacheComponent(1) as Text;
        m_textBtn = m_kParentEntity.m_kUIPrefab.GetCacheComponent(2) as Text;
  
        m_btnClose = m_kParentEntity.m_kUIPrefab.GetCacheComponent(3) as Button;
        m_btnEnsure = m_kParentEntity.m_kUIPrefab.GetCacheComponent(4) as Button;
        m_inputField = m_kParentEntity.m_kUIPrefab.GetCacheComponent(5) as InputField;
        m_textTips = m_kParentEntity.m_kUIPrefab.GetCacheComponent(6) as Text;

        m_btnClose.onClick.AddListener(onBtnClose);
        m_btnEnsure.onClick.AddListener(onBtnEnsure);
    }
    public override void Dispose()
    {
        m_btnClose.onClick.RemoveListener(onBtnClose);
        m_btnEnsure.onClick.RemoveListener(onBtnEnsure);
    }
    public override void TranslateUI()
    {
        base.TranslateUI();
        m_textTitle.text = UI_Helper.GetTextByLanguageID(214);
        m_textBtn.text = UI_Helper.GetTextByLanguageID(141);
        m_textTips.text = UI_Helper.GetTextByLanguageID(215);
        m_textDes.text = UI_Helper.GetTextByLanguageID(216);
    }
    void onBtnEnsure()
    {
        if (string.IsNullOrEmpty(m_inputField.text))
        {
            UI_Helper.ShowCommonTips(215);
            return;
        }


        park.SetName(m_inputField.text);  
        World.Scene.GetComponent<UIManagerComponent>().Remove(UI_PrefabPath.m_sUIPage_BuildingOperation);
        World.Scene.GetComponent<UIManagerComponent>().Remove(UI_PrefabPath.m_sUIPopUpWindow_BuildingName);
        MapGridMgr.Instance.UnFoucs();
        //CS_UpLevel.DataEntry CS_UpLevel = baseData.GetComponent<Building>().CS_UpLevel;
        //string[] split = CS_UpLevel._Expend.Split(' ');
        //int ttype = int.Parse(split[0]);
        //decimal tval = decimal.Parse(split[2]);
        //World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.AddAsset((PlayerBagAsset.ItemType)ttype, -tval);
        //CheckGuidance();
        GuidanceManager._Instance.CheckGuidance(World.Scene.GetComponent<UIManagerComponent>().Get(UI_PrefabPath.m_sUISystem_Hall));
    }


    void onBtnClose()
    {
        World.Scene.GetComponent<UIManagerComponent>().Remove(UI_PrefabPath.m_sUIPopUpWindow_BuildingName);
    }

    public void Init(Park _park)
    {
        park = _park;
        m_inputField.text = park.GetServer.name;
        m_inputField.text = UI_Helper.GetTextByLanguageID(1306);
        m_inputField.ActivateInputField();
    }
}

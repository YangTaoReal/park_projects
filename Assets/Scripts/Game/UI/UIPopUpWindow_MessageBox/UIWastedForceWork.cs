using System;
using System.Collections;
using System.Collections.Generic;
using QTFramework;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[ObjectEventSystem]
public class m_sUIWastedForceWorkAwakeSystem : AAwake<m_sUIWastedForceWork>
{
    public override void Awake(m_sUIWastedForceWork _self)
    {
        _self.Awake();
    }
}


[UIEntityComponent(UI_PrefabPath.m_sUIWastedForceWork)]
public class m_sUIWastedForceWork : UIComponent 
{
 
    public Text ui_titleText;
    public Text ui_abstractText;
    public Button ui_btn01;
    public Text ui_txtBtn01;
    public Button ui_btn02;
    public Text ui_txtBtn02;
 
 
	public void Awake()
    {
        UIEntity uI_Entity = ParentEntity as UIEntity;
        ui_titleText = uI_Entity.m_kUIPrefab.GetCacheComponent(0) as Text;
        ui_abstractText = uI_Entity.m_kUIPrefab.GetCacheComponent(1) as Text;
        ui_btn01 = uI_Entity.m_kUIPrefab.GetCacheComponent(2) as Button;
        ui_txtBtn01 = uI_Entity.m_kUIPrefab.GetCacheComponent(3) as Text;
        ui_btn02 = uI_Entity.m_kUIPrefab.GetCacheComponent(4) as Button;
        ui_txtBtn02 = uI_Entity.m_kUIPrefab.GetCacheComponent(5) as Text;

        ui_btn01.onClick.AddListener(OnBtn01);
        ui_btn02.onClick.AddListener(OnBtn02);
   
    }

    void OnBtn01()
    {
        ModelManager._instance.assistant.ImmediatelyRest();
        World.Scene.GetComponent<UIManagerComponent>().Remove(UI_PrefabPath.m_sUIWastedForceWork);
    }
    void OnBtn02()
    {
        ModelManager._instance.assistant.ImmediatelyRest();
        World.Scene.GetComponent<UIManagerComponent>().Remove(UI_PrefabPath.m_sUIWastedForceWork);
    }



    public override void Dispose()
    {
        base.Dispose();
 
        ui_btn01.onClick.RemoveAllListeners();
        ui_btn02.onClick.RemoveAllListeners();
        
    }

  
 
    public override void TranslateUI()
    {
        base.TranslateUI();

        ui_titleText.text = UI_Helper.GetTextByLanguageID(70002);
        ui_abstractText.text = UI_Helper.GetTextByLanguageID(70003);
        ui_txtBtn01.text = UI_Helper.GetTextByLanguageID(70004);
        ui_txtBtn02.text = UI_Helper.GetTextByLanguageID(70005);
 
    }



 
 
}

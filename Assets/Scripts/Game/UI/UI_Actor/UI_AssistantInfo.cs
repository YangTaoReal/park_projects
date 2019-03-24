using QTFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ObjectEventSystem]
public class UI_AssistantInfoAwakeSystem : AAwake<UI_AssistantInfo>
{
    public override void Awake(UI_AssistantInfo _self)
    {
        _self.Awake();
    }
}

[ObjectEventSystem]
public class UI_AssistantInfoFixedUpdateSystem : AFixedUpdate<UI_AssistantInfo>
{
    public override void FixedUpdate(UI_AssistantInfo _self)
    {
        _self.FixedUpdate();
    }
}


 
[UIEntityComponent(UI_PrefabPath.m_sUIActor_AssistantInfo)]
public class UI_AssistantInfo : UIComponent
{
    public Text m_textTitel;
    public Text m_textDes01;
    public Text m_textDes02;
    public Button m_btnClose;
    public Slider m_slider01;
    public Text m_text01slider01;
    public Text m_text01slider02;
    public Slider m_slider02;
    public Text m_text02slider01;
    public Text m_text02slider02;

    float allTired;
    float allWork;

    internal void Awake()
    {
        m_textTitel = m_kParentEntity.m_kUIPrefab.GetCacheComponent(0) as Text;
        m_textDes01 = m_kParentEntity.m_kUIPrefab.GetCacheComponent(1) as Text;
        m_textDes02 = m_kParentEntity.m_kUIPrefab.GetCacheComponent(2) as Text;
        m_btnClose = m_kParentEntity.m_kUIPrefab.GetCacheComponent(3) as Button;
        m_slider01 = m_kParentEntity.m_kUIPrefab.GetCacheComponent(4) as Slider;
        m_text01slider01 = m_kParentEntity.m_kUIPrefab.GetCacheComponent(5) as Text;
        m_text01slider02 = m_kParentEntity.m_kUIPrefab.GetCacheComponent(6) as Text;
        m_slider02 = m_kParentEntity.m_kUIPrefab.GetCacheComponent(7) as Slider;
        m_text02slider01 = m_kParentEntity.m_kUIPrefab.GetCacheComponent(8) as Text;
        m_text02slider02 = m_kParentEntity.m_kUIPrefab.GetCacheComponent(9) as Text;
 

        m_btnClose.onClick.AddListener(onBtnClose);

        allTired = float.Parse(DBManager.Instance.m_kDisperse.GetEntryPtr(20012)._Val1);
        allWork = float.Parse(DBManager.Instance.m_kDisperse.GetEntryPtr(20003)._Val1);
    }

   
    internal void FixedUpdate()
    {

    
    }

    public override void TranslateUI()
    {
        base.TranslateUI();
        m_textTitel.text = UI_Helper.GetTextByLanguageID(70006);
        m_textDes01.text = UI_Helper.GetTextByLanguageID(70007);
        m_textDes02.text = UI_Helper.GetTextByLanguageID(70010);
        m_text01slider02.text = UI_Helper.GetTextByLanguageID(70008);
        m_text02slider02.text = UI_Helper.GetTextByLanguageID(70009);

    }


    public override void Dispose()
    {
        base.Dispose();
        m_btnClose.onClick.RemoveAllListeners();
   
    }
   
    public void UpdateInfo(float yetWork, float tired)
    {
        float residue = allWork - yetWork;
        m_slider01.value = residue / allWork;

        int m = Mathf.FloorToInt(residue / 60);
        int s = (int)residue - m * 60;
        if (m > 0) m_text01slider01.text = m + "m" + s + "s";
        else m_text01slider01.text = s + "s";

        m_slider02.value = tired / allTired;
        m_text02slider01.text = tired + "/" + allTired; 
    }

    public void onBtnClose()
    {
        World.Scene.GetComponent<UIManagerComponent>().Remove(UI_PrefabPath.m_sUIActor_AssistantInfo);
    }

 

}

using QTFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ObjectEventSystem]
public class UIActivity_FirstRechargeAwakeSystem : AAwake<UIActivity_FirstRecharge>
{
    public override void Awake(UIActivity_FirstRecharge _self)
    {
        _self.Awake();
    }
}

[ObjectEventSystem]
public class UIActivity_FirstRechargeFixedUpdateSystem : AFixedUpdate<UIActivity_FirstRecharge>
{
    public override void FixedUpdate(UIActivity_FirstRecharge _self)
    {
        _self.FixedUpdate();
    }
}


 
[UIEntityComponent(UI_PrefabPath.m_sUIActivity_FirstRecharge)]
public class UIActivity_FirstRecharge : UIComponent
{
    Text m_textTitel;
    Text m_textDes;
    Button m_btnClose;
    Button m_btnGoTo;
    Text m_textGoTo;
    Text m_textTitel02;
    internal void Awake()
    {
 
        m_textTitel = m_kParentEntity.m_kUIPrefab.GetCacheComponent(0) as Text;
        m_textDes = m_kParentEntity.m_kUIPrefab.GetCacheComponent(1) as Text;
        m_btnClose = m_kParentEntity.m_kUIPrefab.GetCacheComponent(2) as Button;
        m_btnGoTo = m_kParentEntity.m_kUIPrefab.GetCacheComponent(3) as Button;
        m_textGoTo = m_kParentEntity.m_kUIPrefab.GetCacheComponent(4) as Text;
        m_textTitel02 = m_kParentEntity.m_kUIPrefab.GetCacheComponent(5) as Text;
 
 

        m_btnClose.onClick.AddListener(onBtnClose);
        m_btnGoTo.onClick.AddListener(onBtnGoTo);
 

        m_textDes.text = UI_Helper.GetTextByLanguageID(60035, "$100");
    }

   
    internal void FixedUpdate()
    {

    
    }

    public override void TranslateUI()
    {
        base.TranslateUI();
        m_textTitel.text = UI_Helper.GetTextByLanguageID(60033);
        m_textTitel02.text = UI_Helper.GetTextByLanguageID(60034);
        m_textGoTo.text = UI_Helper.GetTextByLanguageID(60036);
    }


    public override void Dispose()
    {
        base.Dispose();
        m_btnClose.onClick.RemoveAllListeners();
        m_btnGoTo.onClick.RemoveAllListeners();
    }
   
    public void onBtnClose()
    {
        World.Scene.GetComponent<UIManagerComponent>().Remove(UI_PrefabPath.m_sUIActivity_FirstRecharge);
    }

    public void onBtnGoTo()
    {
        World.Scene.GetComponent<UIManagerComponent>().Remove(UI_PrefabPath.m_sUIActivity_FirstRecharge);


    }
  

}

using QTFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ObjectEventSystem]
public class UIPopUpWindow_MessageBoxComponentAwakeSystem : AAwake<UIPopUpWindow_MessageBoxComponent>
{
    public override void Awake(UIPopUpWindow_MessageBoxComponent _self)
    {
        _self.Awake();
    }
}

[UIEntityComponent(UI_PrefabPath.m_sUIPopUpWindow_MessageBox)]
public class UIPopUpWindow_MessageBoxComponent : UIComponent
{

    public Text m_kTextTitle;
    public Button m_kButtonClose;
    public Text m_kTextContent;
    public Button m_kButtonOk;
    public Text m_kTextOk;

    Action _okAction;
    internal void Awake()
    {
        m_kTextTitle = m_kParentEntity.m_kUIPrefab.GetCacheComponent(0) as Text;
        m_kButtonClose = m_kParentEntity.m_kUIPrefab.GetCacheComponent(1) as Button;
        m_kTextContent = m_kParentEntity.m_kUIPrefab.GetCacheComponent(2) as Text;
        m_kButtonOk = m_kParentEntity.m_kUIPrefab.GetCacheComponent(3) as Button;
        m_kTextOk = m_kParentEntity.m_kUIPrefab.GetCacheComponent(4) as Text;


        m_kButtonClose.onClick.AddListener(OnButtonClick_Close);
        m_kButtonOk.onClick.AddListener(OnButtonClick_Ok);
    }
    public override void Dispose()
    {

        m_kButtonClose.onClick.RemoveListener(OnButtonClick_Close);
        m_kButtonOk.onClick.RemoveListener(OnButtonClick_Ok);

    }
    public override void TranslateUI()
    {
        base.TranslateUI();


        m_kTextOk.text = UI_Helper.GetTextByLanguageID(141);
    }
    private void OnButtonClick_Ok()
    {
        _okAction?.Invoke();
        World.Scene.GetComponent<UIManagerComponent>().Remove(UI_PrefabPath.m_sUIPopUpWindow_MessageBox);
    }

    private void OnButtonClick_Close()
    {
        World.Scene.GetComponent<UIManagerComponent>().Remove(UI_PrefabPath.m_sUIPopUpWindow_MessageBox);
    }

    public void Init(String _title,string _content,Action _action=null)
    {
        m_kTextTitle.text = _title;
        m_kTextContent.text = _content;
        _okAction = _action;
    }
}

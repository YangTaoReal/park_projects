using System;
using System.Collections;
using System.Collections.Generic;

using QTFramework;

using UnityEngine;
using UnityEngine.UI;

[ObjectEventSystem]
public class UIPage_HeadAwakeSystem : AAwake<UIPage_HeadComponent>
{
    public override void Awake(UIPage_HeadComponent _self)
    {
        _self.Awake();
    }
}

[UIEntityComponent(UI_PrefabPath.m_sUIPage_Head)]
public class UIPage_HeadComponent : UIComponent
{
    public RawImage m_RawImageIcon;
    public Button m_ButtonChangeName;
    public Button m_ButtonClose;
    public Button m_ButtonIllustrated;
    public Button m_ButtonHistory;
    public Text m_TextIllustrated;
    public Text m_TextHistory;
    public Text m_TextName;

    public Button m_ButtonSetting;
    public Text m_TextSetting;
    internal void Awake()
    {
        m_RawImageIcon = m_kParentEntity.m_kUIPrefab.GetCacheComponent(0)as RawImage;
        m_TextName = m_kParentEntity.m_kUIPrefab.GetCacheComponent(1)as Text;
        m_ButtonChangeName = m_kParentEntity.m_kUIPrefab.GetCacheComponent(2)as Button;
        m_ButtonClose = m_kParentEntity.m_kUIPrefab.GetCacheComponent(3)as Button;
        m_ButtonIllustrated = m_kParentEntity.m_kUIPrefab.GetCacheComponent(4)as Button;
        m_ButtonHistory = m_kParentEntity.m_kUIPrefab.GetCacheComponent(5)as Button;
        m_TextIllustrated = m_kParentEntity.m_kUIPrefab.GetCacheComponent(6)as Text;
        m_TextHistory = m_kParentEntity.m_kUIPrefab.GetCacheComponent(7)as Text;

        m_TextSetting = m_kParentEntity.m_kUIPrefab.GetCacheComponent(8)as Text;
        m_ButtonSetting = m_kParentEntity.m_kUIPrefab.GetCacheComponent(9)as Button;

        m_ButtonChangeName.onClick.AddListener(onButtonClick_ChangeName);
        m_ButtonClose.onClick.AddListener(onButtonClick_Close);
        m_ButtonIllustrated.onClick.AddListener(onButtonClick_Illustrated);
        m_ButtonHistory.onClick.AddListener(onButtonClick_History);
        m_ButtonSetting.onClick.AddListener(onButtonClick_Setting);

        ObserverHelper<string>.AddEventListener(MessageMonitorType.NameChange, OnNameChange);
        m_TextName.text = World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.m_kPlayerBasicAsset.m_kName;
    }

    private void onButtonClick_Setting()
    {
        World.Scene.GetComponent<UIManagerComponent>().Remove(UI_PrefabPath.m_sUIPage_Head);
        World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPopupWindow_Setting);
    }
    private void OnNameChange(object sender, MessageArgs<string> args)
    {
        m_TextName.text = args.Item;
    }

    private void onButtonClick_History()
    {

    }

    private void onButtonClick_Illustrated()
    {
        World.Scene.GetComponent<UIManagerComponent>().Remove(UI_PrefabPath.m_sUIPage_Head);
        World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPopupWindow_Illustrated);
    }

    private void onButtonClick_Close()
    {
        World.Scene.GetComponent<UIManagerComponent>().Remove(UI_PrefabPath.m_sUIPage_Head);
    }

    private void onButtonClick_ChangeName()
    {
        //World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPage_SignName);
        UIEntity entity = null;
        entity = UI_Helper.ShowSignNamePanel(() =>
        {
            World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.ChangleName(entity.GetComponent<UI_SignName>().ui_inputName.text.Trim());
        });
    }

    public override void TranslateUI()
    {
        base.TranslateUI();
        m_TextIllustrated.text = UI_Helper.GetTextByLanguageID(182);
        m_TextHistory.text = UI_Helper.GetTextByLanguageID(183);
        m_TextSetting.text = UI_Helper.GetTextByLanguageID(188);
    }

    public override void Dispose()
    {
        base.Dispose();
        m_ButtonChangeName.onClick.RemoveListener(onButtonClick_ChangeName);
        m_ButtonClose.onClick.RemoveListener(onButtonClick_Close);
        m_ButtonIllustrated.onClick.RemoveListener(onButtonClick_Illustrated);
        m_ButtonHistory.onClick.RemoveListener(onButtonClick_History);
    }
}
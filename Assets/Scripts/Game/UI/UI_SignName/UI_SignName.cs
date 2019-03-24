using System.Collections;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using QTFramework;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[ObjectEventSystem]
public class UI_SignNameAwakeSystem : AAwake<UI_SignName>
{
    public override void Awake(UI_SignName _self)
    {
        _self.Awake();
    }
}

[UIEntityComponent(UI_PrefabPath.m_sUIPage_SignName)]
public class UI_SignName : UIComponent 
{

    public Button ui_comfirmBtn;
    public InputField ui_inputName;
    public Text ui_TitleText;    //  显示文字
    public Text ui_PlaceholderText;    //  显示文字
    public Text ui_ConfirmBtnText;    //  显示文字
    public Button ui_CloseBtn;

    public void Awake()
    {
        UIEntity uI_Entity = ParentEntity as UIEntity;
        ui_comfirmBtn = uI_Entity.m_kUIPrefab.GetCacheComponent(0) as Button;
        ui_inputName = uI_Entity.m_kUIPrefab.GetCacheComponent(1) as InputField;
        ui_TitleText = uI_Entity.m_kUIPrefab.GetCacheComponent(2) as Text;
        ui_PlaceholderText = uI_Entity.m_kUIPrefab.GetCacheComponent(3) as Text;
        ui_ConfirmBtnText = uI_Entity.m_kUIPrefab.GetCacheComponent(4) as Text;
        ui_CloseBtn = uI_Entity.m_kUIPrefab.GetCacheComponent(5) as Button;

        InitEvent();
    }

    public void InitCallBack(UnityAction confirmCallBack = null)
    {
        
        if (confirmCallBack != null)
        {
            ui_comfirmBtn.onClick.RemoveAllListeners();
            ui_comfirmBtn.onClick.AddListener(confirmCallBack);
        }
        ui_comfirmBtn.onClick.AddListener(ClosePanel);
    }

    private void InitEvent()
    {
        ui_comfirmBtn.interactable = false;
        //ui_comfirmBtn.onClick.AddListener(() =>
        //{
        //    Debug.Log("确认名字");
        //    if (DialogueManager.instance.initialDatabase.GetVariable("hasUserName").InitialBoolValue == false)
        //    {
               
        //    }
        //    World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.ChangleName(ui_inputName.text.Trim());
        //});
        ui_inputName.onValueChanged.AddListener((name) =>
        {
            if (name.Length > 0)
                ui_comfirmBtn.interactable = true;
            if (name.Length == 0)
                ui_comfirmBtn.interactable = false;
        });


        ui_CloseBtn.onClick.AddListener(()=> {
            ClosePanel();
        });
    }

    private void ClosePanel()
    {
        World.Scene.GetComponent<UIManagerComponent>().Remove(UI_PrefabPath.m_sUIPage_SignName);

    }

    public override void TranslateUI()
    {
        base.TranslateUI();
        ui_ConfirmBtnText.text = UI_Helper.GetTextByLanguageID(141);
        ui_TitleText.text = UI_Helper.GetTextByLanguageID(143);
        ui_PlaceholderText.text = UI_Helper.GetTextByLanguageID(144);

    }
}

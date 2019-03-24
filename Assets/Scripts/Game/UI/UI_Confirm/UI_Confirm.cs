using System.Collections;
using System.Collections.Generic;
using QTFramework;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[ObjectEventSystem]
public class UI_ConfirmAwakeSystem : AAwake<UI_Confirm>
{
    public override void Awake(UI_Confirm _self)
    {
        _self.Awake();
    }
}

[UIEntityComponent(UI_PrefabPath.m_sUIPopUpWindow_Confirm)]
public class UI_Confirm : UIComponent 
{

    public Button ui_SingleConfirmBtn;
    public Button ui_DoubleConfirmBtn;
    public Button ui_CancelBtn;
    public Text ui_ScrollText;    //  显示文字
    public Text ui_singleConfirmBtnText;    //  显示文字
    public Text ui_DoubltConfirmBtnText;    //  显示文字
    public Text ui_CancelBtnText;    //  显示文字
    public Text ui_TitelText;//标题
    public UnityAction singleConfirmCallBack = null;
    public UnityAction doubleConfirmCallBack = null;
    public UnityAction cancelCallBack = null;


    public void Awake()
    {
        UIEntity uI_Entity = ParentEntity as UIEntity;
        ui_SingleConfirmBtn = uI_Entity.m_kUIPrefab.GetCacheComponent(0) as Button;
        ui_DoubleConfirmBtn = uI_Entity.m_kUIPrefab.GetCacheComponent(1) as Button;
        ui_CancelBtn = uI_Entity.m_kUIPrefab.GetCacheComponent(2) as Button;
        ui_ScrollText = uI_Entity.m_kUIPrefab.GetCacheComponent(3) as Text;
        ui_singleConfirmBtnText = uI_Entity.m_kUIPrefab.GetCacheComponent(4) as Text;
        ui_DoubltConfirmBtnText = uI_Entity.m_kUIPrefab.GetCacheComponent(5) as Text;
        ui_CancelBtnText = uI_Entity.m_kUIPrefab.GetCacheComponent(6) as Text;
        ui_TitelText = uI_Entity.m_kUIPrefab.GetCacheComponent(7) as Text;
        ui_TitelText.text = "";

        ui_DoubleConfirmBtn.onClick.AddListener(ClosePanel);
        ui_CancelBtn.onClick.AddListener(ClosePanel);
        ui_SingleConfirmBtn.onClick.AddListener(ClosePanel);
    }
    public override void Dispose()
    {
        base.Dispose();
        ui_DoubleConfirmBtn.onClick.RemoveListener(ClosePanel);
        ui_CancelBtn.onClick.RemoveListener(ClosePanel);
        ui_SingleConfirmBtn.onClick.RemoveListener(ClosePanel);

        if (singleConfirmCallBack != null)
        {
            ui_SingleConfirmBtn.onClick.RemoveListener(singleConfirmCallBack);
        }
        if (doubleConfirmCallBack != null)
        {
            ui_DoubleConfirmBtn.onClick.RemoveListener(doubleConfirmCallBack);
        }
        if(cancelCallBack != null)
            ui_CancelBtn.onClick.RemoveListener(cancelCallBack);
        singleConfirmCallBack = null;
        doubleConfirmCallBack = null;
        cancelCallBack = null;
    }
    public override void TranslateUI()
    {
        base.TranslateUI();
        ui_singleConfirmBtnText.text = UI_Helper.GetTextByLanguageID(141);
        ui_DoubltConfirmBtnText.text = UI_Helper.GetTextByLanguageID(141);
        ui_CancelBtnText.text = UI_Helper.GetTextByLanguageID(142);

    }

    public void Init(string describe,UnityAction doubleConfirm, UnityAction cancel)
    {
        if (doubleConfirmCallBack != null)
        {
            ui_DoubleConfirmBtn.onClick.RemoveListener(doubleConfirmCallBack);
        }
        if (cancelCallBack != null)
        {
            ui_CancelBtn.onClick.RemoveListener(cancelCallBack);
        }


        doubleConfirmCallBack = doubleConfirm;
        cancelCallBack = cancel;
        ui_ScrollText.text = describe;
        ShowButtonType(ButtonType.Double);
        if (doubleConfirm != null)
        {
            ui_DoubleConfirmBtn.onClick.AddListener(doubleConfirm);
        }


        if (cancel != null)
        {
    
            ui_CancelBtn.onClick.AddListener(cancel);
        }

    }

    public void Init(string describe,UnityAction singleCallBack = null)
    {
        if (singleConfirmCallBack != null)
        {
            ui_SingleConfirmBtn.onClick.RemoveListener(singleConfirmCallBack);
        }
        singleConfirmCallBack = singleCallBack;
        ui_ScrollText.text = describe;
        ShowButtonType(ButtonType.Single);
 
        if (singleCallBack != null)
        {
            ui_SingleConfirmBtn.onClick.AddListener(singleCallBack);
        }
    }

    private void ShowButtonType(ButtonType type)
    {
        switch(type)
        {
            case ButtonType.Single:
                ui_SingleConfirmBtn.gameObject.SetActive(true);
                ui_DoubleConfirmBtn.gameObject.SetActive(false);
                ui_CancelBtn.gameObject.SetActive(false);
                break;
            case ButtonType.Double:
                ui_SingleConfirmBtn.gameObject.SetActive(false);
                ui_DoubleConfirmBtn.gameObject.SetActive(true);
                ui_CancelBtn.gameObject.SetActive(true);
                break;
        }
    }

    private void ClosePanel()
    {
        World.Scene.GetComponent<UIManagerComponent>().Remove(UI_PrefabPath.m_sUIPopUpWindow_Confirm);
        //Debug.LogError("确认后自动关闭确认窗口");
    }



    public enum ButtonType
    {
        Single,
        Double,
    }

    public void TextChange(string title = null, string content = null, string ensure = null , string cancel = null)
    {
        if (title!= null) ui_TitelText.text = title;
        if (content != null) ui_ScrollText.text = content;
        if (ensure != null) ui_DoubltConfirmBtnText.text = ensure;
        if (cancel != null) ui_CancelBtnText.text = cancel;
 
    }

}

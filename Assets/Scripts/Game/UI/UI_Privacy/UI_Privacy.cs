using System.Collections;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using QTFramework;
using UnityEngine;
using UnityEngine.UI;

[ObjectEventSystem]
public class UI_PrivacyAwakeSystem : AAwake<UI_Privacy>
{
    public override void Awake(UI_Privacy _self)
    {
        _self.Awake();
    }
}


[UIEntityComponent(UI_PrefabPath.m_sUIPage_Privacy)]
public class UI_Privacy : UIComponent 
{


    public Button ui_AgreeBtn;
    public Button ui_CancelBtn;
    public ScrollRect ui_ScrollRect;
    public Text ui_TitleText;
    public Text ui_AgreeBtnText;
    public Text ui_DisagreeBtnText;
    public Text ui_PrivacyText;

    public void Awake()
    {
        UIEntity uI_Entity = ParentEntity as UIEntity;
        ui_AgreeBtn = uI_Entity.m_kUIPrefab.GetCacheComponent(0) as Button;
        ui_CancelBtn = uI_Entity.m_kUIPrefab.GetCacheComponent(1) as Button;
        ui_ScrollRect = uI_Entity.m_kUIPrefab.GetCacheComponent(2) as ScrollRect;
        ui_TitleText = uI_Entity.m_kUIPrefab.GetCacheComponent(3) as Text;
        ui_AgreeBtnText = uI_Entity.m_kUIPrefab.GetCacheComponent(4) as Text;
        ui_DisagreeBtnText = uI_Entity.m_kUIPrefab.GetCacheComponent(5) as Text;
        ui_PrivacyText = uI_Entity.m_kUIPrefab.GetCacheComponent(6) as Text;
        InitEvent();
    }

    UIEntity entity = null;
    private void InitEvent()
    {
        ui_AgreeBtn.interactable = false;
        ui_AgreeBtn.onClick.AddListener(() =>
        {
            Debug.Log("同意协议");
            World.Scene.GetComponent<UIManagerComponent>().Remove(UI_PrefabPath.m_sUIPage_Privacy);
            //World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPage_SignName);
            entity = UI_Helper.ShowSignNamePanel(() =>
            {
                //Debug.Log("对话系统给玩家命名");
                // 对话系统判断
                string name = entity.GetComponent<UI_SignName>().ui_inputName.text.Trim();
                World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.ChangleName(name);
                DialogueManager.instance.initialDatabase.GetVariable("hasUserName").InitialBoolValue = true;
                var actor = DialogueManager.instance.initialDatabase.GetActor(1);
                actor.Name = name;
                actor.fields[4].value = name;   // 英语名
                actor.fields[5].value = name;   // 阿语名
                World.Scene.GetComponent<DialogueManagerComponent>().StartConversation(ConversationClip.Investment);
            });
        });
        ui_CancelBtn.onClick.AddListener(() =>
        {
            //Debug.Log("不同意协议");
            //string text = "不同意的话无法进行游,将直接退,你怕不怕!";
            UI_Helper.ShowConfirmPanel(136, () =>
            {
                Debug.Log("确认按钮，退出游戏");
                Application.Quit();
            }, () =>
            {
                Debug.Log("取消按钮");
            });
        });

        ui_ScrollRect.onValueChanged.AddListener((v) =>
        {
            //Debug.Log("滑动值：" + v);
            if (v.y <= 0.1f)
                SetAgreeBtnEnable();
        });
    }

    public override void TranslateUI()
    {
        base.TranslateUI();
        ui_AgreeBtnText.text = UI_Helper.GetTextByLanguageID(137);
        ui_DisagreeBtnText.text = UI_Helper.GetTextByLanguageID(138);
        ui_TitleText.text = UI_Helper.GetTextByLanguageID(139);
        ui_PrivacyText.text = UI_Helper.GetTextByLanguageID(140);
    }

    private void SetAgreeBtnEnable()
    {
        ui_AgreeBtn.interactable = true;
    }

}

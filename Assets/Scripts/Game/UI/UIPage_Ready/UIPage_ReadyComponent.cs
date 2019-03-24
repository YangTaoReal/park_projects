using System;
using QTFramework;

using UnityEngine;
using UnityEngine.UI;

[ObjectEventSystem]
public class UIPage_ReadyComponentAwakeSystem : AAwake<UIPage_ReadyComponent>
{
    public override void Awake(UIPage_ReadyComponent _self)
    {
        _self.Awake();
    }
}

[UIEntityComponent(UI_PrefabPath.m_sUIPage_Ready)]
public class UIPage_ReadyComponent : UIComponent
{
    public Text m_kText_Tips;
    public Text m_kText_Process;
    public Slider m_kSlider_Process;

    private float kk;
    private bool jiazai;
    public void Awake()
    {
        m_kText_Tips = m_kParentEntity.m_kUIPrefab.GetCacheComponent(0) as Text;
        m_kText_Process = m_kParentEntity.m_kUIPrefab.GetCacheComponent(1) as Text;
        m_kSlider_Process = m_kParentEntity.m_kUIPrefab.GetCacheComponent(2) as Slider;
        m_kText_Process.text = $"{0}%";
        m_kSlider_Process.value = 0;
        ObserverHelper<int>.AddEventListener(MessageMonitorType.RecoverData, NotificationRecoverData);
    }

    private void NotificationRecoverData(object sender, MessageArgs<int> args)
    {
        m_kSlider_Process.value = args.Item / 100f;
        m_kText_Process.text = $"{args.Item }%";
    }

    public override void TranslateUI()
    {
        base.TranslateUI();
        m_kText_Tips.text = UI_Helper.GetTextByLanguageID(249);
    }
}
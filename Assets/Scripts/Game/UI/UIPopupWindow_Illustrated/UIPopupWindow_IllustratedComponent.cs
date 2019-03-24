using System;
using System.Collections;
using System.Collections.Generic;

using QTFramework;

using UnityEngine;
using UnityEngine.UI;

[ObjectEventSystem]
public class UIPopupWindow_IllustratedAwakeSystem : AAwake<UIPopupWindow_Illustrated>
{
    public override void Awake(UIPopupWindow_Illustrated _self)
    {
        _self.Awake();
    }
}

[UIEntityComponent(UI_PrefabPath.m_sUIPopupWindow_Illustrated)]
public class UIPopupWindow_Illustrated : UIComponent
{
    public Button m_ButtonClose;
    public Text m_TextTitle;
    public Toggle m_ToggleAnimal;
    public Toggle m_ToggleBotany;
    public Toggle m_ToggleBuild;
    public Toggle m_ToggleBuff;
    public Text m_TextAnimal1;
    public Text m_TextAnimal2;
    public Text m_TextBotany1;
    public Text m_TextBotany2;
    public Text m_TextBuild1;
    public Text m_TextBuild2;
    public Text m_TextBuff1;
    public Text m_TextBuff2;
    public ScrollRect m_ScrollAnimal;
    public ScrollRect m_ScrollBuff;

    internal void Awake()
    {
        m_ButtonClose = m_kParentEntity.m_kUIPrefab.GetCacheComponent(0) as Button;
        m_TextTitle = m_kParentEntity.m_kUIPrefab.GetCacheComponent(1) as Text;
        m_ToggleAnimal = m_kParentEntity.m_kUIPrefab.GetCacheComponent(2) as Toggle;
        m_ToggleBotany = m_kParentEntity.m_kUIPrefab.GetCacheComponent(3) as Toggle;
        m_ToggleBuild = m_kParentEntity.m_kUIPrefab.GetCacheComponent(4) as Toggle;
        m_ToggleBuff = m_kParentEntity.m_kUIPrefab.GetCacheComponent(5) as Toggle;
        m_TextAnimal1 = m_kParentEntity.m_kUIPrefab.GetCacheComponent(6) as Text;
        m_TextAnimal2 = m_kParentEntity.m_kUIPrefab.GetCacheComponent(7) as Text;
        m_TextBotany1 = m_kParentEntity.m_kUIPrefab.GetCacheComponent(8) as Text;
        m_TextBotany2 = m_kParentEntity.m_kUIPrefab.GetCacheComponent(9) as Text;
        m_TextBuild1 = m_kParentEntity.m_kUIPrefab.GetCacheComponent(10) as Text;
        m_TextBuild2 = m_kParentEntity.m_kUIPrefab.GetCacheComponent(11) as Text;
        m_TextBuff1 = m_kParentEntity.m_kUIPrefab.GetCacheComponent(12) as Text;
        m_TextBuff2 = m_kParentEntity.m_kUIPrefab.GetCacheComponent(13) as Text;
        m_ScrollAnimal = m_kParentEntity.m_kUIPrefab.GetCacheComponent(14) as ScrollRect;
        m_ScrollBuff = m_kParentEntity.m_kUIPrefab.GetCacheComponent(15) as ScrollRect;

        m_ButtonClose.onClick.AddListener(onClick_ButtonClose);
        m_ToggleAnimal.onValueChanged.AddListener(onValueChanged_ToggleAnimal);
        m_ToggleBotany.onValueChanged.AddListener(onValueChanged_ToggleBotany);
        m_ToggleBuild.onValueChanged.AddListener(onValueChanged_ToggleBuild);
        m_ToggleBuff.onValueChanged.AddListener(onValueChanged_ToggleBuff);

        m_ToggleAnimal.isOn = false;
        m_ToggleAnimal.isOn = true;
        m_ToggleBotany.isOn = false;
        m_ToggleBuild.isOn = false;
        m_ToggleBuff.isOn = false;
    }

    public override void Dispose()
    {
        base.Dispose();

        m_ButtonClose.onClick.RemoveListener(onClick_ButtonClose);
        m_ToggleAnimal.onValueChanged.RemoveListener(onValueChanged_ToggleAnimal);
        m_ToggleBotany.onValueChanged.RemoveListener(onValueChanged_ToggleBotany);
        m_ToggleBuild.onValueChanged.RemoveListener(onValueChanged_ToggleBuild);
        m_ToggleBuff.onValueChanged.RemoveListener(onValueChanged_ToggleBuff);
    }
    public override void TranslateUI()
    {
        base.TranslateUI();
        m_TextTitle.text = UI_Helper.GetTextByLanguageID(182); //图鉴
        m_TextAnimal1.text = UI_Helper.GetTextByLanguageID(103);
        m_TextAnimal2.text = UI_Helper.GetTextByLanguageID(103);
        m_TextBotany1.text = UI_Helper.GetTextByLanguageID(101);
        m_TextBotany2.text = UI_Helper.GetTextByLanguageID(101);
        m_TextBuild1.text = UI_Helper.GetTextByLanguageID(113);
        m_TextBuild2.text = UI_Helper.GetTextByLanguageID(113);
        m_TextBuff1.text = UI_Helper.GetTextByLanguageID(203);
        m_TextBuff2.text = UI_Helper.GetTextByLanguageID(203);
    }

    private void onValueChanged_ToggleBuff(bool arg0)
    {
        m_ToggleBuff.graphic.gameObject.SetActive(arg0);
        if (arg0)
        {
            InitBuff();
        }

    }

    private void onValueChanged_ToggleBuild(bool arg0)
    {
        m_ToggleBuild.graphic.gameObject.SetActive(arg0);
        if (arg0)
        {
            InitItem(2);
        }
    }

    private void onValueChanged_ToggleBotany(bool arg0)
    {
        m_ToggleBotany.graphic.gameObject.SetActive(arg0);
        if (arg0)
        {
            InitItem(4);
        }
    }

    private void onValueChanged_ToggleAnimal(bool arg0)
    {
        m_ToggleAnimal.graphic.gameObject.SetActive(arg0);
        if (arg0)
        {
            InitItem(3);
        }
    }

    private void onClick_ButtonClose()
    {
        World.Scene.GetComponent<UIManagerComponent>().Remove(UI_PrefabPath.m_sUIPopupWindow_Illustrated);
    }

    private void InitItem(int type)
    {
        m_ScrollAnimal.gameObject.SetActive(true);
        m_ScrollBuff.gameObject.SetActive(false);

        GetEntity<UIEntity>().ClearChildren();
        var modeList = DBManager.Instance.m_kModel.m_kDataEntryTable.GetEnumerator();

        m_ScrollAnimal.verticalNormalizedPosition = 1;

        while (modeList.MoveNext())
        {
            if (modeList.Current.Value._Type == type && modeList.Current.Value._IsShow)
            {
                UIEntity uIEntity = World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPopupWindow_Illustrated_Item);
                m_kParentEntity.AddChildren(uIEntity);
                UIPopupWindow_Illustrated_Item _UIPopupWindow_Illustrated_Item = uIEntity.GetComponent<UIPopupWindow_Illustrated_Item>();
                uIEntity.m_kUIPrefab.gameObject.transform.SetParent(m_ScrollAnimal.content);
                uIEntity.m_kUIPrefab.gameObject.transform.localPosition = Vector3.zero;
                uIEntity.m_kUIPrefab.gameObject.transform.localEulerAngles = Vector3.zero;
                uIEntity.m_kUIPrefab.gameObject.transform.localScale = Vector3.one;
                _UIPopupWindow_Illustrated_Item.Init(modeList.Current.Value._ID);
            }
        }
    }

    private void InitBuff()
    {
        m_ScrollAnimal.gameObject.SetActive(false);
        m_ScrollBuff.gameObject.SetActive(true);
        GetEntity<UIEntity>().ClearChildren();
        var buffList = DBManager.Instance.m_kBuff.m_kDataEntryTable.GetEnumerator();
        m_ScrollAnimal.verticalNormalizedPosition = 1;
        while (buffList.MoveNext())
        {
            UIEntity uIEntity = World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPopupWindow_Illustrated_BuffItem);
            m_kParentEntity.AddChildren(uIEntity);
            UIPopupWindow_Illustrated_BuffItemComponent _UIPopupWindow_Illustrated_Item = uIEntity.GetComponent<UIPopupWindow_Illustrated_BuffItemComponent>();
            uIEntity.m_kUIPrefab.gameObject.transform.SetParent(m_ScrollBuff.content);
            uIEntity.m_kUIPrefab.gameObject.transform.localPosition = Vector3.zero;
            uIEntity.m_kUIPrefab.gameObject.transform.localEulerAngles = Vector3.zero;
            uIEntity.m_kUIPrefab.gameObject.transform.localScale = Vector3.one;
            _UIPopupWindow_Illustrated_Item.Init(buffList.Current.Value._ID);
        }
    }
}
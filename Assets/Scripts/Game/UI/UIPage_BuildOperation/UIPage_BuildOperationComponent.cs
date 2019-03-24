using QTFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ObjectEventSystem]
public class UIPage_BuildOperationComponentAwakeSystem : AAwake<UIPage_BuildOperationComponent>
{
    public override void Awake(UIPage_BuildOperationComponent _self)
    {
        _self.Awake();
    }
}

[UIEntityComponent(UI_PrefabPath.m_sUIPage_BuildOperation)]
public class UIPage_BuildOperationComponent : UIComponent
{
    public Button m_kButtonRecycle;
    public Button m_kButtonOK;
    public Button m_kButtonClose;
    public Button m_kButtonRotation;

    public Text m_kTextTitle;

    public void Awake()
    {
        m_kButtonClose = m_kParentEntity.m_kUIPrefab.GetCacheComponent(0) as Button;
        m_kButtonRecycle = m_kParentEntity.m_kUIPrefab.GetCacheComponent(1) as Button;
        m_kButtonOK = m_kParentEntity.m_kUIPrefab.GetCacheComponent(2) as Button;
        m_kButtonRotation = m_kParentEntity.m_kUIPrefab.GetCacheComponent(3) as Button;

        m_kTextTitle = m_kParentEntity.m_kUIPrefab.GetCacheComponent(4) as Text;

        m_kButtonClose.onClick.AddListener(OnButtonClick_Close);
        m_kButtonRecycle.onClick.AddListener(OnButtonClick_Recycle);
        m_kButtonOK.onClick.AddListener(OnButtonClick_OK);
        m_kButtonRotation.onClick.AddListener(OnButtonClick_Rotation);

    }

    public override void TranslateUI()
    {
        base.TranslateUI();

        m_kTextTitle.text = UI_Helper.GetTextByLanguageID(253);

    }

    public override void Dispose()
    {
        m_kButtonClose.onClick.RemoveListener(OnButtonClick_Close);
        m_kButtonRecycle.onClick.RemoveListener(OnButtonClick_Recycle);
        m_kButtonOK.onClick.RemoveListener(OnButtonClick_OK);
        m_kButtonRotation.onClick.RemoveListener(OnButtonClick_Rotation);
    }

    private void OnButtonClick_Close()
    {
        MapGridMgr.Instance.CancelEdit();
        World.Scene.GetComponent<UIManagerComponent>().Remove(UI_PrefabPath.m_sUIPage_BuildOperation);
    }
    private void OnButtonClick_Recycle()
    {
        UIEntity uIEntity = World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPopUpWindow_Demolition);
    }
    private void OnButtonClick_OK()
    {
        if (MapGridMgr.Instance.CanEndEdit())
        {
            MapGridMgr.Instance.EndEdit();

            World.Scene.GetComponent<UIManagerComponent>().Remove(UI_PrefabPath.m_sUIPage_BuildOperation);
            World.Scene.GetComponent<UIManagerComponent>().ClearUIStack();
        }
        else
        {

            UI_Helper.ShowCommonTips(247);
        }
    }
    private void OnButtonClick_Rotation()
    {
        MapGridMgr.Instance.RotateEdit();
    }
}

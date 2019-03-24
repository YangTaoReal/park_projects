using QTFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ObjectEventSystem]
public class UIPopupWindow_Illustrated_ItemAwakeSystem : AAwake<UIPopupWindow_Illustrated_Item>
{
    public override void Awake(UIPopupWindow_Illustrated_Item _self)
    {
        _self.Awake();
    }
}

[UIEntityComponent(UI_PrefabPath.m_sUIPopupWindow_Illustrated_Item)]
public class UIPopupWindow_Illustrated_Item : UIComponent
{
    public RawImage m_RawImageIcon;
    public Text m_TextName;
    internal void Awake()
    {
        m_RawImageIcon = m_kParentEntity.m_kUIPrefab.GetCacheComponent(0) as RawImage;
        m_TextName = m_kParentEntity.m_kUIPrefab.GetCacheComponent(1) as Text;
    }

    public void Init(int _idModel)
    {
        var item = DBManager.Instance.m_kModel.GetEntryPtr(_idModel);
        if(item != null)
        {
            m_TextName.text = UI_Helper.GetTextByLanguageID(item._DisplayName);
            m_RawImageIcon.texture = UI_Helper.AllocTexture(item._Icon);
        }
    }
}

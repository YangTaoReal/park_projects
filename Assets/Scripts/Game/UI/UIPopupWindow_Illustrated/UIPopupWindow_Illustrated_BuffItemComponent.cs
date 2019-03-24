using QTFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ObjectEventSystem]
public class UIPopupWindow_Illustrated_BuffItemComponentAwakeSystem : AAwake<UIPopupWindow_Illustrated_BuffItemComponent>
{
    public override void Awake(UIPopupWindow_Illustrated_BuffItemComponent _self)
    {
        _self.Awake();
    }
}

[UIEntityComponent(UI_PrefabPath.m_sUIPopupWindow_Illustrated_BuffItem)]
public class UIPopupWindow_Illustrated_BuffItemComponent : UIComponent
{
    public Text m_TextBuffName;
    public Text m_TextBuffDesc;
    public RectTransform[] m_ArrRectTransform = new RectTransform[5];
    public RawImage[] m_ArrRawImage = new RawImage[5];
    public Text m_TextBuffState;
    public Text m_TextBuffUse;
    public GridLayoutGroup m_GridLayoutGroupItem;
    internal void Awake()
    {
        m_TextBuffName = m_kParentEntity.m_kUIPrefab.GetCacheComponent(0) as Text;
        m_TextBuffDesc = m_kParentEntity.m_kUIPrefab.GetCacheComponent(1) as Text;
        m_ArrRectTransform[0] = m_kParentEntity.m_kUIPrefab.GetCacheComponent(2) as RectTransform;
        m_ArrRectTransform[1] = m_kParentEntity.m_kUIPrefab.GetCacheComponent(3) as RectTransform;
        m_ArrRectTransform[2] = m_kParentEntity.m_kUIPrefab.GetCacheComponent(4) as RectTransform;
        m_ArrRectTransform[3] = m_kParentEntity.m_kUIPrefab.GetCacheComponent(5) as RectTransform;
        m_ArrRectTransform[4] = m_kParentEntity.m_kUIPrefab.GetCacheComponent(6) as RectTransform;
        m_ArrRawImage[0] = m_kParentEntity.m_kUIPrefab.GetCacheComponent(7) as RawImage;
        m_ArrRawImage[1] = m_kParentEntity.m_kUIPrefab.GetCacheComponent(8) as RawImage;
        m_ArrRawImage[2] = m_kParentEntity.m_kUIPrefab.GetCacheComponent(9) as RawImage;
        m_ArrRawImage[3] = m_kParentEntity.m_kUIPrefab.GetCacheComponent(10) as RawImage;
        m_ArrRawImage[4] = m_kParentEntity.m_kUIPrefab.GetCacheComponent(11) as RawImage;
        m_TextBuffState = m_kParentEntity.m_kUIPrefab.GetCacheComponent(12) as Text;
        m_TextBuffUse = m_kParentEntity.m_kUIPrefab.GetCacheComponent(13) as Text;
        m_GridLayoutGroupItem = m_kParentEntity.m_kUIPrefab.GetCacheComponent(14) as GridLayoutGroup;
    }

    public void Init(int _buffID)
    {
        foreach (var _rectTransform in m_ArrRectTransform)
        {
            _rectTransform.gameObject.SetActive(false);
        }

        var item = DBManager.Instance.m_kBuff.GetEntryPtr(_buffID);
        if(item != null)
        {
            m_TextBuffName.text = UI_Helper.GetTextByLanguageID(item._DisplayName);
            m_TextBuffDesc.text = UI_Helper.GetTextByLanguageID(item._Des);
            m_TextBuffState.text = BuffManager._instance.isHaveByCid(_buffID)? UI_Helper.GetTextByLanguageID(205) : UI_Helper.GetTextByLanguageID(206);
            m_TextBuffUse.text = item._Result > 0 ? $"+{(int)(Mathf.Abs(item._Result) * 100)}%" : $"-{(int)(Mathf.Abs(item._Result) * 100)}%";
            string[] modeID = item._Suit.Split('|');
            int index = 0;
            foreach (var _modeID in modeID)
            {
                if (_modeID != "0")
                {
                    var _mode = DBManager.Instance.m_kModel.GetEntryPtr(int.Parse(_modeID));
                    if (_mode != null)
                    {
                        m_ArrRectTransform[index].gameObject.SetActive(true);
                        m_ArrRawImage[index].texture = UI_Helper.AllocTexture(_mode._Icon);
                        index++;
                    }
                }
            }

            if (index == 2)
            {
                m_GridLayoutGroupItem.cellSize = new Vector2(95,95);
            }
            else if (index == 3)
            {
                m_GridLayoutGroupItem.cellSize = new Vector2(80, 80);
            }
            else if (index == 4)
            {
                m_GridLayoutGroupItem.cellSize = new Vector2(65, 65);
            }
            else if (index == 5)
            {
                m_GridLayoutGroupItem.cellSize = new Vector2(50, 50);
            }
        }
    }
}

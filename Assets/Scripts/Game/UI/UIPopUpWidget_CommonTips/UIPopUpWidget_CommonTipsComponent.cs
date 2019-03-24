using System;
using System.Collections;
using System.Collections.Generic;

using QTFramework;

using UnityEngine;

[ObjectEventSystem]
public class UIPopUpWidget_CommonTipsComponentAwakeSystem : AAwake<UIPopUpWidget_CommonTipsComponent>
{
    public override void Awake(UIPopUpWidget_CommonTipsComponent _self)
    {
        _self.Awake();
    }
}

[UIEntityComponent(UI_PrefabPath.m_sUIPopUpWidget_CommonTips)]
public class UIPopUpWidget_CommonTipsComponent : UIComponent
{
    public RectTransform m_kGameobject_Parent;
    List<string> _contentList = new List<string>();

    private Coroutine ShowCoroutine = null;
    internal void Awake()
    {
        m_kGameobject_Parent = m_kParentEntity.m_kUIPrefab.GetCacheComponent(0) as RectTransform;
        ShowCoroutine = null;
    }

    public void ShowTips(string _Content)
    {
        _contentList.Add(_Content);

        if (ShowCoroutine == null)
        {
            ShowCoroutine = m_kParentEntity.m_kUIPrefab.StartCoroutine(ShowList());
        }
    }

    IEnumerator ShowList()
    {
        while (_contentList.Count > 0)
        {
            UIEntity uIEntity = World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPopUpWidget_CommonTipsItem);
            uIEntity.m_kUIPrefab.gameObject.transform.SetParent(m_kGameobject_Parent.transform);
            uIEntity.m_kUIPrefab.gameObject.transform.localPosition = Vector3.zero;
            uIEntity.m_kUIPrefab.gameObject.transform.localEulerAngles = Vector3.zero;
            uIEntity.m_kUIPrefab.gameObject.transform.localScale = Vector3.one;
            m_kParentEntity.AddChildren(uIEntity);
            UIPopUpWidget_CommonTipsComponentItem _tipsComponentItem = uIEntity.GetComponent<UIPopUpWidget_CommonTipsComponentItem>();
            _tipsComponentItem.Init(_contentList[0]);
            _contentList.RemoveAt(0);
            yield return new WaitForSeconds(0.5f);
            m_kParentEntity.m_kUIPrefab.StartCoroutine(RemoveChild(uIEntity));
        }
        ShowCoroutine = null;
    }

    private IEnumerator RemoveChild(UIEntity _uIItem)
    {
        yield return new WaitForSeconds(3);
        m_kParentEntity.RemoveChildren(_uIItem);
    }
}
using QTFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ObjectEventSystem]
public class UIPopUpWindow_FriendItemComponentAwakeSystem : AAwake<UIPage_WeatherInfoItemComponent>
{
    public override void Awake(UIPage_WeatherInfoItemComponent _self)
    {
        _self.Awake();
    }
}

[UIEntityComponent(UI_PrefabPath.m_sUIPopUpWindow_Friend_Item)]
public class UIPopUpWindow_FriendItemComponent : UIComponent
{
    public RawImage m_kRawImage;
    public Text m_kTextName;
    public Button m_GiveGift;

    internal void Awake()
    {
        m_kRawImage = m_kParentEntity.m_kUIPrefab.GetCacheComponent(0) as RawImage;
        m_kTextName = m_kParentEntity.m_kUIPrefab.GetCacheComponent(1) as Text;
        m_GiveGift = m_kParentEntity.m_kUIPrefab.GetCacheComponent(2) as Button;
    }


    public void InitItem(string path, string name)
    {
        m_kTextName.text = name;

        m_kParentEntity.m_kUIPrefab.StartCoroutine(loadTexture(path));
    }

    IEnumerator loadTexture(string path)
    {
        WWW wWW = new WWW(path);
        yield return wWW;
        if (wWW.isDone)
        {
            m_kRawImage.texture = wWW.texture;
        }
    }
}

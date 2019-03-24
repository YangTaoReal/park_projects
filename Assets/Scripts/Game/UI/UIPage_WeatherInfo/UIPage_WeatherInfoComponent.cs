using System;
using System.Collections.Generic;
using QTFramework;

using UnityEngine;
using UnityEngine.UI;

[ObjectEventSystem]
public class UIPage_WeatherInfoComponentAwakeSystem : AAwake<UIPage_WeatherInfoComponent>
{
    public override void Awake(UIPage_WeatherInfoComponent _self)
    {
        _self.Awake();
    }
}


[UIEntityComponent(UI_PrefabPath.m_sUIPage_WeatherInfo)]
public class UIPage_WeatherInfoComponent : UIComponent
{
    public Button m_kButtonClose;
    public RectTransform m_kRectTransformContent;
    internal void Awake()
    {
        m_kRectTransformContent = m_kParentEntity.m_kUIPrefab.GetCacheComponent(0) as RectTransform;
        m_kButtonClose = m_kParentEntity.m_kUIPrefab.GetCacheComponent(1) as Button;

        m_kButtonClose.onClick.AddListener(OnButtonClick_Close);
        InitItem();

        ObserverHelper<bool>.SendMessage(MessageMonitorType.WeatherListOpen, this, new MessageArgs<bool>(true));
    }
    public override void Dispose()
    {
        m_kButtonClose.onClick.RemoveListener(OnButtonClick_Close);
        ObserverHelper<bool>.SendMessage(MessageMonitorType.WeatherListOpen, this, new MessageArgs<bool>(false));
    }
    private void OnButtonClick_Close()
    {
        World.Scene.GetComponent<UIManagerComponent>().Remove(UI_PrefabPath.m_sUIPage_WeatherInfo);
    }

    private void InitItem()
    {
        List<TimeRangWeather> _timeRangWeatherList = Weather.Instance.GetAllDayWeather();
        for (int i = 0; i < _timeRangWeatherList.Count; i++)
        {
            UIEntity uIEntity = World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPage_WeatherInfoItem);
            m_kParentEntity.AddChildren(uIEntity);
            UIPage_WeatherInfoItemComponent uIPage_WeatherInfoItemComponent = uIEntity.GetComponent<UIPage_WeatherInfoItemComponent>();
            uIPage_WeatherInfoItemComponent.InitItem(_timeRangWeatherList[i], _timeRangWeatherList[i] == Weather.Instance.GetCurWeather());
            uIEntity.m_kUIPrefab.gameObject.transform.SetParent(m_kRectTransformContent);
            uIEntity.m_kUIPrefab.gameObject.transform.localPosition = Vector3.zero;
        }
    }
}

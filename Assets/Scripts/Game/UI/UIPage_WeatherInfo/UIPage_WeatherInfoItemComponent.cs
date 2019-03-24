using QTFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ObjectEventSystem]
public class UIPage_WeatherInfoItemComponentAwakeSystem : AAwake<UIPage_WeatherInfoItemComponent>
{
    public override void Awake(UIPage_WeatherInfoItemComponent _self)
    {
        _self.Awake();
    }
}

[ObjectEventSystem]
public class UIPage_WeatherInfoItemComponentFixedUpdateSystem : AFixedUpdate<UIPage_WeatherInfoItemComponent>
{
    public override void FixedUpdate(UIPage_WeatherInfoItemComponent _self)
    {
        _self.FixedUpdate();
    }
}


[UIEntityComponent(UI_PrefabPath.m_sUIPage_WeatherInfoItem)]
public class UIPage_WeatherInfoItemComponent : UIComponent
{
    public RectTransform m_kRectTransformWeather;
    public RectTransform m_kRectTransformNew;

    public Text m_kTextWeatherTips;
    public Image m_kImageIcon;
    public Text m_kTextNewTips;

    internal void Awake()
    {
        m_kRectTransformWeather = m_kParentEntity.m_kUIPrefab.GetCacheComponent(0) as RectTransform;
        m_kRectTransformNew = m_kParentEntity.m_kUIPrefab.GetCacheComponent(1) as RectTransform;

        m_kTextWeatherTips = m_kParentEntity.m_kUIPrefab.GetCacheComponent(2) as Text;
        m_kImageIcon = m_kParentEntity.m_kUIPrefab.GetCacheComponent(3) as Image;
        m_kTextNewTips = m_kParentEntity.m_kUIPrefab.GetCacheComponent(4) as Text;
    }

    internal void FixedUpdate()
    {
        
    }

    public void InitItem(TimeRangWeather _timeRangWeather,bool isNow)
    {
        m_kRectTransformWeather.gameObject.SetActive(true);
        m_kRectTransformNew.gameObject.SetActive(false);
        m_kImageIcon.sprite = UI_Helper.GetSprite(GetWeatherSprite(_timeRangWeather.weaterType));
        m_kTextWeatherTips.text = $"{_timeRangWeather.from}:00-{_timeRangWeather.to}:00 " + ( isNow ? "NOW" : "");
    }

    public static string GetWeatherSprite(WeaterType weaterType)
    {
        if (weaterType == WeaterType.Sun)
        {
            return "bg_tq01";
        }
        else
        {
            return "bg_t01";
        }
    }
}

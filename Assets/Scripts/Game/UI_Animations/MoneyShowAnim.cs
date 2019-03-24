using System.Collections;
using System.Collections.Generic;
using QTFramework;
using UnityEngine;
using UnityEngine.Events;

[ObjectEventSystem]
public class MoneyShowAnimAwakeSystem : AAwake<MoneyShowAnim>
{
    public override void Awake(MoneyShowAnim _self)
    {
        _self.Awake();
    }
}


[UIEntityComponent(UI_PrefabPath.m_sUIPage_MoneyShowAnim)]
public class MoneyShowAnim : UIComponent {

    public Animator animator;
    public OnMoneyShowCallBack onMoneyShowCallBack;

	public void Awake()
    {
        UIEntity uI_Entity = ParentEntity as UIEntity;
        animator = uI_Entity.m_kUIPrefab.GetCacheComponent(0) as Animator;
        onMoneyShowCallBack = uI_Entity.m_kUIPrefab.GetCacheComponent(1) as OnMoneyShowCallBack;
    }

    public void SetCallBack(OnMoneyShowCallBack.CallBackType type, UnityAction callBack = null)
    {
        switch(type)
        {
            case OnMoneyShowCallBack.CallBackType.OnAnimationStart:
                onMoneyShowCallBack.SetCallBack(OnMoneyShowCallBack.CallBackType.OnAnimationStart, callBack);
                break;
            case OnMoneyShowCallBack.CallBackType.OnAnimationEnd:
                onMoneyShowCallBack.SetCallBack(OnMoneyShowCallBack.CallBackType.OnAnimationEnd, callBack);
                break;
        }
    }
}

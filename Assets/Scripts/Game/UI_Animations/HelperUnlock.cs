using System.Collections;
using System.Collections.Generic;
using QTFramework;
using UnityEngine;
using UnityEngine.Events;

[ObjectEventSystem]
public class HelperUnlockAwakeSystem : AAwake<HelperUnlock>
{
    public override void Awake(HelperUnlock _self)
    {
        _self.Awake();
    }
}


[UIEntityComponent(UI_PrefabPath.m_sUIPage_HelperUnlock)]
public class HelperUnlock : UIComponent {

    public Animator animator;
    public OnAnimationCallBack OnAnimationCallBack;

	public void Awake()
    {
        UIEntity uI_Entity = ParentEntity as UIEntity;
        animator = uI_Entity.m_kUIPrefab.GetCacheComponent(0) as Animator;
        OnAnimationCallBack = uI_Entity.m_kUIPrefab.GetCacheComponent(1) as OnAnimationCallBack;
    }

    public void SetCallBack(OnMoneyShowCallBack.CallBackType type, UnityAction callBack = null)
    {
        switch (type)
        {
            case OnMoneyShowCallBack.CallBackType.OnAnimationStart:
                OnAnimationCallBack.SetCallBack(OnAnimationCallBack.CallBackType.OnAnimationStart, callBack);
                break;
            case OnMoneyShowCallBack.CallBackType.OnAnimationEnd:
                OnAnimationCallBack.SetCallBack(OnAnimationCallBack.CallBackType.OnAnimationEnd, callBack);
                break;
        }
    }

}

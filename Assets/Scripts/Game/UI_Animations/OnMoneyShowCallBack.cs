using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnMoneyShowCallBack : MonoBehaviour {

    public UnityAction onAnimationEnd;
    public UnityAction onAnimationStart;

	public void OnAnimationEnd()
    {
        //if(onAnimationEnd != null)
        onAnimationEnd?.Invoke();
    }

    public void OnAnimationStart()
    {
        onAnimationStart?.Invoke();
    }

    public void SetCallBack(CallBackType type, UnityAction callBack)
    {
        switch(type)
        {
            case CallBackType.OnAnimationStart:
                onAnimationStart = callBack;
                break;
            case CallBackType.OnAnimationEnd:
                onAnimationEnd = callBack;
                break;
        }
    }

    public enum CallBackType
    {
        OnAnimationStart,
        OnAnimationEnd,
    }
}


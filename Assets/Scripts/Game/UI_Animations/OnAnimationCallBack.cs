using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnAnimationCallBack : MonoBehaviour {

	
    public UnityAction onAnimationStart;
    public UnityAction onAnimationEnd;

    public void OnAnimationStartCallBack()
    {
        onAnimationStart?.Invoke();
    }

    public void OnAnimationEndCallBack()
    {
        onAnimationEnd?.Invoke();
    }

    public void SetCallBack(CallBackType type, UnityAction callBack)
    {
        switch (type)
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

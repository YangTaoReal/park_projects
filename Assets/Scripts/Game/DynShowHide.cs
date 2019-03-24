using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynShowHide : MonoBehaviour {

    bool isShow = true;
    int srcLayer = 0;

    static Vector3 outPos = new Vector3(999999, 999999, 999999);
    Vector3 srcPos;


    private void Start()
    {
        srcLayer = gameObject.layer;
        srcPos = transform.position;
        isShow = true;
    }

    public static bool IsInView(Vector3 worldPos)
    {
        Vector2 viewPos = Camera.main.WorldToViewportPoint(worldPos);
        if (viewPos.x > -0.1f && viewPos.x < 1.1f && viewPos.y > -0.1f && viewPos.y < 1.1f)
            return true;
        else
        {
            return false;
        }

    }

    void Hide(Transform trasf)
    {
        trasf.gameObject.layer = 31;
        for (int i = 0; i < trasf.childCount; i++)
        {
            Hide(trasf.GetChild(i));
        }
    }

    void Show(Transform trasf)
    {
        trasf.gameObject.layer = srcLayer;
        for (int i = 0; i < trasf.childCount; i++)
        {
            Show(trasf.GetChild(i));
        }
    }


    //private void FixedUpdate()
    //{
    //    if (IsInView(srcPos))
    //    {
    //        if (!isShow)
    //        {
    //            isShow = true;
    //            transform.position = srcPos;
    //            //Show(transform);
    //        }

    //    }
    //    else
    //    {
    //        if (isShow)
    //        {
    //            isShow = false;
    //            transform.position = outPos;
    //            //Hide(transform);
    //        }
    //    }
    //}
}

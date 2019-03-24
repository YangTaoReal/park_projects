using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragBuild : MonoBehaviour,IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public ScrollRect scrollRect;
    private Vector3 BeginPosintion;
    RectTransform m_kRectTransform;
    private bool m_kWantBuilding = false;
    private bool m_kCreat = false;
    public Action m_kCreatActionCallBack;

    void Awake()
    {
        m_kWantBuilding = false;
        m_kRectTransform = gameObject.GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        m_kWantBuilding = false;
        scrollRect.OnBeginDrag(eventData);
        BeginPosintion = m_kRectTransform.anchoredPosition3D;
        SetDraggedPosition(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!m_kWantBuilding)
        {
            scrollRect.OnDrag(eventData);
        }

        SetDraggedPosition(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!m_kWantBuilding)
        {
           scrollRect.OnEndDrag(eventData);
        }

        m_kCreat = false;
        m_kWantBuilding = false;
        m_kRectTransform.anchoredPosition3D = BeginPosintion;
    }

    private void SetDraggedPosition(PointerEventData eventData)
    {
        if (m_kCreat)
        {
            return;
        }
        if (m_kRectTransform.anchoredPosition3D.y > -25 && !m_kCreat)
        {
            m_kRectTransform.anchoredPosition3D = BeginPosintion;
            m_kCreat = true;
            m_kCreatActionCallBack?.Invoke();

            BE.MobileRTSCam.instance.CreatEdit();
            return;
        }
        Vector3 globalMousePos;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(m_kRectTransform, eventData.position, eventData.pressEventCamera, out globalMousePos))
        {
            m_kRectTransform.position = new Vector3(m_kRectTransform.position.x, globalMousePos.y, 0);
        }
        if (m_kRectTransform.anchoredPosition3D.y < -126)
        {
            m_kRectTransform.anchoredPosition3D = BeginPosintion;
        }
        if (m_kRectTransform.anchoredPosition3D.y > -80)
        {
            m_kWantBuilding = true;
        }
    }
}

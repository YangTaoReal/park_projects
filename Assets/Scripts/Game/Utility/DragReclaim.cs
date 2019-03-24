using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragReclaim : MonoBehaviour,IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Vector3 BeginPosintion;
    public Action BeginDragAction;
    public Action OnDragAction;
    public Action OnEndDragAction;
    public RectTransform m_kRectTransform;
    void Awake()
    {
   
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        BeginDragAction?.Invoke();
        BeginPosintion = m_kRectTransform.anchoredPosition3D;
    }

    public void OnDrag(PointerEventData eventData)
    {
        OnDragAction?.Invoke();
        SetDraggedPosition(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        OnEndDragAction?.Invoke();
        m_kRectTransform.anchoredPosition3D = BeginPosintion;
    }

    private void SetDraggedPosition(PointerEventData eventData)
    {
        Vector3 globalMousePos;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(m_kRectTransform, eventData.position, eventData.pressEventCamera, out globalMousePos))
        {
            m_kRectTransform.position = globalMousePos;
        }
    }
}

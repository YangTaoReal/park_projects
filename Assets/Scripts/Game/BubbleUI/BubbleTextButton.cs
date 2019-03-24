using System.Collections;
using System.Collections.Generic;
using QTFramework;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[ObjectEventSystem]
public class BubbleTextButtonAwakeSystem : AAwake<BubbleTextButton>
{
    public override void Awake(BubbleTextButton _self)
    {
        _self.Awake();
    }
}

[ObjectEventSystem]
public class BubbleTextButtonUpdateSystem : ALateUpdate<BubbleTextButton>
{
    public override void LateUpdate(BubbleTextButton _self)
    {
        _self.LateUpdate();
    }
}

[UIEntityComponent(UI_PrefabPath.m_sUIPopUpWindow_BubbleText)]
public class BubbleTextButton : UIComponent {

    private  Button clickBtn;
    private  Text showText;
    public RectTransform nodeTR;
    public Image shapeImg;

    public Transform targetTR;   // 目标的位置 世界坐标

    private UnityAction onClick;

    private readonly Vector2 offsetPos = new Vector2(-1.9f, 214.3f);
    public void Awake()
    {
        UIEntity uI_Entity = ParentEntity as UIEntity;
        showText = uI_Entity.m_kUIPrefab.GetCacheComponent(0) as Text;
        clickBtn = uI_Entity.m_kUIPrefab.GetCacheComponent(1) as Button;
        nodeTR = uI_Entity.m_kUIPrefab.GetCacheComponent(2) as RectTransform;
        shapeImg = uI_Entity.m_kUIPrefab.GetCacheComponent(3) as Image;
    }
    public void LateUpdate()
    {
       
        FixPosition();
    }

    public void FixPosition()
    {
        Vector2 fixPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(UI_Helper.UINode.transform as RectTransform, Camera.main.WorldToScreenPoint(targetTR.position), UI_Helper.UICamera, out fixPos);
        nodeTR.anchoredPosition = new Vector2(fixPos.x + offsetPos.x, fixPos.y + offsetPos.y);
    }

    //public void ShowBuble(Transform target, string text = "",UnityAction onClick = null)
    //{
    //    targetTR = target;
    //    if(!string.IsNullOrEmpty(text))
    //    {
    //        showText.text = text;
    //    }

    //    if(onClick != null)
    //    {
    //        clickBtn.onClick.RemoveAllListeners();
    //        clickBtn.onClick.AddListener(onClick);
    //    }
    //}

    public void ShowBuble(Transform target, int lagId, UnityAction _onClick = null)
    {
        if (onClick != null)
        {
            clickBtn.onClick.RemoveListener(onClick);
        }
        onClick = _onClick;
        targetTR = target;
        showText.text = UI_Helper.GetTextByLanguageID(lagId);

        if (_onClick != null)
        {
            clickBtn.onClick.AddListener(onClick);
            //Debug.Log("bubble按钮的onClick回调已装填");
        }
    }
  
}

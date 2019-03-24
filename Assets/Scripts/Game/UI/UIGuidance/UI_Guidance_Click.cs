using System.Collections;
using System.Collections.Generic;
using QTFramework;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[ObjectEventSystem]
public class UI_GuidanceAwakeSystem : AAwake<UI_Guidance_Click>
{
    public override void Awake(UI_Guidance_Click _self)
    {
        _self.Awake();
    }
}

[UIEntityComponent(UI_PrefabPath.m_sUIPopUpWindow_Guidance_Click)]
public class UI_Guidance_Click : UIComponent 
{
    public RectTransform holeTransform;
    public RectTransform holeShape;  // 洞的形状
    public Image finger;
    public HoleImage maskImage;
    public Image lockImage;
    public RectTransform guidanceTextTR;
    public Text guidanceText;


	public void Awake()
    {
        UIEntity uI_Entity = ParentEntity as UIEntity;
        holeTransform = uI_Entity.m_kUIPrefab.GetCacheComponent(0) as RectTransform;
        holeShape = uI_Entity.m_kUIPrefab.GetCacheComponent(1) as RectTransform;
        finger = uI_Entity.m_kUIPrefab.GetCacheComponent(2) as Image;
        maskImage = uI_Entity.m_kUIPrefab.GetCacheComponent(3) as HoleImage;
        lockImage = uI_Entity.m_kUIPrefab.GetCacheComponent(4) as Image;
        guidanceTextTR = uI_Entity.m_kUIPrefab.GetCacheComponent(5) as RectTransform;
        guidanceText = uI_Entity.m_kUIPrefab.GetCacheComponent(6) as Text;

        //MyDebug.Log("awake中是否get得到lockImage = " + lockImage);
    }

    /// <summary>
    /// 聚焦
    /// </summary>
    /// <param name="screenPos">Screen position.</param>
    /// <param name="spriteSize">洞的大小.</param>
    /// <param name="isShowFinger">是否显示手指动画</c> is show finger.</param>
    /// <param name="isUI">是否是ui物体，ui物体和世界物体计算坐标方法有区别</param>
    /// <param name="isMask">是否强制引导  是否拦截洞以外的点击事件</param>
    public void FocusTarget(Vector2 screenPos,CS_Guidance.DataEntry data)
    {
        if (data._Shape == 0)    // 0 = 圆形
            holeShape.GetComponent<Image>().sprite = UI_Helper.GetSprite("t_yuan");
        else if (data._Shape == 1)   // 1 = 矩形
            holeShape.GetComponent<Image>().sprite = UI_Helper.GetSprite("mask_1");
        finger.gameObject.SetActive(data._isShowFinger);
        maskImage.gameObject.SetActive(data._isMask);
        holeShape.sizeDelta = data._Size;
        holeTransform.anchoredPosition = screenPos;
        ShowGuidanceText((GuidanceTextPos)data._GuidanceTextPos, data._LanID);
    }

    public void ShowGuidanceText(GuidanceTextPos textPos, int languageID = 0)
    {
        if(languageID != 0)
            guidanceText.text = UI_Helper.GetTextByLanguageID(languageID);
        Vector3 pos = guidanceTextTR.anchoredPosition3D;
        Vector2 screen = Vector2.zero,finalPos = Vector2.zero;

        switch(textPos)
        {
            case GuidanceTextPos.None:
                guidanceTextTR.gameObject.SetActive(false);
                break;
            case GuidanceTextPos.Top:
                screen = new Vector3(Screen.width / 2, Screen.height / 6 * 5);
                RectTransformUtility.ScreenPointToLocalPointInRectangle(UI_Helper.UINode.transform as RectTransform, screen, UI_Helper.UICamera, out finalPos);
                guidanceTextTR.anchoredPosition = finalPos;
                guidanceTextTR.gameObject.SetActive(true);
                break;
            case GuidanceTextPos.Middle:
                screen = new Vector3(Screen.width / 2, Screen.height / 2);
                RectTransformUtility.ScreenPointToLocalPointInRectangle(UI_Helper.UINode.transform as RectTransform, screen, UI_Helper.UICamera, out finalPos);
                guidanceTextTR.anchoredPosition = finalPos;
                guidanceTextTR.gameObject.SetActive(true);
                break;
            case GuidanceTextPos.Bottom:
                screen = new Vector3(Screen.width / 2, Screen.height / 6);
                RectTransformUtility.ScreenPointToLocalPointInRectangle(UI_Helper.UINode.transform as RectTransform, screen, UI_Helper.UICamera, out finalPos);
                guidanceTextTR.anchoredPosition = finalPos;
                guidanceTextTR.gameObject.SetActive(true);
                break;
        }
        Debug.Log($"当前pos类型 = {textPos.ToString()},screen={screen},finalPos={finalPos}");
    }

    public void SetMaskAlpha(float a)
    {
        //Debug.Log("还原mask的透明度到" + a);
        Color color = maskImage.color;
        maskImage.color = new Color(color.r, color.g, color.b, a);
        //Debug.LogError($"color={maskImage.color}");
    }

    public void SetLockImageEnabel(bool isEnable)
    {
        lockImage.gameObject.SetActive(isEnable);
        //MyDebug.Log("当前lockImg状态：" + lockImage.gameObject.activeInHierarchy);
    }

    public override void Dispose()
    {
        base.Dispose();
        // 关闭ui 将位置还原
        holeTransform.anchoredPosition3D = Vector3.zero;
        holeTransform.sizeDelta = new Vector2(100, 100);
        SetMaskAlpha(0.4f);
        //Debug.Log("关闭还原位置");
    }

}

public enum GuidanceTextPos
{
    None = 0,   // 为0则不显示文字面板
    Top = 1,
    Middle,
    Bottom,
}

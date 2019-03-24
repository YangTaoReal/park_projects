using System.Collections;
using System.Collections.Generic;
using ArabicSupport;
using QTFramework;

using UnityEngine;
using UnityEngine.Events;

public class UI_Helper
{

    private static Canvas uiNode;
    public static Canvas UINode{
        get{
            if (null == uiNode)
                uiNode = GameObject.Find("UINode").GetComponent<Canvas>();
            return uiNode;
        }
    }
    private static  Camera uiCamera;
    public static  Camera UICamera
    {
        get{
            if (null == uiCamera)
                uiCamera = GameObject.Find("UICamera").GetComponent<Camera>();
            return uiCamera;
        }
    }

    public static Sprite SetQualityBG(int style,int _quality)
    {
        if (style == 2)
        {
            return World.Scene.GetComponent<UIManagerComponent>().m_kCommonSpriteAtlas.GetSprite("Img_LV" + (_quality));
        }
        else
        {
            return World.Scene.GetComponent<UIManagerComponent>().m_kCommonSpriteAtlas.GetSprite("Img_kuang" + (_quality));
        }

      
    }

    public static Sprite GetSprite(string _Image)
    {
        return World.Scene.GetComponent<UIManagerComponent>().m_kCommonSpriteAtlas.GetSprite(_Image);
    }

    public static Texture AllocTexture(string _ImagePath)
    {
        return AssetPoolManager.Instance.Fetch<Texture>(_ImagePath);
    }

    public static void DeallocTexture(Texture _texture)
    {
        AssetPoolManager.Instance.Recycle(_texture);
    }

    public static void ShowCommonTips(string _content)
    {
        UIEntity UIPopUpWidget_CommonTips = World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPopUpWidget_CommonTips);
        if(UIPopUpWidget_CommonTips != null)
        UIPopUpWidget_CommonTips.GetComponent<UIPopUpWidget_CommonTipsComponent>().ShowTips(_content);
    }
    public static void ShowCommonTips(int _languageID, params string[] _str)
    {
        UIEntity UIPopUpWidget_CommonTips = World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPopUpWidget_CommonTips);
        if (UIPopUpWidget_CommonTips != null)
            UIPopUpWidget_CommonTips.GetComponent<UIPopUpWidget_CommonTipsComponent>().ShowTips(UI_Helper.GetTextByLanguageID(_languageID, _str));
    }
    public static string GetTextByLanguageID(int _ID, params string[] _str)
    {
        string str = "";

        CS_Language.DataEntry dataEntry = DBManager.Instance.m_kLanguage.GetEntryPtr(_ID);
        if (dataEntry != null)
        {
            switch (World.m_kLanguage)
            {
                case QTLanguage.Chinese:
                    str = string.Format(dataEntry._Chinese, _str);
                    break;
                case QTLanguage.English:
                    str = string.Format(dataEntry._English, _str);
                    break;
                case QTLanguage.Arabic:
                    str = ArabicFixer.Fix(string.Format(dataEntry._Arabic, _str), true, false);
                    break;
                case QTLanguage.Russian:
                    str = string.Format(dataEntry._Russian, _str);
                    break;
            }
        }
        else
        {
            str = "LanID(" + _ID +")";
            Log.Error("ViewHelper", $"Language ID:{_ID} 没找到哦！");
        }

        return str;
    }
    public static void ShowConfirmPanel(string txt, UnityAction singleConfirmCallBack = null)
    {
        UIEntity entity = World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPopUpWindow_Confirm);
        entity.GetComponent<UI_Confirm>().Init(txt, singleConfirmCallBack);
    }
    public static UIEntity ShowConfirmPanel(int _languageID, UnityAction singleConfirmCallBack= null,params string[] _str)
    {
        UIEntity entity = World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPopUpWindow_Confirm);
        entity.GetComponent<UI_Confirm>().Init(GetTextByLanguageID(_languageID,_str), singleConfirmCallBack);
        return entity;
    }
    public static UIEntity ShowConfirmPanel(string txt, UnityAction doubleConfirmCallBack = null, UnityAction cancelCallBack = null)
    {
        UIEntity entity = World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPopUpWindow_Confirm);
        entity.GetComponent<UI_Confirm>().Init(txt, doubleConfirmCallBack, cancelCallBack);
        return entity;
    }
    public static void ShowConfirmPanel(int _languageID, UnityAction doubleConfirmCallBack= null, UnityAction cancelCallBack = null,params string[] _str)
    {
        UIEntity entity = World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPopUpWindow_Confirm);
        entity.GetComponent<UI_Confirm>().Init(GetTextByLanguageID(_languageID, _str),doubleConfirmCallBack,cancelCallBack);
    }

    public static void ShowGuidance_Click(Transform target,CS_Guidance.DataEntry data,UnityAction CallBack = null)
    {
        GuidanceManager.needClick = true;
        if(CallBack != null)
            World.Scene.GetComponent<GuidanceManager>().currClickCallBack = CallBack;
        Vector2 screenPos = Vector2.zero;            
        if (data._isUI)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(UINode.transform as RectTransform, UICamera.WorldToScreenPoint(target.position), UICamera, out screenPos);
            data._Size = target.GetComponent<RectTransform>().sizeDelta;
            //Debug.Log($"获取的图片的大小 = {target.GetComponent<RectTransform>().sizeDelta}");
        }
        else
        {
            Vector2  pos = Camera.main.WorldToScreenPoint(target.position);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(UINode.transform as RectTransform, pos, UICamera, out screenPos);
        }
        //Debug.LogFormat("buttonPos:{0},转换后屏幕坐标:{1}  尺寸:{2}", UICamera.WorldToScreenPoint(target.transform.position), screenPos, size);
        //if (data._LanID != 0)
            //ShowCommonTips(data._LanID);
        //UIEntity entity = World.Scene.GetComponent<UIManagerComponent>().Get(UI_PrefabPath.m_sUIPopUpWindow_Guidance_Click);
        UIEntity entity = World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPopUpWindow_Guidance_Click);
        UI_Guidance_Click guidance_Click = entity.GetComponent<UI_Guidance_Click>();
        guidance_Click.FocusTarget(screenPos, data);
        guidance_Click.SetLockImageEnabel(false);
        World.Scene.GetComponent<UIManagerComponent>().Get(UI_PrefabPath.m_sUIPopUpWindow_Guidance_Click).GetComponent<UI_Guidance_Click>().SetMaskAlpha(0.4f);
    }
    // 针对只能
    public static void ShowGuidance_Click(Vector3 target, CS_Guidance.DataEntry data, UnityAction CallBack = null)
    {
        GuidanceManager.needClick = true;
        if (CallBack != null)
            World.Scene.GetComponent<GuidanceManager>().currClickCallBack = CallBack;
        Vector2 screenPos = Vector2.zero;

        Vector2 pos = Camera.main.WorldToScreenPoint(target);
        //Debug.LogError($"转回来的pos：{pos}");
        RectTransformUtility.ScreenPointToLocalPointInRectangle(UINode.transform as RectTransform, pos, UICamera, out screenPos);

        //Debug.LogFormat("buttonPos:{0},转换后屏幕坐标:{1}  尺寸:{2}", UICamera.WorldToScreenPoint(target.transform.position), screenPos, size);
        //if (data._LanID != 0)
            //ShowCommonTips(data._LanID);
        //UIEntity entity = World.Scene.GetComponent<UIManagerComponent>().Get(UI_PrefabPath.m_sUIPopUpWindow_Guidance_Click);
        UIEntity entity = World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPopUpWindow_Guidance_Click);
        entity.GetComponent<UI_Guidance_Click>().FocusTarget(screenPos, data);
        World.Scene.GetComponent<UIManagerComponent>().Get(UI_PrefabPath.m_sUIPopUpWindow_Guidance_Click).GetComponent<UI_Guidance_Click>().SetMaskAlpha(0.4f);
    }

    /// <summary>
    /// 传入屏幕坐标
    /// </summary>
    /// <param name="start">Start.</param>
    /// <param name="end">End.</param>
    /// <param name="CallBack">Call back.</param>
    public static void ShowGuidance_Slide(Vector3 start,Vector3 end,UnityAction CallBack = null)
    {
        //Vector3 startPos = UICamera.WorldToScreenPoint(start);
        //Vector3 endPos = UICamera.WorldToScreenPoint(end);
        if (CallBack != null)
            World.Scene.GetComponent<GuidanceManager>().currSlideCallBack = CallBack;
        Vector3 dicretion = end - start;
        Vector2 first = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(UINode.transform as RectTransform, start, UICamera, out first);
        UIEntity entity = World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPopUpWindow_Guidance_Slide);
        entity.GetComponent<UI_Guidance_Slide>().canvasGroup.alpha = 1;
        entity.GetComponent<UI_Guidance_Slide>().SetArrowPos(first, dicretion);
    }

    // 添加有文字显示的bubble
    public static UIEntity AddBubleTextNode(Transform target,int lanId,UnityAction onClick = null)
    {
        UIEntity entity = World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPopUpWindow_BubbleText);
        //entity.m_kUIPrefab.transform.SetParent(parent);
        //entity.m_kUIPrefab.transform.localPosition = new Vector3(-2.34f,-4.23f,1.65f);
        entity.GetComponent<BubbleTextButton>().ShowBuble(target, lanId, onClick);
        return entity;
    }
   
    /// <summary>
    /// 改名卡界面
    /// </summary>
    /// <returns>The sign name panel.</returns>
    public static UIEntity ShowSignNamePanel(UnityAction confirmCallBack)
    {
        UIEntity entity = World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPage_SignName);
        entity.GetComponent<UI_SignName>().InitCallBack(confirmCallBack);
        return entity;
    }

    // 展示金钱奖励界面
    public static UIEntity MoneyShowAnim(UnityAction startCallBack = null,UnityAction endCallBack = null)
    {
        UIEntity entity = World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPage_MoneyShowAnim);
        if (entity != null)
        {
            if (startCallBack != null)
                entity.GetComponent<MoneyShowAnim>().SetCallBack(OnMoneyShowCallBack.CallBackType.OnAnimationStart, startCallBack);
            if (endCallBack != null)
                entity.GetComponent<MoneyShowAnim>().SetCallBack(OnMoneyShowCallBack.CallBackType.OnAnimationEnd, endCallBack);
        }
        return entity;
    }
}

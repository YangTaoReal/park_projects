
using DG.Tweening;
using QTFramework;
using UnityEngine;

[ObjectEventSystem]
public class UI_GuidanceSlideAwakeSystem : AAwake<UI_Guidance_Slide>
{
    public override void Awake(UI_Guidance_Slide _self)
    {
        _self.Awake();
    }
}


[UIEntityComponent(UI_PrefabPath.m_sUIPopUpWindow_Guidance_Slide)]
public class UI_Guidance_Slide : UIComponent
{
    public RectTransform arrowTR;

    public RectTransform startPos;
    public RectTransform endPos;
    public RectTransform fingerTR;  // 手指

    public CanvasGroup canvasGroup;

    private Vector3 fingerstart;
    private Tweener tweener;
    public void Awake()
    {
        UIEntity uI_Entity = ParentEntity as UIEntity;
        arrowTR = uI_Entity.m_kUIPrefab.GetCacheComponent(0) as RectTransform;
        startPos = uI_Entity.m_kUIPrefab.GetCacheComponent(1) as RectTransform;
        endPos = uI_Entity.m_kUIPrefab.GetCacheComponent(2) as RectTransform;
        fingerTR = uI_Entity.m_kUIPrefab.GetCacheComponent(3) as RectTransform;
        canvasGroup = uI_Entity.m_kUIPrefab.GetCacheComponent(4) as CanvasGroup;

        fingerstart = fingerTR.localPosition;
    }

    public void SetArrowPos(Vector3 startPos, Vector3 direction)
    {
        arrowTR.up = direction.normalized;
        arrowTR.anchoredPosition = startPos;
        ShowFinger();
    }

    public override void Dispose()
    {
        base.Dispose();
        // 关闭ui 还原参数
        //canvasGroup.alpha = 1;
        arrowTR.anchoredPosition = Vector2.zero;
        fingerTR.localPosition = fingerstart;
    }

    public void ClosePanel(float fadeTime)
    {
        canvasGroup.DOFade(0, fadeTime).OnComplete(()=>{

            World.Scene.GetComponent<UIManagerComponent>().Remove(UI_PrefabPath.m_sUIPopUpWindow_Guidance_Slide);
        });
    }

    public void ShowFinger()
    {
        //fingerTR.anchoredPosition = startPos.anchoredPosition;
        //Vector2 start, end;
        //RectTransformUtility.ScreenPointToLocalPointInRectangle(UI_Helper.UINode.transform as RectTransform, UI_Helper.UICamera.WorldToScreenPoint(startPos.position), UI_Helper.UICamera, out start);
        //fingerTR.anchoredPosition = start;
        //RectTransformUtility.ScreenPointToLocalPointInRectangle(UI_Helper.UINode.transform as RectTransform, UI_Helper.UICamera.WorldToScreenPoint(endPos.position), UI_Helper.UICamera, out end);
        //Debug.LogError($"endPos:{endPos.position},localpos:{endPos.localPosition},anchoredpos:{endPos.anchoredPosition}");
        //Debug.LogError($"赋值前手指位置:{fingerTR.anchoredPosition},startPos:{startPos.anchoredPosition},endPos:{endPos.anchoredPosition}");

        tweener.Kill();
        fingerTR.localPosition = startPos.localPosition;
        //Debug.LogError($"赋值后手指位置:{fingerTR.anchoredPosition},startPos:{startPos.anchoredPosition},endPos:{endPos.anchoredPosition}");

        //fingerTR.DOLocalMove(endPos.localPosition, 1.5f).SetLoops(-1);
        tweener = fingerTR.DOLocalMove(endPos.localPosition, 1.5f).SetLoops(-1);

    }

}
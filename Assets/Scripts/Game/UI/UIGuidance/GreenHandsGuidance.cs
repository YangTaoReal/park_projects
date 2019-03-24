using System;
using System.Collections;
using System.Collections.Generic;
using BE;
using QTFramework;
using UnityEngine;
using UnityEngine.UI;

[ObjectEventSystem]
public class GreenHandsGuidanceAwakeSystem : AUpdate<GreenHandsGuidance>
{
    public override void Update(GreenHandsGuidance _self)
    {
        _self.Update();
    }
}


public class GreenHandsGuidance : QTComponent
{
    GuidanceManager guidanceManager = null;

    private const float moveCameraTime = 0.3f;
    private bool isSendGUidanceZoomMessage = false; //是否发送过缩放界面的消息了、
    public bool isKaiHuangSuccess = false;  // 是否成功开荒指定的那一块地
	public void Start()
    {
        MyDebug.Log("开始新手引导");
        guidanceManager = World.Scene.GetComponent<GuidanceManager>();

        AddEvent();

        GuidanceManager.currEvent = GuidanceEvent.GreenHandEvent;

        CheckCurrStep();
    }

    public void Update()
    {
        //MyDebug.Log("000000000000000000");
        if(Input.GetMouseButtonUp(0))
        {
            if(GuidanceManager.isGuidancing && GuidanceManager.currStep == GuidanceStep.Step12)
            {
                //MyDebug.Log("抬起按键，检测一次");
                CheckIfSkip();
            }

        }

        if (MobileRTSCam.instance.isHasZoom)
        {
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                if (!GuidanceManager.isGuidancing)
                    return;
                if (GuidanceManager.currStep == GuidanceStep.Step2)
                {
                    if (GuidanceManager.isGuidancing && !isSendGUidanceZoomMessage && GuidanceManager.currStep == GuidanceStep.Step2)
                    {
                        isSendGUidanceZoomMessage = true;
                        //Debug.Log($"发送新手引导缩放界面的消息了");
                        //GuidanceManager._Instance.CheckGuidance(null, MessageMonitorType.GuidanceSlideEvent);


                        Debug.Log($"第二只手指松开了,发送缩放事件消息过去");
                        GuidanceManager._Instance.CheckGuidance(null, MessageMonitorType.GuidanceClickEvent);
                        TimerUtil.SetTimeOut(0.1f, () =>
                        {

                        //MobileRTSCam.instance.CanEdit = true;
                        Debug.Log($"摄像机是否能检测事件 = {MobileRTSCam.instance.isCanGetRayCast}");
                        });
                    }
                }
            }
        }
        //else if(Input.touchCount >= 2 && Input.GetTouch(0).phase == TouchPhase.Ended)
        //{
        //    if (GuidanceManager.currStep == GuidanceStep.Step2)
        //    {
        //        Debug.Log($"第一只手指松开了,恢复地图射线检测事件");
        //        TimerUtil.SetTimeOut(0.1f, () =>
        //        {
        //            GuidanceManager._Instance.CheckGuidance(null, MessageMonitorType.GuidanceSlideEvent);

        //            MobileRTSCam.instance.isCanEdit = true;
        //        });
        //    }
        //}
    }


    public void CheckCurrStep()
    {
        //PlayerPrefs.SetInt("currGuidanceStep", 1);
        int step = PlayerPrefs.GetInt("currGuidanceStep", 0);
        //step = 11;
        GuidanceStep preStep = (GuidanceStep)step;
        step--;
        if (step < 0)
            step = 0;
        GuidanceManager.currStep = (GuidanceStep)step;
        //GuidanceManager.currStep = GuidanceStep.None;

        guidanceManager.ClearAllUI();   // 强制关闭所有的ui 跳转到指定的新手引导ui中

        UIEntity entity = null;
        GameObject mapObj = null;
        Vector3 mapPos = Vector3.zero;
        //.Log($"当前Step:{GuidanceManager.currStep}");
        switch(preStep)
        {
            case GuidanceStep.Step1:
                guidanceManager.SetHallPanelButtonsEnable(false);
                break;
            case GuidanceStep.Step2:    // 缩放引导
                GuidanceManager._Instance.SetHallPanelButtonsEnable(false);
                //entity = World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPage_ZoomGuideAnim);
                break;
            case GuidanceStep.Step3:
                mapObj = ModelManager._instance.GetModleByType(ModelCType.MainBase)[0].go;
                MobileRTSCam.instance.SmoothMoveCamera(0.1f, mapObj.transform.position);
                MapGridMgr.Instance.Foucs(mapObj.GetComponent<TileInfo>());
                break;
            case GuidanceStep.Step4:    // 点击仓库
                entity = World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPage_BuildingOperation);
                mapObj = ModelManager._instance.GetModleByType(ModelCType.MainBase)[0].go;
                MobileRTSCam.instance.SmoothMoveCamera(0.1f, mapObj.transform.position);
                TileInfo info = mapObj.GetComponent<TileInfo>();
                MapGridMgr.Instance.Foucs(info);
                break;
            case GuidanceStep.Step5:
                entity = World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPage_Pack);
                entity.GetComponent<UIPage_PackComponent>().InitUI(false, UIPage_PackComponent.Category.Warehouse);
                mapObj = ModelManager._instance.GetModleByType(ModelCType.MainBase)[0].go;
                MobileRTSCam.instance.SmoothMoveCamera(0.1f, mapObj.transform.position);
                MapGridMgr.Instance.Foucs(mapObj.GetComponent<TileInfo>());
                break;
            case GuidanceStep.Step6:
                //World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPage_PackItemDetails);
                entity = World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPage_Pack);
                entity.GetComponent<UIPage_PackComponent>().InitUI(false, UIPage_PackComponent.Category.Warehouse);
                mapObj = ModelManager._instance.GetModleByType(ModelCType.MainBase)[0].go;
                MobileRTSCam.instance.SmoothMoveCamera(0.1f, mapObj.transform.position);
                MapGridMgr.Instance.Foucs(mapObj.GetComponent<TileInfo>());
                break;
            case GuidanceStep.Step7:    // 点击荒地
                mapPos = MapGridMgr.Instance.GetUserGuideWastedlandTransform();
                MobileRTSCam.instance.SmoothMoveCamera(0.1f, mapPos);
                break;
            case GuidanceStep.Step8:    // 拖拽开荒荒地
                GuidanceManager.currStep = GuidanceStep.Step6;
                mapPos = MapGridMgr.Instance.GetUserGuideWastedlandTransform();
                MobileRTSCam.instance.SmoothMoveCamera(0.1f, mapPos);
                break;
            case GuidanceStep.Step9:    // 确认面板
                mapPos = MapGridMgr.Instance.GetUserGuideWastedlandTransform();
                MobileRTSCam.instance.SmoothMoveCamera(0.1f, mapPos);
                GuidanceManager.currStep = GuidanceStep.Step9;
                entity = World.Scene.GetComponent<UIManagerComponent>().Get(UI_PrefabPath.m_sUISystem_Hall);
                break;
            case GuidanceStep.Step10:   // 点击修建按钮
                mapPos = MapGridMgr.Instance.GetUserGuideWastedlandTransform();
                MobileRTSCam.instance.SmoothMoveCamera(0.1f, mapPos);
                MobileRTSCam.instance.SetCameraZoom(21.5f);
                entity = World.Scene.GetComponent<UIManagerComponent>().Get(UI_PrefabPath.m_sUISystem_Hall);
                break;
            case GuidanceStep.Step11:   // 拖动
                MobileRTSCam.instance.SmoothMoveCamera(0.1f, MapGridMgr.Instance.GetUserGuideWastedlandTransform());
                MobileRTSCam.instance.SetCameraZoom(21.5f);
                entity = World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPage_Build);
                break;
            case GuidanceStep.Step12:   // 点击指定位置
                MobileRTSCam.instance.SetCameraZoom(21.5f);
                MobileRTSCam.instance.SmoothMoveCamera(0.1f, MapGridMgr.Instance.GetUserGuideWastedlandTransform());
                GuidanceManager.currStep = GuidanceStep.Step10;
                entity = World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPage_Build);
                break;
            case GuidanceStep.Step13:   // 
                MobileRTSCam.instance.SetCameraZoom(21.5f);
                MobileRTSCam.instance.SmoothMoveCamera(0.1f, MapGridMgr.Instance.GetUserGuideWastedlandTransform());
                GuidanceManager.currStep = GuidanceStep.Step10;
                entity = World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPage_Build);
                break;
            case GuidanceStep.Step14:   // 
                //GuidanceManager.currStep = GuidanceStep.Step9;
                entity = World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPopUpWindow_BuildingName);
                break;
            case GuidanceStep.Step15:
                //MobileRTSCam.instance.SmoothMoveCamera(0.1f, MapGridMgr.Instance.GetUserGuideWastedlandTransform());
                //GuidanceManager.currStep = GuidanceStep.Step9;
                //entity = World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPage_Build);
                break;
            case GuidanceStep.Step16:
                //GuidanceManager.currStep = GuidanceStep.Step9;
                entity = World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPopUpWindow_SpeedUp);
                BaseData building = MapGridMgr.Instance.GetUserGuideWastedlandObject().GetComponent<Building>().baseData;
                entity.GetComponent<UI_SpeedUP>().InitGuid(SceneLogic._instance.listDispose[0][0].barsMgr.bar, building);

                break;
            case GuidanceStep.Step17:
                //GuidanceManager.currStep = GuidanceStep.Step15;
                MobileRTSCam.instance.SmoothMoveCamera(0.1f, MapGridMgr.Instance.GetUserGuideWastedlandTransform());
                break;
            case GuidanceStep.Step18:
                GuidanceManager.currStep = GuidanceStep.Step16;
                break;
            case GuidanceStep.Step19:
                GuidanceManager.currStep = GuidanceStep.Step16;
                break;
            case GuidanceStep.Step20:
                GuidanceManager.currStep = GuidanceStep.Step16;
                break;
        }
        if(entity != null)
        {
            guidanceManager.SetCurrEntity(entity);
        }


        //MyDebug.Log($"当前指导事件:{GuidanceManager.currEvent.ToString()}--当前步数:{GuidanceManager.currStep}");
        guidanceManager.NextStep();

    }

    //public void CheckIfHasBuild()
    //{
    //    var go = MapGridMgr.Instance.GetUserGuideWastedlandObject();
    //    if(go == null)
    //    {
    //        MyDebug.Log($"当前位置上没有任何建筑物或者荒地");
    //        BaseData data = ModelManager._instance.Load(20501);
    //        data.go.transform.position = MapGridMgr.Instance.GetUserGuideWastedlandTransform();

    //        data.go.SetActive(false);
    //        StartBuildingEven even = new StartBuildingEven();
    //        even.bIs = true;
    //        even.guids = new List<Guid>{data.guid};
    //        even.ReTyp = 1;
    //        even.isReMake = false;//重新开始加载
    //        ObserverHelper<StartBuildingEven>.SendMessage(MessageMonitorType.StartBuilding, this, new MessageArgs<StartBuildingEven>(even));

    //    }
    //    else
    //    {
    //        MyDebug.Log($"go不为空，{go.name}");
    //    }
    //}

    void AddEvent()
    {
        guidanceManager.guidanceEventDic[GuidanceStep.Step1] = Slide_MoveCamera;
        guidanceManager.guidanceEventDic[GuidanceStep.Step2] = Slide_Zoom;  // 添加缩放引导
        guidanceManager.guidanceEventDic[GuidanceStep.Step3] = Click_XiaoMuWu;
        guidanceManager.guidanceEventDic[GuidanceStep.Step4] = Click_Cangku;
        guidanceManager.guidanceEventDic[GuidanceStep.Step5] = Click_CangKuItem;
        guidanceManager.guidanceEventDic[GuidanceStep.Step6] = Click_KongBai;
        guidanceManager.guidanceEventDic[GuidanceStep.Step7] = Click_HuangDi;
        guidanceManager.guidanceEventDic[GuidanceStep.Step8] = Slide_KaiKeng;
        guidanceManager.guidanceEventDic[GuidanceStep.Step9] = Click_DiaoLuo;
        guidanceManager.guidanceEventDic[GuidanceStep.Step10] = Click_XiuJian;
        guidanceManager.guidanceEventDic[GuidanceStep.Step11] = Slide_ZhaLan;
        guidanceManager.guidanceEventDic[GuidanceStep.Step12] = Click_JianZaoPosition;
        guidanceManager.guidanceEventDic[GuidanceStep.Step13] = Click_GouMai;
        guidanceManager.guidanceEventDic[GuidanceStep.Step14] = Click_GuanbiGouMai;
        guidanceManager.guidanceEventDic[GuidanceStep.Step15] = Click_JiaSu;
        guidanceManager.guidanceEventDic[GuidanceStep.Step16] = Click_QueRenJiaSu;
        guidanceManager.guidanceEventDic[GuidanceStep.Step17] = Click_JinRuZhaLan;
        guidanceManager.guidanceEventDic[GuidanceStep.Step18] = Click_FangZhi;
        guidanceManager.guidanceEventDic[GuidanceStep.Step19] = Click_DianJiTuZi;
        guidanceManager.guidanceEventDic[GuidanceStep.Step20] = Click_DianJiShuMu;

    }

    /// <summary>
    /// 滑动屏幕引导
    /// </summary>
    public void Slide_MoveCamera()
    {
        MobileRTSCam.instance.isCanGetRayCast = false;
        World.Scene.GetComponent<UIManagerComponent>().Get(UI_PrefabPath.m_sUISystem_Hall).GetComponent<UISystem_HallComponent>().SetAllButtonsDisable(false);
        GuidanceManager.needSlide = true;
        Vector3 start = new Vector3(Screen.width / 2, Screen.height / 2 + 50);
        //Vector3 start = UI_Helper.UICamera.ScreenToWorldPoint(screen);
        Vector3 end = new Vector3(Screen.width, Screen.height);
        //Vector3 end = UI_Helper.UICamera.ScreenToWorldPoint(right);
        UI_Helper.ShowGuidance_Slide(start, end,()=>{
            guidanceManager.CloseGuidanceSlidePanel(2f);
        });
    }

    /// <summary>
    /// 引导摄像机缩(放大)
    /// </summary>
    public void Slide_Zoom()
    {
        // 展示缩放界面
        TimerUtil.SetTimeOut(2f, () =>
        {
            Debug.Log($"开始缩放引导");

            World.Scene.GetComponent<UIManagerComponent>().ClearUIStack();
            World.Scene.GetComponent<UIManagerComponent>().RemoveAll(UI_PrefabPath.m_sUISystem_Hall);
            MapGridMgr.Instance.EndFreeingWastedland();
            MobileRTSCam.instance.SmoothMoveCamera(moveCameraTime, ModelManager._instance.GetModleByType(ModelCType.MainBase)[0].go.transform.position, () =>
            {
                World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPage_ZoomGuideAnim);
                MobileRTSCam.instance.isCanPinch = true;
                MobileRTSCam.instance.isCanGetRayCast = false;
                MobileRTSCam.instance.SetCameraEnable(false);
            });
        });
      


    }

    /// <summary>
    /// 点击小木屋引导
    /// </summary>
    public void Click_XiaoMuWu()
    {
        MyDebug.Log("开始点击小木屋的引导");
        // 特殊情况，在这里还原缩放引导的参数
        MobileRTSCam.instance.SetCameraEnable(true);
        MobileRTSCam.instance.isCanPinch = false;
        MobileRTSCam.instance.SetCameraZoom(21.5f);
        //World.Scene.GetComponent<UIManagerComponent>().Remove(UI_PrefabPath.m_sUIPage_ZoomGuideAnim);
        // 确认小木屋在摄像机的视野范围内 
        World.Scene.GetComponent<UIManagerComponent>().ClearUIStack();
        World.Scene.GetComponent<UIManagerComponent>().RemoveAll(UI_PrefabPath.m_sUISystem_Hall);
        MapGridMgr.Instance.EndFreeingWastedland();

        GameObject house = ModelManager._instance.GetModleByType(ModelCType.MainBase)[0].go;
        MobileRTSCam.instance.SmoothMoveCamera(moveCameraTime, house.transform.position, () => {

            MobileRTSCam.instance.isCanGetRayCast = true;
            World.Scene.GetComponent<UIManagerComponent>().Get(UI_PrefabPath.m_sUISystem_Hall).GetComponent<UISystem_HallComponent>().SetAllButtonsDisable(true);
            World.Scene.GetComponent<UIManagerComponent>().ClearUIStack();
            World.Scene.GetComponent<UIManagerComponent>().RemoveAll(UI_PrefabPath.m_sUISystem_Hall);
            MapGridMgr.Instance.UnFoucs();
            MapGridMgr.Instance.EndFreeingWastedland();
            MobileRTSCam.instance.SetCameraEnable(false);
            var data = guidanceManager.dataList[(int)GuidanceManager.currStep - 1];
            MyDebug.Log("摄像机移动完成,接下里打开click面板");
            UI_Helper.ShowGuidance_Click(house.transform, data, () => {
                MyDebug.Log($"响应点击回调,打开摄像机自由移动");
                MobileRTSCam.instance.SetCameraEnable(true);
            });
        });
        //TimerUtil.SetTimeOut(2f, () =>
        //{
           

        //});

    }

    /// <summary>
    /// 点击仓库按钮引导
    /// </summary>
    void Click_Cangku()
    {
        MyDebug.Log("开始点击仓库引导");
        Transform tr = guidanceManager.GetTargetTransform("Storage");
        Debug.Log(tr.name);
        MyDebug.Log($"打开仓库获取到的button = {tr},button的大小：{tr.GetComponent<RectTransform>().sizeDelta}");
        CS_Guidance.DataEntry data = guidanceManager.dataList[(int)GuidanceManager.currStep - 1];
        MyDebug.Log($"isUI:{data._isUI},step:{data._Step},size:{data._Size}");
        UI_Helper.ShowGuidance_Click(tr, data, () => {
            MyDebug.Log("点击仓库的回调======");
        });
        TimerUtil.SetTimeOut(0.1f, () =>
        {
            
        });
    }

    /// <summary>
    /// 点击仓库Item引导
    /// </summary>
    void Click_CangKuItem()
    {
        TimerUtil.SetTimeOut(0.1f, () =>
        {
            MyDebug.Log($"开始点击仓库item引导");
            var com = World.Scene.GetComponent<UIManagerComponent>().Get(UI_PrefabPath.m_sUIPage_Pack).GetComponent<UIPage_PackComponent>();
            Transform tr = com.m_kScrollRectScrollWarehouse.content.GetChild(0);
            MyDebug.Log($"打开仓库获取到的Item = {tr},button的大小：{tr.GetComponent<RectTransform>().sizeDelta}");
            CS_Guidance.DataEntry data = guidanceManager.dataList[(int)GuidanceManager.currStep - 1];
            UI_Helper.ShowGuidance_Click(tr, data);
        });

    }

    /// <summary>
    /// 2s后提示点击空白
    /// </summary>
    void Click_KongBai()
    {
        // 提示点击空白处
        MyDebug.Log($"开始空白处引导");
        Vector3 screen = new Vector3(Screen.width / 2, Screen.height / 6, 0);
        //MyDebug.LogError($"screen:{screen}");
        //Vector3 pos = Camera.main.ScreenToWorldPoint(screen);
        //var data = guidanceManager.dataList[(int)GuidanceManager.currStep - 1];
        //MyDebug.LogError($"转化前的pos:{pos}");
        //UI_Helper.ShowGuidance_Click(pos, data, () => {
        //    //MyDebug.Log("=====这是点击空白区域的点击回调，我要在这把所有的面板都关掉");
        //    World.Scene.GetComponent<UIManagerComponent>().ClearUIStack();
        //    World.Scene.GetComponent<UIManagerComponent>().RemoveAll(UI_PrefabPath.m_sUISystem_Hall);
        //    MapGridMgr.Instance.UnFoucs();
        //});

        Ray ray = Camera.main.ScreenPointToRay(screen);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider != null)
            {
                var data = guidanceManager.dataList[(int)GuidanceManager.currStep - 1];
                //MyDebug.LogError($"射线碰撞到的位置是：{hit.point}");
                //var o = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //o.transform.position = hit.collider.transform.position;
                UI_Helper.ShowGuidance_Click(hit.point, data,()=>{
                    //MyDebug.Log("=====这是点击空白区域的点击回调，我要在这把所有的面板都关掉");
                    World.Scene.GetComponent<UIManagerComponent>().ClearUIStack();
                    World.Scene.GetComponent<UIManagerComponent>().RemoveAll(UI_PrefabPath.m_sUISystem_Hall);
                    MapGridMgr.Instance.UnFoucs();
                });
            }
        }

    }

    /// <summary>
    /// 点击荒地引导
    /// </summary>
    void Click_HuangDi()
    {
        MyDebug.Log("开始点击荒地引导");
        Vector3 tr = MapGridMgr.Instance.GetUserGuideWastedlandTransform();
        //Vector3 transform = Camera.main.WorldToScreenPoint(tr);
        // 先让摄像机聚焦到这
        MyDebug.Log($"荒地的位置:{tr},摄像机");
        CS_Guidance.DataEntry data = guidanceManager.dataList[(int)GuidanceManager.currStep - 1];
        MobileRTSCam.instance.SmoothMoveCamera(moveCameraTime, tr,()=>{
            MobileRTSCam.instance.SetCameraEnable(false);
            UI_Helper.ShowGuidance_Click(tr, data, () =>
            {
                MyDebug.Log("点击荒地的引导完成了");
                MobileRTSCam.instance.SetCameraEnable(true);
                UIEntity entity = World.Scene.GetComponent<UIManagerComponent>().Get(UI_PrefabPath.m_sUIPopUpWindow_Guidance_Click);
                UI_Guidance_Click click = entity.GetComponent<UI_Guidance_Click>();
                click.SetLockImageEnabel(true);
                MyDebug.Log("点击荒地回调，在这里把lockImg打开");
                //World.Scene.GetComponent<UIManagerComponent>().Remove(UI_PrefabPath.m_sUIPopUpWindow_Guidance_Click);
                //World.Scene.GetComponent<UIManagerComponent>().RemoveAll(UI_PrefabPath.m_sUISystem_Hall);
            });
        });

    }

    /// <summary>
    /// 拖动开垦
    /// </summary>
    void Slide_KaiKeng()
    {
        MyDebug.Log("开始拖动开垦的引导");
        Transform tr = guidanceManager.GetTargetTransform("reclaim");
        Vector3 start = UI_Helper.UICamera.WorldToScreenPoint(tr.position);
        Vector3 end = Camera.main.WorldToScreenPoint(MapGridMgr.Instance.GetUserGuideWastedlandTransform());
        UI_Helper.ShowGuidance_Slide(start, end);
        CS_Guidance.DataEntry data = guidanceManager.dataList[(int)GuidanceManager.currStep - 1];
        UI_Helper.ShowGuidance_Click(tr, data);
        TimerUtil.SetTimeOut(0.1f, () =>
        {
        });

        //App.Instance.StartCoroutine(DataManager._instance.SaveJsonServer());
        World.Scene.GetComponent<UIManagerComponent>().Get(UI_PrefabPath.m_sUIPopUpWindow_Guidance_Click).GetComponent<UI_Guidance_Click>().SetMaskAlpha(0f);

        MapGridMgr.Instance.onUserGuideWastedlandRemoved = (() =>
        {
            isKaiHuangSuccess = true;
            guidanceManager.CloseGuidanceSlidePanel(1f);

            // 手动调用存储数据 这一步必须被记录下来
            MyDebug.Log("开荒成功，手动调用存储地图数据");
            App.Instance.StartCoroutine(DataManager._instance.SaveJsonServer());

                 //MyDebug.Log("现在是弹出领取界面");
                // 掉落功能没有 现在暂时直接弹出确认界面
                UIEntity entity = UI_Helper.ShowConfirmPanel(1305, () =>
                 {
                     // 新手引导传递回调消息回去
                    //GuidanceData guidanceinfo = new GuidanceData();
                    //guidanceinfo.entity = World.Scene.GetComponent<UIManagerComponent>().Get(UI_PrefabPath.m_sUISystem_Hall);
                    ////MyDebug.Log("确认收到掉落的物品，可以开始下一步了,当前的entity = " + guidanceinfo.entity.m_kUIPrefab.name);
                     
                    //ObserverHelper<GuidanceData>.SendMessage(MessageMonitorType.GuidanceClickEvent, this, new MessageArgs<GuidanceData>(guidanceinfo));
                    GuidanceManager._Instance.CheckGuidance(World.Scene.GetComponent<UIManagerComponent>().Get(UI_PrefabPath.m_sUISystem_Hall));
                });
                 //GuidanceData info = new GuidanceData();
                 //info.entity = entity;
                 //ObserverHelper<GuidanceData>.SendMessage(MessageMonitorType.GuidanceSlideEvent, this, new MessageArgs<GuidanceData>(info));
            GuidanceManager._Instance.CheckGuidance(entity);
        });
    }

    /// <summary>
    /// 掉落物品 点击确认领取按钮引导
    /// </summary>
    void Click_DiaoLuo()
    {
        // 掉落功能没有 现在暂时直接弹出确认界
        Transform tr = guidanceManager.GetTargetTransform("singleConfirmBtn");
        //if (tr == null) return;
        CS_Guidance.DataEntry data = guidanceManager.dataList[(int)GuidanceManager.currStep - 1];
        //MyDebug.Log($"确认领取掉落的引导开始,tr:{tr.name},isUI:{data._isUI},_step:{data._Step}");
        UI_Helper.ShowGuidance_Click(tr, data, () =>
        {
            //MyDebug.Log("接收了掉落的物品");
            //UIEntity entity = World.Scene.GetComponent<UIManagerComponent>().Get(UI_PrefabPath.m_sUISystem_Hall);
            //GuidanceData info = new GuidanceData();
            //info.entity = entity;
            //ObserverHelper<GuidanceData>.SendMessage(MessageMonitorType.GuidanceClickEvent, this, new MessageArgs<GuidanceData>(info));
        });
        //World.Scene.GetComponent<UIManagerComponent>().Get(UI_PrefabPath.m_sUIPopUpWindow_Guidance_Click).GetComponent<UI_Guidance_Click>().SetMaskAlpha(0.4f);

    }

    /// <summary>
    /// 点击修建按钮引导
    /// </summary>
    void Click_XiuJian()
    {
        //MyDebug.Log($"开始第9步引导");
        Transform tr = guidanceManager.GetTargetTransform("m_kButton_Build");

        CS_Guidance.DataEntry data = guidanceManager.dataList[(int)GuidanceManager.currStep - 1];
        UI_Helper.ShowGuidance_Click(tr, data, () =>
        {
            World.Scene.GetComponent<UIManagerComponent>().Remove(UI_PrefabPath.m_sUIPopUpWindow_Guidance_Click);
        });

       
    }

    /// <summary>
    /// 拖动栅栏到场景的引导
    /// </summary>
    void Slide_ZhaLan()
    {
        //var tr = World.Scene.GetComponent<UIManagerComponent>().Get(UI_PrefabPath.m_sUIPage_Build);
        //tr.RemoveChildren
        // MyDebug.Log($"开始拖动栅栏到场景中");
        var scroll = guidanceManager.GetTargetTransform("Scroll View").GetComponent<ScrollRect>();
        scroll.horizontal = false;
        Transform tr = scroll.content.GetChild(0).Find("Node/ContentNode/available/bg");
        Vector2 screenStart = UI_Helper.UICamera.WorldToScreenPoint(tr.position);
        Vector2 screenEnd = new Vector2(Screen.width / 2, Screen.height);
        CS_Guidance.DataEntry data = guidanceManager.dataList[(int)GuidanceManager.currStep - 1];
        UI_Helper.ShowGuidance_Slide(screenStart, screenEnd);
        UI_Helper.ShowGuidance_Click(tr, data);
        MyDebug.Log($"当前第10步,tr.name={tr.name},isui:{data._isUI},ismask:{data._isMask}");
        World.Scene.GetComponent<UIManagerComponent>().Get(UI_PrefabPath.m_sUIPopUpWindow_Guidance_Click).GetComponent<UI_Guidance_Click>().SetMaskAlpha(0f);
        TimerUtil.SetTimeOut(0.1f, () =>
        {
            
        });
    }

    /// <summary>
    /// 点击开出来的新地将栅栏放置到位置上的引导
    /// </summary>
    void Click_JianZaoPosition()
    {
        // 不关拖到哪里  都必须点击位置到指定的开荒的位置去
        MyDebug.Log("第12步了，点击指定位置，把建筑放上去");
        Vector3 transform = MapGridMgr.Instance.GetUserGuideWastedlandTransform();
        //Vector3 transform = Camera.main.WorldToScreenPoint(MapGridMgr.Instance.GetUserGuideWastedlandTransform());
        CS_Guidance.DataEntry data = guidanceManager.dataList[(int)GuidanceManager.currStep - 1];
        MobileRTSCam.instance.SetCameraEnable(false);
        UI_Helper.ShowGuidance_Click(transform, data, () => {
            MobileRTSCam.instance.SetCameraEnable(false);

        });
        //MyDebug.Log($"MapGridMgr.Instance.CurEditoringTileInfo.gameObject.name:{MapGridMgr.Instance.CurEditoringTileInfo.gameObject.name}");
        //CheckIfSkip();
        TimerUtil.SetTimeOut(0.1f, () => { 
           
        });

       
    }

    public void CheckIfSkip()
    {
        MyDebug.Log("检测是否跳过还是引导点击指定位置");
        GameObject curEditorObj = MapGridMgr.Instance.CurEditoringTileInfo.gameObject;
        MyDebug.Log($"currObj.pos = {curEditorObj.transform.position},目标位置：{MapGridMgr.Instance.GetUserGuideWastedlandTransform()}");
        Vector3 pos = MapGridMgr.Instance.GetUserGuideWastedlandTransform();
        if (curEditorObj.transform.position == new Vector3(pos.x, 1, pos.z))
        {
            MyDebug.Log($"当前拖动的物体就在目标位置上，跳过第11步直接开始12步的引导");
            GuidanceManager.currStep = GuidanceStep.Step12; // 强制跳过12步  
            //GuidanceData info = new GuidanceData();
            //info.entity = World.Scene.GetComponent<UIManagerComponent>().Get(UI_PrefabPath.m_sUIPage_BuildBuy);
            //ObserverHelper<GuidanceData>.SendMessage(MessageMonitorType.GuidanceClickEvent, this, new MessageArgs<GuidanceData>(info));
            GuidanceManager._Instance.CheckGuidance(World.Scene.GetComponent<UIManagerComponent>().Get(UI_PrefabPath.m_sUIPage_BuildBuy));
        }
        else
        {
            MyDebug.Log($"当前拖动的物体没有在目标位置上，添加回调开始11步");
            MyDebug.Log("拖动栅栏出来的这步里面了，给下一步的移动添加回调函数");
            MapGridMgr.Instance.onMoveEdit = (go) =>
            {
                //MyDebug.Log("点击移动栅栏到指定位置-----在这里sendmessage--------");
                //GuidanceData info = new GuidanceData();
                //info.entity = World.Scene.GetComponent<UIManagerComponent>().Get(UI_PrefabPath.m_sUIPage_BuildBuy);
                //ObserverHelper<GuidanceData>.SendMessage(MessageMonitorType.GuidanceClickEvent, this, new MessageArgs<GuidanceData>(info));
                GuidanceManager._Instance.CheckGuidance(World.Scene.GetComponent<UIManagerComponent>().Get(UI_PrefabPath.m_sUIPage_BuildBuy));
            };
        }
    }

    /// <summary>
    /// 购买按钮引导，点击后放置成功
    /// </summary>
    void Click_GouMai()
    {
        Transform transform = guidanceManager.GetTargetTransform("goldButton");
        CS_Guidance.DataEntry data = guidanceManager.dataList[(int)GuidanceManager.currStep - 1];
        UI_Helper.ShowGuidance_Click(transform, data,()=>{
            //MyDebug.Log("购买成功，栅栏装载咋地图上，手动调用存储json数据");
            App.Instance.StartCoroutine(DataManager._instance.SaveJsonServer());
        });

    }

    /// <summary>
    /// 引导园区命名确认按钮  关闭购买界面按钮
    /// </summary>
    void Click_GuanbiGouMai()
    {
        MyDebug.Log($"重命名窗口");
        Transform transform = guidanceManager.GetTargetTransform("OK");
        CS_Guidance.DataEntry data = guidanceManager.dataList[(int)GuidanceManager.currStep - 1];
        UI_Helper.ShowGuidance_Click(transform, data);
    }

    /// <summary>
    /// 点击加速按钮
    /// </summary>
    void Click_JiaSu()
    {
        Button btn = SceneLogic._instance.l_Bars[0].bar.m_kBtnSpeedUp;
        CS_Guidance.DataEntry data = guidanceManager.dataList[(int)GuidanceManager.currStep - 1];
        UI_Helper.ShowGuidance_Click(btn.transform, data);
        TimerUtil.SetTimeOut(0.1f, () => { 
           
        });
       
        //btn.onClick.AddListener(() =>
        //{
        //    GuidanceData info = new GuidanceData();
        //    info.entity = World.Scene.GetComponent<UIManagerComponent>().Get(UI_PrefabPath.m_sUIPopUpWindow_SpeedUp);
        //    ObserverHelper<GuidanceData>.SendMessage(MessageMonitorType.GuidanceClickEvent, this, new MessageArgs<GuidanceData>(info));
        //});
    }

    /// <summary>
    /// 点击加速面板的确认加速按钮
    /// </summary>
    void Click_QueRenJiaSu()
    {
        MyDebug.Log("第15步引导");
        Button tr = guidanceManager.GetTargetTransform("confirmBtn").GetComponent<Button>();
        CS_Guidance.DataEntry data = guidanceManager.dataList[(int)GuidanceManager.currStep - 1];
        UI_Helper.ShowGuidance_Click(tr.transform, data);

        //tr.onClick.AddListener(() =>
        //{
        //    if(GuidanceManager.isGuidancing)
        //    {
        //        GuidanceData info = new GuidanceData();
        //        ObserverHelper<GuidanceData>.SendMessage(MessageMonitorType.GuidanceClickEvent, this, new MessageArgs<GuidanceData>(info));
        //    }
        //});

    }

    /// <summary>
    /// 点击栅栏进去引导其它功能
    /// </summary>
    void Click_JinRuZhaLan()
    {
        Vector3 tr = MapGridMgr.Instance.GetUserGuideWastedlandTransform();
        //Vector3 transform = Camera.main.WorldToScreenPoint(MapGridMgr.Instance.GetUserGuideWastedlandTransform());
        if (tr == null)
            return;
        // 先让摄像机聚焦到这
        CS_Guidance.DataEntry data = guidanceManager.dataList[(int)GuidanceManager.currStep - 1];
        MobileRTSCam.instance.SmoothMoveCamera(0.2f, tr,()=>{
            MobileRTSCam.instance.SetCameraEnable(false);
            UI_Helper.ShowGuidance_Click(tr, data,()=>{
                MobileRTSCam.instance.SetCameraEnable(true);
                UIEntity entity = World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPopUpWindow_Guidance_Click);
                UI_Guidance_Click click = entity.GetComponent<UI_Guidance_Click>();
                click.SetLockImageEnabel(true);
            });
        });
    }

    /// <summary>
    /// 点击放置按钮
    /// </summary>
    void Click_FangZhi()
    {
        Button btn = guidanceManager.GetTargetTransform("Placement").GetComponent<Button>();
        CS_Guidance.DataEntry data = guidanceManager.dataList[(int)GuidanceManager.currStep - 1];
        UI_Helper.ShowGuidance_Click(btn.transform, data);
        TimerUtil.SetTimeOut(0.1f, () => { 
        
          
        });

        
    }

    /// <summary>
    /// 点击兔子到修建的园区中去
    /// </summary>
    void Click_DianJiTuZi()
    {

        var scroll = guidanceManager.GetTargetTransform("ScrollWarehouse").GetComponent<ScrollRect>();
        //scroll.vertical = false;
        Transform target = scroll.content.GetChild(0);
        //Vector2 startPos = UI_Helper.UICamera.WorldToScreenPoint(target.position);
        //Vector2 endpos = new Vector2(startPos.x - 100, startPos.y);
        //UI_Helper.ShowGuidance_Slide(startPos, endpos);
        CS_Guidance.DataEntry data = guidanceManager.dataList[(int)GuidanceManager.currStep - 1];
        UI_Helper.ShowGuidance_Click(target, data);
        TimerUtil.SetTimeOut(0.1f, () => { 
        


            //World.Scene.GetComponent<UIManagerComponent>().Get(UI_PrefabPath.m_sUIPopUpWindow_Guidance_Click).GetComponent<UI_Guidance_Click>().SetMaskAlpha(0f);
        });
      

    }

    /// <summary>
    /// 点击植物 随机在园区种植树木
    /// </summary>
    void Click_DianJiShuMu()
    {
        var scroll = guidanceManager.GetTargetTransform("ScrollWarehouse").GetComponent<ScrollRect>();
        //scroll.vertical = false;
        Transform target = scroll.content.GetChild(1);
        CS_Guidance.DataEntry data = guidanceManager.dataList[(int)GuidanceManager.currStep - 1];
        UI_Helper.ShowGuidance_Click(target, data,()=>{
            // 这里要在通知manager一次 才会自动走到引导结束那一步
            //GuidanceData info = new GuidanceData();
            //ObserverHelper<GuidanceData>.SendMessage(MessageMonitorType.GuidanceClickEvent, this, new MessageArgs<GuidanceData>(info));
            TimerUtil.SetTimeOut(1f, () =>
            {
                World.Scene.GetComponent<UIManagerComponent>().RemoveAll(UI_PrefabPath.m_sUISystem_Hall);
                World.Scene.GetComponent<UIManagerComponent>().ClearUIStack();
                MapGridMgr.Instance.UnFoucs();
                UI_Helper.ShowCommonTips(212);
            });

            //TimerUtil.SetTimeOut(int.Parse(DBManager.Instance.m_kDisperse.GetEntryPtr(20009)._Val1), () =>
            //{
            //    //MyDebug.Log("现在引导投资人事件");
            //    World.Scene.GetComponent<UIManagerComponent>().RemoveAll(UI_PrefabPath.m_sUISystem_Hall);
            //    World.Scene.GetComponent<UIManagerComponent>().ClearUIStack();
            //    MapGridMgr.Instance.UnFoucs();
            //    CS_Guidance.DataEntry entryData = new CS_Guidance.DataEntry();
            //    entryData._isUI = true;
            //    entryData._isMask = true;
            //    entryData._isShowFinger = true;
            //    entryData._Shape = 0;

            //    Transform tr = World.Scene.GetComponent<UIManagerComponent>().Get(UI_PrefabPath.m_sUISystem_Hall).GetComponent<UISystem_HallComponent>().m_kButton_Event.transform;
            //    UI_Helper.ShowGuidance_Click(tr, entryData);
            //});
        });

    }

}

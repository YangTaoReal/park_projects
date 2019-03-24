using System;
using System.Collections;
using System.Collections.Generic;

using QTFramework;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

///-----------------------------------------------------------------------------------------
///   Namespace:      BE
///   Class:          MobileRTSCam
///   Description:    classes to manage camera input & control
///   Usage :         
///   Author:         BraveElephant inc.                    
///   Version:        v1.3 (2016-12-06)
///                   - Add Rotation while shift keydown in Editor
///                   v1.2 (2016-11-24)
///                   - Prevent Camerashaking while on touch
///                   - X<Y Rotation Use variable added
///                   - GraphicRaycaster related code removed
///                   v1.1 (2016-09-21)
///                   - border draw gizmo error fix
///                   - prevent camera shaking while touch after drag
///                   v1.0 (2016-02-15)
///-----------------------------------------------------------------------------------------
namespace BE
{

    public interface MobileRTSCamListner
    {
        void OnTouchDown(Ray ray);
        void OnTouchUp(Ray ray);
        void OnTouch(Ray ray);
        void OnDragStart(Ray ray);
        void OnDragEnd(Ray ray);
        void OnDrag(Ray ray);
        void OnLongPress(Ray ray);
        void OnMouseWheel(float fValue);
    }

    public enum PinchType
    {
        None = -1,
        Zoom = 0,
        Rotate = 1,
        Up = 2,
        Max = 3,
    }

    public enum BorderType
    {
        None = -1,
        Rect = 0,
        Circle = 1,
    }

    public class MobileRTSCam : MonoBehaviour
    {

        public static MobileRTSCam instance;

        public MobileRTSCamListner Listner = null;

        private Transform trCamera = null; // transform for zoom
        private Transform trCameraRoot = null; // transform for move(panning)
        [HideInInspector]
        public bool camPanningUse = true;
        public BorderType borderType = BorderType.None;
        public bool BorderUse = true;
        public float XMin = -30.0f; // Camera panning x limit
        public float XMax = 30.0f; // Camera panning x limit
        public float ZMin = -30.0f; // Camera panning z limit
        public float ZMax = 30.0f; // Camera panning z limit
        public float CircleBorderRadius = 30.0f; // Camera panning circle limit
        public float zoomMax = 50.0f;
        public float zoomMin = 20.0f;
        public float zoomCurrent = -34.0f;
        public float zoomSpeed = 2.0f;
        public float zoomFocus = 12.0f;

        public float zoomStart = 34.0f;

        private bool bInTouch = false;
        private Vector3 vCamRootPosOld = Vector3.zero;
        private Vector3 mousePosStart = Vector3.zero;
        private Vector3 mousePosPrev = Vector3.zero;
        private Camera camMain;
        private bool Dragged = false;

        public Plane xzPlane;
        public Ray ray;

        public float CameraFocusTime = 0.5f;

        private Vector3 vPickStart;
        private Vector3 vPickOld;
        private Vector3 vCameraPanDir;
        private bool movingCamera = false;
        [HideInInspector]
        public bool CameraEnableMove = true;
        // Camera Panning Inertia Movement 
        public bool InertiaUse = true;
        private bool InertiaActive = false;
        private Vector3 InertiaSpeed;
        private float InertiaAge = 0.0f;

        private bool InZoom = false;

        // variables for long touch down check
        private bool LongTabCheck = true;
        public float LongTabPeriod = 0.5f;
        private float ClickAfter = 0.0f;

        // disable cam panning for limited time
        private float fCamPanLimit = 0.0f;

        public bool InPinch = false;
        private PinchType pinchType = PinchType.None;
        private Vector3 vPinchDirStart = Vector3.zero;
        private float fPinchDistanceStart = 0.0f;
        private Vector3 vPinchPickCenterStart = Vector3.zero;
        private Vector3 vCamRootRotStart = Vector3.zero;
        private Vector3 vCamRootRot = Vector3.zero;
        private Vector2[] vTouchPosStart = new Vector2[2];
        private float ZoomStart = 0.0f;

        // disable ui while camera is in dragging
        //private GraphicRaycaster  gr;

        public bool UseYRotation = true;
        public bool UseXRotation = true;

        // for editor
#if UNITY_EDITOR
        private Vector3 vEDCamRootRotStart;
        private Vector3 vEDCamRootRot;
        private Vector3 vEDMouseStart;
        private Vector3 vEDMouseMove;
        private bool vEDInRotation = false;
#endif
        public GraphicRaycaster graphicRaycaster;

        public float fDragCheckMin = 0.1f;
        public float fInertiaCheckMin = 0.1f;
        private bool m_CreatEdit = false;
        Vector3 vcStart = Vector3.zero;
        RaycastHit hit;
        void Awake()
        {
            graphicRaycaster = GameObject.Find("UINode").GetComponent<GraphicRaycaster>();
            instance = this;
            trCamera = transform.Find("SceneCamera").transform;
            trCameraRoot = transform;
            //gr = GameObject.Find ("Canvas").GetComponent<GraphicRaycaster>();
            xzPlane = new Plane(new Vector3(0f, 1f, 0f), 0f); // set base plane to xzplane with height zero
            camMain = trCamera.GetComponent<Camera>();
            zoomCurrent = 0;
            zoomCurrent = -zoomStart;
            SetCameraZoom(zoomStart);
        }
        public void CreatEdit()
        {
            m_CreatEdit = true;
        }

        public void SmoothMoveCamera(float _totleTime, Vector3 endPoint, Action moveCallBcak = null)
        {
            StartCoroutine(MoveCa(_totleTime, endPoint, moveCallBcak));
        }

        public void SmoothSelectItem(Vector3 endPoint, Action moveCallBcak = null)
        {
            StartCoroutine(MoveCa(CameraFocusTime, endPoint, moveCallBcak, true));
        }

        private IEnumerator MoveCa(float _totleTime, Vector3 endPoint, Action moveCallBcak, bool isSelect = false)
        {
            vcStart = trCameraRoot.position;
            movingCamera = true;
            float starTime = Time.realtimeSinceStartup;
            while (Time.realtimeSinceStartup - starTime < _totleTime && (vcStart != endPoint))
            {
                SetCameraPosition(Vector3.Lerp(vcStart, endPoint, (Time.realtimeSinceStartup - starTime) / _totleTime));
                yield return new WaitForEndOfFrame();
            }

            if (false)
            {
                starTime = Time.realtimeSinceStartup;
                _oldValue = Mathf.Abs(trCamera.localPosition.z);
                while (Time.realtimeSinceStartup - starTime < _FocusTimeValue)
                {
                    SetCameraZoom(Mathf.Lerp(_oldValue, zoomFocus, (Time.realtimeSinceStartup - starTime) / _FocusTimeValue));
                    yield return new WaitForEndOfFrame();
                }
            }

            SetCameraPosition(endPoint);
            moveCallBcak?.Invoke();
            movingCamera = false;
        }
        private float _FocusTimeValue = 0.5f;
        private float _oldValue;
        public void UnFocusSelectItem()
        {
            return;
            StartCoroutine(UnSelectItem());
        }
        private IEnumerator UnSelectItem()
        {
            SetCameraPosition(new Vector3(trCameraRoot.position.x, vcStart.y, trCameraRoot.position.z));
            movingCamera = true;
            float starTime = Time.realtimeSinceStartup;
            while (Time.realtimeSinceStartup - starTime < _FocusTimeValue)
            {
                SetCameraZoom(Mathf.Lerp(zoomFocus, _oldValue, (Time.realtimeSinceStartup - starTime) / _FocusTimeValue));
                yield return new WaitForEndOfFrame();
            }
            movingCamera = false;
        }

        public void RestEditor()
        {
            MapGridMgr.Instance.EndFreeingWastedland();
            MapGridMgr.Instance.UnFoucs();
            SceneLogic._instance.OnDisSelectModelInfo();
            MapGridMgr.Instance.CancelEdit();

            //MapGridMgr.Instance.HideShowAllBoards(false);

        }

        bool beaginSence = true;
        private bool CanEdit = false;
        public bool isCanGetRayCast = true; // 是否打开摄像机对地皮 房屋的检测
        public bool isSendGUidanceZoomMessage = false; // 是否已近发送了新手引导缩放的消息过去
        public bool isHasZoom = false; // 是否进行了缩放了
        public bool isCanPinch = true; // 是否允许摄像机缩放
        private Vector3 StartP = Vector3.zero;
        List<RaycastResult> raycastResultList = new List<RaycastResult>();

        private float keyDownTim = 0;

        private bool IsPointerOverGameObject()
        {
            PointerEventData eventData = new PointerEventData(UnityEngine.EventSystems.EventSystem.current);
            eventData.position = Input.mousePosition;
            raycastResultList.Clear();
            UnityEngine.EventSystems.EventSystem.current.RaycastAll(eventData, raycastResultList);
            return raycastResultList.Count > 0;
        }

        public void SetCameraEnable(bool isEnable)
        {
            //camPanningUse = isEnable;
            //MyDebug.Log($"当前摄像机是否可移动:{isEnable}");
            CameraEnableMove = isEnable;
            //CanEdit = isEnable;
        }

        private void KeyDownEditor()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 1000f, LayerMask.GetMask("EditorBuild")))
            {
                if (!MapGridMgr.Instance.IsEditoring && hit.collider != null)
                {
                    TileInfo tileInfo = hit.collider.gameObject.GetComponent<TileInfo>();
                    if (tileInfo!=null)
                    {
                        if (MapGridMgr.IsWastedland(tileInfo.baseData.cfg._ID) || (MapGridMgr.IsBuilding(tileInfo.baseData.cfg._ID) && !MapGridMgr.IsBarrier(tileInfo.baseData.cfg._ID)))
                        {
                            MapGridMgr.Instance.Foucs(tileInfo, false);
                        }
                    }
                }
            }
        }

        private void GoToEditor()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 1000f, LayerMask.GetMask("EditorBuild")))
            {
                if (!MapGridMgr.Instance.IsEditoring && hit.collider != null)
                {             
                    TileInfo tileInfo = hit.collider.gameObject.GetComponent<TileInfo>();
                    if (tileInfo == null)
                    {
                        return;
                    }
                    Debug.Log($"是否是荒地{MapGridMgr.IsWastedland(tileInfo.baseData.cfg._ID)}");
                    Debug.Log($"是否已选择{SceneLogic._instance.Selected}");
                    Debug.Log($"是否编辑{MapGridMgr.Instance.IsEditoring}");
                    Debug.Log($"是否已聚焦{ MapGridMgr.Instance.foucsTile}");

                    if (MapGridMgr.IsWastedland(tileInfo.baseData.cfg._ID) || (MapGridMgr.IsBuilding(tileInfo.baseData.cfg._ID) && !MapGridMgr.IsBarrier(tileInfo.baseData.cfg._ID)))
                    {

                        if (tileInfo != null && !MapGridMgr.IsWastedland(tileInfo.baseData.cfg._ID) && !SceneLogic._instance.Selected && MapGridMgr.Instance.foucsTile != null && !MapGridMgr.Instance.IsEditoring)
                        {
                            BaseData data = tileInfo.baseData;
                            if (data == null)
                            {
                                Debug.Log("需要编辑的baseData为空");
                                return;
                            }
                            Building building = data.GetComponent<Building>();
                            if (building == null)
                            {
                                Debug.Log("需要编辑的对象没有找到Building");
                                return;
                            }
                            World.Scene.GetComponent<UIManagerComponent>().Remove(UI_PrefabPath.m_sUIPage_BuildingOperation);
                            MapGridMgr.Instance.ReEdit(tileInfo);
                            World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPage_BuildOperation);
                        }
                        else if (tileInfo != null && !SceneLogic._instance.Selected && MapGridMgr.IsWastedland(tileInfo.baseData.cfg._ID) && MapGridMgr.Instance.foucsTile != null && (!MapGridMgr.Instance.IsEditoringBarrier && !MapGridMgr.Instance.IsEditoringBuilding))
                        {
                            UIEntity uIEntity = World.Scene.GetComponent<UIManagerComponent>().Get(UI_PrefabPath.m_sUIPage_Build);
                            if (uIEntity == null)
                            {
                                MapGridMgr.Instance.SelectWastedland(tileInfo);
                                UIEntity entity = World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPage_Reclaim);
                                //CheckGuidance(entity);
                                GuidanceManager._Instance.CheckGuidance(entity);
                            }
                        }
                    }
                }
            }
        }

        private bool HoldDown = false;
        private Coroutine WaiteEditor;
        IEnumerator waiteEditor()
        {
            KeyDownEditor();
            while (Time.realtimeSinceStartup - keyDownTim<1.5f)
            {
                if (Vector3.Distance(StartP, Input.mousePosition) > 10)
                {
                    MapGridMgr.Instance.UnFoucs();
                    yield break;
                }
                yield return null;
            }
            HoldDown = true;
            beaginSence = false;
            GoToEditor();
            MapGridMgr.Instance.UnFoucs();
        }

        void Update()
        {

            if (movingCamera)
            {
                return;
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (WaiteEditor != null)
                {
                    StopCoroutine(WaiteEditor);
                }
                MapGridMgr.Instance.UnFoucs();
                m_CreatEdit = false;
                beaginSence = true;

                if (HoldDown)
                {
                    CanEdit = false;
                }
                else
                {
                    CanEdit = StartP == Vector3.zero ? true : Vector3.Distance(StartP, Input.mousePosition) < 10;
                }
            }

            if (Input.GetMouseButtonDown(0))
            {   
                if (IsPointerOverGameObject())
                {
                    beaginSence = false;
                    StartP = Vector3.one;
                }
                else
                {
                    StartP = Input.mousePosition;
                    // 新手滑动
                    if (GuidanceManager.isGuidancing && GuidanceManager.needSlide && GuidanceManager.currStep == GuidanceStep.Step1)
                    {
                        GuidanceManager._Instance.CheckGuidance(null, MessageMonitorType.GuidanceSlideEvent);
                    }
                }
                HoldDown = false;
                keyDownTim = Time.realtimeSinceStartup;
                WaiteEditor = StartCoroutine(waiteEditor());
            }

            if (m_CreatEdit)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, 1000f, LayerMask.GetMask("GroundBox")))
                {
                    if (hit.transform != null)
                    {
                        if (hit.collider != null)
                        {
                            MapGridMgr.Instance.TouchDownEditor(hit.point);
                        }
                    }
                }
            }

            if (!beaginSence)
            {
                return;
            }

            bool canSlide = true;

            if (CanEdit)
            {
                if (!isCanGetRayCast)
                    return;
                CanEdit = false;
                StartP = Vector3.zero;

                bool GoBack = false;

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit, 1000f, LayerMask.GetMask("EditorBuildMoveByStep")))
                {
                    if (hit.transform != null)
                    {
                        MapGridMgr.Instance.MoveEditorByStep(hit.transform.name);
                    }
                }
                else if (Physics.Raycast(ray, out hit, 1000f, LayerMask.GetMask("Actor")) && !MapGridMgr.Instance.IsEditoring && !SceneLogic._instance.Selected)
                {
                    if (hit.collider != null)
                    {
                        World.Scene.GetComponent<UIManagerComponent>().RemoveAll(UI_PrefabPath.m_sUISystem_Hall);
                        var actor = hit.collider.gameObject.GetComponent<Actor>();
                        if(actor.baseData.cfg._ID == 50003)
                        {
                            ModelManager._instance.assistant.OpenInfo();
                        }

                    }
                    MapGridMgr.Instance.UnFoucs();
                }
                else if (Physics.Raycast(ray, out hit, 1000f, LayerMask.GetMask("Animal")) && !MapGridMgr.Instance.IsEditoring && !SceneLogic._instance.Selected)
                {
                    if (hit.collider != null)
                    {

                        World.Scene.GetComponent<UIManagerComponent>().RemoveAll(UI_PrefabPath.m_sUISystem_Hall);

                        var animal = hit.collider.gameObject.GetComponent<Animal>();
                        ClickAudio clickAudio = animal.gameObject.GetComponent<ClickAudio>();
                        if (clickAudio != null)
                        {
                            clickAudio.PlayClickAudio();
                        }
                        ObserverHelper<ModelBase>.SendMessage(MessageMonitorType.SelectedModelInfo, this, new MessageArgs<ModelBase>(animal));
                        Debug.LogWarning("Animal =" + hit.collider.name);
                        UIEntity entity = World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPage_AnimalPlantOperation);
                        //CheckGuidance(entity);
                        GuidanceManager._Instance.CheckGuidance(entity);
                    }

                    MapGridMgr.Instance.UnFoucs();
                }
                else if (Physics.Raycast(ray, out hit, 1000f, LayerMask.GetMask("Plant")) && !MapGridMgr.Instance.IsEditoring && !SceneLogic._instance.Selected)
                {
                    if (hit.collider != null)
                    {
                        World.Scene.GetComponent<UIManagerComponent>().RemoveAll(UI_PrefabPath.m_sUISystem_Hall);

                        Debug.LogWarning("Plant =" + hit.collider.name);
                        var plant = hit.collider.gameObject.GetComponent<Plant>();
                        ObserverHelper<ModelBase>.SendMessage(MessageMonitorType.SelectedModelInfo, this, new MessageArgs<ModelBase>(plant));
                        UIEntity entity = World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPage_AnimalPlantOperation);
                        //CheckGuidance(entity);
                        GuidanceManager._Instance.CheckGuidance(entity);

                        ClickAudio clickAudio = plant.gameObject.GetComponent<ClickAudio>();
                        if (clickAudio != null)
                        {
                            clickAudio.PlayClickAudio();
                        }
                    }

                    MapGridMgr.Instance.UnFoucs();
                }
                else if (Physics.Raycast(ray, out hit, 1000f, LayerMask.GetMask("EditorBuild")))
                {

                    if (!MapGridMgr.Instance.IsEditoring && hit.collider != null)
                    {
                        TileInfo tileInfo = hit.collider.gameObject.GetComponent<TileInfo>();
                        ClickAudio clickAudio = tileInfo.baseData.go.GetComponent<ClickAudio>();
                        if (clickAudio != null)
                        {
                            clickAudio.PlayClickAudio();
                        }
                        ObserverHelper<TileInfo>.SendMessage(MessageMonitorType.SelectedTileInfo, this, new MessageArgs<TileInfo>(tileInfo));
                        if (tileInfo != null && !MapGridMgr.IsWastedland(tileInfo.baseData.cfg._ID) && !SceneLogic._instance.Selected && MapGridMgr.Instance.foucsTile == null && !MapGridMgr.Instance.IsEditoring)
                        {
                            World.Scene.GetComponent<UIManagerComponent>().RemoveAll(UI_PrefabPath.m_sUISystem_Hall);

                            MapGridMgr.Instance.EndFreeingWastedland();
                            UIEntity entity = World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPage_BuildingOperation);
                            UIPage_BuildingOperationComponent uIPage_BuildingOperationComponent = entity.GetComponent<UIPage_BuildingOperationComponent>();
                            uIPage_BuildingOperationComponent.SelectItem(tileInfo);
                            MapGridMgr.Instance.Foucs(tileInfo);
                            canSlide = false;
                            //CheckGuidance(entity);
                            GuidanceManager._Instance.CheckGuidance(entity);
                        }
                        else if (tileInfo != null && !SceneLogic._instance.Selected && MapGridMgr.IsWastedland(tileInfo.baseData.cfg._ID) && MapGridMgr.Instance.foucsTile == null && (!MapGridMgr.Instance.IsEditoringBarrier && !MapGridMgr.Instance.IsEditoringBuilding))
                        {
                            UIEntity uIEntity = World.Scene.GetComponent<UIManagerComponent>().Get(UI_PrefabPath.m_sUIPage_Build);
                            if (uIEntity == null)
                            {
                                MapGridMgr.Instance.SelectWastedland(tileInfo);
                                UIEntity entity = World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPage_Reclaim);
                                //CheckGuidance(entity);
                                GuidanceManager._Instance.CheckGuidance(entity);
                            }
                        }
                        else
                        {
                            GoBack = true;
                        }

                    }
                    else if (MapGridMgr.Instance.IsEditoring && hit.collider != null)
                    {
                        if (MapGridMgr.Instance.IsEditoringBarrier)
                        {
                            MapGridMgr.Instance.TouchDownEditor(hit.point);
                        }
                        else if(!MapGridMgr.Instance.IsEditoringBuilding)
                        {
                            TileInfo tileInfo = hit.collider.gameObject.GetComponent<TileInfo>();
                            ClickAudio clickAudio = tileInfo.baseData.go.GetComponent<ClickAudio>();
                            if (clickAudio != null)
                            {
                                clickAudio.PlayClickAudio();
                            }

                            ObserverHelper<TileInfo>.SendMessage(MessageMonitorType.SelectedTileInfo, this, new MessageArgs<TileInfo>(tileInfo));
                            if (tileInfo != null && MapGridMgr.IsWastedland(tileInfo.baseData.cfg._ID) && MapGridMgr.Instance.foucsTile == null)
                            {
                                MapGridMgr.Instance.SelectWastedland(tileInfo);
                            }
                        }


                    }
                }
                else if (Physics.Raycast(ray, out hit, 1000f, LayerMask.GetMask("GroundBox")))
                {
                    if (hit.transform != null)
                    {
                        if (hit.collider != null && (!MapGridMgr.Instance.IsFoucsing && (SceneLogic._instance != null && !SceneLogic._instance.Selected)) && !MapGridMgr.Instance.IsEditoringWastedland)
                        {
                            MapGridMgr.Instance.TouchDownEditor(hit.point);
                        }
                        else
                        {
                            GoBack = true;
                        }
                    }
                    else
                    {
                        GoBack = true;
                    }
                }

                if (GoBack)
                {
                    World.Scene.GetComponent<UIManagerComponent>().RemoveAll(UI_PrefabPath.m_sUISystem_Hall);
                    RestEditor();
                }
            }

            if (!canSlide)
            {
                return;
            }

            if (MapGridMgr.Instance.IsFoucsing || (SceneLogic._instance != null && SceneLogic._instance.Selected))
            {
                return;
            }

            //inertia camera panning
            if (InertiaUse)
            {
                if (InertiaActive && (InertiaSpeed.magnitude > fInertiaCheckMin))
                {
                    SetCameraPosition(trCameraRoot.position - InertiaSpeed);
                    InertiaSpeed = Vector3.Lerp(InertiaSpeed, Vector3.zero, InertiaAge);
                    InertiaAge += Time.smoothDeltaTime;
                }
                else
                {
                    InertiaActive = false;
                }
            }

            if (fCamPanLimit > 0.0f)
                fCamPanLimit -= Time.deltaTime;

            if (Input.touchCount < 2)
            {
                if (InPinch)
                {
                    InPinch = false;
                    bInTouch = false;
                    fCamPanLimit = 0.1f;
                    pinchType = PinchType.None;
                    camPanningUse = true;
                }
            }

            Vector3 vTouch = Input.mousePosition;
            ray = Camera.main.ScreenPointToRay(vTouch);
            float enter;

            //if left MouseButton down
            if (Input.GetMouseButton(0))
            {

                if (UnityEngine.EventSystems.EventSystem.current && UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
                {
                    //Debug.Log("left-click over a GUI element!");
                    return;
                }

#if UNITY_EDITOR
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    //Debug.Log ("left shift key is held down");

                    if (!vEDInRotation)
                    {
                        vEDInRotation = true;
                        vEDCamRootRotStart = trCameraRoot.localRotation.eulerAngles;
                        vEDMouseStart = vTouch;
                        //Debug.Log ("editor rotation start___" + trCameraRoot.localRotation.eulerAngles);
                    }
                    else
                    {
                        if (Vector3.Distance(vTouch, vEDMouseStart) > fDragCheckMin)
                        {
                            //Debug.Log ("change rotation start vTouch:"+vTouch+" vEDMouseStart:"+vEDMouseStart+" vEDCamRootRotStart:"+vEDCamRootRotStart);
                            vEDMouseMove = vTouch - vEDMouseStart;
                            //vEDCamRootRot.x = vEDCamRootRotStart.x - vEDMouseMove.y * 0.1f;
                            vEDCamRootRot.x = vEDCamRootRotStart.x;//不要改变x轴, 计算模型位置会有问题
                            vEDCamRootRot.y = vEDCamRootRotStart.y + vEDMouseMove.x * 0.1f;
                            vEDCamRootRot.z = 0;
                            //vEDCamRootRot.x = Mathf.Clamp(vEDCamRootRot.x, 10.0f, 90.0f);
                            trCameraRoot.localRotation = Quaternion.Euler(vEDCamRootRot);
                            Debug.Log ("change rotation : "+vEDCamRootRot);
                        }
                    }
                }
                else
                {
                    vEDInRotation = false;
                }
#endif

                xzPlane.Raycast(ray, out enter);

                if (!bInTouch)
                {
                    bInTouch = true;
                    //gr.enabled = false;
                    ClickAfter = 0.0f;
                    LongTabCheck = true;
                    Dragged = false;
                    mousePosPrev = mousePosStart = vTouch;

                    if (Listner != null)
                        Listner.OnTouchDown(ray);

                    // Get Picking Position
                    xzPlane.Raycast(ray, out enter);
                    vPickStart = ray.GetPoint(enter) - trCameraRoot.position;
                    vPickOld = vPickStart;
                    vCamRootPosOld = trCameraRoot.position;

                    if (InertiaUse)
                    {
                        InertiaActive = false;
                        InertiaAge = 0.0f;
                        InertiaSpeed = Vector3.zero;
                    }
                    //Debug.Log ("Update buildingSelected:"+((buildingSelected != null) ? buildingSelected.name : "none"));
                }
                else
                {

                    if (Input.touchCount < 2)
                    {

                        //Mouse Button is in pressed & mouse move certain diatance
                        if (Vector3.Distance(vTouch, mousePosStart) > fDragCheckMin)
                        {

                            // set drag flag on
                            if (!Dragged)
                            {
                                Dragged = true;

                                if (Listner != null) Listner.OnDragStart(ray);
                            }

#if UNITY_EDITOR
                            if (!Input.GetKey(KeyCode.LeftShift))
                            {
#endif
                                // prevent camera shaking while touch pressed after drag.
                                if (Vector3.Distance(vTouch, mousePosPrev) > fDragCheckMin)
                                {

                                    if (Listner != null) Listner.OnDrag(ray);

                                    if (camPanningUse)
                                    {
                                        Vector3 vPickNew = ray.GetPoint(enter) - trCameraRoot.position;
                                        if (InertiaUse)
                                        {
                                            InertiaSpeed = 0.3f * InertiaSpeed + 0.7f * (vPickNew - vPickOld);
                                        }
                                        vCameraPanDir = vPickNew - vPickStart;
                                        //Debug.Log ("vCameraPanDir:"+vCameraPanDir);
                                        SetCameraPosition(vCamRootPosOld - vCameraPanDir);
                                        vPickOld = vPickNew;
                                    }
                                }
#if UNITY_EDITOR
                            }
#endif
                        }
                        // Not Move
                        else
                        {

                            if (Dragged)
                            {

                                if (Listner != null) Listner.OnDrag(ray);

                                if (camPanningUse)
                                {
                                    Vector3 vPickNew = ray.GetPoint(enter) - trCameraRoot.position;
                                    if (InertiaUse)
                                    {
                                        InertiaSpeed = 0.3f * InertiaSpeed + 0.7f * (vPickNew - vPickOld);
                                    }
                                    vPickOld = vPickNew;
                                }
                            }
                            else
                            {
                                if (!Dragged)
                                {
                                    ClickAfter += Time.deltaTime;

                                    if (LongTabCheck && (ClickAfter > LongTabPeriod))
                                    {
                                        LongTabCheck = false;
                                        if (Listner != null) Listner.OnLongPress(ray);
                                    }
                                }
                            }
                        }

                        mousePosPrev = vTouch;
                    }
                }
            }
            else
            {

#if UNITY_EDITOR
                if (vEDInRotation)
                {
                    vEDInRotation = false;
                }
#endif

                //Release MouseButton
                if (bInTouch)
                {
                    bInTouch = false;
                    //gr.enabled = true;

                    if (Listner != null) Listner.OnTouchUp(ray);

                    // if in drag state
                    if (Dragged)
                    {

                        if (InertiaUse && (InertiaSpeed.magnitude > fInertiaCheckMin))
                            InertiaActive = true;

                        if (Listner != null) Listner.OnDragEnd(ray);
                    }
                    else
                    {
                        if (Listner != null) Listner.OnTouch(ray);
                    }
                }
            }

            if (UnityEngine.EventSystems.EventSystem.current && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                //  zoom with mouse wheel
                float fInputValue = Input.GetAxis("Mouse ScrollWheel");
                if (Listner != null) Listner.OnMouseWheel(fInputValue);
                if (fInputValue != 0.0f)
                {

                    if (!InZoom)
                    {
                        mousePosStart = vTouch;
                        xzPlane.Raycast(ray, out enter);
                        vPickStart = ray.GetPoint(enter) - trCameraRoot.position;
                        vCamRootPosOld = trCameraRoot.position;
                        InZoom = true;
                    }

                    float zoomDelta = fInputValue * zoomSpeed;
                    SetCameraZoom(zoomCurrent - zoomDelta);
                    if (GuidanceManager.isGuidancing && !isSendGUidanceZoomMessage && GuidanceManager.currStep == GuidanceStep.Step2)
                    {
                        isSendGUidanceZoomMessage = true;
                        //Debug.Log($"发送新手引导缩放界面的消息了");
                        GuidanceManager._Instance.CheckGuidance(null, MessageMonitorType.GuidanceSlideEvent);

                    }

                    UpjustPickPos(vTouch, vPickStart);
                }
                else
                {
                    if (InZoom)
                        InZoom = false;
                }
            }

            // pinch zoom for mobile touch input
            if (Input.touchCount != 2)
                return;
            if (!isCanPinch)
                return;

            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector3 vPinchDir = touchOne.position - touchZero.position;
            float fPinchDistance = vPinchDir.magnitude;
            vPinchDir.Normalize();

            Vector3 vPinchTouchCenter = (touchOne.position - touchZero.position) * 0.5f + touchZero.position;
            ray = Camera.main.ScreenPointToRay(vPinchTouchCenter);
            xzPlane.Raycast(ray, out enter);
            Vector3 vPinchPickCenter = ray.GetPoint(enter) - trCameraRoot.position;

            if (!InPinch)
            {
                vTouchPosStart[0] = touchZero.position;
                vTouchPosStart[1] = touchOne.position;
                ZoomStart = zoomCurrent;
                vCamRootRotStart = trCameraRoot.localRotation.eulerAngles;
                vCamRootRot = vCamRootRotStart;
                vCamRootPosOld = trCameraRoot.position;

                vPinchDirStart = vPinchDir;
                fPinchDistanceStart = fPinchDistance;
                vPinchPickCenterStart = vPinchPickCenter;
                InPinch = true;
                camPanningUse = false;
            }
            else
            {

                Vector2 vTouchZeroDelta = touchZero.position - vTouchPosStart[0];
                Vector2 vTouchOneDelta = touchOne.position - vTouchPosStart[1];
                if ((vTouchZeroDelta.magnitude > 1.0f) && (vTouchOneDelta.magnitude > 1.0f))
                {

                    float angleWithUp = Vector2.Angle(vTouchOneDelta, Vector2.up);
                    float angleBetweenTouches = Vector2.Angle(vTouchZeroDelta, vTouchOneDelta);
                    //Debug.Log ("angleWithUp:"+angleWithUp+"angleBetweenTouches:"+angleBetweenTouches);

                    // check if pinch up
                    if (((angleWithUp < 30.0f) || (150.0f < angleWithUp)) && (angleBetweenTouches < 50.0f))
                    {
                        pinchType = PinchType.Up;

                    }
                    else if ((angleBetweenTouches < 30.0f) || (150.0f < angleBetweenTouches))
                    {
                        pinchType = PinchType.Zoom;

                    }
                    else
                    {
                        pinchType = PinchType.Rotate;
                    }
                }

                if (pinchType == PinchType.Up)
                {
                    if (UseXRotation)
                    {
                        //rotate x
                        float fDelta = touchZero.deltaPosition.y * Time.deltaTime * 10.0f;
                        vCamRootRot.x = Mathf.Clamp(vCamRootRot.x - fDelta, 10.0f, 90.0f);
                        trCameraRoot.localRotation = Quaternion.Euler(vCamRootRot);
                        Debug.Log("change rotation 1");
                    }
                }
                else
                {
                    //zoom
                    float fDelta = fPinchDistance - fPinchDistanceStart;
                    //Debug.Log($"缩放参数 = {ZoomStart - fDelta * zoomSpeed * 0.05f}");
                    SetCameraZoom(ZoomStart - fDelta * zoomSpeed * 0.05f);
                    // 判断新手引导
                    if (GuidanceManager.isGuidancing && !isSendGUidanceZoomMessage && GuidanceManager.currStep == GuidanceStep.Step2)
                    {
                        isHasZoom = true;
                        isSendGUidanceZoomMessage = true;
                        //Debug.Log($"发送新手引导缩放界面的消息了");
                        //GuidanceManager._Instance.CheckGuidance(null, MessageMonitorType.GuidanceSlideEvent);
                    }

                    if (UseYRotation)
                    {
                        // rotate y
                        Vector3 v1 = vPinchDirStart;
                        Vector3 v2 = vPinchDir;
                        float dot = v1.x * v2.x + v1.y * v2.y; //# dot product
                        float det = v1.x * v2.y - v1.y * v2.x; // # determinant
                        float angle = Mathf.Atan2(det, dot); //# atan2(y, x) or atan2(sin, cos)
                        angle *= Mathf.Rad2Deg;

                        vCamRootRot.y = vCamRootRotStart.y + angle;
                        trCameraRoot.localRotation = Quaternion.Euler(vCamRootRot);
                        Debug.Log("change rotation 2");
                    }
                }

                if ((pinchType == PinchType.Zoom) || (pinchType == PinchType.Rotate))
                {
                    UpjustPickPos(vPinchTouchCenter, vPinchPickCenterStart);
                }
            }
        }

        public void UpjustPickPos(Vector3 vTouch, Vector3 vPickStart)
        {
            Ray ray = Camera.main.ScreenPointToRay(vTouch);
            float enter;
            xzPlane.Raycast(ray, out enter);
            Vector3 vPickNew = ray.GetPoint(enter) - trCameraRoot.position;
            vCameraPanDir = vPickNew - vPickStart;
            SetCameraPosition(vCamRootPosOld - vCameraPanDir);
        }

        public void SetCameraPosition(Vector3 vPos)
        {
            if (borderType == BorderType.Rect)
            {
                vPos.x = Mathf.Clamp(vPos.x, XMin, XMax);
                vPos.z = Mathf.Clamp(vPos.z, ZMin, ZMax);
            }
            else if (borderType == BorderType.Circle)
            {
                Vector3 vDir = vPos;
                vDir.y = 0.0f;
                float fLength = vDir.magnitude;
                if (fLength > CircleBorderRadius)
                {
                    vDir.Normalize();
                    vPos = vDir * CircleBorderRadius;
                }
            }
            else { }

            if (CameraEnableMove)
            {
                trCameraRoot.position = vPos;
            }
        }

        public void SetCameraZoom(float value)
        {
            zoomCurrent = Mathf.Clamp(value, zoomMin, zoomMax);
            if (camMain.orthographic)
            {
                camMain.orthographicSize = zoomCurrent;
            }
            else
            {
                trCamera.localPosition = new Vector3(0, 0, -zoomCurrent);
            }
        }

        // Set Zoom value with 0.0 ~ 1.0 ratio of min to max value
        public void SetCameraZoomRatio(float fRatio)
        {
            float fRealValue = (zoomMax - zoomMin) * fRatio + zoomMin;
            SetCameraZoom(fRealValue);
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            if (borderType == BorderType.Rect)
            {
                Gizmos.DrawLine(new Vector3(XMin, 0, ZMin), new Vector3(XMax, 0, ZMin));
                Gizmos.DrawLine(new Vector3(XMin, 0, ZMax), new Vector3(XMax, 0, ZMax));
                Gizmos.DrawLine(new Vector3(XMin, 0, ZMin), new Vector3(XMin, 0, ZMax));
                Gizmos.DrawLine(new Vector3(XMax, 0, ZMin), new Vector3(XMax, 0, ZMax));
            }
            else if (borderType == BorderType.Circle)
            {
                Gizmos.DrawWireSphere(Vector3.zero, CircleBorderRadius);
            }
            else { }
        }
    }

}
using System;
using System.Collections;
using System.Collections.Generic;

using QTFramework;

using UnityEngine;
using UnityEngine.EventSystems;

//荒地牌子
public class WastedBoard
{
    public Guid WastedGuid;
    public BaseData BoardbaseData;
    public int typReclaim; //0是未开垦,1是等待开垦,2是完成开垦
}

public class MapGridMgr : MonoBehaviour
{

    private static MapGridMgr _instance;
    public static MapGridMgr Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject goMapGridMgr = GameObject.Find("MapGridMgr");
                _instance = goMapGridMgr.GetComponent<MapGridMgr>();
            }

            return _instance;
        }
    }

    public enum MapGridType
    {
        Sea,
        Soil,
        Grass,
        Road,
        Barrier,
        Build,
        Wastedland,
        Useable
    }

    // 是否编辑地形模式
    public bool isEditorMode;

    public MapGrid Sea;
    public MapGrid Soil;
    public MapGrid Grass;
    public MapGrid Road;
    public MapGridBarrier Barrier;
    public MapGrid Build;
    public MapGrid Wastedland;
    public GridUseable Useable;

    List<MapGrid> _buildingGrids = new List<MapGrid>();
    public List<MapGrid> buildingGrids
    {
        get
        {
            return _buildingGrids;
        }
    }

    public Camera camera;

    public BuildingBrush buildingBrush;
    public BarrierBrush barrierBrush;
    public GroundBrush grassBrush;
    public RoadBrush roadBrush;
    public RandomBrush wastedlandBrush;

    public GameObject planeRoot;
    public GameObject planeShadow;
    public GameObject arrowRoot;

    List<GameObject> arrowCache = new List<GameObject>();
    List<GameObject> arrowShow = new List<GameObject>();

    List<GameObject> planeShow = new List<GameObject>();
    List<GameObject> planeCache = new List<GameObject>();

    public delegate void MapEvent();
    public delegate void MapProgressEvent(int progressVal); // 0~100

    public delegate void RemoveEvent(List<System.Guid> guids);
    public delegate void RemoveEvent1();
    public delegate void MoveEvent(GameObject gameObject);
    public delegate void BuildEvent(bool isNewBuild, List<System.Guid> guids);
    public delegate void ReBuildEvent(System.Guid guids);

    //地图加载完毕事件通知
    public MapProgressEvent onLoading;
    public MapEvent onLoaded;
    bool isImporting;

    //创建完成
    public BuildEvent onEndEdit;
    public ReBuildEvent onReEdit;
    public RemoveEvent onRemoveEdit;
    public BuildEvent onFreeingWastedland;
    public RemoveEvent1 onUserGuideWastedlandRemoved;
    public MoveEvent onMoveEdit;

    //public int testCfgId;
    //public TileInfo testTile;
    //public AnimalMove[] testAnim;

    Dictionary<MapGridType, int> laodingList = new Dictionary<MapGridType, int>();

    GameBrushBase brushEditoring;
    bool isNewBuild;

    bool isBarrierExpand = true;
    MapData barrierCacheData;

    private void Start()
    {
        isEditorMode = false;

        _buildingGrids.Add(Road);
        _buildingGrids.Add(Barrier);
        _buildingGrids.Add(Build);

        buildingBrush.isGameEditoring = false;
        barrierBrush.isGameEditoring = false;
        grassBrush.isGameEditoring = false;
        roadBrush.isGameEditoring = false;

        GameObject temp;
        for (int i = 0; i < planeRoot.transform.childCount; i++)
        {
            temp = planeRoot.transform.GetChild(i).gameObject;
            temp.SetActive(false);
            planeCache.Add(temp);
        }
        for (int i = 0; i < arrowRoot.transform.childCount; i++)
        {
            temp = arrowRoot.transform.GetChild(i).gameObject;
            temp.SetActive(false);
            arrowCache.Add(temp);
        }
    }

    //private void OnGUI()
    //{
    //    ShowTestButton();
    //}

    //void ShowTestButton()
    //{
    //    GUIStyle style = new GUIStyle();
    //    style.alignment = TextAnchor.LowerRight;
    //    style.fontSize = (int) (Screen.height * 0.06);
    //    style.normal.textColor = new Color(0.0f, 0.0f, 0.5f, 1.0f);

    //    GUI.skin.button.fontSize = (int) (0.035f * Screen.width);
    //    float buttonWidth = 0.2f * Screen.width;
    //    float buttonHeight = 0.15f * Screen.height;
    //    float columnOnePosition = 0.5f * Screen.width;
    //    float columnTwoPosition = 0.75f * Screen.width;

    //    Rect rect1 = new Rect(
    //        columnOnePosition,
    //        0.05f * Screen.height,
    //        buttonWidth,
    //        buttonHeight);

    //    if (GUI.Button(rect1, "Left\nEdit"))
    //    {
    //        MoveEditorByStep("left");
    //    }

    //    Rect rect2 = new Rect(
    //        columnOnePosition,
    //        0.225f * Screen.height,
    //        buttonWidth,
    //        buttonHeight);
    //    if (GUI.Button(rect2, "Right\nEdit"))
    //    {
    //        MoveEditorByStep("right");
    //    }

    //    Rect rect3 = new Rect(
    //        columnOnePosition,
    //        0.425f * Screen.height,
    //        buttonWidth,
    //        buttonHeight);
    //    if (GUI.Button(rect3, "Front\nEdit"))
    //    {
    //        MoveEditorByStep("front");
    //    }

    //    rect3.x += 200;
    //    if (GUI.Button(rect3, "Back\nEdit"))
    //    {
    //        MoveEditorByStep("back");
    //    }

    //    //Rect rect4 = new Rect(
    //    //    columnOnePosition,
    //    //    0.625f * Screen.height,
    //    //    buttonWidth,
    //    //    buttonHeight);
    //    //if (GUI.Button(rect4, "Cancel\nEdit"))
    //    //{
    //    //    CancelEdit();
    //    //}

    //    //rect4.x += 200;
    //    //if (GUI.Button(rect4, "放大\nEdit"))
    //    //{
    //    //    ExpandEdit();
    //    //}
    //}

    TileInfo _foucsTile;
    public TileInfo foucsTile
    {
        get
        {
            return _foucsTile;
        }
    }

    public bool IsFoucsing
    {
        get
        {
            return foucsTile != null;
        }
    }

    //选中
    public void Foucs(TileInfo tileInfo,bool needmoveCamera= true)
    {
        if (tileInfo == null) return;
        if (IsEditoring) return;
        if (_foucsTile == tileInfo) return;

        if (IsWastedland(tileInfo.prefabId))
        {
            tileInfo.Foucs();
        }
        else if (IsBarrier(tileInfo.prefabId))
        {
            if (_foucsTile != null && _foucsTile.prefabId == tileInfo.prefabId) return;

            var list = Barrier.TilesByGameId(tileInfo.gameId);
            foreach (var item in list)
            {
                item.Value.Foucs();
            }
        }
        else if (IsBuilding(tileInfo.prefabId))
        {
            tileInfo.Foucs();
        }

        UnFoucs(_foucsTile);
        _foucsTile = tileInfo;
        if (needmoveCamera)
        {
            BE.MobileRTSCam.instance.SmoothSelectItem(tileInfo.gameObject.transform.position);
        }

    }

    //取消选中
    public void UnFoucs(TileInfo tileInfo = null)
    {
        if (tileInfo == null)
        {
            if (_foucsTile == null)
            {
                return;
            }
            tileInfo = _foucsTile;
        }

        if (IsWastedland(tileInfo.prefabId))
        {
            tileInfo.UnFoucs();
        }
        else if (IsBarrier(tileInfo.prefabId))
        {
            var list = Barrier.TilesByGameId(tileInfo.gameId);
            foreach (var item in list)
            {
                item.Value.UnFoucs();
            }
        }
        else if (IsBuilding(tileInfo.prefabId))
        {
            tileInfo.UnFoucs();
        }
        _foucsTile = null;

        BE.MobileRTSCam.instance.UnFocusSelectItem();
    }

    // 是否编辑中
    public bool IsEditoring
    {
        get
        {
            return brushEditoring != null || IsWastedlandFreeing;
        }

    }
    // 是否编辑建筑
    public bool IsEditoringBuilding
    {
        get
        {
            return brushEditoring == buildingBrush;
        }
    }
    // 是否编辑园区
    public bool IsEditoringBarrier
    {
        get
        {
            return brushEditoring == barrierBrush;
        }
    }
    // 是否编辑开荒
    public bool IsEditoringWastedland
    {
        get
        {
            return IsWastedlandFreeing;
        }
    }

    //当前编辑对象
    public TileInfo GetEditoringInfo()
    {
        if (!IsEditoring) return null;
        return brushEditoring.lastEditTile;
    }

    //正在编辑的地块
    public TileInfo CurEditoringTileInfo
    {
        get
        {
            if (brushEditoring == null) return null;
            return brushEditoring.m_PaintedTile;
        }
    }

    //新建
    public void NewEdit(int cfgId)
    {
        UnFoucs();
        if (IsEditoring) return;

        if (IsWastedland(cfgId))
        {
            return;
        }

        Ray ray = camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000, LayerMask.GetMask(new string[] { "GroundBox" })))
        {
            if (hit.transform == null)
            {
                return;
            }
        }

        brushEditoring = GetBrush(cfgId);
        if (!IsEditoring)
        {
            return;
        }

        var grid = GetGrid(brushEditoring);
        if (grid == null) return;

        if (brushEditoring == barrierBrush)
        {
            barrierBrush.curGameId = Barrier.GetNextGameId();
            barrierCacheData = Barrier.TilesToData(barrierBrush.curGameId);
            ExpandEdit();
        }

        if (brushEditoring.BeginEditor(grid.grid, grid.gameObject, grid.grid.WorldToCell(hit.point), cfgId))
        {
            isNewBuild = true;

            Useable.ShowGrid();

            if (buildingBrush == brushEditoring)
            {
                CheckPlane();
            }

            if (onMoveEdit != null && brushEditoring.m_PaintedTile != null) onMoveEdit(brushEditoring.m_PaintedTile.gameObject);
        }
        else
        {
            brushEditoring = null;
        }
    }

    //重新编辑
    public void ReEdit(TileInfo tileInfo)
    {
        UnFoucs();
        if (tileInfo == null) return;

        if (IsWastedland(tileInfo.prefabId))
        {
            SelectWastedland(tileInfo);
            return;
        }

        if (IsEditoring) return;

        brushEditoring = GetBrush(tileInfo.prefabId);
        if (!IsEditoring)
        {
            return;
        }

        var grid = GetGrid(brushEditoring);
        if (grid == null) return;

        if (brushEditoring == barrierBrush)
        {
            barrierBrush.curGameId = tileInfo.gameId;
            barrierCacheData = Barrier.TilesToData(barrierBrush.curGameId);
            ExpandEdit();

            barrierBrush.onReEdit = (TileInfo tile) =>
            {
                if (onReEdit != null)
                {
                    onReEdit(tile.guid);
                }
            };
        }

        if (brushEditoring.BeginEditor(grid.grid, grid.gameObject, grid.grid.WorldToCell(tileInfo.transform.position), tileInfo))
        {
            isNewBuild = false;

            Useable.ShowGrid();

            if (buildingBrush == brushEditoring)
            {
                CheckPlane();
            }
        }
        else
        {
            brushEditoring = null;
        }
    }

    public void MoveEditorByStep(string stepDirName)
    {
        var x = Build.grid.cellSize.x;
        var z = Build.grid.cellSize.y;
        
        if(stepDirName == "left")
        {
            TouchDownEditor(brushEditoring.m_PaintedTile.transform.position + new Vector3(-x, 0, 0));
        }
        else if (stepDirName == "right")
        {
            TouchDownEditor(brushEditoring.m_PaintedTile.transform.position + new Vector3(x, 0, 0));
        }
        else if (stepDirName == "front")
        {
            TouchDownEditor(brushEditoring.m_PaintedTile.transform.position + new Vector3(0, 0, z));
        }
        else if (stepDirName == "back")
        {
            TouchDownEditor(brushEditoring.m_PaintedTile.transform.position + new Vector3(0, 0, -z));
        }
    }

    //转向
    public void RotateEdit()
    {
        if (!IsEditoring) return;
        brushEditoring.RotateEdit();
    }

    public bool CanEndEdit()
    {
        if (!IsEditoring) return false;
        return brushEditoring.canSit;
    }

    //结束 编辑
    public bool EndEdit()
    {
        if (!IsEditoring) return false;
        if (!brushEditoring.canSit) return false;

        var grid = GetGrid(brushEditoring);
        if (grid == null) return false;

        if (brushEditoring.EndEditor(grid.grid))
        {
            ClearPlane();

            if (onEndEdit != null)
            {
                if (brushEditoring == barrierBrush)
                {
                    var list = Barrier.TilesByGameId(barrierBrush.curGameId);
                    var param = new List<System.Guid>();
                    foreach (var item in list)
                    {
                        param.Add(item.Value.guid);
                    }
                    onEndEdit(isNewBuild, param);

                    if (!isNewBuild)
                    {
                        List<Vector3> area = new List<Vector3>();
                        foreach (var tile in list)
                        {
                            area.Add(tile.Value.transform.position);
                        }
                        //foreach (var anim in testAnim)
                        //{
                        //    if (anim != null) anim.SetArea(area);
                        //}
                    }
                }
                else
                {
                    if (brushEditoring.lastEditTile != null)
                    {
                        var param = new List<System.Guid>();
                        param.Add(brushEditoring.lastEditTile.guid);
                        onEndEdit(isNewBuild, param);
                    }
                }
            }

            brushEditoring = null;
            barrierCacheData = null;

            Useable.HideGrid();

            return true;
        }
        else
        {
            return false;
        }

    }

    //取消
    public void CancelEdit()
    {
        if (!IsEditoring) return;

        var grid = GetGrid(brushEditoring);
        if (grid == null) return;

        if (brushEditoring == barrierBrush)
        {
            if (isNewBuild)
            {
                brushEditoring.RemoveEdit(grid);
            }
            else
            {
                //var list = Barrier.TilesByGameId(barrierBrush.curGameId);
                //foreach (var item in list)
                //{
                //    item.Value.Recycle();
                //    item.Value.DisableOutline();
                //}
                //Barrier.ClearTiles(barrierBrush.curGameId);

                //if (barrierCacheData != null)
                //{
                //    Barrier.DataToTiles(barrierCacheData);
                //}

                brushEditoring.CancelEdit(grid);
            }
        }
        else
        {
            if (isNewBuild)
            {
                brushEditoring.RemoveEdit(grid);
            }
            else
            {
                brushEditoring.CancelEdit(grid);
            }
        }

        brushEditoring = null;
        barrierCacheData = null;
        ClearPlane();

        Useable.HideGrid();
    }

    //删除
    public void RemoveEdit(TileInfo tileInfo = null)
    {
        var param = new List<System.Guid>();

        if (tileInfo != null)
        {
            brushEditoring = GetBrush(tileInfo.prefabId);
            if (brushEditoring == null)
            {
                return;
            }

            if (brushEditoring == barrierBrush)
            {
                barrierBrush.curGameId = tileInfo.gameId;
                var list = Barrier.TilesByGameId(barrierBrush.curGameId);
                foreach (var item in list)
                {
                    param.Add(item.Value.guid);
                }
            }
            else
            {
                var temp = tileInfo.guid;
                param.Add(temp);
            }
        }
        else
        {
            if (brushEditoring == barrierBrush)
            {
                var list = Barrier.TilesByGameId(barrierBrush.curGameId);
                foreach (var item in list)
                {
                    param.Add(item.Value.guid);
                }
            }
            else
            {
                var temp = brushEditoring.m_PaintedTile.guid;
                param.Add(temp);
            }
        }

        var grid = GetGrid(brushEditoring);
        if (grid == null) return;

        if (onRemoveEdit != null) onRemoveEdit(param);
        brushEditoring.RemoveEdit(grid);

        brushEditoring = null;
        barrierCacheData = null;
        ClearPlane();

        Useable.HideGrid();

    }

    //扩张
    public void ExpandEdit()
    {
        if (brushEditoring != barrierBrush)
        {
            return;
        }

        isBarrierExpand = true;
        ShowBarrierExpandPlane();
    }

    //缩小
    public void ReduceEdit()
    {
        if (brushEditoring != barrierBrush)
        {
            return;
        }

        isBarrierExpand = false;
        ShowBarrierReducePlane();
    }

    public int ExpandCount
    {
        get
        {
            var list = barrierBrush.ExpandList;
            if (list != null)
            {
                return list.Count;
            }
            else
            {
                return 0;
            }
        }
    }

    //点击事件通知
    bool isTouchDown;
    Vector3 touchDownPos;
    public void TouchDownEditor(Vector3 worldPos)
    {
        isTouchDown = true;
        touchDownPos = worldPos;
        UpdateTouchDown();
        isTouchDown = false;
    }

    //替换升级
    public void ReplaceBuilding(GameObject old, GameObject newGo)
    {
        if (old == null || newGo == null) return;

        var tileInfoOld = old.GetComponent<TileInfo>();
        var tileInfoNew = newGo.GetComponent<TileInfo>();
        if (tileInfoOld == null || tileInfoNew == null) return;

        brushEditoring = GetBrush(tileInfoOld.prefabId);
        if (brushEditoring == null) return;

        var grid = GetGrid(brushEditoring);
        if (grid == null)
        {
            brushEditoring = null;
            return;
        }

        grid.ReplaceTile(tileInfoOld, tileInfoNew);

        brushEditoring = null;
    }

#region 开荒

    bool IsWastedlandFreeing;
    bool IsWastedlandFreeingOneByOne;
    public Dictionary<TileInfo, Vector3Int> _selectedWastedlandTile = new Dictionary<TileInfo, Vector3Int>();

    static Vector3Int UserGuideWastedlandPos = new Vector3Int(-11, -13, 0);

    //获取新手引导指定地块
    public Vector3 GetUserGuideWastedlandTransform()
    {
        var local = Wastedland.grid.CellToLocalInterpolated(UserGuideWastedlandPos + new Vector3(.5f, .5f, .5f));
        return Wastedland.grid.LocalToWorld(local);
    }

    public GameObject GetUserGuideWastedlandObject()
    {
        if (Wastedland.HasTile(UserGuideWastedlandPos))
        {
            return Wastedland.GetTile(UserGuideWastedlandPos).gameObject;
        }
        else if (Barrier.HasTile(UserGuideWastedlandPos))
        {
            return Barrier.GetTile(UserGuideWastedlandPos).gameObject;
        }
        else if (Build.HasTile(UserGuideWastedlandPos))
        {
            return Build.GetTile(UserGuideWastedlandPos).gameObject;
        }
        else
        {
            return null;
        }
    }

    //已选地块
    public Dictionary<TileInfo, Vector3Int> SelectedWastedlandTile
    {
        get
        {
            return _selectedWastedlandTile;
        }
    }

    // 已选数量
    public int SelectWastedlandCount
    {
        get
        {
            return _selectedWastedlandTile.Count;
        }
    }

    //public List<WastedBoard> WastedBoards = new List<WastedBoard>();
    ////public List<WastedBoard> tempWastedBoards = new List<WastedBoard>();

    ////public List<Dictionary<TileInfo, BaseData>> WastedBoards = new List<Dictionary<TileInfo, BaseData>>();
    //public void DestroyBoards(Guid wasted_guid)
    //{
    //    List<WastedBoard> templist = new List<WastedBoard>();
    //    for (int i = 0; i < WastedBoards.Count; i++)
    //    {
    //        var data = WastedBoards[i];
    //        if (data.WastedGuid == wasted_guid && data.typReclaim != 1)
    //            ModelManager._instance.RecycleByGuid(data.BoardbaseData.guid);
    //        else
    //            templist.Add(data);
    //    }

    //    WastedBoards = templist;
    //    for (int i = 0; i < WastedBoards.Count; i++)
    //    {
    //        WastedBoards[i].BoardbaseData.go.transform.Find("Text").GetComponent<TextMesh>().text = (i + 1).ToString();
    //    }

    //}

    //public void HideShowAllBoards(bool isShow)
    //{
    //    if (WastedBoards.Count == 0) return;
    //    Guid guiNowWork;
    //    for (int i = 0; i < SceneLogic._instance.listWasteland.Count; i++)
    //    {
    //        SceneLogic._instance.listWasteland[i].subgrade.go.SetActive(!isShow);
    //        if(i == SceneLogic._instance.listWasteland.Count - 1)
    //        {
    //            BaseData now = SceneLogic._instance.listWasteland[SceneLogic._instance.listWasteland.Count - 1].rootBuild;
    //            if(!now.GetComponent<Building>().IsPause)
    //                guiNowWork = now.guid;
    //        }
             
    //    }
    //    //for (int i = 0; i < WastedBoards.Count; i++)
    //    //{
    //    //    WastedBoards[i].BoardbaseData.go.SetActive(isShow);
    //    //}

   
    //    for (int i = 0; i < WastedBoards.Count; i++)
    //    {
    //        var data = WastedBoards[i];
    //        if(guiNowWork == Guid.Empty)
    //            data.BoardbaseData.go.SetActive(isShow);
    //        else
    //        {
    //            if (guiNowWork != data.WastedGuid) 
    //                data.BoardbaseData.go.SetActive(isShow);
    //        }
    //    }


    //}

    //public BaseData CreateBoard(Guid wastedGuid, Vector3 pos, int typ = 0)
    //{
    //    int nSelected = WastedBoards.Count;
    //    int limit_sele = int.Parse(DBManager.Instance.m_kDisperse.GetEntryPtr(20002)._Val1);
    //    if (nSelected >= limit_sele)
    //    {
    //        UI_Helper.ShowCommonTips(244);
    //        return null;
    //    }

    //    BaseData BoardbaseData = ModelManager._instance.Load(20004);
    //    BoardbaseData.go.transform.position = pos;
    //    bool isHave = false;
    //    for (int i = 0; i < WastedBoards.Count; i++)
    //    {
    //        if (WastedBoards[i].WastedGuid == wastedGuid)
    //            isHave = true;
    //    }
    //    if (!isHave)
    //    {
    //        WastedBoard wastedBoard = new WastedBoard();
    //        wastedBoard.WastedGuid = wastedGuid;
    //        wastedBoard.BoardbaseData = BoardbaseData;
    //        wastedBoard.typReclaim = typ;
    //        WastedBoards.Insert(WastedBoards.Count, wastedBoard);
    //    }
    //    for (int i = 0; i < WastedBoards.Count; i++)
    //    {
    //        WastedBoards[i].BoardbaseData.go.transform.Find("Text").GetComponent<TextMesh>().text = (i + 1).ToString();
    //    }
    //    return BoardbaseData;
    //}

    // 选则/取消
    public void SelectWastedland(TileInfo tile)
    {
        if (tile == null) return;
        if (!IsWastedland(tile.prefabId)) return;

        for (int i = 0; i < SceneLogic._instance.listSelectWBoard.Count; i++)
        {
            var swb = SceneLogic._instance.listSelectWBoard[i];
            if (tile.guid == swb.WastedGuid && swb.typReclaim == 2) return;
        }

        if (_selectedWastedlandTile.ContainsKey(tile))
        {
            _selectedWastedlandTile.Remove(tile);
           
            //if (_selectedWastedlandTile.Count == 0)
            //{
            //    EndFreeingWastedland();
            //}
        }
        else
        {
            //int nWasteland = SceneLogic._instance.listSelectWBoard.Count;
            //int limit_sele = int.Parse(DBManager.Instance.m_kDisperse.GetEntryPtr(20002)._Val1);
            //if (nWasteland >= limit_sele) return;
            var cell = Useable.grid.WorldToCell(tile.gameObject.transform.position);
            if (!Useable.IsLock(cell)) //不可以创建牌子的地方
            {
                return;
            }

          
            SelectWastedland(tile, cell);
   
        }
    }

    // 开始挨个删除
    public void BeginFreeingOneByOne()
    {
        IsWastedlandFreeingOneByOne = true;
    }

    // 结束挨个删除
    public void EndFreeingOneByOne()
    {
        IsWastedlandFreeingOneByOne = false;
    }

    // 删除所有已选
    public void FreeingWastedland()
    {
        if (!IsWastedlandFreeing) return;
       
        //for (int i = 0; i < SceneLogic._instance.listSelectWBoard.Count; i++)
        //{
        //    var data = SceneLogic._instance.listSelectWBoard[i];
        //    foreach (var item in _selectedWastedlandTile)
        //    {
        //        if (data.WastedGuid == item.Key.guid)
        //        {
        //            if (!Useable.IsLock(item.Value))
        //            {
        //                Debug.LogWarning("这个地块不能开荒");
        //                continue;
        //            }
        //            Useable.Freeing(item.Value);
        //            break;
        //        }
        //    }
        //}
        //foreach (var item in _selectedWastedlandTile)
        //{
        //    if (!Useable.IsLock(item.Value))
        //    {
        //        Debug.LogWarning("这个地块不能开荒");
        //        continue;
        //    }

        //    Useable.Freeing(item.Value);
        //}

        EndFreeingWastedland();
        List<System.Guid> list = new List<System.Guid>();
        for (int i = 0; i < SceneLogic._instance.listSelectWBoard.Count; i++)
        {
            var data = SceneLogic._instance.listSelectWBoard[i];
            bool isHave = false;
            for (int j = 0; j < SceneLogic._instance.listWasteland.Count; j++)
            {
                if (SceneLogic._instance.listWasteland[j].rootBuild.guid == data.WastedGuid)
                {
                    isHave = true;
                    break;
                }
            }
            if (!isHave)
                list.Add(data.WastedGuid);
        }

        if (onFreeingWastedland != null) onFreeingWastedland(false, list);
        SceneLogic._instance.RemoveAllSelectWastedLand();
    }

    //结束开荒
    public void EndFreeingWastedland()
    {
        if (!IsWastedlandFreeing) return;
        _selectedWastedlandTile.Clear();
        IsWastedlandFreeing = false;
        IsWastedlandFreeingOneByOne = false;
        Useable.HideGrid();

    }

    // 标记开荒一个地块
    public void BeginFreeingWastedland(GameObject go)
    {
        if (go == null) return;

        Vector3Int pos = Wastedland.grid.WorldToCell(go.transform.position);      
        if (Useable.IsLock(pos))
        {
            Useable.Freeing(pos);
            return;
        }
        else
        {
            Debug.LogWarning("这个地块不能开坑。");
        }
    }

    // 开荒彻底完成 删除荒地
    public void RemoveWastedland(GameObject go)
    {
        if (go == null) return;

        Vector3Int pos = Wastedland.grid.WorldToCell(go.transform.position);
        if (!Useable.IsFreeing(pos))
        {
            if (Useable.IsFree(pos))
            {
                var tile = go.GetComponent<TileInfo>();
                if (tile != null)
                {
                    tile.Recycle();
                }
            }
            else
            {
                Debug.LogWarning("这个地块不是开荒中的，不能删除");
            }

            return;
        }

        wastedlandBrush.Erase(Wastedland.grid, Wastedland.gameObject, pos);
        Useable.Free(pos);

    }

    //bool IsGoonSelect()
    //{
    //    int num = 0;
    //    for (int i = 0; i < WastedBoards.Count; i++)
    //    {
    //        var wb_guid = WastedBoards[i].WastedGuid;
    //        foreach (var sl in _selectedWastedlandTile)
    //        {
    //            if (wb_guid == sl.Key.guid)
    //            {
    //                num++;
    //                break;
    //            }
    //        }
    //    }

    //    int count = _selectedWastedlandTile.Count + WastedBoards.Count - num;
    //    int limit_sele = int.Parse(DBManager.Instance.m_kDisperse.GetEntryPtr(20002)._Val1);
    //    if (count < limit_sele + 1)
    //    {
    //        return true;
    //    }
    //    return false;
    //}

    void SelectWastedland(TileInfo tile, Vector3Int pos)
    {
        if (tile == null) return;

        //if (_selectedWastedlandTile.ContainsKey(tile))
        //{
        //    _selectedWastedlandTile.Remove(tile);
        //}
        //else
        {
            //if (IsGoonSelect())
            //{
                _selectedWastedlandTile.Add(tile, pos);
                Useable.ShowGrid();
                IsWastedlandFreeing = true;
            //}
        }
    }

    void RemoveWastedland(Vector3Int pos)
    {
        if (!IsWastedlandFreeing) return;

        if (!IsWastedlandFreeingOneByOne) return;

        if (!Useable.IsLock(pos))
        {
            Debug.Log("这个地块不能开荒");
            return;
        }

        if ((int)World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.m_kSpade <= 0)
        {
            return;
        }

        TileInfo tile = Wastedland.GetTile(pos);
        if (tile != null)
        {
            if (_selectedWastedlandTile.Count > 0)
            {
                if (!_selectedWastedlandTile.ContainsKey(tile))
                {
                    return;
                }
                else
                {
                    _selectedWastedlandTile.Remove(tile);
                }
            }
        }
        else
        {
            Useable.Free(pos);
            return;
        }

        List<System.Guid> list = new List<System.Guid>();

        //DestroyBoards(tile.guid);

        list.Add(tile.guid);

        Useable.Free(pos);
        Wastedland.ClearTile(tile);

        if (onFreeingWastedland != null) onFreeingWastedland(true, list);
        if (pos == UserGuideWastedlandPos && onUserGuideWastedlandRemoved != null) onUserGuideWastedlandRemoved();
    }

#endregion 开荒

    public void BeginImport()
    {
        isImporting = true;
        laodingList.Clear();
    }

    MapGrid GetGrid(MapGridType mapGridType)
    {
        switch (mapGridType)
        {
        case MapGridType.Sea:
            {
                return Sea;
            }
        case MapGridType.Soil:
            {
                return Soil;
            }
        case MapGridType.Grass:
            {
                return Grass;
            }
        case MapGridType.Road:
            {
                return Road;
            }
        case MapGridType.Barrier:
            {
                return Barrier;
            }
        case MapGridType.Build:
            {
                return Build;
            }
        case MapGridType.Wastedland:
            {
                return Wastedland;
            }

        default:
            return null;
        }
    }

    MapGrid GetGrid(GameBrushBase brush)
    {
        if (brush == grassBrush)
        {
            return Grass;
        }
        else if (brush == roadBrush)
        {
            return Road;
        }
        else if (brush == barrierBrush)
        {
            return Barrier;
        }
        else if (brush == buildingBrush)
        {
            return Build;
        }
        else
        {
            return null;
        }
    }

    public void ImportFromJson(MapGridType mapGridType, string jsonStr)
    {
        if (mapGridType == MapGridType.Useable)
        {
            Useable.DeserializeFromJson(jsonStr);
            return;
        }

        var grid = GetGrid(mapGridType);
        if (grid == null)
        {
            return;
        }

        grid.onLoading = (int val) =>
        {
            OnChildLoading(mapGridType, val);
        };
        grid.onLoaded = () =>
        {
            OnChildLoaded(mapGridType);
        };
        grid.DeserializeFromJson(jsonStr);

        laodingList.Add(mapGridType, 0);
    }

    public void EndImport()
    {
        isImporting = false;

        if (laodingList.Count == 0)
        {
            if (onLoaded != null) onLoaded();
        }
    }

    void OnChildLoading(MapGridType mapGridType, int val)
    {
        if (val >= 100) val = 99;

        if (laodingList.ContainsKey(mapGridType))
        {
            laodingList[mapGridType] = val;
        }

        if (laodingList.Count > 0 && onLoading != null)
        {
            int temp = 0;
            foreach (var item in laodingList)
            {
                temp += item.Value;
            }

            onLoading(temp / laodingList.Count);
        }
    }

    void OnChildLoaded(MapGridType mapGridType)
    {
        //laodingList.Remove(mapGridType);
        //if (laodingList.Count == 0 && !isImporting)
        //{
        //    if (onLoaded != null) onLoaded();
        //}

        if (laodingList.ContainsKey(mapGridType))
        {
            laodingList[mapGridType] = 100;
        }

        bool isFinish = true;
        foreach (var item in laodingList)
        {
            if (item.Value < 100)
            {
                isFinish = false;
                break;
            }
        }

        if (isFinish)
            if (onLoaded != null) onLoaded();
    }

    public string ExportToJson(MapGridType mapGridType)
    {
        if (mapGridType == MapGridType.Useable)
        {
            return Useable.SerializeToJson();
        }

        var grid = GetGrid(mapGridType);
        if (grid == null)
        {
            return null;
        }

        return grid.SerializeToJson();
    }

    public TileInfo GetTileInfo(Vector3 worldPosition)
    {
        TileInfo ret = null;
        foreach (var gridMap in buildingGrids)
        {
            ret = gridMap.GetTile(worldPosition);
            if (ret != null)
            {
                return ret;
            }
        }

        return null;
    }

    GameBrushBase GetBrush(int cfgId)
    {
        if (IsRoad(cfgId))
        {
            return roadBrush;
        }
        else if (IsGrass(cfgId))
        {
            return grassBrush;
        }
        else if (IsBarrier(cfgId))
        {
            return barrierBrush;
        }
        else if (IsBuilding(cfgId))
        {
            return buildingBrush;
        }

        return null;

    }

    public static bool IsMapEditorObj(int cfgId)
    {
        return IsRoad(cfgId) || IsGrass(cfgId) || IsBarrier(cfgId) || IsBuilding(cfgId) || IsWastedland(cfgId);
    }

    public static bool IsRoad(int cfgId)
    {
        return ModelBase.IsObjType(DBManager.Instance.m_kModel.GetEntryPtr(cfgId)._Ctype, ModelCType.Road);
    }

    public static bool IsBarrier(int cfgId)
    {
        return ModelBase.IsObjType(DBManager.Instance.m_kModel.GetEntryPtr(cfgId)._Ctype, ModelCType.Barrier);
    }

    public static bool IsGrass(int cfgId)
    {
        return ModelBase.IsObjType(DBManager.Instance.m_kModel.GetEntryPtr(cfgId)._Ctype, ModelCType.Grass);
    }

    public static bool IsBuilding(int cfgId)
    {
        //ModleType _Ctype = (ModleType)DBManager.Instance.m_kModel.GetEntryPtr(cfgId)._Ctype;
        //if(_Ctype == ModleType.Road || _Ctype == ModleType.Barrier || _Ctype == ModleType.Grass || 
        //   _Ctype == ModleType.WastedSign || _Ctype == ModleType.ParkSign)
        //{
        //    return false;
        //}

        //if(DBManager.Instance.m_kModel.GetEntryPtr(cfgId)._Type == (int)ModeTyp.Building) return true;
        //return false; 

        return ModelManager._instance.IsHaveBuildData(cfgId);

    }

    public static bool IsWastedland(int cfgId)
    {
        return ModelBase.IsObjType(DBManager.Instance.m_kModel.GetEntryPtr(cfgId)._Ctype, ModelCType.Wastedland);
    }

    GameObject GetPlane()
    {
        GameObject ret;
        if (planeCache.Count > 0)
        {
            ret = planeCache[planeCache.Count - 1];
            planeCache.RemoveAt(planeCache.Count - 1);
        }
        else
        {
            ret = Instantiate(planeRoot.transform.GetChild(0).gameObject);
            ret.transform.SetParent(planeRoot.transform);
        }

        return ret;
    }

    GameObject GetArr()
    {
        GameObject ret;
        if (arrowCache.Count > 0)
        {
            ret = arrowCache[arrowCache.Count - 1];
            arrowCache.RemoveAt(arrowCache.Count - 1);
        }
        else
        {
            ret = Instantiate(arrowRoot.transform.GetChild(0).gameObject);
            ret.transform.SetParent(arrowRoot.transform);
        }

        return ret;
    }

    void BackPlane(GameObject _gameObject)
    {
        _gameObject.SetActive(false);
        planeCache.Add(_gameObject);
    }

    void BackArr(GameObject _gameObject)
    {
        _gameObject.SetActive(false);
        arrowCache.Add(_gameObject);
    }

    void ClearPlane()
    {
        foreach (var plane in planeShow)
        {
            BackPlane(plane);
        }
        planeShow.Clear();

        planeRoot.SetActive(false);
        planeShadow.SetActive(false);

        foreach (var arr in arrowShow)
        {
            BackArr(arr);
        }
        arrowShow.Clear();
        arrowRoot.SetActive(false);
    }

    private void UpdateTouchDown()
    {
        if (IsWastedlandFreeing)
        {
            UpdateWastedland();
        }
        else if (brushEditoring == buildingBrush)
        {
            UpdateBuilding();
        }
        else if (brushEditoring == barrierBrush)
        {
            UpdateBarrier();
        }
    }

    void UpdateWastedland()
    {
        if (IsWastedlandFreeingOneByOne && isTouchDown)
        {
            RemoveWastedland(Wastedland.grid.WorldToCell(touchDownPos));
        }
    }

    void UpdateBuilding()
    {
        UpdateMove();
    }

    void UpdateMove()
    {
        if (isTouchDown && brushEditoring.isGameEditoring)
        {
            var gridMap = GetGrid(brushEditoring);
            if (brushEditoring.Move(gridMap.grid, gridMap.grid.WorldToCell(touchDownPos)))
            {
                CheckPlane();
                if (onMoveEdit != null && brushEditoring.m_PaintedTile != null) onMoveEdit(brushEditoring.m_PaintedTile.gameObject);
            }

            return;
        }
    }

    void CheckPlane()
    {
        if (!IsEditoring) return;

        ClearPlane();

        GameBrushBase brush;
        if (brushEditoring == barrierBrush)
        {
            brush = (BarrierBrush) brushEditoring;
        }
        else
        {
            brush = (BuildingBrush) brushEditoring;
        }

        var gridMap = GetGrid(brushEditoring);

        if (brushEditoring.isGameEditoring)
        {
            planeRoot.gameObject.SetActive(true);
            arrowRoot.gameObject.SetActive(true);
            if (brushEditoring.canSit)
            {
                brush.m_PaintedTile.outLineColor = Color.green;

                int size = brush.m_PaintedTile.size;
                if (size < 1) size = 1;

                planeShadow.SetActive(true);
                planeShadow.transform.localScale = Vector3.one * size;
                var posShadow = brush.m_PaintedTile.transform.position;
                posShadow.y = 0.2f;
                planeShadow.transform.position = posShadow;

                var plane = GetPlane();
                planeShow.Add(plane);

                plane.SetActive(true);
                plane.transform.localScale = Vector3.one * size;
                plane.transform.position = brush.m_PaintedTile.transform.position;
                plane.GetComponent<MeshRenderer>().material.SetColor("_XRayColor", new Color(0, 1, 0, 0.5f));

                var arr = GetArr();
                arrowShow.Add(arr);

                arr.SetActive(true);
                arr.transform.position = brush.m_PaintedTile.transform.position;
                SetArrColor(arr, new Color(0, 1, 0, 0.5f));
                SetArrSize(arr, size);
                //ResetArrRotate(arr.transform, true);
                //for (int i = 0; i < arr.transform.childCount; i++)
                //{
                //    arr.transform.GetChild(i).gameObject.SetActive(true);
                //}
            }
            else
            {
                planeShadow.SetActive(false);

                brush.m_PaintedTile.outLineColor = Color.red;

                for (int i = 0; i < brush.EditorPosList.Count; i++)
                {
                    Vector3Int pos = brush.EditorPosList[i];
                    Vector3 p = gridMap.grid.LocalToWorld(gridMap.grid.CellToLocalInterpolated(new Vector3Int(pos.x, pos.y, 0) + new Vector3(.5f, .5f, .5f)));
                    p.y = brush.m_PaintedTile.transform.position.y - 0.1f;
                    var plane = GetPlane();
                    planeShow.Add(plane);
                    plane.SetActive(true);
                    plane.transform.localScale = Vector3.one * 1;
                    plane.transform.position = p;
                    if (gridMap.IsFree(pos) && !gridMap.HasAllTile(pos))
                    {
                        plane.GetComponent<MeshRenderer>().material.SetColor("_XRayColor", new Color(0, 1, 0, 0.5f));
                    }
                    else
                    {
                        plane.GetComponent<MeshRenderer>().material.SetColor("_XRayColor", new Color(1, 0, 0, 0.5f));
                    }
                }

                int size = brush.m_PaintedTile.size;
                if (size < 1) size = 1;

                var arr = GetArr();
                arrowShow.Add(arr);

                arr.SetActive(true);
                arr.transform.position = brush.m_PaintedTile.transform.position;
                SetArrColor(arr, new Color(1, 0, 0, 0.5f));
                SetArrSize(arr, size);
                //ResetArrRotate(arr.transform, true);
                //for (int i = 0; i < arr.transform.childCount; i++)
                //{
                //    arr.transform.GetChild(i).gameObject.SetActive(true);
                //}
            }

        }
    }

    void SetArrColor(GameObject gameObject, Color color)
    {
        foreach (var spRender in gameObject.GetComponentsInChildren<SpriteRenderer>(true))
        {
            spRender.color = color;
        }
    }

    void SetArrSize(GameObject gameObject, int size)
    {
        var pos = size * 1.5f;
        gameObject.transform.GetChild(0).localPosition = new Vector3(0, 0, pos);
        gameObject.transform.GetChild(1).localPosition = new Vector3(pos, 0, 0);
        gameObject.transform.GetChild(2).localPosition = new Vector3(0, 0, -pos);
        gameObject.transform.GetChild(3).localPosition = new Vector3(-pos, 0, 0);
    }

    void UpdateBarrier()
    {

        UpdateBarrierPlane();
    }

    internal void UpdateBarrierPlane()
    {
        if (barrierBrush.canMove)
        {
            UpdateMove();
            return;
        }
        else if (isBarrierExpand)
        {
            if (isTouchDown && brushEditoring.isGameEditoring)
            {
                var gridMap = GetGrid(brushEditoring);
                var pos = gridMap.grid.WorldToCell(touchDownPos);
                var list = GetBarrierEdgeEmpty();
                if (list.ContainsKey(pos))
                {
                    barrierBrush.Expand(gridMap.grid, pos);
                    ShowBarrierExpandPlane();

                    var list1 = Barrier.TilesByGameId(barrierBrush.curGameId);
                    foreach (var item in list1)
                    {
                        item.Value.EnableOutline();
                    }
                }
            }
        }
        else
        {
            if (isTouchDown && brushEditoring.isGameEditoring)
            {
                var list = GetBarrierEdge();
                var gridMap = GetGrid(brushEditoring);
                var pos = gridMap.grid.WorldToCell(touchDownPos);
                if (list.ContainsKey(pos))
                {
                    barrierBrush.Erase(gridMap, list[pos], pos);
                    ShowBarrierReducePlane();

                    var list1 = Barrier.TilesByGameId(barrierBrush.curGameId);
                    foreach (var item in list1)
                    {
                        item.Value.EnableOutline();
                    }
                }
            }
        }
    }

    void ShowBarrierExpandPlane()
    {

        var list = GetBarrierEdgeEmpty();

        ClearPlane();
        planeRoot.gameObject.SetActive(true);
        //arrowRoot.gameObject.SetActive(true);
        foreach (var info in list)
        {
            ShowBarrierPlane(info.Key, info.Value);
        }
    }

    void ShowBarrierReducePlane()
    {
        var list = GetBarrierEdge();

        ClearPlane();
        planeRoot.gameObject.SetActive(true);
        //arrowRoot.gameObject.SetActive(true);
        foreach (var info in list)
        {
            int idx = 0;
            //for (int i = 0; i < info.Value.transform.childCount; i++){
            //    if(info.Value.transform.GetChild(i).gameObject.activeSelf){
            //        idx = i;
            //        break;
            //    }
            //}
            ShowBarrierPlane(info.Key, idx);
        }
    }

    void ShowBarrierPlane(Vector3Int pos, int idx)
    {
        Vector3 p = Barrier.grid.LocalToWorld(Barrier.grid.CellToLocalInterpolated(new Vector3Int(pos.x, pos.y, 0) + new Vector3(.5f, .5f, .5f)));
        p.y = 0.2f;

        var plane = GetPlane();
        planeShow.Add(plane);
        plane.SetActive(true);
        plane.transform.gameObject.SetActive(true);
        plane.transform.localScale = Vector3.one;
        plane.transform.position = p;

        //var arr = GetArr();
        //arrowShow.Add(arr);
        //arr.SetActive(true);
        //arr.transform.localScale = Vector3.one;
        //arr.transform.position = p;
        //for (int i = 0; i < arr.transform.childCount; i++)
        //{
        //    arr.transform.GetChild(i).gameObject.SetActive(i == idx);
        //}

        if (isBarrierExpand)
        {
            plane.GetComponent<MeshRenderer>().material.SetColor("_XRayColor", new Color(0, 1, 0, 0.5f));
            //SetArrColor(arr, new Color(0, 1, 0, 1f));
            //ResetArrRotate(arr.transform, false);
        }
        else
        {
            plane.GetComponent<MeshRenderer>().material.SetColor("_XRayColor", new Color(1, 1, 0, 0.5f));
            //SetArrColor(arr, new Color(1, 0, 0, 1f));
            //ResetArrRotate(arr.transform, false);
        }

    }

    // 获取边缘区域 (可收缩)
    Dictionary<Vector3Int, TileInfo> GetBarrierEdge()
    {
        Dictionary<Vector3Int, TileInfo> ret = new Dictionary<Vector3Int, TileInfo>();

        var list = Barrier.TilesByGameId(barrierBrush.curGameId);

        foreach (var item in list)
        {
            Vector3Int posTop = item.Key + new Vector3Int(0, 1, 0);
            Vector3Int posRight = item.Key + new Vector3Int(1, 0, 0);
            Vector3Int posDown = item.Key + new Vector3Int(0, -1, 0);
            Vector3Int posLeft = item.Key + new Vector3Int(-1, 0, 0);

            TileInfo posTopTile = Barrier.GetTile(posTop);
            TileInfo posRightTile = Barrier.GetTile(posRight);
            TileInfo posDownTile = Barrier.GetTile(posDown);
            TileInfo posLeftTile = Barrier.GetTile(posLeft);

            var posTopTileVal = (posTopTile == null || posTopTile.gameId != barrierBrush.curGameId) ? 1 : 0;
            var posRightTileVal = (posRightTile == null || posRightTile.gameId != barrierBrush.curGameId) ? 2 : 0;
            var posDownTileVal = (posDownTile == null || posDownTile.gameId != barrierBrush.curGameId) ? 4 : 0;
            var posLeftTileVal = (posLeftTile == null || posLeftTile.gameId != barrierBrush.curGameId) ? 8 : 0;

            int test = posTopTileVal + posRightTileVal + posDownTileVal + posLeftTileVal;

            // 3个边
            if (test == 7 || test == 14 || test == 11 || test == 13)
            {
                ret.Add(item.Key, item.Value);
            }
            else if (test == 3 || test == 6 || test == 12 || test == 9) //2个邻边
            {
                if (CheckOppositeCor2(item.Key, test))
                {
                    ret.Add(item.Key, item.Value);
                }
            }
            else if (test == 1 || test == 2 || test == 4 || test == 8) //1个边
            {
                if (CheckOppositeCor1(item.Key, test))
                {
                    ret.Add(item.Key, item.Value);
                }
            }
            else if (test == 15)
            {
                //ret.Add(item.Key, item.Value);
            }
        }

        return ret;
    }

    // true 可拆
    bool CheckOppositeCor1(Vector3Int pos, int test)
    {
        if (test == 1)
        {
            return CheckOppositeCor2(pos, 3) && CheckOppositeCor2(pos, 9);
        }
        else if (test == 2)
        {
            return CheckOppositeCor2(pos, 3) && CheckOppositeCor2(pos, 6);
        }
        else if (test == 4)
        {
            return CheckOppositeCor2(pos, 6) && CheckOppositeCor2(pos, 12);
        }
        else if (test == 8)
        {
            return CheckOppositeCor2(pos, 12) && CheckOppositeCor2(pos, 9);
        }

        return false;
    }

    //true 可以拆
    bool CheckOppositeCor2(Vector3Int pos, int test)
    {
        if (test == 3)
        {
            var tile = Barrier.GetTile(pos + new Vector3Int(-1, -1, 0));
            if ((tile == null || tile.gameId != barrierBrush.curGameId))
            {
                return false;
            }
        }
        else if (test == 6)
        {
            var tile = Barrier.GetTile(pos + new Vector3Int(-1, 1, 0));
            if ((tile == null || tile.gameId != barrierBrush.curGameId))
            {
                return false;
            }
        }
        else if (test == 12)
        {
            var tile = Barrier.GetTile(pos + new Vector3Int(1, 1, 0));
            if ((tile == null || tile.gameId != barrierBrush.curGameId))
            {
                return false;
            }
        }
        else if (test == 9)
        {
            var tile = Barrier.GetTile(pos + new Vector3Int(1, -1, 0));
            if ((tile == null || tile.gameId != barrierBrush.curGameId))
            {
                return false;
            }
        }

        return true;
    }

    // 获取边缘可扩展区域
    Dictionary<Vector3Int, int> GetBarrierEdgeEmpty()
    {
        Dictionary<Vector3Int, int> ret = new Dictionary<Vector3Int, int>();

        var list = Barrier.TilesByGameId(barrierBrush.curGameId);

        foreach (var item in list)
        {
            Vector3Int posTop = item.Key + new Vector3Int(0, 1, 0);
            Vector3Int posRight = item.Key + new Vector3Int(1, 0, 0);
            Vector3Int posDown = item.Key + new Vector3Int(0, -1, 0);
            Vector3Int posLeft = item.Key + new Vector3Int(-1, 0, 0);

            var posTopTile = Barrier.HasAllTile(posTop);
            var posRightTile = Barrier.HasAllTile(posRight);
            var posDownTile = Barrier.HasAllTile(posDown);
            var posLeftTile = Barrier.HasAllTile(posLeft);

            if (Barrier.IsFree(posTop) && !posTopTile && !ret.ContainsKey(posTop))
            {
                ret.Add(posTop, 2);
            }
            if (Barrier.IsFree(posRight) && !posRightTile && !ret.ContainsKey(posRight))
            {
                ret.Add(posRight, 3);
            }
            if (Barrier.IsFree(posDown) && !posDownTile && !ret.ContainsKey(posDown))
            {
                ret.Add(posDown, 0);
            }
            if (Barrier.IsFree(posLeft) && !posLeftTile && !ret.ContainsKey(posLeft))
            {
                ret.Add(posLeft, 1);
            }
        }

        return ret;
    }
}
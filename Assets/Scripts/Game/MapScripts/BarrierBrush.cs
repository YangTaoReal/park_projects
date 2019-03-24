using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "Barrier Brush", menuName = "Brushes/Barrier brush")]
[CustomGridBrush(false, true, false, "Barrier Brush")]
public class BarrierBrush : GameBrushBase
{
    public TileInfo m_curTile;
    public int curGameId;
    public int m_Z;

    public delegate void EditEvent(TileInfo tile);
    public EditEvent onReEdit;

    List<TileInfo> _expandList = new List<TileInfo>();
    public List<TileInfo> ExpandList{
        get{
            if (!isGameEditoring) return null;
            return _expandList;
        }
    }

    Dictionary<TileInfo, Vector3Int> _reduceList = new Dictionary<TileInfo, Vector3Int>();
    public Dictionary<TileInfo, Vector3Int> ReduceList
    {
        get
        {
            if (!isGameEditoring) return null;
            return _reduceList;
        }
    }

    public Vector3Int tempPos;
    public override List<Vector3Int> EditorPosList
    {
        get
        {
            var ret = new List<Vector3Int>();
            ret.Add(tempPos);
            return ret;
        }
    }

    public Dictionary<Vector3Int, TileInfo> editGameObjects = new Dictionary<Vector3Int, TileInfo>();

    public bool canMove;
    int _curCfgId;

    public override bool BeginEditor(GridLayout grid, GameObject brushTarget, Vector3Int position, int cfgId = -1)
    {
        if (isGameEditoring)
        {
            return false;
        }

        _expandList.Clear();
        _reduceList.Clear();
        canMove = true;
        isGameEditoring = true;
        if(PaintTile(grid, brushTarget, position, cfgId)){
            _lastEditTile = m_PaintedTile;
            m_PaintedTile.EnableOutline();
            _curCfgId = cfgId;
            return true;
        }else{
            canMove = false;
            return false;
        }
    }


    public override bool BeginEditor(GridLayout grid, GameObject brushTarget, Vector3Int position, TileInfo tile = null)
    {
        if (isGameEditoring)
        {
            return false;
        }

        _expandList.Clear();
        _reduceList.Clear();
        canMove = false;
        isGameEditoring = true;

        if(tile == null){
            if(PaintTile(grid, brushTarget, position)){
                return true;
            }
            else{
                return false;
            }
        }else{
            _canSit = true;
            _curCfgId = tile.prefabId;
            _lastEditTile = tile;
            tile.EnableOutline();
            return true;
        }

    }

    public override bool EndEditor(GridLayout grid)
    {
        MapGrid mapGrid = grid.GetComponent<MapGrid>();
        if(m_PaintedTile != null){
            if (!_canSit)
            {
                return false;
            }
            m_PaintedTile.transform.position = new Vector3(m_PaintedTile.transform.position.x, 0, m_PaintedTile.transform.position.z);
            mapGrid.SetTile(tempPos, m_PaintedTile);
            m_PaintedTile.DisableOutline();
        }

        var Barrier = (MapGridBarrier)mapGrid;
        var list = Barrier.TilesByGameId(curGameId);
        foreach (var item in list)
        {
            item.Value.DisableOutline();
        }

        foreach (var item in _reduceList)
        {
            item.Key.Recycle();
            item.Key.DisableOutline();
        }

        _lastEditTile = m_PaintedTile;

        _expandList.Clear();
        _reduceList.Clear();
        canMove = false;
        _canSit = false;
        isGameEditoring = false;
        m_PaintedTile = null;
        editGameObjects.Clear();
        onReEdit = null;

        return true;
    }

    public override void RemoveEdit(MapGrid mapGrid)
    {
        Erase(mapGrid, m_PaintedTile);
        
        var Barrier = (MapGridBarrier)mapGrid;
        var list = Barrier.TilesByGameId(curGameId);
        foreach (var item in list)
        {
            item.Value.Recycle();
            item.Value.DisableOutline();
        }
        Barrier.ClearTiles(curGameId);

        foreach (var item in _reduceList)
        {
            item.Key.Recycle();
            item.Key.DisableOutline();
        }

        _expandList.Clear();
        _reduceList.Clear();
        _canSit = false;
        isGameEditoring = false;
        m_PaintedTile = null;
        editGameObjects.Clear();
        onReEdit = null;
    }

    public override void CancelEdit(MapGrid mapGrid) 
    {
        foreach (var item in _expandList)
        {
            mapGrid.ClearTile(item);
            item.Recycle();
        }

        foreach (var item in _reduceList)
        {
            mapGrid.SetTile(item.Value, item.Key);
            item.Key.gameObject.SetActive(true);
        }

        var barrire = (MapGridBarrier)mapGrid;
        var list = barrire.TilesByGameId(curGameId);
        foreach (var item in list)
        {
            item.Value.DisableOutline();
            Vector3Int posTop = item.Key + new Vector3Int(0, 1, 0);
            Vector3Int posRight = item.Key + new Vector3Int(1, 0, 0);
            Vector3Int posDown = item.Key + new Vector3Int(0, -1, 0);
            Vector3Int posLeft = item.Key + new Vector3Int(-1, 0, 0);
            var neighborTop = GetObjectInCell(mapGrid, posTop, curGameId);
            var neighborRight = GetObjectInCell(mapGrid, posRight, curGameId);
            var neighborDown = GetObjectInCell(mapGrid, posDown, curGameId);
            var neighborLeft = GetObjectInCell(mapGrid, posLeft, curGameId);
            if (neighborTop != null)
            {
                item.Value.transform.GetChild(0).gameObject.SetActive(false);
                neighborTop.transform.GetChild(2).gameObject.SetActive(false);
            }
            else
            {
                item.Value.transform.GetChild(0).gameObject.SetActive(true);
            }

            if (neighborRight != null)
            {
                item.Value.transform.GetChild(1).gameObject.SetActive(false);
                neighborRight.transform.GetChild(3).gameObject.SetActive(false);
            }
            else
            {
                item.Value.transform.GetChild(1).gameObject.SetActive(true);
            }

            if (neighborDown != null)
            {
                item.Value.transform.GetChild(2).gameObject.SetActive(false);
                neighborDown.transform.GetChild(0).gameObject.SetActive(false);
            }
            else
            {
                item.Value.transform.GetChild(2).gameObject.SetActive(true);
            }

            if (neighborLeft != null)
            {
                item.Value.transform.GetChild(3).gameObject.SetActive(false);
                neighborLeft.transform.GetChild(1).gameObject.SetActive(false);
            }
            else
            {
                item.Value.transform.GetChild(3).gameObject.SetActive(true);
            }
        }

        _expandList.Clear();
        _reduceList.Clear();
        _canSit = false;
        isGameEditoring = false;
        m_PaintedTile = null;
        editGameObjects.Clear();
        onReEdit = null;
    }

    public override bool Move(GridLayout grid, Vector3Int position)
    {
        if (!isGameEditoring)
        {
            return false;
        }

        if (tempPos == position) return false;

        MapGrid mapGrid = grid.GetComponent<MapGrid>();
        _canSit = true;
        if (mapGrid.IsFree(position))
        {
            if (mapGrid.HasAllTile(position))
            {
                _canSit = false;
            }
        }
        else
        {
            if (mapGrid.IsLock(position))
            {
                Debug.LogError("需要先开荒这片区域");
            }
            else
            {
                Debug.LogError("无效区域");
            }
            _canSit = false;
        }

        tempPos = position;

        m_PaintedTile.transform.position = GetCurTileWorldPos(grid, position) + new Vector3(0, 1, 0);

        return true;
    }

    Vector3 GetCurTileWorldPos(GridLayout grid, Vector3Int pos)
    {
        return grid.LocalToWorld(grid.CellToLocalInterpolated(new Vector3Int(pos.x, pos.y, m_Z) + new Vector3(.5f, .5f, .5f)));
    }

    public bool Expand(GridLayout grid, Vector3Int position)
    {
        if (!isGameEditoring)
        {
            return false;
        }

        if(m_PaintedTile != null)  m_PaintedTile.DisableOutline();

        if(PaintTile(grid, grid.gameObject, position, _curCfgId))
        {
            if(m_PaintedTile != null) m_PaintedTile.EnableOutline();
            return true;
        }else{
            return false;
        }
    }

    public override void Paint(GridLayout grid, GameObject brushTarget, Vector3Int position){
        isGameEditoring = false;
        PaintTile(grid, brushTarget, position);
    }

    bool PaintTile(GridLayout grid, GameObject brushTarget, Vector3Int position, int cfgId = -1)
    {
        if (brushTarget.GetComponent<Grid>() == null)
        {
            return PaintTile(grid, brushTarget, position, cfgId);
        }
        // Do not allow editing palettes
        if (brushTarget.layer == 31)
            return false;

        MapGrid mapGrid = grid.GetComponent<MapGrid>();


        Vector3Int posTop = position + new Vector3Int(0, 1, 0);
        Vector3Int posRight = position + new Vector3Int(1, 0, 0);
        Vector3Int posDown = position + new Vector3Int(0, -1, 0);
        Vector3Int posLeft = position + new Vector3Int(-1, 0, 0);

        var neighborTop = GetObjectInCell(mapGrid, posTop, curGameId);
        var neighborRight = GetObjectInCell(mapGrid, posRight, curGameId);
        var neighborDown = GetObjectInCell(mapGrid, posDown, curGameId);
        var neighborLeft = GetObjectInCell(mapGrid, posLeft, curGameId);

        if(isGameEditoring){
            if(editGameObjects.Count > 0){
                if(neighborTop == null
                   && neighborRight == null
                   && neighborDown == null
                   && neighborLeft == null){
                    return false;
                }
            }
        }

        TileInfo instance = null;

        _canSit = true;

        if(mapGrid.IsFree(position)){
            if (mapGrid.GetOtherTile(position) != null)
            {
                Debug.Log("paint has exist other building");
                _canSit = false;
            }
        }else{
            if (mapGrid.IsLock(position))
            {
                Debug.Log("需要先开荒这片区域");
            }
            else
            {
                Debug.Log("无效区域");
            }
            _canSit = false;
        }

        var curOld = mapGrid.GetTile(position);
        if (curOld != null)
        {
            if(curOld.gameId != curGameId){
                Debug.LogError("paint has exist other barrier");
                if(isGameEditoring){
                    instance = GetTileInfo(cfgId);
                }else{
                    return false;
                }
                _canSit = false;
            }
            else{
                instance = curOld;
            }

        }else{
            instance = GetTileInfo(cfgId);
        }

        if (instance == null)
        {
            _canSit = false;
            isGameEditoring = false;
            return false;
        }

        _expandList.Add(instance);
        if (onReEdit != null) onReEdit(instance);

        instance.transform.SetParent(brushTarget.transform);
        instance.gameObject.SetActive(true);
        instance.gameId = curGameId;

        if (isGameEditoring && canMove)
        {
            AddEditorTile(position, instance);

            m_PaintedTile = instance;
            m_curTile = instance;
            instance.transform.position = GetCurTileWorldPos(grid, position) + new Vector3(0, 1, 0);

            tempPos = position;
        }
        else
        {
            instance.transform.position = GetCurTileWorldPos(grid, position);
            mapGrid.SetTile(position, instance);
        }


        if (neighborTop != null)
        {       
            instance.transform.GetChild(0).gameObject.SetActive(false);
            neighborTop.transform.GetChild(2).gameObject.SetActive(false);
        }
        else
        {
            instance.transform.GetChild(0).gameObject.SetActive(true);
        }

        if (neighborRight != null)
        {
            instance.transform.GetChild(1).gameObject.SetActive(false);
            neighborRight.transform.GetChild(3).gameObject.SetActive(false);
        }
        else
        {
            instance.transform.GetChild(1).gameObject.SetActive(true);
        }

        if (neighborDown != null)
        {
            instance.transform.GetChild(2).gameObject.SetActive(false);
            neighborDown.transform.GetChild(0).gameObject.SetActive(false);
        }
        else
        {
            instance.transform.GetChild(2).gameObject.SetActive(true);
        }

        if (neighborLeft != null)
        {
            instance.transform.GetChild(3).gameObject.SetActive(false);
            neighborLeft.transform.GetChild(1).gameObject.SetActive(false);
        }
        else
        {
            instance.transform.GetChild(3).gameObject.SetActive(true);
        }

        return true;
    }


    public void Erase(MapGrid mapGrid, TileInfo erased, Vector3Int position)
    {
        if (erased != null)
        {
            Vector3Int posTop = position + new Vector3Int(0, 1, 0);
            Vector3Int posRight = position + new Vector3Int(1, 0, 0);
            Vector3Int posDown = position + new Vector3Int(0, -1, 0);
            Vector3Int posLeft = position + new Vector3Int(-1, 0, 0);

            var neighborTop = GetObjectInCell(mapGrid, posTop, erased.gameId);
            var neighborRight = GetObjectInCell(mapGrid, posRight, erased.gameId);
            var neighborDown = GetObjectInCell(mapGrid, posDown, erased.gameId);
            var neighborLeft = GetObjectInCell(mapGrid, posLeft, erased.gameId);

            if (neighborTop != null)
            {
                neighborTop.transform.GetChild(2).gameObject.SetActive(true);
            }
            if (neighborRight != null)
            {
                neighborRight.transform.GetChild(3).gameObject.SetActive(true);
            }
            if (neighborDown != null)
            {
                neighborDown.transform.GetChild(0).gameObject.SetActive(true);
            }
            if (neighborLeft != null)
            {
                neighborLeft.transform.GetChild(1).gameObject.SetActive(true);
            }

            if (isGameEditoring)
            {
                RemoveEditorTile(position);
            }

            mapGrid.ClearTile(erased);

            if(_expandList.Contains(erased)){
                _expandList.Remove(erased);
                BackTileInfo(erased);
            }else{
                _reduceList.Add(erased, position);
                erased.gameObject.SetActive(false);
            }

            if (onReEdit != null) onReEdit(erased);
        }
    }

    public override void Erase(GridLayout grid, GameObject brushTarget, Vector3Int position)
    {
        if (brushTarget.GetComponent<Grid>() == null)
        {
            Erase(grid, brushTarget.transform.parent.gameObject, position);
            return;
        }

        // Do not allow editing palettes
        if (brushTarget.layer == 31)
            return;

        MapGrid mapGrid = grid.GetComponent<MapGrid>();
        
        var erased = GetObjectInCell(mapGrid, new Vector3Int(position.x, position.y, m_Z), curGameId);
        Erase(grid.GetComponent<MapGrid>(), erased, position);
    }

    public override void Erase(MapGrid mapGrid, TileInfo erased)
    {
        if (erased != null)
        {
            mapGrid.ClearTile(erased);

            if (Application.isPlaying)
            {
                _expandList.Remove(erased);
                erased.Recycle();

                if (onReEdit != null) onReEdit(erased);
            }
            else
            {
#if UNITY_EDITOR
                Selection.activeObject = mapGrid.gameObject;
                GameObject.DestroyImmediate(erased.gameObject);
#else
                GameObject.Destroy(erased.gameObject);
#endif
            }

        }
    }

    void AddEditorTile(Vector3Int pos, TileInfo tileInfo){
        if(editGameObjects.ContainsKey(pos)){
            editGameObjects[pos] = tileInfo;
        }else{
            editGameObjects.Add(pos, tileInfo);
        }
    }

    void RemoveEditorTile(Vector3Int pos){
        editGameObjects.Remove(pos);
    }

    TileInfo GetTileInfo(int cfgId = -1){

        int id;
        if (cfgId > 0)
        {
            id = cfgId;
        }
        else
        {
            id = m_curTile.prefabId;
        }

        TileInfo instance = null;
        if (Application.isPlaying)
        {
            instance = MapGrid.RequestTileInfo(id);
            //for (int i = 0; i < instance.transform.childCount;i++){
            //    instance.transform.GetChild(i).gameObject.SetActive(true);
            //}
        }
        else
        {
            instance = Instantiate(m_curTile);
            instance.name = m_curTile.name;
#if UNITY_EDITOR
            Undo.MoveGameObjectToScene(instance.gameObject, instance.gameObject.scene, "Paint Prefabs");
            Undo.RegisterCreatedObjectUndo((Object)instance, "Paint Prefabs");
#endif
        }

        return instance;
    }

    void BackTileInfo(TileInfo tileInfo)
    {
        if (Application.isPlaying)
        {
            tileInfo.Recycle();
        }
        else
        {
#if UNITY_EDITOR
            Selection.activeObject = tileInfo.transform.parent;
            GameObject.DestroyImmediate(tileInfo.gameObject);
#else
            GameObject.Destroy(tileInfo.gameObject);
#endif
        }

    }

    private static TileInfo GetObjectInCell(MapGrid mapGrid, Vector3Int position, int gameId)
    {
        var ret = mapGrid.GetTile(position);
        if(ret != null && ret.gameId == gameId){
            return ret;
        }
        return null;
    }

}

#if UNITY_EDITOR
[CustomEditor(typeof(BarrierBrush))]
public class BarrierBrushEditor : GridBrushEditorBase
{
    private BarrierBrush brush { get { return target as BarrierBrush; } }


    protected void OnEnable()
    {
        brush.isGameEditoring = false;
    }

    public override void OnInspectorGUI()
    {

        brush.curGameId = EditorGUILayout.IntField("curGameId", brush.curGameId);

        brush.m_curTile = (TileInfo)EditorGUILayout.ObjectField("Prefab", brush.m_curTile, typeof(TileInfo), false, null);

    }
}

#endif

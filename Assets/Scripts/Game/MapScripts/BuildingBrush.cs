using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


[CreateAssetMenu(fileName = "Building Brush", menuName = "Brushes/Building brush")]
[CustomGridBrush(false, true, false, "Building Brush")]
public class BuildingBrush : GameBrushBase
{

    public TileInfo m_curTile;
    public int m_Z;
    public Dictionary<Vector3Int, TileInfo> editGameObjects = new Dictionary<Vector3Int, TileInfo>();


    List<Vector3Int> tempPosList;
    public override List<Vector3Int> EditorPosList
    {
        get
        {
            return tempPosList;
        }
    }

    Vector3Int tempPosMove;
    Vector3Int tempPosBegin;

    public override bool BeginEditor(GridLayout grid, GameObject brushTarget, Vector3Int position, int cfgId = -1)
    {
        if (isGameEditoring)
        {
            return false;
        }

        isGameEditoring = true;

        if (PaintTile(grid, brushTarget, position, cfgId))
        {
            m_PaintedTile.EnableOutline();
            _lastEditTile = m_PaintedTile;
            return true;
        }
        else
        {
            return false;
        }
    }

    public override bool BeginEditor(GridLayout grid, GameObject brushTarget, Vector3Int position, TileInfo tile)
    {
        if (isGameEditoring || tile == null)
        {
            return false;
        }

        isGameEditoring = true;
        _canSit = true;
        m_curTile = tile;
        m_PaintedTile = tile;
        _lastEditTile = tile;
        tempPosMove = position;
        tempPosBegin = position;

        m_PaintedTile.transform.position = GetCurTileWorldPos(grid, position) + new Vector3(0, 1, 0);

        MapGrid mapGrid = grid.GetComponent<MapGrid>();
        tempPosList = GetCurTileInfoAllCellsPos(position);
        foreach (var pos in tempPosList)
        {
            mapGrid.ClearTile(pos);
        }

        tile.EnableOutline();

        return true;
    }

    public override bool EndEditor(GridLayout grid)
    {
        if (isGameEditoring)
        {
            if (!_canSit)
            {
                return false;
            }

            MapGrid mapGrid = grid.GetComponent<MapGrid>();
            foreach (var pos in tempPosList)
            {
                mapGrid.SetTile(pos, m_PaintedTile);
            }

            m_PaintedTile.transform.position = new Vector3(m_PaintedTile.transform.position.x, 0, m_PaintedTile.transform.position.z);

            _lastEditTile = m_PaintedTile;

            m_PaintedTile.DisableOutline();

            _canSit = false;
            isGameEditoring = false;
            m_PaintedTile = null;
            tempPosList.Clear();

            return true;

        }
        else
        {
            return false;
        }
    }

    public override void RotateEdit()
    {
        if (!isGameEditoring) return;

        if (m_PaintedTile != null)
        {
            m_PaintedTile.transform.Rotate(new Vector3(0, 90, 0));
        }
    }

    public override bool Move(GridLayout grid, Vector3Int position)
    {
        if (!isGameEditoring)
        {
            return false;
        }

        if(tempPosMove == position) return false;

        MapGrid mapGrid = grid.GetComponent<MapGrid>();

        tempPosList = GetCurTileInfoAllCellsPos(position);

        _canSit = true;
        foreach (var pos in tempPosList)
        {
            if(mapGrid.IsFree(pos)){
                if (mapGrid.HasAllTile(pos))
                {
                    _canSit = false;
                    break;
                }  
            }
            else{
                if (mapGrid.IsLock(pos))
                {
                    Debug.Log("需要先开荒这片区域");
                }
                else
                {
                    Debug.Log("无效区域");
                }
                _canSit = false;
                break;
            }
        }

        m_PaintedTile.transform.position = GetCurTileWorldPos(grid, position) + new Vector3(0, 1, 0);
        tempPosMove = position;

        return true;
    }

    public override void Paint(GridLayout grid, GameObject brushTarget, Vector3Int position)
    {
        isGameEditoring = false;
        PaintTile(grid, brushTarget, position);
    }


    bool PaintTile(GridLayout grid, GameObject brushTarget, Vector3Int position, int cfgId = -1)
    {
        if (brushTarget == null) return false;

        MapGrid mapGrid = grid.GetComponent<MapGrid>();

        if (brushTarget.GetComponent<Grid>() == null)
        {
            return PaintTile(grid, brushTarget.transform.parent.gameObject, position, cfgId);
        }
        // Do not allow editing palettes
        if (brushTarget.layer == 31)
            return false;

        TileInfo instance = null;

        tempPosList = GetCurTileInfoAllCellsPos(position, cfgId);
        _canSit = true;
        foreach (var pos in tempPosList)
        {
            if (mapGrid.IsFree(pos))
            {
                if (mapGrid.HasAllTile(pos))
                {
                    _canSit = false;
                    Debug.Log("has exist building.");
                    break;
                }
            }
            else{
                if (mapGrid.IsLock(pos))
                {
                    Debug.Log("需要先开荒这片区域");
                }
                else{
                    Debug.Log("无效区域");
                }
                _canSit = false;
            }
        }

        if (!_canSit)
        {
            if (!isGameEditoring)
            {
                return false;
            }
        }

        instance = GetTileInfo(cfgId);
        if (instance == null)
        {
            isGameEditoring = false;
            return false;
        }
        instance.transform.SetParent(brushTarget.transform);
        instance.gameObject.SetActive(true);

#if UNITY_EDITOR
        Selection.activeObject = instance.gameObject;
#endif


        if (isGameEditoring)
        {
            m_PaintedTile = instance;
            m_curTile = instance;
            instance.transform.position = GetCurTileWorldPos(grid, position) + new Vector3(0, 1, 0);
            tempPosMove = position;
        }
        else
        {
            instance.transform.position = GetCurTileWorldPos(grid, position);
            foreach (var pos in tempPosList)
            {
                mapGrid.SetTile(pos, instance);
            }
        }

        return true;
    }

    List<Vector3Int> GetCurTileInfoAllCellsPos(Vector3Int pos, int cfgId = -1)
    {

        int size = 1;
        if (cfgId > 0)
        {
            var dataEntry = DBManager.Instance.m_kModel.GetEntryPtr(cfgId);
            if (dataEntry != null)
            {
                size = dataEntry._Volume;
            }
        }
        else
        {
            size = m_curTile.size;
        }


        var ret = new List<Vector3Int>();

        if (size <= 1)
        {
            ret.Add(pos);
        }
        else if (size == 2)
        {
            var posInt1 = pos + new Vector3Int(0, 1, 0);
            var posInt2 = pos + new Vector3Int(1, 1, 0);
            var posInt3 = pos + new Vector3Int(0, 0, 0);
            var posInt4 = pos + new Vector3Int(1, 0, 0);
            ret.Add(posInt1);
            ret.Add(posInt2);
            ret.Add(posInt3);
            ret.Add(posInt4);

        }
        else if (size == 3)
        {
            var posInt1 = pos + new Vector3Int(-1, 1, 0);
            var posInt2 = pos + new Vector3Int(0, 1, 0);
            var posInt3 = pos + new Vector3Int(1, 1, 0);
            var posInt4 = pos + new Vector3Int(-1, 0, 0);
            var posInt5 = pos + new Vector3Int(0, 0, 0);
            var posInt6 = pos + new Vector3Int(1, 0, 0);
            var posInt7 = pos + new Vector3Int(-1, -1, 0);
            var posInt8 = pos + new Vector3Int(0, -1, 0);
            var posInt9 = pos + new Vector3Int(1, -1, 0);

            ret.Add(posInt1);
            ret.Add(posInt2);
            ret.Add(posInt3);
            ret.Add(posInt4);
            ret.Add(posInt5);
            ret.Add(posInt6);
            ret.Add(posInt7);
            ret.Add(posInt8);
            ret.Add(posInt9);
        }

        return ret;
    }

    Vector3 GetCurTileWorldPos(GridLayout grid, Vector3Int pos)
    {
        var tileInfo = m_curTile;

        if (tileInfo.size <= 1 || tileInfo.size == 3)
        {
            return grid.LocalToWorld(grid.CellToLocalInterpolated(new Vector3Int(pos.x, pos.y, m_Z) + new Vector3(.5f, .5f, .5f)));
        }
        else if (tileInfo.size == 2)
        {
            return grid.LocalToWorld(grid.CellToLocalInterpolated(new Vector3Int(pos.x, pos.y, m_Z) + new Vector3(1f, 1f, 1f)));
        }
        else
        {
            return Vector3.zero;
        }
    }


    public override void RemoveEdit(MapGrid mapGrid)
    {
        if (m_PaintedTile != null) Erase(mapGrid, m_PaintedTile);

        _canSit = false;
        isGameEditoring = false;
        m_PaintedTile = null;
        tempPosList.Clear();
    }

    public override void CancelEdit(MapGrid mapGrid)
    {
        if (isGameEditoring)
        {
            if(m_PaintedTile != null){
                tempPosList = GetCurTileInfoAllCellsPos(tempPosBegin, m_PaintedTile.prefabId);
                foreach (var pos in tempPosList)
                {
                    mapGrid.SetTile(pos, m_PaintedTile);
                }
                m_PaintedTile.transform.position = GetCurTileWorldPos(mapGrid.grid, tempPosBegin);
                _lastEditTile = m_PaintedTile;

                m_PaintedTile.DisableOutline();
            }
        }

        _canSit = false;
        isGameEditoring = false;
        m_PaintedTile = null;
        tempPosList.Clear();
    }

    public override void Erase(MapGrid mapGrid, TileInfo erased)
    {
        if (erased != null)
        {
            mapGrid.ClearTile(erased);

            if (Application.isPlaying && !MapGridMgr.Instance.isEditorMode)
            {
                erased.Recycle();
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

        var erased = GetObjectInCell(grid, brushTarget.transform, new Vector3Int(position.x, position.y, m_Z));
        Erase(grid.GetComponent<MapGrid>(), erased);
    }

    private static TileInfo GetObjectInCell(GridLayout grid, Transform parent, Vector3Int position)
    {
        MapGrid mapGrid = grid.GetComponent<MapGrid>();
        return mapGrid.GetTile(position);
    }


    TileInfo GetTileInfo(int cfgId = -1)
    {
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
            Undo.DestroyObjectImmediate(tileInfo.gameObject);
#else
            GameObject.Destroy(tileInfo.gameObject);
#endif
        }
    }

}

#if UNITY_EDITOR
[CustomEditor(typeof(BuildingBrush))]
public class BuildingBrushEditor : GridBrushEditorBase
{
    private BuildingBrush brush { get { return target as BuildingBrush; } }


    protected void OnEnable()
    {
        brush.isGameEditoring = false;
    }

    public override void OnInspectorGUI()
    {
        brush.m_curTile = (TileInfo)EditorGUILayout.ObjectField("m_curTile", brush.m_curTile, typeof(TileInfo), false, null);
    }
}

#endif

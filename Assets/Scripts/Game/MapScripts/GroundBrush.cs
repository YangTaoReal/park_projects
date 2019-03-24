using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


[CreateAssetMenu(fileName = "Ground brush", menuName = "Brushes/Ground brush")]
[CustomGridBrush(false, true, false, "Ground Brush")]
public class GroundBrush : GameBrushBase
{

    public List<TileInfo> editGameObjects = new List<TileInfo>();

    public int m_Z;

    public enum DrawTile
    {
        One = 0,
        CorMin,
        BeginEnd,
        CorMax,
        Chain
    }

    public bool auto;

    public DrawTile m_drawTile;

    [SerializeField]
    public TileInfo[] m_gameObjects;

    public override void Paint(GridLayout grid, GameObject brushTarget, Vector3Int position)
    {
        if (brushTarget == null) return;

        if (brushTarget.GetComponent<Grid>() == null)
        {
            Paint(grid, brushTarget.transform.parent.gameObject, position);
            return;
        }
        // Do not allow editing palettes
        if (brushTarget.layer == 31)
            return;

        UpdateTile(position, grid, brushTarget, true);
    }

    public override void Erase(GridLayout grid, GameObject brushTarget, Vector3Int position)
    {
        if (brushTarget.GetComponent<Grid>() == null)
        {
#if UNITY_EDITOR
            Selection.activeObject = brushTarget.transform.parent.gameObject;
#endif
            Erase(grid, brushTarget.transform.parent.gameObject, position);
            return;
        }

        // Do not allow editing palettes
        if (brushTarget.layer == 31)
            return;

        var erased = GetObjectInCell(grid, brushTarget.transform, new Vector3Int(position.x, position.y, m_Z));
        if (erased != null)
        {
            MapGrid mapGrid = grid.GetComponent<MapGrid>();
            mapGrid.ClearTile(position);

#if UNITY_EDITOR
            Undo.DestroyObjectImmediate(erased.gameObject);
#else
            Destroy(erased.gameObject);
#endif
        }

    }

    //private static Transform GetObjectInCell(GridLayout grid, Transform parent, Vector3Int position)
    //{
    //    int childCount = parent.childCount;
    //    Vector3 min = grid.LocalToWorld(grid.CellToLocalInterpolated(position));
    //    Vector3 max = grid.LocalToWorld(grid.CellToLocalInterpolated(position + Vector3Int.one));
    //    Bounds bounds = new Bounds((max + min) * .5f, max - min);

    //    for (int i = 0; i < childCount; i++)
    //    {
    //        Transform child = parent.GetChild(i);
    //        if (bounds.Contains(child.position))
    //            return child;
    //    }
    //    return null;
    //}

    private static TileInfo GetObjectInCell(GridLayout grid, Transform parent, Vector3Int position)
    {
        MapGrid mapGrid = grid.GetComponent<MapGrid>();
        return mapGrid.GetTile(position);
    }

    private void UpdateTile(Vector3Int location, GridLayout grid, GameObject brushTarget, bool refushNeighbor = false)
    {
        MapGrid mapGrid = grid.GetComponent<MapGrid>();

        Vector3Int posTop = location + new Vector3Int(0, 1, 0);
        Vector3Int posRight = location + new Vector3Int(1, 0, 0);
        Vector3Int posDown = location + new Vector3Int(0, -1, 0);
        Vector3Int posLeft = location + new Vector3Int(-1, 0, 0);
        Vector3Int posTopLeft = posTop + new Vector3Int(-1, 0, 0);
        Vector3Int posTopRight = posTop + new Vector3Int(1, 0, 0);
        Vector3Int posDownLeft = posDown + new Vector3Int(-1, 0, 0);
        Vector3Int posDownRight = posDown + new Vector3Int(1, 0, 0);

        TileInfo neighborTop = null;
        TileInfo neighborRight = null;
        TileInfo neighborDown = null;
        TileInfo neighborLeft = null;
        TileInfo neighborTopLeft = null;
        TileInfo neighborTopRight = null;
        TileInfo neighborDownLeft = null;
        TileInfo neighborDownRight = null;

        TileInfo prefab = null;
        TileInfo instance = null;
        int mask = 0;
        int maskEx = 0;
        if (auto)
        {

            neighborTop = GetObjectInCell(grid, brushTarget.transform, posTop);
            neighborRight = GetObjectInCell(grid, brushTarget.transform, posRight);
            neighborDown = GetObjectInCell(grid, brushTarget.transform, posDown);
            neighborLeft = GetObjectInCell(grid, brushTarget.transform, posLeft);

            neighborTopLeft = GetObjectInCell(grid, brushTarget.transform, posTopLeft);
            neighborTopRight = GetObjectInCell(grid, brushTarget.transform, posTopRight);
            neighborDownLeft = GetObjectInCell(grid, brushTarget.transform, posDownLeft);
            neighborDownRight = GetObjectInCell(grid, brushTarget.transform, posDownRight);

            if (neighborTop != null)
            {
                mask += 1;
            }
            if (neighborRight != null)
            {
                mask += 2;
            }
            if (neighborDown != null)
            {
                mask += 4;
            }
            if (neighborLeft != null)
            {
                mask += 8;
            }

            if (mask == 15)
            {
                if (neighborTopLeft == null)
                {
                    maskEx += 1;
                }
                if (neighborTopRight == null)
                {
                    maskEx += 2;
                }
                if (neighborDownRight == null)
                {
                    maskEx += 4;
                }
                if (neighborDownLeft == null)
                {
                    maskEx += 8;
                }
            }

            int index = GetIndex((byte)mask, (byte)maskEx);

            prefab = m_gameObjects[index];
        }
        else
        {
            prefab = m_gameObjects[(int)m_drawTile];
        }

        AddTile(grid, brushTarget, location, prefab, GetQuaternion((byte)mask, (byte)maskEx));

        if (refushNeighbor)
        {
            if (neighborTop != null) UpdateTile(posTop, grid, brushTarget, false);
            if (neighborRight != null) UpdateTile(posRight, grid, brushTarget, false);
            if (neighborDown != null) UpdateTile(posDown, grid, brushTarget, false);
            if (neighborLeft != null) UpdateTile(posLeft, grid, brushTarget, false);

            if (neighborTopLeft != null) UpdateTile(posTopLeft, grid, brushTarget, false);
            if (neighborTopRight != null) UpdateTile(posTopRight, grid, brushTarget, false);
            if (neighborDownLeft != null) UpdateTile(posDownLeft, grid, brushTarget, false);
            if (neighborDownRight != null) UpdateTile(posDownRight, grid, brushTarget, false);

            //UpdateTile(posTop, grid, brushTarget, false);
            //UpdateTile(posTopRight, grid, brushTarget, false);
            //UpdateTile(posRight, grid, brushTarget, false);
        }
    }

    void AddTile(GridLayout grid, GameObject brushTarget, Vector3Int location, TileInfo prefab, Quaternion quaternion)
    {

        MapGrid mapGrid = grid.GetComponent<MapGrid>();

        TileInfo instance = null;
        var curOld = GetObjectInCell(grid, brushTarget.transform, location);
        if (curOld != null)
        {
            if (curOld.prefabId == prefab.prefabId && curOld.name == prefab.name)
            {
                if (!auto)
                {
                    curOld.transform.Rotate(new Vector3(0, 90, 0));
                    return;
                }
                else
                {
                    instance = curOld;
                }
            }
            else
            {
                editGameObjects.Remove(curOld);


#if UNITY_EDITOR
                Undo.DestroyObjectImmediate(curOld.gameObject);
#else
                GameObject.Destroy(curOld.gameObject);
#endif
            }
        }

        if (instance == null)
        {
            instance = GetTileInfo(prefab);

#if UNITY_EDITOR
            Undo.MoveGameObjectToScene(instance.gameObject, brushTarget.scene, "Paint Prefabs");
            Undo.RegisterCreatedObjectUndo((Object)instance, "Paint Prefabs");
#endif

            editGameObjects.Add(instance);
        }

        instance.transform.SetParent(brushTarget.transform);
        instance.transform.position = grid.LocalToWorld(grid.CellToLocalInterpolated(new Vector3Int(location.x, location.y, m_Z) + new Vector3(.5f, .5f, .5f)));
        instance.transform.rotation = quaternion;
        instance.gameObject.SetActive(true);

        mapGrid.SetTile(location, instance.GetComponent<TileInfo>());

#if UNITY_EDITOR
        if (instance != null)
        {
            Selection.activeObject = instance;
        }
#endif
    }

    TileInfo GetTileInfo(TileInfo prefab)
    {

        TileInfo instance = null;
        if (Application.isPlaying)
        {
            instance = MapGrid.RequestTileInfo(prefab.prefabId);
        }
        else
        {
            instance = Instantiate(prefab);
            instance.name = prefab.name;
#if UNITY_EDITOR
            Undo.MoveGameObjectToScene(instance.gameObject, instance.gameObject.scene, "Paint Prefabs");
            Undo.RegisterCreatedObjectUndo((Object)instance, "Paint Prefabs");
#endif
        }

        instance.size = prefab.size;
        instance.prefabId = prefab.prefabId;

        return instance;
    }


    private int GetIndex(byte mask, byte maskEx)
    {
        switch (mask)
        {
            case 14:
            case 13:
            case 11:
            case 7: return 2;
            case 3:
            case 9:
            case 6:
            case 12: return 1;
            case 0:
                {
                    if(maskEx == 5 || maskEx == 10){
                        return 5;
                    }else{
                        break;
                    }
                }
        }

        if (mask == 15 && maskEx != 0)
        {
            return 3;
        }

        return 0;
    }

    private Quaternion GetQuaternion(byte mask, byte maskEx)
    {
        switch (mask)
        {
            case 14: return Quaternion.Euler(0, 180, 0);
            case 13: return Quaternion.Euler(0, -90, 0);
            case 11: return Quaternion.Euler(0, 0, 0);
            case 7: return Quaternion.Euler(0, 90, 0);
            case 3: return Quaternion.Euler(0, 90, 0);
            case 9: return Quaternion.Euler(0, 0, 0);
            case 6: return Quaternion.Euler(0, 180, 0);
            case 12: return Quaternion.Euler(0, -90, 0);
            case 0:
                {
                    if (maskEx == 5)
                    {
                        return Quaternion.Euler(0, 90, 0);
                    }
                    else
                    {
                        break;
                    }
                }
        }

        if (mask == 15)
        {
            switch (maskEx)
            {
                case 1: return Quaternion.Euler(0, 0, 0);
                case 2: return Quaternion.Euler(0, 90, 0);
                case 4: return Quaternion.Euler(0, 180, 0);
                case 8: return Quaternion.Euler(0, -90, 0);
            }
        }


        return Quaternion.identity;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(GroundBrush))]
public class GroundBrushEditor : Editor
{
    private GroundBrush brush { get { return (target as GroundBrush); } }

    private SerializedProperty m_type;

    public void OnEnable()
    {
        if (brush.m_gameObjects == null || brush.m_gameObjects.Length != 5)
            brush.m_gameObjects = new TileInfo[5];
    }

    public override void OnInspectorGUI()
    {
        brush.auto = EditorGUILayout.Toggle("自动", brush.auto);

        EditorGUILayout.Space();

        if (!brush.auto)
        {
            brush.m_drawTile = (GroundBrush.DrawTile)EditorGUILayout.EnumPopup("瓦片类型", brush.m_drawTile);
        }

        EditorGUILayout.Space();


        EditorGUI.BeginChangeCheck();
        brush.m_gameObjects[0] = (TileInfo)EditorGUILayout.ObjectField("One", brush.m_gameObjects[0], typeof(TileInfo), false, null);
        brush.m_gameObjects[1] = (TileInfo)EditorGUILayout.ObjectField("CorMin", brush.m_gameObjects[1], typeof(TileInfo), false, null);
        brush.m_gameObjects[2] = (TileInfo)EditorGUILayout.ObjectField("BeginEnd", brush.m_gameObjects[2], typeof(TileInfo), false, null);
        brush.m_gameObjects[3] = (TileInfo)EditorGUILayout.ObjectField("CorMax", brush.m_gameObjects[3], typeof(TileInfo), false, null);
        brush.m_gameObjects[4] = (TileInfo)EditorGUILayout.ObjectField("Chain", brush.m_gameObjects[4], typeof(TileInfo), false, null);
        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(brush);
    }

    [MenuItem("GroundBrush/Rotate #t")]
    static void Rotate()
    {
        GameObject[] sels = Selection.gameObjects;
        foreach (var sel in sels)
        {
            if (sel)
            {
                sel.transform.Rotate(new Vector3(0, 90, 0));
            }
        }
    }
}
#endif
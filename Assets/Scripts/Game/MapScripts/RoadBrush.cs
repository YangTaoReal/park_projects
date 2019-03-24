using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


[CreateAssetMenu(fileName = "Road brush", menuName = "Brushes/Road brush")]
[CustomGridBrush(false, true, false, "Road Brush")]
public class RoadBrush : GameBrushBase
{

    public int m_Z;
    public bool m_flag2;
    public bool m_flag3;
    public bool m_flag4;

    [SerializeField]
    public TileInfo[] m_gameObjects;

    public List<TileInfo> editGameObjects = new List<TileInfo>();

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

        int mask = 0;
        Vector3Int posTop = location + new Vector3Int(0, 1, 0);
        Vector3Int posRight = location + new Vector3Int(1, 0, 0);
        Vector3Int posDown = location + new Vector3Int(0, -1, 0);
        Vector3Int posLeft = location + new Vector3Int(-1, 0, 0);

        var neighborTop = GetObjectInCell(grid, brushTarget.transform, posTop);
        var neighborRight = GetObjectInCell(grid, brushTarget.transform, posRight);
        var neighborDown = GetObjectInCell(grid, brushTarget.transform, posDown);
        var neighborLeft = GetObjectInCell(grid, brushTarget.transform, posLeft);

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

        int index = GetIndex((byte)mask);

        if (!m_flag2 && index == 2) index = 1;
        else if (!m_flag3 && index == 3) index = 1;
        else if (!m_flag4 && index == 4) index = 1;

        TileInfo prefab = null;
        TileInfo instance = null;
        if (index >= 0 && index < m_gameObjects.Length)
        {
            prefab = m_gameObjects[index];
        }

        var curOld = GetObjectInCell(grid, brushTarget.transform, location);
        if (curOld != null)
        {
            if (prefab != null && curOld.name == prefab.name && curOld.GetComponent<TileInfo>().prefabId == prefab.GetComponent<TileInfo>().prefabId)
            {
                instance = curOld;
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
            
            instance = Instantiate(prefab);
            instance.name = prefab.name;
#if UNITY_EDITOR
            Undo.MoveGameObjectToScene(instance.gameObject, brushTarget.scene, "Paint Prefabs");
            Undo.RegisterCreatedObjectUndo((Object)instance, "Paint Prefabs");
#endif

            editGameObjects.Add(instance);
        }

        if (instance != null)
        {
            instance.gameObject.SetActive(true);
            instance.transform.SetParent(brushTarget.transform);
            instance.transform.position = grid.LocalToWorld(grid.CellToLocalInterpolated(new Vector3Int(location.x, location.y, m_Z) + new Vector3(.5f, .5f, .5f)));
            instance.transform.rotation = GetQuaternion((byte)mask);

            mapGrid.SetTile(location, instance.GetComponent<TileInfo>());
        }

        if (refushNeighbor)
        {
            if (neighborTop != null) UpdateTile(posTop, grid, brushTarget, false);
            if (neighborRight != null) UpdateTile(posRight, grid, brushTarget, false);
            if (neighborDown != null) UpdateTile(posDown, grid, brushTarget, false);
            if (neighborLeft != null) UpdateTile(posLeft, grid, brushTarget, false);
        }

#if UNITY_EDITOR
        if (instance != null)
        {
            Selection.activeObject = instance;
        }
#endif

    }

    private int GetIndex(byte mask)
    {
        switch (mask)
        {
            case 0: return 0;
            case 3:
            case 6:
            case 9:
            case 12: return 2;
            case 1:
            case 2:
            case 4:
            case 8: return 5;
            case 5:
            case 10: return 1;
            case 7:
            case 11:
            case 13:
            case 14: return 3;
            case 15: return 4;
        }
        return -1;
    }

    private Quaternion GetQuaternion(byte mask)
    {
        switch (mask)
        {
            case 4:
            case 5:
                return Quaternion.Euler(0f, 90f, 0f);
            case 1:
            case 7:
            case 9:
                return Quaternion.Euler(0f, -90f, 0f);
            case 10:
            case 2:
            case 3:
                return Quaternion.Euler(0f, 0f, 0f);
            case 8:
            case 11:
            case 12:
                return Quaternion.Euler(0f, 180f, 0f);
            case 6:
            case 13:
                return Quaternion.Euler(0f, -270f, 0f);
        }
        return Quaternion.identity;
    }

    private void OnEnable()
    {
        editGameObjects.Clear();
    }


}

#if UNITY_EDITOR
[CustomEditor(typeof(RoadBrush))]
public class RoadBrushEditor : Editor
{
    private RoadBrush brush { get { return (target as RoadBrush); } }

    public void OnEnable()
    {
        if (brush.m_gameObjects == null || brush.m_gameObjects.Length != 6)
            brush.m_gameObjects = new TileInfo[6];
    }

    public override void OnInspectorGUI()
    {

        brush.m_flag2 = EditorGUILayout.Toggle("自动拐角", brush.m_flag2);
        brush.m_flag3 = EditorGUILayout.Toggle("自动丁字", brush.m_flag3);
        brush.m_flag4 = EditorGUILayout.Toggle("自动十字", brush.m_flag4);

        EditorGUILayout.Space();

        EditorGUI.BeginChangeCheck();
        brush.m_gameObjects[0] = (TileInfo)EditorGUILayout.ObjectField("None", brush.m_gameObjects[0], typeof(TileInfo), false, null);
        brush.m_gameObjects[1] = (TileInfo)EditorGUILayout.ObjectField("One", brush.m_gameObjects[1], typeof(TileInfo), false, null);
        brush.m_gameObjects[2] = (TileInfo)EditorGUILayout.ObjectField("Two", brush.m_gameObjects[2], typeof(TileInfo), false, null);
        brush.m_gameObjects[3] = (TileInfo)EditorGUILayout.ObjectField("Three", brush.m_gameObjects[3], typeof(TileInfo), false, null);
        brush.m_gameObjects[4] = (TileInfo)EditorGUILayout.ObjectField("Four", brush.m_gameObjects[4], typeof(TileInfo), false, null);
        brush.m_gameObjects[5] = (TileInfo)EditorGUILayout.ObjectField("BeginEnd", brush.m_gameObjects[5], typeof(TileInfo), false, null);
        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(brush);
    }
}
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using QTFramework;

[CreateAssetMenu(fileName = "Common Brush", menuName = "Brushes/Common brush")]
[CustomGridBrush(false, true, false, "Common Brush")]
public class CommonBrush : GameBrushBase
{
    public TileInfo[] m_Prefabs;
    public int m_Z;

    public int m_Level;

    public override void Paint(GridLayout grid, GameObject brushTarget, Vector3Int position)
    {
        if (brushTarget.GetComponent<Grid>() == null)
        {
            Paint(grid, brushTarget.transform.parent.gameObject, position);
            return;
        }
        // Do not allow editing palettes
        if (brushTarget.layer == 31)
            return;

        TileInfo instance = null;
        instance = Instantiate(m_Prefabs[m_Level]);

        if (instance != null)
        {

            Erase(grid, brushTarget, position);

            instance.size = m_Prefabs[m_Level].size;
            instance.prefabId = m_Prefabs[m_Level].prefabId;

            instance.transform.SetParent(brushTarget.transform);
            instance.transform.position = grid.LocalToWorld(grid.CellToLocalInterpolated(new Vector3Int(position.x, position.y, m_Z) + new Vector3(.5f, .5f, .5f)));

#if UNITY_EDITOR
            Undo.MoveGameObjectToScene(instance.gameObject, instance.gameObject.scene, "Paint Prefabs");
            Undo.RegisterCreatedObjectUndo((Object)instance, "Paint Prefabs");
            Selection.activeObject = instance;
#endif
            MapGrid mapGrid = grid.GetComponent<MapGrid>();
            mapGrid.SetTile(position, instance);
        }
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

        TileInfo erased = GetObjectInCell(grid, brushTarget.transform, new Vector3Int(position.x, position.y, m_Z));
        if (erased != null){
#if UNITY_EDITOR
            Undo.DestroyObjectImmediate(erased.gameObject);
#else
            GameObject.Destroy(erased.gameObject);
#endif
        }
            

    }

    private static TileInfo GetObjectInCell(GridLayout grid, Transform parent, Vector3Int position)
    {
        MapGrid mapGrid = grid.GetComponent<MapGrid>();
        return mapGrid.GetTile(position);
    }

    private static float GetPerlinValue(Vector3Int position, float scale, float offset)
    {
        return Mathf.PerlinNoise((position.x + offset) * scale, (position.y + offset) * scale);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(CommonBrush))]
public class CommonBrushEditor : GridBrushEditorBase
{
    private CommonBrush brush { get { return target as CommonBrush; } }

    private SerializedProperty m_Prefabs;
    private SerializedProperty m_ids;
    private SerializedObject m_SerializedObject;

    protected void OnEnable()
    {
        m_SerializedObject = new SerializedObject(target);
        m_Prefabs = m_SerializedObject.FindProperty("m_Prefabs");
    }

    public override void OnInspectorGUI()
    {
        m_SerializedObject.UpdateIfRequiredOrScript();

        string[] sizesName = new string[brush.m_Prefabs.Length];
        for (int i = 1; i <= sizesName.Length; i++) sizesName[i - 1] = i.ToString();

        brush.m_Level = EditorGUILayout.Popup("绘制第几个", brush.m_Level, sizesName);

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(m_Prefabs, true);

        m_SerializedObject.ApplyModifiedPropertiesWithoutUndo();

        EditorGUILayout.Space();

    }
}

#endif
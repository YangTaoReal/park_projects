using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using QTFramework;

[CreateAssetMenu(fileName = "Random Brush", menuName = "Brushes/Random brush")]
[CustomGridBrush(false, true, false, "Random Brush")]
public class RandomBrush : GridBrushBase
{
    
    public enum GrondType{
        grass,
        tree,
        stone
    }

    [SerializeField]
    public GrondType groundType;


    [SerializeField]
    public TileInfo[] m_grass;

    [SerializeField]
    public TileInfo[] m_tree;

    [SerializeField]
    public TileInfo[] m_stone;

    int m_Z;

    TileInfo requestRandomTile{
        get{
            if(groundType == GrondType.grass){
                if(m_grass != null && m_grass.Length > 0){
                    return m_grass[Random.Range(0, m_grass.Length)];
                }
            }else if(groundType == GrondType.tree)
            {
                if (m_tree != null && m_tree.Length > 0)
                {
                    return m_tree[Random.Range(0, m_tree.Length)];
                }
            }
            else if(groundType == GrondType.stone)
            {
                if (m_stone != null && m_stone.Length > 0)
                {
                    return m_stone[Random.Range(0, m_stone.Length)];
                }
            }


            return null;
        }
    }


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
        var cur = requestRandomTile;
        instance = Instantiate(cur);

        if (instance != null)
        {

            Erase(grid, brushTarget, position);

            instance.size = cur.size;
            instance.prefabId = cur.prefabId;

            instance.transform.SetParent(brushTarget.transform);
            instance.transform.position = grid.LocalToWorld(grid.CellToLocalInterpolated(new Vector3Int(position.x, position.y, m_Z) + new Vector3(.5f, .5f, 0)));
            //instance.transform.localEulerAngles = new Vector3(0, Random.Range(0.0f, 360.0f), 0);
#if UNITY_EDITOR
            //Undo.MoveGameObjectToScene(instance.gameObject, SceneManagerComponent.scene, "Paint Prefabs");
            //Undo.RegisterCreatedObjectUndo((Object)instance, "Paint Prefabs");
            Selection.activeObject = instance;
#endif
            MapGrid mapGrid = grid.GetComponent<MapGrid>();
            mapGrid.SetTile(position, instance);
            var createSerializable = instance.serializable;
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
            MapGrid mapGrid = grid.GetComponent<MapGrid>();
            mapGrid.ClearTile(position);

            if(Application.isPlaying && !MapGridMgr.Instance.isEditorMode){
                erased.Recycle();
            }else{
#if UNITY_EDITOR
                Undo.DestroyObjectImmediate(erased.gameObject);
#else
                GameObject.Destroy(erased.gameObject);
#endif
            }
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

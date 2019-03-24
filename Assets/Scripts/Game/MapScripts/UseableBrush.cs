using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


[CreateAssetMenu(fileName = "Useable Brush", menuName = "Brushes/Useable brush")]
[CustomGridBrush(false, true, false, "Useable Brush")]
public class UseableBrush : GridBrushBase
{
    public GridUseable.UseableType useable;


    public override void Paint(GridLayout grid, GameObject brushTarget, Vector3Int position)
    {
        GridUseable mapGrid = grid.GetComponent<GridUseable>();
        if (mapGrid == null) return;

        // Do not allow editing palettes
        if (brushTarget.layer == 31)
            return;

        mapGrid.Set(position, (byte)useable);
    }

    public override void Erase(GridLayout grid, GameObject brushTarget, Vector3Int position)
    {
        GridUseable mapGrid = grid.GetComponent<GridUseable>();
        if (mapGrid == null) return;

        // Do not allow editing palettes
        if (brushTarget.layer == 31)
            return;

        mapGrid.Remove(position);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(UseableBrush))]
public class UseableBrushEditor : GridBrushEditorBase
{
    private UseableBrush brush { get { return target as UseableBrush; } }

    public override void OnInspectorGUI()
    {

        brush.useable = (GridUseable.UseableType)EditorGUILayout.EnumPopup("类型", brush.useable);
    }
}

#endif

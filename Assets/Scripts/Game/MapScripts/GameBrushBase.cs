using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBrushBase : GridBrushBase
{
    public TileInfo m_PaintedTile;
    public bool isGameEditoring;
    protected bool _canSit;
    public bool canSit
    {
        get
        {
            return _canSit;
        }
    }

    protected TileInfo _lastEditTile;
    public TileInfo lastEditTile{
        get{
            return _lastEditTile;
        }
    }

    public virtual List<Vector3Int>  EditorPosList
    {
        get
        {
            return null;
        }
    }

    public virtual bool BeginEditor(GridLayout grid, GameObject brushTarget, Vector3Int position, int cfgId = -1) { return true; }
    public virtual bool BeginEditor(GridLayout grid, GameObject brushTarget, Vector3Int position, TileInfo tile) { return true;  }
    public virtual bool EndEditor(GridLayout grid) { return true; }
    public virtual bool Move(GridLayout grid, Vector3Int position) { return false; }
    public virtual void RotateEdit() { }
    public virtual void Erase(MapGrid mapGrid, TileInfo erased){}
    public virtual void CancelEdit(MapGrid mapGrid) { }
    public virtual void RemoveEdit(MapGrid mapGrid) { }


    //public static void EnableOutline(GameObject go){
    //    if (go == null) return;
    //    Outline outline = go.GetComponent<Outline>();
    //    if(outline == null){
    //        outline = go.AddComponent<Outline>();

    //        outline.OutlineColor = Color.green;
    //        outline.OutlineWidth = 5;
    //    }

    //    outline.enabled = true;
    //}

    //public  static void DisableOutline(GameObject go)
    //{
    //    if (go == null) return;
    //    Outline outline = go.GetComponent<Outline>();
    //    if (outline != null)
    //    {
    //        outline.enabled = false;
    //    }
    //}

}

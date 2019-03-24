using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class GridUseableTile
{
    public GridUseableTile(int _x, int _y, byte _t)
    {
        x = _x;
        y = _y;
        type = _t;
    }

    public int x;
    public int y;
    public byte type;
}

[System.Serializable]
public class GridUseableData
{
    public List<GridUseableTile> TileList = new List<GridUseableTile>();
}

public class GridUseable : MonoBehaviour
{
    public enum UseableType
    {
        Lock,
        Free,
        Freeing,
    }


    public bool zip = false;
    public Grid grid;
    GridUseableData mapData;
    Dictionary<Vector3Int, GridUseableTile> gridTypes = new Dictionary<Vector3Int, GridUseableTile>();
    public Dictionary<Vector3Int, GridUseableTile> All
    {
        get
        {
            return gridTypes;
        }
    }



#if UNITY_EDITOR

    [DrawGizmo(GizmoType.Selected | GizmoType.NonSelected | GizmoType.Active | GizmoType.InSelectionHierarchy | GizmoType.NotInSelectionHierarchy | GizmoType.Selected)]
    static void OnDrawGizmos(GridUseable gridUseable, GizmoType type)
    {
        //if (Application.isPlaying) return;

        Vector3 cellSize = new Vector3(gridUseable.grid.cellSize.x, 0.1f, gridUseable.grid.cellSize.y);
        var list = gridUseable.All;
        foreach (var item in list)
        {
            if (item.Value.type == (byte)GridUseable.UseableType.Free)
            {
                Gizmos.color = new Color(0, 1, 0, 0.2f);
            }
            else if (item.Value.type == (byte)GridUseable.UseableType.Freeing)
            {
                Gizmos.color = new Color(1, 1, 0, 0.2f);
            }
            else if (item.Value.type == (byte)GridUseable.UseableType.Lock)
            {
                Gizmos.color = new Color(1, 0, 0, 0.2f);
            }

            Vector3 p = gridUseable.grid.LocalToWorld(gridUseable.grid.CellToLocalInterpolated(new Vector3Int(item.Key.x, item.Key.y, 0) + new Vector3(.5f, .5f, 0)));
            Gizmos.DrawCube(p, cellSize);
            Gizmos.DrawWireCube(p, cellSize);
        }
    }
#endif

    public void Set(Vector3Int pos, byte useableType)
    {
        if (gridTypes.ContainsKey(pos))
        {
            gridTypes[pos].type = useableType;
        }
    }

    public void Set(Vector3Int pos, GridUseableTile useable)
    {
        if (gridTypes.ContainsKey(pos))
        {
            gridTypes[pos] = useable;
        }
        else
        {
            gridTypes.Add(pos, useable);
        }
    }

    public void Remove(Vector3Int pos)
    {
        if(gridTypes.ContainsKey(pos))
        {
            var data = gridTypes[pos];
            mapData.TileList.Remove(data);
            gridTypes.Remove(pos);
        }
    }

    public void Free(Vector3Int pos)
    {
        Set(pos, (byte)UseableType.Free);
    }

    public void Freeing(Vector3Int pos)
    {
        Set(pos, (byte)UseableType.Freeing);
    }

    public void Lock(Vector3Int pos)
    {
        Set(pos, (byte)UseableType.Lock);
    }

    //是不是可选地块
    public bool IsPlot(Vector3Int pos)
    {
        if (gridTypes.ContainsKey(pos))
            return true;
        return false;
    }

    public bool IsFree(Vector3Int pos)
    {
        if (gridTypes.ContainsKey(pos))
        {
            return gridTypes[pos].type == (byte)UseableType.Free;
        }
        else
        {
            return false;
        }
    }
    //黄色地块
    public bool IsFreeing(Vector3Int pos)
    {
        if (gridTypes.ContainsKey(pos))
        {
            return gridTypes[pos].type == (byte)UseableType.Freeing;
        }
        else
        {
            return false;
        }
    }
    //红色
    public bool IsLock(Vector3Int pos)
    {
        if (gridTypes.ContainsKey(pos))
        {
            return gridTypes[pos].type == (byte)UseableType.Lock;
        }
        else
        {
            return false;
        }
    }

    public void Clear()
    {
        gridTypes.Clear();
        mapData.TileList.Clear();
    }

    //导出josn字符串
    public string SerializeToJson()
    {
        byte[] data = Serialize();
        return ByteToJson(data);
    }

    //导出二进制数据
    public byte[] Serialize()
    {
        if (grid == null)
        {
            Debug.Log("Grid is null.");
            return null;
        }
        else
        {

            var save = GetMapData();
            if (save == null || save.TileList.Count == 0)
            {
                return null;
            }
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, save);

            byte[] ret;
            if(zip)
            {
                ret = ShareZip.CompressByteToByte(ms.ToArray());
            }
            else
            {
                ret = ms.GetBuffer();
            }
            ms.Close();

            return ret;
        }
    }

    //导入json字符串
    public bool DeserializeFromJson(string jsonStr)
    {
        byte[] data = JsonToByte(jsonStr);
        return Deserialize(data);
    }

    //导入二进制数据
    public bool Deserialize(byte[] data)
    {
        if (zip)
        {
            MemoryStream unzipData = null;
            try
            {
                unzipData = ShareZip.DecompressByteToMS(data);
            }
            catch (System.Exception e)
            {
                Debug.LogError("Deserialize Error:" + e.Message);
                return false;
            }

            BinaryFormatter bf = new BinaryFormatter();
            mapData = (GridUseableData)bf.Deserialize(unzipData);
            unzipData.Close();
        }
        else
        {
            BinaryFormatter bf = new BinaryFormatter();
            mapData = (GridUseableData)bf.Deserialize(new MemoryStream(data));
        }

        SetMapData(mapData);

        return true;
    }


    public static string ByteToJson(byte[] data)
    {
        if (data == null) return null;
        string dataStr = System.Convert.ToBase64String(data);
        string jsonStr = JsonUtility.ToJson(new JsonStr(dataStr));

        return jsonStr;
    }

    public static byte[] JsonToByte(string jsonStr)
    {
        if (jsonStr == null) return null;

        JsonStr json = JsonUtility.FromJson<JsonStr>(jsonStr);
        if (json != null && json.data != null)
        {
            return System.Convert.FromBase64String(json.data);
        }
        else
        {
            Debug.LogError("JsonUtility.FromJson<JsonStr> get null. check format of json string=" + jsonStr + " with the class <JsonStr>");
            return null;
        }
    }

    protected GridUseableData GetMapData()
    {
        //GridUseableData mapData = new GridUseableData();

        //var root = grid.transform;
        //foreach (var item in gridTypes)
        //{
        //    var serData = new GridUseableTile(item.Key.x, item.Key.y, item.Value);
        //    mapData.TileList.Add(serData);
        //}
        return mapData;
    }

    protected void SetMapData(GridUseableData mapData)
    {
        gridTypes.Clear();

        var data = mapData.TileList;
        for (int i = 0; i < data.Count; i++)
        {
            Set(new Vector3Int(data[i].x, data[i].y, 0), data[i]);
        }
    }


    public Material lineMaterial;
    public Material unselectedMaterial;
    public Material selectedMaterial;
    public Material freeingMaterial;
    void DrawLine(Vector3 start, Vector3 end)
    {

        GL.Vertex3(start.x, start.y, start.z);
        GL.Vertex3(end.x, end.y, end.z);
    }

    void DrawRectFrame(Vector3 pos, float size)
    {
        Vector3 posLeftTop = new Vector3(pos.x - size / 2, 0.1f, pos.z + size / 2);
        Vector3 posRightTop = new Vector3(pos.x + size / 2, 0.1f, pos.z + size / 2);
        Vector3 posRightDown = new Vector3(pos.x + size / 2, 0.1f, pos.z - size / 2);
        Vector3 posLeftDown = new Vector3(pos.x - size / 2, 0.1f, pos.z - size / 2);

        DrawLine(posLeftTop, posRightTop);
        DrawLine(posRightTop, posRightDown);
        DrawLine(posRightDown, posLeftDown);
        DrawLine(posLeftDown, posLeftTop);

    }

    void DrawRect(Vector3 pos, float size)
    {
        Vector3 posLeftTop = new Vector3(pos.x - size / 2, 0.1f, pos.z + size / 2);
        Vector3 posRightTop = new Vector3(pos.x + size / 2, 0.1f, pos.z + size / 2);
        Vector3 posRightDown = new Vector3(pos.x + size / 2, 0.1f, pos.z - size / 2);
        Vector3 posLeftDown = new Vector3(pos.x - size / 2, 0.1f, pos.z - size / 2);

        GL.Vertex3(posLeftTop.x, posLeftTop.y, posLeftTop.z);
        GL.Vertex3(posRightTop.x, posRightTop.y, posRightTop.z);
        GL.Vertex3(posRightDown.x, posRightDown.y, posRightDown.z);
        GL.Vertex3(posLeftDown.x, posLeftDown.y, posLeftDown.z);

    }

    bool showgrid = false;
    public void ShowGrid(){
        showgrid = true;
    }

    public void HideGrid()
    {
        showgrid = false;
    }

    private void OnRenderObject()
    {
        if (!showgrid) return;

        List<Vector3> all = new List<Vector3>();
        List<Vector3> selected = new List<Vector3>();
        List<Vector3> unselected = new List<Vector3>();
        List<Vector3> freeing = new List<Vector3>();
        foreach (var item in All)
        {
            Vector3 p = grid.LocalToWorld(grid.CellToLocalInterpolated(new Vector3Int(item.Key.x, item.Key.y, 0) + new Vector3(.5f, .5f, 0)));

            if (item.Value.type == (byte)GridUseable.UseableType.Lock)
            {
                if(MapGridMgr.Instance.SelectedWastedlandTile.ContainsValue(item.Key)){
                    selected.Add(p);
                }else{
                    unselected.Add(p);
                }
            }
            else if(item.Value.type == (byte)GridUseable.UseableType.Freeing)
            {
                freeing.Add(p);
            }

            all.Add(p);
        }

        unselectedMaterial.SetPass(0);
        GL.Begin(GL.QUADS);
        foreach (var p in unselected)
        {
            DrawRect(p, 3);
        }
        GL.End();

        selectedMaterial.SetPass(0);
        GL.Begin(GL.QUADS);
        foreach (var p in selected)
        {
            DrawRect(p, 3);
        }
        GL.End();

        freeingMaterial.SetPass(0);
        GL.Begin(GL.QUADS);
        foreach (var p in freeing)
        {
            DrawRect(p, 3);
        }
        GL.End();

        lineMaterial.SetPass(0);
        GL.Begin(GL.LINES);
        foreach(var p in all){
            DrawRectFrame(p, 3);
        }
        GL.End();
    }

}


#if UNITY_EDITOR
    [CustomEditor(typeof(GridUseable))]
public class GridUseableEditor : Editor
{
    bool saveJson;

    protected void OnEnable()
    {
        saveJson = true;

        GridUseable myScript = (GridUseable)target;
        if (myScript.grid == null)
        {
            myScript.grid = myScript.gameObject.AddComponent<Grid>();
            myScript.grid.cellSwizzle = GridLayout.CellSwizzle.XZY;
            myScript.grid.cellSize = Vector3.one * 3;
        }
    }

    public override void OnInspectorGUI()
    {
        GridUseable myScript = (GridUseable)target;

        DrawDefaultInspector();

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        saveJson = EditorGUILayout.Toggle("存储为json格式文件", saveJson);

        EditorGUILayout.Space();

        if (GUILayout.Button("Export To File"))
        {
            ExportToFile(myScript, saveJson);
        }

        if (GUILayout.Button("Clear"))
        {
            myScript.Clear();
        }

        if (GUILayout.Button("Import From File"))
        {
            ImportFromFile(myScript, saveJson);
        }

        EditorGUILayout.Space();

        //myScript.lineMaterial = (Material)EditorGUILayout.ObjectField("材质", myScript.lineMaterial, typeof(Material), false, null);


        EditorGUILayout.LabelField("数据:", myScript.All.Count.ToString() + "条");
    }

    void ExportToFile(GridUseable mapGrid, bool isJson = false)
    {
        if (mapGrid.grid == null)
        {
            Debug.Log("Grid is null.");
            return;
        }
        else
        {
            byte[] data = mapGrid.Serialize();
            if (data == null)
            {
                Debug.Log("data is empty.");
                return;
            }

            string filePath = SavePath;

            if (isJson)
            {
                string jsonStr = GridUseable.ByteToJson(data);
                data = System.Text.Encoding.Default.GetBytes(jsonStr);
                filePath = filePath + ".json";
            }
            else
            {
                filePath = filePath + ".save";
            }



            string directoryName = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }

            FileStream file = File.Create(filePath);
            file.Write(data, 0, data.Length);
            file.Close();

            Debug.Log("ExportToFile OK!\n" + filePath);
        }

        return;
    }

    void ImportFromFile(GridUseable mapGrid, bool isJson = false)
    {
        if (mapGrid.grid == null)
        {
            Debug.Log("Grid is null.");
            return;
        }
        else
        {
            string filePath = SavePath;
            if (isJson)
            {
                filePath = filePath + ".json";
            }
            else
            {
                filePath = filePath + ".save";
            }

            if (!File.Exists(filePath))
            {
                Debug.LogError("ImportFromFile Failed!!!\n" + filePath + " is not Exists.");
                return;
            }

            FileStream file = File.OpenRead(filePath);
            byte[] data = new byte[file.Length];
            file.Read(data, 0, (int)file.Length);
            file.Close();

            if (isJson)
            {
                string dataStr = System.Text.Encoding.Default.GetString(data);
                data = GridUseable.JsonToByte(dataStr);
            }

            if (data == null)
            {
                Debug.LogError("ImportFromFile Error: data is null.\n" + filePath);
                return;
            }

            if (mapGrid.Deserialize(data))
            {
                Debug.Log("ImportFromFile OK!\n" + filePath);
            }
            else
            {
                Debug.LogError("ImportFromFile Error!\n" + filePath);
            }

        }
    }


    string SavePath
    {
        get
        {
            GridUseable myScript = (GridUseable)target;
            return Application.dataPath + "/Resources/MapData/Map_" + myScript.name;
        }
    }

}
#endif
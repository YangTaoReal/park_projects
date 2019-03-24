using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using QTFramework;

[System.Serializable]
public class SerializableVec3
{
    public SerializableVec3(float _x, float _y, float _z)
    {
        x = _x;
        y = _y;
        z = _z;
    }

    public SerializableVec3(Vector3 vector3)
    {
        FromVector3(vector3);
    }

    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }

    public void FromVector3(Vector3 vector3)
    {
        x = vector3.x;
        y = vector3.y;
        z = vector3.z;
    }

    public float x;
    public float y;
    public float z;

}

[System.Serializable]
public class SerializableTileItem
{

    public SerializableTileItem(int i, Vector3 p, Vector3 r, Vector3 s, System.Guid _guid, int _gameId)
    {
        prefabId = i;
        pos = new SerializableVec3(p);
        rotate = new SerializableVec3(r);
        scale = new SerializableVec3(s);
        if (_guid.Equals(System.Guid.Empty))
        {
            guid = string.Empty;
        }
        else
        {
            guid = _guid.ToString();
        }
        gameId = _gameId;
    }

    public SerializableTileItem(int i, Vector3 p, Vector3 r, Vector3 s, System.Guid _guid)
    {
        prefabId = i;
        pos = new SerializableVec3(p);
        rotate = new SerializableVec3(r);
        scale = new SerializableVec3(s);
        if (_guid.Equals(System.Guid.Empty))
        {
            guid = string.Empty;
        }
        else
        {
            guid = _guid.ToString();
        }
    }

    public string guid;
    public int prefabId;
    public int gameId;
    public SerializableVec3 pos;
    public SerializableVec3 rotate;
    public SerializableVec3 scale;
    public byte[] data;

}

[System.Serializable]
public class MapData
{
    public List<SerializableTileItem> TileList = new List<SerializableTileItem>();
}

public class JsonStr
{

    public JsonStr(string str)
    {
        data = str;
    }
    public string data;
}

public class MapGrid : MonoBehaviour
{
    public bool zip = false;
    public Grid grid;
    public bool combineMesh;
    public bool isBuilding;
    public MapGridMgr mapGridMgr;
    public bool isWastland = false;

    public MapGridMgr.MapEvent onLoaded;
    public MapGridMgr.MapProgressEvent onLoading;

    public Dictionary<Vector3Int, TileInfo> tiles = new Dictionary<Vector3Int, TileInfo>();

    protected void Start()
    {
        if (combineMesh)
        {
            var mf = GetComponent<MeshFilter>();
            var mr = GetComponent<MeshRenderer>();

            if (mf == null || mr == null || mr.material == null)
            {
                Debug.LogError("combineMesh Error: " + "mf == null || mr == null || mr.material == null");
            }

            CombineMesh();
        }

        //StartCoroutine(IEInitTilePos());
        InitTilePos(); //子类覆盖
    }

    public virtual void InitTilePos()
    {
        //if (!isBuilding) return;

        tiles.Clear();

        var root = grid.transform;
        for (int i = 0; i < root.childCount; i++)
        {
            var tileInfo = root.GetChild(i).GetComponent<TileInfo>();
            UpdateTiles(tileInfo.transform.position, tileInfo);
        }
    }

    //IEnumerator IEInitTilePos()
    //{

    //    if (!isBuilding) yield break;

    //    tiles.Clear();

    //    var root = grid.transform;
    //    for (int i = 0; i < root.childCount; i++)
    //    {
    //        var tileInfo = root.GetChild(i).GetComponent<TileInfo>();
    //        UpdateTiles(tileInfo.transform.position, tileInfo);
    //        yield return new WaitForEndOfFrame();
    //    }
    //}

    #region 对外接口

    //导出josn字符串
    public string SerializeToJson()
    {
        byte[] data = Serialize();
        return ByteToJson(data);
    }

    //导出二进制数据
    public byte[] Serialize(bool exportGuid = true)
    {
        if (grid == null)
        {
            Debug.Log("Grid is null.");
            return null;
        }
        else
        {
            MapData save = GetMapData(exportGuid);
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, save);

            byte[] ret;
            if (zip)
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
        MapData mapData = null;
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
            mapData = (MapData)bf.Deserialize(unzipData);
            unzipData.Close();
        }
        else
        {
            BinaryFormatter bf = new BinaryFormatter();
            mapData = (MapData)bf.Deserialize(new MemoryStream(data));
        }

        if (combineMesh)
        {
            StartCoroutine(ISetMapDataSyn(mapData));
        }
        else
        {
            StartCoroutine(ISetMapDataAsyn(mapData));
        }

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

    public virtual void ReplaceTile(TileInfo tileInfoOld, TileInfo tileInfo)
    {
        if (tileInfoOld == null || tileInfo == null) return;

        Vector3Int vector3Int = grid.WorldToCell(tileInfoOld.transform.position);
        if (tiles.ContainsKey(vector3Int))
        {
            tileInfo.transform.SetParent(grid.transform);
            tileInfo.transform.position = tileInfoOld.transform.position;
            tileInfo.transform.rotation = tileInfoOld.transform.rotation;
            tileInfo.transform.localScale = tileInfoOld.transform.localScale;

            tileInfo.baseData = tileInfoOld.baseData;
            tileInfo.baseData.go = tileInfo.gameObject;

            tileInfo.serializable = tileInfoOld.serializable;
            tileInfo.serializable.guid = tileInfo.baseData.guid.ToString();
            tileInfo.serializable.gameId = tileInfoOld.gameId;
            tileInfo.serializable.prefabId = tileInfo.prefabId;

            List<Vector3Int> temp = new List<Vector3Int>();
            foreach (var pair in tiles)
            {
                if (pair.Value == tileInfoOld)
                {
                    temp.Add(pair.Key);
                }
            }

            foreach (var key in temp)
            {
                SetTile(key, tileInfo);
            }
        }
    }

    public virtual void SetTile(Vector3Int vector3Int, TileInfo tileInfo)
    {
        if (tiles.ContainsKey(vector3Int))
        {
            Debug.LogWarning("tiles.ContainsKey:" + vector3Int.ToString() + " go=" + tileInfo.gameObject.name);
            tiles[vector3Int] = tileInfo;
        }
        else
        {
            tiles.Add(vector3Int, tileInfo);
        }
    }

    public bool IsFree(Vector3Int vector3Int)
    {
        return mapGridMgr.Useable.IsFree(vector3Int);
    }

    public bool IsLock(Vector3Int vector3Int)
    {
        return mapGridMgr.Useable.IsLock(vector3Int);
    }

    public TileInfo GetAllTile(Vector3Int vector3Int)
    {
        TileInfo tileInfo;
        foreach (var grid in mapGridMgr.buildingGrids)
        {
            tileInfo = grid.GetTile(vector3Int);
            if (tileInfo != null)
            {
                return tileInfo;
            }
        }

        return null;
    }

    public TileInfo GetOtherTile(Vector3Int vector3Int)
    {
        TileInfo tileInfo;
        foreach (var grid in mapGridMgr.buildingGrids)
        {
            if (grid != this)
            {
                tileInfo = grid.GetTile(vector3Int);
                if (tileInfo != null)
                {
                    return tileInfo;
                }
            }

        }

        return null;
    }

    public TileInfo GetTile(Vector3Int vector3Int)
    {
        if (tiles.ContainsKey(vector3Int))
        {
            return tiles[vector3Int];
        }
        else
        {
            return null;
        }
    }

    public TileInfo GetTile(Vector3 wordlPos)
    {
        Vector3Int pos = grid.WorldToCell(wordlPos);
        return GetTile(pos);
    }

    public bool HasAllTile(Vector3Int vector3Int)
    {
        return GetAllTile(vector3Int) != null;
    }

    public bool HasTile(Vector3Int vector3Int)
    {
        return GetTile(vector3Int) != null;
    }

    public virtual void ClearTile(Vector3Int vector3Int)
    {
        tiles.Remove(vector3Int);
    }

    public virtual void ClearTile(TileInfo tileInfo)
    {
        if (tileInfo == null) return;

        List<Vector3Int> temp = new List<Vector3Int>();
        foreach (var pair in tiles)
        {
            if (pair.Value == tileInfo)
            {
                temp.Add(pair.Key);
            }
        }

        foreach (var key in temp)
        {
            tiles.Remove(key);
        }
    }

    #endregion

    protected virtual MapData GetMapData(bool exportGuid = true)
    {
        //MapData mapData = new MapData();

        //var root = grid.transform;
        //for (int i = 0; i < root.childCount; i++)
        //{
        //    var serData = SerialDataFromTileObject(root.GetChild(i).GetComponent<TileInfo>(), exportGuid);
        //    mapData.TileList.Add(serData);
        //}
        //return mapData;

        List<TileInfo> hasExp = new List<TileInfo>();
        MapData mapData = new MapData();
        foreach (var items in tiles)
        {
            if (hasExp.Contains(items.Value)) continue;

            var serData = SerialDataFromTileObject(items.Value, exportGuid);
            mapData.TileList.Add(serData);
            hasExp.Add(items.Value);
        }

        hasExp = null;
        return mapData;
    }

    // 同步
    protected virtual IEnumerator ISetMapDataSyn(MapData mapData)
    {
        yield return new WaitForEndOfFrame();

        if (onLoading != null) onLoading(0);

        SetMapData(mapData);

        if (combineMesh)
        {
            CombineMesh();
        }

        if (onLoading != null) onLoading(100);

        yield return new WaitForEndOfFrame();

        if (onLoaded != null)
        {
            onLoaded();
        }

    }

    protected void SetMapData(MapData mapData)
    {

        var root = grid.transform;
        var data = mapData.TileList;
        for (int i = 0; i < data.Count; i++)
        {
            var go = TileObjectFromSerialData(data[i]);
            if (go == null)
            {
                Debug.LogWarning("can not find prefab by id " + data[i].prefabId.ToString());
                continue;
            }

            go.transform.SetParent(transform);
            go.transform.localScale = data[i].scale.ToVector3();
            go.transform.localPosition = data[i].pos.ToVector3();
            go.transform.localEulerAngles = data[i].rotate.ToVector3();
            go.gameObject.SetActive(true);

            UpdateTiles(go.transform.position, go.GetComponent<TileInfo>());
        }
    }

    // 异步
    protected virtual IEnumerator ISetMapDataAsyn(MapData mapData)
    {
        yield return new WaitForEndOfFrame();

        if (onLoading != null) onLoading(0);

        var root = grid.transform;
        var data = mapData.TileList;
        int cnt = 0;
        for (int i = 0; i < data.Count; i++)
        {
#if UNITY_EDITOR
            var pos = grid.WorldToCell(data[i].pos.ToVector3());
            if (tiles.ContainsKey(pos))
            {
                Debug.LogError("tiles.ContainsKey:" + pos.ToString() + " From " + name);
                continue;
            }
#endif

            var go = TileObjectFromSerialData(data[i]);
            if (go == null)
            {
                continue;
            }

            cnt++;

            go.transform.SetParent(transform);
            go.transform.localScale = data[i].scale.ToVector3();
            go.transform.localPosition = data[i].pos.ToVector3();
            go.transform.localEulerAngles = data[i].rotate.ToVector3();
            go.gameObject.SetActive(true);

            UpdateTiles(go.transform.position, go.GetComponent<TileInfo>());

            if(isWastland)
            {
                Vector3 scaleTemp;
                Transform trasTemp;
                string grassTag = "wastedlandgrass";
                for (int childIdx = 0; childIdx < go.transform.childCount; childIdx++)
                {
                    trasTemp = go.transform.GetChild(childIdx);
                    if(trasTemp.gameObject.CompareTag(grassTag))
                    {
                        scaleTemp = go.transform.GetChild(childIdx).localScale;
                        scaleTemp *= Random.Range(0.70f, 1.05f);
                        go.transform.GetChild(childIdx).localScale = scaleTemp;
                    }
                }
            }

            if (cnt >= 10)
            {
                float val = (i + 1) / (float)data.Count;
                if (onLoading != null) onLoading((int)(val * 100));
                cnt = 0;
                yield return new WaitForSeconds(0.01f);
            }
        }

        if (combineMesh)
        {
            CombineMesh();
        }

        if (onLoading != null) onLoading(100);

        yield return new WaitForEndOfFrame();

        if (onLoaded != null)
        {
            onLoaded();
        }
    }

    public static TileInfo RequestTileInfo(int cfgId, SerializableTileItem data = null)
    {

        var model = World.Scene.GetComponent<ModelManager>();
        if (model == null)
        {
            Debug.LogError("ModelManager is null");
            return null;
        }
        BaseData modelData = null;
        if (data != null && !string.IsNullOrEmpty(data.guid))
        {
            modelData = model.Load(cfgId, null, System.Guid.Parse(data.guid));
        }
        else
        {
            modelData = model.Load(cfgId, null, System.Guid.Empty, false);
        }

        if (modelData == null)
        {
            Debug.LogError("Get Mode Data is Null with cfg id = " + cfgId);
            return null;
        }

        TileInfo go = modelData.go.GetComponent<TileInfo>();
        if (go == null)
        {
            Debug.LogError("Get TileInfo is Null with cfg id = " + cfgId);
            return null;
        }

        go.DisableOutline();
        go.Init(modelData);
        if (null != data)
        {
            go.serializable = data;
        }
        go.serializable.guid = go.guid.ToString();

        return go;
    }

    protected virtual SerializableTileItem SerialDataFromTileObject(TileInfo tileInfo, bool exportGuid = true)
    {
        //if(exportGuid)
        //{
        //    return new SerializableTileItem(tileInfo.prefabId,
        //                                tileInfo.transform.localPosition,
        //                                tileInfo.transform.localRotation.eulerAngles,
        //                                tileInfo.transform.localScale,
        //                                tileInfo.guid);
        //}
        //else
        //{
        //    return new SerializableTileItem(tileInfo.prefabId,
        //                                tileInfo.transform.localPosition,
        //                                tileInfo.transform.localRotation.eulerAngles,
        //                                tileInfo.transform.localScale,
        //                                System.Guid.Empty);
        //}

        if (exportGuid)
        {
            tileInfo.serializable.pos.FromVector3(tileInfo.transform.localPosition);
            tileInfo.serializable.rotate.FromVector3(tileInfo.transform.localEulerAngles);
            tileInfo.serializable.scale.FromVector3(tileInfo.transform.localScale);
            return tileInfo.serializable;
        }
        else
        {
            return new SerializableTileItem(tileInfo.prefabId,
                tileInfo.transform.localPosition,
                tileInfo.transform.localRotation.eulerAngles,
                tileInfo.transform.localScale,
                System.Guid.Empty);
        }
    }

    protected virtual TileInfo TileObjectFromSerialData(SerializableTileItem data)
    {
        TileInfo go = RequestTileInfo(data.prefabId, data);
        return go;
    }

    protected void CombineMesh()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        if (meshFilters.Length < 2) return;

        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        int i = 0;
        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);
            i++;
        }
        var mesh = new Mesh();
        transform.GetComponent<MeshFilter>().mesh = mesh;
        transform.GetComponent<MeshFilter>().sharedMesh.CombineMeshes(combine);
        transform.gameObject.SetActive(true);
    }

    protected virtual void UpdateTiles(Vector3 pos, TileInfo tileInfo)
    {

        var posInt = grid.WorldToCell(pos);

        if (tileInfo.size <= 1)
        {
            SetTile(posInt, tileInfo);
        }
        else if (tileInfo.size == 2)
        {
            float x = grid.cellSize.x / 2;
            float y = grid.cellSize.y / 2;

            var posInt1 = grid.WorldToCell(pos + new Vector3(-x, 0, y));
            SetTile(posInt1, tileInfo);
            SetTile(posInt1 + new Vector3Int(1, 0, 0), tileInfo);
            SetTile(posInt1 + new Vector3Int(1, -1, 0), tileInfo);
            SetTile(posInt1 + new Vector3Int(0, -1, 0), tileInfo);

        }
        else if (tileInfo.size == 3)
        {
            float x = grid.cellSize.x;
            float y = grid.cellSize.y;

            var posInt1 = grid.WorldToCell(pos + new Vector3(-x, 0, y));
            SetTile(posInt1, tileInfo);
            SetTile(posInt1 + new Vector3Int(1, 0, 0), tileInfo);
            SetTile(posInt1 + new Vector3Int(2, 0, 0), tileInfo);
            SetTile(posInt1 + new Vector3Int(0, -1, 0), tileInfo);
            SetTile(posInt1 + new Vector3Int(1, -1, 0), tileInfo);
            SetTile(posInt1 + new Vector3Int(2, -1, 0), tileInfo);
            SetTile(posInt1 + new Vector3Int(0, -2, 0), tileInfo);
            SetTile(posInt1 + new Vector3Int(1, -2, 0), tileInfo);
            SetTile(posInt1 + new Vector3Int(2, -2, 0), tileInfo);
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(MapGrid))]
public class GameGridEditor : Editor
{
    bool saveJson;

    protected void OnEnable()
    {
        saveJson = true;

        MapGrid myScript = (MapGrid)target;
        if (myScript.grid == null)
        {
            myScript.grid = myScript.gameObject.AddComponent<Grid>();
            myScript.grid.cellSwizzle = GridLayout.CellSwizzle.XZY;
            myScript.grid.cellSize = Vector3.one * 3;
        }

        if (!Application.isPlaying)
        {
            myScript.InitTilePos();
        }
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        saveJson = EditorGUILayout.Toggle("存储为json格式文件", saveJson);

        EditorGUILayout.Space();

        if (GUILayout.Button("Export To File"))
        {
            MapGrid myScript = (MapGrid)target;
            ExportToFile(myScript, saveJson);
        }

        if (GUILayout.Button("Clear Childs"))
        {
            MapGrid myScript = (MapGrid)target;
            while (myScript.transform.childCount > 0)
            {
                for (int child = 0; child < myScript.transform.childCount; child++)
                {
                    Undo.DestroyObjectImmediate(myScript.transform.GetChild(child).gameObject);
                }
            }

            myScript.tiles.Clear();

            var meshFilter = myScript.GetComponent<MeshFilter>();
            if (meshFilter != null)
            {
                meshFilter.sharedMesh = null;
            }
        }

        if (GUILayout.Button("Import From File"))
        {
            MapGrid myScript = (MapGrid)target;
            ImportFromFile(myScript, saveJson);
        }

        EditorGUILayout.Space();
    }

    void ExportToFile(MapGrid mapGrid, bool isJson = false)
    {
        if (mapGrid.grid == null)
        {
            Debug.Log("Grid is null.");
            return;
        }
        else
        {
            byte[] data = mapGrid.Serialize(false);
            if (data == null)
            {
                Debug.Log("data is empty.");
                return;
            }

            string filePath = SavePath;

            if (isJson)
            {
                string jsonStr = MapGrid.ByteToJson(data);
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

    void ImportFromFile(MapGrid mapGrid, bool isJson = false)
    {
        mapGrid.onLoading = (int val) =>
        {
            Debug.Log("onLoading:" + val + "%");
        };
        mapGrid.onLoaded = () =>
        {
            Debug.Log("onLoaded.");
        };

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
                data = MapGrid.JsonToByte(dataStr);
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
            MapGrid myScript = (MapGrid)target;
            return Application.dataPath + "/Resources/MapData/Map_" + myScript.name;
        }
    }
}
#endif
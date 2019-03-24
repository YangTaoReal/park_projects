using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MapGridBarrier : MapGrid {

    Dictionary<int, int> _gameIdList = new Dictionary<int, int>();

    bool ContainGameId(int gameId){
        return _gameIdList.ContainsKey(gameId);
    }

    public override void InitTilePos()
    {
        _gameIdList.Clear();
        base.InitTilePos();
    }

    void RemoveGameId(int gameId)
    {
        if (ContainGameId(gameId))
        {
            var cur = _gameIdList[gameId] - 1;
            if(cur <= 0){
                _gameIdList.Remove(gameId);
            }else{
                _gameIdList[gameId] = cur;
            }
        }
    }


    public void AddGameId(int gameId)
    {
        if (ContainGameId(gameId))
        {
            var old = _gameIdList[gameId];
            _gameIdList[gameId] = old + 1;
        }
        else
        {
            _gameIdList.Add(gameId, 1);
        }
    }

    public int GetNextGameId(){
        int temp = 1;
        while(ContainGameId(temp)){
            temp++;
        }

        return temp;
    }

    public override void SetTile(Vector3Int vector3Int, TileInfo tileInfo)
    {
        var old = GetTile(vector3Int);
        if (old != null)
        {
            ClearTile(old);

            tiles[vector3Int] = tileInfo;
        }
        else
        {
            tiles.Add(vector3Int, tileInfo);
        }

        AddGameId(tileInfo.gameId);
        //Debug.Log("SetTile");
    }

    public override void ClearTile(Vector3Int vector3Int)
    {
        var tileInfo = GetTile(vector3Int);
        if (tileInfo == null)
        {
            return;
        }
        else{
            RemoveGameId(tileInfo.gameId);
            base.ClearTile(vector3Int);
            //Debug.Log("ClearTile");
        }
    }

    public override void ClearTile(TileInfo tileInfo)
    {
        if (tileInfo == null) return;

        RemoveGameId(tileInfo.gameId);

        base.ClearTile(tileInfo);
        //Debug.Log("ClearTile");
    }

    protected override SerializableTileItem SerialDataFromTileObject(TileInfo tileInfo, bool exportGuid = true)
    {
        SerializableTileItem serItem = null;
        if(exportGuid)
        {
            tileInfo.serializable.pos.FromVector3(tileInfo.transform.localPosition);
            tileInfo.serializable.rotate.FromVector3(tileInfo.transform.localEulerAngles);
            tileInfo.serializable.scale.FromVector3(tileInfo.transform.localScale);
            serItem = tileInfo.serializable;
        }
        else
        {
            serItem = new SerializableTileItem(tileInfo.prefabId,
                                       tileInfo.transform.localPosition,
                                       tileInfo.transform.localRotation.eulerAngles,
                                       tileInfo.transform.localScale,
                                       System.Guid.Empty,
                                       tileInfo.gameId);
        }

        serItem.data = new byte[tileInfo.transform.childCount];
        for (int j = 0; j < tileInfo.transform.childCount; j++)
        {
            serItem.data[j] = tileInfo.transform.GetChild(j).gameObject.activeSelf ? (byte)1 : (byte)0;
        }

        return serItem;
    }

    protected override TileInfo TileObjectFromSerialData(SerializableTileItem data)
    {
        TileInfo go = RequestTileInfo(data.prefabId, data);
        if (go == null) return null;
        go.transform.localScale = data.scale.ToVector3();
        go.transform.localPosition = data.pos.ToVector3();
        go.transform.localEulerAngles = data.rotate.ToVector3();
        go.gameId = data.gameId;
        for (int i = 0; i < go.transform.childCount && i < data.data.Length; i++)
        {
            go.transform.GetChild(i).gameObject.SetActive(data.data[i] != 0);
        }

        return go;
    }

    public void ClearTiles(int gameId){
        var list = TilesByGameId(gameId);
        foreach (var tile in list)
        {
            ClearTile(tile.Value);
        }
       
    }

    public Dictionary<Vector3Int, TileInfo> TilesByGameId(int gameId){
        Dictionary<Vector3Int, TileInfo> ret = new Dictionary<Vector3Int, TileInfo>();
        foreach(var item in tiles){
            if(item.Value.gameId == gameId){
                ret.Add(item.Key, item.Value);
            }
        }

        return ret;
    }

    public MapData TilesToData(int gameId){
        MapData mapData = new MapData();
        var list = TilesByGameId(gameId);
        foreach (var item in list)
        {
            var serData = SerialDataFromTileObject(item.Value);
            mapData.TileList.Add(serData);
        }

        return mapData;
    }

    public void DataToTiles(MapData mapData)
    {
        SetMapData(mapData);
    }

    public override void ReplaceTile(TileInfo tileInfoOld, TileInfo tileInfo)
    {
        if (tileInfoOld == null || tileInfo == null) return;

        Vector3Int vector3Int = grid.WorldToCell(tileInfoOld.transform.position);
        if (tiles.ContainsKey(vector3Int))
        {
            tileInfo.transform.SetParent(grid.transform);
            tileInfo.transform.position = tileInfoOld.transform.position;
            tileInfo.transform.rotation = tileInfoOld.transform.rotation;
            tileInfo.transform.localScale = tileInfoOld.transform.localScale;

            for (int i = 0;i < tileInfoOld.transform.childCount; i++)
            {
                tileInfo.transform.GetChild(i).gameObject.SetActive(tileInfoOld.transform.GetChild(i).gameObject.activeSelf);
            }

            tiles[vector3Int] = tileInfo;

            tileInfo.baseData = tileInfoOld.baseData;
            tileInfo.baseData.go = tileInfo.gameObject;
            tileInfo.gameId = tileInfoOld.gameId;
        }
    }


    public Vector3 GetTilesListCore(int gameId)
    {
        Transform l1, l2;
        Transform tile = null;
        foreach (var item in tiles)
        {
            if (item.Value.gameId == gameId)
            {
                l1 = item.Value.transform.GetChild(1);
                l2 = item.Value.transform.GetChild(2);
                if (l1.gameObject.activeSelf && l2.gameObject.activeSelf)
                {
                    if (tile == null)
                    {
                        tile = item.Value.transform;
                    }
                    else
                    {
                        if(item.Value.transform.position.x > tile.position.x || item.Value.transform.position.z < tile.position.z)
                        {
                            tile = item.Value.transform;
                        }
                    }
                }
            }
        }

        if(tile != null)
        {
            var ret = tile.GetChild(1).position;
            ret.z -= grid.cellSize.y / 2;
            return ret;
        }

        return Vector3.zero;
    }
}



#if UNITY_EDITOR
[CustomEditor(typeof(MapGridBarrier))]
public class GameGridBarrierEditor : GameGridEditor
{

}
#endif
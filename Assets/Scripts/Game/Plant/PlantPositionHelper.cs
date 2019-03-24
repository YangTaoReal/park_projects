using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlantPositionHelper {

    public class PlantInfo
    {
        public int size;
        public Vector2 pos;
    }

    class TileUnit{
        
        public Vector2 pos;
        public int size;
        public List<PlantInfo> plantInfos;

        public TileUnit(Vector2 p){
            pos = p;
            size = 0;
            plantInfos = new List<PlantInfo>();
        }

        void InitRect(){
            
            if (rects != null) return;

            rects = new List<KeyValuePair<Rect, int>>();
            float _x = pos.x - 1.5f;
            float _y = pos.y - 1.5f;
            float x, y, w, h;
            w = 1;
            h = 1;
            for (int i = 0; i < 3; i++)
            {
                x = _x + i;
                for (int j = 0; j < 3; j++)
                {
                    y = _y + j;
                    var rect = new Rect(x, y, w, h);
                    int cnt = 0;
                    foreach (var plant in plantInfos)
                    {
                        if (rect.Contains(plant.pos))
                        {
                            cnt++;
                        }
                    }
                    rects.Add(new KeyValuePair<Rect, int>(rect, cnt));
                }
            }
        }

        List<KeyValuePair<Rect, int>> rects;

        public void AddNew(GameObject gameObject, int _size, float r = 0)
        {
            if(rects == null){
                InitRect();
            }

            int tempCnt = int.MaxValue;
            int rectIdx = 0;
            Vector2 tempMidPos = Vector2.zero;
            for (int idx = 0; idx < rects.Count; idx++){
                if(rects[idx].Value < tempCnt){
                    tempCnt = rects[idx].Value;
                }
                tempMidPos += rects[idx].Key.center;
            }
            tempMidPos = tempMidPos / rects.Count;

            float curDst = float.MaxValue;
            float tempDst = 0;
            for (int idx = 0; idx < rects.Count; idx++)
            {
                if (rects[idx].Value <= tempCnt)
                {
                    tempDst = Vector2.Distance(rects[idx].Key.center, tempMidPos);
                    if (tempDst < curDst)
                    {
                        rectIdx = idx;
                        curDst = tempDst;
                    }
                }
            }

            var rectTemp = rects[rectIdx].Key;
            var posTemp = new Vector3(Random.Range(rectTemp.xMin, rectTemp.xMax), 0, Random.Range(rectTemp.yMin, rectTemp.yMax));
            gameObject.transform.position = posTemp;

            rects[rectIdx] = new KeyValuePair<Rect, int>(rectTemp, tempCnt + 1);
            size += _size;

            var plantInfo = new PlantInfo();
            plantInfo.size = _size;
            plantInfo.pos = new Vector2(posTemp.x, posTemp.z);
            plantInfos.Add(plantInfo);
        }
    }

    List<TileUnit> tileUnits;
    int sizeTotal;
    int maxSize;

    //初始化 (园区地块，园区容量，已有植物信息)
    public void Init(List<Vector2> tiles, int _maxSize, List<PlantInfo> plantInfos = null)
    {
        if (tiles == null || tiles.Count == 0) return;

        var list = new List<TileUnit>();
        for (int i = 0; i < tiles.Count; i++){
            list.Add(new TileUnit(tiles[i]));
        }

        tileUnits = list;

        maxSize = _maxSize;
        sizeTotal = 0;

        if(plantInfos != null && plantInfos.Count > 0)
        {
            var rect = new Rect();
            rect.width = 3;
            rect.height = 3;

            for (int i = 0; i < tileUnits.Count && plantInfos.Count > 0; i++)
            {
                rect.center = tileUnits[i].pos;

                for (int j = plantInfos.Count-1; j >= 0; j--)
                {
                    var go = plantInfos[j];
                    if (rect.Contains(go.pos))
                    {
                        tileUnits[i].plantInfos.Add(go);
                        tileUnits[i].size += go.size;
                        sizeTotal += go.size;
                        plantInfos.RemoveAt(j);
                    }
                }
            }
        }
    }

    //可不可以继续添加植物
    public bool CanSet(int size) { return (maxSize - sizeTotal) >= size; }

    //添加植物
    public bool Add(GameObject gameObject, int size, float r = 0){
        if (!CanSet(size)) return false;

        var tile = FindTileUnit(size);
        if (tile == null) return false;

        tile.AddNew(gameObject, size, r);
        sizeTotal += size;

        return true;
    }

    TileUnit FindTileUnit(int size){
        int idx = -1;
        int tempSize = int.MaxValue;
        Vector2 tempMidPos = Vector2.zero;
        for (int i = 0; i < tileUnits.Count; i++)
        {
            if(tileUnits[i].size < tempSize){
                idx = i;
                tempSize = tileUnits[i].size;
            }

            tempMidPos += tileUnits[i].pos;
        }
        tempMidPos = tempMidPos / tileUnits.Count;

        if (idx == -1) return null;

        int tempIdx = 0;
        float curDst = float.MaxValue;
        float tempDst = 0;
        for (int i = 0; i < tileUnits.Count; i++)
        {
            if (tileUnits[i].size <= tempSize)
            {
                tempDst = Vector2.Distance(tileUnits[i].pos, tempMidPos);
                if(tempDst < curDst)
                {
                    tempIdx = i;
                    curDst = tempDst;
                }
            }
        }

        return tileUnits[tempIdx];
    }
}


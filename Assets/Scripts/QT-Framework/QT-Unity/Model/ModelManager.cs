using QTFramework;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Reflection;

[ObjectEventSystem]
public class ModelManagerAwakeSystem : AAwake<ModelManager>
{
    public override void Awake(ModelManager _self)
    {
        _self.Awake();
    }
}
//[ObjectEventSystem]
//public class ActorManagerFixedUpdateSystem : AFixedUpdate<ActorManager>
//{
//    public override void FixedUpdate(ActorManager _self)
//    {
//        _self.FixedUpdate();
//    }
//}




public class BaseData{
    public GameObject go;
    public CS_Model.DataEntry cfg;//配置表
    public Guid guid;


    public T GetComponent<T>() where T : Component
    {
        T com = go.GetComponent<T>();
        if(com == null)
        {
            com = go.AddComponent<T>();
        }
        return com as T;
    }

}



public class ModelManager : QTComponent
{
    public List<Guid> listHaveServer = new List<Guid>();//从服务器得来的数据
    Dictionary<Guid, Park> listPark = new Dictionary<Guid, Park>();//园区列表
    Dictionary<Guid, BaseData> listModel = new Dictionary<Guid, BaseData>();

    public List<StartBuildingEven> listReMake = new List<StartBuildingEven>();//重新需要处理的东西

    public static ModelManager _instance = null;
    public Assistant assistant;
 
    public void Awake()
    {
        _instance = this;
        listModel.Clear();
        listReMake.Clear();
        listPark.Clear();
 
    }

    //public void FixedUpdate()
    //{

    //}


    void PreLoad(string path, int count = 1)
    {
        AssetPoolManager.Instance.PreLoad(path, count);
    }
     

    public GameObject ReplaceModel(BaseData baseData)
    {
        CS_Model.DataEntry dataEntry = DBManager.Instance.m_kModel.GetEntryPtr(baseData.cfg._ID);
        BaseData _baseData = new BaseData();
        _baseData.cfg = dataEntry;
        _baseData.guid = baseData.guid;

        GameObject go = AssetPoolManager.Instance.Fetch<GameObject>(dataEntry._Path);
        go.transform.parent = baseData.go.transform.parent;
        go.transform.localScale = dataEntry._Scale;
        go.transform.position = baseData.go.transform.position;
        go.transform.eulerAngles = baseData.go.transform.eulerAngles;

   
        _baseData.go = go;
        GameObject.Destroy(baseData.go);
        listModel[_baseData.guid] = _baseData;
        return go;
   
        //CS_Model.DataEntry dataEntry = DBManager.Instance.m_kModel.GetEntryPtr(cfg_id);
        //GameObject gameObject = AssetPoolManager.Instance.Fetch<GameObject>(dataEntry._Path);
        //BaseData baseData = new BaseData();
        //baseData.guid = guid;
        //baseData.go = gameObject;
        //baseData.go.transform.localScale = dataEntry._Scale;
        //baseData.cfg = dataEntry;
        //listModel[guid] = baseData;
    }


    /// <summary>
    ///  加载模型
    /// </summary>
    /// <returns>The load.</returns>
    /// <param name="cfg_id">Cfg identifier.</param>
    /// <param name="father">Father.</param>
    /// <param name="_guid">GUID.</param>
    /// <param name="bisSaveData">If set to <c>true</c> bis save data.</param>
    public BaseData Load(int cfg_id, Transform father = null, Guid _guid = default(Guid), bool bisSaveData = true)
    {
        CS_Model.DataEntry dataEntry = DBManager.Instance.m_kModel.GetEntryPtr(cfg_id);
        if (dataEntry == null)
        {
            Log.Error("~~Model配置表里面没有这个东西~~ ID: " + cfg_id);
            return null;
        }

        GameObject gameObject = AssetPoolManager.Instance.Fetch<GameObject>(dataEntry._Path);

        //if (cfg_id == 20501)
        //{
        //    Debug.Log("__@@@@__guid__" + _guid);
        //}
        //bool isNew;
        Guid guid;
        if (_guid != default(Guid) && _guid != Guid.Empty)
        {
            guid = _guid;
            //isNew = false;
        }
        else
        {
            guid = GenerateID.ID;
            //isNew = true;
        }
            
 
        BaseData baseData = new BaseData();
        baseData.guid = guid;
        baseData.go = gameObject;
        baseData.go.transform.localScale = dataEntry._Scale;
        baseData.go.transform.localEulerAngles = dataEntry._Rotate;
        baseData.cfg = dataEntry;
        listModel[guid] = baseData;


        if (MapGridMgr.IsWastedland(baseData.cfg._ID))
            DataManager._instance.wasteland.AddBaseData(baseData);



        //if (dataEntry._Ctype == (int)ModleType.MainBase && isNew) 
        //{
        //    bisSaveData = true;
        //    DataManager._instance.ChangeMapData(baseData.cfg._ID);
        //}

        //if (cfg_id == 20501)
        //Debug.Log("__guid___" + guid + "___bisSaveData_" + bisSaveData + "__cfg_id__" + cfg_id);
        GrassAssignmentData(baseData);
        if (bisSaveData)
        {
            if (IsHaveBuildData(baseData.cfg._ID))
                AssignmentData(baseData, bisSaveData);

            if ( MapGridMgr.IsBarrier(baseData.cfg._ID))
            {
                ParkServer PKserver = DataManager._instance.GetLoalData<ParkServer>(GetParkServerGuid(baseData.guid));
                if (PKserver != null)
                {
                    Park park = GetParkList(new List<Guid>() { baseData.guid });
                    if (park == null)
                    {
                        Park _park = new Park();
                        _park.Init(PKserver);
                        _park.AddChild(baseData);
                        listPark.Add(Guid.Parse(PKserver.guid), _park);
                    }
                    else
                    {
                        park.AddChild(baseData);
                    }
                }
            }
        }

        gameObject.transform.SetParent(father == null ? World.Scene.GetComponent<WorldManagerComponent>().m_kActorNode.transform : father);
        return listModel[guid];
    }

    bool isCome = false;
    void GrassAssignmentData(BaseData baseData)
    {
        baseData.GetComponent<ModelBase>().Init(baseData);
        if (!isCome && MapGridMgr.IsGrass(baseData.cfg._ID))//开荒
        {
            var llguids = World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.m_kPlayerBasicAsset.m_lWasteLand;
            if (llguids.Count > 0)
            {
                isCome = true;
                StartBuildingEven even = new StartBuildingEven();
                even.bIs = false;
                even.ReTyp = 2;
                even.guids = llguids;
                even.isReMake = true;//重新开始加载
                listReMake.Add(even);
            }
        }
    }

    //赋值数据
    void AssignmentData(BaseData baseData, bool bisSaveData)
    {
        baseData.GetComponent<ModelBase>().Init(baseData);
 
        if (baseData.cfg._Type == (int)ModeTyp.Building)
        { 
            BuildingServer _bs = baseData.GetComponent<Building>().GetServer;
            if (_bs != null)
            {
                listHaveServer.Add(baseData.guid);
                if (_bs.buildState == BuildState.Sustain || _bs.buildState == BuildState.UpLv)
                {
                    StartBuildingEven even = new StartBuildingEven();
                    even.bIs = true;
                    if (_bs.buildState == BuildState.Sustain)
                        even.ReTyp = 1;
                    else if (_bs.buildState == BuildState.UpLv)
                        even.ReTyp = 3;
                    List<Guid> guids = new List<Guid>();
                    guids.Add(baseData.guid);
                    even.guids = guids;
                    even.isReMake = true;//重新开始加载
                    listReMake.Add(even);
                }
            }   
 
        }
        if (baseData.cfg._Type == (int)ModeTyp.Animal)
        {
            Animal animal = baseData.GetComponent<Animal>();
            AnimalServer _as = animal.GetServer;
            if (_as != null && !string.IsNullOrEmpty(_as.father_guid))
            {
                Park park = GetParkByFaterGuid(Guid.Parse(_as.father_guid));
                if(park != null)
                {
                    park.AddItem(baseData);
                    var listPos = park.GetParkListPos();
                    animal.animalMove.SetArea(listPos);
                    listHaveServer.Add(baseData.guid); 
                }
            }
        }

        else if (baseData.cfg._Type == (int)ModeTyp.Plant)
        {
            Plant plant = baseData.GetComponent<Plant>();
            PlantServer _ps = plant.GetServer;
            if (_ps != null && !string.IsNullOrEmpty(_ps.father_guid))
            {
                Park park = GetParkByFaterGuid(Guid.Parse(_ps.father_guid));
                if (park != null)
                {
                    park.AddItem(baseData);
                    baseData.go.transform.position = new Vector3(_ps.pos[0], 0, _ps.pos[1]);
                    listHaveServer.Add(baseData.guid);
                }
            }
        }

   
        SetParkItemChild(baseData);

        if (MapGridMgr.IsWastedland(baseData.cfg._ID))
            bisSaveData = false; 

      

        if (bisSaveData && DataManager._instance != null)
        {
            DataManager._instance.AddLocalData(baseData);
        }

    }

    void SetParkItemChild(BaseData baseData)
    {
       if (baseData.cfg._Type == (int)ModeTyp.Animal)
        {
            AnimalServer _bs = baseData.GetComponent<Animal>().GetServer;
            if (_bs.father_guid != "" && _bs.placeState == PlaceState.Park)
            {
                AddParkItemChild(baseData, Guid.Parse(_bs.father_guid));
            }
        }
        else if (baseData.cfg._Type == (int)ModeTyp.Plant)
        {
            PlantServer _bs = baseData.GetComponent<Plant>().GetServer;
            if (_bs.father_guid != "" && _bs.placeState == PlaceState.Park)
            {
                AddParkItemChild(baseData, Guid.Parse(_bs.father_guid));
            }
        }
   
    }
 
    void AddParkItemChild(BaseData baseData, Guid father_guid)
    {
        if (!listPark.ContainsKey(father_guid))
        {
            Debug.Log("__没有这个园区___" + father_guid);
            return;
        }
            
        Park park = listPark[father_guid] as Park;
        park.AddItem(baseData);
    }


    //guid是篱笆子节点的
    public Park GetParkList(List<Guid> guids)
    {
        foreach (var park in listPark)
        {
            for (int i = 0; i < guids.Count; i++)
            {
                if (park.Value.GetServer.listGuids.Contains(guids[i].ToString()))
                    return park.Value;  
                //if(park.Value.dicChild.ContainsKey(guids[i]))
                    //return park.Value;      
            }
          
        }
        return null;
    }

    public Park GetParkByFaterGuid(Guid father_guid)
    {
        if (listPark.ContainsKey(father_guid))
            return listPark[father_guid];
        return null;
    } 

    //guid是篱笆子节点的
    public Guid GetParkServerGuid(Guid guid)
    {
        Dictionary<Guid, ParkServer> dicPark = DataManager._instance.GetAllData<ParkServer>();
        foreach (var park in dicPark)
        {
            if(park.Value.listGuids.Contains(guid.ToString()))
            {
                return Guid.Parse(park.Value.guid);
            }
        }
        return Guid.Empty;
 
    }

    //创建助手
    public Assistant CreateAssistant()
    {
        BaseData data = Load(50003);
        GameObject house = GetModleByType(ModelCType.MainBase)[0].go;
        data.go.transform.position = new Vector3(house.transform.position.x, 0, house.transform.position.z);
        assistant = data.GetComponent<Assistant>();
        assistant.Init();
        assistant.CreateUIBar();

        return assistant;
    }

    //public void AssistantWork()
    //{
    //    assistant.cbMove = () =>
    //    {
    //        assistant.Weed(5);
    //    };
    //    assistant.Move(SceneLogic._instance.selectTileInfo.gameObject);
    //    assistant.cbWeed = () =>
    //    {
    //        GameObject house = GetModleByType(ModleType.MainBase)[0].go;
    //        assistant.Move(house);
    //    };
    //}


    public Park CreatePark(BaseData baseData)
    {
        Park park = new Park();
        park.Init();
        park.AddChild(baseData);
        listPark.Add(Guid.Parse(park.GetServer.guid), park);
        //DataManager._instance.AddLocalData<ParkServer>(park.GetServer.guid, park.GetServer);

     
        UIEntity uien = World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPopUpWindow_BuildingName);
        uien.GetComponent<UIPopUpWindow_BuildingNameComponent>().Init(park);

        //CheckGuidance(uien);
        TimerUtil.SetTimeOut(0.1f, () => {

            App.Instance.StartCoroutine(DataManager._instance.SaveJsonServer());// 购买成功  手动存储数据
        });
        GuidanceManager._Instance.CheckGuidance(uien);
        return park;
    }

    //public void CheckGuidance(UIEntity entity)
    //{
    //    if(GuidanceManager.isGuidancing)
    //    {
    //        TimerUtil.SetTimeOut(0.1f, () => { 
            
    //            App.Instance.StartCoroutine(DataManager._instance.SaveJsonServer());// 购买成功  手动存储数据
    //        });

    //        GuidanceData data = new GuidanceData();
    //        data.entity = entity;
    //        ObserverHelper<GuidanceData>.SendMessage(MessageMonitorType.GuidanceClickEvent, this, new MessageArgs<GuidanceData>(data));
    //    }
    //}

    /// <summary>
    /// Sets the park.
    /// </summary>
    /// <param name="guid">随便传一个选中园区有的篱笆guid.</param>
    /// <param name="child_guid">Child GUID.</param>
    /// <param name="tpy">1是增加, 2是删除.</param>
    public void SetPark(Guid guid, BaseData baseData, int tpy = 1)
    {
       
        Park park = GetParkList(new List<Guid>() { guid });
        if(tpy == 1)
        {
            if(park == null)
            {
                Park _park = new Park();
                _park.Init();
                _park.AddChild(baseData);
                listPark.Add(Guid.Parse(_park.GetServer.guid), _park);
                //DataManager._instance.AddLocalData<ParkServer>(_park.GetServer.guid, _park.GetServer);
            }
            else
            {
                park.AddChild(baseData);
                //DataManager._instance.FixLocalData<ParkServer>(park.GetServer.guid, park.GetServer);
            }
        }
        else if(tpy == 2)
        {
            if (park == null)
            {
                Debug.LogError("__删除篱笆__" + baseData.guid + "__时, 没有找到对应的园区__" + guid);
                return;
            }
            park.DelChild(baseData);

           
            //if (Json.GetStringCount(park.GetServer.listGuids) == 0)
            //{
            //    listPark.Remove(Guid.Parse(park.GetServer.guid));
            //    DataManager._instance.DelLocalData<ParkServer>(park.GetServer.guid);
            //}
            //else
            //{
            //    DataManager._instance.FixLocalData<ParkServer>(park.GetServer.guid, park.GetServer);
            //}    


        }
    }


    public Park isCreateItem(int cid)
    {
        if (SceneLogic._instance.selectTileInfo == null || !MapGridMgr.IsBarrier(SceneLogic._instance.selectTileInfo.prefabId))
        { 
            Debug.LogError("__请先选中一个园区___");
            return null;
        }
        CS_Model.DataEntry dataEntry = DBManager.Instance.m_kModel.GetEntryPtr(cid);
        if (dataEntry._Type != (int)ModelCType.Animal && dataEntry._Type != (int)ModelCType.Plant)
        {
            Debug.LogError("__这个东西不是animal或者plant___" + cid);
            return null;
        }
 
        Park park = GetParkList(new List<Guid>() { SceneLogic._instance.selectTileInfo.guid });
        if(park == null)
        {   Debug.LogError("__数据里面没有这个园区___" + SceneLogic._instance.selectTileInfo.guid);
            return null;
        }
        int Remaining = park.GetRemainingCapacity();
        if (Remaining < dataEntry._Capacity)
        {
            Debug.LogError("__所选园区容量不足___");
            return null;
        }

        return park;
    }

    //选中建筑物升级
    public void SelectLvUp()
    {
        if (SceneLogic._instance.selectTileInfo == null)
        {
            Debug.LogError("__请先选中一个物体___");
            return;
        }
        if(DBManager.Instance.m_kUpLv.GetEntryPtr(SceneLogic._instance.selectTileInfo.prefabId)._NextID == 0)
        {
            Debug.LogError("__建筑满级了___");
            return;
        }
      
        StartBuildingEven even = new StartBuildingEven();
        even.bIs = false;
        even.ReTyp = 3;
        even.isReMake = false;//重新开始加载
        even.guids = new List<Guid>();
        //园区
        if(MapGridMgr.IsBarrier(SceneLogic._instance.selectTileInfo.prefabId))
        {
            Park park = GetParkList(new List<Guid>() { SceneLogic._instance.selectTileInfo.guid });
            foreach(var child in park.dicChild)
            {
                even.guids.Add(child.Key);
            }
        }
        else
        {
            even.guids.Add(SceneLogic._instance.selectTileInfo.guid);
        }
  
        ObserverHelper<StartBuildingEven>.SendMessage(MessageMonitorType.StartUpLv, this, new MessageArgs<StartBuildingEven>(even));
        MapGridMgr.Instance.UnFoucs();
    }


    //创建动物
    public bool CreateAnimalPark(int cid, BaseData _baseData = null)
    {
        Park park = isCreateItem(cid);
        if (park == null) return false;
        BaseData baseData;
        if (_baseData != null)
            baseData = _baseData;
        else
            baseData = Load(cid);
        var list_pos = park.GetParkListPos();
        park.AddItem(baseData);

        AnimalServer animServer = baseData.GetComponent<Animal>().GetServer;
        animServer.father_guid = park.GetServer.guid;
        animServer.placeState = PlaceState.Park;
        DataManager._instance.FixLocalData<AnimalServer>(animServer.guid, animServer);

        baseData.GetComponent<Animal>().BeginMove(list_pos);
        return true;
    }

    //种树
    public bool CreatePlantPark(int cid, BaseData _baseData = null)
    {
        Park park = isCreateItem(cid);
        if (park == null) return false;
        BaseData baseData;
        if (_baseData != null)
            baseData = _baseData;
        else
            baseData = Load(cid);   
        PlantServer plantServer = baseData.GetComponent<Plant>().GetServer;
        var list_plant = park.listItemPark[(int)ModelCType.Plant];
        PlantPositionHelper helper = new PlantPositionHelper();
        List<Vector2> tiles = new List<Vector2>();
        var list_pos = park.GetParkListPos();
        var max = park.GetParkCapacity();
        foreach (var pos in list_pos)
        {
            tiles.Add(new Vector2(pos.x, pos.z));
        }

        if (list_plant.Count == 0)
        {
            List<PlantPositionHelper.PlantInfo> plantInfos = new List<PlantPositionHelper.PlantInfo>();
            helper.Init(tiles, max);
        }
        else
        {
            List<PlantPositionHelper.PlantInfo> list_temp = new List<PlantPositionHelper.PlantInfo>();
            foreach (var plant in list_plant)
            {
                PlantServer _plantServer = plant.Value.GetComponent<Plant>().GetServer;
                PlantPositionHelper.PlantInfo plantInfos = new PlantPositionHelper.PlantInfo();
                plantInfos.pos = new Vector2(_plantServer.pos[0], _plantServer.pos[1]);
                plantInfos.size = plant.Value.cfg._Capacity;
                list_temp.Add(plantInfos);
            }
            helper.Init(tiles, max, list_temp);
            helper.Add(baseData.go, baseData.cfg._Capacity);
            baseData.GetComponent<Plant>().SetPos(baseData.go.transform.position.x, baseData.go.transform.position.z);
        }

        plantServer.father_guid = park.GetServer.guid;
        plantServer.placeState = PlaceState.Park;
        DataManager._instance.FixLocalData<PlantServer>(plantServer.guid, plantServer);
        park.AddItem(baseData);
        return true;
    }

    //还原物体
    public void Restore()
    {
        List<StartBuildingEven> temp_build = new List<StartBuildingEven>();
        List<StartBuildingEven> temp_wasteland = new List<StartBuildingEven>();
        for (int i = 0; i < listReMake.Count; i++)
        {
            StartBuildingEven even = listReMake[i];
            if(even.ReTyp == 1)
                ObserverHelper<StartBuildingEven>.SendMessage(MessageMonitorType.StartBuilding, this, new MessageArgs<StartBuildingEven>(even));
            else if(even.ReTyp == 2)
                ObserverHelper<StartBuildingEven>.SendMessage(MessageMonitorType.StartWastedland, this, new MessageArgs<StartBuildingEven>(even));
            else if (even.ReTyp == 3)
                ObserverHelper<StartBuildingEven>.SendMessage(MessageMonitorType.StartUpLv, this, new MessageArgs<StartBuildingEven>(even));
        }
        listReMake.Clear();
 

        List<Guid> listSettlement = World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.m_kSettlementsAsset.m_kListSettlement;
        var dicAnimal = DataManager._instance.GetAllData<AnimalServer>();
        foreach(var animal in dicAnimal)
        {
            BaseData baseData = Load(animal.Value.cfg_id, null, Guid.Parse(animal.Value.guid));
            PlaceState place = baseData.GetComponent<Animal>().GetServer.placeState;
            if(place == PlaceState.House)//在安置点
            {
                baseData.go.SetActive(false);
            }
            else if(place == PlaceState.Move)
            {
                baseData.go.SetActive(true);
                baseData.go.transform.position = GenerateID.HidePos;
            }
            else
            {
                if(animal.Value.father_guid != "")
                {
                    baseData.go.SetActive(true);
                    //Debug.Log("__111__" + animal.Value.father_guid);
                    //Debug.Log("__222__" + listPark[Guid.Parse(animal.Value.father_guid)].GetServer.listGuids);
                    string str_guid = Json.GetStringByIdx(listPark[Guid.Parse(animal.Value.father_guid)].GetServer.listGuids, 0);
                    Park park = GetParkList(new List<Guid>() { Guid.Parse(str_guid) });
                    var listPos = park.GetParkListPos();
                    baseData.go.transform.position = listPos[0];
                    baseData.GetComponent<Animal>().BeginMove(listPos);  
                }

            }
        }

        var dicPlant = DataManager._instance.GetAllData<PlantServer>();
        foreach (var plant in dicPlant)
        {
            BaseData baseData = Load(plant.Value.cfg_id, null, Guid.Parse(plant.Value.guid));
            PlaceState place = baseData.GetComponent<Plant>().GetServer.placeState;
            if (place == PlaceState.House)//在安置点
            {
                baseData.go.SetActive(false);
            }
            else if (place == PlaceState.Move)
            {
                baseData.go.SetActive(true);
                baseData.go.transform.position = GenerateID.HidePos;
            }
            else
            {
                baseData.go.SetActive(true);
                baseData.go.transform.position = new Vector3(plant.Value.pos[0], 0, plant.Value.pos[1]);
            }
 
        }
     
    
    }



    public List<BaseData> GetModleByType(ModelCType type)
    {
        List<BaseData> listBaseDatas = new List<BaseData>();
        foreach(var model in listModel)
        {
            if(model.Value.cfg._Ctype == (int)type)
            {
                listBaseDatas.Add(model.Value);
            }
        }
        return listBaseDatas;
    }



    public BaseData GetModelByGuid(Guid guid)
    {
        if (!listModel.ContainsKey(guid))
        {
            Log.Error("对象不存在 :" + guid);
            return null;
        }
 
        return listModel[guid];
    }



    /// <summary>
    /// 回收
    /// </summary>
    public void RecycleByGuid(Guid guid)
    {
        if (!listModel.ContainsKey(guid))
            return;

        BaseData baseData = GetModelByGuid(guid);
        AssetPoolManager.Instance.Recycle(baseData.go);
        listModel.Remove(guid);


        if (MapGridMgr.IsBarrier(baseData.cfg._ID))
            SetPark(baseData.guid, baseData, 2);
   

        DataManager._instance.ChangeMapData(baseData.cfg._ID);

       

        if (baseData.cfg._Type == (int)ModeTyp.Building)//建筑
        {
            if (IsHaveBuildData(baseData.cfg._ID))
                DataManager._instance.DelLocalData(baseData);
        }
        else  
            DataManager._instance.DelLocalData(baseData);
    
 
    }

 
    public bool IsHaveBuildData(int cid)
    {
        CS_Model.DataEntry dataEntry = DBManager.Instance.m_kModel.GetEntryPtr(cid);
        if(dataEntry._Type == (int)ModeTyp.Building)//建筑
        {
            ModelCType _Ctype = (ModelCType)dataEntry._Ctype;
            //不存的东西  地基,园区牌子,荒地牌子,荒地
            if( _Ctype == ModelCType.Subgrade || _Ctype == ModelCType.ParkSign || _Ctype == ModelCType.WastedSign || _Ctype == ModelCType.Wastedland)
            {
                return false;
            }
        }
        return true;
    }

    void OnlyRemveModel(GameObject go)
    {
        AssetPoolManager.Instance.Recycle(go);
    }



    public Sprite GetAssetIcon(int _itemType)
    {
        PlayerBagAsset.ItemType itemType = (PlayerBagAsset.ItemType)_itemType;
        string strIcon = "";
        if (itemType == PlayerBagAsset.ItemType.Gold)
            strIcon = "gold";
        else if (itemType == PlayerBagAsset.ItemType.Stone)
            strIcon = "crystal";
        else if (itemType == PlayerBagAsset.ItemType.Sun)
            strIcon = "bg_tq01";
        else if (itemType == PlayerBagAsset.ItemType.Water)
            strIcon = "water_icon";
     
        return UI_Helper.GetSprite(strIcon);
    }


}

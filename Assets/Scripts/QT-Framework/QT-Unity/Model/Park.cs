using System.Collections.Generic;
using UnityEngine;
using System;
 
namespace QTFramework
{
    public enum ParkType
    {
        Idle = 1,
        Move = 2,//移动
        Sustain = 3,//扩建
        Uplv = 4,//升级
    }

    [System.Serializable]
    public class ParkServer
    {
        public string guid;
        public string listItems;//动植物
        public string listGuids;//篱笆
        public string name;
        public ParkType parkType;
        public string MoveTime;
    }
  

    public class Park  
    {
 
        public BaseData parkSign;
        public BarsMgr uiBar;
        int lv;

        public Dictionary<int, Dictionary<Guid, BaseData>> listItemPark;//园区内的所有东西
        public Dictionary<Guid, BaseData> dicChild;//园区内篱笆管理
        ParkServer Server;
        public ParkServer GetServer
        {
            get { return Server; }
        }
        public ParkType ParkType
        {
            get { return Server.parkType; }
            set { Server.parkType = value; }
        }

 

        public void Init(ParkServer _server = null)
        {
           
            Server = new ParkServer();   
            dicChild = new Dictionary<Guid, BaseData>();
            listItemPark = new Dictionary<int, Dictionary<Guid, BaseData>>();
            listItemPark.Add((int)ModelCType.Animal, new Dictionary<Guid, BaseData>());
            listItemPark.Add((int)ModelCType.Plant, new Dictionary<Guid, BaseData>());
            parkSign = null;
            if(_server != null)
            {
                Server = _server;
            }
            else
            {
                Server.guid = GenerateID.ID.ToString();
                Server.listGuids = "";
                Server.listItems = "";
                Server.name = "";
                Server.parkType = ParkType.Idle;
                Server.MoveTime = DateTime.MaxValue.ToString();
                lv = 1;
            }
        }
 

        public void SetName(string name)
        {
            Server.name = name;
            SetNameSign(name);
            DataManager._instance.FixLocalData<ParkServer>(Server.guid, Server);
        }

        public void SetNameSign(string name)
        {
            parkSign.go.transform.Find("Text").GetComponent<TextMesh>().text = name;
        }


 

        //添加篱笆
        public void AddChild(BaseData data)
        {
            if (parkSign == null)
            {
                //parkSign = ModelManager._instance.Load(20003, data.go.transform);
                //parkSign.go.transform.localPosition = Vector3.zero;
                parkSign = ModelManager._instance.Load(20003);
                World.Scene.GetComponent<TimeComponent>().CreateTimer(500, 1, 1, () =>
                {
                    SetNameSign(Server.name);
                    parkSign.go.transform.SetParent(World.Scene.GetComponent<WorldManagerComponent>().m_kActorNode.transform);
                    parkSign.go.transform.position = MapGridMgr.Instance.Barrier.GetTilesListCore(data.go.GetComponent<TileInfo>().gameId);
                });
            }

            if (Server.parkType == ParkType.Uplv && data.GetComponent<Building>().GetServer.buildState == BuildState.Idle)
                Server.parkType = ParkType.Idle;

            lv = data.GetComponent<Building>().GetServer.lv;
            if (!Server.listGuids.Contains(data.guid.ToString()))
            {
                Server.listGuids = Json.DealString(Server.listGuids, 1, data.guid.ToString());
                if(Json.GetStringCount(Server.listGuids) == 1)
                    DataManager._instance.AddLocalData<ParkServer>(Server.guid, Server);
                else
                    DataManager._instance.FixLocalData<ParkServer>(Server.guid, Server);
            }
     
            if (!dicChild.ContainsKey(data.guid))
            {
                parkSign.go.transform.position = MapGridMgr.Instance.Barrier.GetTilesListCore(data.go.GetComponent<TileInfo>().gameId);
                dicChild.Add(data.guid, data);    
            }
                 

  
        }
        //删除篱笆
        public void DelChild(BaseData data)
        {
            if(Server.listGuids.Contains(data.guid.ToString()))
            {
                Server.listGuids = Json.DealString(Server.listGuids, 2, data.guid.ToString());
                if(string.IsNullOrEmpty(Server.listGuids))
                    DataManager._instance.DelLocalData<ParkServer>(Server.guid);
                else
                    DataManager._instance.FixLocalData<ParkServer>(Server.guid, Server);
            }
     
            if (dicChild.ContainsKey(data.guid))
            {
                dicChild.Remove(data.guid);
                if (dicChild.Count == 0)
                {
                    ModelManager._instance.RecycleByGuid(parkSign.guid);
                }
                else
                {
                    parkSign.go.transform.position = MapGridMgr.Instance.Barrier.GetTilesListCore(data.go.GetComponent<TileInfo>().gameId);
                    //foreach(var child in dicChild)
                    //{
                    //    parkSign.go.transform.position = child.Value.go.transform.position;
                    //    break;
                    //}
                }
            }


        }


 
        //添加内容(动物, 植物)
        public void AddItem(BaseData data)
        {
            //if (data.GetComponent<ModelBase>().PlaceState == PlaceState.House) return;
            //List<Guid> listSettlement = World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.m_kSettlementsAsset.m_kListSettlement;
            //if (listSettlement.Contains(data.guid)) return;
            if(!listItemPark.ContainsKey(data.cfg._Ctype)){ Debug.Log("__园区没有存这个类型___" + data.cfg._Ctype); return; }
            if(listItemPark[data.cfg._Ctype].ContainsKey(data.guid)){ Debug.Log("_添加操作_园区已经有这个数据了___" + data.guid); return; }
            listItemPark[data.cfg._Ctype].Add(data.guid, data);
 
            int count = GetAllItemGuid().Count;
            int nItem = Json.GetStringCount(Server.listItems);
            if(nItem != 0 && count != 0)
            {
                if(nItem == count)
                {
                    ParkMove();
                }
            }
            Server.listItems = Json.DealString(Server.listItems, 1, data.guid.ToString());
            DataManager._instance.FixLocalData<ParkServer>(Server.guid, Server);
        }

        //删除内容(动物, 植物)
        public void DelItem(BaseData data)
        {
            if (!listItemPark.ContainsKey(data.cfg._Ctype)){ Debug.Log("__园区没有存这个类型___" + data.cfg._Ctype); return; }
            if (!listItemPark[data.cfg._Ctype].ContainsKey(data.guid)){ Debug.Log("_删除操作_园区已经没有这个数据___" + data.guid); return; }

            listItemPark[data.cfg._Ctype].Remove(data.guid);

            Server.listItems = Json.DealString(Server.listItems, 2, data.guid.ToString());
            DataManager._instance.FixLocalData<ParkServer>(Server.guid, Server);
        }

        public void RemoveAllItem()
        {
            listItemPark[(int)ModeTyp.Animal].Clear();
            listItemPark[(int)ModeTyp.Plant].Clear();
            Server.listItems = "";
            DataManager._instance.FixLocalData<ParkServer>(Server.guid, Server);
        }

        //得到园区的容量
        public int GetParkCapacity()
        {
      
            int Capacity = 0;
            foreach (var item in dicChild)
            {
                Building building = item.Value.GetComponent<Building>();
                CS_UpLevel.DataEntry level = DBManager.Instance.m_kUpLv.GetEntryPtr(building.GetServer.cfg_id);
                Capacity = Capacity + int.Parse(level._Func.Split(' ')[1]);
            }
            return Capacity;
        }

        //得到园区的剩余容量
        public int GetRemainingCapacity()
        {
            int Capacity = GetParkCapacity();
            int uselCap = 0;
            foreach(var list in listItemPark)
            {
                foreach(var item in list.Value)
                {
                    uselCap = uselCap + item.Value.cfg._Capacity;
                }
            }
            return Capacity - uselCap;
        }

        //得到园区所有东西的guid
        public List<Guid> GetAllItemGuid()
        {
            List<Guid> temp_list = new List<Guid>();
            foreach (var list in listItemPark)
            {
                foreach (var item in list.Value)
                {
                    temp_list.Add(item.Value.guid);
                }
            }
            return temp_list;
        }


        //得到园区所有成熟东西的ID
        public List<int> GetAllStateItemID(GrowthState state)
        {
            List<int> temp_list = new List<int>();
            foreach (var list in listItemPark)
            {
                foreach (var item in list.Value)
                {
                    BaseServer baseServer = item.Value.GetComponent<ModelBase>().GetbaseServer;
                    if(state == baseServer.growthState)
                    {
                        temp_list.Add(item.Value.cfg._ID);    
                    }
                }
            }
            return temp_list;
        }
        //得到园区所有东西的ID
        public List<int> GetAllItemID()
        {
            List<int> temp_list = new List<int>();
            foreach (var list in listItemPark)
            {
                foreach (var item in list.Value)
                {
                    temp_list.Add(item.Value.cfg._ID);
                }
            }
            return temp_list;
        }

        //得到园区的list_pos
        public List<Vector3> GetParkListPos()
        {
            List<Vector3> listPos = new List<Vector3>();
            foreach (var item in dicChild)
            {
                listPos.Add(item.Value.go.transform.position);
            }
            return listPos;
        }


        public GameObject GetParkCenter()
        {
            return parkSign.go;
        }

        public void ParkMove()
        {
            List<int> parklist = GetAllItemID();
            if (parklist.Count == 0)
            {
                Debug.Log("___园区里面没有东西___");
                return;
            }
            if (Server.parkType != ParkType.Move) return;
            var lAnimal = listItemPark[(int)ModeTyp.Animal];
            var lPlant = listItemPark[(int)ModeTyp.Plant];
            foreach (var animal in lAnimal){ 
                animal.Value.go.transform.position = GenerateID.HidePos;
                animal.Value.go.GetComponent<Animal>().animalMove.enabled = false;
            }

            foreach (var plant in lPlant) { 
                plant.Value.go.transform.position = GenerateID.HidePos;
            }
            GameObject house = ModelManager._instance.GetModleByType(ModelCType.MainBase)[0].go;
            Vector3 pos = (GetParkCenter().transform.position + house.transform.position) / 2;
            Vector3 screenPos01 = Camera.main.WorldToScreenPoint(house.transform.position);
            Vector3 screenPos02 = Camera.main.WorldToScreenPoint(GetParkCenter().transform.position);
 
            //foreach (var animal in lAnimal) {  animal.Value.GetComponent<Animal>().GetServer.placeState = PlaceState.Move; }
            //foreach (var plant in lPlant) {  plant.Value.GetComponent<Plant>().GetServer.placeState = PlaceState.Move; }
 
            float distance = Vector2.Distance(new Vector2(house.transform.position.x, house.transform.position.z), new Vector2(GetParkCenter().transform.position.x, GetParkCenter().transform.position.z));
            Vector2 fixpos01, fixpos02;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(UI_Helper.UINode.transform as RectTransform, screenPos01, UI_Helper.UICamera, out fixpos01);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(UI_Helper.UINode.transform as RectTransform, screenPos02, UI_Helper.UICamera, out fixpos02);
            float parkMoveTime = GetParkCapacity() * distance / int.Parse(DBManager.Instance.m_kDisperse.GetEntryPtr(10000)._Val1);
            TimeSpan ts = DateTime.Now.Subtract(DateTime.Parse(Server.MoveTime));
            if (ts.TotalSeconds >= parkMoveTime * 60)
            {
                EndMove();
            }
            else
            {
                ParkBarsMgr parkBarsMgr = SceneLogic._instance.AddBar(BarType.park, pos, GetParkCenter(), house, null, this) as ParkBarsMgr;
                parkBarsMgr.bar.SetParkGoBegin(GetParkCenter());
                bool front;
                if (screenPos01.x < screenPos02.x)
                    front = true;
                else
                    front = false;
                parkBarsMgr.bar.InitPark(parklist[0], front, parkBarsMgr.park, distance);
            }

            
                

  
        }

        //迁徙开始
        public void BeginMove()
        {
            List<int> parklist = GetAllItemID();
            if (parklist.Count == 0)
            {
                Debug.Log("___园区里面没有东西___");
                return;
            }
            Server.parkType = ParkType.Move;
            Server.MoveTime = DateTime.Now.ToString();
            DataManager._instance.FixLocalData<ParkServer>(Server.guid, Server);


            var lAnimal = listItemPark[(int)ModeTyp.Animal];
            var lPlant = listItemPark[(int)ModeTyp.Plant];
            foreach (var animal in lAnimal)
            {
                AnimalServer animalServer = animal.Value.GetComponent<Animal>().GetServer;
                animalServer.placeState = PlaceState.Move;
                DataManager._instance.FixLocalData<AnimalServer>(animalServer.guid, animalServer);
            }
            foreach (var plant in lPlant)
            {
                PlantServer plantServer = plant.Value.GetComponent<Plant>().GetServer;
                plantServer.placeState = PlaceState.Move;
                DataManager._instance.FixLocalData<PlantServer>(plantServer.guid, plantServer);
            }

        }

        public void EndUpLevel()
        {
            
        }

        //迁徙结束
        public void EndMove()
        {
            Server.parkType = ParkType.Idle;
            Server.MoveTime = DateTime.MaxValue.ToString();
    
            DataManager._instance.FixLocalData<ParkServer>(Server.guid, Server);
 
            var lAnimal = listItemPark[(int)ModeTyp.Animal];
            var lPlant = listItemPark[(int)ModeTyp.Plant];
            foreach (var animal in lAnimal)
            {
                AnimalServer animalServer = animal.Value.GetComponent<Animal>().GetServer;
                animalServer.placeState = PlaceState.House;
                animalServer.father_guid = "";
                DataManager._instance.FixLocalData<AnimalServer>(animalServer.guid, animalServer);
                animal.Value.go.SetActive(false);
            }
            foreach (var plant in lPlant)
            {
                PlantServer plantServer = plant.Value.GetComponent<Plant>().GetServer;
                plantServer.placeState = PlaceState.House;
                plantServer.father_guid = "";
                DataManager._instance.FixLocalData<PlantServer>(plantServer.guid, plantServer);
                plant.Value.go.SetActive(false);
            }

       
            List<Guid> _objectList = GetAllItemGuid();
            Player player = World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer;
            for (int i = 0; i < _objectList.Count; i++)
            {
                player.AddToSettlements(_objectList[i]);
            }
            RemoveAllItem();
        }

    }

}


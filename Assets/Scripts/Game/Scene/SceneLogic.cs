using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;

using QTFramework;

using UnityEngine;

//[ObjectEventSystem]
//public class ParkSceneAwakeSystem : AAwake<ParkScene>
//{
//    public override void Awake(ParkScene _self)
//    {
//        _self.Awake();
//    }
//}

//[ObjectEventSystem]
//public class ParkSceneFixedUpdateSystem : AFixedUpdate<ParkScene>
//{
//    public override void FixedUpdate(ParkScene _self)
//    {
//        _self.FixedUpdate();
//    }
//}

public class StartBuildingEven
{
    public bool bIs;
    public int ReTyp; //1是建筑物新建. 2是开荒, 3是建筑物升级
    public bool isReMake; //重新开始加载
    public List<Guid> guids;
}

//处理场景逻辑的
public class SceneLogic : MonoBehaviour
{
    public class Dispose
    {
        public BaseData rootBuild;
        public BaseData subgrade;
        public Guid eff_guid;
        public DateTime BeginTime;
        public Guid fahter_guid;
        public BarsMgr barsMgr;
        public bool Enable; //是否激活
    }

    //荒地牌子
    public class WastedBoard
    {
        public Guid WastedGuid;
        public BaseData BoardbaseData;
        public int typReclaim; //1是等待开垦,2是正在
    }


    DataManager dataManager;
    ModelManager modelManager;
    EffectManagerComponent effectManager;
    TimeComponent timeComponent;

    public List<WastedBoard> listSelectWBoard = new List<WastedBoard>();

    public List<Dispose> listWasteland = new List<Dispose>();
    public List<List<Dispose>> listDispose = new List<List<Dispose>>();

    public List<BarsMgr> l_Bars = new List<BarsMgr>(); //所有的血条管理
    List<BarsMgr> l_ParkMoveBars = new List<BarsMgr>(); //园区迁徙

    string dataPath;

    public static SceneLogic _instance = null;
    public static SceneLogic GetInstance() { return _instance; }
    public bool isEnter;

    //List<CS_Items.DataEntry> lfodders = new List<CS_Items.DataEntry>();

    void Awake()
    {
        _instance = this;
        listSelectWBoard.Clear();
        listDispose.Clear();
        listWasteland.Clear();
        l_Bars.Clear();
        isEnter = false;
        dataPath = Application.dataPath;

        //lfodders.Clear();

        //foreach (var data in DBManager.Instance.m_kItems.m_kDataEntryTable)
        //{
        //    if (data.Value._ItemType == (int) PlayerBagAsset.ItemType.Nutrients || data.Value._ItemType == (int) PlayerBagAsset.ItemType.AnimaiMedicine ||
        //        data.Value._ItemType == (int) PlayerBagAsset.ItemType.BotanyMedicine)
        //    {
        //        lfodders.Add(data.Value);
        //    }
        //}

        ObserverHelper<StartBuildingEven>.AddEventListener(MessageMonitorType.StartUpLv, OnStartUpLv);
        ObserverHelper<StartBuildingEven>.AddEventListener(MessageMonitorType.StartBuilding, OnStartBuilding);
        ObserverHelper<List<Guid>>.AddEventListener(MessageMonitorType.RemoveBuilding, OnRemoveBuilding);
        ObserverHelper<StartBuildingEven>.AddEventListener(MessageMonitorType.StartWastedland, OnStartWastedland);
        ObserverHelper<TileInfo>.AddEventListener(MessageMonitorType.SelectedTileInfo, OnSelectedTileInfo);
        ObserverHelper<ModelBase>.AddEventListener(MessageMonitorType.SelectedModelInfo, OnSelectedModelInfo);

        MapGridMgr.Instance.onEndEdit = (bool isNewBuild, List<Guid> guids) =>
        {
            StartBuildingEven even = new StartBuildingEven();
            even.bIs = isNewBuild;
            even.guids = guids;
            even.ReTyp = 1;
            even.isReMake = false; //重新开始加载
            ObserverHelper<StartBuildingEven>.SendMessage(MessageMonitorType.StartBuilding, this, new MessageArgs<StartBuildingEven>(even));
        };

        MapGridMgr.Instance.onRemoveEdit = (List<Guid> guids) =>
        {
            ObserverHelper<List<Guid>>.SendMessage(MessageMonitorType.RemoveBuilding, this, new MessageArgs<List<Guid>>(guids));
        };

        MapGridMgr.Instance.onFreeingWastedland = (bool isfast, List<Guid> guids) =>
        {
            StartBuildingEven even = new StartBuildingEven();
            even.bIs = isfast;
            even.guids = guids;
            even.ReTyp = 2;
            even.isReMake = false; //重新开始加载
            if (isfast)
            {
                World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.CostSpade(guids.Count);
            }
            ObserverHelper<StartBuildingEven>.SendMessage(MessageMonitorType.StartWastedland, this, new MessageArgs<StartBuildingEven>(even));

        };

    }

    public BarsMgr AddBar(BarType type, Vector3 pos, GameObject goBegin = null, GameObject goEnd = null, BaseData building = null, Park park = null)
    {
        UIEntity uien = World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIBar);

        if (park == null)
        {
            BarsMgr barsMgr = new BarsMgr();
            barsMgr.guid = GenerateID.ID;
            barsMgr.pos = pos;
            barsMgr.type = type;
            barsMgr.uIEntity = uien;
            barsMgr.bar = uien.GetComponent<UIBar>();
            barsMgr.bar.InitBuilding(building);
            l_Bars.Add(barsMgr);
            return barsMgr;
        }
        else
        {
            ParkBarsMgr barsMgr = new ParkBarsMgr();
            barsMgr.guid = GenerateID.ID;
            barsMgr.pos = pos;
            barsMgr.type = type;
            barsMgr.uIEntity = uien;
            barsMgr.bar = uien.GetComponent<UIBar>();
            barsMgr.bar.InitBuilding(building);
            barsMgr.goBegin = goBegin;
            barsMgr.goEnd = goEnd;
            barsMgr.park = park;
            l_Bars.Add(barsMgr);
            return barsMgr;
        }

    }

    public void RemoveBar(Guid guid)
    {
        for (int i = 0; i < l_Bars.Count; i++)
        {
            if (l_Bars[i].guid == guid)
            {
                l_Bars[i].uIEntity.Dispose();
                l_Bars.Remove(l_Bars[i]);
                break;
            }
        }

    }

    void Update()
    {
        if (!isEnter)
            return;

        ParkMove();
        Wasteland();

        if (listDispose.Count == 0)
            return;
        for (int i = 0; i < listDispose.Count; i++)
        {
            var lDispose = listDispose[i];
            for (int j = 0; j < lDispose.Count; j++)
            {
                DisposeItem(lDispose[j], 1);
            }
        }

    }

    void ParkMove()
    {
        for (int i = 0; i < l_Bars.Count; i++)
        {
            BarsMgr barsMgr = l_Bars[i];
            if (barsMgr.type == BarType.park)
            {
                ParkBarsMgr parkBars = barsMgr as ParkBarsMgr;
                CalculateBarPos(parkBars);
                parkBars.bar.PrakMoveChanage();
                Vector3 screenPos01 = Camera.main.WorldToScreenPoint(parkBars.goBegin.transform.position);
                Vector3 screenPos02 = Camera.main.WorldToScreenPoint(parkBars.goEnd.transform.position);
                Vector3 dir = screenPos01 - screenPos02;
                float angle = Vector2.Angle(dir, Vector3.right);
                dir = Vector3.Normalize(dir);
                float dot = Vector3.Dot(dir, Vector3.up);
                if (dot < 0)
                    angle = 180 - angle;
                if (angle > 90)
                    angle = -(180 - angle);
                parkBars.bar.FixAngle(angle, BarType.park);
                TimeSpan ts = DateTime.Now.Subtract(DateTime.Parse(parkBars.park.GetServer.MoveTime));
                if (ts.TotalSeconds >= parkBars.bar.parkMoveTime * 60)
                {
                    parkBars.park.EndMove();
                    //parkBars.park.bar = null;
                    RemoveBar(parkBars.guid);
                }
                else
                {
                    bool front;
                    if (screenPos01.x < screenPos02.x)
                        front = true;
                    else
                        front = false;
                    parkBars.bar.UpdatePark((float) ts.TotalSeconds, parkBars.bar.parkMoveTime * 60, front);
                }
            }
        }
    }

    void CalculateBarPos(BarsMgr barsMgr)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(barsMgr.pos);
        barsMgr.bar.FixPos(screenPos, barsMgr.type);
    }

    void Wasteland()
    {
        int numWaste = listWasteland.Count;
        if (numWaste == 0) return;
        for (int i = 0; i < numWaste; i++)
        {
            listWasteland[i].subgrade.go.transform.Find("Text").GetComponent<TextMesh>().text = (numWaste - i).ToString();
        }

        Dispose dispose = listWasteland[numWaste - 1];
        if (!dispose.Enable) return;

        BaseData baseData = modelManager.GetModelByGuid(dispose.rootBuild.guid);
        if (baseData == null)
        {
            listWasteland.Remove(dispose);
            return;
        }

        for (int i = 0; i < listSelectWBoard.Count; i++)
        {
            if (listSelectWBoard[i].WastedGuid == dispose.rootBuild.guid)
                listSelectWBoard[i].typReclaim = 2;
            else
                listSelectWBoard[i].typReclaim = 1;
        }


        Player player = World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer;
        if (player.m_kPlayerBasicAsset.m_beginWaste == default(DateTime))
        {
            player.m_kPlayerBasicAsset.m_beginWaste = dispose.BeginTime;
            DataManager._instance.FixLocalData<PlayerBasicAsset>(player.m_kPlayerBasicAsset.ID, player.m_kPlayerBasicAsset);
        }

        DataManager._instance.wasteland.BeginReclama(baseData.guid);
        if (dispose.eff_guid == Guid.Empty)
        {
            int effid = int.Parse(baseData.cfg._Effect.Split('|') [1]);
            string _split = baseData.cfg._EffPos.Split('|') [1];
            string[] split = _split.Split(' ');
            Vector3 effpos = new Vector3(float.Parse(split[0]), float.Parse(split[1]), float.Parse(split[2]));
            dispose.eff_guid = effectManager.PlayEffectByCid(effid, baseData.go.transform, effpos);
        }

        DisposeItem(dispose, 2);
    }

    //float passTime;
    //typ 1,是普遍建造 2是处理开荒的
    void DisposeItem(Dispose dispose, int typ)
    {
        int disType = 0; //1是新建, 2是升级
        Building building = dispose.rootBuild.GetComponent<Building>();
        //float needTime = 0;
        //DateTime expendTime = dispose.BeginTime;
        if (building.IsPause)
        {
            if (building.BeginPause == default(DateTime))
            {
                building.BeginPause = DateTime.Now;
                building.EndPause = default(DateTime);
                EffectItem effect = effectManager.GetEffectByGuid(dispose.eff_guid);
                if (effect != null) effect.gameObject.SetActive(false);
            }
        }
        else
        {
            if (building.BeginPause != default(DateTime) && building.EndPause == default(DateTime))
            {
                building.EndPause = DateTime.Now;
                float passTime = (float) building.EndPause.Subtract(building.BeginPause).TotalSeconds;
                building.passTime += passTime;
                //dispose.BeginTime = dispose.BeginTime.AddSeconds(passTime);

                building.BeginPause = default(DateTime);
                building.EndPause = default(DateTime);
                EffectItem effect = effectManager.GetEffectByGuid(dispose.eff_guid);
                if (effect != null) effect.gameObject.SetActive(true);
            }
        }

        //TimeSpan ts = DateTime.Now.Subtract(dispose.BeginTime);
 
        float needTime = building.passTime;      
        if (typ == 1)
        {
            BuildingServer server = building.GetServer;
            if (server.buildState == BuildState.Sustain)
            {
                disType = 1;
                needTime += dispose.rootBuild.cfg._BuildTime;
            }
            else if (server.buildState == BuildState.UpLv)
            {
                disType = 2;
                needTime += building.CS_UpLevel._UpTime;
            }
        }
        else if (typ == 2)
        {
            needTime += dispose.rootBuild.cfg._BuildTime;
        }

        DateTime endTime = dispose.BeginTime.AddSeconds(needTime);
        float max = (float)endTime.Subtract(dispose.BeginTime).TotalSeconds;
        float now = (float)DateTime.Now.Subtract(dispose.BeginTime).TotalSeconds;

        if (typ == 1)
        {
            if (dispose.barsMgr != null)
            {
                CalculateBarPos(dispose.barsMgr);
                if (!building.IsPause) dispose.barsMgr.bar.UpdateBuilding(now, max);
            }
        }
        else if (typ == 2)
        {
            if (dispose.barsMgr != null)
            {
                CalculateBarPos(dispose.barsMgr);
                if (!building.IsPause)
                {
                    //.barsMgr.bar.m_btnWasteland.gameObject.SetActive(false);
                    dispose.barsMgr.bar.UpdateWasteland(now, max);
                }
                else
                {
                    //dispose.barsMgr.bar.m_proWasteland.transform.gameObject.SetActive(false);
                    //dispose.barsMgr.bar.m_btnWasteland.gameObject.SetActive(true);
                }

            }
        }
 
        //Debug.Log("____55__");
        if (now >= max && !building.IsPause)
        {
            if (dispose.subgrade != null)
            {
                dispose.subgrade.go.SetActive(true);
                modelManager.RecycleByGuid(dispose.subgrade.guid);
            }
                

            effectManager.StopEffctById(dispose.eff_guid);
            dispose.rootBuild.go.SetActive(true);

            if (disType == 1)
                dispose.rootBuild.GetComponent<Building>().EndBuilding();
            else if (disType == 2)
                dispose.rootBuild.GetComponent<Building>().EndUpLevel();

            if (typ == 1)
            {
                if (dispose.barsMgr != null)
                {
                    RemoveBar(dispose.barsMgr.guid);
                    ParkBarsMgr parkBarsMgr = dispose.barsMgr as ParkBarsMgr;
                    if(parkBarsMgr != null)
                    {
                        parkBarsMgr.park.uiBar = null;
                        if (disType == 2)
                            parkBarsMgr.park.EndUpLevel();
                    }
  
                    dispose.barsMgr = null;
                }

                bool isCome = false;
                for (int i = listDispose.Count - 1; i >= 0; i--)
                {
                    if (isCome) break;
                    var lDispose = listDispose[i];
                    for (int j = lDispose.Count - 1; j >= 0; j--)
                    {
                        if (dispose.rootBuild.guid == lDispose[j].rootBuild.guid)
                        {
                            isCome = true;
                            lDispose.Remove(dispose);
                            if (lDispose.Count == 0)
                                listDispose.Remove(lDispose);
                            break;
                        }
                    }

                }

                if (MapGridMgr.IsBarrier(dispose.rootBuild.cfg._ID))
                {
                    if (disType == 1)
                    {
                        modelManager.SetPark(dispose.fahter_guid, dispose.rootBuild);
                    }

                    List<Guid> listguids = new List<Guid>(){ dispose.rootBuild.guid };
                    modelManager.GetParkList(listguids).GetServer.parkType = ParkType.Idle;
                    ParkServer parkServer = modelManager.GetParkList(listguids).GetServer;
                    //modelManager.GetParkList(dispose.rootBuild.guid).GetServer.parkType = ParkType.Idle;
                    //ParkServer parkServer = modelManager.GetParkList(dispose.rootBuild.guid).GetServer;
                    DataManager._instance.FixLocalData<ParkServer>(parkServer.guid, parkServer);
                  
                }
                BuildingServer server = dispose.rootBuild.GetComponent<Building>().GetServer;
                dataManager.FixLocalData<BuildingServer>(server.guid, server);
            }
            else if (typ == 2)
            {
                if (dispose.barsMgr != null)
                    RemoveBar(dispose.barsMgr.guid);
         

                RemoveSelectWastedLand(dispose.rootBuild.guid, 2);

                Vector3 startPos = dispose.rootBuild.go.transform.position;
                listWasteland.Remove(dispose);

                MapGridMgr.Instance.RemoveWastedland(dispose.rootBuild.go);
                DataManager._instance.wasteland.EndReclama(dispose.rootBuild.guid);
                dataManager.NotificaMapChange("Map_Useable");
                dataManager.NotificaMapChange("Map_Wastedland");
                if (listWasteland.Count == 0)
                {//显示开荒奖励

                    timeComponent.CreateTimer(2000, 0, 1, () => { 
                        UIEntity entity = World.Scene.GetComponent<UIManagerComponent>().Get(UI_PrefabPath.m_sUISystem_Hall);
                        entity.GetComponent<UISystem_HallComponent>().m_RectWasteAward.gameObject.SetActive(true);
                    });
                }

                if (ModelManager._instance.assistant != null)
                {
                    Assistant assistant = ModelManager._instance.assistant;
                    if (listWasteland.Count == 0)
                    {
                        ModelManager._instance.assistant.GoHome();
                    }
                    else
                    {
                        Dispose next_dispose = listWasteland[listWasteland.Count - 1];
                        float WasteMoveTime = float.Parse(DBManager.Instance.m_kDisperse.GetEntryPtr(20005)._Val1);
                        float timeMove = WasteMoveTime * Vector3.Distance(startPos, next_dispose.rootBuild.go.transform.position) / 3;

                        ModelManager._instance.assistant.Move(next_dispose.rootBuild.go.transform.position, timeMove, () =>
                        {
                            next_dispose.Enable = true;
                            next_dispose.BeginTime = DateTime.Now;
                            ModelManager._instance.assistant.Weed(next_dispose.rootBuild.cfg._BuildTime);
                        });
                    }
                }

            }

        }

    }

    void Start()
    {
        dataManager = World.Scene.GetComponent<DataManager>();
        modelManager = World.Scene.GetComponent<ModelManager>();
        effectManager = World.Scene.GetComponent<EffectManagerComponent>();
        timeComponent = World.Scene.GetComponent<TimeComponent>();
        //SaveCount = 1;
    }

    //升级
    void OnStartUpLv(object sender, MessageArgs<StartBuildingEven> even)
    {
    
        Park park = null;
        List<Dispose> _lDispose = new List<Dispose>();
        for (int i = 0; i < even.Item.guids.Count; i++)
        {
            BaseData baseData = modelManager.GetModelByGuid(even.Item.guids[i]);
            Building building = baseData.GetComponent<Building>();
            BuildingServer server = baseData.GetComponent<Building>().GetServer;
            //不需要升级时间 ,  满级
            if (building.CS_UpLevel._UpTime == 0 || building.CS_UpLevel._NextID == 0)
            {
                baseData.go.SetActive(true);
            }
            else
            {
                Dispose dispose = new Dispose();
                dispose.rootBuild = baseData;
                //dispose.fahter_guid = even.Item.guids[0];
                if (park == null && MapGridMgr.IsBarrier(baseData.cfg._ID))
                    park = ModelManager._instance.GetParkList(even.Item.guids);
                if (MapGridMgr.IsBarrier(baseData.cfg._ID) && park != null)
                    dispose.fahter_guid = Guid.Parse(park.GetServer.listGuids.Split('|')[0]);
                else
                    dispose.fahter_guid = even.Item.guids[0];


                if (baseData.cfg._SubgradeID != 0) //有地基
                {
                    baseData.go.SetActive(false);
                    BaseData subgrade = modelManager.Load(baseData.cfg._SubgradeID);
                    subgrade.go.transform.position = baseData.go.transform.position;
                    subgrade.go.SetActive(true);
                    dispose.subgrade = subgrade;
                    int effid = int.Parse(subgrade.cfg._Effect.Split('|') [0]);
                    dispose.eff_guid = effectManager.PlayEffectByCid(effid, subgrade.go.transform);
                }
                else
                {
                    baseData.go.SetActive(true);
                    int effid = int.Parse(baseData.cfg._Effect.Split('|') [0]);
                    dispose.eff_guid = effectManager.PlayEffectByCid(effid, baseData.go.transform);
                }

                baseData.GetComponent<Building>().BeginUpLevel();
                dispose.BeginTime = DateTime.Parse(server.BeginTime);
                if (park != null)
                {
                    if (park.uiBar == null)
                    {
                        park.GetServer.parkType = ParkType.Uplv;
                        dispose.barsMgr = AddBar(BarType.building, park.parkSign.go.transform.position, null, null, dispose.rootBuild, park);
                        park.uiBar = dispose.barsMgr;
                    }
                }
                else
                    dispose.barsMgr = AddBar(BarType.building, dispose.rootBuild.go.transform.position, null, null, dispose.rootBuild);

                _lDispose.Add(dispose);
            }
            if (!even.Item.isReMake) //不是重新开始加载
            {
                dataManager.ChangeMapData(baseData.cfg._ID);
                dataManager.AddLocalData<BuildingServer>(baseData.guid.ToString(), server);
            }
        }

        if (_lDispose.Count > 0)
            listDispose.Add(_lDispose);

    }

    //新建
    void OnStartBuilding(object sender, MessageArgs<StartBuildingEven> even)
    {
        bool isBarrier = false;
        BaseData firstBD = modelManager.GetModelByGuid(even.Item.guids[0]);
        if (MapGridMgr.IsBarrier(firstBD.cfg._ID)) isBarrier = true;
       
     
        //bool isParkCreate = false;
        Park park = null;
        List<bool> listCreate = new List<bool>();
        if(isBarrier)
        {
            park = ModelManager._instance.GetParkList(even.Item.guids);
            if (park != null)
            {
                for (int i = 0; i < even.Item.guids.Count; i++)
                {
                    if (even.Item.isReMake) //是重新开始加载
                    {
                        listCreate.Add(true);
                    }
                    else
                    {
                        if (!park.dicChild.ContainsKey(even.Item.guids[i]))
                            listCreate.Add(true);
                        else
                            listCreate.Add(false); 
                    }
       
                    BaseData baseData = modelManager.GetModelByGuid(even.Item.guids[i]);
                    park.AddChild(baseData);
                
                }
            }
            else
            {
                if (isBarrier && even.Item.bIs && even.Item.guids.Count == 1)
                {
                    //isParkCreate = true;
                    park = ModelManager._instance.CreatePark(firstBD);
                    listCreate.Add(even.Item.bIs);
                }
            }
        }
        else
            listCreate.Add(even.Item.bIs);
 

      
 
        List<Dispose> _lDispose = new List<Dispose>();
        for (int i = 0; i < even.Item.guids.Count; i++)
        {
            if (!listCreate[i])
                continue;
            BaseData baseData = modelManager.GetModelByGuid(even.Item.guids[i]);
            BuildingServer server = baseData.GetComponent<Building>().GetServer;
            //不需要建造时间
            if (baseData.cfg._BuildTime == 0)
                baseData.go.SetActive(true);
            else
            {
                Dispose dispose = new Dispose();
                dispose.rootBuild = baseData;
                if (MapGridMgr.IsBarrier(baseData.cfg._ID) && park != null)
                    dispose.fahter_guid = Guid.Parse(park.GetServer.listGuids.Split('|')[0]);
                else
                    dispose.fahter_guid = even.Item.guids[0];
                

                if (baseData.cfg._SubgradeID != 0) //有地基
                {
                    baseData.go.SetActive(false);
                    BaseData subgrade = modelManager.Load(baseData.cfg._SubgradeID);
                    subgrade.go.transform.position = baseData.go.transform.position;
                    subgrade.go.SetActive(true);
                    dispose.subgrade = subgrade;
                    int effid = int.Parse(subgrade.cfg._Effect.Split('|') [0]);
                    dispose.eff_guid = effectManager.PlayEffectByCid(effid, subgrade.go.transform);
                }
                else
                {
                    baseData.go.SetActive(true);
                    int effid = int.Parse(baseData.cfg._Effect.Split('|') [0]);
                    dispose.eff_guid = effectManager.PlayEffectByCid(effid, baseData.go.transform);
                }

                baseData.GetComponent<Building>().BeginBuilding();
                if (GuidanceManager.isGuidancing)
                {
                    int step = PlayerPrefs.GetInt("currGuidanceStep", 1);
                    if (step < 17)
                    {
                        baseData.GetComponent<Building>().IsPause = true;
                        server.BeginTime = DateTime.MaxValue.ToString();
                        DataManager._instance.FixLocalData<BuildingServer>(server.guid, server);
                    }
                }
                // 监测放置建筑成功
                if (GameEventManager._Instance != null && GameEventManager._Instance.onPlacedBuildSuccess != null) GameEventManager._Instance.onPlacedBuildSuccess();
                dispose.BeginTime = DateTime.Parse(server.BeginTime);
                if (park != null)
                {
                    if (park.uiBar == null)
                    {
                        park.GetServer.parkType = ParkType.Sustain;
                        dispose.barsMgr = AddBar(BarType.building, park.parkSign.go.transform.position, null, null, dispose.rootBuild, park);
                        park.uiBar = dispose.barsMgr;
                    }
                }
                else
                    dispose.barsMgr = AddBar(BarType.building, dispose.rootBuild.go.transform.position, null, null, dispose.rootBuild);
                _lDispose.Add(dispose);
            }
            if (!even.Item.isReMake) //不是重新开始加载
            {
                dataManager.ChangeMapData(baseData.cfg._ID);
                dataManager.AddLocalData<BuildingServer>(baseData.guid.ToString(), server);
            }
        }

        if (_lDispose.Count > 0)
            listDispose.Add(_lDispose);

    }

    void OnRemoveBuilding(object sender, MessageArgs<List<Guid>> even)
    {

        //for (int i = 0; i < even.Item.Count; i++)
        //{
        //    BaseData baseData = modelManager.GetModelByGuid(even.Item[i]);
        //    dataManager.ChangeMapData(baseData.cfg._ID);
        //}

    }

    //开荒
    void OnStartWastedland(object sender, MessageArgs<StartBuildingEven> even)
    {
        PlayerBasicAsset playerBasicAsset = World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.m_kPlayerBasicAsset;
        List<Guid> listguids = new List<Guid>();
        for (int i = 0; i < even.Item.guids.Count; i++)
        {
            listguids.Add(even.Item.guids[i]);
        }

        //重新加载
        if (even.Item.isReMake && !even.Item.bIs)
        {
            List<Guid> templistguids = new List<Guid>();
            double passTime = -1;
            if (playerBasicAsset.m_beginWaste != default(DateTime))
                passTime = DateTime.Now.Subtract(playerBasicAsset.m_beginWaste).TotalSeconds;
            double allTime = 0;
            for (int i = 0; i < listguids.Count; i++)
            {
                BaseData baseData = modelManager.GetModelByGuid(listguids[i]);
                allTime += baseData.cfg._BuildTime;
                if (passTime < allTime)
                {
                    templistguids.Add(listguids[i]);
                }
                else //完成
                {
                    Dispose dispose = new Dispose();
                    dispose.rootBuild = baseData;
                    dispose.subgrade = null;
                    dispose.eff_guid = Guid.Empty;
                    dispose.BeginTime = DateTime.MinValue;
                    dispose.barsMgr = null;
                    DisposeItem(dispose, 2);
                }
            }
            listguids = templistguids;
        }

        for (int i = 0; i < listguids.Count; i++)
        {
            BaseData baseData = modelManager.GetModelByGuid(listguids[i]);
            //快速开垦
            if (even.Item.bIs == true)
            {
                for (int j = listWasteland.Count - 1; j >= 0; j--)
                {
                    var _Dispose = listWasteland[j];
                    if (_Dispose.rootBuild.guid == baseData.guid && _Dispose.BeginTime == default(DateTime))
                    {
                        if (_Dispose.barsMgr != null)  RemoveBar(_Dispose.barsMgr.guid);
                        modelManager.RecycleByGuid(_Dispose.subgrade.guid);
                        //RemoveSelectWastedLand(_Dispose.rootBuild.guid);
                        listWasteland.Remove(_Dispose);
                        break;
                    }
                }

                for (int j = listSelectWBoard.Count - 1; j >= 0; j--)
                {
                    var _WastedBoard = listSelectWBoard[j];
                    if(_WastedBoard.WastedGuid == baseData.guid)
                    {
                        RemoveSelectWastedLand(_WastedBoard);
                        break;
                    }
                }

                int effid = int.Parse(baseData.cfg._Effect.Split('|') [0]);
                effectManager.PlayEffectByCid(effid, baseData.go.transform);
                DataManager._instance.wasteland.EndReclama(baseData.guid);
                MapGridMgr.Instance.RemoveWastedland(baseData.go);
                World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.SetTask(TaskType.assart, 1);
            }
            else
            {
                Dispose dispose = new Dispose();
                dispose.rootBuild = baseData;
                dispose.eff_guid = Guid.Empty;
                BaseData BoardbaseData = ModelManager._instance.Load(20004);
                BoardbaseData.go.transform.position = baseData.go.transform.position;
                dispose.subgrade = BoardbaseData;
                dispose.barsMgr = AddBar(BarType.wasted, dispose.rootBuild.go.transform.position);
                dispose.Enable = false;
                World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.WasteLand(1, listguids[i]);
                listWasteland.Insert(0, dispose);
 
            }

            //重新加载
            if (even.Item.isReMake)
            {
                BaseData wastedBoard = SelectWastedLand(listguids[i], baseData.go.transform.position);
                wastedBoard.go.gameObject.SetActive(false);
            }

        }

        if (!even.Item.bIs && listWasteland.Count > 0)
        {
            Assistant assistant = ModelManager._instance.assistant;
            if (assistant == null)
                assistant = ModelManager._instance.CreateAssistant();
            assistant.BeginWorkWeed();
    
        }

        dataManager.NotificaMapChange("Map_Wastedland");
        dataManager.NotificaMapChange("Map_Useable");

    }

    public TileInfo selectTileInfo;
    //选中物体
    void OnSelectedTileInfo(object sender, MessageArgs<TileInfo> even)
    {
        selectTileInfo = even.Item;

        if(MapGridMgr.IsWastedland(selectTileInfo.baseData.cfg._ID))
        {
            SelectWastedLand(selectTileInfo.guid, selectTileInfo.baseData.go.transform.position);
        }

    }

    void RemoveSelectWastedLand(WastedBoard data)
    {
        listSelectWBoard.Remove(data);
        ModelManager._instance.RecycleByGuid(data.BoardbaseData.guid);
        for (int j = listWasteland.Count - 1; j >= 0; j--)
        {
            if (listWasteland[j].rootBuild.guid == data.WastedGuid)
            {
                ModelManager._instance.RecycleByGuid(listWasteland[j].subgrade.guid);
                if (listWasteland[j].barsMgr != null)
                    RemoveBar(listWasteland[j].barsMgr.guid);
                listWasteland.Remove(listWasteland[j]);
                World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.WasteLand(2, data.WastedGuid);
                break;
            }
        }
    }
    BaseData AddSelectWastedLand(Guid guid, Vector3 pos)
    {
        BaseData baseData = null;
        WastedBoard wastedBoard = new WastedBoard();
        BaseData BoardbaseData = ModelManager._instance.Load(20004);
        BoardbaseData.go.transform.position = pos;
        wastedBoard.BoardbaseData = BoardbaseData;
        wastedBoard.WastedGuid = guid;
        wastedBoard.typReclaim = 1;
        listSelectWBoard.Add(wastedBoard);
        baseData = BoardbaseData;
        return baseData;
    }

    BaseData SelectWastedLand(Guid guid, Vector3 pos)
    {
        bool isHave = false;
        for (int i = 0; i < listSelectWBoard.Count; i++)
        {
            var data = listSelectWBoard[i];
            if (data.WastedGuid == guid) 
            {
                isHave = true;
            
                if (listWasteland.Count > 0 && guid == listWasteland[listWasteland.Count - 1].rootBuild.guid)
                {
                    data.typReclaim = 2;
                }
              
                if(data.typReclaim == 1)
                {
                    RemoveSelectWastedLand(data);
                    //listSelectWBoard.Remove(data);
                    //ModelManager._instance.RecycleByGuid(data.BoardbaseData.guid);
                    //for (int j = listWasteland.Count - 1; j >= 0; j--)
                    //{
                    //    if (listWasteland[j].rootBuild.guid == guid)
                    //    {
                    //        ModelManager._instance.RecycleByGuid(listWasteland[j].subgrade.guid);
                    //        if(listWasteland[j].barsMgr != null)
                    //            RemoveBar(listWasteland[j].barsMgr.guid);
                    //        listWasteland.Remove(listWasteland[j]);
                    //        World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.WasteLand(2, guid);
                    //        break;
                    //    }
                    //}
                }
                break; 

            }
        }
        BaseData baseData = null;
        int limit_sele = int.Parse(DBManager.Instance.m_kDisperse.GetEntryPtr(20002)._Val1);
        if (!isHave)
        {
            if (listSelectWBoard.Count < limit_sele)
            {
                baseData = AddSelectWastedLand(guid, pos);
            }  
            else if(listSelectWBoard.Count == limit_sele)
            {
                WastedBoard endData = listSelectWBoard[listSelectWBoard.Count - 1];
                RemoveSelectWastedLand(endData);
                baseData = AddSelectWastedLand(guid, pos);
            }
        }
  
        for (int i = 0; i < listSelectWBoard.Count; i++)
        {
            listSelectWBoard[i].BoardbaseData.go.transform.Find("Text").GetComponent<TextMesh>().text = (i + 1).ToString();
        }



        return baseData;
    }

   
    public void RemoveSelectWastedLand(Guid guiWastedLand, int typReclaim = 1)
    {

        for (int i = 0; i < listSelectWBoard.Count; i++)
        {
            var data = listSelectWBoard[i];
            if (data.WastedGuid == guiWastedLand && data.typReclaim == typReclaim)
            {
                listSelectWBoard.Remove(data);
                ModelManager._instance.RecycleByGuid(data.BoardbaseData.guid);
            }

            //if (data.WastedGuid == guiWastedLand && data.typReclaim == 1)
            //{
            //    listSelectWBoard.Remove(data);
            //    ModelManager._instance.RecycleByGuid(data.BoardbaseData.guid);
            //    break;
            //}
        }
        for (int i = 0; i < listSelectWBoard.Count; i++)
        {
            listSelectWBoard[i].BoardbaseData.go.transform.Find("Text").GetComponent<TextMesh>().text = (i + 1).ToString();
        }
    }

    public void RemoveAllSelectWastedLand()
    {
        for (int i = 0; i < listSelectWBoard.Count; i++)
        {
            var data = listSelectWBoard[i];
            ModelManager._instance.RecycleByGuid(data.BoardbaseData.guid);
        }
        listSelectWBoard.Clear();

        for (int i = listWasteland.Count - 1; i >= 0; i--)
        {
            var data = listWasteland[i];
            var basedata = SelectWastedLand(data.rootBuild.guid, data.rootBuild.go.transform.position);
            basedata.go.SetActive(false);
        }
    }

    public bool SpeedUpComplete(Guid guid, int second)
    {
        for (int i = 0; i < listDispose.Count; i++)
        {
            var _lDispose = listDispose[i];
            for (int j = 0; j < _lDispose.Count; j++)
            {
                if (_lDispose[j].rootBuild.guid == guid)
                {
                    for (int n = 0; n < _lDispose.Count; n++)
                    {
                        _lDispose[n].rootBuild.GetComponent<Building>().IsPause = false;
                        BuildingServer server = _lDispose[n].rootBuild.GetComponent<Building>().GetServer;

                        DateTime nowT;
                        if (GuidanceManager.isGuidancing && PlayerPrefs.GetInt("currGuidanceStep", 1) < 17)
                        {
                            nowT = DateTime.Now.AddSeconds(-second);
                        }
                        else
                            nowT = DateTime.Parse(server.BeginTime).AddSeconds(-second);
                        //DateTime nowT = DateTime.Parse(server.BeginTime).AddSeconds(-second);
                        _lDispose[n].BeginTime = nowT;
                        _lDispose[n].rootBuild.GetComponent<Building>().GetServer.BeginTime = nowT.ToString();
                        dataManager.FixLocalData<BuildingServer>(server.guid, _lDispose[n].rootBuild.GetComponent<Building>().GetServer);
                    }
                    return true;
                }
            }
        }
        return false;
    }

    void CheckGuidance()
    {
        // 特殊情况 新手引导拖动建筑到场景的引导 如果拖出来的位置刚刚好就是我们需要的地方 那就不会调用回调 我们这在这里做特殊处理
        if (GuidanceManager.isGuidancing && GuidanceManager.currStep == GuidanceStep.Step11)
        {
            Debug.Log("点击移动了物体-----在这里sendmessage--------");
            GuidanceData info = new GuidanceData();
            info.entity = World.Scene.GetComponent<UIManagerComponent>().Get(UI_PrefabPath.m_sUIPage_BuildBuy);
            ObserverHelper<GuidanceData>.SendMessage(MessageMonitorType.GuidanceClickEvent, this, new MessageArgs<GuidanceData>(info));
        }
    }
    public bool Selected = false;
    public ModelBase modelBase;
    void OnSelectedModelInfo(object sender, MessageArgs<ModelBase> even)
    {
        if (modelBase != null)
            modelBase.DisableOutline();

        modelBase = even.Item;
        if (modelBase != null)
            modelBase.EnableOutline(Color.white, 2);

        Selected = true;
        if (modelBase.baseData.cfg._Type == (int) ModeTyp.Animal ||
            modelBase.baseData.cfg._Type == (int) ModeTyp.Plant ||
            modelBase.baseData.cfg._Type == (int) ModeTyp.Building ||
            modelBase.baseData.cfg._Type == (int) ModeTyp.Actor)
        {
            BE.MobileRTSCam.instance.SmoothSelectItem(modelBase.gameObject.transform.position);
        }
    }

    public void OnDisSelectModelInfo()
    {
        if (modelBase != null)
            modelBase.DisableOutline();
        Selected = false;
        if (modelBase != null && modelBase.baseData != null && (modelBase.baseData.cfg._Type == (int) ModeTyp.Animal ||
                modelBase.baseData.cfg._Type == (int) ModeTyp.Plant ||
                modelBase.baseData.cfg._Type == (int) ModeTyp.Building ||
                modelBase.baseData.cfg._Type == (int) ModeTyp.Actor))
        {
            BE.MobileRTSCam.instance.UnFocusSelectItem();
        }
        modelBase = null;
    }

 

    //public delegate void Event();

    //void OpenBox(string title, string content, string ensure, Event cbEvent = null)
    //{
    //    //
    //    UIEntity _uien = UI_Helper.ShowConfirmPanel("TextChange",
    //        () =>
    //        {
    //            if (cbEvent != null)
    //                cbEvent();
    //        },
    //        () => { });
    //    _uien.GetComponent<UI_Confirm>().TextChange(title, content, ensure);
    //}

 
    public void SetWaterPool()
    {
        if (ModelManager._instance.GetModleByType(ModelCType.WaterPool).Count == 0) return;
        var data = ModelManager._instance.GetModleByType(ModelCType.WaterPool)[0];
        float rate = (float)World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.m_kPlayerBasicAsset.m_kWater / data.cfg._Capacity;
        if (rate > 1) rate = 1;
        GameObject goWaterPool = data.go;
        if(data.cfg._ID == 20603)
        {
            GameObject goWater01 = goWaterPool.transform.Find("water01").gameObject;
            GameObject goWater02 = goWaterPool.transform.Find("water02").gameObject;
            GameObject goWater03 = goWaterPool.transform.Find("water03").gameObject;
            goWater01.SetActive(false);
            goWater02.SetActive(false);
            goWater03.SetActive(false);
            if (rate <= 0.3) 
                goWater01.SetActive(true);
            else if (rate <= 0.7 && rate > 0.3) 
                goWater02.SetActive(true);
            else 
                goWater03.SetActive(true);
        }
        else
        {
            goWaterPool.transform.Find("water").transform.localScale = new Vector3(1, rate, 1);  
        }


    }

  
    //一键喂养
    public bool QuickFeed(List<BaseData> listBaseData)
    {
        Player player = World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer;
        List<PlayerBagAsset.BagItem> lNutrients = new List<PlayerBagAsset.BagItem>(); //通用饲料
        List<PlayerBagAsset.BagItem> lAnimalNutrients = new List<PlayerBagAsset.BagItem>(); //动物饲料
        List<PlayerBagAsset.BagItem> lBotanyNutrients = new List<PlayerBagAsset.BagItem>(); //植物饲料
        List<PlayerBagAsset.BagItem> lMedicine = new List<PlayerBagAsset.BagItem>(); //通用药剂
        List<PlayerBagAsset.BagItem> lAnimalMedicine = new List<PlayerBagAsset.BagItem>(); //动物药剂
        List<PlayerBagAsset.BagItem> lBotanyMedicine = new List<PlayerBagAsset.BagItem>(); //植物药剂
        PlayerBagAsset playerBagAsset = player.m_kPlayerBagAsset;
        for (int i = 0; i < playerBagAsset.m_kBag.Count; i++)
        {
            var data = playerBagAsset.m_kBag[i];
            if (data.m_kItemType == PlayerBagAsset.ItemType.Nutrients) lNutrients.Add(data);
            else if (data.m_kItemType == PlayerBagAsset.ItemType.AnimaiNutrients) lAnimalNutrients.Add(data);
            else if (data.m_kItemType == PlayerBagAsset.ItemType.BotanyNutrients) lBotanyNutrients.Add(data);
            else if (data.m_kItemType == PlayerBagAsset.ItemType.Medicine) lMedicine.Add(data);
            else if (data.m_kItemType == PlayerBagAsset.ItemType.AnimaiMedicine) lAnimalMedicine.Add(data);
            else if (data.m_kItemType == PlayerBagAsset.ItemType.BotanyMedicine) lBotanyMedicine.Add(data);
        }

        int lackwater = 0;//还需要多少水
        int costwater = 0;//最终扣除多少水

        int lackAllAN = 0;//总共需要多少动物饲料
        int lackAN = 0;//还需要多少动物饲料
        Dictionary<int, int> costAN = new Dictionary<int, int>();
        int lackAllPN = 0;
        int lackPN = 0;//还需要多少植物饲料
        Dictionary<int, int> costPN = new Dictionary<int, int>();
        Dictionary<int, int> costCN = new Dictionary<int, int>();//通用饲料


        int lackAllAM = 0;
        int lackAM = 0;//还需要多少动物药剂
        Dictionary<int, int> costAM = new Dictionary<int, int>();
        int lackAllPM = 0;
        int lackPM = 0;//还需要多少植物药剂
        Dictionary<int, int> costPM = new Dictionary<int, int>();
        Dictionary<int, int> costCM = new Dictionary<int, int>();//通用药料

 
        for (int i = 0; i < listBaseData.Count; i++)
        {
            BaseData _baseData = listBaseData[i];
            ModelBase _modelBase = _baseData.GetComponent<ModelBase>();
            BaseServer server = _modelBase.GetbaseServer;
            List<Buff> lmsBuff = _modelBase.GetMyselfBuff;
            List<List<int>> llState = _modelBase.GetStatePro;
            for (int j = 0; j < llState.Count; j++)
            {
                int need = llState[j][2] - (int)server.proVal[j];
                if ((StatePro)llState[j][0] == StatePro.Thirst)//口渴
                {
                    costwater += need;
                    if ((int)player.m_kPlayerBasicAsset.m_kWater < costwater)
                        lackwater = costwater - (int)player.m_kPlayerBasicAsset.m_kWater;
                }
                else if ((StatePro)llState[j][0] == StatePro.Hunger)//饥饿
                {
                    if (_baseData.cfg._Type == (int)ModeTyp.Animal) 
                        lackAllAN += need;
                    else if (_baseData.cfg._Type == (int)ModeTyp.Plant)
                        lackAllPN += need;
                }

                foreach(Buff _buff in lmsBuff)
                {
                    if(_buff.CS_Buff._Type == (int)BuffType.Ill)
                    {
                        int splitBuff2 = int.Parse(_buff.CS_Buff._disType.Split(' ')[2]);
                        if (_baseData.cfg._Type == (int)ModeTyp.Animal)
                            lackAllAM += splitBuff2;
                        else if (_baseData.cfg._Type == (int)ModeTyp.Plant)
                            lackAllPM += splitBuff2;
                    }
                }
            }
        }

    
        int nneedAN = 0;
        bool isEnoughAN = false;//动物饲料足够不
        for (int i = 0; i < lAnimalNutrients.Count; i++)
        {
            if (isEnoughAN) break;
            var data = lAnimalNutrients[i];
            var CS = DBManager.Instance.m_kItems.GetEntryPtr(data.m_kItemID);
            int val = int.Parse(CS._Use.Split(' ')[2]);
            for (int j = 0; j < data.m_kCount; j++)
            {
                nneedAN += val;
                if (costAN.ContainsKey(data.m_kItemID))
                    costAN[data.m_kItemID] = j;
                else
                    costAN.Add(data.m_kItemID, 1);
                if(lackAllAN <= nneedAN)
                {
                    isEnoughAN = true;
                    break;
                }
            }
        }
        int nneedPN = 0;
        bool isEnoughPN = false;//植物饲料足够不
        for (int i = 0; i < lBotanyNutrients.Count; i++)
        {
            if (isEnoughPN) break;
            var data = lBotanyNutrients[i];
            var CS = DBManager.Instance.m_kItems.GetEntryPtr(data.m_kItemID);
            int val = int.Parse(CS._Use.Split(' ')[2]);
            for (int j = 0; j < data.m_kCount; j++)
            {
                nneedPN += val;
                if (costPN.ContainsKey(data.m_kItemID))
                    costPN[data.m_kItemID] = j;
                else
                    costPN.Add(data.m_kItemID, 1);
                if (lackAllPN <= nneedPN)
                {
                    isEnoughPN = true;
                    break;
                }
            }
        }


        int _lackN = lackAllAN - nneedAN + lackAllPN - nneedPN;
        if (!isEnoughAN || !isEnoughPN)
        {
            for (int i = 0; i < lNutrients.Count; i++)
            {
                if (isEnoughPN && isEnoughAN) break;
                var data = lNutrients[i];
                var CS = DBManager.Instance.m_kItems.GetEntryPtr(data.m_kItemID);
                int val = int.Parse(CS._Use.Split(' ')[2]);
                for (int j = 0; j < data.m_kCount; j++)
                {
                    _lackN -= val;
                    if (costCN.ContainsKey(data.m_kItemID))
                        costCN[data.m_kItemID] = j;
                    else
                        costCN.Add(data.m_kItemID, 1);
                    if(_lackN <= 0)
                    {
                        isEnoughAN = true;
                        isEnoughPN = true;
                        break;
                    }
                }
            }
        }
   
        if(!isEnoughAN && !isEnoughPN)
        {
            lackAN = Mathf.CeilToInt(_lackN / 2);
            lackPN = Mathf.CeilToInt(_lackN / 2);
        }
        else
        {
            if (!isEnoughAN) lackAN = _lackN;
            if (!isEnoughPN) lackPN = _lackN;
        }



 


        int nneedAM = 0;
        bool isEnoughAM = false;//动物药剂足够不
        for (int i = 0; i < lAnimalMedicine.Count; i++)
        {
            if (isEnoughAM) break;
            var data = lAnimalMedicine[i];
            var CS = DBManager.Instance.m_kItems.GetEntryPtr(data.m_kItemID);
            int val = int.Parse(CS._Use.Split(' ')[2]);
            for (int j = 0; j < data.m_kCount; j++)
            {
                nneedAM += val;
                if (costAM.ContainsKey(data.m_kItemID))
                    costAM[data.m_kItemID] = j;
                else
                    costAM.Add(data.m_kItemID, 1);
                if (lackAllAM <= nneedAM)
                {
                    isEnoughAM = true;
                    break;
                }
            }
        }
        int nneedPM = 0;
        bool isEnoughPM = false;//植物药剂足够不
        for (int i = 0; i < lBotanyMedicine.Count; i++)
        {
            if (isEnoughPM) break;
            var data = lBotanyMedicine[i];
            var CS = DBManager.Instance.m_kItems.GetEntryPtr(data.m_kItemID);
            int val = int.Parse(CS._Use.Split(' ')[2]);
            for (int j = 0; j < data.m_kCount; j++)
            {
                nneedPM += val;
                if (costPM.ContainsKey(data.m_kItemID))
                    costPM[data.m_kItemID] = j;
                else
                    costPM.Add(data.m_kItemID, 1);
                if (lackAllPN <= nneedPM)
                {
                    isEnoughPM = true;
                    break;
                }
            }
        }


        int _lackM = lackAllAM - nneedAM + lackAllPM - nneedPM;
        if (!isEnoughAM || !isEnoughPM)
        {
            for (int i = 0; i < lMedicine.Count; i++)
            {
                if (isEnoughAM && isEnoughPM) break;
                var data = lMedicine[i];
                var CS = DBManager.Instance.m_kItems.GetEntryPtr(data.m_kItemID);
                int val = int.Parse(CS._Use.Split(' ')[2]);
                for (int j = 0; j < data.m_kCount; j++)
                {
                    _lackM -= val;
                    if (costCM.ContainsKey(data.m_kItemID))
                        costCM[data.m_kItemID] = j;
                    else
                        costCM.Add(data.m_kItemID, 1);
                    if (_lackM <= 0)
                    {
                        isEnoughAM = true;
                        isEnoughPM = true;
                        break;
                    }
                }
            }
        }

        if (!isEnoughAM && !isEnoughPM)
        {
            lackAM = Mathf.CeilToInt(_lackM / 2);
            lackPM = Mathf.CeilToInt(_lackM / 2);
        }
        else
        {
            if (!isEnoughAM) lackAM = _lackM;
            if (!isEnoughPM) lackPM = _lackM;
        }


        List<Dictionary<int, int>> listCost = new List<Dictionary<int, int>>();
        bool isSucceed;
        if(lackwater == 0 && lackAN == 0 && lackPN == 0 && lackAM == 0 && lackPM == 0)
        {
            if (costAN.Count > 0) listCost.Add(costAN);
            if (costPN.Count > 0) listCost.Add(costPN);
            if (costCN.Count > 0) listCost.Add(costCN);
            if (costAM.Count > 0) listCost.Add(costAM);
            if (costPM.Count > 0) listCost.Add(costPM);
            if (costCM.Count > 0) listCost.Add(costCM);

            //player.AddAsset(PlayerBagAsset.ItemType.Water, -costwater);
            //foreach(var data in costAN)
            //{
            //    var CS = DBManager.Instance.m_kItems.GetEntryPtr(data.Key);
            //    player.CosetItem(data.Key, (PlayerBagAsset.ItemType)CS._ItemType, data.Value);
            //}
            //foreach (var data in costPN)
            //{
            //    var CS = DBManager.Instance.m_kItems.GetEntryPtr(data.Key);
            //    player.CosetItem(data.Key, (PlayerBagAsset.ItemType)CS._ItemType, data.Value);
            //}
            //foreach (var data in costCN)
            //{
            //    var CS = DBManager.Instance.m_kItems.GetEntryPtr(data.Key);
            //    player.CosetItem(data.Key, (PlayerBagAsset.ItemType)CS._ItemType, data.Value);
            //}
            //foreach (var data in costAM)
            //{
            //    var CS = DBManager.Instance.m_kItems.GetEntryPtr(data.Key);
            //    player.CosetItem(data.Key, (PlayerBagAsset.ItemType)CS._ItemType, data.Value);
            //}
            //foreach (var data in costPM)
            //{
            //    var CS = DBManager.Instance.m_kItems.GetEntryPtr(data.Key);
            //    player.CosetItem(data.Key, (PlayerBagAsset.ItemType)CS._ItemType, data.Value);
            //}
            //foreach (var data in costCM)
            //{
            //    var CS = DBManager.Instance.m_kItems.GetEntryPtr(data.Key);
            //    player.CosetItem(data.Key, (PlayerBagAsset.ItemType)CS._ItemType, data.Value);
            //}

 
            //for (int i = 0; i < listBaseData.Count; i++)
            //{
            //    BaseData _baseData = listBaseData[i];
            //    ModelBase _modelBase = _baseData.GetComponent<ModelBase>();
            //    _modelBase.FillOnceProVal(StatePro.Hunger);
            //    _modelBase.FillOnceProVal(StatePro.Thirst);
            //    _modelBase.RemoveMyselfDeBuff();
            //}
 
            //World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.SetTask(TaskType.Feed, 1);


       

      

            isSucceed = true;
        }
        else
        {
            CS_Items.DataEntry cid_baseAN = null;
            CS_Items.DataEntry cid_basePN = null;
            CS_Items.DataEntry cid_baseCN = null;
            CS_Items.DataEntry cid_baseAM = null;
            CS_Items.DataEntry cid_basePM = null;
            CS_Items.DataEntry cid_baseCM = null;


            Dictionary<int, int> _costAN = new Dictionary<int, int>();
            Dictionary<int, int> _costPN = new Dictionary<int, int>();
            Dictionary<int, int> _costCN = new Dictionary<int, int>();//通用饲料
            Dictionary<int, int> _costAM = new Dictionary<int, int>();
            Dictionary<int, int> _costPM = new Dictionary<int, int>();
            Dictionary<int, int> _costCM = new Dictionary<int, int>();//通用药料


            var ItemList = DBManager.Instance.m_kItems.m_kDataEntryTable.GetEnumerator();
            while (ItemList.MoveNext())
            {
                if (cid_baseAN == null && ItemList.Current.Value._ItemType == (int)PlayerBagAsset.ItemType.AnimaiNutrients) cid_baseAN = ItemList.Current.Value;
                else if (cid_basePN == null && ItemList.Current.Value._ItemType == (int)PlayerBagAsset.ItemType.BotanyNutrients) cid_basePN = ItemList.Current.Value;
                else if (cid_baseCN == null && ItemList.Current.Value._ItemType == (int)PlayerBagAsset.ItemType.Nutrients) cid_baseCN = ItemList.Current.Value;
                else if (cid_baseAM == null && ItemList.Current.Value._ItemType == (int)PlayerBagAsset.ItemType.AnimaiMedicine) cid_baseAM = ItemList.Current.Value;
                else if (cid_basePM == null && ItemList.Current.Value._ItemType == (int)PlayerBagAsset.ItemType.BotanyMedicine) cid_basePM = ItemList.Current.Value;
                else if (cid_baseCM == null && ItemList.Current.Value._ItemType == (int)PlayerBagAsset.ItemType.Medicine) cid_baseCM = ItemList.Current.Value;
            }
          

            if(lackAN > 0)
            {
                //没有动物饲料
                if (cid_baseAN == null)
                {
                    int _val = int.Parse(cid_baseCN._Use.Split(' ')[2]);
                    int _num = Mathf.CeilToInt(lackAllAN / _val);
                    _costCN.Add(cid_baseCN._ID, _num);
                }
                else
                {
                    int _val = int.Parse(cid_baseAN._Use.Split(' ')[2]);
                    int _num = Mathf.CeilToInt(lackAllAN / _val);
                    _costAN.Add(cid_baseAN._ID, _num);
                }
            }

            if(lackPN > 0)
            {
                //没有植物饲料
                if (cid_basePN == null)
                {
                    int _val = int.Parse(cid_baseCN._Use.Split(' ')[2]);
                    int _num = Mathf.CeilToInt(lackAllPN / _val);
                    if (_costCN.ContainsKey(cid_baseCN._ID))
                        _costCN[cid_baseCN._ID] += _num;
                    else
                        _costCN.Add(cid_baseCN._ID, _num);
                }
                else
                {
                    int _val = int.Parse(cid_basePN._Use.Split(' ')[2]);
                    int _num = Mathf.CeilToInt(lackAllPN / _val);
                    _costPN.Add(cid_basePN._ID, _num);
                } 
            }


            if (lackAM > 0)
            {
                //没有动物药剂
                if (cid_baseAM == null)
                {
                    int _val = int.Parse(cid_baseCM._Use.Split(' ')[2]);
                    int _num = Mathf.CeilToInt(lackAllAM / _val);
                    _costCM.Add(cid_baseCM._ID, _num);
                }
                else
                {
                    int _val = int.Parse(cid_baseAM._Use.Split(' ')[2]);
                    int _num = Mathf.CeilToInt(lackAllAM / _val);
                    _costAM.Add(cid_baseAM._ID, _num);
                }
            }

            if (lackPM > 0)
            {
                //没有植物饲料
                if (cid_basePM == null)
                {
                    int _val = int.Parse(cid_baseCM._Use.Split(' ')[2]);
                    int _num = Mathf.CeilToInt(lackAllPM / _val);
                    if (_costCM.ContainsKey(cid_baseCM._ID))
                        _costCM[cid_baseCM._ID] += _num;
                    else
                        _costCM.Add(cid_baseCM._ID, _num);
                }
                else
                {
                    int _val = int.Parse(cid_basePM._Use.Split(' ')[2]);
                    int _num = Mathf.CeilToInt(lackAllPM / _val);
                    _costPM.Add(cid_basePM._ID, _num);
                }
            }

   

            if (_costAN.Count > 0) listCost.Add(_costAN);
            if (_costPN.Count > 0) listCost.Add(_costPN);
            if (_costCN.Count > 0) listCost.Add(_costCN);
            if (_costAM.Count > 0) listCost.Add(_costAM);
            if (_costPM.Count > 0) listCost.Add(_costPM);
            if (_costCM.Count > 0) listCost.Add(_costCM);

            // //单次喂养
            //if(listBaseData.Count == 1)
            //{

            //}
            //else
            //{
                
            //}

            isSucceed = false;
        }
       

        UIEntity uiFast = World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPopUpWindow_FastFeed);
        uiFast.GetComponent<UIPopUpWindow_FastFeedComponent>().Init(costwater, listCost);

        return isSucceed;
    }


}
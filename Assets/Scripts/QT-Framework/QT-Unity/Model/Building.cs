using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 

namespace QTFramework
{
    [System.Serializable]
    public class BuildingServer
    {
        public string guid;
        public string BeginTime;
        public BuildState buildState;//建造状态
        public int lv;//等级
        public int cfg_id;
    }
 

 

    public class Building : ModelBase
    {
        public CS_UpLevel.DataEntry CS_UpLevel;
        BuildingServer Server;
        public BuildingServer GetServer
        {
            get { return Server; }
        }
        bool _ispause;

        public DateTime BeginPause;
        public DateTime EndPause;
        public float passTime;//暂停流失时间
 
        public bool IsPause
        {
            get { return _ispause; }
            set { _ispause = value;  }
        }

        void Awake()
        {
 
        
        }

        // Update is called once per frame
        void Update()
        {

  

        }

    
        public void Init(BuildingServer _server = null, BaseData _baseData = null)
        {
            if (Server == null)
                Server = new BuildingServer();
            baseData = _baseData;
            IsPause = false;
            CS_UpLevel = DBManager.Instance.m_kUpLv.GetEntryPtr(baseData.cfg._UpLvID);

          
            if (_server != null)
            {
                Server = _server;
                TimeSpan ts = DateTime.Now.Subtract(DateTime.Parse(Server.BeginTime));
                if (Server.buildState == BuildState.Sustain)
                {
                    if (ts.TotalSeconds >= baseData.cfg._BuildTime)
                        EndBuilding();
                }
                else if (Server.buildState == BuildState.UpLv)
                {
                    if (ts.TotalSeconds >= CS_UpLevel._UpTime)
                        FrameEndUpLevel();
                }
            }
            else  
            {
                Server.cfg_id = baseData.cfg._ID;
                Server.buildState = BuildState.Idle;
                Server.lv = 1;
                Server.guid = baseData.guid.ToString();
                Server.BeginTime = DateTime.MaxValue.ToString();
            }

        
        }
     
 

        public void BeginBuilding()
        {
            if(baseData.cfg._BuildTime == 0)
                Server.buildState = BuildState.Idle;
            else
                Server.buildState = BuildState.Sustain;
            if(Server.BeginTime.Equals(DateTime.MaxValue.ToString()))
                Server.BeginTime = DateTime.Now.ToString();
            if(DataManager._instance.GetLoalData<BuildingServer>(Guid.Parse(Server.guid)) == null)
                DataManager._instance.AddLocalData<BuildingServer>(Server.guid, Server);
            else
                DataManager._instance.FixLocalData<BuildingServer>(Server.guid, Server);

        }

        public void EndBuilding()
        {
            Server.buildState = BuildState.Idle;
            Server.BeginTime = DateTime.MaxValue.ToString();
            DataManager._instance.FixLocalData<BuildingServer>(Server.guid, Server);
        }

        public void BeginUpLevel()
        {
            if (CS_UpLevel._UpTime == 0)
                Server.buildState = BuildState.Idle;
            else
                Server.buildState = BuildState.UpLv;
            if (Server.BeginTime.Equals(DateTime.MaxValue.ToString()))
                Server.BeginTime = DateTime.Now.ToString();
            DataManager._instance.FixLocalData<BuildingServer>(Server.guid, Server);
        }

        IEnumerator FrameEndUpLevel()
        {
            yield return new WaitForEndOfFrame();
            EndUpLevel();
        }

        public void EndUpLevel()
        {
            Server.buildState = BuildState.Idle;
            Server.lv += 1;
            Server.BeginTime = DateTime.MaxValue.ToString();
            if(CS_UpLevel._NextID == 0)//不能升级了
            {
                
            }
            else
            {
                baseData.cfg = DBManager.Instance.m_kModel.GetEntryPtr(CS_UpLevel._NextID);
                GameObject goOld = baseData.go;
                baseData.go = ModelManager._instance.ReplaceModel(baseData);
                CS_UpLevel = DBManager.Instance.m_kUpLv.GetEntryPtr(baseData.cfg._UpLvID);
                Server.cfg_id = baseData.cfg._ID;
                Building building = baseData.GetComponent<Building>();
                building.baseData = baseData;
                building.CS_UpLevel = CS_UpLevel;
                building.Server = Server;
                MapGridMgr.Instance.ReplaceBuilding(goOld, building.gameObject);
            }
            Debug.Log("__升级完成__" + Server.cfg_id);
            DataManager._instance.FixLocalData<BuildingServer>(Server.guid, Server);
            DataManager._instance.ChangeMapData(baseData.cfg._ID);
            GameEventManager._Instance.onUpgradeBuild();

            if (baseData.cfg._Ctype == (int)ModelCType.WaterPool) 
                SceneLogic._instance.SetWaterPool();


        }

 

    }

}


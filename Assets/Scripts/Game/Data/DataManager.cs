using QTFramework;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
 
using System.IO;
using System.Text;
using ParkGameEvent;

[ObjectEventSystem]
public class DataManagerAwakeSystem : AAwake<DataManager>
{
    public override void Awake(DataManager _self)
    {
        _self.Awake();
    }
}

[ObjectEventSystem]
public class DataManagerFixedUpdateSystem : AFixedUpdate<DataManager>
{
    public override void FixedUpdate(DataManager _self)
    {
        _self.FixedUpdate();
    }
}

enum DataType
{
    MapDaTa = 1,
    PlayerBase = 2,

}



public class LocalData
{
    Dictionary<string, Dictionary<string, object>> listData = new Dictionary<string, Dictionary<string, object>>();


    public bool InitData(string tname, string guid, object obj)
    {
        if (!listData.ContainsKey(tname))
        {
            listData[tname] = new Dictionary<string, object>();
            Log.Debug("__添加了新的数据列表__" + tname);
        }
        else
        {
            if (listData[tname].ContainsKey(guid))
            {
                //Log.Debug(" __已经有这个数据了 :__ " + guid);
                return false;
            }
        }
    
        listData[tname].Add(guid, obj);
 
        return true;
    }
 

    public Dictionary<string, object> GetListByT<T>()
    {
        if (!listData.ContainsKey(typeof(T).Name))
        {
            //Debug.LogError("__没有这个数据列表__" + typeof(T).Name);
            return new Dictionary<string, object>();
        }
        return listData[typeof(T).Name];
    }

    public Dictionary<string, Dictionary<string, object>> GetList()
    {
        return listData;
    }

    public Dictionary<string, T> GetDicData<T>(Dictionary<string, object> obj = null)
    {
        Dictionary<string, T> dicT = new Dictionary<string, T>();
        if (obj == null)
            obj = GetListByT<T>();
        foreach (var data in obj)
        {
            dicT.Add(data.Key, (T)data.Value);
        }
        return dicT;
    }



}



public class DataManager : QTComponent 
{
 
    //纠正的数据(地编上有的数据)
    public Dictionary<string, List<Guid>> listFix = new Dictionary<string, List<Guid>>();

    //JsonData
    public Dictionary<string, StringBuilder> listJsonData = new Dictionary<string, StringBuilder>();
 

    //地图的基础信息.  数据是地编导出的
    Dictionary<string, string> BaseMapData = new Dictionary<string, string>();
    //有服务器改变的数据
    List<string> listServerChange = new List<string>();
    //有地编改变的数据
    List<string> listMapChange = new List<string>();

    LocalData localData;
    DateTime startTime;
    bool bisBeginSave;
    string dataPath;

 

    public static DataManager _instance = null;
    public WastelandManager wasteland;

    public void Awake()
    {
        _instance = this;
        wasteland = new WastelandManager();
        localData = new LocalData();
        startTime = DateTime.Now;
        bisBeginSave = false;
        listServerChange.Clear();
        listMapChange.Clear();
        listFix.Clear();

      
        dataPath = Application.dataPath;
        //SaveCount = 1;
        InitMapData();
        bool bisLocal = true;
        if (bisLocal)// 取本地数据
        {
            InitLocalData();

        }
        else
        {
          
        }

        Json.OnChanageEnd = (name) => {
            listServerChange.Remove(name);
            //Debug.LogError("__listServerChange__" + listServerChange.Count);
        };
 
    }

 

    //int SaveCount = 1;
    public void FixedUpdate()
    {
        //return;
        TimeSpan _ts = DateTime.Now.Subtract(startTime);
        if (_ts.TotalSeconds > 3)
            bisBeginSave = true;
        
        if (!bisBeginSave)
            return;
        
        DateTime old = startTime;
        TimeSpan ts = DateTime.Now.Subtract(old);
        if (ts.TotalSeconds >= GenerateID.SaveRate)//5秒存一次数据
        {
    
            if (listServerChange.Count > 0)
            {
                ////////////////// 数据备份 /////////////////////
                //var list_data = localData.GetList();
                //foreach (var name in listServerChange)
                //{
                    //string str = "aaa";
                    //string str = Json.ToJson(list_data[name]);
                    //Debug.Log("~~len~~" + str.Length + "___name__" + name + "__count__");
                    //string val = Json.GZipCompressString(str);
                    //PPDataSetString(name, val);
                //}
                ////////////////////////////////////////////////
                App.Instance.StartCoroutine(SaveJsonServer());
         
          
            }

            if (listMapChange.Count > 0)
            {
                foreach (string name in listMapChange)
                {
                    SaveMapData(name);
                }
                listMapChange.Clear();
            }
 
            startTime = DateTime.Now; 
        }
    }

    //存服务器数据
    public IEnumerator SaveJsonServer()
    {
        yield return null;
        for (int i = listServerChange.Count - 1; i >= 0; i--)
        {
            string file_name = listServerChange[i];
            string str = "";

            if(BisSaveString(file_name))
            {
                StringBuilder stringBuilder = listJsonData[file_name];
                str = stringBuilder.ToString();
            }
            else
            {
                Dictionary<string, Dictionary<string, object>> list_data = localData.GetList();
                if (list_data.ContainsKey(file_name))
                {
                    Dictionary<string, object> obj = list_data[file_name];
                    switch (file_name)
                    {
                        case "PlayerBagAsset":
                            Dictionary<string, PlayerBagAsset> dicBagAsset = localData.GetDicData<PlayerBagAsset>(obj);
                            Dictionary<string, PlayerBagAssetServer> dicTempBagAsset = new Dictionary<string, PlayerBagAssetServer>();
                            foreach (var data in dicBagAsset)
                            {
                                PlayerBagAssetServer asset = new PlayerBagAssetServer();
                                asset.ID = data.Value.ID;
                                asset.BagVolume = data.Value.BagVolume;
                                asset.m_kBag = Json.ToJsonList<PlayerBagAsset.BagItem>(data.Value.m_kBag);
                                dicTempBagAsset.Add(data.Key, asset);
                            }
                            str = Json.ToJson(dicTempBagAsset);
                            break;
                        case "PlayerBasicAsset":
                            Dictionary<string, PlayerBasicAsset> dicBasicAsset = localData.GetDicData<PlayerBasicAsset>(obj);
                            Dictionary<string, PlayerBasicAssetServer> dicTempBasicAsset = new Dictionary<string, PlayerBasicAssetServer>();
                            foreach (var data in dicBasicAsset)
                            {
                                PlayerBasicAssetServer asset = new PlayerBasicAssetServer();
                                asset.ID = data.Value.ID;
                                asset.m_kName = data.Value.m_kName;
                                asset.m_kExp = data.Value.m_kExp;
                                asset.m_kLevel = data.Value.m_kLevel;
                                asset.m_nWastedLand = data.Value.m_nWastedLand;
                                asset.m_kStone = (uint)data.Value.m_kStone;
                                asset.m_kSun = (uint)data.Value.m_kSun;
                                asset.m_kGold = (uint)data.Value.m_kGold;
                                asset.m_kWater = (uint)data.Value.m_kWater;
                                asset.m_nWastedLand = (uint)data.Value.m_nWastedLand;
                                asset.m_beginWaste = data.Value.m_beginWaste.ToString();
                                asset.m_lWasteLand = "";
                                for (int j = 0; j < data.Value.m_lWasteLand.Count; j++)
                                {
                                    asset.m_lWasteLand = Json.DealString(asset.m_lWasteLand, 1, data.Value.m_lWasteLand[j].ToString());
                                }
                                asset.m_IsCanAssistant = data.Value.m_IsCanAssistant;
                                asset.m_ToatlGameTime = data.Value.m_ToatlGameTime;
                                asset.m_OnLineTime = data.Value.m_OnLineTime.ToString();
                                asset.m_OfflineTime = data.Value.m_OfflineTime.ToString();
                                asset.m_CreateAccountTime = data.Value.m_CreateAccountTime.ToString();
    
                                dicTempBasicAsset.Add(data.Key, asset);
                            }
                            str = Json.ToJson(dicTempBasicAsset);
                            break;
                        case "SettlementsAsset":
                            Dictionary<string, SettlementsAsset> dicSettle = localData.GetDicData<SettlementsAsset>(obj);
                            Dictionary<string, SettlementsAssetServer> dicTempSettle = new Dictionary<string, SettlementsAssetServer>();
                            foreach (var data in dicSettle)
                            {
                                SettlementsAssetServer asset = new SettlementsAssetServer();
                                asset.ID = data.Value.ID;
                                asset.m_kListSettlement = "";
                                for (int j = 0; j < data.Value.m_kListSettlement.Count; j++)
                                {
                                    asset.m_kListSettlement = Json.DealString(asset.m_kListSettlement, 1, data.Value.m_kListSettlement[j].ToString());
                                }
                                dicTempSettle.Add(data.Key, asset);
                            }
                            str = Json.ToJson(dicTempSettle);
                            break;

                        case "OneGameEvent":
                            Dictionary<string, OneGameEvent> GameEventDic = localData.GetDicData<OneGameEvent>(obj);
                            Dictionary<string, OneGameEventServer> dicTempEvent = new Dictionary<string, OneGameEventServer>();
                            foreach (var data in GameEventDic)
                            {
                                OneGameEventServer asset = new OneGameEventServer();
                                asset._ID = data.Value.DataEntry._ID.ToString();
                                asset._ConditionType = data.Value.DataEntry._ConditionType;
                                asset._ConditionValue = Json.Vector3ToStr(data.Value.DataEntry._ConditionValue);
                                asset._ResultType = data.Value.DataEntry._ResultType;
                                asset._ResultValue = Json.Vector3ToStr(data.Value.DataEntry._ResultValue);
                                asset._CheckType = data.Value.DataEntry._CheckType;
                                asset._RegType = data.Value.DataEntry._RegType;
                                asset._Count = data.Value.DataEntry._Count;
                                asset._DestoryType = data.Value.DataEntry._DestoryType;
                                dicTempEvent.Add(data.Key, asset);
                            }
                            str = Json.ToJson(dicTempEvent);
                            break;
                        case "RecordEventItem":
                            Dictionary<string, RecordEventItem> RecordEventItemDic = localData.GetDicData<RecordEventItem>(obj);
                            Dictionary<string, RecordEventItemServer> dicTempRecordEventItem = new Dictionary<string, RecordEventItemServer>();
                            foreach (var data in RecordEventItemDic)
                            {
                                RecordEventItemServer asset = new RecordEventItemServer();
                                asset._ID = data.Key;
                                if (data.Value.finishTime == default(DateTime)) asset.finishTime = "";
                                else asset.finishTime = data.Value.finishTime.ToString();
                                if (data.Value.registerTime == default(DateTime)) asset.registerTime = "";
                                else asset.registerTime = data.Value.registerTime.ToString();
                                asset.registerOnlineTime = data.Value.registerOnlineTime;
                                dicTempRecordEventItem.Add(data.Key, asset);
                            }
                            str = Json.ToJson(dicTempRecordEventItem);
                            break;

                    }
                }
 
            }

     
          
            string path = GetDataInfoPath(file_name);
            str = Json.GZipCompressString(str);
            FixJsonData(file_name, str, path);
        }
    }
 

    ////存服务器数据
    //public IEnumerator QQSaveJsonServer()
    //{
    //    yield return null;
    //    if (listServerChange.Count > 0)
    //    {
    //        var list_data = localData.GetList();
    //        for (int i = listServerChange.Count - 1; i >= 0; i--)
    //        {
    //            string file_name = listServerChange[i];
    //            if (file_name != null && file_name!= "")
    //            {
    //                if (list_data.ContainsKey(file_name))
    //                {
    //                    Dictionary<string, object> obj = list_data[file_name];
           
    //                    string str = "";
    //                    switch (file_name)
    //                    {
    //                        case "BuildingServer":
    //                            str = Json.ToJson(localData.GetDicData<BuildingServer>(obj));
    //                            break;
    //                        case "ParkServer":
    //                            str = Json.ToJson(localData.GetDicData<ParkServer>(obj));
    //                            break;
    //                        case "ActorServer":
    //                            str = Json.ToJson(localData.GetDicData<ActorServer>(obj));
    //                            break;
    //                        case "AnimalServer":
    //                            str = Json.ToJson(localData.GetDicData<AnimalServer>(obj));
    //                            break;
    //                        case "PlantServer":
    //                            str = Json.ToJson(localData.GetDicData<PlantServer>(obj));
    //                            break;
    //                        case "PlayerBagAsset":
    //                            Dictionary<string, PlayerBagAsset> dicBagAsset = localData.GetDicData<PlayerBagAsset>(obj);
    //                            Dictionary<string, PlayerBagAssetServer> dicTempBagAsset = new Dictionary<string, PlayerBagAssetServer>();
    //                            foreach (var data in dicBagAsset)
    //                            {
    //                                PlayerBagAssetServer asset = new PlayerBagAssetServer();
    //                                asset.ID = data.Value.ID;
    //                                asset.BagVolume = data.Value.BagVolume;
    //                                asset.m_kBag = Json.ToJsonList<PlayerBagAsset.BagItem>(data.Value.m_kBag);
                                   
    //                                dicTempBagAsset.Add(data.Key, asset);
    //                            }
    //                            str = Json.ToJson(dicTempBagAsset);
    //                            break;
    //                        case "PlayerBasicAsset":
    //                            Dictionary<string, PlayerBasicAsset> dicBasicAsset = localData.GetDicData<PlayerBasicAsset>(obj);
    //                            Dictionary<string, PlayerBasicAssetServer> dicTempBasicAsset = new Dictionary<string, PlayerBasicAssetServer>();
    //                            foreach(var data in dicBasicAsset)
    //                            {
    //                                PlayerBasicAssetServer asset = new PlayerBasicAssetServer();
    //                                asset.ID = data.Value.ID;
    //                                asset.m_kName = data.Value.m_kName;
    //                                asset.m_kExp = data.Value.m_kExp;
    //                                asset.m_kLevel = data.Value.m_kLevel;
    //                                asset.m_kStone = (uint)data.Value.m_kStone;
    //                                asset.m_kSun = (uint)data.Value.m_kSun;
    //                                asset.m_kGold = (uint)data.Value.m_kGold;
    //                                asset.m_kWater = (uint)data.Value.m_kWater;
    //                                //asset.m_kSpade = (uint)data.Value.m_kSpade;
    //                                asset.m_beginWaste = data.Value.m_beginWaste.ToString();
    //                                asset.m_lWasteLand = "";
    //                                for (int j = 0; j < data.Value.m_lWasteLand.Count; j++)
    //                                {
    //                                    asset.m_lWasteLand = Json.DealString(asset.m_lWasteLand, 1, data.Value.m_lWasteLand[j].ToString());
    //                                }
    //                                //Debug.Log("__str_" + asset.m_lWasteLand+ "__num__" + data.Value.m_lWasteLand.Count);
    //                                dicTempBasicAsset.Add(data.Key, asset);
    //                            }
    //                            str = Json.ToJson(dicTempBasicAsset);
    //                            break;
    //                        case "SettlementsAsset":
    //                            Dictionary<string, SettlementsAsset> dicSettle = localData.GetDicData<SettlementsAsset>(obj);
    //                            Dictionary<string, SettlementsAssetServer> dicTempSettle = new Dictionary<string, SettlementsAssetServer>();
    //                            foreach (var data in dicSettle)
    //                            {
    //                                SettlementsAssetServer asset = new SettlementsAssetServer();
    //                                asset.ID = data.Value.ID;
    //                                asset.m_kListSettlement = "";
    //                                for (int j = 0; j < data.Value.m_kListSettlement.Count; j++)
    //                                {
    //                                    asset.m_kListSettlement = Json.DealString(asset.m_kListSettlement, 1, data.Value.m_kListSettlement[j].ToString());
    //                                }
    //                                dicTempSettle.Add(data.Key, asset);
    //                            }
    //                            str = Json.ToJson(dicTempSettle);
    //                            break;
    //                        case "WastelandServer":
    //                            str = Json.ToJson(localData.GetDicData<WastelandServer>(obj)); 
    //                            break;
    //                    }
 
    //                    //Debug.LogError("~~len~~" + str.Length + "___name__" + file_name);
    //                    if (str == "")
    //                        continue;
    //                    string val = Json.GZipCompressString(str);
    //                    if (obj.Count == 0)
    //                        val = "";
    //                    string path = GetDataInfoPath(file_name);
    //                    FixJsonData(file_name, val, path);
    //                }
    //            }
    
    //        }
         
  
    //    }
  

    //}


    //存地编数据
    void SaveMapData(string file_name)
    {
        //Log.Debug("____地编开始存储____" + file_name);
        string val = MapGridMgr.Instance.ExportToJson(GetMapGridType(file_name));
        if (val == "" || val == null)
            return;
        string path = GetMapDataPath(file_name);
        FixJsonData(file_name, val, path);
    }

 
 
    //读取本地数据
    void InitLocalData()
    {
        listJsonData.Clear();
 

        string path = Json.pathDataInfo; //Application.persistentDataPath + "/JsonFile/DataInfo/";
        if (!File.Exists(path))
        {
            if (!Directory.Exists(path.Substring(0, path.LastIndexOf("/"))))
            {
                Directory.CreateDirectory(path.Substring(0, path.LastIndexOf("/")));
            }
        }
 

  


        DirectoryInfo dir = new DirectoryInfo(path);
        foreach (FileInfo file in dir.GetFiles("*.json"))
        {
            string file_name = file.Name.Split('.')[0];
            FileStream stream = new FileStream(file.FullName, FileMode.Open);
            int fsLen = (int)stream.Length;
            byte[] heByte = new byte[fsLen];
            stream.Read(heByte, 0, heByte.Length);
            string val = System.Text.Encoding.UTF8.GetString(heByte);
        
            //string val = file.OpenText().ReadToEnd().TrimStart();
            //if (val == null || val == "")//如果数据是空的
            //{
            //    val = PPDataGetString(file_name);//使用备份数据
            //    string file_path = GetDataInfoPath(file_name);
            //    Json.SaveJson(file_path, val);
            //}
 
            string str = Json.GZipDecompressString(val);//解压
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(str);
            listJsonData[file_name] = stringBuilder;
            stream.Close();
            stream.Dispose();
        }

   
        

     

  


        foreach (var JsonData in listJsonData)
        {
            if (JsonData.Value.Length != 0)
            {
                switch (JsonData.Key)
                {
                    case "BuildingServer":
                        foreach (var data in Json.FromJson<string, BuildingServer>(JsonData.Value.ToString()))
                        {
                            localData.InitData("BuildingServer", data.Key, data.Value);
                        }
                        break;
                    case "ParkServer":
                        foreach (var data in Json.FromJson<string, ParkServer>(JsonData.Value.ToString()))
                        {
                            localData.InitData("ParkServer", data.Key, data.Value);
                        }
                        break;
                    case "ActorServer":
                        foreach (var data in Json.FromJson<string, ActorServer>(JsonData.Value.ToString()))
                        {
                            localData.InitData("ActorServer", data.Key, data.Value);
                        }
                        break;
                    case "AnimalServer":
                        foreach (var data in Json.FromJson<string, AnimalServer>(JsonData.Value.ToString()))
                        {
                            localData.InitData("AnimalServer", data.Key, data.Value);
                        }
                        break;
                    case "PlantServer":
                        foreach (var data in Json.FromJson<string, PlantServer>(JsonData.Value.ToString()))
                        {
                            localData.InitData("PlantServer", data.Key, data.Value);
                        }
                        break;
                    case "PlayerBagAsset":
                        foreach (var data in Json.FromJson<string, PlayerBagAssetServer>(JsonData.Value.ToString()))
                        {
                            PlayerBagAsset asset = new PlayerBagAsset();
                            asset.ID = data.Value.ID;
                            asset.BagVolume = data.Value.BagVolume;
                            asset.m_kBag = Json.ListFromJson<PlayerBagAsset.BagItem>(data.Value.m_kBag);
                            localData.InitData("PlayerBagAsset", data.Key, asset);
                            break;
                        }
                        break;
                    case "PlayerBasicAsset":
                        foreach (var data in Json.FromJson<string, PlayerBasicAssetServer>(JsonData.Value.ToString()))
                        {
                            PlayerBasicAsset asset = new PlayerBasicAsset();
                            asset.ID = data.Value.ID;
                            asset.m_kName = data.Value.m_kName;
                            asset.m_kExp = data.Value.m_kExp;
                            asset.m_nWastedLand = data.Value.m_nWastedLand;
                            asset.m_kLevel = data.Value.m_kLevel;
                            asset.m_kStone = data.Value.m_kStone;
                            asset.m_kSun = data.Value.m_kSun;
                            asset.m_kGold = data.Value.m_kGold;
                            asset.m_kWater = data.Value.m_kWater;
                            asset.m_nWastedLand = data.Value.m_nWastedLand;
                            asset.m_ToatlGameTime = data.Value.m_ToatlGameTime;
                            asset.m_IsCanAssistant = data.Value.m_IsCanAssistant;
                            asset.m_OnLineTime = DateTime.Parse(data.Value.m_OnLineTime);
                            asset.m_OfflineTime = DateTime.Parse(data.Value.m_OfflineTime);
                            asset.m_CreateAccountTime = DateTime.Parse(data.Value.m_CreateAccountTime);
 
                            asset.m_lWasteLand = Json.StringAnalysisGuid(data.Value.m_lWasteLand);
                            if (string.IsNullOrEmpty(data.Value.m_beginWaste))
                                asset.m_beginWaste = default(DateTime);
                            else
                                asset.m_beginWaste = DateTime.Parse(data.Value.m_beginWaste);
                            localData.InitData("PlayerBasicAsset", data.Key, asset);
                            break;
                        }
                        break;
                    case "SettlementsAsset":
                        foreach (var data in Json.FromJson<string, SettlementsAssetServer>(JsonData.Value.ToString()))
                        {
                            SettlementsAsset asset = new SettlementsAsset();
                            asset.ID = data.Value.ID;
                            string strSettle = data.Value.m_kListSettlement;
                            asset.m_kListSettlement = Json.StringAnalysisGuid(data.Value.m_kListSettlement);
                            localData.InitData("SettlementsAsset", data.Key, asset);
                            break;
                        }
                        break;
                    case "WastelandServer":
                        List<WastelandServer> server = new List<WastelandServer>();
                        foreach (var data in Json.FromJson<string, WastelandServer>(JsonData.Value.ToString()))
                        {
                            server.Add(data.Value);
                            localData.InitData("WastelandServer", data.Key, data.Value);
                        }
                        wasteland.Init(server);
                        break;
            
                    case "OneGameEvent":
                        Dictionary<int, OneGameEvent> GameEventDic = new Dictionary<int, OneGameEvent>();
                        foreach (var data in Json.FromJson<string, OneGameEventServer>(JsonData.Value.ToString()))
                        {
                            OneGameEvent asset = new OneGameEvent();
                            asset.DataEntry = new CS_GameEvent.DataEntry();
                            asset.DataEntry._ID = int.Parse(data.Key);
                            asset.DataEntry._ConditionType = data.Value._ConditionType;
                            asset.DataEntry._ConditionValue = Json.StrToVector3(data.Value._ConditionValue);
                            asset.DataEntry._ResultType = data.Value._ResultType;
                            asset.DataEntry._ResultValue = Json.StrToVector3(data.Value._ResultValue);
                            asset.DataEntry._CheckType = data.Value._CheckType;
                            asset.DataEntry._RegType = data.Value._RegType;
                            asset.DataEntry._Count = data.Value._Count;
                            asset.DataEntry._DestoryType = data.Value._DestoryType;
                            localData.InitData("OneGameEvent", data.Key, asset);
                            GameEventDic.Add(asset.DataEntry._ID, asset);
                        }
                        GameEventManager._Instance.SetGameEvent(GameEventDic);

                        break;
                    case "RecordEventItem":
                        Dictionary<int, RecordEventItem> RecordEventItemDic = new Dictionary<int, RecordEventItem>();
                        foreach (var data in Json.FromJson<string, RecordEventItemServer>(JsonData.Value.ToString()))
                        {
                            RecordEventItem asset = new RecordEventItem();
                            if (string.IsNullOrEmpty(data.Value.finishTime))
                                asset.finishTime = default(DateTime);
                            else
                                asset.finishTime = DateTime.Parse(data.Value.finishTime);
                            if (string.IsNullOrEmpty(data.Value.registerTime))
                                asset.registerTime = default(DateTime);
                            else
                                asset.registerTime = DateTime.Parse(data.Value.registerTime);
                            asset.registerOnlineTime = data.Value.registerOnlineTime;
                            localData.InitData("RecordEventItem", data.Key, asset);
                            RecordEventItemDic.Add(int.Parse(data.Key), asset);
                        }
                        GameEventManager._Instance.SetRecord(RecordEventItemDic);
                        break;
                    default:
                        Debug.LogError("__没有这个东西啊__" + JsonData.Key);
                        break;
                }
            }

        }
 
    }

  
    public T ObjectByData<T>(object obj)
    {
        return (T)Convert.ChangeType(obj, typeof(T));
    }

    public T GetOnlyData<T>()
    {        
        foreach (var data in GetAllData<T>())
        {
            return (T)Convert.ChangeType(data.Value, typeof(T));
        }
        return default(T);
    }


    void InitMapData()
    {
        BaseMapData.Clear();
        string path = Json.pathMapData;//Application.persistentDataPath + "/JsonFile/MapData/";
        if (!File.Exists(path))
        {
            if (!Directory.Exists(path.Substring(0, path.LastIndexOf("/"))))
            {
                Directory.CreateDirectory(path.Substring(0, path.LastIndexOf("/")));
            }
        }
 

       //初始化的地编数据
        TextAsset[] allText = Resources.LoadAll<TextAsset>("MapData");
        foreach(var txt in allText)
        {
            string file_path = path + txt.name + ".json";
            if (!File.Exists(file_path))
            {
                Json.SaveJson(file_path, txt.text);
                BaseMapData[txt.name] = txt.text;
            }
            else
            {
                StreamReader reader = new StreamReader(file_path);
                string jsonData = reader.ReadToEnd();
                reader.Close();
                reader.Dispose();
                BaseMapData[txt.name] = jsonData;
            }
    
             
        }

      
        //DirectoryInfo dir = new DirectoryInfo(path);
        //FileInfo[] files = dir.GetFiles();
        //foreach (var file in files)
        //{
        //    if (file.Extension == ".json")
        //    {
        //        string val = file.OpenText().ReadToEnd().TrimStart();
        //        string file_name = file.Name.Split('.')[0];

        //        if (file_name.Contains("_"))
        //            BaseMapData[file_name] = val;
        //    }
        //}
    }


    public Dictionary<string, string> GetMapBaseData()
    {
        return BaseMapData;
    }
    public string GetMapDtatName(MapGridMgr.MapGridType mapGridType)
    {
        switch (mapGridType)
        {
            case MapGridMgr.MapGridType.Sea:
                return "Map_Sea";
            case MapGridMgr.MapGridType.Soil:
                return "Map_Soil";
            case MapGridMgr.MapGridType.Grass:
                return "Map_Grass";
            case MapGridMgr.MapGridType.Road:
                return "Map_Road";
            case MapGridMgr.MapGridType.Barrier:
                return "Map_Barrier";
            case MapGridMgr.MapGridType.Build:
                return "Map_Build";
            case MapGridMgr.MapGridType.Wastedland:
                return "Map_Wastedland";
            case MapGridMgr.MapGridType.Useable:
                return "Map_Useable";


            default: return null;
        }
    }
    public MapGridMgr.MapGridType GetMapGridType(string name)
    {
        switch (name)
        {
            case "Map_Sea":
                return MapGridMgr.MapGridType.Sea;
            case "Map_Soil":
                return MapGridMgr.MapGridType.Soil;
            case "Map_Grass":
                return MapGridMgr.MapGridType.Grass;
            case "Map_Road":
                return MapGridMgr.MapGridType.Road;
            case "Map_Barrier":
                return MapGridMgr.MapGridType.Barrier;
            case "Map_Build":
                return MapGridMgr.MapGridType.Build;
            case "Map_Wastedland":
                return MapGridMgr.MapGridType.Wastedland;
            case "Map_Useable":
                return MapGridMgr.MapGridType.Useable;
            default: return default(MapGridMgr.MapGridType);
        }
    }

    ///// <summary>
    ///// 创建json数据
    ///// </summary>
    ///// <param name="file_name">File name.</param>
    ///// <param name="obj">Object.</param>
    //public void CreateJsonData(string file_name, object obj, string file_path)
    //{
    //    if (listJsonData.ContainsKey(file_name))
    //    {
    //        Log.Debug("__这个数据已经存在了__文件名是: " + file_name);
    //        return;
    //    }
    //    string str = Json.ToJson(obj);
    //    listJsonData[file_name] = str;
    //    Json.SaveJson(file_path, str);
    //}


    public void CreateJsonData(string file_name, string val, string file_path)
    {
        if (listJsonData.ContainsKey(file_name))
        {
            Log.Debug("__这个数据已经存在了__文件名是: " + file_name);
            return;
        }
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(val);
        listJsonData[file_name] = stringBuilder;
        Json.SaveJson(file_path, val);
    }

    //public T GetDataFromJson<T>(string file_name)
    //{
    //    if (!listJsonData.ContainsKey(file_name))
    //    {
    //        Log.Debug("__没有这个数据__文件名是 :" + file_name);
    //        return default(T);
    //    }
    //    string root = typeof(T).Name;
    //    var obj = listJsonData[file_name];
    //    obj = Json.FromJson<T>(Json.ToJson(obj));
    //    return (T)Convert.ChangeType(obj, typeof(T));
    //}


    ///// <summary>
    ///// 修改json数据. 直接全部覆盖
    ///// </summary>
    ///// <param name="obj">Object.</param>
    //public void FixJsonData(string file_name, object obj, string file_path)
    //{
    //    if (file_name == null || file_name == "")
    //        return;

    //    if (!listJsonData.ContainsKey(file_name))
    //    {
    //        CreateJsonData(file_name, obj, file_path);
    //        return;
    //    }
    //    string str = Json.ToJson(obj);
    //    //Json.CreateFile(path, str);
    //    Json.SaveJson(file_path, str);
    //}

    public void FixJsonData(string file_name, string val, string file_path)
    {
        if (file_name == null || file_name == "")
            return;

        if (!listJsonData.ContainsKey(file_name))
        {
            CreateJsonData(file_name, val, file_path);
            return;
        }
        //Json.CreateFile(path, val);
        Json.SaveJson(file_path, val);
    }

    ///////////////////////////////////////////// 本地数据操作 ///////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public void NotificaServerChange(string name)
    {
        if (!listServerChange.Contains(name))
            listServerChange.Add(name);
    }
    public void NotificaMapChange(string name)
    {
        if (!listMapChange.Contains(name))
            listMapChange.Add(name);
    }

    //地编有改动
    public void ChangeMapData(int cfg_id)
    {
        string name = "";
        if (MapGridMgr.IsBarrier(cfg_id))
            name = GetMapDtatName(MapGridMgr.MapGridType.Barrier);
        else if (MapGridMgr.IsBuilding(cfg_id))
            name = GetMapDtatName(MapGridMgr.MapGridType.Build);
 
        
        if (name != "")
            NotificaMapChange(name);
    }

 
    bool BisSaveString(string name)
    {
        if (name == "BuildingServer" || name == "ParkServer" || name == "ActorServer" || name == "AnimalServer" || name == "PlantServer"
            || name == "WastelandServer")
        {
            return true;
        }
        return false;
    }


    bool BisOperation<T>(object data)
    {
        if (typeof(T).Name == "BuildingServer")
        {
            BuildingServer bs = data as BuildingServer;
            int cid = bs.cfg_id;
            //只存园区和建筑物
            if (MapGridMgr.IsBarrier(cid) || MapGridMgr.IsBuilding(cid))
                return true;
            else
                return false;
        }
  
        return true;
    }


    
    public void AddLocalData<T>(string guid, T data)
    {
        string nameT = typeof(T).Name;
        if (!BisOperation<T>(data)) return;
        bool isHave = localData.InitData(nameT, guid, data);
        if (!isHave) return;
            

        NotificaServerChange(nameT);
        if (!BisSaveString(nameT)) return;
        if (!listJsonData.ContainsKey(nameT)) listJsonData.Add(nameT, new StringBuilder());
        var strSB = listJsonData[nameT];
        string str = strSB.ToString();
        if (str == "")//一个都没有
        {
            Dictionary<string, T> dicT = new Dictionary<string, T>();
            dicT.Add(guid, data);
            str = Json.ToJson(dicT);
            strSB.Append(str);
            listJsonData[nameT] = strSB;
            return;
        }
        int idx01 = str.IndexOf("\"],\"");
        string strguid = ",\"" + guid + "\"";
        strSB = strSB.Insert(idx01 + 1, strguid);
        str = strSB.ToString();
        idx01 = str.IndexOf("}]");
        string strobj = Json.ToJson(data);
        strSB = strSB.Insert(idx01 + 1, "," + strobj);
        listJsonData[nameT] = strSB;
 
    }


    public void FixLocalData<T>(string guid, T data)
    {
        string nameT = typeof(T).Name;
        if (!BisOperation<T>(data)) return;
        var list = localData.GetListByT<T>();
        if (!list.ContainsKey(guid))
        {
            Log.Error("没有这个数据 :" + guid + "__name___" + nameT);
            return;
        }
        list[guid] = data;
        NotificaServerChange(nameT);
 
        if (!BisSaveString(nameT)) return;
        var strSB = listJsonData[nameT];
        string strNew = Json.ToJson(data);
        string str = strSB.ToString();
        int idx01 = str.IndexOf("\"guid\":\"" + guid);
        int idx03 = str.IndexOf("}", idx01);
        string strOld = str.Substring(idx01, idx03 - idx01);
        strOld = strOld.Insert(0, "{");
        strOld = strOld.Insert(strOld.Length, "}");
        listJsonData[nameT] = strSB.Replace(strOld, strNew);
    }



    public void DelLocalData<T>(string guid)
    {
        string nameT = typeof(T).Name;
        var list = localData.GetListByT<T>();
        if (!list.ContainsKey(guid))
        {
            Log.Error("没有这个数据 :" + guid + "__name___" + nameT);
            return;
        }
        list.Remove(guid);
        NotificaServerChange(nameT);

        if (!BisSaveString(nameT)) return;
        var strSB = listJsonData[nameT];
        string str = strSB.ToString();
        int idx01 = str.IndexOf(guid);
        string strFront = str.Substring(idx01 - 2, guid.Length + 4);
        string interval01 = strFront.Substring(0, 1);
        string interval02 = strFront.Substring(strFront.Length - 1, 1);
        //只有一个
        if (interval01 == "[" && interval02 == "]")
        {
            listJsonData[nameT] = strSB.Clear();
            return;
        }
        if (interval01 == "[")//数组第一个
            strSB = strSB.Remove(idx01 - 1, guid.Length + 3);
        else
            strSB = strSB.Remove(idx01 - 2, guid.Length + 3);
        str = strSB.ToString();
        idx01 = str.IndexOf("{\"guid\":\"" + guid);
        int idx02 = str.IndexOf("}", idx01);
        string strBehind = str.Substring(idx01 - 1, idx02 - idx01 + 3);
        interval01 = strBehind.Substring(0, 1);
        if (interval01 == "[")//数组第一个
            strSB = strSB.Remove(idx01, idx02 - idx01 + 2);
        else
            strSB = strSB.Remove(idx01 - 1, idx02 - idx01 + 2);
        listJsonData[nameT] = strSB;
 
    }



    //public void QQAddLocalData<T>(string guid, object data)
    //{
    //    if (!BisOperation<T>(data))  
    //        return;

    //    if(localData.InitData(typeof(T).Name, guid, data))
    //        NotificaServerChange(typeof(T).Name);

    //}

    //public void QQFixLocalData<T>(string guid, object data)
    //{
    //    if (!BisOperation<T>(data)) 
    //        return;
 
    //    var list = localData.GetListByT<T>();
    //    if (!list.ContainsKey(guid))
    //    {
    //        Log.Error("没有这个数据 :" + guid + "__name___" + typeof(T).Name);
    //        return;
    //    }
    //    list[guid] = data;
    //    NotificaServerChange(typeof(T).Name);
    //}

    //public void QQDelLocalData<T>(string guid)
    //{
    //    var list = localData.GetListByT<T>();
    //    if (!list.ContainsKey(guid))
    //    {
    //        Log.Error("没有这个数据 :" + guid + "__name___" + typeof(T).Name);
    //        return;
    //    }
    //    list.Remove(guid);
    //    NotificaServerChange(typeof(T).Name);
    //}

    public void DelLocalData(BaseData data)
    {
        if (data.cfg._Type == (int)ModeTyp.Actor)//人物
            DelLocalData<ActorServer>(data.guid.ToString());
        else if (data.cfg._Type == (int)ModeTyp.Building)//建筑
            DelLocalData<BuildingServer>(data.guid.ToString());
        else if (data.cfg._Type == (int)ModeTyp.Animal)//动物
            DelLocalData<AnimalServer>(data.guid.ToString());
        else if (data.cfg._Type == (int)ModeTyp.Plant)//植物
            DelLocalData<PlantServer>(data.guid.ToString());
    }

    public void AddLocalData(BaseData data)
    {
        if (data.cfg._Type == (int)ModeTyp.Actor)//人物
            AddLocalData<ActorServer>(data.guid.ToString(), data.go.GetComponent<Actor>().GetServer);
        else if (data.cfg._Type == (int)ModeTyp.Building)//建筑
            AddLocalData<BuildingServer>(data.guid.ToString(), data.go.GetComponent<Building>().GetServer);
        else if (data.cfg._Type == (int)ModeTyp.Animal)//动物
            AddLocalData<AnimalServer>(data.guid.ToString(), data.go.GetComponent<Animal>().GetServer);
        else if (data.cfg._Type == (int)ModeTyp.Plant)//植物
            AddLocalData<PlantServer>(data.guid.ToString(), data.go.GetComponent<Plant>().GetServer);

    }

    public int GetNumByCtype(int cfg_id)
    {
        CS_Model.DataEntry dataEntry = DBManager.Instance.m_kModel.GetEntryPtr(cfg_id);
        int num = 0;
 
        var dic = localData.GetDicData<BuildingServer>();
        foreach (var data in dic)
        {
            CS_Model.DataEntry entry = DBManager.Instance.m_kModel.GetEntryPtr(data.Value.cfg_id);
            if (entry._Ctype == dataEntry._Ctype) num = num + 1;
        }

        return num;
    }


    public int GetNumByCid(int cfg_id)
    {
        CS_Model.DataEntry dataEntry = DBManager.Instance.m_kModel.GetEntryPtr(cfg_id);
        int num = 0;
        if (dataEntry._Type == (int)ModeTyp.Actor)
        {
            num = 0;
            var dic = localData.GetDicData<ActorServer>();
            foreach (var data in dic)
            {
                if (data.Value.cfg_id == cfg_id) num = num + 1;
            }
        }
        else if (dataEntry._Type == (int)ModeTyp.Building)
        {
            num = 0;
            var dic = localData.GetDicData<BuildingServer>();
            foreach (var data in dic)
            {
                if(data.Value.cfg_id == cfg_id) num = num + 1;
            }
        }
        else if (dataEntry._Type == (int)ModeTyp.Animal)
        {
            num = 0;
            var dic = localData.GetDicData<AnimalServer>();
            foreach (var data in dic)
            {
                if (data.Value.cfg_id == cfg_id) num = num + 1;
            }
        }
        else if (dataEntry._Type == (int)ModeTyp.Plant)
        {
            num = 0;
            var dic = localData.GetDicData<PlantServer>();
            foreach (var data in dic)
            {
                if (data.Value.cfg_id == cfg_id) num = num + 1;
            }
        }
        return num;
    }

    public Dictionary<Guid, T> GetAllData<T>()
    {
        var list = localData.GetListByT<T>();
        Dictionary<Guid, T> dic = new Dictionary<Guid, T>();
        foreach (var data in list)
        {
            T cdata = (T)Convert.ChangeType(data.Value, typeof(T));
            dic.Add(Guid.Parse(data.Key), cdata);
        }
        return dic;
    }

    public T GetLoalData<T>(Guid _guid)
    {
        string guid = _guid.ToString();
        var list = localData.GetListByT<T>();
        if (!list.ContainsKey(guid))
            return default(T);

        if (!listFix.ContainsKey(typeof(T).Name))
            listFix.Add(typeof(T).Name, new List<Guid>());
        listFix[typeof(T).Name].Add(_guid);


        return (T)Convert.ChangeType(list[guid], typeof(T));
    }

    public object GetLoalData(Guid _guid)
    {
        string guid = _guid.ToString();
        var temp_list = localData.GetList();
        foreach (var list in temp_list)
        {
            foreach (var data in list.Value)
            {
                if (data.Key == guid)
                {
                    return data.Value;
                }
            }
        }
        return null;
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



    //void PPDataSetString(string key, string value)
    //{
    //    PlayerPrefs.SetString(key, value);
    //}

    //string PPDataGetString(string key)
    //{
    //    return PlayerPrefs.GetString(key);
    //}



    //获取地编和服务器不一样的数据
    public List<Guid> GetDiffFixData<T>()
    {
        if (!listFix.ContainsKey(typeof(T).Name))
            listFix.Add(typeof(T).Name, new List<Guid>());
        
        var _listFix = listFix[typeof(T).Name];
        var _listServer = localData.GetListByT<T>();
        List<Guid> tempServer = new List<Guid>();

        foreach (var server in _listServer)
        {
            tempServer.Add(Guid.Parse(server.Key));
        }

        List<Guid> temp = new List<Guid>();
        for (int i = 0; i < tempServer.Count; i++)
        {
            if (!_listFix.Contains(tempServer[i]))
            {
                temp.Add(tempServer[i]);
            }
        }
        return temp;
    }

    string GetMapDataPath(string file_name)
    {
        return Json.pathMapData + file_name + ".json";
    }

    string GetDataInfoPath(string file_name)
    {
        return Json.pathDataInfo + file_name + ".json";
    }
 

}
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using QTFramework;

using UnityEngine;

//public class WastelandServer
//{
//    public bool isFirst;
//    public string guid;
//    public List<Wasteland> wastelands;

//    public string beginTime;
//}

[System.Serializable]
public class WastelandServer
{
    public string guid;
    public string beginTime; //空是未开始, -1是开荒结束了
    public int cid;
}

public class WastelandDaseData : WastelandServer
{
    public BaseData baseData;
}

//public class WastelandDaseData  
//{
//    public BaseData baseData;

//    public void BeginReclama(DateTime now = default(DateTime))
//    {
//        var server = DataManager._instance.wasteland.GetServer;
//        for (int i = 0; i < server.wastelands.Count; i++)
//        {
//            if(server.wastelands[i].guid == guid)
//            {
//                server.wastelands[i].beginTime = DateTime.Now.ToString();
//                break;
//            }
//        }

//        DataManager._instance.wasteland.GetServer = server;
//        if (now == default(DateTime))
//            beginTime = DateTime.Now.ToString();
//        else
//            beginTime = now.ToString();

//        //DataManager._instance.FixLocalData<WastelandServer>(server.guid, server);
//    }

//    public void EndReclama()
//    {
//        var server = DataManager._instance.wasteland.GetServer;

//        for (int i = 0; i < server.wastelands.Count; i++)
//        {
//            if (server.wastelands[i].guid == guid)
//            {
//                server.wastelands.Remove(server.wastelands[i]);
//                break;
//            }
//        }
//        DataManager._instance.wasteland.GetServer = server;

//        var wasteland = DataManager._instance.wasteland.WastelandDatas;
//        for (int i = 0; i < wasteland.Count; i++)
//        {
//            if (wasteland[i].guid == guid)
//            {
//                wasteland.Remove(wasteland[i]);
//                break;
//            }
//        }
//        DataManager._instance.wasteland.WastelandDatas = wasteland;

//        //DataManager._instance.FixLocalData<WastelandServer>(server.guid, server);

//        //Debug.Log("___@@@@@__" + wasteland.Count);
//        //Debug.Log("___#####__" + server.wastelands.Count);
//    }

//}

//荒地单独处理
public class WastelandManager
{
    List<WastelandDaseData> lWastelandDaseData = new List<WastelandDaseData>();
    public void Init(List<WastelandServer> server)
    {
        lWastelandDaseData.Clear();
        if (server.Count != 0)
        {
            for (int i = 0; i < server.Count; i++)
            {
                WastelandDaseData wastelandDaseData = new WastelandDaseData();
                wastelandDaseData.baseData = null;
                wastelandDaseData.beginTime = server[i].beginTime;
                wastelandDaseData.guid = server[i].guid;
                wastelandDaseData.cid = server[i].cid;
                lWastelandDaseData.Add(wastelandDaseData);
            }
        }
    }

    public void AddBaseData(BaseData baseData)
    {
        bool isHave = false;
        for (int i = 0; i < lWastelandDaseData.Count; i++)
        {
            if (lWastelandDaseData[i].guid == baseData.guid.ToString())
            {
                lWastelandDaseData[i].baseData = baseData;
                isHave = true;
                break;
            }
        }

        if (!isHave)
        {
            WastelandDaseData wastelandDaseData = new WastelandDaseData();
            wastelandDaseData.beginTime = "";
            wastelandDaseData.guid = baseData.guid.ToString();
            wastelandDaseData.baseData = baseData;

            WastelandServer server = new WastelandServer();
            server.guid = wastelandDaseData.guid;
            server.beginTime = wastelandDaseData.beginTime;
            server.cid = baseData.cfg._ID;

            lWastelandDaseData.Add(wastelandDaseData);
            DataManager._instance.AddLocalData<WastelandServer>(server.guid, server);
            DataManager._instance.NotificaMapChange("Map_Wastedland");
        }

    }

    public void BeginReclama(Guid guid)
    {
        for (int i = 0; i < lWastelandDaseData.Count; i++)
        {
            WastelandDaseData wastelandDaseData = lWastelandDaseData[i];
            if (wastelandDaseData.guid == guid.ToString() && wastelandDaseData.beginTime == "")
            {
                WastelandServer server = new WastelandServer();
                wastelandDaseData.beginTime = DateTime.Now.ToString();
                server.beginTime = wastelandDaseData.beginTime;
                server.guid = wastelandDaseData.guid;
                server.cid = wastelandDaseData.baseData.cfg._ID;
                DataManager._instance.FixLocalData<WastelandServer>(server.guid, server);
                break;
            }

        }
    }

    public void EndReclama(Guid guid)
    {
        //Debug.Log("____6666__");
        for (int i = 0; i < lWastelandDaseData.Count; i++)
        {
            WastelandDaseData wastelandDaseData = lWastelandDaseData[i];
            if (wastelandDaseData.guid == guid.ToString())
            {
                WastelandServer server = new WastelandServer();
                wastelandDaseData.beginTime = "-1";
                server.beginTime = wastelandDaseData.beginTime;
                server.guid = wastelandDaseData.guid;
                server.cid = wastelandDaseData.baseData.cfg._ID;

                DataManager._instance.FixLocalData<WastelandServer>(server.guid, server);
                //Debug.Log("____1111____");
                World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.WasteLand(2, Guid.Parse(server.guid));
                World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.SetTask(TaskType.assart, 1);
                break;
            }
        }
    }

    public WastelandDaseData GetWastelandByGuid(Guid guid)
    {
        for (int i = 0; i < lWastelandDaseData.Count; i++)
        {
            if (lWastelandDaseData[i].guid == guid.ToString())
                return lWastelandDaseData[i];
        }
        return null;
    }

    //WastelandServer Server;
    //public WastelandServer GetServer
    //{
    //    get { return Server; }
    //    set { Server = value; }
    //}

    //List<WastelandDaseData> lWastelandDatas = new List<WastelandDaseData>();
    //public List<WastelandDaseData> WastelandDatas
    //{
    //    get { return lWastelandDatas; }
    //    set { lWastelandDatas = value; }
    //}

    //public static WastelandServer WastelandServer { get; internal set; }

    //public void Init(WastelandServer server = null)
    //{
    //    lWastelandDatas.Clear();
    //    if (server == null)
    //    {
    //        Server = new WastelandServer();
    //        WastelandServer wastelandServer = new WastelandServer();
    //        wastelandServer.guid = GenerateID.ID.ToString();
    //        wastelandServer.wastelands = new List<Wasteland>();
    //        Server = wastelandServer;
    //        Server.isFirst = true;
    //        //DataManager._instance.AddLocalData<WastelandServer>(Server.guid, Server);
    //    }
    //    else
    //    {
    //        for (int i = 0; i < server.wastelands.Count; i++)
    //        {
    //            WastelandDaseData data = new WastelandDaseData();
    //            data.baseData = null;
    //            data.guid = "";
    //            data.beginTime = server.wastelands[i].beginTime;
    //            lWastelandDatas.Add(data);
    //        }
    //        Server = server;
    //        Server.isFirst = false;

    //        //Debug.Log("__11111_@@@@@__" + lWastelandDatas.Count);
    //        //Debug.Log("__11111_#####__" + Server.wastelands.Count);
    //    }

    //}

    //public void Add(BaseData baseData)
    //{
    //    if (Server == null)
    //    {
    //        Init();
    //    }
    //    bool isHave = false;
    //    for (int i = 0; i < Server.wastelands.Count; i++)
    //    {
    //        if(Server.wastelands[i].guid == baseData.guid.ToString())
    //        {
    //            lWastelandDatas[i].baseData = baseData;
    //            lWastelandDatas[i].guid = baseData.guid.ToString();
    //            isHave = true;
    //            break; 
    //        }
    //    }

    //    if(!isHave && Server.isFirst)
    //    {
    //        Wasteland wasteland = new Wasteland();
    //        wasteland.guid = baseData.guid.ToString();
    //        wasteland.beginTime = "";
    //        Server.wastelands.Add(wasteland);
    //        WastelandDaseData wldata = new WastelandDaseData();
    //        wldata.guid = baseData.guid.ToString();
    //        wldata.beginTime = "";
    //        wldata.baseData = baseData;
    //        lWastelandDatas.Add(wldata);
    //    }

    //    //Debug.Log("_2222__@@@@@__" + lWastelandDatas.Count);
    //    //Debug.Log("__2222_#####__" + Server.wastelands.Count);
    //    //Debug.Log("____@~~~~~_" + GameObject.Find("Wastedland").transform.childCount);

    //}

    //public void FixServer(Guid guid)
    //{
    //    for (int i = 0; i < GetServer.wastelands.Count; i++)
    //    {
    //        if(GetServer.wastelands[i].guid == guid.ToString())
    //        {

    //        }
    //    }
    //}

    //public WastelandDaseData GetWastelandByGuid(Guid guid)
    //{
    //    for (int i = 0; i < lWastelandDatas.Count; i++)
    //    {
    //        if (lWastelandDatas[i].guid == guid.ToString())
    //            return lWastelandDatas[i];
    //    }
    //    return null;
    //}

}
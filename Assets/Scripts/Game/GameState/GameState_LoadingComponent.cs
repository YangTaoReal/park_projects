/***********************************************************
 * 游戏准备阶段，
 * 加载对应的游戏地图、初始化玩家数据
 * author:SmartCoder
 * *********************************************************/

using QTFramework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameState_LoadingComponent : StateBase
{
    private bool LoadingFinish = false;
    public override void Init()
    {
        base.Init();
        Log.Info("LC_GameState_Loading", "初始化");
    }

    public override void StateStart(StateMachineBase kStateMachine)
    {
        base.StateStart(kStateMachine);

        GuidanceManager.isGuidancing = PlayerPrefs.GetInt("isGuidancing", 0) == 0 ? false : true;
        //开始游戏初始化
        World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPage_Ready);

        CS_MapInfo.DataEntry _mapInfo = DBManager.Instance.m_kMapInfo.GetEntryPtr(1000000);
        if (_mapInfo != null)
        {
            Camera _camera = World.Scene.GetComponent<WorldManagerComponent>().m_kGameCamera;

            _camera.transform.parent.transform.localPosition =new Vector3( _mapInfo._CameraPosition.x*0.01f, _mapInfo._CameraPosition.y * 0.01f, _mapInfo._CameraPosition.z * 0.01f);
            _camera.transform.parent.localEulerAngles = _mapInfo._CameraRotation;
            _camera.fieldOfView = _mapInfo._FOV;
            World.Scene.GetComponent<SceneManagerComponent>().BeginLoad(_mapInfo._ResName, MapCallBack);
        }



        LoadingFinish = false;
        //这里读档这些
    }

    private void MapCallBack(AsyncOperation obj)
    {
        Debug.Log("地图加载完成。。。开始恢复");
        GameObject goMapGridMgr = GameObject.Find("MapGridMgr");
        MapGridMgr mapGridMgr = goMapGridMgr.GetComponent<MapGridMgr>();
        mapGridMgr.onLoaded = mapGridMgrOnLoaded;
        mapGridMgr.onLoading = mapGridMgrOnLoading;
        DataManager dataManager = World.Scene.GetComponent<DataManager>();
        Dictionary<string, string> listMapData = dataManager.GetMapBaseData();

        List<GameObject> listGO = new List<GameObject>();
        foreach (Transform go in goMapGridMgr.transform)
        {
            listGO.Add(go.gameObject);
        }

        mapGridMgr.BeginImport();
        foreach (var data in listMapData)
        {
            string com_name = data.Key.Split('_')[1];

            foreach (MapGridMgr.MapGridType suit in Enum.GetValues(typeof(MapGridMgr.MapGridType)))
            {
                if (com_name == suit.ToString())
                {
                    mapGridMgr.ImportFromJson(suit, data.Value);
                    break;
                }
            }
        }
        mapGridMgr.EndImport();

     
    }

    private void mapGridMgrOnLoading(int progressVal)
    {
        //Debug.Log("@@@@@@地图恢复到了"+ progressVal);
        ObserverHelper<int>.SendMessage(MessageMonitorType.RecoverData, this, new MessageArgs<int>(progressVal));
    }

    private void mapGridMgrOnLoaded()
    {
       // Debug.Log("@@@@@@@@地图恢复完成");
        ModelManager modelManager = World.Scene.GetComponent<ModelManager>();
        modelManager.Restore();
        SceneLogic._instance.SetWaterPool();
        //DataManager dataManager = World.Scene.GetComponent<DataManager>();
        //Debug.LogError("____" + dataManager.GetDiffFixData<BuildingServer>().Count);

        LoadingFinish = true;
    }


    public override StateProcessResult StateProcess(StateMachineBase kStateMachine)
    {
        if (LoadingFinish)
        {
            return StateProcessResult.Finish;
        }
        else
        {
            return StateProcessResult.Hold;
        }
 
    }

    public override void StateEnd(StateMachineBase kStateMachine)
    {
        World.Scene.GetComponent<GameStateMachineComponent>().SetNextState(World.Scene.GetComponent<GameStateMachineComponent>().GetComponent<GameState_PlayingComponent>());
        base.StateEnd(kStateMachine);
        World.Scene.GetComponent<UIManagerComponent>().Remove(UI_PrefabPath.m_sUIPage_Ready);
        SceneLogic._instance.isEnter = true;
     
    }
}

using QTFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

[ObjectEventSystem]
public class UIPage_BuildingOperationComponentAwakeSystem : AAwake<UIPage_BuildingOperationComponent>
{
    public override void Awake(UIPage_BuildingOperationComponent _self)
    {
        _self.Awake();
    }
}


[UIEntityComponent(UI_PrefabPath.m_sUIPage_BuildingOperation)]
public class UIPage_BuildingOperationComponent : UIComponent
{
    public Button m_kButtonExpantion;
    public Button m_kButtonUpgrade;
    public Button m_kButtonShop;
    public Button m_kButtonPlacement;
    public Button m_kButtonTemporary;
    public Button m_kButtonStorage;
    public Button m_kButtonGardenReName;
    public Button m_kButtonControlpanel;
    public Button m_kButtonOnekey;
    public Button m_kButtonDetails;
    public Button m_kButtonEdit;
    public RectTransform m_kRectTransformCabin;
    public RectTransform m_kRectTransformGarden;
    public RectTransform m_kRectTransformReservoir;
    public RectTransform m_kRectTransformAirport;
    
    public Text m_kTextTitle;
    public Text m_kTextLevel;

    public Text m_kTextExpantion;
    public Text m_kTextUpgrade;
    public Text m_kTextShop;
    public Text m_kTextPlacement;
    public Text m_kTextTemporary;
    public Text m_kTextStorage;
    public Text m_kTextGardenReName;
    public Text m_kTextControlpanel;
    public Text m_kTextOnekey;
    public Text m_kTextDetails;
    public Text m_kTextEdit;

    public Text m_textEntrepot;
    public Text m_textEntrepotNum;
    public Text m_textHouse;
    public Text m_textHouseNum;

    public Button m_kButtonClsoe;

    public Button m_btnFood;
    public Button m_btnWater;
    public Button m_btnSeek;
    public Button m_btnDanger;


    public Button m_btnState01;
    public Button m_btnState02;
    private TileInfo tileInfo;
    internal void Awake()
    {
        m_kButtonExpantion = m_kParentEntity.m_kUIPrefab.GetCacheComponent(0) as Button;
        m_kButtonUpgrade = m_kParentEntity.m_kUIPrefab.GetCacheComponent(1) as Button;
        m_kButtonShop = m_kParentEntity.m_kUIPrefab.GetCacheComponent(2) as Button;
        m_kButtonPlacement = m_kParentEntity.m_kUIPrefab.GetCacheComponent(3) as Button;
        m_kButtonTemporary = m_kParentEntity.m_kUIPrefab.GetCacheComponent(4) as Button;
        m_kButtonStorage = m_kParentEntity.m_kUIPrefab.GetCacheComponent(5) as Button;
        m_kButtonGardenReName = m_kParentEntity.m_kUIPrefab.GetCacheComponent(6) as Button;
        m_kButtonControlpanel = m_kParentEntity.m_kUIPrefab.GetCacheComponent(7) as Button;
        m_kButtonOnekey = m_kParentEntity.m_kUIPrefab.GetCacheComponent(8) as Button;

        m_kRectTransformCabin = m_kParentEntity.m_kUIPrefab.GetCacheComponent(9) as RectTransform;
        m_kRectTransformGarden = m_kParentEntity.m_kUIPrefab.GetCacheComponent(10) as RectTransform;
        m_kRectTransformReservoir = m_kParentEntity.m_kUIPrefab.GetCacheComponent(11) as RectTransform;
        m_kRectTransformAirport = m_kParentEntity.m_kUIPrefab.GetCacheComponent(12) as RectTransform;

        m_kTextTitle = m_kParentEntity.m_kUIPrefab.GetCacheComponent(13) as Text;
        m_kTextLevel = m_kParentEntity.m_kUIPrefab.GetCacheComponent(14) as Text;


        m_kTextExpantion = m_kParentEntity.m_kUIPrefab.GetCacheComponent(15) as Text;
        m_kTextUpgrade = m_kParentEntity.m_kUIPrefab.GetCacheComponent(16) as Text;
        m_kTextShop = m_kParentEntity.m_kUIPrefab.GetCacheComponent(17) as Text;
        m_kTextPlacement = m_kParentEntity.m_kUIPrefab.GetCacheComponent(18) as Text;
        m_kTextTemporary = m_kParentEntity.m_kUIPrefab.GetCacheComponent(19) as Text;
        m_kTextStorage = m_kParentEntity.m_kUIPrefab.GetCacheComponent(20) as Text;
        m_kTextGardenReName = m_kParentEntity.m_kUIPrefab.GetCacheComponent(21) as Text;
        m_kTextControlpanel = m_kParentEntity.m_kUIPrefab.GetCacheComponent(22) as Text;
        m_kTextOnekey = m_kParentEntity.m_kUIPrefab.GetCacheComponent(23) as Text;

        m_kButtonClsoe = m_kParentEntity.m_kUIPrefab.GetCacheComponent(24) as Button;
        m_kButtonDetails = m_kParentEntity.m_kUIPrefab.GetCacheComponent(25) as Button;
        m_kTextDetails = m_kParentEntity.m_kUIPrefab.GetCacheComponent(26) as Text;



        m_btnFood = m_kParentEntity.m_kUIPrefab.GetCacheComponent(27) as Button;
        m_btnWater = m_kParentEntity.m_kUIPrefab.GetCacheComponent(28) as Button;
        m_btnSeek = m_kParentEntity.m_kUIPrefab.GetCacheComponent(29) as Button;
        m_btnDanger = m_kParentEntity.m_kUIPrefab.GetCacheComponent(30) as Button;
 

        m_textEntrepot = m_kParentEntity.m_kUIPrefab.GetCacheComponent(31) as Text;
        m_textEntrepotNum = m_kParentEntity.m_kUIPrefab.GetCacheComponent(32) as Text;
        m_textHouse = m_kParentEntity.m_kUIPrefab.GetCacheComponent(33) as Text;
        m_textHouseNum = m_kParentEntity.m_kUIPrefab.GetCacheComponent(34) as Text;
 

        m_btnState01 = m_kParentEntity.m_kUIPrefab.GetCacheComponent(35) as Button;
        m_btnState02 = m_kParentEntity.m_kUIPrefab.GetCacheComponent(36) as Button;

        m_kButtonEdit = m_kParentEntity.m_kUIPrefab.GetCacheComponent(37) as Button;
        m_kTextEdit = m_kParentEntity.m_kUIPrefab.GetCacheComponent(38) as Text;

        m_kButtonExpantion.onClick.AddListener(OnButtonClick_Expantion);
        m_kButtonUpgrade.onClick.AddListener(OnButtonClick_Upgrade);
        m_kButtonShop.onClick.AddListener(OnButtonClick_Shop);
        m_kButtonPlacement.onClick.AddListener(OnButtonClick_Placement);
        m_kButtonTemporary.onClick.AddListener(OnButtonClick_Temporary);
        m_kButtonStorage.onClick.AddListener(OnButtonClick_Storage);
        m_kButtonGardenReName.onClick.AddListener(OnButtonClick_GardenReName);
        m_kButtonControlpanel.onClick.AddListener(OnButtonClick_Controlpanel);
        m_kButtonOnekey.onClick.AddListener(OnButtonClick_Onekey);
        m_kButtonDetails.onClick.AddListener(OnButtonClick_Details);
        m_kButtonClsoe.onClick.AddListener(OnButtonClick_Close);
        m_kButtonEdit.onClick.AddListener(OnButtonClick_Edit);


        m_btnFood.onClick.AddListener(OnBtnFood);
        m_btnWater.onClick.AddListener(OnBtnWater);
        m_btnSeek.onClick.AddListener(OnBtnSeek);
        m_btnDanger.onClick.AddListener(OnBtnDanger);

 
     
    }



    void OnBtnFood()
    {
        
    }

    void OnBtnWater()
    {

    }

    void OnBtnSeek()
    {

    }

    void OnBtnDanger()
    {

    }

 

    private void OnButtonClick_Details()
    {
        if (SceneLogic._instance.selectTileInfo == null) return;

        World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPage_ParkDetial);
    }

    private void OnButtonClick_Close()
    {
        World.Scene.GetComponent<UIManagerComponent>().Remove(UI_PrefabPath.m_sUIPage_BuildingOperation);
        BE.MobileRTSCam.instance.RestEditor();
    }

    public override void TranslateUI()
    {
        base.TranslateUI();
        m_kTextExpantion.text = UI_Helper.GetTextByLanguageID(115);
        m_kTextUpgrade.text = UI_Helper.GetTextByLanguageID(116);
        m_kTextShop.text = UI_Helper.GetTextByLanguageID(105);
        m_kTextPlacement.text = UI_Helper.GetTextByLanguageID(117);
        m_kTextTemporary.text = UI_Helper.GetTextByLanguageID(118);
        m_kTextStorage.text = UI_Helper.GetTextByLanguageID(119);
        m_kTextGardenReName.text = UI_Helper.GetTextByLanguageID(120);
        m_kTextControlpanel.text = UI_Helper.GetTextByLanguageID(121);
        m_kTextOnekey.text = UI_Helper.GetTextByLanguageID(122);
        m_kTextDetails.text = UI_Helper.GetTextByLanguageID(213);
        m_kTextEdit.text = UI_Helper.GetTextByLanguageID(253);
    }


    public override void Dispose()
    {
        base.Dispose();

        m_kButtonExpantion.onClick.RemoveListener(OnButtonClick_Expantion);

        m_kButtonUpgrade.onClick.RemoveListener(OnButtonClick_Upgrade);
        m_kButtonShop.onClick.RemoveListener(OnButtonClick_Shop);
        m_kButtonPlacement.onClick.RemoveListener(OnButtonClick_Placement);
        m_kButtonTemporary.onClick.RemoveListener(OnButtonClick_Temporary);
        m_kButtonStorage.onClick.RemoveListener(OnButtonClick_Storage);
        m_kButtonGardenReName.onClick.RemoveListener(OnButtonClick_GardenReName);
        m_kButtonControlpanel.onClick.RemoveListener(OnButtonClick_Controlpanel);
        m_kButtonOnekey.onClick.RemoveListener(OnButtonClick_Onekey);
        m_kButtonDetails.onClick.RemoveListener(OnButtonClick_Details);
        m_kButtonClsoe.onClick.RemoveListener(OnButtonClick_Close);
    }


    /// <summary>
    /// 左边的面板
    /// </summary>
    void LeftPanel()
    {
        Building building = tileInfo.baseData.GetComponent<Building>();
        CS_UpLevel.DataEntry level = DBManager.Instance.m_kUpLv.GetEntryPtr(building.GetServer.cfg_id);
        Player player = World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer;
        m_btnState02.gameObject.SetActive(false);
        if (tileInfo.baseData.cfg._Ctype == (int)ModelCType.MainBase)//小木屋
        {
            m_btnState02.gameObject.SetActive(true);
          
            List<Guid> _settlements = player.m_kSettlementsAsset.m_kListSettlement;
 
            m_textEntrepot.text = UI_Helper.GetTextByLanguageID(241);
            m_textEntrepotNum.text = player.GetPackLeftNumber() + "/" + player.m_kPlayerBagAsset.BagVolume;
            m_textHouse.text = UI_Helper.GetTextByLanguageID(118);
            m_textHouseNum.text = _settlements.Count.ToString();
           
            m_btnState01.image.sprite = UI_Helper.GetSprite("tbo_00");
            m_btnState02.image.sprite = UI_Helper.GetSprite("tbo_01");
        }
        else if(tileInfo.baseData.cfg._Ctype == (int)ModelCType.Barrier)//园区
        {
 
            Park park = ModelManager._instance.GetParkList(new List<Guid>() { Guid.Parse(building.GetServer.guid) });
            m_textEntrepot.text = UI_Helper.GetTextByLanguageID(240);
            int all = park.GetParkCapacity();
            m_textEntrepotNum.text = all - park.GetRemainingCapacity() + "/" + all;
            m_btnState01.image.sprite = UI_Helper.GetSprite("tbo_03");
        }
        else if (tileInfo.baseData.cfg._Ctype == (int)ModelCType.Airport)//飞机场
        {
            m_textEntrepot.text = UI_Helper.GetTextByLanguageID(239);
            m_textEntrepotNum.text = 0 + "/" + level._Func.Split(' ')[1];
            m_btnState01.image.sprite = UI_Helper.GetSprite("tbo_02");
        }
        else if (tileInfo.baseData.cfg._Ctype == (int)ModelCType.WaterPool)//蓄水池
        {
            m_textEntrepot.text = UI_Helper.GetTextByLanguageID(238);
            m_textEntrepotNum.text = player.m_kPlayerBasicAsset.m_kWater + "/" + ModelManager._instance.GetModleByType(ModelCType.WaterPool)[0].cfg._Capacity;
            m_btnState01.image.sprite = UI_Helper.GetSprite("tbo_05");
        }
   
        m_btnFood.gameObject.SetActive(false);
        m_btnWater.gameObject.SetActive(false);
        m_btnSeek.gameObject.SetActive(false);
        m_btnDanger.gameObject.SetActive(false);

    }

    /// <summary>
    /// 一键喂养
    /// </summary>
    private void OnButtonClick_Onekey()
    {
        BaseData data = tileInfo.baseData;
        Building building = data.GetComponent<Building>();

        Park park = ModelManager._instance.GetParkList(new List<Guid>(){ Guid.Parse(building.GetServer.guid)});
        var dicAnimla = park.listItemPark[(int)ModelCType.Animal];
        var dicPlant = park.listItemPark[(int)ModelCType.Plant];
        List<BaseData> lbaseData = new List<BaseData>();
        foreach(var item in dicAnimla)
        {
            lbaseData.Add(item.Value);
        }
        foreach (var item in dicPlant)
        {
            lbaseData.Add(item.Value);
        }
        if(lbaseData.Count == 0)
        {
            UI_Helper.ShowCommonTips(UI_Helper.GetTextByLanguageID(227));
            return;
        }

        SceneLogic._instance.QuickFeed(lbaseData);


    }
    /// <summary>
    /// 控制面板
    /// </summary>
    private void OnButtonClick_Controlpanel()
    {
        
    }
    /// <summary>
    /// 园区命名
    /// </summary>
    private void OnButtonClick_GardenReName()
    {
        BaseData data = tileInfo.baseData;
        Building building = data.GetComponent<Building>();
        Park park = ModelManager._instance.GetParkList(new List<Guid>() { Guid.Parse(building.GetServer.guid) });
        UIEntity uien = World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPopUpWindow_BuildingName);      
        uien.GetComponent<UIPopUpWindow_BuildingNameComponent>().Init(park);
    }
    /// <summary>
    /// 创库
    /// </summary>
    private void OnButtonClick_Storage()
    {
        UIEntity entity = World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPage_Pack);
        entity.GetComponent<UIPage_PackComponent>().InitUI(false, UIPage_PackComponent.Category.Warehouse);
        // 给新手引导添加回调事件
        //if (GuidanceManager.isGuidancing)
        //{
        //    GuidanceData data = new GuidanceData();
        //    data.entity = entity;
        //    data.type = World.Scene.GetComponent<UIManagerComponent>().GetType(UI_PrefabPath.m_sUIPage_Pack);
        //    ObserverHelper<GuidanceData>.SendMessage(MessageMonitorType.GuidanceClickEvent, this, new MessageArgs<GuidanceData>(data));
        //}
        GuidanceManager._Instance.CheckGuidance(entity);
    }

    //放置
    private void OnButtonClick_Placement()
    {
        BaseData data = tileInfo.baseData;
        Building building = data.GetComponent<Building>();
        Park park = ModelManager._instance.GetParkList(new List<Guid>() { Guid.Parse(building.GetServer.guid) });
        if (park.GetServer.parkType == ParkType.Move) { UI_Helper.ShowCommonTips(UI_Helper.GetTextByLanguageID(210)); return; }
        if (park.GetServer.parkType == ParkType.Sustain) { UI_Helper.ShowCommonTips(UI_Helper.GetTextByLanguageID(208)); return; }
        if (park.GetServer.parkType == ParkType.Uplv) { UI_Helper.ShowCommonTips(UI_Helper.GetTextByLanguageID(207)); return; }
     
        UIEntity entity = World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPage_Pack);
        entity.GetComponent<UIPage_PackComponent>().InitUI(true,UIPage_PackComponent.Category.Warehouse);
         
        //UIEntity entity = World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPage_Place);
        //CheckGuidance(entity);
        GuidanceManager._Instance.CheckGuidance(entity);
    }

    /// <summary>
    /// 安置点
    /// </summary>
    private void OnButtonClick_Temporary()
    {
        UIEntity entity = World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPage_Pack);
        entity.GetComponent<UIPage_PackComponent>().InitUI(false, UIPage_PackComponent.Category.Settlements);
        //CheckGuidance(entity);
        GuidanceManager._Instance.CheckGuidance(entity);
    }

    /// <summary>
    /// 商店
    /// </summary>
    private void OnButtonClick_Shop()
    {
        
        World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPage_Shop);
    }
    //编辑
    private void OnButtonClick_Edit()
    {
        BaseData data = tileInfo.baseData;
        if (data == null)
        {
            Debug.Log("需要编辑的baseData为空");
            return;
        }
        Building building = data.GetComponent<Building>();
        if (building == null)
        {
            Debug.Log("需要编辑的对象没有找到Building");
            return;
        }
        World.Scene.GetComponent<UIManagerComponent>().Remove(UI_PrefabPath.m_sUIPage_BuildingOperation);
        MapGridMgr.Instance.ReEdit(tileInfo);
        World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPage_BuildOperation);
    }
    /// <summary>
    /// 升级
    /// </summary>
    private void OnButtonClick_Upgrade()
    {
        BaseData data = tileInfo.baseData;
        Building building = data.GetComponent<Building>();
        if (building.CS_UpLevel._NextID == 0) { UI_Helper.ShowCommonTips(UI_Helper.GetTextByLanguageID(173)); return; }
        Park park = ModelManager._instance.GetParkList(new List<Guid>() { Guid.Parse(building.GetServer.guid) });
        if(park != null)//是园区
        {
            if (park.GetServer.parkType == ParkType.Move) { UI_Helper.ShowCommonTips(UI_Helper.GetTextByLanguageID(210)); return; }
            if (park.GetServer.parkType == ParkType.Sustain) { UI_Helper.ShowCommonTips(UI_Helper.GetTextByLanguageID(208)); return; }
            if (park.GetServer.parkType == ParkType.Uplv) { UI_Helper.ShowCommonTips(UI_Helper.GetTextByLanguageID(207)); return; }
            if (park.GetAllItemID().Count != 0 && park.GetServer.parkType != ParkType.Move)
            {
                UIEntity _uien = UI_Helper.ShowConfirmPanel("TextChange",
                    () => {
                        park.BeginMove();
                        park.ParkMove();
                        World.Scene.GetComponent<UIManagerComponent>().Remove(UI_PrefabPath.m_sUIPage_BuildingOperation);
                        MapGridMgr.Instance.UnFoucs();
                    },
                    () => { });
                _uien.GetComponent<UI_Confirm>().TextChange(UI_Helper.GetTextByLanguageID(200), UI_Helper.GetTextByLanguageID(201), UI_Helper.GetTextByLanguageID(202));
            }
            else
            {
                UIEntity uien = World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPopUpWindow_BuildingLvUpBox);
                uien.GetComponent<UIPopUpWindow_BuildingLvUpComponent>().Init(data);
            }
        }
        else
        {
            UIEntity uien = World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPopUpWindow_BuildingLvUpBox);
            uien.GetComponent<UIPopUpWindow_BuildingLvUpComponent>().Init(data);
        }
    }
    /// <summary>
    /// 扩建
    /// </summary>
    private void OnButtonClick_Expantion()
    {
      
        Park park = ModelManager._instance.GetParkList(new List<Guid>() { SceneLogic._instance.selectTileInfo.guid });

        if (park.GetServer.parkType == ParkType.Move) { UI_Helper.ShowCommonTips(UI_Helper.GetTextByLanguageID(210)); return; }
        if (park.GetServer.parkType == ParkType.Sustain) { UI_Helper.ShowCommonTips(UI_Helper.GetTextByLanguageID(208)); return; }
        if (park.GetServer.parkType == ParkType.Uplv) { UI_Helper.ShowCommonTips(UI_Helper.GetTextByLanguageID(207)); return; }

        if (park.GetAllItemID().Count != 0 && park.GetServer.parkType != ParkType.Move)
        { 
            UIEntity _uien = UI_Helper.ShowConfirmPanel("TextChange",
                () => {
                    park.BeginMove();
                    park.ParkMove();
                    World.Scene.GetComponent<UIManagerComponent>().Remove(UI_PrefabPath.m_sUIPage_BuildingOperation);
                    MapGridMgr.Instance.UnFoucs();
                },
                () => { });
            _uien.GetComponent<UI_Confirm>().TextChange(UI_Helper.GetTextByLanguageID(200), UI_Helper.GetTextByLanguageID(201), UI_Helper.GetTextByLanguageID(202));

        }
        else
        {
            OK_CallBack();
        }
    }

    private void OK_CallBack()
    {
        //Park park = ModelManager._instance.GetParkList(SceneLogic._instance.selectTileInfo.guid);
        //List<Guid> _objectList = park.GetAllItmeGuid();

        //Player player = World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer;
        //for (int i=0;i< _objectList.Count;i++)
        //{
        //    player.AddToSettlements(_objectList[i]);
        //}
  
        MapGridMgr.Instance.ReEdit(tileInfo);
        World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPage_GardenEdit);
    }

    public void SelectItem(TileInfo _tileInfo)
    {
        //点击类型：1小木屋 2园区 3蓄水池 4飞机场
        //按钮类型：1扩建2升级3商店4安置5安置点6仓库7园区命名8控制面板9一键喂养
        tileInfo = _tileInfo;
        SelectModel(_tileInfo.baseData);
        LeftPanel();

    }

    public void SelectModel(BaseData baseData)
    {
        if (baseData == null) return;

        CS_Model.DataEntry dataEntry = baseData.cfg;
        var cfgId = dataEntry._ID;

        if(ModelBase.IsObjType(cfgId, ModelCType.Building))
        {
            m_kTextLevel.text = tileInfo.baseData.GetComponent<Building>().GetServer.lv.ToString();
        }

        if (dataEntry != null && dataEntry._PointFun != "xx")
        {
            m_kTextTitle.text = UI_Helper.GetTextByLanguageID(dataEntry._DisplayName);

            string[] _PointFun = dataEntry._PointFun.Split('=');
            List<string> ButtonList = _PointFun[1].Split('&').ToList();
            m_kRectTransformCabin.gameObject.SetActive(_PointFun[0] == "1");
            m_kRectTransformGarden.gameObject.SetActive(_PointFun[0] == "2");
            m_kRectTransformReservoir.gameObject.SetActive(_PointFun[0] == "3");
            m_kRectTransformAirport.gameObject.SetActive(_PointFun[0] == "4");

            m_kButtonExpantion.gameObject.SetActive(ButtonList.Contains("1"));
            m_kButtonUpgrade.gameObject.SetActive(ButtonList.Contains("2"));
            m_kButtonShop.gameObject.SetActive(ButtonList.Contains("3"));
            m_kButtonPlacement.gameObject.SetActive(ButtonList.Contains("4"));
            m_kButtonTemporary.gameObject.SetActive(ButtonList.Contains("5"));
            m_kButtonStorage.gameObject.SetActive(ButtonList.Contains("6"));
            m_kButtonGardenReName.gameObject.SetActive(ButtonList.Contains("7"));
            m_kButtonControlpanel.gameObject.SetActive(ButtonList.Contains("8"));
            m_kButtonOnekey.gameObject.SetActive(ButtonList.Contains("9"));
            m_kButtonDetails.gameObject.SetActive(ButtonList.Contains("10"));
            m_kButtonEdit.gameObject.SetActive(ButtonList.Contains("11"));
        }
    }
}

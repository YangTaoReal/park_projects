using System;
using System.Collections;
using System.Collections.Generic;
using QTFramework;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[ObjectEventSystem]
public class UI_SpeedUPAwakeSystem : AAwake<UI_SpeedUP>
{
    public override void Awake(UI_SpeedUP _self)
    {
        _self.Awake();
    }
}


[UIEntityComponent(UI_PrefabPath.m_sUIPopUpWindow_SpeedUp)]
public class UI_SpeedUP : UIComponent 
{
    UIBar UIBar;
    public Text ui_titleText;
    public Text ui_abstractText;
    public Text ui_confirmBtnText;
    public Text ui_currNumText;
    public Text m_textTime;
    public Slider ui_remainTimeSlider;
    public Slider ui_chooseNumSlider;

    public Button ui_closeBtn;
    public Button ui_AddItemBtn;
    public Button ui_reduceNumBtn;
    public Button ui_addNumBtn;
    public Button ui_confirmBtn;

    public ScrollRect ui_propScrollRect;

    public List<UI_SpeedUpItem> itemList = new List<UI_SpeedUpItem>();


    float nowTime;
    float maxTime;
    BaseData building;
    //Park park;

    public UI_SpeedUpItem currItem; // 当前选中的item
    private int currNum;
    public int CurrNum
    {
        get{
            return currNum;
        }
        set
        {
            currNum = value;
            ui_currNumText.text = currNum.ToString();
            ui_chooseNumSlider.value = currNum;
        }
    }
	public void Awake()
    {
        UIEntity uI_Entity = ParentEntity as UIEntity;
        ui_titleText = uI_Entity.m_kUIPrefab.GetCacheComponent(0) as Text;
        ui_abstractText = uI_Entity.m_kUIPrefab.GetCacheComponent(1) as Text;
        ui_confirmBtnText = uI_Entity.m_kUIPrefab.GetCacheComponent(2) as Text;
        ui_remainTimeSlider = uI_Entity.m_kUIPrefab.GetCacheComponent(3) as Slider;
        ui_chooseNumSlider = uI_Entity.m_kUIPrefab.GetCacheComponent(4) as Slider;
        ui_closeBtn = uI_Entity.m_kUIPrefab.GetCacheComponent(5) as Button;
        ui_AddItemBtn = uI_Entity.m_kUIPrefab.GetCacheComponent(6) as Button;
        ui_reduceNumBtn = uI_Entity.m_kUIPrefab.GetCacheComponent(7) as Button;
        ui_addNumBtn = uI_Entity.m_kUIPrefab.GetCacheComponent(8) as Button;
        ui_confirmBtn = uI_Entity.m_kUIPrefab.GetCacheComponent(9) as Button;
        ui_currNumText = uI_Entity.m_kUIPrefab.GetCacheComponent(10) as Text;
        ui_propScrollRect = uI_Entity.m_kUIPrefab.GetCacheComponent(11) as ScrollRect;
        m_textTime = uI_Entity.m_kUIPrefab.GetCacheComponent(12) as Text;


 
        Init();
    }

    public override void Dispose()
    {
        base.Dispose();
        ui_closeBtn.onClick.RemoveAllListeners();
        ui_AddItemBtn.onClick.RemoveAllListeners();
        ui_reduceNumBtn.onClick.RemoveAllListeners();
        ui_addNumBtn.onClick.RemoveAllListeners();
        ui_confirmBtn.onClick.RemoveAllListeners();
        ui_chooseNumSlider.onValueChanged.RemoveAllListeners();
        UIBar.UI_SpeedUP = null;
    }

    public void InitGuid(UIBar uIBar, BaseData _building = null)
    {
        building = _building;
        //park = _park;
        if (building != null) Debug.Log("___加载的是building___");
        //if (park != null) Debug.Log("___park___");

        UIBar = uIBar;
 
    }

    private void Init()
    {
 
        InitEvent();
        InitProps();

    }

    public override void TranslateUI()
    {
        base.TranslateUI();
        ui_titleText.text = UI_Helper.GetTextByLanguageID(204);
        ui_abstractText.text = UI_Helper.GetTextByLanguageID(205);
        ui_confirmBtnText.text = UI_Helper.GetTextByLanguageID(141);

    }



    private void InitProps()
    {
        CurrNum = 0;
        GetEntity<UIEntity>().ClearChildren();
        itemList.Clear();
        PlayerBagAsset playerBagAsset = World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.m_kPlayerBagAsset;
        Player player = World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer;
     
        //SelectItem

        int[] itemIDs = { 900006, 900007, 900008 };
        for (int i = 0; i < itemIDs.Length; i++)
        {
            PlayerBagAsset.BagItem item = player.SelectItem(itemIDs[i]);
            if(item != null && item.m_kCount > 0)
            {
                CS_Items.DataEntry dataEntry = DBManager.Instance.m_kItems.GetEntryPtr(item.m_kItemID);
                UIEntity entity = World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPage_Pack_SpeedUpItem);
                UI_SpeedUpItem iTmp = entity.GetComponent<UI_SpeedUpItem>();
                iTmp.Init(dataEntry, item.m_kCount);
                entity.m_kUIPrefab.transform.SetParent(ui_propScrollRect.content);
                itemList.Add(iTmp);
                m_kParentEntity.AddChildren(entity);
            }
        }
 
        if (itemList.Count == 0)
        {
            CurrNum = 0;
            ui_chooseNumSlider.value = 0;
        }
        else
            World.Scene.GetComponent<TimeComponent>().CreateTimer(300, 0, 1, () =>
            {
                OnSelectedItem(itemList[0]);
            });
           

    }

    public void UpdateProgress(float now, float max)
    {
        float rate = Mathf.Floor(now / max * 1000);
        rate = float.Parse(rate.ToString().Split('.')[0]);
        ui_remainTimeSlider.value = now / max;
        float tVal = max / 3600;
        string ret = tVal.ToString("0.000");
        m_textTime.text = ret + "H";
        nowTime = now;
        maxTime = max;
        if (now / max >= 1)
            CloseSpeedUpPanel();
    }


    private void InitEvent()
    {
      
        ui_closeBtn.onClick.AddListener(() =>
        {
            CloseSpeedUpPanel();
        });
  
        ui_AddItemBtn.onClick.AddListener(() =>
        {
            //Debug.Log("添加item");
            CloseSpeedUpPanel();
            UIEntity uIEntity = World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPage_Shop);
            uIEntity.GetComponent<UIPage_ShopComponent>().m_kToggleStageProperty.isOn = true;
        });
 
        ui_addNumBtn.onClick.AddListener(() =>
        {
            //Debug.Log("增加一个数量");
            AddNum();
        });
 
        ui_reduceNumBtn.onClick.AddListener(() =>
        {
            //Debug.Log("减少一个数量");
            ReduceNum();
        });
 
        ui_confirmBtn.onClick.AddListener(() =>
        {
            //Debug.Log($"确认加速，选中的加速道具：{currItem.info._ID},选中的数量：{CurrNum}");
            if (currItem == null) 
            {
                Debug.Log("请选择至少一个加速道具");
                return;
            }
            string[] split = currItem.info._Use.Split(' ');

            currNum = int.Parse(ui_currNumText.text);

            int second = int.Parse(split[2]) * currNum;
            bool isSucceed;
            if(GuidanceManager.isGuidancing)
                isSucceed = SceneLogic._instance.SpeedUpComplete(building.guid, 99999999);
            else
                isSucceed = SceneLogic._instance.SpeedUpComplete(building.guid, second);
            if(isSucceed)
            {
                
            }



            World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.CosetItem(currItem.info._ID, (PlayerBagAsset.ItemType)currItem.info._ItemType, currNum);
            //InitProps();
            //CheckGuidance();
            GuidanceManager._Instance.CheckGuidance(null);
            CloseSpeedUpPanel();
        });
        Player player = World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer;
        ui_chooseNumSlider.onValueChanged.AddListener((num) =>
        {
            if (itemList.Count == 0) return;
            int own = player.SelectItem(currItem.info._ID).m_kCount;
            if ((int)num >= own)
                CurrNum = own;
            else
                CurrNum = (int)num;

        });
    }

    //public void CheckGuidance()
    //{
    //    if(GuidanceManager.isGuidancing)
    //    {
    //        GuidanceData data = new GuidanceData();
    //        ObserverHelper<GuidanceData>.SendMessage(MessageMonitorType.GuidanceClickEvent, this, new MessageArgs<GuidanceData>(data));
    //    }
    //}
    // 选中item
    public void OnSelectedItem(UI_SpeedUpItem item)
    {
  
        for (int i = 0; i < itemList.Count; i++)
        {
            if (itemList[i] == item)
            {
                itemList[i].SetSelecedImage(true);
                currItem = itemList[i];
            }
            else
                itemList[i].SetSelecedImage(false);
        }

        string[] split= currItem.info._Use.Split(' ');
        int needMax = Mathf.CeilToInt((maxTime - nowTime) / int.Parse(split[2]));

        ui_chooseNumSlider.maxValue = needMax;
        CurrNum = 1;
        ui_chooseNumSlider.value = 1;
 
 
    }

    // 添加数量
    public void AddNum()
    {
        if (itemList.Count == 0) return;
        if(nowTime == 0 && maxTime == 0)
        {
            OnSelectedItem(itemList[0]);
        }
      

        if(CurrNum >= ui_chooseNumSlider.maxValue)
        {
            CurrNum = (int)ui_chooseNumSlider.maxValue;
            return;
        }
        CurrNum++;
    }

    // 减少数量
    public void ReduceNum()
    {
        if (itemList.Count == 0) return;
        if (nowTime == 0 && maxTime == 0)
        {
            OnSelectedItem(itemList[0]);
        }

        if (CurrNum <= 1)
        {
            CurrNum = 1;
            return;
        }
            
        CurrNum--;
    }

    // 关闭面板
    public void CloseSpeedUpPanel()
    {
        World.Scene.GetComponent<UIManagerComponent>().Remove(UI_PrefabPath.m_sUIPopUpWindow_SpeedUp);
    }
}

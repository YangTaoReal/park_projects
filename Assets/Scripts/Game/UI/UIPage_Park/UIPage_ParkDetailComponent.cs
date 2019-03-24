using QTFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

[ObjectEventSystem]
public class UIPage_ParkDetailComponentAwakeSystem : AAwake<UIPage_ParkDetailComponent>
{
    public override void Awake(UIPage_ParkDetailComponent _self)
    {
        _self.Awake();
    }
}

[ObjectEventSystem]
public class UIPage_ParkDetailComponentFixedUpdateSystem : AFixedUpdate<UIPage_ParkDetailComponent>
{
    public override void FixedUpdate(UIPage_ParkDetailComponent _self)
    {
        _self.FixedUpdate();
    }
}


[UIEntityComponent(UI_PrefabPath.m_sUIPage_ParkDetial)]
public class UIPage_ParkDetailComponent : UIComponent
{
    public Text m_kTextTitle;

    public Button m_kButtonFastfeed;

    public Text m_kTextStageTile;
    public Text m_kTextHungerTitle;
    public Text m_kTextThirstTile;
    public Text m_kTextOutputTitle;
    public Text m_kTextStatusTitle;


    public Toggle m_kToggleAnimalTap;
    public Toggle m_kTogglePlantTap;

    public RectTransform ScorllViewContent;

    public RectTransform ScorllViewOperateItem;
    public RectTransform ScorllViewItem0;

    public RawImage m_kStateCacheImgOk;
    public RawImage m_kStateCacheImgSeek;
    public RawImage m_kStateCacheImgHunger;
    public RawImage m_kStateCacheImgThirst;

    public Button m_kButtonOperateLocation;
    public Button m_kButtonOperateShop;

    public RectTransform m_ObjectNextOp;

    public EventTrigger eventTrigger;

    public RectTransform contentNode;


    private class BagItem
    {
        public BagItem(PlayerBagAsset.BagItem bagItem)
        {
            info = bagItem;
            cfg = DBManager.Instance.m_kItems.GetEntryPtr(bagItem.m_kItemID);
        }

        public PlayerBagAsset.BagItem info;
        public CS_Items.DataEntry cfg;
    }

    internal void Awake()
    {
        m_kTextTitle = m_kParentEntity.m_kUIPrefab.GetCacheComponent(0) as Text;

        m_kButtonFastfeed = m_kParentEntity.m_kUIPrefab.GetCacheComponent(1) as Button;

        m_kTextStageTile = m_kParentEntity.m_kUIPrefab.GetCacheComponent(2) as Text;
        m_kTextHungerTitle = m_kParentEntity.m_kUIPrefab.GetCacheComponent(3) as Text;
        m_kTextThirstTile = m_kParentEntity.m_kUIPrefab.GetCacheComponent(4) as Text;
        m_kTextOutputTitle = m_kParentEntity.m_kUIPrefab.GetCacheComponent(5) as Text;
        m_kTextStatusTitle = m_kParentEntity.m_kUIPrefab.GetCacheComponent(6) as Text;

        m_kToggleAnimalTap = m_kParentEntity.m_kUIPrefab.GetCacheComponent(7) as Toggle;
        m_kTogglePlantTap = m_kParentEntity.m_kUIPrefab.GetCacheComponent(8) as Toggle;

        ScorllViewContent = m_kParentEntity.m_kUIPrefab.GetCacheComponent(9) as RectTransform;
        ScorllViewOperateItem = m_kParentEntity.m_kUIPrefab.GetCacheComponent(10) as RectTransform;

        ScorllViewItem0 = m_kParentEntity.m_kUIPrefab.GetCacheComponent(11) as RectTransform;
        m_kStateCacheImgOk = m_kParentEntity.m_kUIPrefab.GetCacheComponent(12) as RawImage;
        m_kStateCacheImgSeek = m_kParentEntity.m_kUIPrefab.GetCacheComponent(13) as RawImage;
        m_kStateCacheImgHunger = m_kParentEntity.m_kUIPrefab.GetCacheComponent(14) as RawImage;
        m_kStateCacheImgThirst = m_kParentEntity.m_kUIPrefab.GetCacheComponent(15) as RawImage;
        m_kButtonOperateLocation = m_kParentEntity.m_kUIPrefab.GetCacheComponent(16) as Button;
        m_kButtonOperateShop = m_kParentEntity.m_kUIPrefab.GetCacheComponent(17) as Button;

        m_ObjectNextOp = m_kParentEntity.m_kUIPrefab.GetCacheComponent(18) as RectTransform;
        eventTrigger = m_kParentEntity.m_kUIPrefab.GetCacheComponent(19) as EventTrigger;

        contentNode = m_kParentEntity.m_kUIPrefab.GetCacheComponent(20) as RectTransform;

        var oldPos = contentNode.transform.localPosition;
        contentNode.transform.localPosition = new Vector3(oldPos.x, oldPos.y+contentNode.sizeDelta.y/5, oldPos.z);
        contentNode.transform.DOLocalMoveY(oldPos.y, 0.5f).SetEase(Ease.OutBack);


        Park park = ModelManager._instance.GetParkList(new List<Guid>() { SceneLogic._instance.selectTileInfo.guid });
        SelectModel(park);



        m_kButtonFastfeed.onClick.AddListener(OnButtonClick_FastFeed);
        m_kToggleAnimalTap.onValueChanged.AddListener(onValueChanged_SelectTapAnimal);
        m_kTogglePlantTap.onValueChanged.AddListener(onValueChanged_SelectTapPlant);
        m_kButtonOperateLocation.onClick.AddListener(OnButtonClick_Location);
        m_kButtonOperateShop.onClick.AddListener(OnButtonClick_Shop);

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerDown;
        entry.callback.AddListener(OnEmptyTouch);

        eventTrigger.triggers.Clear();
        eventTrigger.triggers.Add(entry);

        ScorllViewOperateItem.gameObject.SetActive(false);

        World.Scene.GetComponent<UIManagerComponent>().Remove(UI_PrefabPath.m_sUIPage_BuildingOperation);
    }

    private void onValueChanged_SelectTapPlant(bool arg0)
    {
        if (arg0)
        {
          
            list_data = list_plant;
            operateIdx = -1;

            DoUpdate();
        }
    }

    private void onValueChanged_SelectTapAnimal(bool arg0)
    {
        if (arg0)
        {
            list_data = list_animal;
            operateIdx = -1;

            DoUpdate();
        }
    }
    public override void TranslateUI()
    {
        base.TranslateUI();
        m_kTextTitle.text = "PARK DETIAL";

        UpdateUI();
    }

    float nextUpdateTime = 0;
    internal void FixedUpdate()
    {
        if(nextUpdateTime > 0)
        {
            nextUpdateTime -= Time.fixedDeltaTime;
            return;
        }

        nextUpdateTime = 1;

        UpdateUI();
    }


    void DoUpdate()
    {
        nextUpdateTime = 0;
    }

    public override void Dispose()
    {
        base.Dispose();
        m_kButtonFastfeed.onClick.RemoveListener(OnButtonClick_FastFeed);
        m_kToggleAnimalTap.onValueChanged.RemoveListener(onValueChanged_SelectTapAnimal);
        m_kTogglePlantTap.onValueChanged.RemoveListener(onValueChanged_SelectTapPlant);
        m_kButtonOperateLocation.onClick.RemoveListener(OnButtonClick_Location);
        m_kButtonOperateShop.onClick.RemoveListener(OnButtonClick_Shop);
    }


    bool isFastFeed = false;
    void OnButtonClick_FastFeed()
    {
        var tempData = data;
        isFastFeed = true;

        foreach (var keyval in list_data)
        {
            data = keyval.Value;
        }


        isFastFeed = false;
        data = tempData;
    }



    private void OnButtonClick_Shop()
    {
        World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPage_Shop);
    }

    void OnButtonClick_Location()
    {
        
    }

    private void OnEmptyTouch(BaseEventData data)
    {
        World.Scene.GetComponent<UIManagerComponent>().Remove(UI_PrefabPath.m_sUIPage_Shop);
    }

    Park parkData = null;
    Dictionary<Guid, BaseData> list_plant = null;
    Dictionary<Guid, BaseData> list_animal = null;
    Dictionary<Guid, BaseData> list_data = null;

    BaseData data = null;
    int nextOpType;
    void SelectModel(Park park)
    {
        parkData = park;

        list_plant = parkData.listItemPark[(int)ModelCType.Plant];
        list_animal = parkData.listItemPark[(int)ModelCType.Animal];

        if (list_plant.Count > list_animal.Count)
        {
            m_kToggleAnimalTap.isOn = true;
            onValueChanged_SelectTapAnimal(true);
        }
        else
        {
            m_kTogglePlantTap.isOn = true;
            onValueChanged_SelectTapPlant(true);
        }

        ScorllViewContent.offsetMax = new Vector2(0, 0);
    }

    int operateIdx = -1;
    void UpdateList()
    {
        ScorllViewOperateItem.transform.SetParent(null);

        Transform node = ScorllViewContent.transform;
        int max = list_data.Count;
        int idx = 0;
        foreach (var keyval in list_data)
        {
            var item = keyval.Value;
            Transform child = null;
            if (node.childCount <= idx)
            {
                child = GameObject.Instantiate<Transform>(ScorllViewItem0.transform);
                child.SetParent(node);
                child.localScale = Vector3.one;
                child.localRotation = Quaternion.identity;
            }
            else
            {
                child = node.GetChild(idx);
            }

            child.gameObject.SetActive(true);

            child.GetChild(0).GetComponent<RawImage>().texture = UI_Helper.AllocTexture(item.cfg._Icon);
            child.GetChild(1).GetComponent<Text>().text = UI_Helper.GetTextByLanguageID(item.cfg._DisplayName);

            ModelBase modelBase = item.GetComponent<ModelBase>();
            if (modelBase == null) continue;

            BaseServer Server = modelBase.GetbaseServer;
            if (Server == null) continue;

            List<List<int>> m_lStatePro = modelBase.GetStatePro;
            for (int i = 0; i < m_lStatePro.Count; i++)
            {
                List<int> lstate = m_lStatePro[i];
                var curval = Server.proVal[i];
                //if (curval < 0) curval = 0;
                var maxval = lstate[2];
                string _str = modelBase.GetStateName((StatePro)lstate[0]);
                if (i == WATER_IDX)
                {
                    var water = child.GetChild(4);
                    water.GetChild(0).GetComponent<Image>().fillAmount = Convert.ToSingle(curval / maxval);
                    water.GetChild(1).GetComponent<Text>().text = curval + "/" + maxval;
                    water.GetChild(2).GetComponent<Text>().text = curval + "/" + maxval;
                }
                else if (i == FOOD_IDX)
                {
                    var food = child.GetChild(3);
                    food.GetChild(0).GetComponent<Image>().fillAmount = Convert.ToSingle(curval / maxval);
                    food.GetChild(1).GetComponent<Text>().text = curval + "/" + maxval;
                    food.GetChild(2).GetComponent<Text>().text = curval + "/" + maxval;
                }
            }

            var stage = child.GetChild(2);
            stage.GetChild(1).GetComponent<Text>().text = modelBase.GetGrowthName();
            if(modelBase.GetbaseServer.growthState == GrowthState.Young)
            {
                var dataInOut = DBManager.Instance.m_kInOutPut.GetEntryPtr(item.cfg._InOutPutID);
                TimeSpan timeSpan = DateTime.Now.Subtract(DateTime.Parse(modelBase.GetbaseServer.BeginTime));
                float rate = (float)timeSpan.TotalSeconds / (dataInOut._GrowTime * 60);
                stage.GetChild(0).GetComponent<Image>().fillAmount = rate;

                var left = (dataInOut._GrowTime * 60) - (int)timeSpan.TotalSeconds;
                stage.GetChild(2).GetComponent<Text>().text = left.ToString();
            }
            else
            {
                stage.GetChild(0).GetComponent<Image>().fillAmount = 1f;
                stage.GetChild(2).GetComponent<Text>().text = "";
            }




            var output = child.GetChild(5);
            CS_InOutPut.DataEntry CS_InOutPut = DBManager.Instance.m_kInOutPut.GetEntryPtr(item.cfg._InOutPutID);
            float val = (float)CS_InOutPut._MatureAwardTime / 60;
            output.GetChild(1).GetComponent<Text>().text = val.ToString("f2") + "H";

            var status = child.GetChild(6).GetChild(1);

            for (int i = 0; i < status.childCount; i++)
            {
                status.GetChild(i).gameObject.SetActive(false);
            }

            var bufflist = modelBase.GetMyselfBuff;
            bool ok = true;
            foreach (var buff in bufflist)
            {
                if (buff.CS_Buff._Type == (int)BuffType.Food)
                {
                    status.GetChild(2).gameObject.SetActive(true);
                    ok = false;
                }
                else if (buff.CS_Buff._Type == (int)BuffType.Ill)
                {
                    status.GetChild(1).gameObject.SetActive(true);
                    ok = false;
                }
                else if (buff.CS_Buff._Type == (int)BuffType.Water)
                {
                    status.GetChild(3).gameObject.SetActive(true);
                    ok = false;
                }
                else if (buff.CS_Buff._Type == (int)BuffType.Danger)
                {
                    status.GetChild(4).gameObject.SetActive(true);
                    ok = false;
                }
            }

            status.GetChild(0).gameObject.SetActive(ok);

            var clickIdx = idx;
            child.GetComponent<Button>().onClick.AddListener(() =>
            {
                operateIdx = clickIdx;
                data = item;

                DoUpdate();
            });

            idx++;
        }

        for (int i = idx; i < node.childCount; i++)
        {
            node.GetChild(i).gameObject.SetActive(false);
        }


        ScorllViewOperateItem.transform.SetParent(node);
        if(operateIdx >= 0 && operateIdx<max)
        {
            ScorllViewOperateItem.gameObject.SetActive(true);
            ScorllViewOperateItem.transform.SetSiblingIndex(operateIdx+1);

            max++;
        }
        else{
            ScorllViewOperateItem.gameObject.SetActive(false);
        }
    }














    static int WATER_IDX = 0;
    static int FOOD_IDX = 1;
    void UpdateUI()
    {
        UpdateList();

        UpdateNextOp();
    }
     
    void UpdateNextOp()
    {
        if (operateIdx < 0) return;

        var list = GetPackage();

        Transform node = m_ObjectNextOp.transform;
        int idx = 1;
        foreach (var item in list)
        {
            Transform child = null;
            if(node.childCount -1 <= idx)
            {
                child = GameObject.Instantiate<Transform>(node.GetChild(1));
                child.SetParent(node);
                child.localScale = Vector3.one;
                child.localRotation = Quaternion.identity;
                child.SetSiblingIndex(idx);
            }
            else
            {
                child = node.GetChild(idx);
            }

            child.gameObject.SetActive(true);

            child.GetChild(0).GetComponent<RawImage>().texture = UI_Helper.AllocTexture(item.cfg._Icon);
            child.GetChild(1).GetComponent<Text>().text = "x" + item.info.m_kCount;

            var idxTemp = idx;
            var btn = child.GetComponent<Button>();
            btn.onClick.RemoveAllListeners();
            if(BagItemIsFood(item))
            {
                btn.onClick.AddListener(() =>
                {
                    Feed(idxTemp);
                });
            }
            else if(BagItemIsWater(item))
            {

            }
            else if(BagItemIsMedic(item))
            {
                btn.onClick.AddListener(() =>
                {
                    Medic(idxTemp);
                });
            }

            idx++;

        }

        //var rectTransf = m_ObjectNextOp.GetComponent<RectTransform>();
        //rectTransf.sizeDelta = new Vector2( 200 + (list.Count) * 102, rectTransf.sizeDelta.y);

        for (int i = idx; i < node.childCount-1; i++){
            node.GetChild(i).gameObject.SetActive(false);
        }
    }

    int GetCurValNeedNum(int idx, int unitVal)
    {
        ModelBase modelBase = data.GetComponent<ModelBase>();
        if (modelBase == null) return 0;

        BaseServer Server = modelBase.GetbaseServer;
        if (Server == null) return 0;

        List<List<int>> m_lStatePro = modelBase.GetStatePro;
        List<int> lstate = m_lStatePro[idx];
        var curval = (int)Server.proVal[idx];
        var maxval = lstate[2];

        var need = maxval - curval;
        if (need <= 0) return 0;

        int needNum = 0;
        if(need % unitVal == 0)
        {
            needNum = (int)(need / unitVal);
        }
        else
        {
            needNum =(int) Math.Floor((float)need/unitVal) + 1;
        }

        return needNum;
    }

    static int WATER_FIX_ID = 100003;
    void Drink()
    {
        var needNum = GetCurValNeedNum(WATER_IDX, 1);
        if(needNum <= 0)
        {
            UI_Helper.ShowConfirmPanel(UI_Helper.GetTextByLanguageID(256), null);
            return;
        }

        Player player = World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer;
        int mywater = (int)player.m_kPlayerBasicAsset.m_kWater;
        if(mywater <= 0){
            UI_Helper.ShowConfirmPanel(UI_Helper.GetTextByLanguageID(187), null);
            return;
        }
        if (needNum > mywater) needNum = mywater;

        string str = UI_Helper.GetTextByLanguageID(185, needNum.ToString(), (mywater - needNum).ToString());
        UI_Helper.ShowConfirmPanel(str, () =>
        {
            ModelBase modelBase = data.GetComponent<ModelBase>();
            modelBase.FillOnceStatePro(BuffType.Water, WATER_FIX_ID, needNum);

        }, () =>
        {
            
        });
    }

    void Feed(int idx){
        var list = GetPackage();
        if (idx >= list.Count) return;

        var item = list[idx];
        var arr = item.cfg._Use.Split(' ');
        var price = int.Parse(arr[2]);
        var needNum = GetCurValNeedNum(FOOD_IDX, price);
        if (needNum <= 0)
        {
            UI_Helper.ShowConfirmPanel(UI_Helper.GetTextByLanguageID(258), null);
            return;
        }
        if (needNum > item.info.m_kCount) needNum = item.info.m_kCount;

        string msg = UI_Helper.GetTextByLanguageID(184, (price * needNum).ToString(), needNum.ToString(), UI_Helper.GetTextByLanguageID(item.cfg._Name));
        UI_Helper.ShowConfirmPanel(msg, () =>
        {
            ModelBase modelBase = data.GetComponent<ModelBase>();
            modelBase.FillOnceStatePro(BuffType.Food, item.cfg._ID, needNum);
            UpdateNextOp();

        }, () =>
        {
            
        });
    }

    void Medic(int idx)
    {
        var bufflist = data.GetComponent<ModelBase>().GetMyselfBuff;
        Buff tagBuff = null;
        foreach (var buff in bufflist)
        {
            if (buff.CS_Buff._Type == (int)BuffType.Ill)
            {
                tagBuff = buff;
                break;
            }
        }
        if (tagBuff == null){
            UI_Helper.ShowConfirmPanel(UI_Helper.GetTextByLanguageID(259), null);
            return;
        }

        var list = GetPackage();
        if (idx >= list.Count) return;

        var item = list[idx];

        int needNum = 0;
        if(tagBuff.CS_Buff._disType.Contains('|'))
        {
            var arrBase = tagBuff.CS_Buff._disType.Split('|');
            foreach(var str in arrBase){
                var arr = str.Split(' ');
                var id = int.Parse(arr[1]);
                if(id == item.cfg._ID){
                    needNum = int.Parse(arr[2]);
                    break;
                }
            }
        }
        else{
            var arr = tagBuff.CS_Buff._disType.Split(' ');
            var id = int.Parse(arr[1]);
            if (id == item.cfg._ID)
            {
                needNum = int.Parse(arr[2]);
            }
        }

        if(needNum <= 0){
            return;
        }

        if (needNum > item.info.m_kCount) needNum = item.info.m_kCount;

        string msg = UI_Helper.GetTextByLanguageID(186, needNum.ToString(), UI_Helper.GetTextByLanguageID(item.cfg._Name));
        UI_Helper.ShowConfirmPanel(msg, () =>
        {
            ModelBase modelBase = data.GetComponent<ModelBase>();
            modelBase.FillOnceStatePro(BuffType.Ill, item.cfg._ID, needNum);

            UpdateNextOp();

        }, () =>
        {
            
        });
    }

    List<BagItem> GetPackage()
    {
        var bagItems = World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.m_kPlayerBagAsset.m_kBag;
        List<BagItem> SelectItem = new List<BagItem>();

        bool isPlant = true;
        if (data.cfg._Type == (int)ModeTyp.Animal)
        {
            isPlant = false;   
        }

        for (int i = 0; i < bagItems.Count; i++)
        {
            if (bagItems[i].m_kItemType == PlayerBagAsset.ItemType.Nutrients)
            {
                SelectItem.Add(new BagItem(bagItems[i]));
            }
            else if(bagItems[i].m_kItemType == PlayerBagAsset.ItemType.BotanyNutrients && isPlant)
            {
                SelectItem.Add(new BagItem(bagItems[i]));
            }
            else if (bagItems[i].m_kItemType == PlayerBagAsset.ItemType.AnimaiNutrients && !isPlant)
            {
                SelectItem.Add(new BagItem(bagItems[i]));
            }
            else if (bagItems[i].m_kItemType == PlayerBagAsset.ItemType.Medicine)
            {
                SelectItem.Add(new BagItem(bagItems[i]));
            }
            else if (bagItems[i].m_kItemType == PlayerBagAsset.ItemType.BotanyMedicine && isPlant)
            {
                SelectItem.Add(new BagItem(bagItems[i]));
            }
            else if (bagItems[i].m_kItemType == PlayerBagAsset.ItemType.AnimaiMedicine && !isPlant)
            {
                SelectItem.Add(new BagItem(bagItems[i]));
            }
            else if (bagItems[i].m_kItemType == PlayerBagAsset.ItemType.Water)
            {
                SelectItem.Add(new BagItem(bagItems[i]));
            }
        }

        return SelectItem;
    }

    bool BagItemIsWater(BagItem bagItem)
    {
        return bagItem.cfg._ItemType == (int)PlayerBagAsset.ItemType.Water;
    }
    bool BagItemIsFood(BagItem bagItem)
    {
        bool isPlant = true;
        if (data.cfg._Type == (int)ModeTyp.Animal)
        {
            isPlant = false;
        }

        if (bagItem.cfg._ItemType == (int)PlayerBagAsset.ItemType.Nutrients)
        {
            return true;
        }
        else if (bagItem.cfg._ItemType == (int)PlayerBagAsset.ItemType.BotanyNutrients && isPlant)
        {
            return true;
        }
        else if (bagItem.cfg._ItemType == (int)PlayerBagAsset.ItemType.AnimaiNutrients && !isPlant)
        {
            return true;
        }

        return false;
    }
    bool BagItemIsMedic(BagItem bagItem)
    {
        bool isPlant = true;
        if (data.cfg._Type == (int)ModeTyp.Animal)
        {
            isPlant = false;
        }

        if (bagItem.cfg._ItemType == (int)PlayerBagAsset.ItemType.Medicine)
        {
            return true;
        }
        else if (bagItem.cfg._ItemType == (int)PlayerBagAsset.ItemType.BotanyMedicine && isPlant)
        {
            return true;
        }
        else if (bagItem.cfg._ItemType == (int)PlayerBagAsset.ItemType.AnimaiMedicine && !isPlant)
        {
            return true;
        }

        return false;
    }
}
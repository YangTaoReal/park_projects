using QTFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static PixelCrushers.ActiveSaver;

[ObjectEventSystem]
public class UIPage_AnimalPlantOperationComponentAwakeSystem : AAwake<UIPage_AnimalPlantOperationComponent>
{
    public override void Awake(UIPage_AnimalPlantOperationComponent _self)
    {
        _self.Awake();
    }
}

[ObjectEventSystem]
public class UIPage_AnimalPlantOperationComponentFixedUpdateSystem : AFixedUpdate<UIPage_AnimalPlantOperationComponent>
{
    public override void FixedUpdate(UIPage_AnimalPlantOperationComponent _self)
    {
        _self.FixedUpdate();
    }
}


[UIEntityComponent(UI_PrefabPath.m_sUIPage_AnimalPlantOperation)]
public class UIPage_AnimalPlantOperationComponent : UIComponent
{
    //public RectTransform m_kRectTransformCabin;

    public RawImage m_kImageHeadIcon;
    public Text m_kTextTitle;

    public Text m_kTextGrowTile;
    public Text m_kTextWaterTile;
    public Image m_kTextWaterProgress;
    public Text m_kTextFoodTile;
    public Image m_kTextFoodProgress;
    public Text m_kTextOutputTitle;
    public Text m_kTextWaterValue;
    public Text m_kTextFoodValue;
    public Text m_kTextOutPutVal;

    public RawImage m_kObjectHungryState;
    public RawImage m_kObjectThirstyState;
    public RawImage m_kObjectSickState;
    public RawImage m_kObjectDangerState;

    public Button m_kButtonFeedMedicine;
    public Button m_kButtonFeedWater;
    public Button m_kButtonFeedFood;

    public RectTransform m_ObjectNextOp;

    public Button m_kButtonShop;

    public Text m_kTextStateTitle;

    public EventTrigger eventTrigger;


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

        m_kImageHeadIcon = m_kParentEntity.m_kUIPrefab.GetCacheComponent(0) as RawImage;
        m_kTextTitle = m_kParentEntity.m_kUIPrefab.GetCacheComponent(1) as Text;

        m_kTextGrowTile = m_kParentEntity.m_kUIPrefab.GetCacheComponent(2) as Text;
        m_kTextWaterTile = m_kParentEntity.m_kUIPrefab.GetCacheComponent(3) as Text;
        m_kTextWaterProgress = m_kParentEntity.m_kUIPrefab.GetCacheComponent(4) as Image;
        m_kTextFoodTile = m_kParentEntity.m_kUIPrefab.GetCacheComponent(5) as Text;

        m_kTextFoodProgress = m_kParentEntity.m_kUIPrefab.GetCacheComponent(6) as Image;
        m_kTextOutputTitle = m_kParentEntity.m_kUIPrefab.GetCacheComponent(7) as Text;
        m_kTextWaterValue = m_kParentEntity.m_kUIPrefab.GetCacheComponent(8) as Text;
        m_kTextFoodValue = m_kParentEntity.m_kUIPrefab.GetCacheComponent(9) as Text;
        m_kTextOutPutVal = m_kParentEntity.m_kUIPrefab.GetCacheComponent(10) as Text;

        m_kObjectHungryState = m_kParentEntity.m_kUIPrefab.GetCacheComponent(11) as RawImage;
        m_kObjectThirstyState = m_kParentEntity.m_kUIPrefab.GetCacheComponent(12) as RawImage;
        m_kObjectSickState = m_kParentEntity.m_kUIPrefab.GetCacheComponent(13) as RawImage;
        m_kObjectDangerState = m_kParentEntity.m_kUIPrefab.GetCacheComponent(21) as RawImage;

        m_kButtonFeedMedicine = m_kParentEntity.m_kUIPrefab.GetCacheComponent(14) as Button;
        m_kButtonFeedWater = m_kParentEntity.m_kUIPrefab.GetCacheComponent(15) as Button;
        m_kButtonFeedFood = m_kParentEntity.m_kUIPrefab.GetCacheComponent(16) as Button;

        m_ObjectNextOp = m_kParentEntity.m_kUIPrefab.GetCacheComponent(17) as RectTransform;
        m_kButtonShop = m_kParentEntity.m_kUIPrefab.GetCacheComponent(18) as Button;
        m_kTextStateTitle = m_kParentEntity.m_kUIPrefab.GetCacheComponent(19) as Text;

        eventTrigger = m_kParentEntity.m_kUIPrefab.GetCacheComponent(20) as EventTrigger;


        m_kButtonFeedMedicine.onClick.AddListener(OnButtonClick_FeedMedicine);
        m_kButtonFeedWater.onClick.AddListener(OnButtonClick_FeedWater);
        m_kButtonFeedFood.onClick.AddListener(OnButtonClick_FeedFood);
        m_kButtonShop.onClick.AddListener(OnButtonClick_Shop);

        //EventTrigger.Entry entry = new EventTrigger.Entry();
        //entry.eventID = EventTriggerType.PointerDown;
        //entry.callback.AddListener(OnEmptyTouch);
        //eventTrigger.triggers.Add(entry);

        SelectModel(SceneLogic._instance.modelBase.baseData);
    }

    public override void TranslateUI()
    {
        base.TranslateUI();
        m_kTextOutputTitle.text = UI_Helper.GetTextByLanguageID(179);
        m_kTextStateTitle.text = UI_Helper.GetTextByLanguageID(178);

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

    public override void Dispose()
    {
        base.Dispose();
        m_kButtonFeedMedicine.onClick.RemoveListener(OnButtonClick_FeedMedicine);
        m_kButtonFeedWater.onClick.RemoveListener(OnButtonClick_FeedWater);
        m_kButtonFeedFood.onClick.RemoveListener(OnButtonClick_FeedFood);
        m_kButtonShop.onClick.RemoveListener(OnButtonClick_Shop);
        data = null;
    }

    private void OnButtonClick_FeedMedicine()
    {
        nextOpType = 1;
        m_ObjectNextOp.gameObject.SetActive(!m_ObjectNextOp.gameObject.activeSelf);
        ResetNextOpPos(m_kButtonFeedMedicine.transform.position);
        UpdateNextOp();
    }

    private void OnButtonClick_FeedWater()
    {
        //nextOpType = 2;
        //m_ObjectNextOp.gameObject.SetActive(!m_ObjectNextOp.gameObject.activeSelf);
        //ResetNextOpPos(m_kButtonFeedWater.transform.position);
        //UpdateNextOp();

        m_ObjectNextOp.gameObject.SetActive(false);
        Drink();
    }

    private void OnButtonClick_FeedFood()
    {
        nextOpType = 3;
        m_ObjectNextOp.gameObject.SetActive(!m_ObjectNextOp.gameObject.activeSelf);
        ResetNextOpPos(m_kButtonFeedFood.transform.position);
        UpdateNextOp();
    }

    void ResetNextOpPos(Vector3 position)
    {
        m_ObjectNextOp.position = position;
        m_ObjectNextOp.localPosition += new Vector3(0, 150, 0);
    }

    private void OnButtonClick_Shop()
    {
        UIEntity uIEntity = World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPage_Shop);
        uIEntity.GetComponent<UIPage_ShopComponent>().m_kToggleNutrients.isOn = true;
    }


    private void OnEmptyTouch(BaseEventData data)
    {
        m_ObjectNextOp.gameObject.SetActive(false);
    }

    BaseData data = null;
    int nextOpType;
    void SelectModel(BaseData baseData)
    {
        m_ObjectNextOp.gameObject.SetActive(false);
        data = baseData;
    }


    //static int WATER_IDX = 0;
    //static int FOOD_IDX = 1;
    void UpdateUI()
    {

        if (data == null) return;

        var cfg = data.cfg;

        m_kImageHeadIcon.texture = UI_Helper.AllocTexture(cfg._Icon);
        m_kTextTitle.text = UI_Helper.GetTextByLanguageID(cfg._DisplayName);

        ModelBase modelBase = data.GetComponent<ModelBase>();
        if (modelBase == null) return;

        BaseServer Server = modelBase.GetbaseServer;
        if (Server == null) return;

        List<List<int>> m_lStatePro = modelBase.GetStatePro;
        for (int i = 0; i < m_lStatePro.Count; i++)
        {
            List<int> lstate = m_lStatePro[i];
            var curval = Server.proVal[i];
            //if (curval < 0) curval = 0;
            var maxval = lstate[2];
            string _str = modelBase.GetStateName((StatePro)lstate[0]);

            if ((StatePro)lstate[0] == StatePro.Hunger)
            {
                m_kTextFoodTile.text = _str;
                m_kTextFoodProgress.fillAmount = Convert.ToSingle(curval / maxval);
                m_kTextFoodValue.text = curval + "/" + maxval;
            }
            else if((StatePro)lstate[0] == StatePro.Thirst)
            {
                m_kTextWaterTile.text = _str;
                m_kTextWaterProgress.fillAmount = Convert.ToSingle(curval / maxval);
                m_kTextWaterValue.text = curval + "/" + maxval;
            }

            //if (i == WATER_IDX)
            //{
            //    m_kTextWaterTile.text = _str;
            //    m_kTextWaterProgress.fillAmount = Convert.ToSingle(curval / maxval);
            //    m_kTextWaterValue.text = curval + "/" + maxval;
            //}
            //else if (i == FOOD_IDX)
            //{
            //    m_kTextFoodTile.text = _str;
            //    m_kTextFoodProgress.fillAmount = Convert.ToSingle(curval / maxval);
            //    m_kTextFoodValue.text = curval + "/" + maxval;
            //}
        }

        m_kTextGrowTile.text = modelBase.GetGrowthName();
        CS_InOutPut.DataEntry CS_InOutPut = DBManager.Instance.m_kInOutPut.GetEntryPtr(cfg._InOutPutID);

        float val = (float)CS_InOutPut._MatureAwardTime / 60;
        m_kTextOutPutVal.text = val.ToString("f2") + "H";


        m_kObjectHungryState.gameObject.SetActive(false);
        m_kObjectThirstyState.gameObject.SetActive(false);
        m_kObjectSickState.gameObject.SetActive(false);
        m_kObjectDangerState.gameObject.SetActive(false);

        var bufflist = data.GetComponent<ModelBase>().GetMyselfBuff;
        foreach (var buff in bufflist)
        {
            if (buff.CS_Buff._Type == (int)BuffType.Food)
            {
                m_kObjectHungryState.gameObject.SetActive(true);
            }
            else if (buff.CS_Buff._Type == (int)BuffType.Ill)
            {
                m_kObjectSickState.gameObject.SetActive(true);
            }
            else if (buff.CS_Buff._Type == (int)BuffType.Water)
            {
                m_kObjectThirstyState.gameObject.SetActive(true);
            }
            else if (buff.CS_Buff._Type == (int)BuffType.Danger)
            {
                m_kObjectDangerState.gameObject.SetActive(true);
            }
        }
    }
     
    void UpdateNextOp()
    {
        if (!m_ObjectNextOp.gameObject.activeSelf) return;

        List<BagItem> list = null;
        if(nextOpType == 1)
        {
            list = GetPackageFood();
           
        }
        else if(nextOpType == 3)
        {
            list = GetPackageMedicine();
        }
        else
        {
            list = new List<BagItem>();
        }

        Transform node = m_ObjectNextOp.Find("list");
        for (int i = 0; i < list.Count; i++)
        {
            var item = list[i];
            Transform child = null;
            if(node.childCount <= i)
            {
                child = GameObject.Instantiate<Transform>(node.GetChild(0));
                child.SetParent(node);
                child.localScale = Vector3.one;
                child.localRotation = Quaternion.identity;
            }
            else
            {
                child = node.GetChild(i);
            }

            child.gameObject.SetActive(true);

            child.GetComponent<RawImage>().texture = UI_Helper.AllocTexture(item.cfg._Icon);
            child.GetChild(0).GetComponent<Text>().text = "x" + item.info.m_kCount;

            var idx = i;
            var btn = child.GetComponent<Button>();
            btn.onClick.RemoveAllListeners();
            if(nextOpType == 1)
            {
                btn.onClick.AddListener(() =>
                {
                    Feed(idx);
                });
            }
            else if(nextOpType == 2)
            {
                
            }
            else if(nextOpType == 3)
            {
                btn.onClick.AddListener(() =>
                {
                    Medic(idx);
                });
            }

        }

        var rectTransf = m_ObjectNextOp.GetComponent<RectTransform>();
        rectTransf.sizeDelta = new Vector2( 200 + (list.Count) * 102, rectTransf.sizeDelta.y);

        for (int i = list.Count; i < node.childCount; i++){
            node.GetChild(i).gameObject.SetActive(false);
        }
    }

    int GetCurValNeedNum(StatePro statePro, int unitVal)
    {
        ModelBase modelBase = data.GetComponent<ModelBase>();
        if (modelBase == null) return 0;

        BaseServer Server = modelBase.GetbaseServer;
        if (Server == null) return 0;

        List<List<int>> m_lStatePro = modelBase.GetStatePro;
        int idx = -1;
        for (int i = 0; i < m_lStatePro.Count; i++)
        {
            if ((StatePro)m_lStatePro[i][0] == statePro)
            {
                idx = i;
                break;
            }
        }

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
        int needNum = GetCurValNeedNum(StatePro.Thirst, 1);
        if(needNum <= 0)
        {
            UI_Helper.ShowConfirmPanel(UI_Helper.GetTextByLanguageID(256), null);
            return;
        }

        Player player = World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer;
        int HaveNum = (int)player.m_kPlayerBasicAsset.m_kWater;
        if(HaveNum <= 0){
            UI_Helper.ShowCommonTips(UI_Helper.GetTextByLanguageID(187));
            //UI_Helper.ShowConfirmPanel(UI_Helper.GetTextByLanguageID(187), null);
            return;
        }
        int costNum = needNum;
        if (needNum > HaveNum)
        {
            costNum = HaveNum;
            //UI_Helper.ShowCommonTips(UI_Helper.GetTextByLanguageID(257));
            //return;
        }
            
       
            //needNum = (int)mywater;

        string str = UI_Helper.GetTextByLanguageID(185, needNum.ToString(), (HaveNum - costNum).ToString());
        UI_Helper.ShowConfirmPanel(str, () =>
        {
            ModelBase modelBase = data.GetComponent<ModelBase>();
            modelBase.FillOnceStatePro(BuffType.Water, WATER_FIX_ID, costNum);


        }, () =>
        {
            
        });
    }

    void Feed(int idx){
        var list = GetPackageFood();
        if (idx >= list.Count) return;

        var item = list[idx];
        var arr = item.cfg._Use.Split(' ');
        var price = int.Parse(arr[2]);
        var needNum = GetCurValNeedNum(StatePro.Hunger, price);
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

        var list = GetPackageMedicine();
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

    List<BagItem> GetPackageFood()
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
        }

        return SelectItem;
    }

    List<BagItem> GetPackageMedicine()
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
            if (bagItems[i].m_kItemType == PlayerBagAsset.ItemType.Medicine)
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
        }

        return SelectItem;
    }
}
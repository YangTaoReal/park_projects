using QTFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ObjectEventSystem]
public class UIPage_Pack_TemporaryItemAwakeSystem : AAwake<UIPage_Pack_TemporaryItem>
{
    public override void Awake(UIPage_Pack_TemporaryItem _self)
    {
        _self.Awake();
    }
}

[ObjectEventSystem]
public class UIPage_Pack_TemporaryItemFixedUpdateSystem : AFixedUpdate<UIPage_Pack_TemporaryItem>
{
    public override void FixedUpdate(UIPage_Pack_TemporaryItem _self)
    {
        _self.FixedUpdate();
    }
}


[UIEntityComponent(UI_PrefabPath.m_sUIPage_Pack_TemporaryItem)]
public class UIPage_Pack_TemporaryItem : UIComponent
{
    public Text m_kTextGrowingTitle;
    public Text m_kTextGrowingNumber;
    public Slider m_kSliderGrowing;
    public Text m_kTextName;
    public RawImage m_kRawImageIcon;
    public Text m_kTextThirst;
    public Text m_kTextThirstNumber;
    public Text m_kTextFeed;
    public Text m_kTextFeedNumber;

    public Image m_imgThirst;
    public Image m_imgFeed;


    public Button m_btnFood;
    public Button m_btnWater;
    public Button m_btnSeek;
    public Button m_btnDanger;
    public Button m_btnBg;

    private BaseData baseData;
    internal void Awake()
    {
        m_kTextGrowingTitle = m_kParentEntity.m_kUIPrefab.GetCacheComponent(0) as Text;
        m_kTextGrowingNumber = m_kParentEntity.m_kUIPrefab.GetCacheComponent(1) as Text;
        m_kSliderGrowing = m_kParentEntity.m_kUIPrefab.GetCacheComponent(2) as Slider;
        m_kTextName = m_kParentEntity.m_kUIPrefab.GetCacheComponent(3) as Text;
        m_kRawImageIcon = m_kParentEntity.m_kUIPrefab.GetCacheComponent(4) as RawImage;
        m_kTextThirst = m_kParentEntity.m_kUIPrefab.GetCacheComponent(5) as Text;
        m_kTextThirstNumber = m_kParentEntity.m_kUIPrefab.GetCacheComponent(6) as Text;
        m_kTextFeed = m_kParentEntity.m_kUIPrefab.GetCacheComponent(7) as Text;
        m_kTextFeedNumber = m_kParentEntity.m_kUIPrefab.GetCacheComponent(8) as Text;
        m_imgThirst = m_kParentEntity.m_kUIPrefab.GetCacheComponent(9) as Image;
        m_imgFeed = m_kParentEntity.m_kUIPrefab.GetCacheComponent(10) as Image;
        //m_kButtonFood = m_kParentEntity.m_kUIPrefab.GetCacheComponent(9) as Button;
        //m_kButtonWater = m_kParentEntity.m_kUIPrefab.GetCacheComponent(10) as Button;

        m_btnFood = m_kParentEntity.m_kUIPrefab.GetCacheComponent(11) as Button;
        m_btnWater = m_kParentEntity.m_kUIPrefab.GetCacheComponent(12) as Button;
        m_btnSeek = m_kParentEntity.m_kUIPrefab.GetCacheComponent(13) as Button;
        m_btnDanger = m_kParentEntity.m_kUIPrefab.GetCacheComponent(14) as Button;
        m_btnBg = m_kParentEntity.m_kUIPrefab.GetCacheComponent(15) as Button;


        m_btnFood.onClick.AddListener(OnBtnFood);
        m_btnWater.onClick.AddListener(OnBtnWater);
        m_btnSeek.onClick.AddListener(OnBtnSeek);
        m_btnDanger.onClick.AddListener(OnBtnDanger);
        m_btnBg.onClick.AddListener(OnBtnBg);
 
        m_kSliderGrowing.gameObject.SetActive(false);

        baseData = null;
    }

    public void FixedUpdate()
    {
        UpdateUI();
    }

    public override void TranslateUI()
    {
        base.TranslateUI();

    }
    public override void Dispose()
    {
        base.Dispose();
        m_btnFood.onClick.RemoveAllListeners();
        m_btnWater.onClick.RemoveAllListeners();
        m_btnSeek.onClick.RemoveAllListeners();
        m_btnDanger.onClick.RemoveAllListeners();
        m_btnBg.onClick.RemoveAllListeners();
    }
    void OnBtnWater()
    {

    }
    void OnBtnFood()
    {

    }
    void OnBtnSeek()
    {

    }
    void OnBtnDanger()
    {

    }
    void OnBtnBg()
    {
        if (SceneLogic._instance.selectTileInfo == null || baseData == null) return;
        Park park = ModelManager._instance.GetParkList(new List<Guid>() { SceneLogic._instance.selectTileInfo.guid });
        bool isSucceed = false;
        if(baseData.cfg._Type == (int)ModeTyp.Animal)
        {
            park.AddItem(baseData);
            isSucceed = ModelManager._instance.CreateAnimalPark(baseData.cfg._ID, baseData);
            if (isSucceed) GameEventManager._Instance.onPlaceAnimalOrPlant();
        }
        else if(baseData.cfg._Type == (int)ModeTyp.Plant)
        {
            park.AddItem(baseData);
            isSucceed = ModelManager._instance.CreatePlantPark(baseData.cfg._ID, baseData);
            if (isSucceed) GameEventManager._Instance.onPlaceAnimalOrPlant();
        }

        if(isSucceed)
        {
            if (baseData.cfg._Type == (int)ModeTyp.Animal)
                baseData.GetComponent<AnimalMove>().enabled = true;

            baseData.go.SetActive(true);
            World.Scene.GetComponent<UIManagerComponent>().Remove(UI_PrefabPath.m_sUIPage_Pack_TemporaryItem);
            Player player = World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer;
            player.RemoveSettlements(baseData.guid);
            var com = World.Scene.GetComponent<UIManagerComponent>().Get(UI_PrefabPath.m_sUIPage_Pack);
            com.GetComponent<UIPage_PackComponent>().InitSettlements();  
        }

     
  
    }

    public void InitItem(BaseData _baseData)
    {
        baseData = _baseData;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (baseData == null) return;
        CS_Model.DataEntry dataEntry = DBManager.Instance.m_kModel.GetEntryPtr(baseData.cfg._ID);
        if (dataEntry == null) return;
        ModelBase modelBase = baseData.GetComponent<ModelBase>();
        if (modelBase == null) return;
        BaseServer Server = modelBase.GetbaseServer;
        if (Server == null) return;

        m_kTextName.text = UI_Helper.GetTextByLanguageID(dataEntry._DisplayName);
        m_kRawImageIcon.texture = UI_Helper.AllocTexture(dataEntry._Icon);
        List<List<int>> m_lStatePro = modelBase.GetStatePro;
        for (int i = 0; i < m_lStatePro.Count; i++)
        {
            List<int> lstate = m_lStatePro[i];
            var curval = Server.proVal[i];
            //if (curval < 0) curval = 0;
            var maxval = lstate[2];
            string _str = modelBase.GetStateName((StatePro)lstate[0]);
            if (i == 0)
            {
                m_kTextThirst.text = _str;
                m_imgThirst.fillAmount = Convert.ToSingle(curval / maxval);
                m_kTextThirstNumber.text = curval + "/" + maxval;
            }
            else if (i == 1)
            {
                m_kTextFeed.text = _str;
                m_imgFeed.fillAmount = Convert.ToSingle(curval / maxval);
                m_kTextFeedNumber.text = curval + "/" + maxval;
            }
        }

 
        m_btnFood.gameObject.SetActive(false);
        m_btnWater.gameObject.SetActive(false);
        m_btnSeek.gameObject.SetActive(false);
        m_btnDanger.gameObject.SetActive(false);

        var bufflist = baseData.GetComponent<ModelBase>().GetMyselfBuff;
        foreach (var buff in bufflist)
        {
            if (buff.CS_Buff._Type == (int)BuffType.Food)
            {
                m_btnFood.gameObject.SetActive(true);
            }
            else if (buff.CS_Buff._Type == (int)BuffType.Water)
            {
                m_btnWater.gameObject.SetActive(true);
            }
            else if (buff.CS_Buff._Type == (int)BuffType.Ill)
            {
                m_btnSeek.gameObject.SetActive(true);
            }
            else if (buff.CS_Buff._Type == (int)BuffType.Danger)
            {
                m_btnDanger.gameObject.SetActive(true);
            }
        }
    }



}

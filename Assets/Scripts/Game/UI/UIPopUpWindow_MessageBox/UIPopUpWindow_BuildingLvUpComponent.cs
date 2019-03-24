using System;
using System.Collections;
using System.Collections.Generic;

using QTFramework;

using UnityEngine;
using UnityEngine.UI;

[ObjectEventSystem]
public class UIPopUpWindow_BuildingLvUpComponentAwakeSystem : AAwake<UIPopUpWindow_BuildingLvUpComponent>
{
    public override void Awake(UIPopUpWindow_BuildingLvUpComponent _self)
    {
        _self.Awake();
    }
}

[UIEntityComponent(UI_PrefabPath.m_sUIPopUpWindow_BuildingLvUpBox)]
public class UIPopUpWindow_BuildingLvUpComponent : UIComponent
{
    public Text m_textTitle;
    public Text m_textDes;
    public Text m_textBtn;
    public Text m_textImpact;
    public Text m_textAsset;
    public RawImage m_rawImgIcon;
    public Image m_imgAsset;
    public Button m_btnClose;
    public Button m_btnEnsure;
    public Text m_textTime;
    public Text m_textLvUpDes;
    BaseData baseData;
    //Action _okAction;
    internal void Awake()
    {
        m_textTitle = m_kParentEntity.m_kUIPrefab.GetCacheComponent(0) as Text;
        m_textDes = m_kParentEntity.m_kUIPrefab.GetCacheComponent(1) as Text;
        m_textBtn = m_kParentEntity.m_kUIPrefab.GetCacheComponent(2) as Text;
        m_textImpact = m_kParentEntity.m_kUIPrefab.GetCacheComponent(3) as Text;
        m_textAsset = m_kParentEntity.m_kUIPrefab.GetCacheComponent(4) as Text;

        m_rawImgIcon = m_kParentEntity.m_kUIPrefab.GetCacheComponent(5) as RawImage;
        m_imgAsset = m_kParentEntity.m_kUIPrefab.GetCacheComponent(6) as Image;

        m_btnClose = m_kParentEntity.m_kUIPrefab.GetCacheComponent(7) as Button;
        m_btnEnsure = m_kParentEntity.m_kUIPrefab.GetCacheComponent(8) as Button;

        m_textTime = m_kParentEntity.m_kUIPrefab.GetCacheComponent(9) as Text;
        m_textLvUpDes = m_kParentEntity.m_kUIPrefab.GetCacheComponent(10) as Text;

        m_btnClose.onClick.AddListener(onBtnClose);
        m_btnEnsure.onClick.AddListener(onBtnEnsure);
    }
    public override void Dispose()
    {
        m_btnClose.onClick.RemoveListener(onBtnClose);
        m_btnEnsure.onClick.RemoveListener(onBtnEnsure);
    }
    public override void TranslateUI()
    {
        base.TranslateUI();

        m_textBtn.text = UI_Helper.GetTextByLanguageID(171);
    }

    void onBtnEnsure()
    {
        //_okAction?.Invoke();

        World.Scene.GetComponent<UIManagerComponent>().Remove(UI_PrefabPath.m_sUIPage_BuildingOperation);
        World.Scene.GetComponent<UIManagerComponent>().Remove(UI_PrefabPath.m_sUIPopUpWindow_BuildingLvUpBox);
        BE.MobileRTSCam.instance.RestEditor();
        CS_UpLevel.DataEntry CS_UpLevel = baseData.GetComponent<Building>().CS_UpLevel;
        string[] split = CS_UpLevel._Expend.Split(' ');
        int ttype = int.Parse(split[0]);
        if (ttype == 0)
        {
            return;
        }
        decimal tval = decimal.Parse(split[2]);
        World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.AddAsset((PlayerBagAsset.ItemType) ttype, -tval, SuccessCallBack);

    }
    private void SuccessCallBack(decimal _price)
    {
        ModelManager._instance.SelectLvUp();
        World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.SetTask(TaskType.BuildingUpgrade, 1);
    }
    void onBtnClose()
    {
        World.Scene.GetComponent<UIManagerComponent>().Remove(UI_PrefabPath.m_sUIPopUpWindow_BuildingLvUpBox);
    }

    public void Init(BaseData _baseData)
    {
        baseData = _baseData;
        BuildingServer server = baseData.GetComponent<Building>().GetServer;
        CS_UpLevel.DataEntry CS_UpLevel = baseData.GetComponent<Building>().CS_UpLevel;

        CS_Model.DataEntry nextModel = DBManager.Instance.m_kModel.GetEntryPtr(CS_UpLevel._NextID);

        //m_imgIcon.sprite = UI_Helper.GetSprite(nextModel._Icon);
        m_rawImgIcon.texture = UI_Helper.AllocTexture(nextModel._Icon);

        float val = CS_UpLevel._UpTime / 3600.0f;
        m_textTime.text = val.ToString("f2") + "H";
        m_textDes.text = UI_Helper.GetTextByLanguageID(CS_UpLevel._Des);
        m_textTitle.text = UI_Helper.GetTextByLanguageID(172, (server.lv + 1).ToString());

        string[] split = CS_UpLevel._Expend.Split(' ');
        if (int.Parse(split[0]) == 0)
        {
            m_textAsset.gameObject.SetActive(false);
            m_imgAsset.gameObject.SetActive(false);
        }
        else
        {
            m_textAsset.gameObject.SetActive(true);
            m_imgAsset.gameObject.SetActive(true);

            m_imgAsset.sprite = ModelManager._instance.GetAssetIcon(int.Parse(split[0]));
            m_textAsset.text = split[2];
        }

        m_textLvUpDes.text = UI_Helper.GetTextByLanguageID(CS_UpLevel._Des);

    }

}
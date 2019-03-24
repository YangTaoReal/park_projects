using QTFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ObjectEventSystem]
public class UIPage_PackItemDetailsComponentAwakeSystem : AAwake<UIPage_PackItemDetailsComponent>
{
    public override void Awake(UIPage_PackItemDetailsComponent _self)
    {
        _self.Awake();
    }
}


[UIEntityComponent(UI_PrefabPath.m_sUIPage_PackItemDetails)]
public class UIPage_PackItemDetailsComponent : UIComponent
{
    public Button m_ButtonClose;
    public Button m_maskBtn;
    public Text m_TextTitle;
    public Text m_TextAmountTitle;
    public Text m_TextAmountNumber;
    public Text m_TextOccupyTitle;
    public Text m_TextOccupyNumber;
    public RawImage m_RawImageIcon;
    public Text m_TextDesc;

    public Text m_TextGrowUptitle;
    public Text m_TextRipetitle;

    public Text m_TextGrowupTitle1;
    public Text m_TextGrowupTitle2;
    public Text m_TextGrowupTitle3;
    public Text m_TextGrowupTitle4;

    public Image m_ImageGrowup4;

    public Text m_TextGrowupNumber1;
    public Text m_TextGrowupNumber2;
    public Text m_TextGrowupNumber3;
    public Text m_TextGrowupNumber4;

    public Text m_TextRipeTitle1;
    public Text m_TextRipeTitle2;
    public Text m_TextRipeTitle3;


    public Image m_ImageRipe3;

    public Text m_TextRipeNumber1;
    public Text m_TextRipeNumber2;
    public Text m_TextRipeNumber3;

    public RectTransform m_RectTransformNoItem;

    private PlayerBagAsset.BagItem Item;

    internal void Awake()
    {
        m_ButtonClose = m_kParentEntity.m_kUIPrefab.GetCacheComponent(0) as Button;
        m_maskBtn = m_kParentEntity.m_kUIPrefab.GetCacheComponent(1) as Button;
        m_TextTitle = m_kParentEntity.m_kUIPrefab.GetCacheComponent(2) as Text;
        m_TextAmountTitle = m_kParentEntity.m_kUIPrefab.GetCacheComponent(3) as Text;
        m_TextAmountNumber = m_kParentEntity.m_kUIPrefab.GetCacheComponent(4) as Text;
        m_TextOccupyTitle = m_kParentEntity.m_kUIPrefab.GetCacheComponent(5) as Text;
        m_TextOccupyNumber = m_kParentEntity.m_kUIPrefab.GetCacheComponent(6) as Text;
        m_RawImageIcon = m_kParentEntity.m_kUIPrefab.GetCacheComponent(7) as RawImage;
        m_TextDesc = m_kParentEntity.m_kUIPrefab.GetCacheComponent(8) as Text;

        m_TextGrowUptitle = m_kParentEntity.m_kUIPrefab.GetCacheComponent(9) as Text;
        m_TextRipetitle = m_kParentEntity.m_kUIPrefab.GetCacheComponent(10) as Text;

        m_TextGrowupTitle1 = m_kParentEntity.m_kUIPrefab.GetCacheComponent(11) as Text;
        m_TextGrowupTitle2 = m_kParentEntity.m_kUIPrefab.GetCacheComponent(12) as Text;
        m_TextGrowupTitle3 = m_kParentEntity.m_kUIPrefab.GetCacheComponent(13) as Text;
        m_TextGrowupTitle4 = m_kParentEntity.m_kUIPrefab.GetCacheComponent(14) as Text;

        m_ImageGrowup4 = m_kParentEntity.m_kUIPrefab.GetCacheComponent(15) as Image;

        m_TextGrowupNumber1 = m_kParentEntity.m_kUIPrefab.GetCacheComponent(16) as Text;
        m_TextGrowupNumber2 = m_kParentEntity.m_kUIPrefab.GetCacheComponent(17) as Text;
        m_TextGrowupNumber3 = m_kParentEntity.m_kUIPrefab.GetCacheComponent(18) as Text;
        m_TextGrowupNumber4 = m_kParentEntity.m_kUIPrefab.GetCacheComponent(19) as Text;

        m_TextRipeTitle1 = m_kParentEntity.m_kUIPrefab.GetCacheComponent(20) as Text;
        m_TextRipeTitle2 = m_kParentEntity.m_kUIPrefab.GetCacheComponent(21) as Text;
        m_TextRipeTitle3 = m_kParentEntity.m_kUIPrefab.GetCacheComponent(22) as Text;

        m_ImageRipe3 = m_kParentEntity.m_kUIPrefab.GetCacheComponent(23) as Image;

        m_TextRipeNumber1 = m_kParentEntity.m_kUIPrefab.GetCacheComponent(24) as Text;
        m_TextRipeNumber2 = m_kParentEntity.m_kUIPrefab.GetCacheComponent(25) as Text;
        m_TextRipeNumber3 = m_kParentEntity.m_kUIPrefab.GetCacheComponent(26) as Text;

        m_RectTransformNoItem = m_kParentEntity.m_kUIPrefab.GetCacheComponent(27) as RectTransform;
     



        m_ButtonClose.onClick.AddListener(onClick_Close);
        m_maskBtn.onClick.RemoveAllListeners();
        m_maskBtn.onClick.AddListener(onMaskBtnClick);
    }
    public override void TranslateUI()
    {
        m_TextAmountTitle.text = UI_Helper.GetTextByLanguageID(110);//数量
        m_TextAmountNumber.text = UI_Helper.GetTextByLanguageID(111);//占地体积

        m_TextGrowupTitle1.text = UI_Helper.GetTextByLanguageID(234);//生长周期
        m_TextGrowupTitle2.text = UI_Helper.GetTextByLanguageID(235);//饮水消耗
        m_TextGrowupTitle3.text = UI_Helper.GetTextByLanguageID(236);//饲料消耗
        m_TextGrowupTitle4.text = UI_Helper.GetTextByLanguageID(237);//金币产出

        m_TextRipeTitle1.text = UI_Helper.GetTextByLanguageID(234);//生长周期
        m_TextRipeTitle2.text = UI_Helper.GetTextByLanguageID(235);//饮水消耗
        m_TextRipeTitle3.text = UI_Helper.GetTextByLanguageID(237);//金币产出
    }
    private void onMaskBtnClick()
    {
        if (GuidanceManager.isGuidancing && GuidanceManager.currStep == GuidanceStep.Step6)
        {
            //World.Scene.GetComponent<UIManagerComponent>().Remove(UI_PrefabPath.m_sUIPage_PackItemDetails);
            //World.Scene.GetComponent<UIManagerComponent>().Remove(UI_PrefabPath.m_sUIPage_Pack);

            World.Scene.GetComponent<UIManagerComponent>().RemoveAll(UI_PrefabPath.m_sUISystem_Hall);
            World.Scene.GetComponent<UIManagerComponent>().ClearUIStack();

            //GuidanceData data = new GuidanceData();
            //ObserverHelper<GuidanceData>.SendMessage(MessageMonitorType.GuidanceClickEvent, this, new MessageArgs<GuidanceData>(data));
            //Debug.Log("第五步新手引导");
            GuidanceManager._Instance.CheckGuidance(null);
        }
    }

    private void onClick_Close()
    {
        World.Scene.GetComponent<UIManagerComponent>().Remove(UI_PrefabPath.m_sUIPage_PackItemDetails);
    }

    public void InitUI(PlayerBagAsset.BagItem _Item)
    {
        if (_Item.m_kItemType == PlayerBagAsset.ItemType.Animal || _Item.m_kItemType == PlayerBagAsset.ItemType.Botany)
        {
            m_RectTransformNoItem.gameObject.SetActive(true);
            CS_Model.DataEntry dataEntry = DBManager.Instance.m_kModel.GetEntryPtr(_Item.m_kItemID);
            if (dataEntry == null)
            {
                return;
            }
            m_RawImageIcon.texture = UI_Helper.AllocTexture(dataEntry._Icon);
            m_TextDesc.text = UI_Helper.GetTextByLanguageID(dataEntry._Desc);
            m_TextTitle.text = UI_Helper.GetTextByLanguageID(dataEntry._DisplayName);
            Player player = World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer;
            
            if (_Item == null)
            {
                m_TextAmountNumber.text = "0";
            }
            else
            {
                m_TextAmountNumber.text = _Item.m_kCount.ToString();
            }
            m_TextOccupyNumber.text = dataEntry._Capacity.ToString();

            CS_InOutPut.DataEntry inOutPut = null;
            inOutPut = DBManager.Instance.m_kInOutPut.GetEntryPtr(dataEntry._InOutPutID);
            if (inOutPut == null)
            {
                return;
            }

            //m_TextGrowupTitle1.text = UI_Helper.GetTextByLanguageID(234);//生长周期
            //m_TextGrowupTitle2.text = UI_Helper.GetTextByLanguageID(235);//饮水消耗
            //m_TextGrowupTitle3.text = UI_Helper.GetTextByLanguageID(236);//饲料消耗
            //m_TextGrowupTitle4.text = UI_Helper.GetTextByLanguageID(237);//金币产出

            m_TextGrowupNumber1.text = $"{(inOutPut._GrowTime / 60f).ToString("0.0")}/h";
            List<Vector3Int> youthbnumberXiaoHao = GetVector3Ints(inOutPut._YoungInt);
            for (int i = 0; i < youthbnumberXiaoHao.Count; i++)
            {
                if (youthbnumberXiaoHao[i].x == 1)
                {
                    m_TextGrowupNumber2.text = $"{(youthbnumberXiaoHao[i].y / 60f).ToString("0.0")}/h";
                    break;
                }
                else if(youthbnumberXiaoHao[i].x == 2)
                {
                    m_TextGrowupNumber3.text = $"{(youthbnumberXiaoHao[i].y / 60f).ToString("0.0")}/h";
                    break;
                }
            }

            List<Vector3Int> youthbnumberList = GetVector3Ints(inOutPut._YoungOut);
            for (int i = 0; i < youthbnumberList.Count; i++)
            {
                if (youthbnumberList[i].x == 1)
                {
                    m_ImageGrowup4.sprite = UI_Helper.GetSprite("game_gold_1");
                    m_TextGrowupNumber4.text = $"{(youthbnumberList[i].z / 60f).ToString("0.0")}/h";
                    break;
                }
                else if (youthbnumberList[i].x == 2)
                {
                    m_ImageGrowup4.sprite = UI_Helper.GetSprite("game_gold_2");
                    m_TextGrowupNumber4.text = $"{(youthbnumberList[i].z / 60f).ToString("0.0")}/h";
                    break;
                }
            }


            //m_TextRipeTitle1.text = UI_Helper.GetTextByLanguageID(234);//生长周期
            //m_TextRipeTitle2.text = UI_Helper.GetTextByLanguageID(235);//饮水消耗
            //m_TextRipeTitle3.text = UI_Helper.GetTextByLanguageID(237);//饲料消耗
            //m_TextRipeTitle4.text = UI_Helper.GetTextByLanguageID(236);//金币产出

            List<Vector3Int> grownXiaoHao = GetVector3Ints(inOutPut._MatureInt);
            for (int i = 0; i < grownXiaoHao.Count; i++)
            {
                if (grownXiaoHao[i].x == 1)
                {
                    m_TextRipeNumber1.text = $"{(grownXiaoHao[i].y / 60f).ToString("0.0")}/h";
                    break;
                }
                else if (youthbnumberXiaoHao[i].x == 2)
                {
                    m_TextRipeNumber2.text = $"{(grownXiaoHao[i].y / 60f).ToString("0.0")}/h";
                    break;
                }
            }

            List<Vector3Int> grownNumberList = GetVector3Ints(inOutPut._MatureOut);
            for (int i = 0; i < grownNumberList.Count; i++)
            {
                if (grownNumberList[i].x == 1)
                {
                    m_ImageRipe3.sprite = UI_Helper.GetSprite("game_gold_1");
                    m_TextRipeNumber3.text = $"{(grownNumberList[i].z / 60f).ToString("0.0")}/h";
                    break;
                }
                else if (grownNumberList[i].x == 2)
                {
                    m_ImageRipe3.sprite = UI_Helper.GetSprite("game_gold_2");
                    m_TextRipeNumber3.text = $"{(grownNumberList[i].z / 60f).ToString("0.0")}/h";
                    break;
                }
            }
        }
        else
        {
            m_RectTransformNoItem.gameObject.SetActive(false);
            CS_Items.DataEntry dataEntry = DBManager.Instance.m_kItems.GetEntryPtr(_Item.m_kItemID);
            if (dataEntry == null)
            {
                return;
            }
            m_TextTitle.text = UI_Helper.GetTextByLanguageID(dataEntry._DisplayName);
            m_RawImageIcon.texture = UI_Helper.AllocTexture(dataEntry._Icon);
            m_TextDesc.text = UI_Helper.GetTextByLanguageID(dataEntry._Desc);
            if (_Item == null)
            {
                m_TextAmountNumber.text = "0";
            }
            else
            {
                m_TextAmountNumber.text = _Item.m_kCount.ToString();
            }
        }


    }

    private List<Vector3Int> GetVector3Ints(string v3List)
    {
        List<Vector3Int> Vector3IntList = new List<Vector3Int>();
        string[] _useList = v3List.Split('|');
        for (int i = 0; i < _useList.Length; i++)
        {
            string[] _usexyz = _useList[i].Split(' ');
            if (_usexyz.Length < 3)
            {
                return Vector3IntList;
            }
            Vector3Int vector3Int = new Vector3Int(int.Parse(_usexyz[0]), int.Parse(_usexyz[1]), int.Parse(_usexyz[2]));
            Vector3IntList.Add(vector3Int);
        }
        return Vector3IntList;
    }
}

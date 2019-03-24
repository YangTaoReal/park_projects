using System;
using System.Collections;
using System.Collections.Generic;

using QTFramework;

using UnityEngine;
using UnityEngine.UI;

[ObjectEventSystem]
public class UIPage_ShopBuyComponentAwakeSystem : AAwake<UIPage_ShopBuyComponent>
{
    public override void Awake(UIPage_ShopBuyComponent _self)
    {
        _self.Awake();
    }
}

[UIEntityComponent(UI_PrefabPath.m_sUIPage_ShopBuy)]
public class UIPage_ShopBuyComponent : UIComponent
{
    public Button m_kButtonClose;
    public Text m_kTextTitle;
    public Text m_kTextAmounttitle;
    public Text m_kTextAmountNumber;
    public Text m_kTextOCCUPYtitle;
    public Text m_kTextOCCUPYNumber;
    public RawImage m_kRawImageIcon;
    public Text m_kTextDesc;
    public Text m_kTextHourlytitle;
    public Text m_kTextyouthbTitler;
    public Text m_kTextyouthbnumber;
    public Text m_kTextgrowntitle;
    public Text m_kTextgrownNumber;
    public Button m_kButtonleft;
    public Button m_kButtonright;
    public InputField m_kInputFieldBuyNumber;
    public Button m_kButtonGoldBuy;
    public Button m_kButtonStoneBuy;
    public Text m_kTextGoldBuyNumber;
    public Text m_kTextStoneNumber;
    public Text m_kTextGoldBuy;
    public Text m_kTextStoneBuy;
    public RectTransform m_kRectTransformHourly;
    public Image m_kImageIcon;

    private int currentNumber = 1;

    private CS_Shop.DataEntry m_ShopDataEntry = null;
    internal void Awake()
    {
        m_kButtonClose = m_kParentEntity.m_kUIPrefab.GetCacheComponent(0) as Button;
        m_kTextTitle = m_kParentEntity.m_kUIPrefab.GetCacheComponent(1) as Text;
        m_kTextAmounttitle = m_kParentEntity.m_kUIPrefab.GetCacheComponent(2) as Text;
        m_kTextAmountNumber = m_kParentEntity.m_kUIPrefab.GetCacheComponent(3) as Text;
        m_kTextOCCUPYtitle = m_kParentEntity.m_kUIPrefab.GetCacheComponent(4) as Text;
        m_kTextOCCUPYNumber = m_kParentEntity.m_kUIPrefab.GetCacheComponent(5) as Text;
        m_kRawImageIcon = m_kParentEntity.m_kUIPrefab.GetCacheComponent(6) as RawImage;
        m_kTextDesc = m_kParentEntity.m_kUIPrefab.GetCacheComponent(7) as Text;
        m_kTextHourlytitle = m_kParentEntity.m_kUIPrefab.GetCacheComponent(8) as Text;
        m_kTextyouthbnumber = m_kParentEntity.m_kUIPrefab.GetCacheComponent(9) as Text;
        m_kTextgrowntitle = m_kParentEntity.m_kUIPrefab.GetCacheComponent(10) as Text;
        m_kTextgrownNumber = m_kParentEntity.m_kUIPrefab.GetCacheComponent(11) as Text;
        m_kButtonleft = m_kParentEntity.m_kUIPrefab.GetCacheComponent(12) as Button;
        m_kButtonright = m_kParentEntity.m_kUIPrefab.GetCacheComponent(13) as Button;
        m_kInputFieldBuyNumber = m_kParentEntity.m_kUIPrefab.GetCacheComponent(14) as InputField;
        m_kButtonGoldBuy = m_kParentEntity.m_kUIPrefab.GetCacheComponent(15) as Button;
        m_kButtonStoneBuy = m_kParentEntity.m_kUIPrefab.GetCacheComponent(16) as Button;
        m_kTextGoldBuyNumber = m_kParentEntity.m_kUIPrefab.GetCacheComponent(17) as Text;
        m_kTextStoneNumber = m_kParentEntity.m_kUIPrefab.GetCacheComponent(18) as Text;
        m_kTextyouthbTitler = m_kParentEntity.m_kUIPrefab.GetCacheComponent(19) as Text;
        m_kTextGoldBuy = m_kParentEntity.m_kUIPrefab.GetCacheComponent(20) as Text;
        m_kTextStoneBuy = m_kParentEntity.m_kUIPrefab.GetCacheComponent(21) as Text;

        m_kRectTransformHourly = m_kParentEntity.m_kUIPrefab.GetCacheComponent(22) as RectTransform;
        m_kImageIcon = m_kParentEntity.m_kUIPrefab.GetCacheComponent(23) as Image; 

        m_kButtonleft.onClick.AddListener(OnButtonClick_Left);
        m_kButtonright.onClick.AddListener(OnButtonClick_Right);


        m_kButtonleft.GetComponent<OnButtonPressed>().m_kActionDown = ActionDownLeft;
        m_kButtonright.GetComponent<OnButtonPressed>().m_kActionDown = ActionDownRight;


        m_kButtonGoldBuy.onClick.AddListener(OnButtonClick_GoldBuy);
        m_kButtonStoneBuy.onClick.AddListener(OnButtonClick_StoneBuy);
        m_kInputFieldBuyNumber.onValueChanged.AddListener(OnValueChanged_InputFieldBuyNumber);

        m_kButtonClose.onClick.AddListener(OnButtonClick_Close);
    }

    private void ActionDownRight()
    {
        OnButtonClick_Right();
    }

    private void ActionDownLeft()
    {
        OnButtonClick_Left();
    }

    public override void Dispose()
    {
        base.Dispose();

        m_kButtonleft.onClick.RemoveListener(OnButtonClick_Left);
        m_kButtonright.onClick.RemoveListener(OnButtonClick_Right);
        m_kButtonGoldBuy.onClick.RemoveListener(OnButtonClick_GoldBuy);
        m_kButtonStoneBuy.onClick.RemoveListener(OnButtonClick_StoneBuy);
        m_kInputFieldBuyNumber.onValueChanged.RemoveListener(OnValueChanged_InputFieldBuyNumber);
        m_kButtonClose.onClick.RemoveListener(OnButtonClick_Close);

    }
    public override void TranslateUI()
    {
        m_kTextAmounttitle.text = UI_Helper.GetTextByLanguageID(110); //数量
        m_kTextOCCUPYtitle.text = UI_Helper.GetTextByLanguageID(111); //占地体积

        m_kTextGoldBuy.text = UI_Helper.GetTextByLanguageID(112); //买
        m_kTextStoneBuy.text = UI_Helper.GetTextByLanguageID(112); //买
        m_kTextHourlytitle.text = UI_Helper.GetTextByLanguageID(135); //收益
    }
    private void OnValueChanged_InputFieldBuyNumber(string arg0)
    {
        int inputNumber = int.Parse(arg0);
        if (inputNumber > 0 && inputNumber != currentNumber)
        {
            currentNumber = inputNumber;
            m_kInputFieldBuyNumber.text = currentNumber.ToString();
            if (m_ShopDataEntry != null)
            {
                m_kTextGoldBuyNumber.text = (currentNumber * m_ShopDataEntry._GoldPrice.z).ToString();
                m_kTextStoneNumber.text = (currentNumber * m_ShopDataEntry._StonePrice.z).ToString();
            }
        }
    }

    private void OnButtonClick_GoldBuy()
    {
        Player player = World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer;
        if (player.CanPutInBag((int) m_ShopDataEntry._Goods.y, (int) m_ShopDataEntry._Goods.z))
        {
            player.CostGold((decimal) (currentNumber * m_ShopDataEntry._GoldPrice.z), ActionGoldCallBack);
        }
        else
        {
            UI_Helper.ShowCommonTips(231);
        }

    }
    private void ActionGoldCallBack(decimal _price)
    {
        BuyItem();
        OnButtonClick_Close();
    }

    private void OnButtonClick_StoneBuy()
    {
        Player player = World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer;
        if (player.CanPutInBag((int) m_ShopDataEntry._Goods.y, (int) m_ShopDataEntry._Goods.z))
        {
            player.CostStone((decimal) (currentNumber * m_ShopDataEntry._StonePrice.z), ActionStoneCallBack);
        }
        else
        {
            UI_Helper.ShowCommonTips(231);
        }
    }

    private void ActionStoneCallBack(decimal _price)
    {
        BuyItem();
        OnButtonClick_Close();
    }


    private void BuyItem()
    {
        Player player = World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer;

        if ((PlayerBagAsset.ItemType)m_ShopDataEntry._Goods.x == PlayerBagAsset.ItemType.Gift)
        {

            CS_Items.DataEntry dataEntry = DBManager.Instance.m_kItems.GetEntryPtr((int)m_ShopDataEntry._Goods.y);
            if (dataEntry == null)
            {
                return;
            }
            List<Vector3> vector3s = GetVector3Ints(dataEntry._Use);
            for (int i=0;i< vector3s.Count;i++)
            {
                if (vector3s[i].x == 1)
                {
                    player.AddGold((decimal)vector3s[i].z * currentNumber);
                }
                else if (vector3s[i].x == 2)
                {
                    player.AddStone((decimal)vector3s[i].z * currentNumber);
                }
                else if (vector3s[i].x == 3)
                {
                    player.AddSun((decimal)vector3s[i].z * currentNumber);
                }
                else if (vector3s[i].x == 4)
                {
                    player.AddWater((decimal)vector3s[i].z * currentNumber);
                }
                else if (vector3s[i].x == 5)
                {
                    player.AddItemToBag((int)vector3s[i].y, PlayerBagAsset.ItemType.Animal, (int)vector3s[i].z * currentNumber);
                }
                else if (vector3s[i].x ==6)
                {
                    player.AddItemToBag((int)vector3s[i].y, PlayerBagAsset.ItemType.Botany, (int)vector3s[i].z * currentNumber);
                }
                else if (vector3s[i].x == 8)
                {
                    player.AddItemToBag((int)vector3s[i].y, PlayerBagAsset.ItemType.StageProperty, (int)vector3s[i].z*currentNumber);
                }
            }
        }
        else
        {
            player.AddItemToBag((int)m_ShopDataEntry._Goods.y, (PlayerBagAsset.ItemType)m_ShopDataEntry._Goods.x, currentNumber);
        }


        World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.SetTask(TaskType.BuyItem, (int)currentNumber);
        if ((PlayerBagAsset.ItemType)m_ShopDataEntry._Goods.x == PlayerBagAsset.ItemType.Animal)
        {
            World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.SetTask(TaskType.BuyAnimal, currentNumber);
        }
        if ((PlayerBagAsset.ItemType)m_ShopDataEntry._Goods.x == PlayerBagAsset.ItemType.Botany)
        {
            World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.SetTask(TaskType.BuyPlant, currentNumber);
        }


    }

    private void OnButtonClick_Right()
    {
        if (currentNumber >= 999)
        {
            currentNumber = 999;
        }
        else
        {
            currentNumber++;
        }

        m_kInputFieldBuyNumber.text = currentNumber.ToString();
        if (m_ShopDataEntry != null)
        {
            m_kTextGoldBuyNumber.text = (currentNumber * m_ShopDataEntry._GoldPrice.z).ToString();
            m_kTextStoneNumber.text = (currentNumber * m_ShopDataEntry._StonePrice.z).ToString();
        }
    }

    private void OnButtonClick_Left()
    {
        if (currentNumber <= 1)
        {
            currentNumber = 99;
        }
        else
        {
            currentNumber--;
        }

        m_kInputFieldBuyNumber.text = currentNumber.ToString();
        if (m_ShopDataEntry != null)
        {
            m_kTextGoldBuyNumber.text = (currentNumber * m_ShopDataEntry._GoldPrice.z).ToString();
            m_kTextStoneNumber.text = (currentNumber * m_ShopDataEntry._StonePrice.z).ToString();
        }
    }

    private void OnButtonClick_Close()
    {
        World.Scene.GetComponent<UIManagerComponent>().Remove(UI_PrefabPath.m_sUIPage_ShopBuy);
    }

    public void Init(int _shopID)
    {
        currentNumber = 1;
        m_kInputFieldBuyNumber.text = currentNumber.ToString();

        m_ShopDataEntry = DBManager.Instance.m_kShop.GetEntryPtr(_shopID);
        if (m_ShopDataEntry == null)
        {
            return;
        }

        m_kTextTitle.text = UI_Helper.GetTextByLanguageID(m_ShopDataEntry._DisplayName);
        m_kRawImageIcon.texture = UI_Helper.AllocTexture(m_ShopDataEntry._Icon);
        Player player = World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer;

        if (m_ShopDataEntry._Category == (int) ShopCategory.Animal || m_ShopDataEntry._Category == (int) ShopCategory.Botany)
        {
            m_kTextOCCUPYtitle.gameObject.SetActive(true);
            m_kTextOCCUPYNumber.gameObject.SetActive(true);
            m_kRectTransformHourly.gameObject.SetActive(true);
 
            CS_Model.DataEntry _modelDataEntry = DBManager.Instance.m_kModel.GetEntryPtr((int) m_ShopDataEntry._Goods.y);
            if (_modelDataEntry == null)
            {
                return;
            }

            m_kImageIcon.sprite = UI_Helper.SetQualityBG(2, _modelDataEntry._Quality);

            m_kTextDesc.text = UI_Helper.GetTextByLanguageID(_modelDataEntry._Desc);
            m_kTextOCCUPYNumber.text = _modelDataEntry._Capacity.ToString();

            var inOutPut = DBManager.Instance.m_kInOutPut.GetEntryPtr(_modelDataEntry._InOutPutID);
            if (inOutPut == null)
            {
                return;
            }
            PlayerBagAsset.BagItem bagItem = player.SelectItem(_modelDataEntry._ID);
            if (bagItem == null)
            {
                m_kTextAmountNumber.text = "0";
            }
            else
            {
                m_kTextAmountNumber.text = bagItem.m_kCount.ToString();
            }

            List<Vector3> youthbnumberList = GetVector3Ints(inOutPut._YoungOut);
            for (int i = 0; i < youthbnumberList.Count; i++)
            {
                if (youthbnumberList[i].x == 1)
                {
                    m_kTextyouthbnumber.text = youthbnumberList[i].z.ToString();
                    break;
                }
            }

            List<Vector3> grownNumberList = GetVector3Ints(inOutPut._MatureOut);
            for (int i = 0; i < grownNumberList.Count; i++)
            {
                if (grownNumberList[i].x == 1)
                {
                    m_kTextgrownNumber.text = grownNumberList[i].z.ToString();
                    break;
                }
            }
        }
        else
        {
            m_kTextOCCUPYtitle.gameObject.SetActive(false);
            m_kTextOCCUPYNumber.gameObject.SetActive(false);
            m_kRectTransformHourly.gameObject.SetActive(false);
            CS_Items.DataEntry item = DBManager.Instance.m_kItems.GetEntryPtr((int) m_ShopDataEntry._Goods.y);
            if (item == null)
            {
                return;
            }

            m_kImageIcon.sprite = UI_Helper.SetQualityBG(2, item._Quality);

            m_kTextDesc.text = UI_Helper.GetTextByLanguageID(item._Desc);
            PlayerBagAsset.BagItem bagItem = player.SelectItem(item._ID);
            if (bagItem == null)
            {
                m_kTextAmountNumber.text = "0";
            }
            else
            {
                m_kTextAmountNumber.text = bagItem.m_kCount.ToString();
            }

        }

        m_kTextGoldBuyNumber.text = (currentNumber * m_ShopDataEntry._GoldPrice.z).ToString();
        m_kTextStoneNumber.text = (currentNumber * m_ShopDataEntry._StonePrice.z).ToString();
    }

    private List<Vector3> GetVector3Ints(string v3List)
    {
        List<Vector3> Vector3IntList = new List<Vector3>();
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
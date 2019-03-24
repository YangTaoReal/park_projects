using QTFramework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ObjectEventSystem]
public class UIPage_ShopComponentAwakeSystem : AAwake<UIPage_ShopComponent>
{
    public override void Awake(UIPage_ShopComponent _self)
    {
        _self.Awake();
    }
}

public enum ShopCategory
{
    All = 0,
    Animal,
    Botany,
    Nutrients,
    StageProperty,
    Building,
    Gift
}

[UIEntityComponent(UI_PrefabPath.m_sUIPage_Shop)]
public class UIPage_ShopComponent : UIComponent
{

    public Text m_kTextTitle;
    public Toggle m_kToggleGift;
    public Toggle m_kToggleAnimal;
    public Toggle m_kToggleBotany;
    public Toggle m_kToggleNutrients;
    public Toggle m_kToggleStageProperty;

    public Image m_kImageGift;
    public Text m_kTextGift;

    public Image m_kImageAnimal;
    public Text m_kTextAnimal;

    public Image m_kImageBotany;
    public Text m_kTextBotany;

    public Image m_kImageNutrients;
    public Text m_kTextNutrients;

    public Image m_kImageStageProperty;
    public Text m_kTextStageProperty;

    public RectTransform m_kRectTransformContent;
    public Button m_kButtonClose;
    public ScrollRect m_kScrollRect;
    private ShopCategory m_kShopCategory;

    internal void Awake()
    {
        m_kTextTitle = m_kParentEntity.m_kUIPrefab.GetCacheComponent(0) as Text;
        m_kToggleGift = m_kParentEntity.m_kUIPrefab.GetCacheComponent(1) as Toggle;
        m_kToggleAnimal = m_kParentEntity.m_kUIPrefab.GetCacheComponent(2) as Toggle;
        m_kToggleBotany = m_kParentEntity.m_kUIPrefab.GetCacheComponent(3) as Toggle;
        m_kToggleNutrients = m_kParentEntity.m_kUIPrefab.GetCacheComponent(4) as Toggle;
        m_kToggleStageProperty = m_kParentEntity.m_kUIPrefab.GetCacheComponent(5) as Toggle;

        m_kImageGift = m_kParentEntity.m_kUIPrefab.GetCacheComponent(6) as Image;
        m_kTextGift = m_kParentEntity.m_kUIPrefab.GetCacheComponent(7) as Text;

        m_kImageAnimal = m_kParentEntity.m_kUIPrefab.GetCacheComponent(8) as Image;
        m_kTextAnimal = m_kParentEntity.m_kUIPrefab.GetCacheComponent(9) as Text;

        m_kImageBotany = m_kParentEntity.m_kUIPrefab.GetCacheComponent(10) as Image;
        m_kTextBotany = m_kParentEntity.m_kUIPrefab.GetCacheComponent(11) as Text;

        m_kImageNutrients = m_kParentEntity.m_kUIPrefab.GetCacheComponent(12) as Image;
        m_kTextNutrients = m_kParentEntity.m_kUIPrefab.GetCacheComponent(13) as Text;

        m_kImageStageProperty = m_kParentEntity.m_kUIPrefab.GetCacheComponent(14) as Image;
        m_kTextStageProperty = m_kParentEntity.m_kUIPrefab.GetCacheComponent(15) as Text;

        m_kRectTransformContent = m_kParentEntity.m_kUIPrefab.GetCacheComponent(16) as RectTransform;
        m_kButtonClose = m_kParentEntity.m_kUIPrefab.GetCacheComponent(17) as Button;
        m_kScrollRect = m_kParentEntity.m_kUIPrefab.GetCacheComponent(18) as ScrollRect;

        m_kToggleGift.isOn = true;

        m_kToggleGift.onValueChanged.AddListener(onValueChanged_Gift);
        m_kToggleAnimal.onValueChanged.AddListener(onValueChanged_Animal);
        m_kToggleBotany.onValueChanged.AddListener(onValueChanged_Botany);
        m_kToggleNutrients.onValueChanged.AddListener(onValueChanged_Nutrients);
        m_kToggleStageProperty.onValueChanged.AddListener(onValueChanged_StageProperty);

        m_kButtonClose.onClick.AddListener(OnButtonClick_Close);

        m_kImageGift.sprite = UI_Helper.GetSprite("all");
        m_kImageAnimal.sprite = UI_Helper.GetSprite("animal");
        m_kImageBotany.sprite = UI_Helper.GetSprite("plant");
        m_kImageNutrients.sprite = UI_Helper.GetSprite("food");
        m_kImageStageProperty.sprite = UI_Helper.GetSprite("act");
        m_kShopCategory = ShopCategory.Gift;
        InitShop();
    }
    public override void TranslateUI()
    {
        base.TranslateUI();
        m_kTextTitle.text = UI_Helper.GetTextByLanguageID(105); //商城
        m_kTextGift.text = UI_Helper.GetTextByLanguageID(252); //礼包
        m_kTextAnimal.text = UI_Helper.GetTextByLanguageID(103); //动物
        m_kTextBotany.text = UI_Helper.GetTextByLanguageID(101); //植物
        m_kTextNutrients.text = UI_Helper.GetTextByLanguageID(102); //饲料
        m_kTextStageProperty.text = UI_Helper.GetTextByLanguageID(104); //道具
    }
    public override void Dispose()
    {
        base.Dispose();
        m_kToggleGift.onValueChanged.RemoveListener(onValueChanged_Gift);
        m_kToggleAnimal.onValueChanged.RemoveListener(onValueChanged_Animal);
        m_kToggleBotany.onValueChanged.RemoveListener(onValueChanged_Botany);
        m_kToggleNutrients.onValueChanged.RemoveListener(onValueChanged_Nutrients);
        m_kToggleStageProperty.onValueChanged.RemoveListener(onValueChanged_StageProperty);

        m_kButtonClose.onClick.RemoveListener(OnButtonClick_Close);

    }
    private void OnButtonClick_Close()
    {
        World.Scene.GetComponent<UIManagerComponent>().Remove(UI_PrefabPath.m_sUIPage_Shop);
    }

    /// <summary>
    /// 道具
    /// </summary>
    /// <param name="arg0"></param>
    private void onValueChanged_StageProperty(bool arg0)
    {
        if (arg0)
        {
            m_kShopCategory = ShopCategory.StageProperty;
            InitShop();
        }

    }

    /// <summary>
    /// 养料
    /// </summary>
    /// <param name="arg0"></param>
    private void onValueChanged_Nutrients(bool arg0)
    {
        if (arg0)
        {
            m_kShopCategory = ShopCategory.Nutrients;
            InitShop();
        }
    }

    /// <summary>
    /// 植物
    /// </summary>
    /// <param name="arg0"></param>
    private void onValueChanged_Botany(bool arg0)
    {
        if (arg0)
        {
            m_kShopCategory = ShopCategory.Botany;
            InitShop();
        }
    }

    /// <summary>
    /// 动物
    /// </summary>
    /// <param name="arg0"></param>
    private void onValueChanged_Animal(bool arg0)
    {
        if (arg0)
        {
            m_kShopCategory = ShopCategory.Animal;
            InitShop();
        }
    }

    /// <summary>
    /// 所有
    /// </summary>
    /// <param name="arg0"></param>
    private void onValueChanged_Gift(bool arg0)
    {
        if (arg0)
        {
            m_kShopCategory = ShopCategory.Gift;
            InitShop();
        }
    }

    private struct ShopItem
    {
        public int ShopID;
        public bool Ulock;
        public int Sort;
    }
    private void InitShop()
    {
        GetEntity<UIEntity>().ClearChildren();
        m_kScrollRect.verticalNormalizedPosition = 1;
        // m_kRectTransformContent.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
        Player player = World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer;

        List<ShopItem> shopList = new List<ShopItem>();
             
        var ShopAssetList = player.m_kPlayerShopAsset.m_kShop.GetEnumerator();
        while (ShopAssetList.MoveNext())
        {
            CS_Shop.DataEntry shopDataEntry = DBManager.Instance.m_kShop.GetEntryPtr(ShopAssetList.Current.Key);
            ShopItem shopItem = new ShopItem();
            shopItem.ShopID = shopDataEntry._ID;
            shopItem.Sort = shopDataEntry._Sort;
            shopItem.Ulock = ShopAssetList.Current.Value;
            shopList.Add(shopItem);
        }
        shopList.Sort((x, y) => x.Sort.CompareTo(y.Sort));

        foreach (var item in shopList)
        { 
            if (item.Ulock)
            {
                CS_Shop.DataEntry shopDataEntry = DBManager.Instance.m_kShop.GetEntryPtr(item.ShopID);
                if (shopDataEntry == null)
                {
                    continue;
                }

                if  (shopDataEntry._Category == (int) m_kShopCategory&& shopDataEntry._Category != (int) ShopCategory.Building)
                {

                    UIEntity uIEntity = World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPage_Shop_Item);
                    m_kParentEntity.AddChildren(uIEntity);
                    UIPage_Shop_ItemComponent _shop_Item = uIEntity.GetComponent<UIPage_Shop_ItemComponent>();
                    uIEntity.m_kUIPrefab.gameObject.transform.SetParent(m_kRectTransformContent);
                    uIEntity.m_kUIPrefab.gameObject.transform.localPosition = Vector3.zero;
                    _shop_Item.InitItem(item.ShopID);
                }
            }
            else
            {
                //未解锁的
            }
        }
    }
}
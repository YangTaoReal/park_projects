using QTFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ObjectEventSystem]
public class UIPopUpWindow_TaskComponentAwakeSystem : AAwake<UIPopUpWindow_TaskComponent>
{
    public override void Awake(UIPopUpWindow_TaskComponent _self)
    {
        _self.Awake();
    }
}

[UIEntityComponent(UI_PrefabPath.m_sUIPopUpWindow_Task)]
public class UIPopUpWindow_TaskComponent : UIComponent
{
    public Button m_ButtonClose;
    public Text m_TextTitle;
    public ScrollRect m_ScrollRectItemNode;
    public Slider m_SliderTotalVitality;
    public Button m_ButtonLevel1;
    public Button m_ButtonLevel2;
    public Button m_ButtonLevel3;
    public Button m_ButtonLevel4;
    public Button m_ButtonLevel5;
    public Text m_TextLevel1;
    public Text m_TextLevel2;
    public Text m_TextLevel3;
    public Text m_TextLevel4;
    public Text m_TextLevel5;
    public RectTransform m_RectTransformTips;
    public RawImage m_RawImageGoodsIcon;
    public Text m_TextGoldNumber;
    public Text m_TextGoodsNumber;
    public Text m_TextSoneNumber;
    public Text m_TextActivity;

    public RectTransform m_RectTransformRewad1;
    public RectTransform m_RectTransformRewad2;
    public RectTransform m_RectTransformRewad3;
    public Button m_ButtonTips;
    internal void Awake()
    {
        m_ButtonClose = m_kParentEntity.m_kUIPrefab.GetCacheComponent(0) as Button;
        m_TextTitle = m_kParentEntity.m_kUIPrefab.GetCacheComponent(1) as Text;
        m_ScrollRectItemNode = m_kParentEntity.m_kUIPrefab.GetCacheComponent(2) as ScrollRect;
        m_SliderTotalVitality = m_kParentEntity.m_kUIPrefab.GetCacheComponent(3) as Slider;
        m_ButtonLevel1 = m_kParentEntity.m_kUIPrefab.GetCacheComponent(4) as Button;
        m_ButtonLevel2 = m_kParentEntity.m_kUIPrefab.GetCacheComponent(5) as Button;
        m_ButtonLevel3 = m_kParentEntity.m_kUIPrefab.GetCacheComponent(6) as Button;
        m_ButtonLevel4 = m_kParentEntity.m_kUIPrefab.GetCacheComponent(7) as Button;
        m_ButtonLevel5 = m_kParentEntity.m_kUIPrefab.GetCacheComponent(8) as Button;
        m_TextLevel1 = m_kParentEntity.m_kUIPrefab.GetCacheComponent(9) as Text;
        m_TextLevel2 = m_kParentEntity.m_kUIPrefab.GetCacheComponent(10) as Text;
        m_TextLevel3 = m_kParentEntity.m_kUIPrefab.GetCacheComponent(11) as Text;
        m_TextLevel4 = m_kParentEntity.m_kUIPrefab.GetCacheComponent(12) as Text;
        m_TextLevel5 = m_kParentEntity.m_kUIPrefab.GetCacheComponent(13) as Text;
        m_RectTransformTips = m_kParentEntity.m_kUIPrefab.GetCacheComponent(14) as RectTransform;
        m_RawImageGoodsIcon = m_kParentEntity.m_kUIPrefab.GetCacheComponent(15) as RawImage;
        m_TextGoldNumber = m_kParentEntity.m_kUIPrefab.GetCacheComponent(16) as Text;
        m_TextGoodsNumber = m_kParentEntity.m_kUIPrefab.GetCacheComponent(17) as Text;
        m_TextSoneNumber = m_kParentEntity.m_kUIPrefab.GetCacheComponent(18) as Text;
        m_TextActivity = m_kParentEntity.m_kUIPrefab.GetCacheComponent(19) as Text;

        m_RectTransformRewad1 = m_kParentEntity.m_kUIPrefab.GetCacheComponent(20) as RectTransform;
        m_RectTransformRewad2 = m_kParentEntity.m_kUIPrefab.GetCacheComponent(21) as RectTransform;
        m_RectTransformRewad3 = m_kParentEntity.m_kUIPrefab.GetCacheComponent(22) as RectTransform;
        m_ButtonTips = m_kParentEntity.m_kUIPrefab.GetCacheComponent(23) as Button;
        m_ButtonClose.onClick.AddListener(onClick_Close);
        m_ButtonLevel1.onClick.AddListener(onClick_Level1);
        m_ButtonLevel2.onClick.AddListener(onClick_Level2);
        m_ButtonLevel3.onClick.AddListener(onClick_Level3);
        m_ButtonLevel4.onClick.AddListener(onClick_Level4);
        m_ButtonLevel5.onClick.AddListener(onClick_Level5);
        m_ButtonTips.onClick.AddListener(onClick_Tips);
        InitUI();

        ObserverHelper<bool>.AddEventListener(MessageMonitorType.RefreshTaskUI, RefreshTaskUI);

        World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.SetTaskNumber(TaskType.OnLine,(DateTime .Now - World.LoginTime).Seconds);
    }

    private void RefreshTaskUI(object sender, MessageArgs<bool> args)
    {
        SetTopUI();
    }

    private void onClick_Tips()
    {
        m_RectTransformTips.gameObject.SetActive(false);
    }

    private void onClick_Level5()
    {
        onClick_Level(100004, m_ButtonLevel5.transform.parent);
    }

private void onClick_Level4()
    {
        onClick_Level(100003, m_ButtonLevel4.transform.parent);
    }

    private void onClick_Level3()
    {
        onClick_Level(100002, m_ButtonLevel3.transform.parent);
    }

    private void onClick_Level2()
    {
        onClick_Level(100001, m_ButtonLevel2.transform.parent);
    }

    private void onClick_Level1()
    {
        onClick_Level(100000, m_ButtonLevel1.transform.parent);
    }

    private void onClick_Level(int _taskID,Transform _transform)
    {
        CS_Task.DataEntry taskLevel = DBManager.Instance.m_kTask.GetEntryPtr(_taskID);
        Player player = World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer;
        if (player.m_kPlayerBasicAsset.m_UIntVitality>= taskLevel._VitalityComplete)
        {
            GetReward(_taskID);
        }
        else
        {
            InitTips(_taskID, _transform);
        }
    }
    private void onClick_Close()
    {
        World.Scene.GetComponent<UIManagerComponent>().Remove(UI_PrefabPath.m_sUIPopUpWindow_Task);
    }

    public override void Dispose()
    {
        base.Dispose();

        m_ButtonClose.onClick.RemoveListener(onClick_Close);
        m_ButtonLevel1.onClick.RemoveListener(onClick_Level1);
        m_ButtonLevel2.onClick.RemoveListener(onClick_Level2);
        m_ButtonLevel3.onClick.RemoveListener(onClick_Level3);
        m_ButtonLevel4.onClick.RemoveListener(onClick_Level4);
        m_ButtonLevel5.onClick.RemoveListener(onClick_Level5);
        m_ButtonTips.onClick.RemoveListener(onClick_Tips);
    }

    public override void TranslateUI()
    {
        base.TranslateUI();
        m_TextTitle.text = UI_Helper.GetTextByLanguageID(220);
        //m_TextActivity.text = UI_Helper.GetTextByLanguageID(219);
    }
    private void SetTopUI()
    {
        m_RectTransformTips.gameObject.SetActive(false);

        CS_Task.DataEntry taskLevel1 = DBManager.Instance.m_kTask.GetEntryPtr(100000);
        CS_Task.DataEntry taskLevel2 = DBManager.Instance.m_kTask.GetEntryPtr(100001);
        CS_Task.DataEntry taskLevel3 = DBManager.Instance.m_kTask.GetEntryPtr(100002);
        CS_Task.DataEntry taskLevel4 = DBManager.Instance.m_kTask.GetEntryPtr(100003);
        CS_Task.DataEntry taskLevel5 = DBManager.Instance.m_kTask.GetEntryPtr(100004);

        int maxVitality = taskLevel5._VitalityComplete;
        Player player = World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer;
        m_SliderTotalVitality.value = player.m_kPlayerBasicAsset.m_UIntVitality / (float)maxVitality;

        m_TextLevel1.text = taskLevel1._VitalityComplete.ToString();
        m_TextLevel2.text = taskLevel2._VitalityComplete.ToString();
        m_TextLevel3.text = taskLevel3._VitalityComplete.ToString();
        m_TextLevel4.text = taskLevel4._VitalityComplete.ToString();
        m_TextLevel5.text = taskLevel5._VitalityComplete.ToString();

        m_TextActivity.text = UI_Helper.GetTextByLanguageID(219, player.m_kPlayerBasicAsset.m_UIntVitality.ToString());
        SetGiftButtonState(taskLevel1, m_ButtonLevel1);
        SetGiftButtonState(taskLevel2, m_ButtonLevel2);
        SetGiftButtonState(taskLevel3, m_ButtonLevel3);
        SetGiftButtonState(taskLevel4, m_ButtonLevel4);
        SetGiftButtonState(taskLevel5, m_ButtonLevel5);
    }

    private void SetGiftButtonState(CS_Task.DataEntry taskLevel,Button _button)
    {
        string sprit = "gift_02";
        if (taskLevel._ID == 100000)
        {
            sprit = "gift_01";
        }
        else if (taskLevel._ID ==  100001)
        {
            sprit = "gift_02";
        }
        else if (taskLevel._ID == 100002)
        {
            sprit = "gift_03";
        }
        else if (taskLevel._ID == 100003)
        {
            sprit = "gift_04";
        }
        else if (taskLevel._ID == 100004)
        {
            sprit = "gift_05";
        }
        Player player = World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer;
        if (player.m_kPlayerBasicAsset.m_UIntVitality >= taskLevel._VitalityComplete)
        {
            if (player.m_kPlayerBasicAsset.m_GetVitality.Contains(taskLevel._ID))
            {
                _button.GetComponent<Image>().sprite = UI_Helper.GetSprite(sprit+"_2");
            }
            else
            {
                _button.GetComponent<Image>().sprite = UI_Helper.GetSprite(sprit + "_0");
            }
        }
        else
        {
            _button.GetComponent<Image>().sprite = UI_Helper.GetSprite(sprit+"_3");
        }


    }
    private void InitUI()
    {
        SetTopUI();

        var taskMoveMent = DBManager.Instance.m_kTask.m_kDataEntryTable.GetEnumerator();
        while(taskMoveMent.MoveNext())
        {
            if (taskMoveMent.Current.Value._ID == 100000
                || taskMoveMent.Current.Value._ID == 100001
                || taskMoveMent.Current.Value._ID == 100002
                || taskMoveMent.Current.Value._ID == 100003
                || taskMoveMent.Current.Value._ID == 100004
                )
            {
                continue;
            }
            if (isCompleteVitality(taskMoveMent.Current.Value._ID))
            {
                continue;
            }
            UIEntity uIEntity = World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPopUpWindow_TaskItem);
            m_kParentEntity.AddChildren(uIEntity);
            UIPopUpWindow_TaskItemComponent _UIPopUpWindow_TaskItemComponent = uIEntity.GetComponent<UIPopUpWindow_TaskItemComponent>();
            uIEntity.m_kUIPrefab.gameObject.transform.SetParent(m_ScrollRectItemNode.content);
            uIEntity.m_kUIPrefab.gameObject.transform.localPosition = Vector3.zero;
            uIEntity.m_kUIPrefab.gameObject.transform.localPosition = Vector3.zero;
            _UIPopUpWindow_TaskItemComponent.Init(taskMoveMent.Current.Value._ID);
        }
    }
    private bool isCompleteVitality(int _taskID)
    {
        CS_Task.DataEntry taskLevel = DBManager.Instance.m_kTask.GetEntryPtr(_taskID);
        if (taskLevel == null)
        {
            return false;
        }
        Player _player = World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer;
        return _player.m_kPlayerBasicAsset.m_GetVitality.Contains(_taskID);
    }
    private void InitTips(int tips,Transform _prent)
    {
        m_RectTransformRewad1.gameObject.SetActive(false);
        m_RectTransformRewad2.gameObject.SetActive(false);
        m_RectTransformRewad3.gameObject.SetActive(false);

        m_RectTransformTips.gameObject.SetActive(true);

        m_RectTransformTips.transform.SetParent(_prent);
        m_RectTransformTips.transform.localEulerAngles = Vector3.zero;
        m_RectTransformTips.transform.localPosition = new Vector3(0,65,0);
        m_RectTransformTips.transform.localScale = Vector3.one;


        m_RectTransformTips.transform.SetParent(m_RectTransformTips.parent.parent);
        m_RectTransformTips.SetAsFirstSibling();
        CS_Task.DataEntry taskLevel1 = DBManager.Instance.m_kTask.GetEntryPtr(tips);
        if (taskLevel1 == null)
        {
            return;
        }

        string[] rewad = taskLevel1._Item.Split('|');
        foreach (var item in rewad)
        {
            string[] xyz = item.Split(' ');
            int x = Convert.ToInt32(xyz[0]);
            int y = Convert.ToInt32(xyz[1]);
            int z = Convert.ToInt32(xyz[2]);
            if (x == 1)
            {
                m_RectTransformRewad1.gameObject.SetActive(true);
                m_TextGoldNumber.text = z.ToString();
            }
            else if (x == 2)
            {
                m_RectTransformRewad2.gameObject.SetActive(true);
                m_TextSoneNumber.text = z.ToString();
            }
            else
            {

                CS_Items.DataEntry dataEntry = DBManager.Instance.m_kItems.GetEntryPtr(y);
                if (dataEntry != null)
                {
                    m_RawImageGoodsIcon.texture = UI_Helper.AllocTexture(dataEntry._Icon);
                }

                m_RectTransformRewad3.gameObject.SetActive(true);
                m_TextGoodsNumber.text = z.ToString();
            }
        }
    }


    private void GetReward(int _taskID)
    {
        Player player = World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer;
        CS_Task.DataEntry taskLevel = DBManager.Instance.m_kTask.GetEntryPtr(_taskID);
        if (taskLevel == null)
        {
            return;
        }

        string[] rewad = taskLevel._Item.Split('|');
        foreach (var item in rewad)
        {
            string[] xyz = item.Split(' ');
            int x = Convert.ToInt32(xyz[0]);
            int y = Convert.ToInt32(xyz[1]);
            int z = Convert.ToInt32(xyz[2]);
            if (x == 1)
            {
                player.AddGold(z);
            }
            else if (x == 2)
            {
                player.AddStone(z);
            }
            else
            {
                if (x == 6 || x == 5)
                {
                    player.AddItemToBag(y, (PlayerBagAsset.ItemType)x, z);
                }
            }
        }
        World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.GetVitalityReward(_taskID);
        SetTopUI();
    }
}

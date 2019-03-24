using System;
using System.Collections;
using System.Collections.Generic;

using QTFramework;

using UnityEngine;
using UnityEngine.UI;

[ObjectEventSystem]
public class UIPopUpWindow_TaskItemComponentAwakeSystem : AAwake<UIPopUpWindow_TaskItemComponent>
{
    public override void Awake(UIPopUpWindow_TaskItemComponent _self)
    {
        _self.Awake();
    }
}

[UIEntityComponent(UI_PrefabPath.m_sUIPopUpWindow_TaskItem)]
public class UIPopUpWindow_TaskItemComponent : UIComponent
{
    public RawImage m_RawImageIcon;
    public Text m_TextTitle;
    public Text m_TextRewad;

    public RectTransform m_RectTransformRewadN1;
    public RectTransform m_RectTransformRewadN2;
    public RectTransform m_RectTransformRewadN3;

    public RawImage m_RawImageGoodsIcon;
    public Text m_TextGoldNumber;
    public Text m_TextGoodsNumber;
    public Text m_TextStoneNumber;

    public RectTransform m_RectTransformState1;
    public RectTransform m_RectTransformState2;
    public RectTransform m_RectTransformState3;

    public Button m_kButtonGetRewad;
    public Button m_kButtonGoTo;
    public RectTransform m_RectTransformTitleBG;
    private int m_TaskID;
    public void Awake()
    {
        m_RawImageIcon = m_kParentEntity.m_kUIPrefab.GetCacheComponent(0)as RawImage;
        m_TextTitle = m_kParentEntity.m_kUIPrefab.GetCacheComponent(1)as Text;
        m_TextRewad = m_kParentEntity.m_kUIPrefab.GetCacheComponent(2)as Text;

        m_RectTransformRewadN1 = m_kParentEntity.m_kUIPrefab.GetCacheComponent(3)as RectTransform;
        m_RectTransformRewadN2 = m_kParentEntity.m_kUIPrefab.GetCacheComponent(4)as RectTransform;
        m_RectTransformRewadN3 = m_kParentEntity.m_kUIPrefab.GetCacheComponent(5)as RectTransform;

        m_RawImageGoodsIcon = m_kParentEntity.m_kUIPrefab.GetCacheComponent(6)as RawImage;
        m_TextGoldNumber = m_kParentEntity.m_kUIPrefab.GetCacheComponent(7)as Text;
        m_TextGoodsNumber = m_kParentEntity.m_kUIPrefab.GetCacheComponent(8)as Text;
        m_TextStoneNumber = m_kParentEntity.m_kUIPrefab.GetCacheComponent(9)as Text;

        m_RectTransformState1 = m_kParentEntity.m_kUIPrefab.GetCacheComponent(10)as RectTransform;
        m_RectTransformState2 = m_kParentEntity.m_kUIPrefab.GetCacheComponent(11)as RectTransform;
        m_RectTransformState3 = m_kParentEntity.m_kUIPrefab.GetCacheComponent(12)as RectTransform;

        m_kButtonGetRewad = m_kParentEntity.m_kUIPrefab.GetCacheComponent(13)as Button;
        m_kButtonGoTo = m_kParentEntity.m_kUIPrefab.GetCacheComponent(14)as Button;

        m_RectTransformTitleBG = m_kParentEntity.m_kUIPrefab.GetCacheComponent(15)as RectTransform;

        m_kButtonGetRewad.onClick.AddListener(onClick_GetRewad);
        m_kButtonGoTo.onClick.AddListener(onClick_GoTo);
    }

    private void onClick_GoTo()
    {
        CS_Task.DataEntry taskLevel = DBManager.Instance.m_kTask.GetEntryPtr(m_TaskID);
        if (taskLevel == null)
        {
            return;
        }

        switch ((TaskType)taskLevel._TaskType)
        {
        case TaskType.Login:
            break;
        case TaskType.ShovelShit:
            break;
        case TaskType.ClearTheRubbish:
            break;
        case TaskType.CollectResources:
            break;
        case TaskType.Item:
            break;
        case TaskType.Gold:
            World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPage_Shop);
            break;
        case TaskType.Share:
            break;
        case TaskType.Visit:
            break;
        case TaskType.BuyItem:
            World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPage_Shop);
            break;
        case TaskType.Feed:

            break;
        case TaskType.Stone:
            World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPage_Shop);

            break;
        }
        World.Scene.GetComponent<UIManagerComponent>().ClearUIStack();
    }

    private void onClick_GetRewad()
    {
        CS_Task.DataEntry taskLevel = DBManager.Instance.m_kTask.GetEntryPtr(m_TaskID);
        if (taskLevel == null)
        {
            return;
        }
        Player _player = World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer;
        string[] rewad = taskLevel._Item.Split('|');
        foreach (var item in rewad)
        {
            string[] xyz = item.Split(' ');
            int x = Convert.ToInt32(xyz[0]);
            int y = Convert.ToInt32(xyz[1]);
            int z = Convert.ToInt32(xyz[2]);
            if (x == 1)
            {
                _player.AddGold(z);
            }
            else if (x == 2)
            {
                _player.AddStone(z);
            }
            else
            {
                _player.AddItemToBag(y, (PlayerBagAsset.ItemType)x, z);
            }
        }

        World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.GetVitalityReward(m_TaskID);
        World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.AddVitality((uint)taskLevel._Vitality);
        Init(m_TaskID);

        ObserverHelper<bool>.SendMessage(MessageMonitorType.RefreshTaskUI, this, new MessageArgs<bool>(false));
    }

    public override void TranslateUI()
    {
        base.TranslateUI();
        m_TextRewad.text = UI_Helper.GetTextByLanguageID(228);
    }

    public override void Dispose()
    {
        base.Dispose();
    }
    public void Init(int _taskID)
    {
        m_TaskID = _taskID;
        m_RectTransformRewadN1.gameObject.SetActive(false);
        m_RectTransformRewadN2.gameObject.SetActive(false);
        m_RectTransformRewadN3.gameObject.SetActive(false);

        CS_Task.DataEntry taskLevel = DBManager.Instance.m_kTask.GetEntryPtr(_taskID);
        if (taskLevel == null)
        {
            return;
        }

        m_RawImageIcon.texture = UI_Helper.AllocTexture(taskLevel._Icon);
        m_TextTitle.text = UI_Helper.GetTextByLanguageID(taskLevel._Name);

        string[] rewad = taskLevel._Item.Split('|');
        foreach (var item in rewad)
        {
            string[] xyz = item.Split(' ');
            int x = Convert.ToInt32(xyz[0]);
            int y = Convert.ToInt32(xyz[1]);
            int z = Convert.ToInt32(xyz[2]);
            if (x == 1)
            {
                m_TextGoldNumber.text = z.ToString();
                m_RectTransformRewadN1.gameObject.SetActive(true);
            }
            else if (x == 2)
            {
                m_TextStoneNumber.text = z.ToString();
                m_RectTransformRewadN3.gameObject.SetActive(true);
            }
            else
            {
                CS_Items.DataEntry dataEntry = DBManager.Instance.m_kItems.GetEntryPtr(y);
                if (dataEntry != null)
                {
                    m_RawImageGoodsIcon.texture = UI_Helper.AllocTexture(dataEntry._Icon);
                }
                m_TextGoodsNumber.text = z.ToString();
                m_RectTransformRewadN2.gameObject.SetActive(true);
            }
        }
        Player _player = World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer;
        if (!_player.m_kPlayerBasicAsset.m_GetVitality.Contains(_taskID) && isCompleteVitality(_taskID))
        {
            m_RectTransformState1.gameObject.SetActive(false);
            m_RectTransformState2.gameObject.SetActive(true);
            m_RectTransformState3.gameObject.SetActive(false);
            m_RectTransformTitleBG.gameObject.SetActive(true);
        }
        else if (!isCompleteVitality(_taskID))
        {
            m_RectTransformState1.gameObject.SetActive(false);
            m_RectTransformState2.gameObject.SetActive(false);
            m_RectTransformState3.gameObject.SetActive(true);
            m_RectTransformTitleBG.gameObject.SetActive(false);
        }
        else
        {
            m_RectTransformTitleBG.gameObject.SetActive(true);
            m_RectTransformState1.gameObject.SetActive(true);
            m_RectTransformState2.gameObject.SetActive(false);
            m_RectTransformState3.gameObject.SetActive(false);
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

        int _vitalityNumber = _player.m_kPlayerBasicAsset.m_VitalityNumber[(TaskType)taskLevel._TaskType];
        return _vitalityNumber >= taskLevel._Number;
    }
}
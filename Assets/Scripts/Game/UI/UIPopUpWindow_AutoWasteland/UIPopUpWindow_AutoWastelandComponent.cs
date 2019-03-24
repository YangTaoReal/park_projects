using QTFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ObjectEventSystem]
public class UIPopUpWindow_AutoWastelandComponentAwakeSystem : AAwake<UIPopUpWindow_AutoWastelandComponent>
{
    public override void Awake(UIPopUpWindow_AutoWastelandComponent _self)
    {
        _self.Awake();
    }
}

[UIEntityComponent(UI_PrefabPath.m_sUIPopUpWindow_AutoWasteland)]
public class UIPopUpWindow_AutoWastelandComponent : UIComponent
{

    public Text m_kTextTitle;
    public Text m_kTextNumberTitle;
    public Text m_kTextNumber;
    public Text m_kTextDayTitle;
    public Text m_kTextDay;
    public Text m_kTextTips;

    public Button m_kButtonClose;
    public Button m_kButtonOk;
    public Text m_kTextOk;
    internal void Awake()
    {
        m_kTextTitle = m_kParentEntity.m_kUIPrefab.GetCacheComponent(0) as Text;
        m_kTextNumberTitle = m_kParentEntity.m_kUIPrefab.GetCacheComponent(1) as Text;
        m_kTextNumber = m_kParentEntity.m_kUIPrefab.GetCacheComponent(2) as Text;
        m_kTextDayTitle = m_kParentEntity.m_kUIPrefab.GetCacheComponent(3) as Text;
        m_kTextDay = m_kParentEntity.m_kUIPrefab.GetCacheComponent(4) as Text;
        m_kTextTips = m_kParentEntity.m_kUIPrefab.GetCacheComponent(5) as Text;

        m_kButtonClose = m_kParentEntity.m_kUIPrefab.GetCacheComponent(6) as Button;
        m_kButtonOk = m_kParentEntity.m_kUIPrefab.GetCacheComponent(7) as Button;

        m_kTextOk = m_kParentEntity.m_kUIPrefab.GetCacheComponent(8) as Text;
        m_kButtonClose.onClick.AddListener(OnButtonClick_Close);
        m_kButtonOk.onClick.AddListener(OnButtonClick_Ok);
        Init();
    }

    public override void TranslateUI()
    {
        base.TranslateUI();
        m_kTextTitle.text = UI_Helper.GetTextByLanguageID(149);
        m_kTextNumberTitle.text = UI_Helper.GetTextByLanguageID(150);
        m_kTextDayTitle.text = UI_Helper.GetTextByLanguageID(151);
        m_kTextTips.text = UI_Helper.GetTextByLanguageID(153);
        m_kTextOk.text = UI_Helper.GetTextByLanguageID(152);
    }
    private void OnButtonClick_Ok()
    {
        int lock_num = int.Parse(DBManager.Instance.m_kDisperse.GetEntryPtr(20002)._Val1);
        if (SceneLogic._instance.listWasteland.Count >= lock_num)
        {
            UI_Helper.ShowCommonTips(70001);
            return;
        }


        //for (int i = 0; i < MapGridMgr.Instance.WastedBoards.Count; i++)
        //{
        //    MapGridMgr.Instance.WastedBoards[i].typReclaim = 1;
        //}
        //MapGridMgr.Instance.HideShowAllBoards(false);
        //SceneLogic._instance.RemoveAllSelectWastedLand();
        MapGridMgr.Instance.FreeingWastedland();
        World.Scene.GetComponent<UIManagerComponent>().RemoveAll(UI_PrefabPath.m_sUISystem_Hall);
    }

    private void OnButtonClick_Close()
    {
        World.Scene.GetComponent<UIManagerComponent>().Remove(UI_PrefabPath.m_sUIPopUpWindow_AutoWasteland);
    }

    public override void Dispose()
    {
        //for (int i = 0; i < SceneLogic._instance.listWasteland.Count; i++)
        //{
        //    SceneLogic._instance.listWasteland[i].rootBuild.GetComponent<Building>().IsPause = false;
        //}
        m_kButtonClose.onClick.RemoveListener(OnButtonClick_Close);
        m_kButtonOk.onClick.RemoveListener(OnButtonClick_Ok);
    }


    public void Init()
    {

        int times = 0;
        List<SceneLogic.WastedBoard> templist = new List<SceneLogic.WastedBoard>();
        for (int i = 0; i < SceneLogic._instance.listSelectWBoard.Count; i++)
        {
            bool isHave = false;
            var selectWB = SceneLogic._instance.listSelectWBoard[i];
            for (int j = 0; j < SceneLogic._instance.listWasteland.Count; j++)
            {
                var WL = SceneLogic._instance.listWasteland[j];
                if(selectWB.WastedGuid == WL.rootBuild.guid)
                {
                    isHave = true;
                    break;
                }
            }    
            if (!isHave) templist.Add(selectWB);
        }
        int count = templist.Count;
        for (int i = 0; i < count; i++)
        {
            Guid guid = templist[i].WastedGuid;
            foreach (var tile in MapGridMgr.Instance._selectedWastedlandTile)
            {
                if(tile.Key.guid == guid)
                {
                    times += DBManager.Instance.m_kModel.GetEntryPtr(tile.Key.prefabId)._BuildTime;
                    break;
                }
            }
        }

   
        m_kTextDay.text = times + "s";
        m_kTextNumber.text = UI_Helper.GetTextByLanguageID(154, count.ToString());
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using BE;
using PixelCrushers.DialogueSystem;
using QTFramework;
using UnityEngine;
using UnityEngine.Events;

[ObjectEventSystem]
public class DialogueManagerComponentAwakeSystem : AAwake<DialogueManagerComponent>
{
    public override void Awake(DialogueManagerComponent _self)
    {
        _self.Awake();
    }
}


public class DialogueManagerComponent : QTComponent 
{



    public static DialogueManagerComponent _Instance;

    private Transform canvasRootTR;
    private GameObject barkUI;

   

    BaseData plane = null;
    BaseData swimPlayer = null;
    UIEntity bubbltEntity = null;

    public int lastConversationID = -1;
    public int CurrentConversationID = -1;

    private bool isDialoguing = false;   // 是否正在对话中 以此来判断是否重连对话
    public bool IsDialoguing
    {
        get { return isDialoguing; }
        set
        {
            isDialoguing = value;
            PlayerPrefs.SetInt("isDialoguing", isDialoguing == true ? 1 : 0);
        }
    }

    private ConversationClip currConversationClip;  // 当前正在进行哪个对话
    public ConversationClip CurrConversationClip
    {
        get { return currConversationClip; }
        set{
            currConversationClip = value;
            PlayerPrefs.SetInt("currConversationClip", (int)currConversationClip);
        }
    }

    public Transform CanvasRootTR
    {
        get { if (canvasRootTR == null) 
            canvasRootTR = DialogueManager.instance.transform.Find("Canvas").transform;
                return canvasRootTR;
        }
    }

    public void Awake()
    {
        _Instance = this;
        InitData();
    }

    /// <summary>
    /// 初始化一些对话系统的设置
    /// </summary>
    private void InitData()
    {
        InitActorsPortraitTexture();
    }

    private void InitActorsPortraitTexture()
    {
        var actors = DialogueManager.instance.initialDatabase.actors;
        for (int i = 0; i < actors.Count; i++)
        {
            int id = 50000 + actors[i].id;
            if (id == 50001) continue;  // 玩家自己现在还没有形象
            //Debug.LogError($"当前的id：{id}, 图片路径:{DBManager.Instance.m_kModel.GetEntryPtr(id)._Icon}");
            actors[i].portrait = UI_Helper.AllocTexture(DBManager.Instance.m_kModel.GetEntryPtr(id)._Icon) as Texture2D;
        }
    }
    /// <summary>
    /// 展示坠机场景
    /// </summary>
    public void ShowAirPlaneScene()
    {
        //Debug.LogError("____***&*&**&*&*&*&*&");
        Debug.Log("展示坠机场景");
        Vector3 planePos = Vector3.zero;
        Vector3 swimPlayerPos = Vector3.zero;

        CS_MapInfo.DataEntry dataEntry = DBManager.Instance.m_kMapInfo.GetEntryPtr(1000000);
        if (dataEntry != null)
        {
            string[] Arguments = dataEntry._Arguments.Split(';');
            for (int i = 0; i < Arguments.Length; i++)
            {
                string[] oneArg = Arguments[i].Split('=');
                if(oneArg[0].Equals("PlaneID"))
                {
                    int id = Convert.ToInt32(oneArg[1]);
                    plane = ModelManager._instance.Load(id, World.Scene.GetComponent<WorldManagerComponent>().m_kActorNode.transform);
                }
                if(oneArg[0].Equals("SwimPlayerID"))
                {
                    int id = Convert.ToInt32(oneArg[1]);
                    swimPlayer = ModelManager._instance.Load(id, World.Scene.GetComponent<WorldManagerComponent>().m_kActorNode.transform);
                    //Debug.LogError("____@@@@___" + swimPlayer.guid);
                }
                if(oneArg[0].Equals("PlanePos"))
                {
                    string[] values = oneArg[1].Split('|');
                    planePos = new Vector3(Convert.ToSingle(values[0]), Convert.ToSingle(values[1]), Convert.ToSingle(values[2]));
                }
                if (oneArg[0].Equals("SwimPlayerPos"))
                {
                    string[] values = oneArg[1].Split('|');
                    swimPlayerPos = new Vector3(Convert.ToSingle(values[0]), Convert.ToSingle(values[1]), Convert.ToSingle(values[2]));
                }
            }
            if (plane != null)
                plane.go.transform.position = planePos;
            if (swimPlayer != null)
            {
                //Debug.LogError("___######___" + swimPlayer.guid);
                swimPlayer.go.transform.position = swimPlayerPos;
                bubbltEntity = UI_Helper.AddBubleTextNode(swimPlayer.go.transform, 1301, () =>
                {
                    Debug.Log("点击了救人按钮,救下了他");
                    World.Scene.GetComponent<UIManagerComponent>().Remove(UI_PrefabPath.m_sUIPopUpWindow_Guidance_Click);
                    BE.MobileRTSCam.instance.SmoothMoveCamera(0.5f, ModelManager._instance.GetModleByType(ModelCType.MainBase)[0].go.transform.position, () => {

                        World.Scene.GetComponent<DialogueManagerComponent>().StartConversation(ConversationClip.Investment);
                        //Debug.LogError("__********___" + swimPlayer.guid);
                        ModelManager._instance.RecycleByGuid(swimPlayer.guid);
                        ModelManager._instance.RecycleByGuid(plane.guid);
                        bubbltEntity.Dispose(); 
                        bubbltEntity = null;
                    });
                });

                BE.MobileRTSCam.instance.SmoothMoveCamera(0.5f, swimPlayer.go.transform.position,()=>{

                    TimerUtil.SetTimeOut(0.1f, () => { 
                    
                        //UI_Helper.ShowGuidance_Click()
                        CS_Guidance.DataEntry data = new CS_Guidance.DataEntry();
                        data._isUI = true;
                        data._isMask = true;
                        data._Shape = 1;
                        data._isShowFinger = true;
                        UI_Helper.ShowGuidance_Click(bubbltEntity.GetComponent<BubbleTextButton>().shapeImg.transform, data);
                        World.Scene.GetComponent<UIManagerComponent>().Get(UI_PrefabPath.m_sUIPopUpWindow_Guidance_Click).GetComponent<UI_Guidance_Click>().SetMaskAlpha(0f);
                    });
                   
                });

            }
            
        }
    }

    public void StartReportEvent()
    {
       
        if(!DialogueManager.instance.initialDatabase.GetVariable("hasReport").InitialBoolValue)
        {
            //Debug.Log($"动植物数量{int.Parse(DBManager.Instance.m_kDisperse.GetEntryPtr(20006)._Val1)}");
            if (DialogueManager.instance.initialDatabase.GetVariable("hasReport").InitialBoolValue)
            {
                return;
            }

            ResetGameUIPanel();
            MobileRTSCam.instance.SmoothMoveCamera(0.1f, ModelManager._instance.GetModleByType(ModelCType.MainBase)[0].go.transform.position, () => {
                MobileRTSCam.instance.SetCameraZoom(35f);
                Debug.Log($"动植物数量超过{int.Parse(DBManager.Instance.m_kDisperse.GetEntryPtr(20006)._Val1)}，现在触发记者事件");
                StartConversation(ConversationClip.Reporter);
            });

        }
    }

    public void StartInvestment()
    {
        Debug.Log("现在引导投资人事件");
        TimerUtil.SetTimeOut(int.Parse(DBManager.Instance.m_kDisperse.GetEntryPtr(20009)._Val1), () =>
        {
            ResetGameUIPanel();
            CS_Guidance.DataEntry entryData = new CS_Guidance.DataEntry();
            entryData._isUI = true;
            entryData._isMask = true;
            entryData._isShowFinger = true;
            entryData._Shape = 0;

            Transform tr = World.Scene.GetComponent<UIManagerComponent>().Get(UI_PrefabPath.m_sUISystem_Hall).GetComponent<UISystem_HallComponent>().m_kButton_Event.transform;
            UI_Helper.ShowGuidance_Click(tr, entryData);
        });
    }

    /// <summary>
    /// 开始事件的时候  关闭所有界面  摄像机聚焦到小木屋
    /// </summary>
    public void ResetGameUIPanel()
    {
        World.Scene.GetComponent<UIManagerComponent>().ClearUIStack();
        World.Scene.GetComponent<UIManagerComponent>().RemoveAll(UI_PrefabPath.m_sUISystem_Hall);
        MapGridMgr.Instance.EndFreeingWastedland();
        MapGridMgr.Instance.UnFoucs();
    }

    public void TimerDownToStartEventHelper()
    {
        int time = int.Parse(DBManager.Instance.m_kDisperse.GetEntryPtr(20007)._Val1);
        TimerUtil.SetTimeOut(time, () =>
        {
            //Debug.Log("开始开荒事件对话系统");
            if (DialogueManager.instance.initialDatabase.GetVariable("hasEventHelper").InitialBoolValue == false)
            {
                ResetGameUIPanel();
                BE.MobileRTSCam.instance.SmoothMoveCamera(0.1f, ModelManager._instance.GetModleByType(ModelCType.MainBase)[0].go.transform.position, () => {
                    MobileRTSCam.instance.SetCameraZoom(35f);
                    StartConversation(ConversationClip.EventHelper);
                });
            }

        });
    }

    /// <summary>
    /// 完成助手事件  成功解锁
    /// </summary>
    public void HelperUnlock()
    {
        UIEntity entity = World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPage_HelperUnlock);
        entity.GetComponent<HelperUnlock>().OnAnimationCallBack.SetCallBack(OnAnimationCallBack.CallBackType.OnAnimationEnd, () =>
        {
            Debug.Log("这里是解锁后的回调");
            World.Scene.GetComponent<UIManagerComponent>().Remove(UI_PrefabPath.m_sUIPage_HelperUnlock);
            DialogueManager.instance.initialDatabase.GetVariable("hasEventHelper").InitialBoolValue = true;
            TimerUtil.SetTimeOut(0.5f, () =>
            {
                UI_Helper.ShowCommonTips(1303);
            });
        });
    }

    /// <summary>
    /// 记者事件完成的回调
    /// </summary>
    public void ReportEventOver()
    {
        Debug.Log("记者事件结束");
        DialogueManager.instance.initialDatabase.GetVariable("hasReport").InitialBoolValue = true;
        TimerUtil.SetTimeOut(0.2f,()=>{
            
            UI_Helper.ShowCommonTips(1302);
        });
    }

	/// <summary>
    /// 展示协议面板
    /// </summary>
    public void  ShowInvestPanel()
    {
        Debug.Log("在这里展示协议面板");
        World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPage_Privacy);
    }

    public void ResetDialogueDataForFirstOpenGame()
    {
        var variables = DialogueManager.instance.initialDatabase.variables;
        for (int i = 0; i < variables.Count; i++)
        {
            variables[i].InitialBoolValue = false;
        }

        var player = DialogueManager.instance.initialDatabase.GetActor(1);
        var Investor = DialogueManager.instance.initialDatabase.GetActor(2);

        player.Name = "我";
        player.fields[4].value = "Myself";
        player.fields[5].value = "Myself";

        Investor.Name = "落水者";
        Investor.fields[4].value = "Drowner";
        Investor.fields[5].value = "Drowner";
    }

    public void ResetDialogueDataForInvestor()
    {
        var player = DialogueManager.instance.initialDatabase.GetActor(1);
        var Investor = DialogueManager.instance.initialDatabase.GetActor(2);

        player.Name = "我";
        player.fields[4].value = "Myself";
        player.fields[5].value = "Myself";

        Investor.Name = "落水者";
        Investor.fields[4].value = "Drowner";
        Investor.fields[5].value = "Drowner";
    }

    /// <summary>
    /// 获得投资人投资的金币
    /// </summary>
    public void GetInvestMoney()
    {
      
        UIEntity entity = null;
        entity = UI_Helper.MoneyShowAnim(null, () =>
        {
            //Debug.Log("金钱奖励动画播放完毕，这是回调");
            DialogueManager.instance.initialDatabase.GetVariable("hasGotInvest").InitialBoolValue = true;
            Debug.Log($"金钱奖励动画播放完毕，这是回调，hasGotInvest = {DialogueManager.instance.initialDatabase.GetVariable("hasGotInvest").InitialBoolValue}");
            entity.Dispose();
            // 加钱
            Debug.Log($"加钱: {DBManager.Instance.m_kDisperse.GetEntryPtr(20010)._Val1}");
            World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.AddGold(decimal.Parse(DBManager.Instance.m_kDisperse.GetEntryPtr(20010)._Val1));
        });
        //DialogueLua.SetVariable("hasGotInvest", true);
        //Debug.Log($"在这里引导获得金币，hasGotInvest = { DialogueLua.GetVariable("hasGotInvest").asBool}");
    }


    public void StartConversation(ConversationClip clip)
    {
        // 在开始每次对话前 先设置语言
        World.SetLanguage(QTLanguage.Chinese);
        switch(World.m_kLanguage)
        {
            case QTLanguage.Arabic:
                DialogueManager.SetLanguage("ar");
                break;
            case QTLanguage.English:
                DialogueManager.SetLanguage("en");
                break;
            case QTLanguage.Chinese:
                DialogueManager.SetLanguage("");    // 这里中文是默认的  直接传空字符就行
                break;
            default:
                DialogueManager.SetLanguage("en");
                break;
        }
        CurrConversationClip = clip;
        IsDialoguing = true;
        DialogueManager.StartConversation(clip.ToString());
    }

}

public enum ConversationClip
{
    Investment = 1,
    EventHelper = 2,
    BiologicalExperts = 3,
    Reporter = 4,
}

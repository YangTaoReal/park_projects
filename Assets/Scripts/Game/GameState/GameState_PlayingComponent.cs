using System;
using System.Collections;
using System.Collections.Generic;

using PixelCrushers.DialogueSystem;

using QTFramework;

using UnityEngine;

public class GameState_PlayingComponent : StateBase
{
    public override void Init()
    {
        base.Init();
        Log.Info("LC_GameState_Play", "初始化");
    }

    public override void StateStart(StateMachineBase kStateMachine)
    {
        base.StateStart(kStateMachine);
        World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUISystem_Hall);
        World.Scene.GetComponent<AudioManagerComponent>().ChangeAudio(AudioChannel.AudioChannelType.BGM, DBManager.Instance.m_kAudio.GetEntryPtr(100001)._AudioPath, true);
        Weather.Instance.enabled = true;
        //return;
        // 第一次打开游戏 视为创建账号阶段 整个游戏中只触发一次   
        if (PlayerPrefs.GetInt("isFirstOpenGame", 1) == 1)
        {
            FirstOpenGame();
        }

        JudgeReconnection();
        GameEventManager._Instance.onLogInGame();   // 检测登录游戏
    }

    // 第一次登陆游戏 策划需求 将这一步看做注册账号的一步
    public void FirstOpenGame()
    {
        // 第一次打开游戏 新手引导 放在地图加载之前 否则出错
        PlayerPrefs.SetInt("isFirstOpenGame", 0);
        //World.Scene.GetComponent<GuidanceManager>().StartGuidance(GuidanceEvent.GreenHandEvent);      // 新手引导
        // 第一次打开游戏 在这里将对话事件的bool值赋值 目的防止之前在电脑上测试后值被改了 打包手机发现无法触发对话事件
        DialogueManagerComponent._Instance.ResetDialogueDataForFirstOpenGame();
        // 第一次打开游戏 默认为注册了游戏  只响应一次
        if (GameEventManager._Instance.onResgister != null) GameEventManager._Instance.onResgister();
        // 记录下创建账号的时间
        World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.m_kPlayerBasicAsset.m_CreateAccountTime = DateTime.Now;
    }

    // 判断重连
    public void JudgeReconnection()
    {
        Debug.Log($"新手引导状态,currGuidanceEvent:{PlayerPrefs.GetInt("currGuidanceEvent")},{GuidanceEvent.GreenHandEvent.ToString()}是否完成：{PlayerPrefs.GetInt(GuidanceEvent.GreenHandEvent.ToString(), 0)}");
        if (PlayerPrefs.GetInt("currGuidanceEvent", 100) == 100 && PlayerPrefs.GetInt(GuidanceEvent.GreenHandEvent.ToString(), 0) == 0)
        {
            // 新手引导重连
            //Debug.Log($"新手引导重连");
            // 更改了游戏操作方式 新手引导机制会出问题 现在关闭新手引导
            PlayerPrefs.SetInt("isGuidancing", 0);
            PlayerPrefs.SetInt("currGuidanceStep", 1);
            PlayerPrefs.SetInt(GuidanceEvent.GreenHandEvent.ToString(), 1);
            //World.Scene.GetComponent<GuidanceManager>().StartGuidance(GuidanceEvent.GreenHandEvent);
        }
        else if(PlayerPrefs.GetInt(GuidanceEvent.GreenHandEvent.ToString(), 0) == 1 && PlayerPrefs.GetInt("isDialoguing") == 1) // 正在对话
        {
            switch((ConversationClip)PlayerPrefs.GetInt("currConversationClip"))
            {
                case ConversationClip.Investment:
                        if(DialogueManager.instance.initialDatabase.GetVariable("hasUserName").InitialBoolValue == false)
                            DialogueManagerComponent._Instance.ResetDialogueDataForInvestor();
                        DialogueManagerComponent._Instance.StartConversation(ConversationClip.Investment);
                    break;
                case ConversationClip.Reporter:
                    DialogueManagerComponent._Instance.StartConversation(ConversationClip.Reporter);
                    break;
                case ConversationClip.EventHelper:
                    DialogueManagerComponent._Instance.StartConversation(ConversationClip.EventHelper);
                    break;
            }
        }
        //else if (PlayerPrefs.GetInt(GuidanceEvent.GreenHandEvent.ToString(), 0) == 1 && DialogueManager.instance.initialDatabase.GetVariable("hasGotInvest").InitialBoolValue == false)
        //{
        //    Debug.Log("新手引导完毕，但是投资人事件还没有获得金币奖励，现在打开对话框");
        //    if(DialogueManager.instance.initialDatabase.GetVariable("hasUserName").InitialBoolValue == false)
        //        DialogueManagerComponent._Instance.ResetDialogueDataForInvestor();
        //    DialogueManagerComponent._Instance.StartConversation(ConversationClip.Investment);

        //}
        //else if (DialogueManager.instance.initialDatabase.GetVariable("hasReport").InitialBoolValue && !DialogueManager.instance.initialDatabase.GetVariable("hasEventHelper").InitialBoolValue)
        //{
        //    Debug.Log("记者事件已经触发了，但是助手事件没有触发，在这里自动触发");
        //    DialogueManagerComponent._Instance.StartConversation(ConversationClip.EventHelper);
        //}
        if (PlayerPrefs.GetInt("currGuidanceEvent", 100) == 100 && PlayerPrefs.GetInt(GuidanceEvent.GreenHandEvent.ToString(), 0) == 1)
        {
            PlayerPrefs.GetInt("isGuidancing", 0);  // 新手引导如果已经完成  改成false
        }
    }

    private bool CanEdit = false;
    private Vector3 StartP = Vector3.zero;
    public override StateProcessResult StateProcess(StateMachineBase kStateMachine)
    {
        //if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        //{

        //    if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
        //    {
        //        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
        //        {
        //            return StateProcessResult.Hold;
        //        }

        //    }

        //}
        //else
        //{
        //    if (Input.GetMouseButtonDown(0) && UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        //    {
        //        return StateProcessResult.Hold;
        //    }
        //}

        //if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        //{

        //    if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
        //    {
        //        if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
        //        {
        //            StartP = Input.GetTouch(0).position;
        //        }          
        //    }
        //}
        //else
        //{
        //    if (Input.GetMouseButtonDown(0))
        //    {
        //        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        //        {
        //            StartP = Vector3.one;
        //            Debug.Log("UI了");
        //        }
        //        else
        //        {
        //            Debug.Log("开始：" + Input.mousePosition);
        //            StartP = Input.mousePosition;
        //        }       
        //    }          
        //}

        //if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        //{

        //    if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended)
        //    {
        //            //Debug.Log("距离："+ Vector3.Distance(StartP, Input.GetTouch(0).position));
        //            CanEdit=Vector3.Distance(StartP, Input.GetTouch(0).position) < 1;
        //    }
        //}
        //else
        //{
        //    if (Input.GetMouseButtonUp(0) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        //    {
        //        Debug.Log(StartP+"结束：" + Input.mousePosition);
        //        Debug.Log("距离：" + Vector3.Distance(StartP, Input.mousePosition));
        //        CanEdit = StartP==Vector3.zero?true: Vector3.Distance(StartP, Input.mousePosition) < 1;
        //    }
        //}

        return StateProcessResult.Hold;
    }

    public override void StateEnd(StateMachineBase kStateMachine)
    {
        base.StateEnd(kStateMachine);
    }
}
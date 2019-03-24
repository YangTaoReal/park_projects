using System.Collections.Generic;
using UnityEngine;
using System;
 
using DG.Tweening;
using BE;

namespace QTFramework
{
 
 
    enum StateAssistant
    {
        idle = 1,
        work = 2,
        nopower = 3,//没有体力了
        rest = 4,//休息
    }

    public class Assistant : Actor
    {

        UIBar uiBar;
        Tween tweenMove;
        Tween tweenAttack;
        //GameObject house;
        Vector3 posHouse;
        StateAssistant state;
        SceneLogic.Dispose nowDispose;

        float yetWorkTime;//已经工作的时间
        float yetTiredTime;//已经疲劳的时间
        float yetRestTime;//已经休息的时间

        float allWork;
        float allRest;
        float WasteMoveTime;

        DateTime beginRest;
        UI_AssistantInfo uiInfo;
        // Use this for initialization
        void Awake()
        {
            yetWorkTime = 0;
            yetRestTime = 0;
            yetTiredTime = 0;
        }

   
 
        void LateUpdate()
        {
            if (uiBar == null) return;
            Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
            uiBar.FixPos(screenPos, BarType.assistant);
            if(state == StateAssistant.rest)
            {
                yetRestTime = (int)DateTime.Now.Subtract(beginRest).TotalSeconds;
                if(yetRestTime >= allRest)
                {
                    state = StateAssistant.idle;
                    yetRestTime = 0;
                    yetWorkTime = 0;
                    yetTiredTime += float.Parse(DBManager.Instance.m_kDisperse.GetEntryPtr(20013)._Val1);
                    allRest = int.Parse(DBManager.Instance.m_kDisperse.GetEntryPtr(20004)._Val1);
                    if (SceneLogic._instance.listWasteland.Count > 0)
                    {
                        //BeginWorkWeed();
                        List<SceneLogic.Dispose> listWasteland = SceneLogic._instance.listWasteland;
                        nowDispose = listWasteland[listWasteland.Count - 1];
                        nowDispose.Enable = true;
                        WastelandDaseData wldata = DataManager._instance.wasteland.GetWastelandByGuid(nowDispose.rootBuild.guid);
                        if (!string.IsNullOrEmpty(wldata.beginTime))
                            nowDispose.BeginTime = DateTime.Parse(wldata.beginTime);
                        else
                            nowDispose.BeginTime = DateTime.Now;

                        Weed(nowDispose.rootBuild.cfg._BuildTime);
                    }
                }
                else
                {
                    for (int i = 0; i < SceneLogic._instance.listSelectWBoard.Count; i++)
                    {
                        if (SceneLogic._instance.listSelectWBoard[i].WastedGuid == nowDispose.rootBuild.guid)
                        {
                            SceneLogic._instance.listSelectWBoard[i].BoardbaseData.go.SetActive(false);
                            break;
                        }
                    }
                    nowDispose.subgrade.go.SetActive(false);
                }

            }
        }
        public void Init()
        {
            baseData = GetComponent<Actor>().baseData;
            animWrap = GetComponent<Actor>().animWrap;
            posHouse = ModelManager._instance.GetModleByType(ModelCType.MainBase)[0].go.transform.position;
            state = StateAssistant.idle;
            allWork = float.Parse(DBManager.Instance.m_kDisperse.GetEntryPtr(20003)._Val1);
            allRest = float.Parse(DBManager.Instance.m_kDisperse.GetEntryPtr(20004)._Val1);
            WasteMoveTime = float.Parse(DBManager.Instance.m_kDisperse.GetEntryPtr(20005)._Val1);
        }


        public void ImmediatelyRest()
        {
            allRest = -1;
        }

        public void GoHome()
        {
            float distance = Vector3.Distance(transform.position, posHouse);
            float timeMove = distance / baseData.cfg._Move;

            Move(posHouse, timeMove, ()=>{
                //if(state == StateAssistant.nopower)
                //{
                //    beginRest = DateTime.Now;
                //    state = StateAssistant.rest;
                //    transform.position = GenerateID.HidePos;
                //}
                //else
                //{
                    state = StateAssistant.idle;
                    List<SceneLogic.Dispose> listWasteland = SceneLogic._instance.listWasteland;
                    if (listWasteland.Count > 0)//新的请求
                    {
                        BeginWorkWeed();
                    }
                    else
                    {
                        transform.position = GenerateID.HidePos;
                    }
                //}

            }); 
        }
 


        public void BeginWorkWeed()
        {
            //if (state == StateAssistant.nopower || state == StateAssistant.rest){ 
            //    UI_Helper.ShowCommonTips(242); 
            //    return; 
            //}
            if (state != StateAssistant.idle) return;
            transform.position = posHouse;
            state = StateAssistant.work;

            List<SceneLogic.Dispose> listWasteland = SceneLogic._instance.listWasteland;
            if(listWasteland.Count == 0)
            {
                Debug.LogError("___没有荒地了 ??___");
                return;
            }
            GameObject goEnd = listWasteland[listWasteland.Count - 1].rootBuild.go;
 
            float distance = Vector3.Distance(transform.position, goEnd.transform.position);
            float timeMove = distance / baseData.cfg._Move;

            Move(goEnd.transform.position, timeMove, () => {
                nowDispose = listWasteland[listWasteland.Count - 1];
                nowDispose.Enable = true;
                MapGridMgr.Instance.BeginFreeingWastedland(nowDispose.rootBuild.go);

                WastelandDaseData wldata = DataManager._instance.wasteland.GetWastelandByGuid(nowDispose.rootBuild.guid);
                if (!string.IsNullOrEmpty(wldata.beginTime))
                    nowDispose.BeginTime = DateTime.Parse(wldata.beginTime);
                else
                    nowDispose.BeginTime = DateTime.Now;


                Weed(nowDispose.rootBuild.cfg._BuildTime);
            });
        }


        public delegate void CBMove();
        //public CBMove cbMove;
        public void Move(Vector3 posEnd, float timeMove, CBMove cbMove)
        {
            if (tweenMove != null)
            {
                tweenMove.Kill();
                tweenMove = null;
            }
            if (tweenAttack != null)
            {
                tweenAttack.Kill();
                tweenAttack = null;
            }

            bool isHouse = false;
 
            //Vector3 _posEnd = new Vector3(posEnd.x, 0, posEnd.z);
            //transform.LookAt(goEnd.transform);
            transform.LookAt(posEnd);

            if (Vector3.Distance(transform.position, posHouse) <= 2)
                isHouse = true;
            if(Vector3.Distance(posEnd, posHouse) <= 2)   //小木屋 
                isHouse = true;

            if(isHouse)
            {//跑步
                animWrap.PlayAnimType(AnimType.Walk, 0);
                uiBar.m_tranAssistant.gameObject.SetActive(false);
            }
            else
            {//走路
                animWrap.PlayAnimType(AnimType.Walk, 1);
                uiBar.m_tranAssistant.gameObject.SetActive(true);
            }
 
            tweenMove = transform.DOMove(posEnd, timeMove).SetEase(Ease.Linear).OnComplete(()=>
            {
                if (cbMove != null)
                {
                    cbMove();
                    cbMove = null;
                }  
                tweenMove.Kill();
                tweenMove = null;
            });
            DateTime dateTime = DateTime.Now;
            //uiBar.m_tranAssistant.gameObject.SetActive(true);
            tweenMove.OnUpdate(() =>
            {
                if (uiBar != null)
                {
                    float now = (float)DateTime.Now.Subtract(dateTime).TotalSeconds;
                    uiBar.UpdateAssistant(now, timeMove);
                }
            });  


        }

        public delegate void CBWeed();
        //public CBWeed cbWeed;
        public void Weed(float time)
        {
            if (tweenMove != null) 
            {
                tweenMove.Kill();
                tweenMove = null;
            }
            if (tweenAttack != null)
            {
                tweenAttack.Kill();
                tweenAttack = null;
            }

            animWrap.PlayAnimType(AnimType.Attack);
            float begin = 0;
            tweenAttack = DOTween.To(() => begin, x => begin = x, time, time).SetEase(Ease.Linear).OnComplete(() =>
            {
                tweenAttack.Kill();
                tweenAttack = null;
            });
            uiBar.m_tranAssistant.gameObject.SetActive(false);
            List<SceneLogic.Dispose> listWasteland = SceneLogic._instance.listWasteland;
            nowDispose = listWasteland[listWasteland.Count - 1];
            Building building = nowDispose.rootBuild.GetComponent<Building>();
            nowDispose.subgrade.go.SetActive(false);
 
            if (building.IsPause)
                building.IsPause = false;
            DateTime oldTime = DateTime.Now;
            tweenAttack.OnUpdate(() =>
            {
                
                for (int i = 0; i < SceneLogic._instance.listSelectWBoard.Count; i++)
                {
                    if (SceneLogic._instance.listSelectWBoard[i].WastedGuid == nowDispose.rootBuild.guid)
                    {
                        SceneLogic._instance.listSelectWBoard[i].BoardbaseData.go.SetActive(false);
                        break;
                    }
                }
                nowDispose.subgrade.go.SetActive(false);


                float diffTime = (float)DateTime.Now.Subtract(oldTime).TotalSeconds;
                oldTime = DateTime.Now;
                yetWorkTime += diffTime;
                if(uiInfo != null) uiInfo.UpdateInfo(yetWorkTime, yetTiredTime);
                //Debug.LogError("__begin__" + begin);
         
                if (yetWorkTime >= allWork && state != StateAssistant.nopower)
                {
                    tweenAttack.Kill();
                    tweenAttack = null;
                    building.IsPause = true;
                    nowDispose.barsMgr.bar.m_tranWasteland.gameObject.SetActive(false);
                    //dispose.subgrade.go.SetActive(true);
                    state = StateAssistant.nopower;
                    //GoHome();
                    BeginRest();
                }
            });

        }


        void BeginRest()
        {
            beginRest = DateTime.Now;
            state = StateAssistant.rest;
            animWrap.PlayAnimType(AnimType.Idle);
            uiBar.m_tranAssistant.gameObject.SetActive(false);
 

            if (uiInfo != null) uiInfo.onBtnClose();
        }


        public void OpenInfo()
        {
            string wavSoundPath = @"Assets\Data\Audio\@.wav";

            AudioManagerComponent Audio = World.Scene.GetComponent<AudioManagerComponent>();
            if (yetTiredTime <= 20) Audio.PlayAudio(AudioChannel.AudioChannelType.Voice, wavSoundPath.Replace("@", "NPC_assistant_spirit"));
            else if (yetTiredTime >= 80) Audio.PlayAudio(AudioChannel.AudioChannelType.Voice, wavSoundPath.Replace("@", "NPC_assistant_tired"));
            else Audio.PlayAudio(AudioChannel.AudioChannelType.Voice, wavSoundPath.Replace("@", "NPC_assistant3"));

            MobileRTSCam.instance.SmoothMoveCamera(0.1f, transform.position);
            if(state == StateAssistant.rest)
            {
                World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIWastedForceWork);
                //UIEntity _uien = UI_Helper.ShowConfirmPanel("TextChange",
                //    () => {
                //        ModelManager._instance.assistant.ImmediatelyRest();
                //    },
                //    () => {
                //        ModelManager._instance.assistant.ImmediatelyRest();
                //    }
                //);
                //string title = UI_Helper.GetTextByLanguageID(70002);
                //string content = UI_Helper.GetTextByLanguageID(70003);
                //string ensure = UI_Helper.GetTextByLanguageID(70004);
                //string cancel = UI_Helper.GetTextByLanguageID(70005);
                //_uien.GetComponent<UI_Confirm>().TextChange(title, content, ensure, cancel);
            }
            else
            {
                UIEntity entity = World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIActor_AssistantInfo);
                uiInfo = entity.GetComponent<UI_AssistantInfo>();
                uiInfo.UpdateInfo(yetWorkTime, yetTiredTime);
            }
                
        }



        public void CreateUIBar()
        {
            if (uiBar != null) return;
            UIEntity uien = World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIBar);
            uiBar = uien.GetComponent<UIBar>();
        }

        public void OnDestroy()
        {
            if (uiBar != null)
            {
                uiBar.Dispose();
                World.Scene.GetComponent<UIManagerComponent>().Remove(UI_PrefabPath.m_sUIBar);
            }
            if (tweenMove != null)
            {
                tweenMove.Kill();
                tweenMove = null;
            }
            if (tweenAttack != null)
            {
                tweenAttack.Kill();
                tweenAttack = null;
            }
        }

     

    }

}


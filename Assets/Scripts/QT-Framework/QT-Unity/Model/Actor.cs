using System.Collections.Generic;
using UnityEngine;
using System;
 
using DG.Tweening;

namespace QTFramework
{
    [System.Serializable]
    public class ActorServer
    {
        public int cfg_id;
        public Guid guid;
    }
  

    public class Actor : ModelBase
    {

        public AnimWrap animWrap;
        ActorServer Server;
        //UIBar uiBar;
        //Tween tweenMove;
        //Tween tweenAttack;
        public ActorServer GetServer
        {
            get { return Server; }
        }

        // Use this for initialization
        void Awake()
        {
            gameObject.layer = LayerMask.NameToLayer("Actor");
        }

        // Update is called once per frame
        void Update()
        {
            
        }

 
        void LateUpdate()
        {
            //if (uiBar == null) return;
            //Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
            //uiBar.FixPos(screenPos, BarType.role);
        }

        public void Init(ActorServer _server = null, BaseData _baseData = null)
        {
            baseData = _baseData;
            if(Server == null)
                Server = new ActorServer();
            if(_server != null)
            {
                Server = _server;
            }
            else  
            {
                Server.cfg_id = baseData.cfg._ID;
                Server.guid = baseData.guid;
            }

            animWrap = baseData.GetComponent<AnimWrap>();
        }

        //public delegate void CBBeginMove();
        //public CBBeginMove cBMove;
        //public void BeginMove(GameObject goEnd)
        //{
        //    Vector3 posEnd = new Vector3(goEnd.transform.position.x, 0, goEnd.transform.position.z);
        //    float distance = Vector3.Distance(transform.position, posEnd);
        //    float time = distance / baseData.cfg._Move;
        //    transform.LookAt(goEnd.transform);
        //    animWrap.PlayAnimType(AnimType.Walk);
        //    if (tweenMove != null) tweenMove.Kill();
        //    tweenMove = transform.DOMove(posEnd, time).SetEase(Ease.Linear).OnComplete(()=>
        //    {
        //        if (cBMove != null) 
        //        {
        //            cBMove();
        //            cBMove = null;
        //        }
                    
        //    });
        //    //float max = distance;
        //    DateTime dateTime = DateTime.Now;
        //    tweenMove.OnUpdate(() =>
        //    {
        //        if (uiBar != null)
        //        {
        //            float now = (float)DateTime.Now.Subtract(dateTime).TotalSeconds;
        //            //float now = Vector3.Distance(transform.position, posEnd);
        //            uiBar.UpdateAssistant(now, time);
        //        }
        //    });  
        //}

        //public delegate void CBBeginAttack();
        //public CBBeginAttack cBAttack;
        //public void BeginAttack(float time)
        //{
        //    animWrap.PlayAnimType(AnimType.Attack);
        //    if (tweenAttack != null) tweenAttack.Kill();
        //    float begin = 0;
        //    tweenAttack = DOTween.To(()=> begin, x => begin = x, time, time).SetEase(Ease.Linear).OnComplete(() =>
        //    {
        //        if (cBAttack != null) 
        //        {
        //            cBAttack();
        //            cBAttack = null;
        //        }
                    
        //    });
           
        //    tweenAttack.OnUpdate(() =>
        //    {
        //        if (uiBar != null)
        //        {
        //            uiBar.UpdateAssistant(begin, time);
        //        }
        //    });

        //}


        //public void CreateUIBar()
        //{
        //    if (uiBar != null) return;
        //    UIEntity uien = World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIBar);
        //    uiBar = uien.GetComponent<UIBar>();
        
        //}

        //public void OnDestroy()
        //{
        //    if (uiBar != null)
        //    {
        //        uiBar.Dispose();
        //        World.Scene.GetComponent<UIManagerComponent>().Remove(UI_PrefabPath.m_sUIBar);
        //    }
        //    if (tweenMove != null) tweenMove.Kill();
        //    if (tweenAttack != null) tweenAttack.Kill();
        //}

        //public override void Init(GameObject _go, uint _id) 
        //{
        //    go = _go;
        //    id = _id;
        //    animWrap = go.GetComponent<AnimWrap>();
        //    if (animWrap == null)
        //        Log.Warning("这个角色模型 :" + go.name + " 没有挂animWrap脚本");
        //} 


        //public void PlayAnim(string animName)
        //{
        //    animWrap.AfreshPlay(animName);
        //}

        //public void PauseAnim()
        //{
        //    animWrap.Pause();
        //}

        //public void GoonAnim()
        //{
        //    animWrap.Goon();
        //}


        //public delegate void AnimEvent(AnimData anim);
        ////public event AnimEvent callback;


        //public void CallBackAnimBegin(AnimEvent callback)
        //{
        //    if (callback == null) return;
        //    animWrap.OnAnimationBegin += (anim) => { callback(anim); };
        //}

        //public void CallBackAnimEnd(AnimEvent callback)
        //{
        //    if (callback == null) return;
        //    animWrap.OnAnimationEnd += (anim) => { callback(anim); };
        //}


    }

}


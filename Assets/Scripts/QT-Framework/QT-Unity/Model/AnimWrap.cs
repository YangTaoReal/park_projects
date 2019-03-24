using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



public enum AnimType
{
    Idle = 1,
    Walk = 2,
    Die = 3,
    Attack = 4,
}


//[Serializable]
//public class AnimDatass
//{
//    public AnimData[] AnimData = new AnimData[0];
//}

[Serializable]
public class AnimData
{
    public AnimationClip clip;
 
    [HideInInspector] public float speed = 1;
    [HideInInspector] public int nLoop = 1;//-1是无限循环
    [HideInInspector] public int timesPlayed = 0;//播放了多少次
    [HideInInspector] public float secondsPlayed = 0;//当前播放了多少时间. 不叠加
    [HideInInspector] public float length = 0;// 动画长度
}


public class AnimWrap : MonoBehaviour {

    public List<AnimData> Idles = new List<AnimData>();
    public List<AnimData> Walks = new List<AnimData>();
    public List<AnimData> Dies = new List<AnimData>();
    public List<AnimData> Attacks = new List<AnimData>();
  
    public Dictionary<int, List<AnimData>> animations = new Dictionary<int, List<AnimData>>();
 

    Animator animator;
    AnimatorOverrideController aController;

    public delegate void AnimEvent(AnimData animationData);
    public event AnimEvent OnAnimationBegin;
    public event AnimEvent OnAnimationEnd;


    AnimData nowAnimData;

    AnimType nowAnimType;//当前动作类型
    void Awake()
    {
        animations.Clear();
        animator = GetComponent<Animator>();
        aController = new AnimatorOverrideController();
        aController.runtimeAnimatorController = animator.runtimeAnimatorController;
        foreach (AnimType item in Enum.GetValues(typeof(AnimType)))
        {
            if (item == AnimType.Idle)
                animations.Add((int)item, Idles);
            else if (item == AnimType.Walk)
                animations.Add((int)item, Walks);
            else if (item == AnimType.Die)
                animations.Add((int)item, Dies);
            else if (item == AnimType.Attack)
                animations.Add((int)item, Attacks);

        }
    }

    void Start()
    {
    
    }

 

    AnimData RandomAnim(AnimType state, int _idx = -1)
    {
        nowAnimType = state;
        List<AnimData> listAnim = animations[(int)state];
        if(listAnim == null || listAnim.Count == 0)
        {
            return null;
        }

        int idx;
        if (_idx == -1)
            idx = UnityEngine.Random.Range(0, listAnim.Count);
        else
            idx = _idx;
        return listAnim[idx];
    }

    /// <summary>
    /// 暂停
    /// </summary>
    public void Pause()
    {
        animator.speed = 0;
    }

    /// <summary>
    /// 继续
    /// </summary>
    public void Goon()
    {
        //animator.speed = nowAnimData.speed;
    }


    public float GetAnimLength()
    {
        return nowAnimData.length;
    }


    void FixedUpdate()
    {
        return;
        if (nowAnimData == null) 
            return;

        if (nowAnimData.secondsPlayed == nowAnimData.length)
        {
            nowAnimData.timesPlayed++;
            if (nowAnimData.nLoop == -1)
            {
                PlayAnim(nowAnimData);
            }
            else
            {
                if (nowAnimData.timesPlayed == nowAnimData.nLoop)
                {
                    if (OnAnimationEnd != null)
                    {
                        OnAnimationEnd(nowAnimData);
                    }
                        
                }
                else if (nowAnimData.timesPlayed < nowAnimData.nLoop)
                {
                    PlayAnim(nowAnimData);
                }   
            }
 
        }
  
        else
        {
            nowAnimData.secondsPlayed += (Time.fixedDeltaTime * animator.speed);
            if (nowAnimData.secondsPlayed > nowAnimData.length)
                nowAnimData.secondsPlayed = nowAnimData.length;
        }

    }

    // Update is called once per frame
    void Update()
    {

    }

    //AnimData GetAnimData(string name)
    //{
    //    AnimData animData = null;
    //    //foreach (AnimData data in animations)
    //    //{
    //    //    if (name == data.clipName)
    //    //    {
    //    //        animData = data;
    //    //        break;
    //    //    }
    //    //}
    //    return animData;
    //}
    ///// <summary>
    ///// 从头开始播放动作
    ///// </summary>
    ///// <param name="name">Name.</param>
    //public void AfreshPlay(string name)
    //{
        

    //    AnimData animData = GetAnimData(name);
       
    //    animData.timesPlayed = 0;
    //    PlayAnim(animData);

    //    if (OnAnimationBegin != null)
    //        OnAnimationBegin(nowAnimData);
    //}

    public void PlayAnimType(AnimType state, int idx = -1)
    {
        AnimData animData = RandomAnim(state, idx);
        if (animData == null)
            return;
        animData.timesPlayed = 0;
        PlayAnim(animData);
        if (OnAnimationBegin != null)
            OnAnimationBegin(nowAnimData);
    }

    void PlayAnim(AnimData animData)
    {
        if (animData == null)
            return;
  
        animData.secondsPlayed = 0;
        aController["_State"] = animData.clip;
        //animator.speed = animData.speed;
        animator.speed = 1;
        animator.runtimeAnimatorController = aController;
        animator.Play("_State", 0, 0);
        nowAnimData = animData;
    }

    
}

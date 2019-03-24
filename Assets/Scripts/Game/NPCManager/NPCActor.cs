using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using QTFramework;

public class NPCActor : MonoBehaviour {

    public enum AnimationState
    {
        Idle,
        Walk,
    }

    public Animator animator;

    public float timer;     // 计时器
    private float stateTime;    // 状态持续时间 离散表随机
    private bool NeedUpdateDir = true;
    private float rotateSpeed = 90;
    private Vector2 randomRotate = new Vector2(0, 180);
    private bool isOutsideBuild = false;
    [Range(0, 100.0f)]
    public float walkSpeed = 1;

    private AnimationState currState;
    public AnimationState CurrState{
        get { return currState; }
        set{
            currState = value;
            AnimationStateChange(currState);
        }
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        CurrState = AnimationState.Walk;
    }

    public void AnimationStateChange(AnimationState state)
    {
        switch(state)
        {
            case AnimationState.Idle:
                ChangeToIdel();
                stateTime = 1;// Random.Range(2f, 5f);
                break;
            case AnimationState.Walk:
                ChangeToWalk();
                stateTime = 12;// Random.Range(4f, 9f);
                break;
        }
        //Debug.Log($"当前状态:{CurrState.ToString()},当前状态持续时间:{stateTime}");
    }

    public void ChangeToIdel()
    {
        animator.SetBool("toIdle", true);
        animator.SetBool("toWalk", false);
    }

    public void ChangeToWalk()
    {
        animator.SetBool("toWalk", true);
        animator.SetBool("toIdle", false);
    }

    //public void MoveActor()
    //{
        
    //}

    void UpdateState(float dt)
    {

        stateTime -= dt;
        if (stateTime < 0)
        {
            if (CurrState == AnimationState.Idle)
            {
                CurrState = AnimationState.Walk;
                //MoveNext();
            }
            else if (CurrState == AnimationState.Walk)
            {
                CurrState = AnimationState.Idle;
            }
            NeedUpdateDir = true;
        }
    }

    float speed;
    int sign = 1;
    private void FixedUpdate()
    {
        UpdateState(Time.fixedDeltaTime);
        if (CurrState == AnimationState.Idle)
        {
            return;
        }
        else
        {
            
            if (NeedUpdateDir)
            {
                NeedUpdateDir = false;
                rotateSpeed = Random.Range(randomRotate.x, randomRotate.y);
                //sign = Random.Range(1, 101);
                //if (sign >= 50)
                //    sign = 1;
                //else
                //    sign = -1;
                //rotateSpeed *= sign;
                Debug.Log($"随机转向了,rotateSpeed = {rotateSpeed}");
                transform.Rotate(Vector3.up, Time.fixedDeltaTime * rotateSpeed);
                transform.DORotate(new Vector3(0, rotateSpeed, 0), 0.5f).OnComplete(() => { 
                });

                speed = 0;
            }
            else
            {
                speed = walkSpeed;
            }

            //Debug.Log($"移动速度:{speed}");
            transform.position += transform.forward * speed * Time.fixedDeltaTime;

        }
    }

    private void OnTriggerExit(Collider other)
    {
        Building build = other.gameObject.GetComponent<Building>();
        if (build == null)
            return;
        if(build.baseData.cfg._Type == 2)
        {
            //isOutsideBuild = true;
            //Debug.Log($"刚刚走出建筑物，现在打开建筑物导航0000000");
        }
                              
    }

    // 碰到物体的转向 和随机方向的转向不同
    public void TurnAround()
    {
        float y = transform.rotation.eulerAngles.y;
        if (y > 360)
            y -= 360;
        if (y + 180 >= 360)
            y = (y + 180) - 360;
        else
            y = y + 180;
        Vector3 before = new Vector3(0, y, 0);
        //transform.DORotate(before, 0.1f);
        transform.forward = -transform.forward;
        Debug.Log($"碰到障碍物转向,转向前方向 = {before},转向后的方向 = {transform.forward}");

    }

    //private void OnTriggerStay(Collider other)
    //{
    //    Building build = other.gameObject.GetComponent<Building>();
    //    if (build == null || !isOutsideBuild)
    //        return;
    //    if (build.baseData.cfg._Ctype != 23)
    //    {
    //        Debug.Log($"撞到了:{other.gameObject.name},cType = {build.baseData.cfg._Ctype}");
    //        randomRotate = new Vector2(0, 180);
    //        NeedUpdateDir = true;
    //    }
    //}

    private void OnTriggerEnter(Collider other)
    {
        Building build = other.gameObject.GetComponent<Building>();
        if (build == null)
            return;
        if(build.baseData.cfg._Ctype != 23)
        {
            //Debug.Log($"撞到了建筑,转向:{other.gameObject.name},cType = {build.baseData.cfg._Ctype}");
            //randomRotate = new Vector2(0, 180);
            //NeedUpdateDir = true;
            TurnAround();
        }
    }

}

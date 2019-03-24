using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalMove : MonoBehaviour
{
    // 动画切换事件
    public delegate void AnimationEvent();
    public AnimationEvent OnIdle;
    public AnimationEvent OnWalk;

    // 速度
    [Range(0, 100.0f)]
    public float walkSpeed;

    public float rotateSpeed = 90;

    enum State
    {
        nil,
        idle,
        walk
    }
    State _state;
    State state
    {
        get
        {
            return _state;
        }

        set
        {
            if (_state == value) return;

            _state = value;

            if (_state == State.idle)
            {
                ChangeToIdelAnim();
                stateTime = Random.Range(1.0f, 5.0f);
            }
            else if (_state == State.walk)
            {
                ChangeToWalkAnim();
                stateTime = float.MaxValue;
                speed = walkSpeed;
            }
        }
    }

    float speed;
    bool NeedUpdateDir = true;

    Dictionary<int, List<int>> neighbor = new Dictionary<int, List<int>>();
    //List<Rect> rectList = new List<Rect>();
    List<Vector3> posList = new List<Vector3>();
    Queue<int> movedList = new Queue<int>();
    List<int> tempList = new List<int>();
    Vector3 moveDir;
    Vector3 moveTo;
    int idx;
    float stateTime;

    void Start()
    {
        InitPos();
    }

    // 清理行走区域
    public void ClearArea()
    {
        posList.Clear();
        //rectList.Clear();
        neighbor.Clear();
        movedList.Clear();
        tempList.Clear();
        NeedUpdateDir = true;

        _state = State.nil;
        stateTime = 0;
    }

    // 设置行走区域
    public void SetArea(List<Vector3> vectors)
    {
        if (vectors == null || vectors.Count == 0) return;

        ClearArea();

        for (int i = 0; i < vectors.Count; i++)
        {
            posList.Add(vectors[i]);
            //var rect = new Rect();
            //rect.center = new Vector2(vectors[i].x, vectors[i].z);
            //rect.width = rect.height = 2.5f;
            //rectList.Add(rect);
            for (int j = 0; j < vectors.Count; j++)
            {
                if (i != j && Vector3.Distance(vectors[i], vectors[j]) <= 3)
                {
                    if(!neighbor.ContainsKey(i)){
                        neighbor.Add(i, new List<int>());
                    }
                    neighbor[i].Add(j);
                }
            }
        }

        InitPos();
    }

    void InitPos()
    {
        if (neighbor.Count == 0)
        {
            if (posList.Count == 0) return;
        }

        idx = Random.Range(0, posList.Count);
        float val = Random.Range(-1f, 1f);
        transform.position = posList[idx] + new Vector3(val, 0, val);

        state = State.idle;

        movedList.Enqueue(idx);
        NeedUpdateDir = true;
    }

    void MoveNext()
    {

        if (movedList.Count >= posList.Count)
        {
            movedList.Clear();
        }

        float val = Random.Range(-1f, 1f);
        if (neighbor.Count > 0)
        {
            tempList.Clear();
            int j = -1;
            for (int i = 0; i < neighbor[idx].Count; i++)
            {
                if(!movedList.Contains(neighbor[idx][i]))
                {
                    tempList.Add(i);
                }
            }
            if(tempList.Count == 0)
            {
                j = Random.Range(0, neighbor[idx].Count);
                movedList.Clear();
            }
            else
            {
                j = tempList[Random.Range(0, tempList.Count)];
            }

            idx = neighbor[idx][j];
        }
        else
        {
            if (posList.Count > 0)
            {
                idx = Random.Range(0, posList.Count);
            }
            else
            {
                state = State.idle;
                return;
            }
        }

        moveTo = posList[idx] + new Vector3(val, 0, val);
        moveDir = Vector3.Normalize(moveTo - transform.position);
        NeedUpdateDir = true;

        if(!movedList.Contains(idx))
        {
            movedList.Enqueue(idx);
        }
    }

    void ChangeToIdelAnim()
    {
        if (OnIdle != null) OnIdle();
    }

    void ChangeToWalkAnim()
    {
        if (OnWalk != null) OnWalk();
    }

    void UpdateState(float dt)
    {

        stateTime -= dt;
        if (stateTime < 0)
        {
            if (state == State.idle)
            {
                state = State.walk;
                MoveNext();
            }
            else if (state == State.walk)
            {
                state = State.idle;
            }
        }
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        UpdateState(Time.fixedDeltaTime);

        if (state == State.idle)
        {
            return;
        }
        else
        {
            if (Vector3.Distance(moveTo, transform.position) < 0.2f)
            {
                state = State.idle;
            }
            else
            {
                if(NeedUpdateDir)
                {
                    var a = Vector3.SignedAngle(moveDir, transform.forward, Vector3.up);
                    if (a > 5)
                    {
                        transform.Rotate(Vector3.up, -Time.fixedDeltaTime * rotateSpeed);
                    }
                    else if (a < -5)
                    {
                        transform.Rotate(Vector3.up, Time.fixedDeltaTime * rotateSpeed);
                    }
                    else
                    {
                        transform.LookAt(moveTo);
                        NeedUpdateDir = false;
                    }
                }
                else
                {
                    transform.position += transform.forward * speed * Time.fixedDeltaTime;
                }
            }

        }
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("barrier"))// || other.CompareTag("animal"))
    //    {
    //        state = State.idle;
    //    }
    //}
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(ContentSizeFitter))]
public class LoopGridLayoutGroup : MonoBehaviour
{
    public Action<int, UIEntity> OnValueChange;
    public enum Constraint
    {
        ColumnCount,
        RowCount,
    }
    public enum Axis
    {
        Horizontal,
        Vertical
    }
    public enum MoveDirec
    {
        None,
        Left,
        Right,
        Up,
        Down
    }
    public enum ChildAlignment
    {
        UpperLeft,
        UpperCenter,
        UpperRight,
        MiddleLeft,
        MiddleCenter,
        MiddleRight,
        LowerLeft,
        LowerCenter,
        LowerRight,
    }
    /// <summary>
    /// 列表长度
    /// </summary>
    [HideInInspector]
    private int m_kMaxItemCount;
    /// <summary>
    /// 当前索引
    /// </summary>
    [HideInInspector]
    private int m_kCurrentIndex = 0;
    /// <summary>
    /// 可视Item数量
    /// </summary>
    [HideInInspector]
    public Vector2Int m_kShowItemCount = Vector2Int.zero;
    /// <summary>
    /// Item个数
    /// </summary>
    [HideInInspector]
    private Vector2Int m_kItemCount = Vector2Int.zero;
    /// <summary>
    /// 滑动方向
    /// </summary>
    private MoveDirec m_kMoveDirec = MoveDirec.None;
    /// <summary>
    /// Content的RectTransform
    /// </summary>
    private RectTransform m_kRectTransform;

    /// <summary>
    /// 结束位置
    /// </summary>
    public Vector3 m_kEndPos;
    /// <summary>
    /// 上一次停留的位置
    /// </summary>
    [HideInInspector]
    public Vector3 m_kLastStopPos = Vector3.zero;
    /// <summary>
    /// Grid的宽度
    /// </summary>
    [HideInInspector]
    public Vector2 m_kLoopGridRect = Vector2.zero;

    Dictionary<int, UIEntity> m_kItemEntity = new Dictionary<int, UIEntity>();

    [HideInInspector]
    public Axis m_kAxis;
    /// <summary>
    /// 起始位置
    /// </summary>
    public Vector3 m_kStartPos;
    public ScrollRect m_kScrollRect;
    /// <summary>
    /// 水平和竖直方向的间隔
    /// </summary>
    public Vector2Int m_kSpacing;
    /// <summary>
    /// 每个Item的大小
    /// </summary>
    public Vector2Int m_kCellSize;
    /// <summary>
    /// 子节点开始位置
    /// </summary>
    public ChildAlignment m_kChildAlignment;
    /// <summary>
    /// 行列数
    /// </summary>
    public Constraint m_kConstraint;
    /// <summary>
    /// 行列数
    /// </summary>
    public int ConstraintCount=0;
    public void Awake()
    {
        m_kRectTransform = transform.GetComponent<RectTransform>();
    }
    public void Init()
    {
        m_kMoveDirec = MoveDirec.None;

        //起始位置
        m_kRectTransform.anchoredPosition3D = m_kStartPos;
        RectTransform ParentRectTransform = this.gameObject.transform.parent.GetComponent<RectTransform>();
        m_kLoopGridRect =new Vector2(ParentRectTransform.rect.width, ParentRectTransform.rect.height);


        //初始化
        if (m_kAxis == Axis.Horizontal)
        {
            //可视区域Item数量
            m_kShowItemCount.x = ((int)m_kLoopGridRect.x / m_kCellSize.x);
            if ((int)m_kLoopGridRect.x % m_kCellSize.x != 0)
            {
                m_kShowItemCount.x++;
            }
            m_kShowItemCount.y = ConstraintCount;
            //一共存在的Item数量（比可视化多一个就可以）
            m_kItemCount.x = m_kShowItemCount.x + 1;


            for (int i = 0; i < ConstraintCount ; i++)
            {
                for (int j=0;j< m_kItemCount.x; j++)
                {
                    RectTransform _TempRectTransform = m_kRectTransform.GetChild(j).GetComponent<RectTransform>();
                    _TempRectTransform.anchoredPosition3D = new Vector3(m_kCellSize.x *j, i* m_kCellSize.y, 0);
                }  
            }

            m_kEndPos = new Vector3(m_kLoopGridRect.x - (m_kMaxItemCount * m_kCellSize.x - m_kStartPos.x), m_kRectTransform.anchoredPosition3D.y, m_kRectTransform.anchoredPosition3D.z);
        }
        else if (m_kAxis == Axis.Vertical)
        {
            //可视区域Item数量
            m_kShowItemCount.y = ((int)m_kLoopGridRect.y / m_kCellSize.y);
            if ((int)m_kLoopGridRect.y % m_kCellSize.y != 0)
            {
                m_kShowItemCount.y++;
            }
            m_kShowItemCount.x = ConstraintCount;
            //一共存在的Item数量（比可视化多一个就可以）
            m_kItemCount.y = m_kShowItemCount.y + 1;


            for (int i = 0; i < ConstraintCount; i++)
            {
                for (int j = 0; j < m_kItemCount.y; j++)
                {
                    RectTransform _TempRectTransform = m_kRectTransform.GetChild(j).GetComponent<RectTransform>();
                    _TempRectTransform.anchoredPosition3D = new Vector3(m_kCellSize.x * i, -j * m_kCellSize.y, 0);
                }
            }

            m_kEndPos = new Vector3(m_kRectTransform.anchoredPosition3D.x, m_kMaxItemCount * m_kCellSize.y - m_kLoopGridRect.y - m_kStartPos.y, m_kRectTransform.anchoredPosition3D.z);
            Debug.Log("@@@@@@@@@" + m_kEndPos);
        }

        //上次起点停留的位置（为了计算滑动方向）
        m_kLastStopPos = m_kRectTransform.anchoredPosition3D;

        m_kCurrentIndex = m_kItemCount.x-1;

        Debug.Log($"宽度{m_kLoopGridRect.x};初始化当前索引{m_kCurrentIndex};列表长度{m_kMaxItemCount};需要可视区域显示{m_kShowItemCount}个;需要加载{m_kItemCount}个;起始位置{m_kStartPos};结束位置{m_kEndPos}");
    }
    public void Update()
    {
        if (BoundaryDetection())
        {
            return;
        }
        UpdateItemNode();
    }

    private bool BoundaryDetection()
    {
        if (m_kAxis == Axis.Horizontal)
        {
            if (m_kRectTransform.anchoredPosition3D.x > m_kStartPos.x)
            {
                m_kRectTransform.anchoredPosition3D = m_kStartPos;
                return true;
            }
            if (m_kRectTransform.anchoredPosition3D.x < m_kEndPos.x)
            {
                m_kRectTransform.anchoredPosition3D = m_kEndPos;
                return true;
            }
        }
        else if (m_kAxis == Axis.Vertical)
        {
            if (m_kRectTransform.anchoredPosition3D.y < m_kStartPos.y)
            {
                m_kRectTransform.anchoredPosition3D = m_kStartPos;
                return true;
            }
            if (m_kRectTransform.anchoredPosition3D.y > m_kEndPos.y)
            {
                m_kRectTransform.anchoredPosition3D = m_kEndPos;
                return true;
            }
        }

        return false;
    }


    private void UpdateItemNode()
    {
        if (m_kAxis == Axis.Horizontal)
        {
            if (m_kRectTransform.anchoredPosition3D.x < m_kLastStopPos.x)
            {
                if (m_kMoveDirec == MoveDirec.Right)
                {
                    m_kCurrentIndex += m_kShowItemCount.x;
                }
                m_kMoveDirec = MoveDirec.Left;
            }
            else if (m_kRectTransform.anchoredPosition3D.x > m_kLastStopPos.x)
            {
                if (m_kMoveDirec == MoveDirec.Left)
                {
                    m_kCurrentIndex -= m_kShowItemCount.x;
                }
                m_kMoveDirec = MoveDirec.Right;
            }
        }
        else if (m_kAxis == Axis.Vertical)
        {
            if (m_kRectTransform.anchoredPosition3D.y < m_kLastStopPos.y)
            {
                if (m_kMoveDirec == MoveDirec.Up)
                {
                    m_kCurrentIndex += m_kShowItemCount.y;
                }
                m_kMoveDirec = MoveDirec.Down;
            }
            else if (m_kRectTransform.anchoredPosition3D.y > m_kLastStopPos.y)
            {
                if (m_kMoveDirec == MoveDirec.Down)
                {
                    m_kCurrentIndex -= m_kShowItemCount.y;
                }
                m_kMoveDirec = MoveDirec.Up;
            }
        }





        if (m_kLastStopPos == m_kRectTransform.anchoredPosition3D)
        {
            return;
        }

        if (m_kMoveDirec == MoveDirec.Left)
        {
            MoveLeft();
        }
        else if (m_kMoveDirec == MoveDirec.Right)
        {
            MoveRight();
        }
        else if (m_kMoveDirec == MoveDirec.Up)
        {
            MoveUp();
        }
        else if (m_kMoveDirec == MoveDirec.Down)
        {
            MoveDown();
        }
        m_kLastStopPos = m_kRectTransform.anchoredPosition3D;
    }

    /// <summary>
    /// 向左滑动
    /// </summary>
    private void MoveLeft()
    {
        float weizhi = 0;
        weizhi = Mathf.Abs(m_kRectTransform.anchoredPosition3D.x - m_kStartPos.x);
        if (m_kCurrentIndex < (m_kMaxItemCount - 1) && weizhi > (m_kCurrentIndex + 1 - m_kShowItemCount.x) * m_kCellSize.x)
        {
            m_kCurrentIndex++;
            RectTransform rectTransform = m_kRectTransform.GetChild(0).GetComponent<RectTransform>();
            rectTransform.SetAsLastSibling();
            rectTransform.anchoredPosition3D = new Vector3(m_kCellSize.x * m_kCurrentIndex, 0, 0);
            OnValueChange?.Invoke(m_kCurrentIndex, m_kItemEntity[rectTransform.gameObject.GetInstanceID()]);
        }
    }

    /// <summary>
    /// 向右滑动
    /// </summary>
    private void MoveRight()
    {
        float weizhi = 0;
        weizhi = Mathf.Abs(m_kRectTransform.anchoredPosition3D.x - m_kEndPos.x);
        if (m_kCurrentIndex > 0 && weizhi > (m_kMaxItemCount - m_kShowItemCount.x - m_kCurrentIndex) * m_kCellSize.x)
        {
            m_kCurrentIndex--;
            RectTransform rectTransform = m_kRectTransform.GetChild(m_kRectTransform.childCount - 1).GetComponent<RectTransform>();
            rectTransform.SetAsFirstSibling();
            rectTransform.anchoredPosition3D = new Vector3(m_kCellSize.x * m_kCurrentIndex, 0, 0);
            OnValueChange?.Invoke(m_kCurrentIndex, m_kItemEntity[rectTransform.gameObject.GetInstanceID()]);
        }
    }

    /// <summary>
    /// 向上滑动
    /// </summary>
    private void MoveUp()
    {
        float weizhi = 0;
        weizhi = Mathf.Abs(m_kRectTransform.anchoredPosition3D.x - m_kStartPos.x);
        if (m_kCurrentIndex < (m_kMaxItemCount - 1) && weizhi > (m_kCurrentIndex + 1 - m_kShowItemCount.x) * m_kCellSize.x)
        {
            m_kCurrentIndex++;
            RectTransform rectTransform = m_kRectTransform.GetChild(0).GetComponent<RectTransform>();
            rectTransform.SetAsLastSibling();
            rectTransform.anchoredPosition3D = new Vector3(m_kCellSize.x * m_kCurrentIndex, 0, 0);
            Debug.Log($"当前缩影{m_kCurrentIndex}");
        }
    }

    /// <summary>
    /// 向下滑动
    /// </summary>
    private void MoveDown()
    {
        float weizhi = 0;
        weizhi = Mathf.Abs(m_kRectTransform.anchoredPosition3D.x - m_kStartPos.x);
        if (m_kCurrentIndex < (m_kMaxItemCount - 1) && weizhi > (m_kCurrentIndex + 1 - m_kShowItemCount.x) * m_kCellSize.x)
        {
            m_kCurrentIndex++;
            RectTransform rectTransform = m_kRectTransform.GetChild(0).GetComponent<RectTransform>();
            rectTransform.SetAsLastSibling();
            rectTransform.anchoredPosition3D = new Vector3(m_kCellSize.x * m_kCurrentIndex, 0, 0);
            Debug.Log($"当前缩影{m_kCurrentIndex}");
        }
    }
    /// <summary>
    /// 初始化LooplayoutGroup
    /// </summary>
    /// <param name="_MaxItemCount"></param>
    public void Init(Dictionary<int, UIEntity> _kUIEntity, int _MaxItemCount)
    {
        m_kItemEntity = _kUIEntity;
        m_kMaxItemCount = _MaxItemCount;

        Init();
    }
}

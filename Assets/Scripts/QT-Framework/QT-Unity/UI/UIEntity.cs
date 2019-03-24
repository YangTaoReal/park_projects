using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using QTFramework;

using UnityEngine;

#region ----UI类型------
public enum UILayer
{
    /// <summary>
    ///自定义
    /// </summary>
    Defined = -1,
    //界面里面的Item
    Item = 0,
    ///系统级UI
    System = 30,
    ///菜单级UI
    Page = 60,
    ///弹窗UI1
    PopUpWindow = 90,
    //挂件（Tips）
    PopUpWidget = 150,
    ///顶层
    PopUpTop = 210,

}
#endregion

#region ----UI锚点类型------
public enum UIAnchorType
{
    //左上
    Anchor_TopLeft = 1,
    //上居中
    Anchor_TopCenter,
    //上右
    Anchor_TopRight,
    //右居中
    Anchor_RightCenter,
    //下右
    Anchor_BottomRight,
    //下居中
    Anchor_BottomCenter,
    //左下
    Anchor_BottomLeft,
    //左居中
    Anchor_LeftCenter,
    //全屏居中
    Anchor_Center
}
#endregion

[ObjectEventSystem]
public class UIEntityAwakeSystem : AAwake<UIEntity, string>
    {
        public override void Awake (UIEntity self, string _UIFullPath)
        {
            self.Awake (_UIFullPath);
        }
    }

[ObjectEventSystem]
public class UIEntityStartSystem : AStart<UIEntity>
    {
        public override void Start (UIEntity self)
        {
            self.Start ( );
        }
    }
public class UIEntity : QTEntity
{
    public int ChildrenCount
    {
        get
        {
            return m_kDicChildren.Count;
        }
    }

    public UIPrefab m_kUIPrefab;
    private Dictionary<string, List<UIEntity>> m_kDicChildren = new Dictionary<string, List<UIEntity>> ( );



    public void Awake (string _UIFullPath)
    {
        this.m_kDicChildren.Clear ( );
        GameObject go = AssetPoolManager.Instance.Fetch(_UIFullPath) as GameObject;
        go.gameObject.SetActive(true);
        m_kUIPrefab = go.GetComponent<UIPrefab> ( );
        m_kUIPrefab.m_kUIFullPath = _UIFullPath;
        m_kUIPrefab.m_kUIName = go.name;
        m_kUIPrefab.transform.SetParent (World.Scene.GetComponent<WorldManagerComponent> ( ).m_kUIRootNode);
        m_kUIPrefab.OpenUI();
        UpdateAnchor ( );
    }

    public void Start ( )
    {
        if (m_kUIPrefab.m_kUEAnimation != null)
        {
            string tempName = Path.GetFileNameWithoutExtension (m_kUIPrefab.m_kUIFullPath) + "_open";
            if (m_kUIPrefab.m_kUEAnimation.GetClip (tempName) != null)
            {
                m_kUIPrefab.m_kUEAnimation.Play (tempName);
            }
        }
    }
    public override void Dispose ( )
    {
        if (this.Disposed)
        {
            return;
        }
        base.Dispose ( );

        var childrenList = m_kDicChildren.GetEnumerator ( );
        while (childrenList.MoveNext ( ))
        {
            foreach (var ui in childrenList.Current.Value)
            {
                ui.Dispose ( );
            }
        }

        m_kDicChildren.Clear ( );
        m_kUIPrefab.CloseUI();
        AssetPoolManager.Instance.Recycle (m_kUIPrefab.gameObject);
    }
    public void AddChildren (UIEntity _children)
    {
        List<UIEntity> childrenList = null;

        m_kDicChildren.TryGetValue (_children.m_kUIPrefab.m_kUIFullPath, out childrenList);

        if (childrenList == null)
        {
            childrenList = new List<UIEntity> ( );
            m_kDicChildren [_children.m_kUIPrefab.m_kUIFullPath] = childrenList;
        }

        childrenList.Add (_children);
        _children.ParentEntity = this;
    }
    public void RemoveChildren (UIEntity _children)
    {
        var childrenList = m_kDicChildren.GetEnumerator ( );
        while (childrenList.MoveNext ( ))
        {
            foreach (var ui in childrenList.Current.Value)
            {
                if (ui == _children)
                {
                    childrenList.Current.Value.Remove (_children);
                    ui.Dispose ( );
                    return;
                }
            }
        }
    }
    public void ClearChildren()
    {
        var childrenList = m_kDicChildren.ToList();
        foreach (var childrens in childrenList)
        {
            foreach (var ui in childrens.Value)
            {
                ui.Dispose();
            }
        }
        m_kDicChildren.Clear();
    }
    public int GetChildrenCount(string _children)
    {
        List<UIEntity> childrenList = null;

        m_kDicChildren.TryGetValue(_children, out childrenList);

        if (childrenList == null)
        {
            return 0;
        }
        return childrenList.Count;
    }
    /// <summary>
    /// 界面更新
    /// </summary>
    public override void UpdateLogic ( )
    {

    }

    /// <summary>
    /// 翻译
    /// </summary>
    public virtual void TranslateView ( )
    {

    }

    /// <summary>
    /// 更新层级
    /// </summary>
    /// <param name="_iDepth"></param>
    public void UpdateDepth (int _iDepth)
    {
        m_kUIPrefab.UpdateSortOrderLayer (_iDepth);
    }

    public void UpdateAnchor ( )
    {
        //左上
        if (m_kUIPrefab.m_kUIAnchorType == UIAnchorType.Anchor_TopLeft)
        {
            m_kUIPrefab.m_kGameObjectRectTransform.anchorMin = new Vector2 (0, 1);
            m_kUIPrefab.m_kGameObjectRectTransform.anchorMax = new Vector2 (0, 1);
        }
        //顶部居中
        else if (m_kUIPrefab.m_kUIAnchorType == UIAnchorType.Anchor_TopCenter)
        {
            m_kUIPrefab.m_kGameObjectRectTransform.anchorMin = new Vector2 (0, 1);
            m_kUIPrefab.m_kGameObjectRectTransform.anchorMax = new Vector2 (1, 1);
        }
        //右上
        else if (m_kUIPrefab.m_kUIAnchorType == UIAnchorType.Anchor_TopRight)
        {
            m_kUIPrefab.m_kGameObjectRectTransform.anchorMin = new Vector2 (1, 1);
            m_kUIPrefab.m_kGameObjectRectTransform.anchorMax = new Vector2 (1, 1);
        }
        //右居中
        else if (m_kUIPrefab.m_kUIAnchorType == UIAnchorType.Anchor_RightCenter)
        {
            m_kUIPrefab.m_kGameObjectRectTransform.anchorMin = new Vector2 (1, 0);
            m_kUIPrefab.m_kGameObjectRectTransform.anchorMax = new Vector2 (1, 1);
        }
        //右下
        else if (m_kUIPrefab.m_kUIAnchorType == UIAnchorType.Anchor_BottomRight)
        {
            m_kUIPrefab.m_kGameObjectRectTransform.anchorMin = new Vector2 (1, 0);
            m_kUIPrefab.m_kGameObjectRectTransform.anchorMax = new Vector2 (1, 0);
        }
        //底部居中
        else if (m_kUIPrefab.m_kUIAnchorType == UIAnchorType.Anchor_BottomCenter)
        {
            m_kUIPrefab.m_kGameObjectRectTransform.anchorMin = new Vector2 (0, 0);
            m_kUIPrefab.m_kGameObjectRectTransform.anchorMax = new Vector2 (1, 0);
        }
        //左下
        else if (m_kUIPrefab.m_kUIAnchorType == UIAnchorType.Anchor_BottomLeft)
        {
            m_kUIPrefab.m_kGameObjectRectTransform.anchorMin = new Vector2 (0, 0);
            m_kUIPrefab.m_kGameObjectRectTransform.anchorMax = new Vector2 (0, 0);
        }
        //左居中
        else if (m_kUIPrefab.m_kUIAnchorType == UIAnchorType.Anchor_LeftCenter)
        {
            m_kUIPrefab.m_kGameObjectRectTransform.anchorMin = new Vector2 (0, 0);
            m_kUIPrefab.m_kGameObjectRectTransform.anchorMax = new Vector2 (0, 1);
        }
        //居中
        else if (m_kUIPrefab.m_kUIAnchorType == UIAnchorType.Anchor_Center)
        {
            m_kUIPrefab.m_kGameObjectRectTransform.anchorMin = new Vector2 (0, 0);
            m_kUIPrefab.m_kGameObjectRectTransform.anchorMax = new Vector2 (1, 1);

        }

        if (m_kUIPrefab.m_kUIType == UILayer.Item)
        {
            m_kUIPrefab.m_kGameObjectRectTransform.anchorMin = new Vector2 (0.5f, 0.5f);
            m_kUIPrefab.m_kGameObjectRectTransform.anchorMax = new Vector2 (0.5f, 0.5f);
        }

        m_kUIPrefab.m_kGameObjectRectTransform.offsetMin = new Vector2 (0, 0);
        m_kUIPrefab.m_kGameObjectRectTransform.offsetMax = new Vector2 (0, 0);
        m_kUIPrefab.m_kGameObjectRectTransform.anchoredPosition3D = new Vector3 (0, 0, 0);
        m_kUIPrefab.m_kGameObjectRectTransform.sizeDelta = new Vector2 (0, 0);
        m_kUIPrefab.m_kGameObjectRectTransform.localEulerAngles = new Vector3 (0, 0, 0);
        m_kUIPrefab.m_kGameObjectRectTransform.localScale = Vector3.one;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

using QTFramework;

using UnityEngine;
using UnityEngine.U2D;

[ObjectEventSystem]
public class UIManagerComponentAwakeSystem : AAwake<UIManagerComponent>
    {
        public override void Awake (UIManagerComponent self)
        {
            self.Awake ( );
        }
    }

[ObjectEventSystem]
public class UIManagerComponentLoadSystem : ALoad<UIManagerComponent>
    {
        public override void Load (UIManagerComponent self)
        {
            self.Load ( );
        }
    }

/// <summary>
/// 管理所有UI
/// </summary>
public class UIManagerComponent : QTComponent
{
    /// <summary>
    /// 所有打开的UI实体对象
    /// </summary>
    private readonly Dictionary<string, UIEntity> m_DicUI_Entity = new Dictionary<string, UIEntity> ( );
    /// <summary>
    /// UI路径对应的UI组件
    /// </summary>
    private readonly Dictionary<string, Type> m_DicUI_Type = new Dictionary<string, Type> ( );
    /// <summary>
    /// UI栈用于恢复UI等
    /// </summary>
    private readonly Stack<string> m_kUIStack = new Stack<string> ( );
    private Queue<UIEntity> uIEntities = new Queue<UIEntity> ( );

    public SpriteAtlas m_kCommonSpriteAtlas;

    public override void Dispose ( )
    {
        if (this.Disposed)
        {
            return;
        }

        base.Dispose ( );

        foreach (string type in m_DicUI_Entity.Keys.ToArray ( ))
        {
            UIEntity ui;
            if (!m_DicUI_Entity.TryGetValue (type, out ui))
            {
                continue;
            }
            m_DicUI_Entity.Remove (type);
            ui.Dispose ( );
        }

        this.m_DicUI_Entity.Clear ( );
    }

    public void Awake ( )
    {
        this.Load ( );

        m_kCommonSpriteAtlas = AssetPoolManager.Instance.Fetch<SpriteAtlas> ("Assets/Data/UI/Atlas/UIAtlas.spriteatlas");
    }

    public void Load ( )
    {
        List<Type> types = EventSystem.Instance.GetTypes ( );

        foreach (Type type in types)
        {
            object [] attrs = type.GetCustomAttributes (typeof (UIEntityComponentAttribute), false);
            if (attrs.Length == 0)
            {
                continue;
            }

            UIEntityComponentAttribute attribute = attrs [0] as UIEntityComponentAttribute;
            if (m_DicUI_Type.ContainsKey (attribute.m_kUIFullPath))
            {
                Log.Debug ($"已经存在同类UI Factory: {attribute.m_kUIFullPath}");
                throw new Exception ($"已经存在同类UI Factory: {attribute.m_kUIFullPath}");
            }
            else
            {
                m_DicUI_Type [attribute.m_kUIFullPath] = type;
            }
        }
    }

    public UIEntity Create(string _UIFullPath)
    {
        UIEntity ui = null;
        m_DicUI_Entity.TryGetValue(_UIFullPath, out ui);
        if (ui == null || ui.m_kUIPrefab.m_kUIType == UILayer.Item)
        {
            ui = QTComponentFactory.Instance.Create<UIEntity, string>(_UIFullPath);
            UIComponent uIComponent = ui.AddComponent(m_DicUI_Type[_UIFullPath]) as UIComponent;
            if (uIComponent != null)
            {
                uIComponent.TranslateUI();
            }
   
            if (ui.m_kUIPrefab.m_kUIType != UILayer.Item)
            {
                if (ui.m_kUIPrefab.m_kUIType == UILayer.Page)
                {
                    var _openUI = m_DicUI_Entity.ToList();

                    string uiPath = "";
                    for (int i = 0; i < _openUI.Count; i++)
                    {
                        if (i == 0)
                        {
                            uiPath += $"{_openUI[i].Key}";
                        }
                        else
                        {
                            uiPath += $"|{_openUI[i].Key}";
                        }
                    }

                    m_kUIStack.Push(uiPath);

                    for (int i = 0; i < _openUI.Count; i++)
                    {
                        if (_openUI[i].Value.m_kUIPrefab.m_kUIType == UILayer.Page || _openUI[i].Value.m_kUIPrefab.m_kUIType == UILayer.PopUpWindow || _openUI[i].Value.m_kUIPrefab.m_kUIType == UILayer.System)
                        {
                            m_DicUI_Entity.Remove(_openUI[i].Key);
                            _openUI[i].Value.Dispose();
                        }

                    }

                }
                RealignmentUISort(ui);
                m_DicUI_Entity.Add(_UIFullPath, ui);
            }
    
        }

        return ui;
    }

    public void Remove (string _UIFullPath)
    {
        bool needShowStack = false;

        UIEntity ui;
        if (!m_DicUI_Entity.TryGetValue (_UIFullPath, out ui))
        {
            return;
        }

        needShowStack = ui.m_kUIPrefab.m_kUIType == UILayer.Page;
        m_DicUI_Entity.Remove (_UIFullPath);
        ui.Dispose ( );

        if (needShowStack && m_kUIStack.Count > 0)
        {
            string PathStr = m_kUIStack.Pop ( );
            if (!string.IsNullOrEmpty (PathStr))
            {
                string [] _uiList = PathStr.Split ('|');
                for (int i = 0; i < _uiList.Length; i++)
                {
                    UIEntity uiTemp = null;
                    m_DicUI_Entity.TryGetValue(_uiList[i], out uiTemp);
                    if(uiTemp != null)
                    {
                        continue;
                    }
                    ui = QTComponentFactory.Instance.Create<UIEntity, string> (_uiList [i]);
                    UIComponent uIComponent = ui.AddComponent (m_DicUI_Type [_uiList [i]]) as UIComponent;
                    if(!m_DicUI_Entity.ContainsKey(_uiList[i]))
                    m_DicUI_Entity.Add (_uiList [i], ui);
                }
            }

        }
        else if (needShowStack && m_kUIStack.Count == 0)
        {
            Create (UI_PrefabPath.m_sUISystem_Hall);
        }
    }

    public void RemoveAll (string _UIFullPath = null)
    {
        var DicUI = m_DicUI_Entity.Keys.ToList();

        foreach (string type in DicUI)
        {
            if (_UIFullPath == type)
            {
                continue;
            }
            UIEntity ui;
            if (!this.m_DicUI_Entity.TryGetValue (type, out ui))
            {
                continue;
            }
            this.m_DicUI_Entity.Remove (type);
            ui.Dispose ( );
        }

        if (m_DicUI_Entity.Count == 0)
        {
            ClearUIStack();
            Create(UI_PrefabPath.m_sUISystem_Hall);
        }
    }

    public UIEntity Get (string type)
    {
        UIEntity ui;
        this.m_DicUI_Entity.TryGetValue (type, out ui);
        return ui;
    }

    public Type GetType(string _UIFullPath)
    {
        if(m_DicUI_Type.Keys.Contains(_UIFullPath))
        {
            return m_DicUI_Type[_UIFullPath];
        }
        return null;
    }

    public List<string> GetUITypeList ( )
    {
        return new List<string> (this.m_DicUI_Entity.Keys);
    }

    /// <summary>
    /// 重新计算各个界面的层级
    /// </summary>
    private void RealignmentUISort (UIEntity _current )
    {

        var _openUI = m_DicUI_Entity.ToList();

        int Page = (int)UILayer.Page;
        int PopUpWindow = (int)UILayer.PopUpWindow;
        int PopUpWidget = (int)UILayer.PopUpWidget;
        int PopUpTop = (int)UILayer.PopUpTop;



        for (int i = 0; i < _openUI.Count; i++)
        {
            if (_openUI[i].Value.m_kUIPrefab.m_kUIType == UILayer.Page)
            {
                _openUI[i].Value.UpdateDepth(Page);
                Page += 2;
            }
            else if (_openUI[i].Value.m_kUIPrefab.m_kUIType == UILayer.PopUpWindow)
            {
                _openUI[i].Value.UpdateDepth(PopUpWindow);
                PopUpWindow += 2;
            }
            else if (_openUI[i].Value.m_kUIPrefab.m_kUIType == UILayer.PopUpWidget)
            {
                _openUI[i].Value.UpdateDepth(PopUpWidget);
                PopUpWidget += 2;
            }
            else if (_openUI[i].Value.m_kUIPrefab.m_kUIType == UILayer.PopUpTop)
            {
                _openUI[i].Value.UpdateDepth(PopUpTop);
                PopUpTop += 2;
            }
        }
        if (_current.m_kUIPrefab.m_kUIType == UILayer.Page)
        {
            _current.UpdateDepth(Page);
        }
        else if (_current.m_kUIPrefab.m_kUIType == UILayer.PopUpWindow)
        {
            _current.UpdateDepth(PopUpWindow);
        }
        else if (_current.m_kUIPrefab.m_kUIType == UILayer.PopUpWidget)
        {
            _current.UpdateDepth(PopUpWidget);
        }
        else if (_current.m_kUIPrefab.m_kUIType == UILayer.PopUpTop)
        {
            _current.UpdateDepth(PopUpTop);
        }
    }

    public void ClearUIStack ( )
    {
        m_kUIStack.Clear ( );
    }
}

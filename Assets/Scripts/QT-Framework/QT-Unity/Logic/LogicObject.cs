/*************************************
* 逻辑对象父类
* 抽象类
* author:SmartCoder
**************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QTFramework;
public abstract class LogicObject : QTEntity 
{
    /// <summary>
    /// 逻辑对象分类
    /// </summary>
    public enum LogicalObjectType
    {
        None = 0,
    }
    /// <summary>
    ///逻辑对象ID
    /// </summary>
    public uint m_uID { get; set; }
    /// <summary>
    ///逻辑对象名称
    /// </summary>
    public string m_sName { get; set; }
    /// <summary>
    /// 逻辑对象类型
    /// </summary>
    public LogicalObjectType m_eLogicalObjectType { get; set; }

}

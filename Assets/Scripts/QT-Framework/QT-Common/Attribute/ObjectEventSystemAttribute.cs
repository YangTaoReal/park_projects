/***********************************************************
 * 特性
 * 事件特性标记
 * author:SmartCoder
 * *********************************************************/

using System;

namespace QTFramework
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ObjectEventSystemAttribute:Attribute
    {
    }
}

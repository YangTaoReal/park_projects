/***********************************************************
 * 组件Update
 * 接口
 * author:SmartCoder
 * *********************************************************/

using System;

namespace QTFramework
{
    public interface IUpdateSystem : IEventSystem
    {
        void Run(object _o);
    }

    public abstract class AUpdate<T> : IUpdateSystem
    {
        public Type Type()
        {
            return typeof(T);
        }
        public void Run(object _o)
        {
            Update((T)_o);
        }
        public abstract void Update(T _self);
    }
}

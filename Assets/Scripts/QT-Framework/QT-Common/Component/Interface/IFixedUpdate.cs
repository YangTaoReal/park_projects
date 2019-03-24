/***********************************************************
 * 组件FixedUpdate
 * 接口
 * author:SmartCoder
 * *********************************************************/

using System;

namespace QTFramework
{
    public interface IFixedUpdateSystem : IEventSystem
    {
        void Run(object _o);
    }

    public abstract class AFixedUpdate<T> : IFixedUpdateSystem
    {
        public Type Type()
        {
            return typeof(T);
        }
        public void Run(object o)
        {
            FixedUpdate((T)o);
        }
        public abstract void FixedUpdate(T self);
    }
}

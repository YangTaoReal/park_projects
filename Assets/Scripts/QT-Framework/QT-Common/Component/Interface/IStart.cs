/***********************************************************
 * 组件Start
 * 接口
 * author:SmartCoder
 * *********************************************************/

using System;

namespace QTFramework
{
    public interface IStartSystem : IEventSystem
    {
        void Run(object _o);
    }

    public abstract class AStart<T> : IStartSystem
    {
        public Type Type()
        {
            return typeof(T);
        }
        public void Run(object o)
        {
            Start((T)o);
        }
        public abstract void Start(T self);
    }
}

/***********************************************************
 * 组件OnDestroy
 * 接口
 * author:SmartCoder
 * *********************************************************/


using System;

namespace QTFramework
{
    public interface IOnDestroySystem : IEventSystem
    {
        void Run(object _o);
    }

    public abstract class AOnDestroy<T> : IOnDestroySystem
    {
        public Type Type()
        {
            return typeof(T);
        }
        public void Run(object _o)
        {
            OnDestroy((T)_o);
        }
        public abstract void OnDestroy(T self);
    }
}

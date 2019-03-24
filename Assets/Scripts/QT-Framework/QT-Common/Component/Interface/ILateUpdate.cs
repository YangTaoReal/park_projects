/***********************************************************
 * 组件LateUpdate
 * 接口
 * author:SmartCoder
 * *********************************************************/

using System;

namespace QTFramework
{
    public interface ILateUpdateSystem : IEventSystem
    {
        void Run(object _o);
    }

    public abstract class ALateUpdate<T> : ILateUpdateSystem
    {
        public Type Type()
        {
            return typeof(T);
        }
        public void Run(object o)
        {
            LateUpdate((T)o);
        }
        public abstract void LateUpdate (T self);
    }
}

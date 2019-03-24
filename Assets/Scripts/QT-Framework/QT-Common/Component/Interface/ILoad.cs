/***********************************************************
 * 组件Load
 * 接口
 * author:SmartCoder
 * *********************************************************/

using System;

namespace QTFramework
{
    public interface ILoadSystem : IEventSystem
    {
        void Run(object _o);
    }

    public abstract class ALoad<T> : ILoadSystem
    {
        public Type Type()
        {
            return typeof(T);
        }
        public void Run(object o)
        {
            Load((T)o);
        }
        public abstract void Load(T self);
    }
}

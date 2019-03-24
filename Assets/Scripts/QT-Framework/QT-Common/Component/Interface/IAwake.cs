/***********************************************************
 * 组件Awake
 * 接口
 * author:SmartCoder
 * *********************************************************/

using System;

namespace QTFramework
{
    public interface IAwakeSystem : IEventSystem
    {
    }
    public interface IAwake
    {
        void Run(object o);
    }
    public interface IAwake<A>
    {
        void Run(object o, A a);
    }
    public interface IAwake<A, B>
    {
        void Run(object o, A a, B b);
    }
    public interface IAwake<A, B, C>
    {
        void Run(object o, A a, B b, C c);
    }
    public abstract class AAwake<T> : IAwakeSystem, IAwake
    {
        public Type Type()
        {
            return typeof(T);
        }
        public void Run(object _o)
        {
            Awake((T)_o);
        }
        public abstract void Awake(T self);
    }
    public abstract class AAwake<T, P1> : IAwakeSystem, IAwake<P1>
    {
        public Type Type()
        {
            return typeof(T);
        }

        public void Run(object _o, P1 _p1)
        {
            Awake((T)_o, _p1);
        }

        public abstract void  Awake(T _self, P1 _p1);
    }
    public abstract class AAwake<T, P1, P2> : IAwakeSystem, IAwake<P1, P2>
    {
        public Type Type()
        {
            return typeof(T);
        }
        public void Run(object _o, P1 _p1, P2 _p2)
        {
            Awake((T)_o, _p1,_p2);
        }
        public abstract void  Awake(T _self, P1 _p1, P2 _p2);
    }
    public abstract class AAwake<T, P1, P2, P3> : IAwakeSystem, IAwake<P1, P2, P3>
    {
        public Type Type()
        {
            return typeof(T);
        }

        public void Run(object _o, P1 _p1, P2 _p2, P3 _p3)
        {
            Awake((T)_o, _p1, _p2, _p3);
        }

        public abstract void Awake(T _self, P1 _p1, P2 _p2, P3 _p3);
    }
}

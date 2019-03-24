/***********************************************************
 * 组件工厂
 * 组件的创建 和 移除
 * author:SmartCoder
 * *********************************************************/

using System;

namespace QTFramework
{
    public class QTComponentFactory : ASingleton<QTComponentFactory>
    {
        public QTComponent Create(Type _type)
        {
            QTComponent _qtComponent = QTComponentPoolManager.Instance.Fetch(_type);
            EventSystem.Instance.Awake(_qtComponent);
            return _qtComponent;
        }
        public QTComponent Create<P1>(Type _type,P1 _p1)
        {
            QTComponent _qtComponent = QTComponentPoolManager.Instance.Fetch(_type);
            EventSystem.Instance.Awake(_qtComponent, _p1);
            return _qtComponent;
        }

        public QTComponent Create< P1, P2>(Type _type,P1 _p1, P2 _p2)
        {
            QTComponent _qtComponent = QTComponentPoolManager.Instance.Fetch(_type);
            EventSystem.Instance.Awake(_qtComponent, _p1, _p2);
            return _qtComponent;
        }

        public QTComponent Create< P1, P2, P3>(Type _type, P1 _p1, P2 _p2, P3 _p3)
        {
            QTComponent _qtComponent = QTComponentPoolManager.Instance.Fetch(_type);
            EventSystem.Instance.Awake(_qtComponent, _p1, _p2, _p3);
            return _qtComponent;
        }
        public QTComponent Create(QTEntity parent,Type type)
        {
            QTComponent _qtComponent = QTComponentPoolManager.Instance.Fetch(type);
            _qtComponent.ParentEntity = parent;
            EventSystem.Instance.Awake(_qtComponent);
            return _qtComponent;
        }

        public  T Create<T>() where T : QTComponent
        {
            T _qtComponent = QTComponentPoolManager.Instance.Fetch<T>();
            EventSystem.Instance.Awake(_qtComponent);
            return _qtComponent;
        }

        public  T Create<T, P1>(P1 _p1) where T : QTComponent
        {
            T _qtComponent = QTComponentPoolManager.Instance.Fetch<T>();
            EventSystem.Instance.Awake(_qtComponent, _p1);
            return _qtComponent;
        }

        public  T Create<T, P1, P2>(P1 _p1, P2 _p2) where T : QTComponent
        {
            T _qtComponent = QTComponentPoolManager.Instance.Fetch<T>();
            EventSystem.Instance.Awake(_qtComponent, _p1, _p2);
            return _qtComponent;
        }

        public  T Create<T, P1, P2, P3>(P1 _p1, P2 _p2, P3 _p3) where T : QTComponent
        {
            T _qtComponent = QTComponentPoolManager.Instance.Fetch<T>();
            EventSystem.Instance.Awake(_qtComponent, _p1, _p2, _p3);
            return _qtComponent;
        }

        public  T Create<T>(QTEntity _parent) where T : QTComponent
        {
            T _qtComponent = QTComponentPoolManager.Instance.Fetch<T>();
            _qtComponent.ParentEntity = _parent;
            EventSystem.Instance.Awake(_qtComponent);
            return _qtComponent;
        }
        public  T Create<T, P1>(QTEntity _parent, P1 _p1) where T : QTComponent
        {
            T _qtComponent = QTComponentPoolManager.Instance.Fetch<T>();
            _qtComponent.ParentEntity = _parent;
            EventSystem.Instance.Awake(_qtComponent, _p1);
            return _qtComponent;
        }

        public  T Create<T, P1, P2>(QTEntity _parent, P1 _p1, P2 _p2) where T : QTComponent
        {
            T _qtComponent = QTComponentPoolManager.Instance.Fetch<T>();
            _qtComponent.ParentEntity = _parent;
            EventSystem.Instance.Awake(_qtComponent, _p1, _p2);
            return _qtComponent;
        }

        public  T Create<T, P1, P2, P3>(QTEntity _parent, P1 _p1, P2 _p2, P3 _p3) where T : QTComponent
        {
            T _qtComponent = QTComponentPoolManager.Instance.Fetch<T>();
            _qtComponent.ParentEntity = _parent;
            EventSystem.Instance.Awake(_qtComponent, _p1, _p2, _p3);
            return _qtComponent;
        }

        public void Remove(QTComponent _qtComponent)
        {
            EventSystem.Instance.Remove(_qtComponent.InstanceId);
            _qtComponent.Dispose();
            QTComponentPoolManager.Instance.Recycle(_qtComponent);
        }
    }
}

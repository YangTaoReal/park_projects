/***********************************************************
 * 实体基类
 * 一切实体对象的基类
 * author:SmartCoder
 * *********************************************************/

using System;
using System.Collections.Generic;
using System.Linq;

namespace QTFramework
{
    public abstract partial class QTEntity :QTComponent
    {
        /// <summary>
        /// 实体下挂在的所有组件
        /// </summary>
        private readonly Dictionary<Type, QTComponent> m_DictComponent = new Dictionary<Type, QTComponent>();

        protected QTEntity ()
        {
            m_DictComponent = new Dictionary<Type, QTComponent>();
        }
        public QTComponent AddComponent(Type type)
        {
            QTComponent component = QTComponentFactory.Instance.Create(this,type);

            if (this.m_DictComponent.ContainsKey(component.GetType()))
            {
                throw new Exception($"AddComponent, component already exist, id: {this.InstanceId}, component: {type.Name}");
            }

            this.m_DictComponent.Add(component.GetType(), component);
            return component;
        }

        public T AddComponent<T>()where T:QTComponent,new()
        {
            T _component = QTComponentFactory.Instance.Create<T>(this);

            if (m_DictComponent.ContainsKey(_component.GetType()))
            {
                throw new Exception($"AddComponent, component already exist, id: {this.InstanceId}, component: {typeof(T).Name}");
            }

            m_DictComponent.Add(_component.GetType(), _component);
            return null;
        }

        public T AddComponent<T,P1>(P1 p1) where T : QTComponent, new()
        {
            T _component = QTComponentFactory.Instance.Create<T, P1>(this, p1);
            AddComponenToDic<T>(_component);
            return _component;
        }

        public T AddComponent<T, P1,P2>(P1 _p1,P2 _p2) where T : QTComponent, new()
        {
            T _component = QTComponentFactory.Instance.Create<T, P1, P2>(this, _p1, _p2);
            AddComponenToDic<T>(_component);
            return _component;
        }
        public T AddComponent<T, P1, P2,P3>(P1 _p1, P2 _p2,P3 _p3) where T : QTComponent, new()
        {
            T _component = QTComponentFactory.Instance.Create<T, P1, P2, P3>(this, _p1, _p2, _p3);
            AddComponenToDic<T>(_component);
            return _component;
        }
        private void AddComponenToDic<T>(T _t) where T : QTComponent, new()
        {
            if (m_DictComponent.ContainsKey(_t.GetType()))
            {
                throw new Exception($"AddComponent, component already exist, id: {this.InstanceId}, component: {typeof(T).Name}");
            }

            m_DictComponent.Add(_t.GetType(), _t);
        }

        public void RemoveComponent<K>() where K : QTComponent
        {
            QTComponent component;
            if (!m_DictComponent.TryGetValue(typeof(K), out component))
            {
                return;
            }

            m_DictComponent.Remove(typeof(K));

            component.Dispose();
        }

        public void RemoveComponent(Type type)
        {
            QTComponent component;
            if (!m_DictComponent.TryGetValue(type, out component))
            {
                return;
            }

            m_DictComponent.Remove(type);
            QTComponentFactory.Instance.Remove(component);
        }

        public K GetComponent<K>() where K : QTComponent
        {
            QTComponent component;
            if (!m_DictComponent.TryGetValue(typeof(K), out component))
            {
                return default(K);
            }
            return (K)component;
        }

        public QTComponent GetComponent(Type type)
        {
            QTComponent component;
            if (!m_DictComponent.TryGetValue(type, out component))
            {
                return null;
            }
            return component;
        }

        public QTComponent[] GetComponents()
        {
            return m_DictComponent.Values.ToArray();
        }


        public override void Dispose()
        {
            if (this.Disposed)
            {
                return;
            }

            base.Dispose();

            var _child = m_DictComponent.ToList();
            while (_child.Count>0)
            {
                RemoveComponent(_child[0].Key);
                _child.RemoveAt(0);
            }
        }


    }
}

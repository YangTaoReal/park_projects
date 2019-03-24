/***********************************************************
 * QTComponent池管理
 * 管理每种QTComponent池
 * author:SmartCoder
 * *********************************************************/

using System;
using System.Collections.Generic;


namespace QTFramework
{
    public class QTComponentPoolManager : ASingleton<QTComponentPoolManager>
    {
        /// <summary>
        /// 所有组件池
        /// </summary>
        private readonly Dictionary<Type, QTComponentPool<QTComponent>> m_DictComponentPool = new Dictionary<Type, QTComponentPool<QTComponent>>();

        public override void Initialize()
        {
            base.Initialize();

        }

        public QTComponent Fetch(Type _type)
        {
            QTComponentPool<QTComponent> _componentPool;

            if (!m_DictComponentPool.TryGetValue(_type, out _componentPool))
            {
                _componentPool = new QTComponentPool<QTComponent>(_type, 1);
                m_DictComponentPool.Add(_type, _componentPool);
            }

            QTComponent obj = null;
            obj = _componentPool.Allocate();
            obj.InstanceId = Guid.NewGuid();
            EventSystem.Instance.AddComponent(obj);

            return obj as QTComponent;
        }


        /// <summary>
        /// 泛型分配
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Fetch<T>() where T : QTComponent
        {
            Type _type = typeof(T);
            QTComponent obj = Fetch(_type);
            return obj as T;
        }

        /// <summary>
        /// 回收
        /// </summary>
        /// <param name="_obj"></param>
        public void Recycle(QTComponent _obj)
        {
            Type _type = _obj.GetType();
            QTComponentPool<QTComponent> _componentPool;
            if (m_DictComponentPool.TryGetValue(_type, out _componentPool))
            {
                _componentPool.Recycle(_obj);
            }
        }
    }
}

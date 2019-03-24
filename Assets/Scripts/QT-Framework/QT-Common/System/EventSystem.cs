using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace QTFramework
{
    public enum AssemblyDLLType
    {
        Model,
        Hotfix,
    }
    public enum EventSystemType
    {
        Load,
        Awake,
        Start,
        Update,
        FixedUpdate,
        LateUpdate,
    }

    public class EventSystem : ASingleton<EventSystem>
    {
        /// <summary>
        /// 所有的程序集
        /// </summary>
        private readonly Dictionary<AssemblyDLLType, Assembly> m_DictAssemblies = new Dictionary<AssemblyDLLType, Assembly>();
        /// <summary>
        /// 所有组件
        /// </summary>
        private readonly Dictionary<Guid, QTComponent> m_kDictAllComponents = new Dictionary<Guid, QTComponent>();
        /// <summary>
        /// 所有事件
        /// </summary>
        private readonly Dictionary<EventSystemType, PoolDictionary<Type, IEventSystem>> m_DictEventSystem = new Dictionary<EventSystemType, PoolDictionary<Type, IEventSystem>>();


        private readonly Queue<Guid> m_QueStarts = new Queue<Guid>();

        private Queue<Guid> m_Queloaders = new Queue<Guid>();
        private Queue<Guid> m_Queloaders2 = new Queue<Guid>();

        private Queue<Guid> m_QueUpdates = new Queue<Guid>();
        private Queue<Guid> m_QueUpdates2 = new Queue<Guid>();

        private Queue<Guid> m_QueFixedUpdate = new Queue<Guid>();
        private Queue<Guid> m_QueFixedUpdate2 = new Queue<Guid>();

        private Queue<Guid> m_QueLateUpdates = new Queue<Guid>();
        private Queue<Guid> m_QueLateUpdates2 = new Queue<Guid>();

        /// <summary>
        /// 单利初始化
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            m_DictEventSystem[EventSystemType.Load] = new PoolDictionary<Type, IEventSystem>();
            m_DictEventSystem[EventSystemType.Awake] = new PoolDictionary<Type, IEventSystem>();
            m_DictEventSystem[EventSystemType.Start] = new PoolDictionary<Type, IEventSystem>();
            m_DictEventSystem[EventSystemType.Update] = new PoolDictionary<Type, IEventSystem>();
            m_DictEventSystem[EventSystemType.FixedUpdate] = new PoolDictionary<Type, IEventSystem>();
            m_DictEventSystem[EventSystemType.LateUpdate] = new PoolDictionary<Type, IEventSystem>();
        }

        /// <summary>
        /// 单利卸载
        /// </summary>
        public override void Uninitialize()
        {
            base.Uninitialize();
        }

        /// <summary>
        /// 添加程序集
        /// </summary>
        /// <param name="_dllType"></param>
        /// <param name="_assembly"></param>
        public void AddAssembly(AssemblyDLLType _dllType, Assembly _assembly)
        {
            m_DictAssemblies[_dllType] = _assembly;
        }

        /// <summary>
        /// 添加完程序集后反射程序集类的
        /// </summary>
        public void ReflectAssembly()
        {

            foreach (var item in m_DictEventSystem)
            {
                item.Value.Clear();
            }

            List<Type> types = new List<Type>();
            foreach (Assembly tempAssembly in this.m_DictAssemblies.Values.ToArray())
            {
                types.AddRange(tempAssembly.GetTypes());
            }

            foreach (Type type in types)
            {
                ScanObjectEventSystemAttribute(type);
            }

            Load();
        }

        public  List<Type> GetTypes()
        {
            List<Type> typeNew = new List<Type>();
            List<Type> types = new List<Type>();
            foreach (Assembly tempAssembly in this.m_DictAssemblies.Values.ToArray())
            {
               
                types.AddRange(tempAssembly.GetTypes());
            }

            foreach (Type type in types)
            {
                typeNew.Add(type);
            }
            return typeNew;
        }
        private void ScanObjectEventSystemAttribute(Type _type)
        {
            object[] attrs = _type.GetCustomAttributes(typeof(ObjectEventSystemAttribute), false);
            if (attrs.Length == 0)
            {
                return;
            }

            object obj = Activator.CreateInstance(_type);


            ILoadSystem _loadSystem = obj as ILoadSystem;
            if (_loadSystem != null)
            {
                m_DictEventSystem[EventSystemType.Load].Add(_loadSystem.Type(), _loadSystem);
            }

            IAwakeSystem _awakeSystem = obj as IAwakeSystem;
            if (_awakeSystem != null)
            {
                m_DictEventSystem[EventSystemType.Awake].Add(_awakeSystem.Type(), _awakeSystem);
            }

            IStartSystem _startSystem = obj as IStartSystem;
            if (_startSystem != null)
            {
                m_DictEventSystem[EventSystemType.Start].Add(_startSystem.Type(), _startSystem);
            }

            IUpdateSystem _updateSystem = obj as IUpdateSystem;
            if (_updateSystem != null)
            {
                m_DictEventSystem[EventSystemType.Update].Add(_updateSystem.Type(), _updateSystem);
            }

            IFixedUpdateSystem _FixedUpdateSystem = obj as IFixedUpdateSystem;
            if (_FixedUpdateSystem != null)
            {
                m_DictEventSystem[EventSystemType.FixedUpdate].Add(_FixedUpdateSystem.Type(), _FixedUpdateSystem);
            }

            ILateUpdateSystem _lateUpdateSystem = obj as ILateUpdateSystem;
            if (_lateUpdateSystem != null)
            {
                m_DictEventSystem[EventSystemType.LateUpdate].Add(_lateUpdateSystem.Type(), _lateUpdateSystem);
            }
        }

        public void AddComponent(QTComponent _component)
        {
            m_kDictAllComponents.Add(_component.InstanceId, _component);


            Type type = _component.GetType();

            if (m_DictEventSystem[EventSystemType.Load].ContainsKey(type))
            {
                m_Queloaders.Enqueue(_component.InstanceId);
            }

            if (m_DictEventSystem[EventSystemType.Start].ContainsKey(type))
            {
                this.m_QueStarts.Enqueue(_component.InstanceId);
            }

            if (m_DictEventSystem[EventSystemType.Update].ContainsKey(type))
            {
                this.m_QueUpdates.Enqueue(_component.InstanceId);
            }

            if (m_DictEventSystem[EventSystemType.FixedUpdate].ContainsKey(type))
            {
                this.m_QueFixedUpdate.Enqueue(_component.InstanceId);
            }
            
            if (m_DictEventSystem[EventSystemType.LateUpdate].ContainsKey(type))
            {
                this.m_QueLateUpdates.Enqueue(_component.InstanceId);
            }

        }

        public void Remove(Guid id)
        {
            m_kDictAllComponents.Remove(id);
        }



        private void Load()
        {
            while (m_Queloaders.Count > 0)
            {
                QTComponent _component;
                Guid _instanceId = m_Queloaders.Dequeue();
                if (!m_kDictAllComponents.TryGetValue(_instanceId, out _component))
                {
                    continue;
                }
                if (_component.Disposed)
                {
                    continue;
                }

                List<IEventSystem> iLoadSystems = m_DictEventSystem[EventSystemType.Load][_component.GetType()];
                if (iLoadSystems == null)
                {
                    continue;
                }

                m_Queloaders2.Enqueue(_instanceId);

                foreach (IEventSystem iLoadSystem in iLoadSystems)
                {
                    try
                    {
                        (iLoadSystem as ILoadSystem).Run(_component);
                    }
                    catch (Exception e)
                    {
                        Log.Error("EventSystem", e);
                    }
                }
            }

            Swap(ref m_Queloaders, ref m_Queloaders2);
        }

        public void Awake(QTComponent component)
        {
            List<IEventSystem> iAwakeSystems = m_DictEventSystem[EventSystemType.Awake][component.GetType()];
            if (iAwakeSystems == null)
            {
                return;
            }

            foreach (IEventSystem aAwakeSystem in iAwakeSystems)
            {
                if (aAwakeSystem == null)
                {
                    continue;
                }

                IAwake iAwake = aAwakeSystem as IAwake;
                if (iAwake == null)
                {
                    continue;
                }

                try
                {
                    iAwake.Run(component);
                }
                catch (Exception e)
                {
                    Log.Error("EventSystem", e);
                }
            }
        }

        public void Awake<P1>(QTComponent component, P1 _p1)
        {
            List<IEventSystem> iAwakeSystems = m_DictEventSystem[EventSystemType.Awake][component.GetType()];
            if (iAwakeSystems == null)
            {
                return;
            }

            foreach (IEventSystem aAwakeSystem in iAwakeSystems)
            {
                if (aAwakeSystem == null)
                {
                    continue;
                }

                IAwake<P1> iAwake = aAwakeSystem as IAwake<P1>;
                if (iAwake == null)
                {
                    continue;
                }

                try
                {
                    iAwake.Run(component, _p1);
                }
                catch (Exception e)
                {
                    Log.Error("EventSystem", e);
                }
            }
        }

        public void Awake<P1, P2>(QTComponent component, P1 _p1, P2 _p2)
        {
            List<IEventSystem> iAwakeSystems = m_DictEventSystem[EventSystemType.Awake][component.GetType()];
            if (iAwakeSystems == null)
            {
                return;
            }

            foreach (IEventSystem aAwakeSystem in iAwakeSystems)
            {
                if (aAwakeSystem == null)
                {
                    continue;
                }

                IAwake<P1, P2> iAwake = aAwakeSystem as IAwake<P1, P2>;
                if (iAwake == null)
                {
                    continue;
                }

                try
                {
                    iAwake.Run(component, _p1, _p2);
                }
                catch (Exception e)
                {
                    Log.Error("EventSystem", e);
                }
            }
        }
        public void Awake<P1, P2, P3>(QTComponent component, P1 _p1, P2 _p2, P3 _p3)
        {
            List<IEventSystem> iAwakeSystems = m_DictEventSystem[EventSystemType.Awake][component.GetType()];
            if (iAwakeSystems == null)
            {
                return;
            }

            foreach (IEventSystem aAwakeSystem in iAwakeSystems)
            {
                if (aAwakeSystem == null)
                {
                    continue;
                }

                IAwake<P1, P2, P3> iAwake = aAwakeSystem as IAwake<P1, P2, P3>;
                if (iAwake == null)
                {
                    continue;
                }

                try
                {
                    iAwake.Run(component, _p1, _p2, _p3);
                }
                catch (Exception e)
                {
                    Log.Error("EventSystem", e);
                }
            }
        }

        public void Start()
        {
            while (m_QueStarts.Count > 0)
            {
                Guid _instanceId = m_QueStarts.Dequeue();
                QTComponent component;
                if (!m_kDictAllComponents.TryGetValue(_instanceId, out component))
                {
                    continue;
                }

                List<IEventSystem> iStartSystems = m_DictEventSystem[EventSystemType.Start][component.GetType()];
                if (iStartSystems == null)
                {
                    continue;
                }

                foreach (IEventSystem iStartSystem in iStartSystems)
                {
                    try
                    {
                        (iStartSystem as IStartSystem).Run(component);
                    }
                    catch (Exception e)
                    {
                        Log.Error("EventSystem", e);
                    }
                }
            }
        }

        public void Update()
        {
            this.Start();


            while (m_QueUpdates.Count > 0)
            {
                Guid instanceId = m_QueUpdates.Dequeue();
                QTComponent _component;
                if (!m_kDictAllComponents.TryGetValue(instanceId, out _component))
                {
                    continue;
                }
                if (_component.Disposed)
                {
                    continue;
                }

                List<IEventSystem> iUpdateSystems = m_DictEventSystem[EventSystemType.Update][_component.GetType()];
                if (iUpdateSystems == null)
                {
                    continue;
                }

                m_QueUpdates2.Enqueue(instanceId);

                foreach (IEventSystem iUpdateSystem in iUpdateSystems)
                {
                    try
                    {
                        (iUpdateSystem as IUpdateSystem).Run(_component);
                    }
                    catch (Exception e)
                    {
                        Log.Error("EventSystem", e);
                    }
                }
            }

            Swap(ref m_QueUpdates, ref m_QueUpdates2);
        }

        public void FixedUpdate()
        {


            while (m_QueFixedUpdate.Count > 0)
            {
                Guid instanceId = m_QueFixedUpdate.Dequeue();
                QTComponent _component;
                if (!m_kDictAllComponents.TryGetValue(instanceId, out _component))
                {
                    continue;
                }
                if (_component.Disposed)
                {
                    continue;
                }

                List<IEventSystem> iFixedUpdateSystems = m_DictEventSystem[EventSystemType.FixedUpdate][_component.GetType()];
                if (iFixedUpdateSystems == null)
                {
                    continue;
                }

                m_QueFixedUpdate2.Enqueue(instanceId);

                foreach (IEventSystem iUpdateSystem in iFixedUpdateSystems)
                {
                    try
                    {
                        (iUpdateSystem as IFixedUpdateSystem).Run(_component);
                    }
                    catch (Exception e)
                    {
                        Log.Error("EventSystem", e);
                    }
                }
            }

            Swap(ref m_QueFixedUpdate, ref m_QueFixedUpdate2);
        }
        public void LateUpdate()
        {
            while (m_QueLateUpdates.Count > 0)
            {
                Guid instanceId = m_QueLateUpdates.Dequeue();
                QTComponent _component;
                if (!m_kDictAllComponents.TryGetValue(instanceId, out _component))
                {
                    continue;
                }
                if (_component.Disposed)
                {
                    continue;
                }

                List<IEventSystem> iLateUpdateSystems = m_DictEventSystem[EventSystemType.LateUpdate][_component.GetType()];
                if (iLateUpdateSystems == null)
                {
                    continue;
                }

                m_QueLateUpdates2.Enqueue(instanceId);

                foreach (IEventSystem iLateUpdateSystem in iLateUpdateSystems)
                {
                    try
                    {
                        (iLateUpdateSystem as ILateUpdateSystem).Run(_component);
                    }
                    catch (Exception e)
                    {
                        Log.Error("EventSystem", e);
                    }
                }
            }

            Swap(ref m_QueLateUpdates, ref m_QueLateUpdates2);
        }

        public static void Swap<T>(ref T t1, ref T t2)
        {
            T t3 = t1;
            t1 = t2;
            t2 = t3;
        }
    }
}

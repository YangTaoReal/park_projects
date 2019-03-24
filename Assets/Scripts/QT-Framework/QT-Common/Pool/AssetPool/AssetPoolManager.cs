/***********************************************
 * 资源管理类
 * 资源预加载、资源分配等
 * author:SmartCoder
 **********************************************/
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace QTFramework
{
    public class AssetPoolManager : AMonoSingleton<AssetPoolManager>
    {
        public override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// 资源池对象
        /// </summary>
        private readonly Dictionary<string, AssetPool<UnityEngine.Object>> m_DictComponentPool = new Dictionary<string, AssetPool<UnityEngine.Object>>();

        /// <summary>
        /// 分配资源
        /// </summary>
        /// <param name="_type"></param>
        /// <returns></returns>
        public UnityEngine.Object Fetch(string _assetName)
        {

            AssetPool<UnityEngine.Object> _assetPool;

            if (!m_DictComponentPool.TryGetValue(_assetName, out _assetPool))
            {
                _assetPool = new AssetPool<UnityEngine.Object>(_assetName,0);
                m_DictComponentPool.Add(_assetName, _assetPool);
            }


            UnityEngine.Object obj = null;
            obj = _assetPool.Allocate();

            if (obj as GameObject)
            {
                GameObject _go = obj as GameObject;
                _go.transform.SetParent(instance.transform);
                _go.transform.position = new Vector3(0, 0, 0);
                _go.SetActive(true);
            }
            return obj;
        }
        /// <summary>
        /// 泛型分配
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Fetch<T>(string _assetName) where T:class
        {
            T t = Fetch(_assetName) as T;
            return t;
        }

        /// <summary>
        /// 回收
        /// </summary>
        /// <param name="_obj"></param>
        public void Recycle(UnityEngine.Object _obj)
        {
            if (_obj as GameObject)
            {
                GameObject _go = _obj as GameObject;
                _go.transform.SetParent(instance.transform);
                _go.SetActive(false);
                _go.transform.localPosition = Vector3.zero;
                _go.transform.localScale = Vector3.one;
                _go.transform.localEulerAngles = Vector3.zero;
            }

            string[] kObjNameArray = _obj.name.Split('|');
            if (kObjNameArray.Length < 2)
            {
                Log.Info("AssetPoolManager", $"Instance have error name  : {_obj.name}");
                return;
            }
            AssetPool<UnityEngine.Object> _componentPool;

            if (m_DictComponentPool.TryGetValue(kObjNameArray[0], out _componentPool))
            {      
                _componentPool.Recycle(_obj);
            }
            else
            {
                Log.Info("AssetPoolManager", $"Instance have error name  : {_obj.name}");
            }
        }

        public void PreLoad(string _assetName, int _count)
        {
            AssetPool<UnityEngine.Object> _assetPool;

            if (!m_DictComponentPool.TryGetValue(_assetName, out _assetPool))
            {
                _assetPool = new AssetPool<UnityEngine.Object>(_assetName, _count);
                m_DictComponentPool.Add(_assetName, _assetPool);
            }
        }

    }
}

/***********************************************************
 * 单例工厂
 * 单例类的创建  初始化  和 卸载
 * author:SmartCoder
 * *********************************************************/

using System;
using UnityEngine;

namespace QTFramework
{
    public sealed class SingletonFactory
    {
        /// <summary>
        /// 创建普通类单例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T CreateSingleton<T>() where T : ISingleton
        {
            T _t = Activator.CreateInstance<T>();
            return _t;
        }

        /// <summary>
        /// 创建mono单例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T CreateMonoSingleton<T>() where T : MonoBehaviour, ISingleton
        {
            T instance = null;
            instance = GameObject.FindObjectOfType(typeof(T)) as T;
            if (instance == null)
            {
                instance = new GameObject(typeof(T).Name).AddComponent<T>();
                instance.transform.localPosition = new Vector3(0, 0, 1000000);
            }
            return instance;
        }



        public static void Init()
        {
            Log.Info("SingletonFactory", "初始化所有的单利类");

            EventSystem.Instance.Initialize();
            QTComponentPoolManager.Instance.Initialize();
            DBManager.Instance.Initialize();
            AssetPoolManager.Instance.Initialize();
        }


        public static void UpdateLogic()
        {
         
        }

        public static void Uninit()
        {
            Log.Info("SingletonFactory", "卸载所有的单利类");

            EventSystem.Instance.Uninitialize();
            QTComponentPoolManager.Instance.Uninitialize();
            DBManager.Instance.Uninitialize();
            AssetPoolManager.Instance.Uninitialize();
        }
    }
}
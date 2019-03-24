/*************************************
* 游戏启动入口
* author:SmartCoder
**************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace QTFramework
{
    public class AppEditor : MonoBehaviour
    {
        
    }



#if UNITY_EDITOR
    [CustomEditor(typeof(AppEditor))]
    public class AppEditorEditor : Editor
    {

        private void OnEnable()
        {
            AppEditor appEditor = (AppEditor)target;
            Debug.Log("appEditor.LaunchGame()");

            SingletonFactory.Init();

            Log.Info("App", "加载程序集...");
            EventSystem.Instance.AddAssembly(AssemblyDLLType.Model, typeof(App).Assembly);
            Log.Info("App", "开始反射程序集...");
            EventSystem.Instance.ReflectAssembly();

            //=====================本地化管理==========================
            World.Scene.AddComponent<LocalizationComponent>();
            //=====================时间管理============================
            World.Scene.AddComponent<TimeComponent>();
            //=====================场景加载管理========================
            World.Scene.AddComponent<SceneManagerComponent>();
            //=====================界面管理============================
            World.Scene.AddComponent<UIManagerComponent>();
            //=====================音效管理============================
            World.Scene.AddComponent<AudioManagerComponent>();
            //=====================世界对象管理========================
            World.Scene.AddComponent<WorldManagerComponent, GameObject>(appEditor.gameObject);
            //=====================特效管理===========================
            World.Scene.AddComponent<EffectManagerComponent>();
            //=====================模型管理===========================
            World.Scene.AddComponent<ModelManager>();

            EditorApplication.update += Update;
            EditorApplication.update += FixedUpdate;
            EditorApplication.update += LateUpdate;

        }


        private void Update()
        {

            EventSystem.Instance.Update();
        }

        private void FixedUpdate()
        {
            EventSystem.Instance.FixedUpdate();
            SingletonFactory.UpdateLogic();
        }


        private void LateUpdate()
        {
            EventSystem.Instance.LateUpdate();
        }

        private void OnApplicationQuit()
        {

        }  

    }
#endif
}


/*************************************
* 游戏启动入口
* author:SmartCoder
**************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BE;
using Facebook.Unity;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace QTFramework
{
    public class App : AMonoSingleton<App>
    {
        private readonly OneThreadSynchronizationContext contex = new OneThreadSynchronizationContext();
        UI_Launch uI_Launch;
        private DateTime onLineTime;
        private  void Start()
        {
            Application.targetFrameRate = 60;
            Log.Info("App", "游戏启动...");
            InitSDK();
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Application.runInBackground = true;
            SynchronizationContext.SetSynchronizationContext(this.contex);
            DontDestroyOnLoad(gameObject);
            uI_Launch = this.GetComponentInChildren<UI_Launch>(true);
            uI_Launch.m_kSlider_Progress.gameObject.SetActive(true);
            uI_Launch.m_kTransformBegin.gameObject.SetActive(false);
            uI_Launch.m_kButton_Beain.enabled = false;
            StartCoroutine(CheckHotUpdate());
        }

        private IEnumerator CheckHotUpdate()
        {       
            uI_Launch.gameObject.SetActive(true);
            for (int i = 0; i < 10; i++)
            {
                uI_Launch.SetSlider(i / 10f);
                yield return new WaitForEndOfFrame();
            }
            Log.Info("App", "准备游戏...");
            LaunchGame();
        }

        private static string GetStreamingAssetsPath()
        {
            if (Application.isEditor)
                return "file://" + System.Environment.CurrentDirectory.Replace("\\", "/"); // Use the build output folder directly.
            else if (Application.isMobilePlatform || Application.isConsolePlatform)
                return Application.streamingAssetsPath;
            else // For standalone player.
                return "file://" + Application.streamingAssetsPath;
        }
        private void LaunchGame()
        {
            SingletonFactory.Init();

            Log.Info("App", "加载程序集...");
            EventSystem.Instance.AddAssembly(AssemblyDLLType.Model, typeof(App).Assembly);
            Log.Info("App", "开始反射程序集...");
            EventSystem.Instance.ReflectAssembly();
            World.Init();
            //=====================本地化管理==========================
            World.Scene.AddComponent<LocalizationComponent>();
            //=====================事件管理===========================
            World.Scene.AddComponent<GameEventManager>();
            //=====================时间管理============================
            World.Scene.AddComponent<TimeComponent>();
            //=====================场景加载管理========================
            World.Scene.AddComponent<SceneManagerComponent>();
            //=====================界面管理============================
            World.Scene.AddComponent<UIManagerComponent>();
            //=====================音效管理============================
            World.Scene.AddComponent<AudioManagerComponent>();
            //=====================世界对象管理========================
            World.Scene.AddComponent<WorldManagerComponent, GameObject>(gameObject);
            //=====================特效管理===========================
            World.Scene.AddComponent<EffectManagerComponent>();
            //=====================模型管理===========================
            World.Scene.AddComponent<ModelManager>();
            //=====================数据管理===========================
            World.Scene.AddComponent<DataManager>();
            //=====================Buff管理===========================
            World.Scene.AddComponent<BuffManager>();
            //=====================角色===========================
            World.Scene.AddComponent<PlayerManagerComponent>();
            //=====================对话系统管理===========================
            World.Scene.AddComponent<DialogueManagerComponent>();
            //=====================新手引导===========================
            World.Scene.AddComponent<GuidanceManager>();
            //=====================游戏状态机===========================
            World.Scene.AddComponent<GameStateMachineComponent>();
            //=====================NPC管理器===========================
            World.Scene.AddComponent<NPCManager>();
           
            uI_Launch.m_kSlider_Progress.gameObject.SetActive(false);
            uI_Launch.m_kTransformBegin.gameObject.SetActive(true);
            uI_Launch.m_kButton_Beain.enabled = true;
            World.Scene.GetComponent<AudioManagerComponent>().PlayAudio(AudioChannel.AudioChannelType.BGM, DBManager.Instance.m_kAudio.GetEntryPtr(100000)._AudioPath, true);
            if (MyDebug.isShowLog)
            {
                MyDebug.CreateInstance();
            }
        }


        private void Update()
        {
            this.contex.Update();
            EventSystem.Instance.Update();

            KeyEvent();
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
            DBManager.Instance.Dispose();
            CalculateTotalGameTime();
            // 监测退出游戏
            GameEventManager._Instance.onQuiteGame();
            Debug.Log("PC关闭");
        }

        private void OnApplicationFocus(bool focus)
        {
            if(focus)
            {
                onLineTime = DateTime.Now;
            }
            else
            {
                CalculateTotalGameTime();
                // 失去焦点也判定为退出游戏
                GameEventManager._Instance.onQuiteGame();
            }
        }

        // 计算在线时间
        public void CalculateTotalGameTime()
        {
            TimeSpan span;
            if (World.Scene.GetComponent<PlayerManagerComponent>() != null)
            {
                World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.m_kPlayerBasicAsset.m_OnLineTime = onLineTime;
                World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.m_kPlayerBasicAsset.m_OfflineTime = DateTime.Now;
                span = World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.m_kPlayerBasicAsset.m_OfflineTime -
                                      World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.m_kPlayerBasicAsset.m_OnLineTime;
                float beforTime = World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.m_kPlayerBasicAsset.m_ToatlGameTime;
                World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.m_kPlayerBasicAsset.m_ToatlGameTime += span.Seconds;
                Debug.Log($"之前累计在线时间{beforTime}目前为止累计在线游戏时间:{World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.m_kPlayerBasicAsset.m_ToatlGameTime}");
            }
            foreach (var item in GameEventManager._Instance.mGameEventData.RecordDic)
            {
                // 如果事件在注册列表里面 就要记录它的在线时间
                if(GameEventManager._Instance.mGameEventData.GameEventDic.ContainsKey(item.Key))
                {
                    item.Value.registerOnlineTime += span.Seconds;
                }
            }
        }

        public class BuildingServerA
        {
            public string guid;
            public string BeginTime;
            public BuildState buildState;//建造状态
            public int lv;//等级
            public string father_guid;
            public int cfg_id;
        }

        void SaveTT(string file_name, object obj)
        {
        
     
            string file_path = Json.pathDataInfo + file_name + ".json";
 

     
 
            using (FileStream streamA = new FileStream(file_path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
            {
                int fsLen = (int)streamA.Length;
                byte[] heByte = new byte[fsLen];
                int r = streamA.Read(heByte, 0, heByte.Length);
                string myStr = System.Text.Encoding.UTF8.GetString(heByte);
              
                Debug.Log("(*&(*&~~~~~" + myStr);
            } 


            FileStream stream = new FileStream(file_path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
                                    
            string strJson = JsonUtility.ToJson(obj);
            byte[] bytes = Encoding.UTF8.GetBytes(strJson);
            Debug.Log("-~~~~~~~~" + strJson);

            stream.BeginWrite(bytes, 0, bytes.Length, new AsyncCallback(SaveTTEnd), stream);

            //string strFilePath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, @"文件路径\test.txt");
            //if (File.Exists(strFilePath))
            //{
            //    string strContent = File.ReadAllText(strFilePath);
            //    strContent = Regex.Replace(strContent, "将要被修改的内容", "修改后的内容");
            //    File.WriteAllText(strFilePath, strContent);
            //}

           
       
             
             
       
        }
        static void SaveTTEnd(IAsyncResult asr)
        {
            using (FileStream file = (FileStream)asr.AsyncState)
            {
                string[] sArray = file.Name.Split(new char[2] { '/', '.' });
 

                file.EndWrite(asr);
                file.Dispose();
                file.Close();
       
            }
        }

        void KeyEvent()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                Debug.Log("按下了Q键");
                World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.AddGold(1000000);
                World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.AddWater(999);
                //BuildingServerA serverA = new BuildingServerA();
                //serverA.BeginTime = DateTime.Now.ToString();
                //serverA.buildState = BuildState.Sustain;
                //serverA.lv = 1;
                //serverA.guid = Guid.NewGuid().ToString();
                //serverA.father_guid = Guid.NewGuid().ToString();
                //serverA.cfg_id = 10000;


                //SaveTT("BuildingServerA", serverA);




            }

      



            if (Input.GetKeyDown(KeyCode.W))
            {
                Debug.Log("按下了W键");
                PlayerPrefs.SetInt("isFirstOpenGame", 1);
                PlayerPrefs.SetInt("isDialoguing", 0);
                //PlayerPrefs.SetInt("isGuidancing", 0);

                //PlayerPrefs.SetInt("currGuidanceStep", 1);
                //PlayerPrefs.SetInt(GuidanceEvent.GreenHandEvent.ToString(), 1);

                //BuildingServerA serverA = new BuildingServerA();
                //serverA.BeginTime = DateTime.Now.ToString();
                //serverA.buildState = BuildState.Sustain;
                //serverA.lv = 1;
                //serverA.guid = Guid.NewGuid().ToString();
                //serverA.father_guid = Guid.NewGuid().ToString();
                //serverA.cfg_id = 10000;
            }

        }

        void InitSDK()
        {
            if (!FB.IsInitialized)
            {
                Debug.Log("初始化Facebook");
                FB.Init(InitCallback, OnHideUnity);
            }
            else
            {
                Debug.Log("已经初始化Facebook");
                FB.ActivateApp();
            }
        }
        private void InitCallback()
        {
            if (FB.IsInitialized)
            {
                Debug.Log("初始化Facebook成功");
                FB.ActivateApp();
            }
            else
            {
                Debug.Log("Failed to Initialize the Facebook SDK");
            }
        }

        private void OnHideUnity(bool isGameShown)
        {
            if (!isGameShown)
            {
                // Pause the game - we will need to hide
                Time.timeScale = 0;
            }
            else
            {
                // Resume the game - we're getting focus again
                Time.timeScale = 1;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

    public class SystemMonitor: MonoBehaviour
    {
        private static SystemMonitor mInstance = null;
        private double updateInterval = 0.5;
        private double lastInterval; // Last interval end time
        private int frames = 0; // Frames over current interval
        public double fps; // Current FPS

        private double lowestFrames = 99f;
        private float lastCalTime = 0f;

        public bool isShowSys = false;

		public float usedTime = 0;


        public Resolution[] resolutions;
        public static SystemMonitor GetInstance()
        {
            if (mInstance == null)
            {
                GameObject goInstance = GameObject.Find("_Debug");
				if (goInstance == null)
				{
                    goInstance = new GameObject("_Debug");
					UnityEngine.Object.DontDestroyOnLoad(goInstance);
				}
				mInstance = goInstance.AddComponent<SystemMonitor>();
            }
            return mInstance; 
        }
        private void Start()
        {
            resolutions = Screen.resolutions;
            float nowtime = Time.realtimeSinceStartup;

            lastInterval = nowtime;
            frames = 0;

			//Screen.SetResolution(960, 640, false, 60);//设置屏幕分辨率的大小和频率,
        }
        private void Update()
		{
            float nowtime = Time.realtimeSinceStartup;

            ++frames;
            double timeNow = nowtime;
            if (timeNow > lastInterval + updateInterval)
            {
                fps = frames / (timeNow - lastInterval);
                frames = 0;
                lastInterval = timeNow;
            }
        }

		private void OnGUI_todo()
		{
			GUILayout.Label("dd");
		}
		private void OnGUI()
        {
			GUILayout.Space(0);
            GUILayout.BeginHorizontal();
			string showStr = "C";
            if(isShowSys==false)
            {
                showStr = "O";
            }

			if (GUILayout.Button(showStr, GUILayout.Width(23), GUILayout.Height(23)))
			{
				isShowSys = !isShowSys;
			}

            GUIStyle bb=GUI.skin.box;
            bb.richText = true;
            bb.alignment = TextAnchor.MiddleLeft;
            bb.fontSize = 14;

            if(isShowSys)
            {
                string strSM = "";

                strSM += string.Format("分辨率:<color=#00ffffff>{0}x{1}</color>,", Screen.width, Screen.height);
                //strSM += string.Format("帧率:<color=#00ffffff>{0}</color>,", fps.ToString("f2"));
                strSM += string.Format("内存:<color=#00ffffff>{0}</color>,", SystemInfo.systemMemorySize);
                strSM += string.Format("显存:<color=#00ffffff>{0}</color>,", SystemInfo.graphicsMemorySize);
                strSM += string.Format("deltaTime:<color=#00ffffff>{0}</color>,", Time.deltaTime.ToString("f4"));
                strSM += string.Format("游戏运行时间:<color=#00ffffff>{0}</color>,", Time.realtimeSinceStartup.ToString("f1"));
                
                strSM += string.Format("设备唯一id:<color=#ff00ffff>{0}</color>\n", SystemInfo.deviceUniqueIdentifier);

                strSM += string.Format("显卡:<color=#ffff00ff>{0}</color>,", SystemInfo.graphicsDeviceName);
                strSM += string.Format("显卡版本:<color=#ffff00ff>{0}</color>,", SystemInfo.graphicsDeviceVersion);
                strSM += string.Format("Shader版本:<color=#ffff00ff>{0}</color>\n", SystemInfo.graphicsShaderLevel);

                strSM += string.Format("操作系统:<color=#3ca5acff>{0}</color>\n", SystemInfo.operatingSystem);

                strSM += string.Format("CPU架构:<color=#3b9fe5ff>{0}</color>,", SystemInfo.processorType);
                strSM += string.Format("CPU核心数:<color=#3b9fe5ff>{0}</color>\n", SystemInfo.processorCount);

                strSM += string.Format("持久保存路径:<color=#8b7decff>{0}</color>\n", Application.persistentDataPath);
                strSM += string.Format("内部资源路径:<color=#8b7decff>{0}</color>\n", Application.streamingAssetsPath);

                //strSM += string.Format("ip:<color=#96aa2a>{0}</color>,", Network.player.ipAddress);
                //strSM += string.Format("内存占用:<color=#87ba66>{0}</color>", GC.GetTotalMemory(true));

                // 所有对象的内存占用
                #if ENABLE_PROFILER
                int byteCount = 0;
                int objCount = 0;
                UnityEngine.Object[] allObject = UnityEngine.Resources.FindObjectsOfTypeAll(typeof(UnityEngine.Object));
                foreach(UnityEngine.Object obj in allObject)
                {
                    byteCount += UnityEngine.Profiling.Profiler.GetRuntimeMemorySize(obj);
                    objCount++;
                }
				strSM += string.Format("对象数量:<color=#6F60D0>{0}</color>,", objCount);
                strSM += string.Format("内存占用:<color=#87ba66>{0:F2}MB</color>,", byteCount/1024.0f/1024.0f);

                uint monoHeap = UnityEngine.Profiling.Profiler.GetMonoHeapSize();
                strSM += string.Format("分配Mono堆:<color=#87ba66>{0:F2}MB</color>,", monoHeap / 1024.0f / 1024.0f);

                uint monoUsedHeap = UnityEngine.Profiling.Profiler.GetMonoUsedSize();
                strSM += string.Format("Mono使用内存:<color=#87ba66>{0:F2}MB</color>,", monoUsedHeap / 1024.0f / 1024.0f);

				

                #endif


                //////////////////////////////////////////////////////////////////////////
                // 打印所有对象
                //UnityEngine.Object[] objects = FindObjectsOfType(typeof(UnityEngine.Object));

                //Dictionary<string, int> dictionary = new Dictionary<string, int>();

                //foreach(UnityEngine.Object obj in objects)
                //{
                //    string key = obj.GetType().ToString();
                //    if(dictionary.ContainsKey(key))
                //    {
                //        dictionary[key]++;
                //    }
                //    else
                //    {
                //        dictionary[key] = 1;
                //    }
                //}

                //List<KeyValuePair<string, int>> myList = new List<KeyValuePair<string, int>>(dictionary);
                //myList.Sort(
                //    delegate(KeyValuePair<string, int> firstPair,
                //    KeyValuePair<string, int> nextPair)
                //    {
                //        return nextPair.Value.CompareTo((firstPair.Value));
                //    }
                //);

                //foreach(KeyValuePair<string, int> entry in myList)
                //{
                //    //GUILayout.Label(entry.Key + ": " + entry.Value);
                //    strSM += entry.Key + ": " + entry.Value + "\n";
                //}



				GUILayout.Label(strSM, bb);


				//for(int l=0; l < resolutions.Length; l++)
				//{
				//	GUI.Label(new Rect(200, 200 + 40 * l, 300, 50), resolutions[l].width.ToString() + "    " + resolutions[l].height.ToString());//输出所支持的屏幕分辨率的大小,
				//}

                //GUILayout.Label(String.Format("{0}x{1}", Screen.width, Screen.height) + ", " +
                //    String.Format("fps:{0}", fps.ToString("f2")) + ", " +
                //    String.Format("memory :{0}", SystemInfo.systemMemorySize) + ", " +
                //    String.Format("grp mem:{0}", SystemInfo.graphicsMemorySize));
            }

            //if(GUILayout.Button("GC", GUILayout.Width(50), GUILayout.Height(50)))
            //{
            //    //Resources.UnloadUnusedAssets();
            //    //GC.Collect();

            //    int count = ResourceManager.GetInstance().dictResources.Count;
            //    Core.Unity.Debug.Log("dictResources.Count:" + count);
            //    foreach(KeyValuePair<string, AssetBundleCreateRequest> res in ResourceManager.GetInstance().dictResources)
            //    {
            //        AssetBundleCreateRequest req = res.Value;
            //        if(req != null && req.isDone)
            //        {
            //            req.assetBundle.Unload(true);
            //            ResourceManager.GetInstance().dictResources[res.Key] = null;
            //        }
            //    }
            //    Resources.UnloadUnusedAssets();
            //    GC.Collect();
            //}

            bb.fontSize = 12;
			float nowTime = Time.realtimeSinceStartup;
			if(nowTime - lastCalTime > 10f)
			{
				lastCalTime = nowTime;
				lowestFrames = 99f;
			}

			// 最小帧数
			if(fps < lowestFrames)
			{
				lowestFrames = fps;
				lastCalTime = nowTime;
			}
			GUILayout.Label(string.Format("FPS:<color=#00ffffff>{0}</color>(<color=#ff0000ff>{1}</color>)", fps.ToString("f2"), lowestFrames.ToString("f2")), bb);

			bb.fontSize = 14;

            GUILayout.EndHorizontal();
        }
    }

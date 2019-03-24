/*************************************
 * DB3  本地管理器
 * author:SmartCoder
 **************************************/
using System;
using System.IO;

using Mono.Data.Sqlite;

using QTFramework;

using UnityEngine;

public class DBManager : ASingleton<DBManager>
    {
        private string m_kDBFile = Application.streamingAssetsPath + "/GameData/GameDB.db3";
        private SqliteConnection m_kConnection;

        public readonly CS_Model m_kModel = new CS_Model ( );
        public readonly CS_MapInfo m_kMapInfo = new CS_MapInfo ( );
        public readonly CS_Effect m_kEffect = new CS_Effect ( );
        public readonly CS_Items m_kItems = new CS_Items ( );
        public readonly CS_Shop m_kShop = new CS_Shop ( );
        public readonly CS_UpLevel m_kUpLv = new CS_UpLevel ( );
        public readonly CS_InOutPut m_kInOutPut = new CS_InOutPut ( );
        public readonly CS_Language m_kLanguage = new CS_Language ( );
        public readonly CS_Buff m_kBuff = new CS_Buff ( );
        public readonly CS_Disperse m_kDisperse = new CS_Disperse ( );
        public readonly CS_Guidance m_kGuidance = new CS_Guidance ( );
        public readonly CS_AssetBundleTable m_kAssetBundleTable = new CS_AssetBundleTable ( );
        public readonly CS_Task m_kTask = new CS_Task ( );
        public readonly CS_Initialize m_kInitialize = new CS_Initialize ( );
        public readonly CS_Audio m_kAudio = new CS_Audio();
        public readonly CS_DropList m_kDropList = new CS_DropList();
        public readonly CS_GameEvent m_kGameEvent = new CS_GameEvent();
        public readonly CS_NPC m_kNPC = new CS_NPC();

        public void Dispose ( )
        {
            m_kConnection.Dispose ( );
        }

#region ------单例初始化-------
        /// <summary>
        /// 单例初始化
        /// </summary>
        public override void Initialize ( )
        {
            base.Initialize ( );
            LoadDB3 ( );
            m_kConnection.Open ( );
            m_kModel.Init ( );
            m_kMapInfo.Init ( );
            m_kEffect.Init ( );
            m_kItems.Init ( );
            m_kShop.Init ( );
            m_kUpLv.Init ( );
            m_kInOutPut.Init ( );
            m_kLanguage.Init ( );
            m_kBuff.Init ( );
            m_kDisperse.Init ( );
            m_kGuidance.Init ( );
            m_kAssetBundleTable.Init ( );
            m_kTask.Init ( );
            m_kInitialize.Init ( );
            m_kAudio.Init();
            m_kDropList.Init();
            m_kGameEvent.Init();
            m_kNPC.Init();
            m_kConnection.Close ( );
           
 
        }
#endregion

#region ------执行数据库语句-------
        /// <summary>
        /// 执行语句
        /// </summary>
        /// <param name="sqlQuery"></param>
        /// <returns></returns>
        public SqliteDataReader Query (string _sqlQuery)
        {
            try
            {
                SqliteCommand cmd = new SqliteCommand (_sqlQuery, m_kConnection);
                return cmd.ExecuteReader ( );
            }
            catch (Exception e)
            {
                Log.Error ("DBManager", e);
                return null;
            }
        }
#endregion

#region ------加载DB3-------
        /// <summary>
        /// 加载DB3
        /// </summary>
        private void LoadDB3 ( )
        {
#if  UNITY_EDITOR
            LoadDB3_Editor ( );
#elif UNITY_IPHONE && !UNITY_EDITOR
            LoadDB3_IOS ( );
#elif UNITY_ANDROID && !UNITY_EDITOR
            LoadDB3_Android ( );
#endif

        }
#endregion

#region ------编辑器下加载DB3-------
        /// <summary>
        /// 编辑器下加载DB3
        /// </summary>
        private void LoadDB3_Editor ( )
        {
            m_kConnection = new SqliteConnection (@"Data Source = " + m_kDBFile + "; " + "Version=3; connection = new ");
        }
#endregion

#region ------IOS加载DB3-------
        /// <summary>
        /// IOS加载DB3
        /// </summary>
        private void LoadDB3_IOS ( )
        {
            string kPath = Application.dataPath + "/Raw/GameData/GameDB.db3";
            m_kConnection = new SqliteConnection (@"Data Source = " + kPath + "; " + "Version=3; connection = new ");
        }
#endregion

#region ------安卓加载db3-------
        /// <summary>
        /// 安卓加载db3
        /// </summary>
        private void LoadDB3_Android ( )
        {
            m_kDBFile = Application.persistentDataPath + "/StreamingAssets/GameData/GameDB.db3";
            if (File.Exists (m_kDBFile))
            {}
            else
            {
                if (!Directory.Exists (m_kDBFile.Substring (0, m_kDBFile.LastIndexOf ("/"))))
                {
                    Directory.CreateDirectory (m_kDBFile.Substring (0, m_kDBFile.LastIndexOf ("/")));
                }
                float fStarTime = Time.realtimeSinceStartup;
                WWW m_kDBLoader = new WWW ("jar:file://" + Application.dataPath + "!/assets/GameData/" + "GameDB.db3");
                while (!m_kDBLoader.isDone)
                {
                    continue;
                }
                fStarTime = Time.realtimeSinceStartup;

                if (m_kDBLoader.error != null)
                {
                    m_kDBLoader.Dispose ( );
                }
                else
                {
                    System.IO.File.WriteAllBytes (m_kDBFile, m_kDBLoader.bytes);
                    fStarTime = Time.realtimeSinceStartup;
                    m_kDBLoader.Dispose ( );
                }
            }
            m_kConnection = new SqliteConnection (@"Data Source = " + m_kDBFile + "; " + "Version=3; connection = new ");
        }
#endregion
    }

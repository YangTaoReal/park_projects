using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mono.Data.Sqlite;
public class CS_Guidance
{
    public class DataEntry
    {
        public System.Int32 _ID = 0;
        public System.Int32 _GuidanceType = 0;
        public System.Int32 _LanID = 0;
        public System.Int32 _Step = 0;
        public System.Int32 _GuidanceTextPos = 0;
        public System.Int32 _EndType = 0;
        public System.Int32 _Shape = 0;
        public Vector3 _Size = Vector3.zero;
        public System.Int32 _EventType = 0;
        public System.Boolean _isShowFinger = false;
        public System.Boolean _isUI = false;
        public System.Boolean _isMask = false;
    }
    public Dictionary<System.Int32, DataEntry> m_kDataEntryTable = new Dictionary<System.Int32, DataEntry>();
    public void Init()
    {
        m_kDataEntryTable.Clear();
        System.String kSqlCMD = "SELECT * FROM Guidance";
        m_kDataEntryTable.Clear();
        SqliteDataReader kDataReader = DBManager.Instance.Query(kSqlCMD);
        string[] v3int = null;
        while (kDataReader.HasRows && kDataReader.Read())
        {
            DataEntry kNewEntry = new DataEntry();
            kNewEntry._ID = kDataReader.GetInt32(0);
            kNewEntry._GuidanceType = kDataReader.GetInt32(1);
            kNewEntry._LanID = kDataReader.GetInt32(2);
            kNewEntry._Step = kDataReader.GetInt32(3);
            kNewEntry._GuidanceTextPos = kDataReader.GetInt32(4);
            kNewEntry._EndType = kDataReader.GetInt32(5);
            kNewEntry._Shape = kDataReader.GetInt32(6);
            v3int = kDataReader.GetString(7).Split(' ');
            kNewEntry._Size = new Vector3(float.Parse(v3int[0]), float.Parse(v3int[1]), float.Parse(v3int[2]));
            kNewEntry._EventType = kDataReader.GetInt32(8);
            kNewEntry._isShowFinger = kDataReader.GetBoolean(9);
            kNewEntry._isUI = kDataReader.GetBoolean(10);
            kNewEntry._isMask = kDataReader.GetBoolean(11);
            m_kDataEntryTable[kNewEntry._ID] = kNewEntry;
        }
        kDataReader.Close();
    }
    public DataEntry GetEntryPtr(System.Int32 _ID)
    {
        if (m_kDataEntryTable.ContainsKey(_ID))
        {
            return m_kDataEntryTable[_ID];
        }
        return null;
    }
    public bool ContainsID(System.Int32 _ID)
    {
        return m_kDataEntryTable.ContainsKey(_ID);
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mono.Data.Sqlite;
public class CS_GameEvent
{
    public class DataEntry
    {
        public System.Int32 _ID = 0;
        public System.String _Mark = "";
        public System.Int32 _ConditionType = 0;
        public Vector3 _ConditionValue = Vector3.zero;
        public System.Int32 _ResultType = 0;
        public Vector3 _ResultValue = Vector3.zero;
        public System.Int32 _CheckType = 0;
        public System.Int32 _RegType = 0;
        public System.Int32 _Count = 0;
        public System.Int32 _DestoryType = 0;
    }
    public Dictionary<System.Int32, DataEntry> m_kDataEntryTable = new Dictionary<System.Int32, DataEntry>();
    public void Init()
    {
        m_kDataEntryTable.Clear();
        System.String kSqlCMD = "SELECT * FROM GameEvent";
        m_kDataEntryTable.Clear();
        SqliteDataReader kDataReader = DBManager.Instance.Query(kSqlCMD);
        string[] v3int = null;
        while (kDataReader.HasRows && kDataReader.Read())
        {
            DataEntry kNewEntry = new DataEntry();
            kNewEntry._ID = kDataReader.GetInt32(0);
            kNewEntry._Mark = kDataReader.GetString(1);
            kNewEntry._ConditionType = kDataReader.GetInt32(2);
            v3int = kDataReader.GetString(3).Split(' ');
            kNewEntry._ConditionValue = new Vector3(float.Parse(v3int[0]), float.Parse(v3int[1]), float.Parse(v3int[2]));
            kNewEntry._ResultType = kDataReader.GetInt32(4);
            v3int = kDataReader.GetString(5).Split(' ');
            kNewEntry._ResultValue = new Vector3(float.Parse(v3int[0]), float.Parse(v3int[1]), float.Parse(v3int[2]));
            kNewEntry._CheckType = kDataReader.GetInt32(6);
            kNewEntry._RegType = kDataReader.GetInt32(7);
            kNewEntry._Count = kDataReader.GetInt32(8);
            kNewEntry._DestoryType = kDataReader.GetInt32(9);
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

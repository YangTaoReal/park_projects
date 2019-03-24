using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mono.Data.Sqlite;
public class CS_Task
{
    public class DataEntry
    {
        public System.Int32 _ID = 0;
        public System.String _Mark = "";
        public System.Int32 _Name = 0;
        public System.String _Icon = "";
        public System.String _Item = "";
        public System.Int32 _TaskType = 0;
        public System.Int32 _Number = 0;
        public System.Int32 _Vitality = 0;
        public System.Int32 _VitalityComplete = 0;
    }
    public Dictionary<System.Int32, DataEntry> m_kDataEntryTable = new Dictionary<System.Int32, DataEntry>();
    public void Init()
    {
        m_kDataEntryTable.Clear();
        System.String kSqlCMD = "SELECT * FROM Task";
        m_kDataEntryTable.Clear();
        SqliteDataReader kDataReader = DBManager.Instance.Query(kSqlCMD);
        while (kDataReader.HasRows && kDataReader.Read())
        {
            DataEntry kNewEntry = new DataEntry();
            kNewEntry._ID = kDataReader.GetInt32(0);
            kNewEntry._Mark = kDataReader.GetString(1);
            kNewEntry._Name = kDataReader.GetInt32(2);
            kNewEntry._Icon = kDataReader.GetString(3);
            kNewEntry._Item = kDataReader.GetString(4);
            kNewEntry._TaskType = kDataReader.GetInt32(5);
            kNewEntry._Number = kDataReader.GetInt32(6);
            kNewEntry._Vitality = kDataReader.GetInt32(7);
            kNewEntry._VitalityComplete = kDataReader.GetInt32(8);
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

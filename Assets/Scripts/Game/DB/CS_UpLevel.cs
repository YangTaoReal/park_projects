using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mono.Data.Sqlite;
public class CS_UpLevel
{
    public class DataEntry
    {
        public System.Int32 _ID = 0;
        public System.String _Mark = "";
        public System.Int32 _Des = 0;
        public System.Int32 _Lv = 0;
        public System.Int32 _ModelID = 0;
        public System.String _Func = "";
        public System.Int32 _UpTime = 0;
        public System.Int32 _NextID = 0;
        public System.String _Expend = "";
        public System.Int32 _EffectID = 0;
    }
    public Dictionary<System.Int32, DataEntry> m_kDataEntryTable = new Dictionary<System.Int32, DataEntry>();
    public void Init()
    {
        m_kDataEntryTable.Clear();
        System.String kSqlCMD = "SELECT * FROM UpLevel";
        m_kDataEntryTable.Clear();
        SqliteDataReader kDataReader = DBManager.Instance.Query(kSqlCMD);
        while (kDataReader.HasRows && kDataReader.Read())
        {
            DataEntry kNewEntry = new DataEntry();
            kNewEntry._ID = kDataReader.GetInt32(0);
            kNewEntry._Mark = kDataReader.GetString(1);
            kNewEntry._Des = kDataReader.GetInt32(2);
            kNewEntry._Lv = kDataReader.GetInt32(3);
            kNewEntry._ModelID = kDataReader.GetInt32(4);
            kNewEntry._Func = kDataReader.GetString(5);
            kNewEntry._UpTime = kDataReader.GetInt32(6);
            kNewEntry._NextID = kDataReader.GetInt32(7);
            kNewEntry._Expend = kDataReader.GetString(8);
            kNewEntry._EffectID = kDataReader.GetInt32(9);
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

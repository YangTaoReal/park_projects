using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mono.Data.Sqlite;
public class CS_InOutPut
{
    public class DataEntry
    {
        public System.Int32 _ID = 0;
        public System.String _Mark = "";
        public System.Int32 _GrowTime = 0;
        public System.Single _YoungScale = 0;
        public System.Int32 _YoungAwardTime = 0;
        public System.String _YoungOut = "";
        public System.String _YoungInt = "";
        public System.Single _MatureScale = 0;
        public System.Int32 _MatureAwardTime = 0;
        public System.String _MatureOut = "";
        public System.String _MatureInt = "";
        public System.Int32 _SaveTime = 0;
        public System.Int32 _FloorVal = 0;
    }
    public Dictionary<System.Int32, DataEntry> m_kDataEntryTable = new Dictionary<System.Int32, DataEntry>();
    public void Init()
    {
        m_kDataEntryTable.Clear();
        System.String kSqlCMD = "SELECT * FROM InOutPut";
        m_kDataEntryTable.Clear();
        SqliteDataReader kDataReader = DBManager.Instance.Query(kSqlCMD);
        while (kDataReader.HasRows && kDataReader.Read())
        {
            DataEntry kNewEntry = new DataEntry();
            kNewEntry._ID = kDataReader.GetInt32(0);
            kNewEntry._Mark = kDataReader.GetString(1);
            kNewEntry._GrowTime = kDataReader.GetInt32(2);
            kNewEntry._YoungScale = kDataReader.GetFloat(3);
            kNewEntry._YoungAwardTime = kDataReader.GetInt32(4);
            kNewEntry._YoungOut = kDataReader.GetString(5);
            kNewEntry._YoungInt = kDataReader.GetString(6);
            kNewEntry._MatureScale = kDataReader.GetFloat(7);
            kNewEntry._MatureAwardTime = kDataReader.GetInt32(8);
            kNewEntry._MatureOut = kDataReader.GetString(9);
            kNewEntry._MatureInt = kDataReader.GetString(10);
            kNewEntry._SaveTime = kDataReader.GetInt32(11);
            kNewEntry._FloorVal = kDataReader.GetInt32(12);
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

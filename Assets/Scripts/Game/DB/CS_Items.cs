using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mono.Data.Sqlite;
public class CS_Items
{
    public class DataEntry
    {
        public System.Int32 _ID = 0;
        public System.String _Mark = "";
        public System.Int32 _Name = 0;
        public System.Int32 _DisplayName = 0;
        public System.Int32 _Quality = 0;
        public System.String _Icon = "";
        public System.Int32 _Desc = 0;
        public System.Int32 _ItemType = 0;
        public System.String _Use = "";
    }
    public Dictionary<System.Int32, DataEntry> m_kDataEntryTable = new Dictionary<System.Int32, DataEntry>();
    public void Init()
    {
        m_kDataEntryTable.Clear();
        System.String kSqlCMD = "SELECT * FROM Items";
        m_kDataEntryTable.Clear();
        SqliteDataReader kDataReader = DBManager.Instance.Query(kSqlCMD);
        while (kDataReader.HasRows && kDataReader.Read())
        {
            DataEntry kNewEntry = new DataEntry();
            kNewEntry._ID = kDataReader.GetInt32(0);
            kNewEntry._Mark = kDataReader.GetString(1);
            kNewEntry._Name = kDataReader.GetInt32(2);
            kNewEntry._DisplayName = kDataReader.GetInt32(3);
            kNewEntry._Quality = kDataReader.GetInt32(4);
            kNewEntry._Icon = kDataReader.GetString(5);
            kNewEntry._Desc = kDataReader.GetInt32(6);
            kNewEntry._ItemType = kDataReader.GetInt32(7);
            kNewEntry._Use = kDataReader.GetString(8);
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

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mono.Data.Sqlite;
public class CS_Language
{
    public class DataEntry
    {
        public System.Int32 _ID = 0;
        public System.String _Chinese = "";
        public System.String _English = "";
        public System.String _Arabic = "";
        public System.String _Russian = "";
    }
    public Dictionary<System.Int32, DataEntry> m_kDataEntryTable = new Dictionary<System.Int32, DataEntry>();
    public void Init()
    {
        m_kDataEntryTable.Clear();
        System.String kSqlCMD = "SELECT * FROM Language";
        m_kDataEntryTable.Clear();
        SqliteDataReader kDataReader = DBManager.Instance.Query(kSqlCMD);
        while (kDataReader.HasRows && kDataReader.Read())
        {
            DataEntry kNewEntry = new DataEntry();
            kNewEntry._ID = kDataReader.GetInt32(0);
            kNewEntry._Chinese = kDataReader.GetString(1);
            kNewEntry._English = kDataReader.GetString(2);
            kNewEntry._Arabic = kDataReader.GetString(3);
            kNewEntry._Russian = kDataReader.GetString(4);
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

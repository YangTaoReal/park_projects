using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mono.Data.Sqlite;
public class CS_AssetBundleTable
{
    public class DataEntry
    {
        public System.String _AssetPath = "";
        public System.String _BundelName = "";
    }
    public Dictionary<System.String, DataEntry> m_kDataEntryTable = new Dictionary<System.String, DataEntry>();
    public void Init()
    {
        m_kDataEntryTable.Clear();
        System.String kSqlCMD = "SELECT * FROM AssetBundleTable";
        m_kDataEntryTable.Clear();
        SqliteDataReader kDataReader = DBManager.Instance.Query(kSqlCMD);
        while (kDataReader.HasRows && kDataReader.Read())
        {
            DataEntry kNewEntry = new DataEntry();
            kNewEntry._AssetPath = kDataReader.GetString(0);
            kNewEntry._BundelName = kDataReader.GetString(1);
            m_kDataEntryTable[kNewEntry._AssetPath] = kNewEntry;
        }
        kDataReader.Close();
    }
    public DataEntry GetEntryPtr(System.String _AssetPath)
    {
        if (m_kDataEntryTable.ContainsKey(_AssetPath))
        {
            return m_kDataEntryTable[_AssetPath];
        }
        return null;
    }
    public bool ContainsID(System.String _AssetPath)
    {
        return m_kDataEntryTable.ContainsKey(_AssetPath);
    }
}

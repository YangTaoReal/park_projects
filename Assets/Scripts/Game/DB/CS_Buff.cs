using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mono.Data.Sqlite;
public class CS_Buff
{
    public class DataEntry
    {
        public System.Int32 _ID = 0;
        public System.String _Mark = "";
        public System.Int32 _DisplayName = 0;
        public System.Int32 _Des = 0;
        public System.Single _Result = 0;
        public System.Int32 _ContinueTime = 0;
        public System.Int32 _typPlace = 0;
        public System.String _Suit = "";
        public System.Int32 _Type = 0;
        public System.String _disType = "";
    }
    public Dictionary<System.Int32, DataEntry> m_kDataEntryTable = new Dictionary<System.Int32, DataEntry>();
    public void Init()
    {
        m_kDataEntryTable.Clear();
        System.String kSqlCMD = "SELECT * FROM Buff";
        m_kDataEntryTable.Clear();
        SqliteDataReader kDataReader = DBManager.Instance.Query(kSqlCMD);
        while (kDataReader.HasRows && kDataReader.Read())
        {
            DataEntry kNewEntry = new DataEntry();
            kNewEntry._ID = kDataReader.GetInt32(0);
            kNewEntry._Mark = kDataReader.GetString(1);
            kNewEntry._DisplayName = kDataReader.GetInt32(2);
            kNewEntry._Des = kDataReader.GetInt32(3);
            kNewEntry._Result = kDataReader.GetFloat(4);
            kNewEntry._ContinueTime = kDataReader.GetInt32(5);
            kNewEntry._typPlace = kDataReader.GetInt32(6);
            kNewEntry._Suit = kDataReader.GetString(7);
            kNewEntry._Type = kDataReader.GetInt32(8);
            kNewEntry._disType = kDataReader.GetString(9);
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

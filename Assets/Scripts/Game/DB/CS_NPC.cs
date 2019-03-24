using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mono.Data.Sqlite;
public class CS_NPC
{
    public class DataEntry
    {
        public System.Int32 _ID = 0;
        public System.String _Mark = "";
        public System.Int32 _Name = 0;
        public System.Int32 _ModelID = 0;
        public System.Int32 _MoveType = 0;
        public System.Int32 _LifeTime = 0;
        public System.Int32 _ModelType = 0;
        public Vector3 _PosOffset = Vector3.zero;
    }
    public Dictionary<System.Int32, DataEntry> m_kDataEntryTable = new Dictionary<System.Int32, DataEntry>();
    public void Init()
    {
        m_kDataEntryTable.Clear();
        System.String kSqlCMD = "SELECT * FROM NPC";
        m_kDataEntryTable.Clear();
        SqliteDataReader kDataReader = DBManager.Instance.Query(kSqlCMD);
        string[] v3int = null;
        while (kDataReader.HasRows && kDataReader.Read())
        {
            DataEntry kNewEntry = new DataEntry();
            kNewEntry._ID = kDataReader.GetInt32(0);
            kNewEntry._Mark = kDataReader.GetString(1);
            kNewEntry._Name = kDataReader.GetInt32(2);
            kNewEntry._ModelID = kDataReader.GetInt32(3);
            kNewEntry._MoveType = kDataReader.GetInt32(4);
            kNewEntry._LifeTime = kDataReader.GetInt32(5);
            kNewEntry._ModelType = kDataReader.GetInt32(6);
            v3int = kDataReader.GetString(7).Split(' ');
            kNewEntry._PosOffset = new Vector3(float.Parse(v3int[0]), float.Parse(v3int[1]), float.Parse(v3int[2]));
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

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mono.Data.Sqlite;
public class CS_MapInfo
{
    public class DataEntry
    {
        public System.Int32 _ID = 0;
        public System.Int32 _Name = 0;
        public System.String _ResName = "";
        public Vector3 _CameraPosition = Vector3.zero;
        public Vector3 _CameraRotation = Vector3.zero;
        public System.Single _FOV = 0;
        public System.String _Arguments = "";
    }
    public Dictionary<System.Int32, DataEntry> m_kDataEntryTable = new Dictionary<System.Int32, DataEntry>();
    public void Init()
    {
        m_kDataEntryTable.Clear();
        System.String kSqlCMD = "SELECT * FROM MapInfo";
        m_kDataEntryTable.Clear();
        SqliteDataReader kDataReader = DBManager.Instance.Query(kSqlCMD);
        string[] v3int = null;
        while (kDataReader.HasRows && kDataReader.Read())
        {
            DataEntry kNewEntry = new DataEntry();
            kNewEntry._ID = kDataReader.GetInt32(0);
            kNewEntry._Name = kDataReader.GetInt32(1);
            kNewEntry._ResName = kDataReader.GetString(2);
            v3int = kDataReader.GetString(3).Split(' ');
            kNewEntry._CameraPosition = new Vector3(float.Parse(v3int[0]), float.Parse(v3int[1]), float.Parse(v3int[2]));
            v3int = kDataReader.GetString(4).Split(' ');
            kNewEntry._CameraRotation = new Vector3(float.Parse(v3int[0]), float.Parse(v3int[1]), float.Parse(v3int[2]));
            kNewEntry._FOV = kDataReader.GetFloat(5);
            kNewEntry._Arguments = kDataReader.GetString(6);
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

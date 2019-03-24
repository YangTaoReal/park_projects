using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mono.Data.Sqlite;
public class CS_Shop
{
    public class DataEntry
    {
        public System.Int32 _ID = 0;
        public System.String _Mark = "";
        public System.Int32 _Name = 0;
        public System.Int32 _DisplayName = 0;
        public System.String _Icon = "";
        public System.Int32 _Category = 0;
        public Vector3 _Goods = Vector3.zero;
        public Vector3 _GoldPrice = Vector3.zero;
        public Vector3 _StonePrice = Vector3.zero;
        public System.Int32 _Promotion = 0;
        public System.Int32 _Sort = 0;
    }
    public Dictionary<System.Int32, DataEntry> m_kDataEntryTable = new Dictionary<System.Int32, DataEntry>();
    public void Init()
    {
        m_kDataEntryTable.Clear();
        System.String kSqlCMD = "SELECT * FROM Shop";
        m_kDataEntryTable.Clear();
        SqliteDataReader kDataReader = DBManager.Instance.Query(kSqlCMD);
        string[] v3int = null;
        while (kDataReader.HasRows && kDataReader.Read())
        {
            DataEntry kNewEntry = new DataEntry();
            kNewEntry._ID = kDataReader.GetInt32(0);
            kNewEntry._Mark = kDataReader.GetString(1);
            kNewEntry._Name = kDataReader.GetInt32(2);
            kNewEntry._DisplayName = kDataReader.GetInt32(3);
            kNewEntry._Icon = kDataReader.GetString(4);
            kNewEntry._Category = kDataReader.GetInt32(5);
            v3int = kDataReader.GetString(6).Split(' ');
            kNewEntry._Goods = new Vector3(float.Parse(v3int[0]), float.Parse(v3int[1]), float.Parse(v3int[2]));
            v3int = kDataReader.GetString(7).Split(' ');
            kNewEntry._GoldPrice = new Vector3(float.Parse(v3int[0]), float.Parse(v3int[1]), float.Parse(v3int[2]));
            v3int = kDataReader.GetString(8).Split(' ');
            kNewEntry._StonePrice = new Vector3(float.Parse(v3int[0]), float.Parse(v3int[1]), float.Parse(v3int[2]));
            kNewEntry._Promotion = kDataReader.GetInt32(9);
            kNewEntry._Sort = kDataReader.GetInt32(10);
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

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mono.Data.Sqlite;
public class CS_Model
{
    public class DataEntry
    {
        public System.Int32 _ID = 0;
        public System.String _Mark = "";
        public System.Int32 _Name = 0;
        public System.Int32 _DisplayName = 0;
        public System.String _Icon = "";
        public System.Int32 _Desc = 0;
        public System.Int32 _Quality = 0;
        public System.Boolean _IsShow = false;
        public System.Int32 _BuildMax = 0;
        public System.String _PointFun = "";
        public System.Single _Move = 0;
        public System.Int32 _Type = 0;
        public System.Int32 _Ctype = 0;
        public Vector3 _Scale = Vector3.zero;
        public System.Single _Radius = 0;
        public Vector3 _Rotate = Vector3.zero;
        public System.String _Path = "";
        public System.Int32 _Volume = 0;
        public System.Int32 _Capacity = 0;
        public System.String _GetWay = "";
        public System.String _BuildingTerrain = "";
        public System.Int32 _BuildTime = 0;
        public System.Int32 _UpLvID = 0;
        public System.Int32 _InOutPutID = 0;
        public System.String _Effect = "";
        public System.String _EffPos = "";
        public System.Int32 _SubgradeID = 0;
    }
    public Dictionary<System.Int32, DataEntry> m_kDataEntryTable = new Dictionary<System.Int32, DataEntry>();
    public void Init()
    {
        m_kDataEntryTable.Clear();
        System.String kSqlCMD = "SELECT * FROM Model";
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
            kNewEntry._Desc = kDataReader.GetInt32(5);
            kNewEntry._Quality = kDataReader.GetInt32(6);
            kNewEntry._IsShow = kDataReader.GetBoolean(7);
            kNewEntry._BuildMax = kDataReader.GetInt32(8);
            kNewEntry._PointFun = kDataReader.GetString(9);
            kNewEntry._Move = kDataReader.GetFloat(10);
            kNewEntry._Type = kDataReader.GetInt32(11);
            kNewEntry._Ctype = kDataReader.GetInt32(12);
            v3int = kDataReader.GetString(13).Split(' ');
            kNewEntry._Scale = new Vector3(float.Parse(v3int[0]), float.Parse(v3int[1]), float.Parse(v3int[2]));
            kNewEntry._Radius = kDataReader.GetFloat(14);
            v3int = kDataReader.GetString(15).Split(' ');
            kNewEntry._Rotate = new Vector3(float.Parse(v3int[0]), float.Parse(v3int[1]), float.Parse(v3int[2]));
            kNewEntry._Path = kDataReader.GetString(16);
            kNewEntry._Volume = kDataReader.GetInt32(17);
            kNewEntry._Capacity = kDataReader.GetInt32(18);
            kNewEntry._GetWay = kDataReader.GetString(19);
            kNewEntry._BuildingTerrain = kDataReader.GetString(20);
            kNewEntry._BuildTime = kDataReader.GetInt32(21);
            kNewEntry._UpLvID = kDataReader.GetInt32(22);
            kNewEntry._InOutPutID = kDataReader.GetInt32(23);
            kNewEntry._Effect = kDataReader.GetString(24);
            kNewEntry._EffPos = kDataReader.GetString(25);
            kNewEntry._SubgradeID = kDataReader.GetInt32(26);
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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SettlementsAssetServer
{
    public string ID;
    public string m_kListSettlement;
}



/// <summary>
/// 安置点
/// </summary>
//[System.Serializable]
public class SettlementsAsset
{
    string _id;
    public string ID
    {
        get { return _id; }
        set { _id = value; }
    }
    public List<Guid> m_kListSettlement = new List<Guid>();
}


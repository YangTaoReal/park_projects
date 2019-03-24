using QTFramework;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;


using System.IO;
 
using System.Reflection;
 
//因策划要求. 改成了一个毫无拓展性的buff组建 

[ObjectEventSystem]
public class BuffManagerAwakeSystem : AAwake<BuffManager>
{
    public override void Awake(BuffManager _self)
    {
        _self.Awake();
    }
}

[ObjectEventSystem]
public class BuffManagerFixedUpdateSystem : AFixedUpdate<BuffManager>
{
    public override void FixedUpdate(BuffManager _self)
    {
        _self.FixedUpdate();
    }
}
 
//buff产生的场所
public enum buffPlace
{
    All = 0,//不限制地方
    Park = 1,//园区
    House = 2,//小木屋
}

//buff类型
public enum BuffType
{
    EffOutPut = 1,//影响产出
    Water = 2,//缺水
    Food = 3,//缺少食物
    Ill = 4,//生病
    Danger = 5,//濒危
}

 
public class Buff
{
    public int id = 0;
    public CS_Buff.DataEntry CS_Buff;
}


public class BuffManager : QTComponent
{
    public int lackWaterBuffID = 100000;
    public int lackFoodBuffID = 100001;
    public int lackIllBuffID = 100002;
    public int lackDangerBuffID = 100003;

    int buffId;
    List<Buff> m_lBuffs = new List<Buff>();

    public static BuffManager _instance = null;
    Dictionary<int, List<CS_Buff.DataEntry>> dicBuffMgr = new Dictionary<int, List<CS_Buff.DataEntry>>();
    public void Awake()
    {
        _instance = this;
        m_lBuffs.Clear();
        buffId = 0;

        List<int> lAnimalPlants = new List<int>();
        foreach (var data in DBManager.Instance.m_kModel.m_kDataEntryTable)
        {
            if(data.Value._Type == (int)ModelCType.Animal || data.Value._Type == (int)ModelCType.Plant)
            {
                lAnimalPlants.Add(data.Value._ID);
            }
        }

        dicBuffMgr.Clear();
        for (int i = 0; i < lAnimalPlants.Count; i++)
        {
            if (!dicBuffMgr.ContainsKey(lAnimalPlants[i]))
                dicBuffMgr[lAnimalPlants[i]] = new List<CS_Buff.DataEntry>();
            foreach (var data in DBManager.Instance.m_kBuff.m_kDataEntryTable)
            {
                if(data.Value._ID >= 200000)
                {
                    string[] split = data.Value._Suit.Split('|');
                    for (int j = 0; i < split.Length; j++)
                    {
                        if (int.Parse(split[j]) == lAnimalPlants[i])
                        {
                            dicBuffMgr[lAnimalPlants[i]].Add(data.Value);
                            break;
                        }
                    }
                }

            } 
        }

 
     
    }

    public void FixedUpdate()
    {
 



    }


  
    //得到自身所有的buff
    public List<Buff> GetSelfAllBuff(BaseData baseData)
    {
        List<Buff> listBuffs = new List<Buff>();
        ModelBase modelBase = baseData.GetComponent<ModelBase>();
        BaseServer baseServer = modelBase.GetbaseServer;
        if (baseServer.growthState == GrowthState.Young) return listBuffs;
        var CS_InOutPut = DBManager.Instance.m_kInOutPut.GetEntryPtr(baseData.cfg._InOutPutID);
        Guid father_guid;
        Park park = null;
        if(!string.IsNullOrEmpty(baseServer.father_guid))
        {
            father_guid = Guid.Parse(baseServer.father_guid);
            park = ModelManager._instance.GetParkByFaterGuid(father_guid);
        }
        

        for (int i = 0; i < dicBuffMgr[baseData.cfg._ID].Count; i++)
        {
            var CS_Buff = dicBuffMgr[baseData.cfg._ID][i];
            if (CS_Buff._typPlace == 0)//不限制场地
            {
                List<List<int>> m_lStatePro = modelBase.GetStatePro;
                for (int j = 0; j < m_lStatePro.Count; j++)
                {
                    if (modelBase.GetbaseServer.proVal[j] <= 0)
                    {
                        if (m_lStatePro[j][0] == (int)StatePro.Thirst || m_lStatePro[j][0] == (int)StatePro.Hunger)
                        {
                            Buff buff = new Buff();
                            buff.id = buffId++;
                            if (m_lStatePro[j][0] == (int)StatePro.Thirst)
                                buff.CS_Buff = DBManager.Instance.m_kBuff.GetEntryPtr(lackWaterBuffID);//100000
                            else if (m_lStatePro[j][0] == (int)StatePro.Hunger)
                            {
                                buff.CS_Buff = DBManager.Instance.m_kBuff.GetEntryPtr(lackFoodBuffID);//100001
                                TimeSpan tsLack = DateTime.Now.Subtract(DateTime.Parse(baseServer.hungerTime));
                                if (tsLack.TotalSeconds >= (CS_InOutPut._FloorVal / m_lStatePro[i][1]) * 60)
                                {//出现濒危buff;
                                    Buff _buff = new Buff();
                                    _buff.id = buffId++;
                                    _buff.CS_Buff = DBManager.Instance.m_kBuff.GetEntryPtr(lackDangerBuffID);//100003
                                    listBuffs.Add(_buff);
                                    m_lBuffs.Add(_buff);
                                }
                            }
                            listBuffs.Add(buff);
                            m_lBuffs.Add(buff);
                        }
                    }
                }
            }
            else if(CS_Buff._typPlace == 1)//在园区
            {
                string[] split = CS_Buff._Suit.Split('|');
                List<int> lInt = new List<int>();
                foreach (string str in split)
                {
                    lInt.Add(int.Parse(str));
                }
                if (isParkHave(park, lInt))
                {
                    Buff buff = new Buff();
                    buff.id = buffId++;
                    buff.CS_Buff = CS_Buff;
                    listBuffs.Add(buff);
                    m_lBuffs.Add(buff);
                }
            }
            else if(CS_Buff._typPlace == 2)//在小木屋
            {
                
            }
        }


        //if(string.IsNullOrEmpty(baseServer.father_guid))//在小木屋
        //{
            
        //}
        //else//在园区
        //{
        //    //Guid father_guid = Guid.Parse(baseServer.father_guid);
        //    //Park park = ModelManager._instance.GetParkByFaterGuid(father_guid);
        //    for (int i = 0; i < dicBuffMgr[baseData.cfg._ID].Count; i++)
        //    {
        //        string[] split = dicBuffMgr[baseData.cfg._ID][i]._Suit.Split('|');
        //        List<int> lInt = new List<int>();
        //        foreach (string str in split)
        //        {
        //            int cid = int.Parse(str);
        //            if (cid != 0) lInt.Add(cid);
        //        }
        //        if (isParkHave(park, lInt))
        //        {
        //            Buff buff = new Buff();
        //            buff.id = buffId++;
        //            buff.CS_Buff = dicBuffMgr[baseData.cfg._ID][i];
        //            listBuffs.Add(buff);
        //            m_lBuffs.Add(buff);
        //        }
        //    }
        //}





      

        //ModelBase modelBase = baseData.GetComponent<ModelBase>();
        //List<List<int>> m_lStatePro = modelBase.GetStatePro;
        //for (int i = 0; i < m_lStatePro.Count; i++)
        //{
        //    if (modelBase.GetbaseServer.proVal[i] <= 0)
        //    {
        //        if (m_lStatePro[i][0] == (int)StatePro.Thirst || m_lStatePro[i][0] == (int)StatePro.Hunger)
        //        {
        //            Buff buff = new Buff();
        //            buff.id = buffId++;
        //            if(m_lStatePro[i][0] == (int)StatePro.Thirst)
        //                buff.CS_Buff = DBManager.Instance.m_kBuff.GetEntryPtr(lackWaterBuffID);//100000
        //            else if(m_lStatePro[i][0] == (int)StatePro.Hunger)
        //                buff.CS_Buff = DBManager.Instance.m_kBuff.GetEntryPtr(lackFoodBuffID);//100001
        //            listBuffs.Add(buff);
        //            m_lBuffs.Add(buff);
        //        }
        //    }
        //}
 
        return listBuffs;

    }
 

    //public void AddBuff(int cid_buff)
    //{
    //    Buff buff = new Buff();
    //    buff.id = buffId++;
    //    buff.CS_Buff = DBManager.Instance.m_kBuff.GetEntryPtr(cid_buff);
    //    m_lBuffs.Add(buff);
    //}
 
 
    bool isParkHave(Park park, List<int> listIDs)
    {
        if(park == null) return false;
        List<int> IDs = park.GetAllStateItemID(GrowthState.Mmature);
        int nHave = 0;
        for (int i = 0; i < listIDs.Count; i++)
        {
            if (IDs.Contains(listIDs[i]))
                nHave++;
        }
        if (nHave == listIDs.Count)
            return true;
        else
            return false;
    }


    public void Remove(int _id)
    {
        for (int i = 0; i < m_lBuffs.Count; i++)
        {
            if (m_lBuffs[i].id == _id)
            {
                m_lBuffs.Remove(m_lBuffs[i]);
                return;
            }
        }
    }

    //public Buff GetBuff(int buff_id)
    //{
    //    for (int i = 0; i < m_lBuffs.Count; i++)
    //    {
    //        if (m_lBuffs[i].id == buff_id)
    //        {
    //            return m_lBuffs[i];
    //        }
    //    }
    //    return null;
    //}


    public bool isHaveByCid(int cid)
    {
        for (int i = 0; i < m_lBuffs.Count; i++)
        {
            if (m_lBuffs[i].CS_Buff._ID == cid)
            {
                return true;
            }
        }
        return false;
    }


    public void AllRemove()
    {
        m_lBuffs.Clear();
    }

}
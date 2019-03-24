using System.Collections.Generic;
using UnityEngine;
using System;
 
namespace QTFramework
{
    [System.Serializable]
    public class AnimalServer : BaseServer
    {

    }
  
    public class Animal : ModelBase
    {
        AnimalServer Server;
        AnimWrap animWrap;
        Player player;

     
        int SaveCount = 1;
  
        public List<List<int>> m_lOut = new List<List<int>>();//产出
        public AnimalMove animalMove;
        public CS_InOutPut.DataEntry CS_InOutPut;
        public AnimalServer GetServer
        {
            get { return Server; }
        }
    

        void Awake()
        {
            SaveCount = 1;
            m_lStatePro.Clear();
            m_lOut.Clear();
            m_lMyselfBuff.Clear();
            gameObject.layer = LayerMask.NameToLayer("Animal");
          
        }

 
 
        void Update()
        {
            if (Server.cfg_id == 0) return;
            if (CS_InOutPut == null) return;
            if (Server.placeState == PlaceState.House) 
            {
                Server.placeTime = DateTime.Now.ToString();
            }
            else if(Server.placeState == PlaceState.Park) 
            {
                Server.placeTime = "";
                float needTime = 0;
                int disType = 0;//1是成长,2是产出
                if (Server.growthState == GrowthState.Young)
                {
                    disType = 1;
                    needTime = CS_InOutPut._GrowTime * 60;
                }
                else if (Server.growthState == GrowthState.Mmature)
                {
                    disType = 2;
                    needTime = CS_InOutPut._MatureAwardTime * 60;
                }

                TimeSpan ts = DateTime.Now.Subtract(DateTime.Parse(Server.BeginTime));
                if (ts.TotalSeconds >= needTime)
                {
                    if (disType == 1)
                    {
                        Server.growthState = GrowthState.Mmature;
                        ScaleModel();
                        SetOutPut();
                        SetIntPut();
                        Server.BeginTime = DateTime.Now.ToString();
                        GameEventManager._Instance.onUpgradeAnimal();
                    }
                    else if (disType == 2)
                    {
                        int count = Mathf.FloorToInt((float)ts.TotalSeconds / needTime);
                        for (int i = 0; i < count; i++)
                        {
                            OutputAward();
                        }
                    }
                    DataManager._instance.FixLocalData<AnimalServer>(Server.guid, Server);
                }

                if (ts.TotalSeconds >= 60 * SaveCount)
                {
                    SaveCount++;
                    for (int i = 0; i < Server.proVal.Length; i++)
                    {
                        Server.proVal[i] = Server.proVal[i] - m_lStatePro[i][1];
                        if (Server.proVal[i] <= 0) Server.proVal[i] = 0;
                        if (Server.proVal[i] == 0 && m_lStatePro[i][0] == (int)StatePro.Hunger)//在饥饿状态
                        {
                            if (Server.hungerTime.Equals(DateTime.MaxValue.ToString()))
                                Server.hungerTime = DateTime.Now.ToString();
                        }
                        else
                        {
                            Server.hungerTime = DateTime.MaxValue.ToString();
                        }
                    }
                    DataManager._instance.FixLocalData<AnimalServer>(Server.guid, Server);
                }
            }


 
        }

  

        public void Init(AnimalServer _server = null, BaseData _baseData = null, Guid father = default(Guid))
        {
            if(player == null) player = World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer;
            baseData = _baseData;
            CS_InOutPut = DBManager.Instance.m_kInOutPut.GetEntryPtr(baseData.cfg._InOutPutID);
            if(Server == null) Server = new AnimalServer();
            if(_server != null)
            {
                Server = _server;
                TimeSpan ts = DateTime.Now.Subtract(DateTime.Parse(Server.BeginTime));
                if (Server.growthState == GrowthState.Young)
                {
                    if (ts.TotalSeconds >= CS_InOutPut._GrowTime * 60)
                    {
                        Server.growthState = GrowthState.Mmature;
                        Server.BeginTime = DateTime.Now.ToString();
                    }
                }
                else if (Server.growthState == GrowthState.Mmature)
                {
                    if (ts.TotalSeconds >= CS_InOutPut._MatureAwardTime * 60)
                    {
                        OutputAward();
                    }      
                }
            }
            else 
            {
                Server.cfg_id = baseData.cfg._ID;
                Server.guid = baseData.guid.ToString();
                Server.growthState = GrowthState.Young;
                Server.placeState = PlaceState.Park;
                Server.BeginTime = DateTime.Now.ToString();
                Server.hungerTime = DateTime.MaxValue.ToString();
    
                Server.proVal = new double[2];
                Server.proVal[0] = -1;//默认值
                Server.proVal[1] = -1;//默认值
          
                if (father == default(Guid))
                    Debug.LogError("__没有选中园区么?__");
                else
                    Server.father_guid = father.ToString();
            }
  
            ScaleModel();
            SetOutPut();
            SetIntPut();
         
            World.Scene.GetComponent<TimeComponent>().CreateTimer(3000, 1, 1, () =>
            {
                GetBuff();
            });

      
            animWrap = baseData.GetComponent<AnimWrap>();
            animalMove = baseData.GetComponent<AnimalMove>();
            animalMove.walkSpeed = baseData.cfg._Move;
  
        }
 
        public void SetProVal(int idx, int addVal)
        {
            Server.proVal[idx] = Server.proVal[idx] + addVal;
            if(Server.proVal[idx] >= m_lStatePro[idx][2])
            {
                Server.proVal[idx] = m_lStatePro[idx][2];
            }
        }
        //public void SetBuff(List<Buff> buffs)
        //{
        //    m_lMyselfBuff = buffs;
        //}


        public List<Buff> GetBuff()
        {
            //if (string.IsNullOrEmpty(Server.father_guid)) return new List<Buff>();
            m_lMyselfBuff = BuffManager._instance.GetSelfAllBuff(baseData);
            return m_lMyselfBuff;
        }
    
        void SetIntPut()
        {
            if (CS_InOutPut == null) return;
            if (Server.growthState == GrowthState.Young)
                m_lStatePro = AnalysisStatePro(CS_InOutPut._YoungInt);
            else if (Server.growthState == GrowthState.Mmature)
                m_lStatePro = AnalysisStatePro(CS_InOutPut._MatureInt);

            double[] temp = new double[m_lStatePro.Count];
            for (int i = 0; i < m_lStatePro.Count; i++)
            {
                if(Server.proVal[i] == -1)
                {
                    temp[i] = m_lStatePro[i][2];
                }
                else
                {
                    if (i < Server.proVal.Length)
                    {
                        temp[i] = Server.proVal[i];
                    }
                    else
                        temp[i] = m_lStatePro[i][2];  
                }

            }
            Server.proVal = temp;
        }

        void SetOutPut()
        {
            if (CS_InOutPut == null)
                return;
            if (Server.growthState == GrowthState.Mmature)
            {
                m_lOut = AnalysisStatePro(CS_InOutPut._MatureOut);
            }
        }
    
        //产出奖励
        void OutputAward()
        {
            GetBuff();
            for (int i = 0; i < m_lOut.Count; i++)
            {
                List<int> _out = m_lOut[i];
                int val = _out[2];
                float rate = 0;
                foreach(var buff in m_lMyselfBuff)
                {
                    if(buff.CS_Buff._Type == (int)BuffType.EffOutPut)
                    {
                        rate = rate + buff.CS_Buff._Result;
                    }
                }

                float deval = (1 + rate) * val;
                if (deval <= 0)
                    deval = 0;
                player.AddAsset((PlayerBagAsset.ItemType)_out[0], (decimal)deval);
            }
            Server.BeginTime = DateTime.Now.ToString();
        }

        void ScaleModel()
        {
            if (baseData == null) return;
            if(Server.growthState == GrowthState.Young)
                baseData.go.transform.localScale = baseData.cfg._Scale * CS_InOutPut._YoungScale;
            else if(Server.growthState == GrowthState.Mmature)
                baseData.go.transform.localScale = baseData.cfg._Scale * CS_InOutPut._MatureScale;
        }

        public void BeginMove(List<Vector3> pos)
        {
            animalMove.OnIdle = () =>
            {
                animWrap.PlayAnimType(AnimType.Idle);
            };

            animalMove.OnWalk = () =>
            {
                animWrap.PlayAnimType(AnimType.Walk);
            };
            animalMove.SetArea(pos);
        }


        void OnDestroy()
        {
 
            for (int i = 0; i < m_lMyselfBuff.Count; i++)
            {
                BuffManager._instance.Remove(m_lMyselfBuff[i].id);
            }
            m_lMyselfBuff.Clear();
            SaveCount = 1;
            m_lStatePro.Clear();
            m_lOut.Clear();
        }


    }

}


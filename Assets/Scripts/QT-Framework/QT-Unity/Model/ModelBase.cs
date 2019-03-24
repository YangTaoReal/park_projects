using System;
using System.Collections;
using System.Collections.Generic;

using QTFramework;

using UnityEngine;

//建筑物功能
public enum BuildFunction
{
    Stock = 1, //库存
    ParkVolume = 2, //园区体积
    Ridership = 3, //客流量
    WaterStock = 3, //储水量
}

public enum StatePro
{
    Thirst = 1, //口渴,干旱
    Hunger = 2, //饥饿,施肥
    Sunny = 3, //阳光

    Death = 99, //死亡
}

public enum ModeTyp
{
    Actor = 1,//角色
    Building = 2,//建筑物
    Animal = 3,//动物
    Plant = 4,//植物

}


public enum ModelCType
{
    Actor = 1, //人物
    Building = 2, //建筑
    Animal = 3, //动物
    Plant = 4, //植物
    Subgrade = 20, //地基
    Road = 21, //道路
    Grass = 22, //草坪
    Wastedland = 23, //荒地
    Barrier = 24, //园区
    Floor = 25, //地面
    MainBase = 26, //小木屋
    ParkSign = 27, //园区牌子
    Airport = 28, //飞机场
    WaterPool = 29, //蓄水池
    Assistant = 30, //助手
    WastedSign = 31, //荒地牌子
}

public enum BuildState
{
    Idle = 1,
    Sustain = 2, //建造中
    UpLv = 3, //升级中
    //End = 3,//建造结束
}

public enum GrowthState
{
    Young = 1, //成长期
    Mmature = 2, //成熟
}

public enum PlaceState
{
    Park = 1, //在园区
    Move = 2, //在移动
    House = 3 //在安置点
}

[System.Serializable]
//只是植物和动物
public class BaseServer
{
    public string guid;
    public int cfg_id;
    public string father_guid;
    public PlaceState placeState;//所在地
    public GrowthState growthState; //成长状态
    public string BeginTime;
    public string hungerTime; //饥饿时间
    public string placeTime; //所在地呆了多长时间
    public double[] proVal; //属性剩余值
}

public class ModelBase : MonoBehaviour
{
    public BaseData baseData { get; set; }

    //PlaceState _placeState;
    //public PlaceState PlaceState
    //{
    //    get
    //    {
    //        return _placeState;
    //    }
    //    set
    //    {
    //        _placeState = value;
    //    }
    //}

    public List<List<int>> m_lStatePro = new List<List<int>>(); //消耗属性
    public List<List<int>> GetStatePro
    {
        get
        {
            SetStatePro();
            return m_lStatePro;
        }
    }

    BaseServer baseServer;
    public BaseServer GetbaseServer
    {
        get
        {
            SetBaseServer();
            return baseServer;
        }
    }

    public List<Buff> m_lMyselfBuff = new List<Buff>(); //自身的buff列表
    public List<Buff> GetMyselfBuff
    {
        get
        {
            SetMyselfBuff();
            return m_lMyselfBuff;
        }
    }

    void Awake()
    {

    }

    void Start()
    {
        //baseServer = new BaseServer();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Init(BaseData _baseData)
    {
        baseServer = new BaseServer();
        Guid father = default(Guid);
        if (SceneLogic._instance != null && SceneLogic._instance.selectTileInfo != null && MapGridMgr.IsBarrier(SceneLogic._instance.selectTileInfo.prefabId))
        {
            father = ModelManager._instance.GetParkServerGuid(SceneLogic._instance.selectTileInfo.guid);
        }

        baseData = _baseData;

        if (baseData.cfg._Type == (int) ModeTyp.Actor)
        {
            Actor actor = baseData.GetComponent<Actor>();
            ActorServer server = DataManager._instance.GetLoalData<ActorServer>(baseData.guid);
            actor.Init(server, baseData);
        }
        else if (baseData.cfg._Type == (int) ModeTyp.Building)
        {
            Building building = baseData.GetComponent<Building>();
            if (DataManager._instance != null)
            {
                BuildingServer server = DataManager._instance.GetLoalData<BuildingServer>(baseData.guid);
                building.Init(server, baseData);
            }
        }
        else if (baseData.cfg._Type == (int) ModeTyp.Animal)
        {
            Animal animal = baseData.GetComponent<Animal>();
            if (DataManager._instance != null)
            {
                AnimalServer server = DataManager._instance.GetLoalData<AnimalServer>(baseData.guid);
                //PlaceState = PlaceState.Park;
                animal.Init(server, baseData, father);

            }

        }

        else if (_baseData.cfg._Type == (int) ModeTyp.Plant)
        {
            Plant plant = baseData.GetComponent<Plant>();
            if (DataManager._instance != null)
            {
                PlantServer server = DataManager._instance.GetLoalData<PlantServer>(baseData.guid);
                //PlaceState = PlaceState.Park;
                plant.Init(server, baseData, father);

            }
        }
    }

    //填充某个属性
    public void FillOnceProVal(StatePro statePro, int addVal)
    {
        List<List<int>> llState = GetStatePro;
        BaseServer server = GetbaseServer;
        for (int i = 0; i < llState.Count; i++)
        {
            if ((StatePro) llState[i][0] == statePro)
            {
                //SetServerProVal(i, llState[i][2]);
                SetServerProVal(i, addVal);
                return;
            }
        }
 
    }




    ////单个属性喂养
    //public bool OnceFeed()
    //{
        
    //}


    public void FillOnceStatePro(BuffType state, int cid, int num)
    {
        Player player = World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer;
        CS_Items.DataEntry CS_Items = DBManager.Instance.m_kItems.GetEntryPtr(cid);
        string[] splititem = CS_Items._Use.Split(' ');

        List<Buff> lmsBuff = GetMyselfBuff;
        List<List<int>> llState = GetStatePro;
        BaseServer server = GetbaseServer;
        int splititem0 = int.Parse(splititem[0]);
        int splititem2 = int.Parse(splititem[2]);


        bool isSucceed = false;
        for (int i = 0; i < llState.Count; i++)
        {
            //喂食
            if ((StatePro) llState[i][0] == StatePro.Hunger)
            {
                if(splititem0 == (int)PlayerBagAsset.ItemType.Nutrients || splititem0 == (int)PlayerBagAsset.ItemType.AnimaiNutrients || splititem0 == (int)PlayerBagAsset.ItemType.BotanyNutrients)
                {
                    int val = splititem2 * num;
                    if (val > 0) isSucceed = true;
                    //Debug.Log("__llState[i][2]__" + llState[i][2] + "~~~(int)server.proVal[i]~~~" + (int)server.proVal[i]);
                    FillOnceProVal(StatePro.Hunger, val);
                    player.CosetItem(cid, (PlayerBagAsset.ItemType)CS_Items._ItemType, num);
                    break;
                }

            }

            //喂水
            if ((StatePro) llState[i][0] == StatePro.Thirst && splititem0 == (int)PlayerBagAsset.ItemType.Water)
            {
                //int lacknum = llState[i][2] - (int)server.proVal[i];
                int cost;
                if (player.m_kPlayerBasicAsset.m_kWater >= num)
                    cost = num;
                else
                    cost = (int)player.m_kPlayerBasicAsset.m_kWater;
                if (cost > 0) isSucceed = true;
                FillOnceProVal(StatePro.Thirst, cost);
                player.AddAsset(PlayerBagAsset.ItemType.Water, -cost);
                break;
            }
               

        }

        if(isSucceed)
        {
            World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.SetTask(TaskType.Feed, 1);
            if (state == BuffType.Water && isHaveBuff(BuffManager._instance.lackWaterBuffID)) RemoveMyselfBuff(cid, num);
            else if (state == BuffType.Food && isHaveBuff(BuffManager._instance.lackFoodBuffID)) RemoveMyselfBuff(cid, num);
            else if (state == BuffType.Ill && isHaveBuff(BuffManager._instance.lackIllBuffID)) 
            {
                if (RemoveMyselfBuff(cid, num))
                {
                    player.CosetItem(cid, (PlayerBagAsset.ItemType)CS_Items._ItemType, num);
                }   
            }
     
            else if (state == BuffType.Danger && isHaveBuff(BuffManager._instance.lackDangerBuffID)) RemoveMyselfBuff(cid, num);
        }


     
    }

    ////濒危buff 1,增加 2是减少
    //public void DangerBuff(int typ)
    //{
    //    int cid = BuffManager._instance.lackDangerBuffID;
    //    if (isHaveBuff(cid))
    //    {
    //        Debug.Log("___已经有濒死buff了___");
    //        return;
    //    }
    //    if (typ == 1)
    //        BuffManager._instance.AddBuff(cid);
    //    else if (typ == 2)
    //    {
    //        List<Buff> lmsBuff = GetMyselfBuff;
    //        for (int i = 0; i < lmsBuff.Count; i++)
    //        {
    //            if (lmsBuff[i].CS_Buff._ID == cid)
    //            {
    //                BuffManager._instance.Remove(lmsBuff[i].id);
    //                if (baseData.cfg._Type == (int) ModeTyp.Animal)
    //                    GetComponent<Animal>().SetBuff(lmsBuff);
    //                else if (baseData.cfg._Type == (int) ModeTyp.Plant)
    //                    GetComponent<Plant>().SetBuff(lmsBuff);
    //                return;
    //            }
    //        }

    //    }

    //}

 

    public bool isHaveBuff(int cfg_buffid)
    {
        List<Buff> lmsBuff = GetMyselfBuff;
        foreach (Buff buff in lmsBuff)
        {
            if (buff.CS_Buff._ID == cfg_buffid)
                return true;
        }
        return false;
    }

    void SetServerProVal(int idx, int addVal)
    {
        if (baseData.cfg._Type == (int) ModeTyp.Animal)
        {
            GetComponent<Animal>().SetProVal(idx, addVal);
        }
        else if (baseData.cfg._Type == (int) ModeTyp.Plant)
        {
            GetComponent<Plant>().SetProVal(idx, addVal);
        }
    }

    // 消除所有负面debuff
    public void RemoveMyselfDeBuff()
    {
        List<Buff> lmsBuff = GetMyselfBuff;
        for (int i = lmsBuff.Count - 1; i >= 0; i--)
        {
            Buff _buff = lmsBuff[i];
            bool isRemove = false;
            if(_buff.CS_Buff._Type == (int)BuffType.Food) isRemove = true;
            else if (_buff.CS_Buff._Type == (int)BuffType.Ill) isRemove = true;
            else if (_buff.CS_Buff._Type == (int)BuffType.Water) isRemove = true;
            else if (_buff.CS_Buff._Type == (int)BuffType.Danger) isRemove = true;
          
            if(isRemove)
            {
                lmsBuff.Remove(lmsBuff[i]);
                BuffManager._instance.Remove(lmsBuff[i].id);
            }
        }
    }

    //消除自身buff
    bool RemoveMyselfBuff(int item_cid, int item_num)
    {
        var playerBasicAsset = World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.m_kPlayerBasicAsset;
        List<Buff> lmsBuff = GetMyselfBuff;
        bool isRemove = false;
        var CS_Items = DBManager.Instance.m_kItems.GetEntryPtr(item_cid);
        for (int i = 0; i < lmsBuff.Count; i++)
        {
            string[] Split = lmsBuff[i].CS_Buff._disType.Split(' ');
            int split0 = int.Parse(Split[0]);
            if (split0 != 0)
            {
                int split1 = int.Parse(Split[1]);
                int split2 = int.Parse(Split[2]);
                if(split0 == (int)PlayerBagAsset.ItemType.Water)
                    if(playerBasicAsset.m_kWater >= split2) isRemove = true;
                else if(split0 == (int)PlayerBagAsset.ItemType.Gold)
                    if (playerBasicAsset.m_kGold >= split2) isRemove = true;
                else if (split0 == (int)PlayerBagAsset.ItemType.Stone)
                    if (playerBasicAsset.m_kStone >= split2) isRemove = true;
                else if (split0 == (int)PlayerBagAsset.ItemType.Sun)
                    if (playerBasicAsset.m_kSun >= split2) isRemove = true;
                else
                {
                    if(CS_Items._ItemType == (int)PlayerBagAsset.ItemType.Nutrients || CS_Items._ItemType == (int)PlayerBagAsset.ItemType.AnimaiNutrients
                       || CS_Items._ItemType == (int)PlayerBagAsset.ItemType.BotanyNutrients || CS_Items._ItemType == (int)PlayerBagAsset.ItemType.Medicine
                       || CS_Items._ItemType == (int)PlayerBagAsset.ItemType.AnimaiMedicine || CS_Items._ItemType == (int)PlayerBagAsset.ItemType.BotanyMedicine)
                    {
                        string[] splitUse = CS_Items._Use.Split(' ');
                        if(item_num * int.Parse(splitUse[2]) >= split2)
                            isRemove = true;
                    }
                    else
                    {//用道具cid的
                        
                    }
                }
     
            }

            if(isRemove)
            {
                lmsBuff.Remove(lmsBuff[i]);
                BuffManager._instance.Remove(lmsBuff[i].id);
                break;
            }
        }
        return isRemove;
    }

    void SetMyselfBuff()
    {
        if (baseData.cfg._Type == (int) ModeTyp.Animal)
        {
            m_lMyselfBuff = GetComponent<Animal>().GetBuff();
        }
        else if (baseData.cfg._Type == (int) ModeTyp.Plant)
        {
            m_lMyselfBuff = GetComponent<Plant>().GetBuff();
        }
    }

    void SetStatePro()
    {
        if (baseData.cfg._Type == (int) ModeTyp.Animal)
        {
            m_lStatePro = GetComponent<Animal>().m_lStatePro;
        }
        else if (baseData.cfg._Type == (int) ModeTyp.Plant)
        {
            m_lStatePro = GetComponent<Plant>().m_lStatePro;
        }
    }
    void SetBaseServer()
    {
        if (baseData.cfg._Type == (int) ModeTyp.Animal)
        {
            AnimalServer aServer = GetComponent<Animal>().GetServer;
            baseServer.cfg_id = aServer.cfg_id;
            baseServer.guid = aServer.guid;
            baseServer.father_guid = aServer.father_guid;
            baseServer.growthState = aServer.growthState;
            baseServer.BeginTime = aServer.BeginTime;
            baseServer.placeTime = aServer.placeTime;
            baseServer.proVal = aServer.proVal;
            baseServer.placeState = aServer.placeState;
        }
        else if (baseData.cfg._Type == (int) ModeTyp.Plant)
        {
            PlantServer pServer = GetComponent<Plant>().GetServer;
            baseServer.cfg_id = pServer.cfg_id;
            baseServer.guid = pServer.guid;
            baseServer.father_guid = pServer.father_guid;
            baseServer.growthState = pServer.growthState;
            baseServer.BeginTime = pServer.BeginTime;
            baseServer.placeTime = pServer.placeTime;
            baseServer.proVal = pServer.proVal;
            baseServer.placeState = pServer.placeState;
        }
    }

    //属性值, 最大值, 每分钟消耗
    public List<List<int>> AnalysisStatePro(string str)
    {
        List<List<int>> states = new List<List<int>>();
        if (str == "xx")
            return states;
        string[] Split = str.Split('|');
        for (int i = 0; i < Split.Length; i++)
        {
            string[] _Split = Split[i].Split(' ');
            List<int> pro = new List<int>();
            int intState = int.Parse(_Split[0]);
            pro.Add(intState);
            pro.Add(int.Parse(_Split[1]));
            pro.Add(int.Parse(_Split[2]));
            states.Add(pro);
        }
        return states;
    }

    public string GetStateName(StatePro state)
    {
        string name = "";
        if (baseData.cfg._Type == (int) ModeTyp.Animal)
        {
            if (state == StatePro.Thirst)
                name = UI_Helper.GetTextByLanguageID(174);
            else if (state == StatePro.Hunger)
                name = UI_Helper.GetTextByLanguageID(176);
        }
        else if (baseData.cfg._Type == (int) ModeTyp.Plant)
        {
            if (state == StatePro.Thirst)
                name = UI_Helper.GetTextByLanguageID(175);
            else if (state == StatePro.Hunger)
                name = UI_Helper.GetTextByLanguageID(177);
        }
        return name;
    }

    public string GetGrowthName()
    {
        string name = "";
        if (baseData.cfg._Type == (int) ModeTyp.Animal)
        {
            AnimalServer server = baseData.GetComponent<Animal>().GetServer;
            if (server.growthState == GrowthState.Young)
                name = UI_Helper.GetTextByLanguageID(180);
            else if (server.growthState == GrowthState.Mmature)
                name = UI_Helper.GetTextByLanguageID(181);
        }
        else if (baseData.cfg._Type == (int) ModeTyp.Plant)
        {
            PlantServer server = baseData.GetComponent<Plant>().GetServer;
            if (server.growthState == GrowthState.Young)
                name = UI_Helper.GetTextByLanguageID(180);
            else if (server.growthState == GrowthState.Mmature)
                name = UI_Helper.GetTextByLanguageID(181);
        }
        return name;
    }

    /// <summary>
    /// 回收
    /// </summary>
    public void Recycle()
    {
        if (baseData == null) return;
        ModelManager._instance.RecycleByGuid(baseData.guid);

    }

    //public virtual void PlayAnim(string animName)
    //{
    //    if (animWrap == null) return;
    //    animWrap.AfreshPlay(animName);
    //}

    //public virtual void PauseAnim()
    //{
    //    if (animWrap == null) return;
    //    animWrap.Pause();
    //}

    //public virtual void GoonAnim()
    //{
    //    if (animWrap == null) return;
    //    animWrap.Goon();
    //}

    //public delegate void AnimEvent(AnimData anim);
    ////public event AnimEvent callback;

    //public virtual void CallBackAnimBegin(AnimEvent callback)
    //{
    //    if (callback == null) return;
    //    animWrap.OnAnimationBegin += (anim) => { callback(anim); };
    //}

    //public virtual void CallBackAnimEnd(AnimEvent callback)
    //{
    //    if (callback == null) return;
    //    animWrap.OnAnimationEnd += (anim) => { callback(anim); };
    //}

    Outline outline;
    public void EnableOutline(int w = 2)
    {
        if (outline == null)
        {
            outline = gameObject.GetComponent<Outline>();
        }

        if (outline == null)
        {
            outline = gameObject.AddComponent<Outline>();
            if (outline != null)
            {
                outline.OutlineMode = Outline.Mode.OutlineVisible;
                outline.OutlineWidth = w;
            }
        }

        if (outline != null)
        {
            outLineColor = Color.green;
            outline.enabled = true;
        }
    }

    public void EnableOutline(Color color, int w = 2)
    {
        EnableOutline(w);
        outLineColor = color;
    }

    public void DisableOutline()
    {
        if (outline != null)
        {
            outline.enabled = false;
        }
    }

    public Color outLineColor
    {
        set
        {
            if (outline != null)
            {
                outline.OutlineColor = value;
            }
        }
    }

    public void Foucs()
    {
        EnableOutline(Color.white);
    }

    public void UnFoucs()
    {
        DisableOutline();
    }

    public static bool IsObjType(int cType, ModelCType type)
    {
        if ((int) type == cType)
            return true;
        return false;
    }

}
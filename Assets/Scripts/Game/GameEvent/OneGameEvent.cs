using System;
using System.Collections;
using System.Collections.Generic;
using BE;
using QTFramework;
using UnityEngine;

namespace ParkGameEvent
{
    public class OneGameEvent
    {

        public CS_GameEvent.DataEntry DataEntry;    // 该事件的所有配置信息

        public OneGameEvent(CS_GameEvent.DataEntry _dataEntry)
        {
            DataEntry = _dataEntry;
        }
        public OneGameEvent(){
            
        }
  
        public ConditionType GetConditionType()
        {
            return (ConditionType)DataEntry._ConditionType;
        }

        public CheckType GetCheckType()
        {
            return (CheckType)DataEntry._CheckType;
        }

        public RegisterType GetRegisterType()
        {
            return (RegisterType)DataEntry._RegType;
        }

        public ResultType GetResultType()
        {
            return (ResultType)DataEntry._ResultType;
        }

        /// <summary>
        /// 触发了检测 现在检查自己是否达到目标
        /// </summary>
        public void CheckSelfCondition()
        {
            //Debug.Log($"类型触发了检测条件");
            bool isCheckPass = false;
            switch ((ConditionType)DataEntry._ConditionType)
            {
                case ConditionType.AnimalAndPlantNumber:
                    isCheckPass = CheckAnimalAndPlantNumber();
                    break;
                case ConditionType.FinishEvent:
                    isCheckPass = CheckFinishEvent();
                    break;
                case ConditionType.SingleLogInTime:
                    isCheckPass = CheckSingleLogInTime();
                    break;
                case ConditionType.AnimalNumber:
                    isCheckPass = CheckAnimalNumber();
                    break;
                case ConditionType.PlantNumber:
                    isCheckPass = CheckPlantNumber();
                    break;
                case ConditionType.HasWastelandNumber:
                    isCheckPass = CheckHasWastelandNumber();
                    break;
                case ConditionType.DialogueOver:
                    isCheckPass = CheckDialogueOver();
                    break;
                case ConditionType.TotalGameTime:
                    isCheckPass = CheckTotalGameTime();
                    break;
                case ConditionType.CreateTime:
                    isCheckPass = CheckCreateTime();
                    break;
                case ConditionType.RegisterEventOnlineTime:
                    isCheckPass = CheckRegisterEventOnlineTime();
                    break;
                case ConditionType.IllustratedPlantNumber:

                    break;
                case ConditionType.IllustratedAnimalNumber:

                    break;
                case ConditionType.PickUpLitterNumber:

                    break;
                case ConditionType.DealShitNumber:

                    break;
                case ConditionType.CureAnimalNumber:   // 治疗动物

                    break;
                case ConditionType.TotalVisitorNumber:
                    
                    break;
                case ConditionType.None:    // None的话 直接满足条件触发
                    isCheckPass = true;
                    break;
            }
            if (isCheckPass)
                TriggerResult();
        }

        private void TriggerResult()
        {
            bool isSuccessTrigger = false;
            switch ((ResultType)DataEntry._ResultType)
            {
                case ResultType.StartDialogueConversation:
                    isSuccessTrigger = TriggerDialogueConversation();
                    break;
                case ResultType.RegisterEvent:
                    isSuccessTrigger = TriggerRegisterNewEvent();
                    break;
            }
            // 触发完结果枚举 就要检测一次是否满足销毁条件
            if(isSuccessTrigger)
                IfAutoRrmoveEvent(); 
        }

        private void IfAutoRrmoveEvent()
        {
            if((DestroyType)DataEntry._DestoryType == DestroyType.ReturnZero)
            {
                DataEntry._Count--;
                if(DataEntry._Count <= 0)
                {
                    GameEventManager._Instance.RemoveEvent(DataEntry._ID);
                    return;
                }
                // 没有删除事件 这里需要序列化保存一次
                OneGameEvent item = new OneGameEvent(DataEntry);
                DataManager._instance.FixLocalData<OneGameEvent>(DataEntry._ID.ToString(), item);

            }
            //return false;
        }

#region 触发结果类型

        private bool TriggerEventHelper()
        {
            //Debug.Log($"现在触发助手事件");
            DialogueManagerComponent._Instance.TimerDownToStartEventHelper();
            return true;
        }

        private bool TriggerDialogueConversation()
        {
            //DialogueManagerComponent._Instance.StartConversation(ConversationClip.EventHelper);
            bool isSuccess = false;
            switch((ConversationClip)DataEntry._ResultValue.x)
            {
                case ConversationClip.Investment:
                    isSuccess = TriggerInvestment();
                    break;
                case ConversationClip.Reporter:
                    isSuccess = TriggerReporterConversation();
                    break;
                case ConversationClip.EventHelper:
                    isSuccess = TriggerEventHelper();
                    break;

            }
            return true;
        }

        private bool TriggerReporterConversation()
        {
            DialogueManagerComponent._Instance.StartReportEvent();
            return true;
        }

        private bool TriggerRegisterNewEvent()
        {
            // 结果类型是触发重新注册一个新的事件
            GameEventManager._Instance.TriggerToRegisterNewEvent((int)DataEntry._ResultValue.x);
            return true;
        }

        private bool TriggerBiologicalExperts()
        {
            DialogueManagerComponent._Instance.StartConversation(ConversationClip.BiologicalExperts);
            return true;
        }

        private bool TriggerInvestment()
        {
            DialogueManagerComponent._Instance.StartInvestment();
            return true;
        }

       

#endregion

#region  检测条件类型

        private bool CheckAnimalAndPlantNumber()
        {
            int target = (int)DataEntry._ConditionValue.x;
            int animal = ModelManager._instance.GetModleByType(ModelCType.Animal).Count;
            int plant = ModelManager._instance.GetModleByType(ModelCType.Plant).Count;
            //Debug.Log($"检测到动植物总数{animal + plant}目标数{target}");
            if ((animal + plant) >= target)
            {
                //Debug.Log($"检测到动植物总数{animal + plant}大于目标数{target},可以触发result了");
                return true;
            }

            return false;
        }

        private bool CheckFinishEvent()
        {
            if(GameEventManager._Instance.mGameEventData.RecordDic.ContainsKey((int)DataEntry._ResultValue.x))
            {
                return true;
            }
            return false;
        }

        private bool CheckSingleLogInTime()
        {
            float realTime = Time.realtimeSinceStartup;
            Debug.Log($"检测单次游戏在线时间，本次游戏在线时间:{realTime},目标在线时间：{DataEntry._ConditionValue.x}");
            if(realTime > DataEntry._ConditionValue.x * 60) // 表中是以分钟计时的
            {
                return true;
            }
            return false;
        }

        private bool CheckAnimalNumber()
        {
            var parkAnimals = ModelManager._instance.GetModleByType(ModelCType.Animal);
            var bagAnimals = World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.GetBagsByType(PlayerBagAsset.ItemType.Animal);
            int animalID = (int)DataEntry._ConditionValue.y;
            int level = (int)DataEntry._ConditionValue.z;
            int targetNum = (int)DataEntry._ConditionValue.x;
            int currNum = 0;
            if (animalID == -1)
                currNum = parkAnimals.Count + bagAnimals.Count;
            else
            {
                List<BaseData> iTmpList = new List<BaseData>();
                for (int i = 0; i < parkAnimals.Count; i++)
                {
                    if (parkAnimals[i].cfg._ID == animalID)
                        iTmpList.Add(parkAnimals[i]);
                }
                if(level == -1)
                {
                    currNum = iTmpList.Count + World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.SelectItem(animalID).m_kCount;
                }
                else
                {
                    if(level == 1)  // 成长期
                    {
                        for (int i = 0; i < iTmpList.Count; i++)
                        {
                            if (iTmpList[i].GetComponent<Animal>().GetServer.growthState == GrowthState.Young)
                                currNum++;
                        }
                        currNum += World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.SelectItem(animalID).m_kCount;
                    }
                    else if(level == 2) // 成熟期
                    {
                        for (int i = 0; i < iTmpList.Count; i++)
                        {
                            if (iTmpList[i].GetComponent<Animal>().GetServer.growthState == GrowthState.Mmature)
                                currNum++;
                        }
                    }
                }

            }
            if (currNum >= targetNum)
                return true;
            return false;
        }

        private bool CheckPlantNumber()
        {
            var parkPlants = ModelManager._instance.GetModleByType(ModelCType.Plant);
            var bagPlants = World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.GetBagsByType(PlayerBagAsset.ItemType.Botany);
            int plantID = (int)DataEntry._ConditionValue.y;
            int level = (int)DataEntry._ConditionValue.z;
            int targetNum = (int)DataEntry._ConditionValue.x;
            int currNum = 0;
            if (plantID == -1)
                currNum = parkPlants.Count + bagPlants.Count;
            else
            {
                List<BaseData> iTmpList = new List<BaseData>();
                for (int i = 0; i < parkPlants.Count; i++)
                {
                    if (parkPlants[i].cfg._ID == plantID)
                        iTmpList.Add(parkPlants[i]);
                }
                if (level == -1)
                {
                    currNum = iTmpList.Count + World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.SelectItem(plantID).m_kCount;
                }
                else
                {
                    if (level == 1)  // 成长期
                    {
                        for (int i = 0; i < iTmpList.Count; i++)
                        {
                            if (iTmpList[i].GetComponent<Animal>().GetServer.growthState == GrowthState.Young)
                                currNum++;
                        }
                        currNum += World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.SelectItem(plantID).m_kCount;
                    }
                    else if (level == 2) // 成熟期
                    {
                        for (int i = 0; i < iTmpList.Count; i++)
                        {
                            if (iTmpList[i].GetComponent<Animal>().GetServer.growthState == GrowthState.Mmature)
                                currNum++;
                        }
                    }
                }

            }
            if (currNum >= targetNum)
                return true;
            return false;
        }

        private bool CheckHasWastelandNumber()
        {
            uint count = World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.m_kPlayerBasicAsset.m_nWastedLand;
            if (count >= (int)DataEntry._ConditionValue.x)
                return true;
            return false;
        }

        private bool CheckDialogueOver()
        {
            if(DialogueManagerComponent._Instance.CurrentConversationID == (int)DataEntry._ConditionValue.x)
            {
                return true;
            }
            return false;
        }
        // 总的在线时间
        private bool CheckTotalGameTime()
        {
            DateTime curTime = DateTime.Now;
            DateTime onLineTime = World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.m_kPlayerBasicAsset.m_OnLineTime;
            TimeSpan span = curTime - onLineTime;
            int currOnlineTime = World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.m_kPlayerBasicAsset.m_ToatlGameTime + span.Seconds;
            if(currOnlineTime >= (int)DataEntry._ConditionValue.x)
            {
                return true;
            }
            return false;
        }
        // 账号创建的时长
        private bool CheckCreateTime()
        {
            DateTime createTime = World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.m_kPlayerBasicAsset.m_CreateAccountTime;
            DateTime timeNow = DateTime.Now;
            TimeSpan span = timeNow - createTime;
            if(span.Seconds >= (int)DataEntry._ConditionValue.x)
            {
                return true;
            }
            return false;
        }

        private bool CheckRegisterEventOnlineTime()
        {
            RecordEventItem item;
            if(GameEventManager._Instance.mGameEventData.RecordDic.TryGetValue((int)DataEntry._ConditionValue.y,out item))
            {
                if(item.registerOnlineTime >= (int)DataEntry._ConditionValue.x)
                {
                    return true;
                }
            }
            return false;
        }
#endregion

    }
}

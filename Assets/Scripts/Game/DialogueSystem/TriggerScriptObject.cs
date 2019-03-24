using System.Collections;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using QTFramework;
using UnityEngine;

public class TriggerScriptObject : MonoBehaviour {


    private  string mp3SoundPath = @"Assets\Data\Audio\@.mp3";
    private  string wavSoundPath = @"Assets\Data\Audio\@.wav";


    //    void OnConversationStart(Transform actor)
    //{
    //    //Debug.LogError("对话事件开始" + actor.name);
    //}

    void OnConversationLineEnd(Subtitle subtitle) 
    {
        int ConID = subtitle.dialogueEntry.conversationID;
        int lineId = subtitle.dialogueEntry.id;
        //Debug.LogFormat("conversationID:{0},lineID = {1},内容：{2}", ConID, lineId,subtitle.dialogueEntry.currentDialogueText);
        if(ConID == 1 && lineId == 7)
        {
            //Debug.LogError(subtitle.dialogueEntry.currentDialogueText);
            // 展示协议面板回调
            World.Scene.GetComponent<DialogueManagerComponent>().ShowInvestPanel();
        }
        else if(ConID == 1 && lineId == 8)
        {
            // 得到宝箱回调
            World.Scene.GetComponent<DialogueManagerComponent>().GetInvestMoney();
        }
        else if(ConID == 4 && lineId == 9)
        {
            // 倒计时开始触发助手事件
            //DialogueManagerComponent._Instance.TimerDownToStartEventHelper();
            // 记者事件完成的回调
            DialogueManagerComponent._Instance.ReportEventOver();
        }
        else if(ConID == 2 && lineId == 13)
        {
            // 成功解锁助手开荒
            DialogueManagerComponent._Instance.HelperUnlock();
        }
        else if(ConID == 1 && lineId == 3)
        {
            // 银行家介绍完自己 在这里将他的名字改成投资人
            Debug.Log("介绍完了自己了");
            var actor = DialogueManager.instance.initialDatabase.GetActor(2);
            actor.Name = "投资人";

            actor.fields[4].value = "Investor";
            actor.fields[5].value = "Investor";     // 阿拉伯语翻译
            //Conversation con = DialogueManager.instance.DatabaseManager.MasterDatabase.GetConversation(1);
            //DialogueManager.UpdateResponses();
            //for (int i = 0; i < con.dialogueEntries.Count; i++)
            //{
            //    if (con.dialogueEntries[i].ActorID == 2)
            //    {
                    
            //    }
            //}
        }

    }

    void OnConversationLine (Subtitle subtitle)
    {
        //Debug.Log($"000000000000000");
        int ConID = subtitle.dialogueEntry.conversationID;
        int lineId = subtitle.dialogueEntry.id;
        // 不论是谁 都要播放翻页的音效
        int round = Random.Range(0, 2);
        if (round == 0)
            World.Scene.GetComponent<AudioManagerComponent>().PlayAudio(AudioChannel.AudioChannelType.SoundEffect, mp3SoundPath.Replace("@", "UI_talk1"));
        else 
            World.Scene.GetComponent<AudioManagerComponent>().PlayAudio(AudioChannel.AudioChannelType.SoundEffect, mp3SoundPath.Replace("@", "UI_talk2"));
        string ss = "";
        if(ConID == 1)
        {
            if(subtitle.dialogueEntry.ActorID == 2)
            {
                round = Random.Range(1, 5);
                ss = wavSoundPath.Replace("@", "NPC_banker" + round);
                World.Scene.GetComponent<AudioManagerComponent>().PlayAudio(AudioChannel.AudioChannelType.SoundEffect, wavSoundPath.Replace("@", "NPC_banker" + round));

            }
        }
        else if(ConID == 2)
        {
            if (subtitle.dialogueEntry.ActorID == 3)
            {
                round = Random.Range(1, 7);
                ss = wavSoundPath.Replace("@", "NPC_assistant_talk" + round);
                World.Scene.GetComponent<AudioManagerComponent>().PlayAudio(AudioChannel.AudioChannelType.SoundEffect, wavSoundPath.Replace("@", "NPC_assistant_talk"  + round));
            }
        }
        else if (ConID == 3)
        {
            if (subtitle.dialogueEntry.ActorID == 4)
            {
                round = Random.Range(1, 5);
                ss = wavSoundPath.Replace("@", "NPC_Dr" + round);
                World.Scene.GetComponent<AudioManagerComponent>().PlayAudio(AudioChannel.AudioChannelType.SoundEffect, wavSoundPath.Replace("@", "NPC_Dr" + round));
            }
        }
        else if (ConID == 4)
        {
            if (subtitle.dialogueEntry.ActorID == 5)
            {
                round = Random.Range(1, 5);
                ss = wavSoundPath.Replace("@", "NPC_reporter" + round);
                World.Scene.GetComponent<AudioManagerComponent>().PlayAudio(AudioChannel.AudioChannelType.SoundEffect, wavSoundPath.Replace("@", "NPC_reporter" + round));

            }
        }

        //Debug.LogError($"播放音效的路径:{ss}");
    }

    void OnConversationEnd(Transform actor)
    {
        //Debug.Log($"对话完成,上一次结束时候的对话id:{DialogueManager.instance.lastConversationID},现在这一次结束的对话的id:{DialogueManager.instance.CurrentConversationState.subtitle.dialogueEntry.conversationID}");
        DialogueManagerComponent._Instance.lastConversationID = DialogueManager.instance.lastConversationID;
        DialogueManagerComponent._Instance.CurrentConversationID = DialogueManager.instance.CurrentConversationState.subtitle.dialogueEntry.conversationID;
        DialogueManagerComponent._Instance.IsDialoguing = false;
        if(GameEventManager._Instance.onDialogueEventOver != null)GameEventManager._Instance.onDialogueEventOver();

    }
}

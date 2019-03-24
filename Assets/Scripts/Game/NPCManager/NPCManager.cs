using System.Collections;
using System.Collections.Generic;
using QTFramework;
using UnityEngine;

[ObjectEventSystem]
public class NPCManagerAwakeSysterm : AAwake<NPCManager>
{
    // 事件系统需要根据gameevent表来决定什么时候注册 检测那些值
    // 达到指定值之后再完
    public override void Awake(NPCManager self)
    {
        self.Awake();
    }
}

public class NPCManager : QTComponent {
    

    //public List<BaseData>

    public void Awake()
    {
    }

    /// <summary>
    /// 根据配置表信息生成NPC
    /// </summary>
    public void BornNPC(int _NpcID)
    {
        Debug.Log($"刷新npc");
        CS_NPC.DataEntry dataEntry = DBManager.Instance.m_kNPC.GetEntryPtr(_NpcID);
        BaseData npc = ModelManager._instance.Load(dataEntry._ModelID);
        if(dataEntry._ModelType  != 0)
        {
            BaseData build = ModelManager._instance.GetModleByType((ModelCType)dataEntry._ModelType)[0];
            npc.go.transform.position = build.go.transform.position + dataEntry._PosOffset;
            NPCActor actor = npc.go.GetComponent<NPCActor>();
            if (actor == null)
                actor = npc.go.AddComponent<NPCActor>();
        }
        else
        {
            // 随机一个出来

        }

    }


}

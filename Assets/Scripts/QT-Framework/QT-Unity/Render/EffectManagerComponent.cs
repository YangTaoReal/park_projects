using System.Collections;
using System.Collections.Generic;
using QTFramework;
using UnityEngine;
using System;
using System.Threading.Tasks;

[ObjectEventSystem]
public class EffectManagerCompontAwakeSysterm : AAwake<EffectManagerComponent>
{
    public override void Awake(EffectManagerComponent self)
    {
        self.Awake();
    }
}


public class EffectManagerComponent : QTComponent
{

    //private int startID = 1000;

    public readonly Dictionary<Guid, EffectItem> effectDic = new Dictionary<Guid, EffectItem>();

    public void Awake()
    {
        Log.Info("EffectManagerComponent", "特效管理器组件挂载");

    }


    public Guid PlayEffectByCid(int cfg_id, Transform parentTR, Vector3 pos = default(Vector3))
    {
        CS_Effect.DataEntry dataEntry = DBManager.Instance.m_kEffect.GetEntryPtr(cfg_id);
        return PlayEffect(dataEntry._Path, parentTR, pos);
    }

    /// <summary>
    /// 播放一个特效
    /// </summary>
    /// <param name="fullPath">完整的特效路径(包括后缀名)</param>
    /// <param name="loopCount">循环播放次数，-1代表无限循环</param>
    /// <param name="parentTR">父物体，-1代表无限循环</param>
    public Guid PlayEffect(string fullPath, Transform parentTR, Vector3 pos = default(Vector3))
    {
        Debug.LogFormat("加载的路径：{0}", fullPath);
        GameObject go = AssetPoolManager.Instance.Fetch(fullPath) as GameObject;
        if(go == null)
        {
            Debug.LogError("fetch = null");
            return Guid.Empty;
        }
        go.transform.SetParent(parentTR);
        go.transform.localScale = Vector3.one;
        if(pos == default(Vector3))
            go.transform.localPosition = Vector3.zero;
        else
            go.transform.localPosition = pos;
        
        EffectItem item = go.GetComponent<EffectItem>();
        if (null == item)
            item = go.AddComponent<EffectItem>();
        item.EffectID = GenerateID.ID;
        effectDic.Add(item.EffectID, item);
        item.Play();
        return item.EffectID;
    }

    /// <summary>
    /// 按照id索引 关闭特效
    /// </summary>
    /// <param name="id">Identifier.</param>
    public void StopEffctById(Guid id)
    {
        if (id == Guid.Empty)
            return;
        EffectItem item;
        if(effectDic.TryGetValue(id,out item))
        {
            item.Stop();
        }
        else
        {
            Debug.LogError("没找到id" + id);
        }
    }

    public EffectItem GetEffectByGuid(Guid guid)
    {
        if (guid == Guid.Empty) return null;
        return effectDic[guid];
    }

    /// <summary>
    /// 从字典移除item
    /// </summary>
    /// <param name="id">Identifier.</param>
    public void RemoveItemById(Guid id)
    {
        if (effectDic.ContainsKey(id))
            effectDic.Remove(id);
    }

    /// <summary>
    /// 关闭所有特效
    /// </summary>
    public void StopAllEffect()
    {
        var v = effectDic.GetEnumerator();
        while(v.MoveNext())
        {
            v.Current.Value.Stop();
        }
        effectDic.Clear();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QTFramework;
using System.Linq;

[ObjectEventSystem]
public class GfxManagerComponentAwakeSystem : AAwake<GfxManagerComponent>
{
    public override void Awake(GfxManagerComponent _self)
    {
        _self.Awake();
    }
}

[ObjectEventSystem]
public class GfxManagerComponentUpdateSystem : AUpdate<GfxManagerComponent>
{
    public override void Update(GfxManagerComponent _self)
    {
        _self.Update();
    }
}

public class GfxManagerComponent : QTFramework.QTComponent
{
    private static int _gfxID = 1;
    private static int gfxID
    {
        get
        {
            return _gfxID++;
        }
    }
    /// <summary>
    /// 存放播放的UI
    /// </summary>
    private readonly Dictionary<int, RD_Gfx> m_kdictionary = new Dictionary<int, RD_Gfx>();


    public void Awake()
    {
        Log.Info("AudioManagerComponent", "特效管理组件挂载");
    }
    public void Update()
    {
        TimeComponent timeComponent = World.Scene.GetComponent<TimeComponent>();

        List<int> _currentGfxID = m_kdictionary.Keys.ToList();

        for (int i = 0; i < _currentGfxID.Count; i++)
        {
            RD_Gfx _gfx = null;
            if (m_kdictionary.TryGetValue(_currentGfxID[i], out _gfx))
            {
                if (!_gfx.m_kLoop && timeComponent.RenderProcessTime - _gfx.m_kStartPlayTime > _gfx.m_kParticleLifeTime)
                {
                    _gfx.Deactive();
                    m_kdictionary.Remove(_currentGfxID[i]);
                    AssetPoolManager.Instance.Recycle(_gfx.gameObject);         
                }
            }
        }
    }




    public int PlayGfx(string _gfxPath,Transform _parent)
    {
        TimeComponent timeComponent = World.Scene.GetComponent<TimeComponent>();

        RD_Gfx rD_Gfx = AssetPoolManager.Instance.Fetch<RD_Gfx>(_gfxPath);
        rD_Gfx.gameObject.transform.SetParent(_parent);
        rD_Gfx.gameObject.transform.localEulerAngles = Vector3.zero;
        rD_Gfx.gameObject.transform.localPosition = Vector3.zero;
        rD_Gfx.gameObject.transform.localScale = Vector3.one;
        int id = gfxID;
        m_kdictionary[id] = rD_Gfx;
        rD_Gfx.Play(timeComponent.RenderProcessTime);
        return id;
    }
    public int PlayGfx(string _gfxPath, Transform _parent,Vector3 _positon)
    {
        TimeComponent timeComponent = World.Scene.GetComponent<TimeComponent>();

        RD_Gfx rD_Gfx = AssetPoolManager.Instance.Fetch<RD_Gfx>(_gfxPath);
        rD_Gfx.gameObject.transform.SetParent(_parent);
        rD_Gfx.gameObject.transform.localEulerAngles = Vector3.zero;
        rD_Gfx.gameObject.transform.localScale = Vector3.one;
        rD_Gfx.gameObject.transform.position = _positon;

        int id = gfxID;
        m_kdictionary[id] = rD_Gfx;
        rD_Gfx.Play(timeComponent.RenderProcessTime);
        return id;
    }
    public int RemoveGfx(int _gfxId)
    {
        RD_Gfx _gfx = FindGfxByID(_gfxId);
        if (_gfx != null)
        {
            AssetPoolManager.Instance.Recycle(_gfx.gameObject);
        }
        m_kdictionary.Remove(_gfxId);
        return 0;
    }



    public RD_Gfx FindGfxByID(int _gfxID)
    {
        RD_Gfx _gfx = null;
        m_kdictionary.TryGetValue(_gfxID, out _gfx);
        return _gfx;
    }
}

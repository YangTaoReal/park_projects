using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;

public class TileInfo : ModelBase {

    SerializableTileItem _serializable;
    public SerializableTileItem serializable
    {
        get{
            if(_serializable == null)
            {
                _serializable = new SerializableTileItem(prefabId,
                                       transform.localPosition,
                                       transform.localRotation.eulerAngles,
                                       transform.localScale,
                                       guid,
                                       gameId);
            }
            return _serializable;
        }
        set
        {
            _serializable = value;
        }
    }

    public int _id;
    public int prefabId
    {
        get
        {
            if (baseData != null && baseData.cfg != null)
            {
                return baseData.cfg._ID;
            }
            else
            {
                return _id;
            }

        }
        set
        {
            if (baseData != null && baseData.cfg != null)
            {
                baseData.cfg._ID = value;
                _id = value;
            }
            else
            {
                _id = value;
            }
        }
    }

    public int _size = 1;//0/1=1*1格子  2=2*2格子 。。。
    public int size {
        get{
            if (baseData != null && baseData.cfg != null)
            {
                return baseData.cfg._Volume;
            }
            else
            {
                return _size;
            }
        }
        set{
            if (baseData != null && baseData.cfg != null)
            {
                baseData.cfg._Volume = value;
                _size = value;
            }
            else
            {
                _size = value;
            }
        }
    }

    public System.Guid guid
    {
        get
        {
            if(baseData != null){
                return baseData.guid;
            }else{
                return System.Guid.Empty;
            }

        }
    }

    int _groupId;
    public int gameId
    {
        set{
            _groupId = value;
            serializable.gameId = _groupId;
        }

        get
        {
            return _groupId;
        }
    }

    private void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("EditorBuild");
    }

}


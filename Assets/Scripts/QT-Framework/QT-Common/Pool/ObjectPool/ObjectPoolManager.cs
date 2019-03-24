using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QTFramework;
using System;

public class ObjectPoolManager : ASingleton<ObjectPoolManager>
{
    private readonly Dictionary<Type, ObjectPool<object>> m_DictComponentPool = new Dictionary<Type, ObjectPool<object>>();


    private object Fetch(Type _type)
    {
        ObjectPool<object> _objectPool;
        m_DictComponentPool.TryGetValue(_type, out _objectPool);

        return _objectPool.Allocate();

    }

    public T Fetch<T>()
    {
        T t = (T)Fetch(typeof(T));
        return t;
    }

    public void Recycle<T>(T obj)
    {
        ObjectPool<object> _objectPool;
        m_DictComponentPool.TryGetValue(typeof(T), out _objectPool);
        _objectPool.Recycle(obj);
    }
}

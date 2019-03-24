using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> : APool<T>
{
    public  Action<T> mResetMethod;

    public  ObjectPool(int initCount = 0)
    {
        m_kCreater = new ObjectCreater<T>();

        for (int i = 0; i < initCount; i++)
        {
            mCacheStack.Push(m_kCreater.Create());
        }
    }

    public override void Recycle(T obj)
    {
        mCacheStack.Push(obj);
    }
}

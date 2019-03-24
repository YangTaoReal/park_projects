/***********************************************
 * 资源池
 * 资源的分配和回收资源
 * author:SmartCoder
 **********************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AssetPool<T> : APool<T> where T:class
{
    public Action<T> mResetMethod;
    public AssetPool(string _AssetName, int initCount = 0)
    {
        m_kCreater = new AssetCreater<T>(_AssetName);
        PreLoad(initCount);

    }

    private void PreLoad(int initCount)
    {
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

using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public abstract class APool<T> : IPool<T>
{
    protected ICreater<T> m_kCreater;
    protected Stack<T> mCacheStack = new Stack<T>();
    protected int mMaxCount = 5;

    public int CurCount
    {
        get { return mCacheStack.Count; }
    }
    public virtual T Allocate()
    {
        T _t;
        if (mCacheStack.Count == 0)
        {
            _t = m_kCreater.Create();
        }
        else
        {
            _t = mCacheStack.Pop();
        }
        return _t;
    }

    public abstract void Recycle(T obj);

}

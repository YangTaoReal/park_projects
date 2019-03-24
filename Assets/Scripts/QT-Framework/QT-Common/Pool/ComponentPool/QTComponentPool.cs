/***********************************************************
 * QTComponent池
 * 分配  回收
 * author:SmartCoder
 * *********************************************************/

using System;

namespace QTFramework
{
    public class QTComponentPool<T> : APool<T>
    {
        public QTComponentPool(Type _type, int initCount = 0)
        {
            m_kCreater = new QTComponentCreater<T>(_type);

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
}



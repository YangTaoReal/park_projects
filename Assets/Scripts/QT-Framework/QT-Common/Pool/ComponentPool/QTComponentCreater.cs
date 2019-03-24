/***********************************************************
 * QTComponent创建器
 * 创建组件
 * author:SmartCoder
 * *********************************************************/

using System;
using System.Threading.Tasks;

public class QTComponentCreater<T> : ICreater<T>
{
    private Type m_kType;
    public QTComponentCreater(Type _t)
    {
        m_kType = _t;
    }

    public T Create()
    {
        return (T)Activator.CreateInstance(m_kType);
    }

    public Task<T> CreateAsync()
    {
        throw new NotImplementedException();
    }
}

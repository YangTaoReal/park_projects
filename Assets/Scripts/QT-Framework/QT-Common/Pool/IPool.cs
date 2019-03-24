/***********************************************************
 * 池
 * 池接口规定池必须具有 分配和回收功能
 * author:SmartCoder
 * *********************************************************/

using System.Threading.Tasks;

public interface IPool<T>
{
    T Allocate();
    void Recycle(T obj);
}

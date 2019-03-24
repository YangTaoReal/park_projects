/***********************************************************
 * 创建器
 * 创建器接口
 * author:SmartCoder
 * *********************************************************/


using System.Threading.Tasks;

public interface ICreater<T>
{
    T Create();
}
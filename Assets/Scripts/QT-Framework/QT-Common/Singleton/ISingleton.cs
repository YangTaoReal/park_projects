/***********************************************************
 * 单例接口
 * 规定单例类两个接口初始化、卸载
 * author:SmartCoder
 * *********************************************************/


namespace QTFramework
{
    public interface ISingleton
    {
        void Initialize();
        void Uninitialize();
    }
}
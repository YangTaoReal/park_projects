/*************************************
 * 逻辑单例的抽象类
 * author:SmartCoder
 **************************************/

namespace QTFramework
{
    public abstract class ASingleton<T> : ISingleton where T : ASingleton<T>
    {
        private static T instance = null;
        private static object mLock = new object();
        public static T Instance
        {
            get
            {
                lock (mLock)
                {
                    if (instance == null)
                    {
                        instance = SingletonFactory.CreateSingleton<T>();
                    }
                }
                return instance;
            }
        }

        public virtual  void Initialize()
        {
         
        }

        public virtual void UpdateLogic()
        {
            
        }

        public virtual void Uninitialize()
        {
            instance = null;
        }
    }
}

/*************************************
 * mono单例的抽象类
 * author:SmartCoder
 **************************************/
using UnityEngine;

namespace QTFramework
{
    public abstract class AMonoSingleton<T> : MonoBehaviour ,ISingleton where T : AMonoSingleton<T>
    {
        protected static T instance = null;
        private static object mLock = new object();
        public static T Instance
        {
            get
            {
                lock (mLock)
                {
                    if (instance == null)
                    {
                        instance = SingletonFactory.CreateMonoSingleton<T>();
                        if(Application.isPlaying){
                            DontDestroyOnLoad(instance.gameObject);
                        }
                    }
                }
                return instance;
            }
        }

        public virtual void Initialize()
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

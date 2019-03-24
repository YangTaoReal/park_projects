/***********************************************************
 * 组件
 * ecs万物皆组件
 * author:SmartCoder
 * *********************************************************/
using System;

namespace QTFramework
{
    /// <summary>
    /// 组件
    /// </summary>
    public abstract  class QTComponent : IDisposable
    {
        /// <summary>
        /// ID
        /// </summary>
        public Guid InstanceId
        {
            get;
            set;
        }

        /// <summary>
        /// 挂载的实体
        /// </summary>
        public QTEntity ParentEntity
        {
            get;
            set;
        }

        /// <summary>
        /// 是否已被释放
        /// </summary>
        public bool Disposed
        {
            get
            {
                return this.InstanceId == Guid.Empty;
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        protected QTComponent()
        {
            InstanceId = Guid.NewGuid();
        }


        /// <summary>
        /// 获取实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetEntity<T>() where T : QTEntity
        {
            return ParentEntity as T;
        }

        /// <summary>
        /// 渲染逻辑更新
        /// </summary>
        /// <param name="_currentTime"></param>
        /// <param name="_deltaTime"></param>
        public virtual void UpdateRender()
        {
        }

        /// <summary>
        /// 游戏逻辑更新
        /// </summary>
        /// <param name="fCurrentTime"></param>
        public virtual void UpdateLogic()
        {
        }


        /// <summary>
        /// 销毁
        /// </summary>
        public virtual void Dispose()
        {
            if (this.Disposed)
            {
                return;
            }
            InstanceId = Guid.Empty;
        }
    }
}

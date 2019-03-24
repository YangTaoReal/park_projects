using QTFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ObjectEventSystem]
public class LocalizationComponentAwakeSystem : AAwake<LocalizationComponent>
{
    public override void Awake(LocalizationComponent self)
    {
        self.Awake();
    }
}


#region ------游戏语言环境类型-------
public enum WorldLanguageType
{
    Chinese,//中文
    English,//英文
    Arabic,//阿拉伯语言
}
#endregion

public class LocalizationComponent : QTComponent
{
    public  WorldLanguageType m_kLanguage { get; set; }

    public void Awake()
    {
        Log.Info("LocalizationComponent", "场景组件挂载");
        m_kLanguage = WorldLanguageType.Chinese;
    }


}

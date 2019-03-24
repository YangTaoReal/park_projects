/*************************************
 * 游戏世界
 * author:SmartCoder
 **************************************/
using System;
using UnityEngine;

namespace QTFramework
{
    public enum QTLanguage
    {
        None = 0,
        Chinese, //中文
        English, //英文
        Arabic, //阿拉伯语言
        Russian, //俄罗斯
    }

    public static class World
    {
        public static readonly Scene Scene = new Scene();
        public static readonly DateTime LoginTime = DateTime.Now;
        public static QTLanguage m_kLanguage { get; set; }

        public static void Init()
        {
            switch (Application.systemLanguage)
            {
            case SystemLanguage.Russian:
                m_kLanguage = QTLanguage.Russian;
                break;
            case SystemLanguage.Arabic:
                    m_kLanguage = QTLanguage.Arabic;
                break;
            case SystemLanguage.Chinese:
            case SystemLanguage.ChineseSimplified:
            case SystemLanguage.ChineseTraditional:
                m_kLanguage = QTLanguage.Chinese;
                break;
            case SystemLanguage.English:
            default:
                m_kLanguage = QTLanguage.English;
                break;
            }

            if (PlayerPrefs.HasKey("QTLanguage"))
            {
                SetLanguage((QTLanguage) PlayerPrefs.GetInt("QTLanguage"));
            }
        }
        public static void SetLanguage(QTLanguage _language)
        {
            m_kLanguage = _language;
        }
    }
}
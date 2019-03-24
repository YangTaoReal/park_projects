using System.Collections;
using System.Collections.Generic;
using QTFramework;
using UnityEngine;
using UnityEngine.UI;

[ObjectEventSystem]
public class ZoomGuideAnimAwakeSystem : AAwake<ZoomGuideAnim>
{
    public override void Awake(ZoomGuideAnim _self)
    {
        _self.Awake();
    }
}


[UIEntityComponent(UI_PrefabPath.m_sUIPage_ZoomGuideAnim)]
public class ZoomGuideAnim : UIComponent {

    public Text ui_ZoomText;

    public void Awake()
    {
        UIEntity uI_Entity = ParentEntity as UIEntity;
        ui_ZoomText = uI_Entity.m_kUIPrefab.GetCacheComponent(0) as Text;
    }

    public override void TranslateUI()
    {
        base.TranslateUI();
        ui_ZoomText.text = UI_Helper.GetTextByLanguageID(1304);
    }

}

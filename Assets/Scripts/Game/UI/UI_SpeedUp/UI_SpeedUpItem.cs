using System.Collections;
using System.Collections.Generic;
using QTFramework;
using UnityEngine;
using UnityEngine.UI;

[ObjectEventSystem]
public class UI_SpeedUpItemAwakeSystem : AAwake<UI_SpeedUpItem>
{
    public override void Awake(UI_SpeedUpItem _self)
    {
        _self.Awake();
    }
}


[UIEntityComponent(UI_PrefabPath.m_sUIPage_Pack_SpeedUpItem)]
public class UI_SpeedUpItem : UIComponent 
{
    public Text ui_NumText;
    public RawImage ui_propImage;
    public Image ui_selectedImage;
    public Button ui_propBtn;

    public CS_Items.DataEntry info;
    public void Awake()
    {
        UIEntity uI_Entity = ParentEntity as UIEntity;
        ui_NumText = uI_Entity.m_kUIPrefab.GetCacheComponent(0) as Text;
        ui_propImage = uI_Entity.m_kUIPrefab.GetCacheComponent(1) as RawImage;
        ui_selectedImage = uI_Entity.m_kUIPrefab.GetCacheComponent(2) as Image;
        ui_propBtn = uI_Entity.m_kUIPrefab.GetCacheComponent(3) as Button;

        //World.Scene.GetComponent<UIManagerComponent>().Get(UI_PrefabPath.m_sUIPopUpWindow_SpeedUp).GetComponent<UI_SpeedUP>().onSelectedItem = SelectedItem(this);

        //OnSelectedItem = ((go)=>{
        //    World.Scene.GetComponent<UIManagerComponent>().Get(UI_PrefabPath.m_sUIPopUpWindow_SpeedUp).GetComponent<UI_SpeedUP>().OnSelectItem(go);
        //});
        InitEvent();
    }

    public void Init(CS_Items.DataEntry data,int totalCount)
    {
        info = data;
        ui_NumText.text = totalCount.ToString();
        ui_propImage.texture = UI_Helper.AllocTexture(info._Icon);
        SetSelecedImage(false);

    }

    private void InitEvent()
    {
        ui_propBtn.onClick.AddListener(() =>
        {
            Selected(this);
        });
    }

    public void SetSelecedImage(bool isSelected)
    {
        ui_selectedImage.gameObject.SetActive(isSelected);
    }

    public void Selected(UI_SpeedUpItem item)
    {
        World.Scene.GetComponent<UIManagerComponent>().Get(UI_PrefabPath.m_sUIPopUpWindow_SpeedUp).GetComponent<UI_SpeedUP>().OnSelectedItem(item);
    }
}

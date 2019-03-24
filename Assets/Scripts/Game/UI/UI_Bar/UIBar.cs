using QTFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ObjectEventSystem]
public class UIBarAwakeSystem : AAwake<UIBar>
{
    public override void Awake(UIBar _self)
    {
        _self.Awake();
    }
}

[ObjectEventSystem]
public class UIBarFixedUpdateSystem : AFixedUpdate<UIBar>
{
    public override void FixedUpdate(UIBar _self)
    {
        _self.FixedUpdate();
    }
}


public enum BarType
{
    building = 1,
    park = 2,
    assistant = 3,//助手
    wasted = 4,//开荒
}

public class BarsMgr
{
    public UIEntity uIEntity;
    public Guid guid;
    public Vector3 pos;
    public BarType type;
    public UIBar bar;
}

public class ParkBarsMgr : BarsMgr
{
    public GameObject goBegin;
    public GameObject goEnd;
    public Park park;
}

[UIEntityComponent(UI_PrefabPath.m_sUIBar)]
public class UIBar : UIComponent
{

    float lineLen;
    Park park;
    BaseData building;


    public float parkMoveTime;
    public Slider m_kProgess;
    public Text m_kTextProVal;
    public Text m_kTextTime;
    public Text m_kTextSpeedUp;
    public Text m_TextParkItem;
    public Text m_TextWasted;

    public Image m_imgLine;
    public RawImage m_rawImageParkItemIcon;

    public RectTransform m_kTranRoot;
    public RectTransform m_kTranBuilding;
    public RectTransform m_kTranPark;

    public RectTransform m_ParkItem;

    public Button m_kBtnSpeedUp;
    public Button m_BtnParkItem;

    public RectTransform m_tranAssistant;
    public Slider m_proAssistant;
    public Text m_textTimeAssistant;

    public RectTransform m_tranWasteland;
    public Slider m_proWasteland;
    public Text m_textTimeWasteland;
    //public Button m_btnWasteland;

    UI_SpeedUP _uI_SpeedUP;
    public UI_SpeedUP UI_SpeedUP
    {
        get { return _uI_SpeedUP; }
        set { _uI_SpeedUP = value; }
    }


    public GameObject parkGoBegin;
    //public GameObject parkGoEnd;

    internal void Awake()
    {
        m_kProgess = m_kParentEntity.m_kUIPrefab.GetCacheComponent(0) as Slider;
        m_kTextProVal = m_kParentEntity.m_kUIPrefab.GetCacheComponent(1) as Text;
        m_kTextTime = m_kParentEntity.m_kUIPrefab.GetCacheComponent(2) as Text;
        m_kTranBuilding = m_kParentEntity.m_kUIPrefab.GetCacheComponent(3) as RectTransform;
        m_kTranRoot = m_kParentEntity.m_kUIPrefab.GetCacheComponent(4) as RectTransform;


        m_imgLine = m_kParentEntity.m_kUIPrefab.GetCacheComponent(5) as Image;
        m_kTranPark = m_kParentEntity.m_kUIPrefab.GetCacheComponent(6) as RectTransform;

        m_kBtnSpeedUp = m_kParentEntity.m_kUIPrefab.GetCacheComponent(7) as Button;
        m_kTextSpeedUp = m_kParentEntity.m_kUIPrefab.GetCacheComponent(8) as Text;

        m_ParkItem = m_kParentEntity.m_kUIPrefab.GetCacheComponent(9) as RectTransform;
        m_rawImageParkItemIcon = m_kParentEntity.m_kUIPrefab.GetCacheComponent(10) as RawImage;

        m_BtnParkItem = m_kParentEntity.m_kUIPrefab.GetCacheComponent(11) as Button;
        m_TextParkItem = m_kParentEntity.m_kUIPrefab.GetCacheComponent(12) as Text;

        m_tranAssistant = m_kParentEntity.m_kUIPrefab.GetCacheComponent(13) as RectTransform;
        m_proAssistant = m_kParentEntity.m_kUIPrefab.GetCacheComponent(14) as Slider;
        m_textTimeAssistant = m_kParentEntity.m_kUIPrefab.GetCacheComponent(15) as Text;

        m_tranWasteland = m_kParentEntity.m_kUIPrefab.GetCacheComponent(16) as RectTransform;
        m_proWasteland = m_kParentEntity.m_kUIPrefab.GetCacheComponent(17) as Slider;
        m_textTimeWasteland = m_kParentEntity.m_kUIPrefab.GetCacheComponent(18) as Text;
   
        //m_btnWasteland = m_kParentEntity.m_kUIPrefab.GetCacheComponent(19) as Button;
        m_TextWasted = m_kParentEntity.m_kUIPrefab.GetCacheComponent(20) as Text;
 
        m_kTranBuilding.transform.gameObject.SetActive(false);
        m_kTranPark.transform.gameObject.SetActive(false);
        m_tranAssistant.transform.gameObject.SetActive(false);
        m_tranWasteland.transform.gameObject.SetActive(false);


        m_kBtnSpeedUp.onClick.AddListener(onBtnSpeedUp);
        m_BtnParkItem.onClick.AddListener(onBtnParkSpeedUp);
        //m_btnWasteland.onClick.AddListener(onBtnWasteland);
    }

   
    internal void FixedUpdate()
    {

    
    }

    public override void TranslateUI()
    {
        base.TranslateUI();
        m_kTextSpeedUp.text = UI_Helper.GetTextByLanguageID(204);
        m_TextWasted.text = UI_Helper.GetTextByLanguageID(245);
    }


    public override void Dispose()
    {
        base.Dispose();
        m_kBtnSpeedUp.onClick.RemoveAllListeners();
        m_BtnParkItem.onClick.RemoveAllListeners();
        //m_btnWasteland.onClick.RemoveAllListeners();
    }
   
    //typ 1是建筑物
    public void FixPos(Vector3 pos, BarType type)
    {
        Vector2 fixpos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(UI_Helper.UINode.transform as RectTransform, pos, UI_Helper.UICamera, out fixpos);
        if(type == BarType.building)
        {
            m_kTranBuilding.transform.gameObject.SetActive(true);
            m_kTranRoot.anchoredPosition = new Vector2(fixpos.x - 50, fixpos.y + 200);
        }
        else if (type == BarType.park)
            m_kTranRoot.anchoredPosition = new Vector2(fixpos.x, fixpos.y);
        else if (type == BarType.assistant)
            m_kTranRoot.anchoredPosition = new Vector2(fixpos.x, fixpos.y + 120);
        else if (type == BarType.wasted)
            m_kTranRoot.anchoredPosition = new Vector2(fixpos.x, fixpos.y + 120);
    }

    public void FixAngle(float angle, BarType type)
    {
        m_kTranRoot.localEulerAngles = Vector3.zero;
        m_ParkItem.localEulerAngles = Vector3.zero;
        if (type == BarType.park)
        {
            m_kTranRoot.localEulerAngles = new Vector3(0, 0, angle);
            m_ParkItem.localEulerAngles = new Vector3(0, 0, -angle);
        }
      
    }
 
    public void UpdateBuilding(float now, float max)
    {
        float rate = Mathf.Floor(now / max * 1000);
        rate = float.Parse(rate.ToString().Split('.')[0]);
        m_kProgess.value = now / max;
        m_kTextProVal.text = rate / 10 + "%";
        float tVal = max / 3600;
        string ret = tVal.ToString("0.000");
        m_kTextTime.text = ret + "H";
        if (_uI_SpeedUP != null)
            _uI_SpeedUP.UpdateProgress(now, max);
    }

    public void UpdatePark(float now, float max, bool front)
    {
        if (park == null) return;

        GameObject house = ModelManager._instance.GetModleByType(ModelCType.MainBase)[0].go;
        Vector3 screenPos01 = Camera.main.WorldToScreenPoint(house.transform.position);
        Vector3 screenPos02 = Camera.main.WorldToScreenPoint(parkGoBegin.transform.position);
        Vector2 fixpos01, fixpos02;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(UI_Helper.UINode.transform as RectTransform, screenPos01, UI_Helper.UICamera, out fixpos01);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(UI_Helper.UINode.transform as RectTransform, screenPos02, UI_Helper.UICamera, out fixpos02);
        lineLen = Vector2.Distance(fixpos01, fixpos02); 

 

        m_kTranPark.transform.gameObject.SetActive(true);
        float val;
        float rate = now / max;
        if(front)
            val = (rate - 0.5f) * lineLen;
        else
            val = (0.5f - rate) * lineLen;

        int diff = (int)(max - now);
        TimeSpan ts = new TimeSpan(0, 0, diff);
        if(ts.Hours > 0)
            m_TextParkItem.text = ts.Hours + "h." + ts.Minutes + "m." + ts.Seconds + "s";
        else
            m_TextParkItem.text = ts.Minutes + "m." + ts.Seconds + "s";

        m_ParkItem.transform.localPosition = new Vector3(val, m_ParkItem.transform.localPosition.y, 0);

        //float all = (BE.MobileRTSCam.instance.zoomMax + BE.MobileRTSCam.instance.zoomMin) / 2;
        //float zoomRate = BE.MobileRTSCam.instance.zoomCurrent / all;
        //m_ParkItem.transform.localScale = Vector3.one * zoomRate;

        if (_uI_SpeedUP != null)
            _uI_SpeedUP.UpdateProgress(now, max);
    }


    public void UpdateAssistant(float now, float max)
    {
        float diff = max - now;
        if(Mathf.FloorToInt(diff / 60) > 0)
        {
            int Minutes = Mathf.FloorToInt(diff / 60);
            int Seconds = (int)diff - Minutes * 60;
            m_textTimeAssistant.text = Minutes + "m." + Seconds + "s";
        }
        else
        {
            m_textTimeAssistant.text = diff.ToString("0.0") + "s";
        }
 
        m_proAssistant.value = now / max;
    }

    public void UpdateWasteland(float now, float max)
    {
        m_tranWasteland.transform.gameObject.SetActive(true);
        float diff = max - now;
        if (Mathf.FloorToInt(diff / 60) > 0)
        {
            int Minutes = Mathf.FloorToInt(diff / 60);
            int Seconds = (int)diff - Minutes * 60;
            m_textTimeWasteland.text = Minutes + "m." + Seconds + "s";
        }
        else
        {
            m_textTimeWasteland.text = diff.ToString("0.0") + "s";
        }
        m_proWasteland.value = now / max;
    }


    public void PrakMoveChanage()
    {
        if (lineLen == 0) return;
        

        m_imgLine.rectTransform.sizeDelta = new Vector2(lineLen, m_imgLine.rectTransform.sizeDelta.y);
    }

 
    public void SetParkGoBegin(GameObject obj)
    {
        //lineLen = len;
        parkGoBegin = obj;
    }
    public void InitBuilding(BaseData _building)
    {
        building = _building;
    }

    public void InitPark(int cid, bool front, Park _park, float dis)
    {
        FixAngle(0, BarType.park);
        park = _park;
        parkMoveTime = park.GetParkCapacity() * dis / int.Parse(DBManager.Instance.m_kDisperse.GetEntryPtr(10000)._Val1);
        //front 正面
        CS_Model.DataEntry CS_Model = DBManager.Instance.m_kModel.GetEntryPtr(cid);
        float x;
        if (front)
        {
            x = lineLen / 2;
            m_rawImageParkItemIcon.transform.localEulerAngles = Vector3.zero;
            m_TextParkItem.transform.localEulerAngles = Vector3.zero;
        }
        else
        {
            x = -lineLen / 2;
            m_rawImageParkItemIcon.transform.localEulerAngles = new Vector3(0, 180, 0);
            m_TextParkItem.transform.localEulerAngles = Vector3.zero;
            //m_TextParkItem.transform.localEulerAngles = new Vector3(0, 180, 0);
        }



        m_ParkItem.transform.localPosition = new Vector3(x, m_ParkItem.transform.localPosition.y, 0);
        m_rawImageParkItemIcon.texture = UI_Helper.AllocTexture(CS_Model._Icon);
    }




    public void onBtnSpeedUp()
    {
        //Debug.LogError("__~~~");

        UIEntity uien = World.Scene.GetComponent<UIManagerComponent>().Create(UI_PrefabPath.m_sUIPopUpWindow_SpeedUp);
        _uI_SpeedUP = uien.GetComponent<UI_SpeedUP>();
        _uI_SpeedUP.InitGuid(this, building);

        if(GuidanceManager.isGuidancing)
        {
            GuidanceData info = new GuidanceData();
            info.entity = uien; //World.Scene.GetComponent<UIManagerComponent>().Get(UI_PrefabPath.m_sUIPopUpWindow_SpeedUp);
            ObserverHelper<GuidanceData>.SendMessage(MessageMonitorType.GuidanceClickEvent, this, new MessageArgs<GuidanceData>(info));
        }

    }

    //public void onBtnWasteland()
    //{
    //    UIEntity _uien = UI_Helper.ShowConfirmPanel("TextChange",
    //            () => {
    //                ModelManager._instance.assistant.ImmediatelyRest();
    //            },
    //            () => { });
    //    _uien.GetComponent<UI_Confirm>().TextChange(UI_Helper.GetTextByLanguageID(245), UI_Helper.GetTextByLanguageID(246));
 
    //}


    public void onBtnParkSpeedUp()
    {
        
    }

  

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using QTFramework;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CloudPanel : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

    public delegate void EraseEvent(float progress);
    public EraseEvent OnErase;

    public Camera cameraUI;
    [Range(10, 70)]
    public int brushScale;
    public int grad;
    public EraserTexture[] erasers;
    public float clearValue = 0.5f;
    public float waterRatMin = 1.2f;
    public float waterRatMax= 1.5f;
    public Text firstTips;

	// Use this for initialization
	void Awake () {
        foreach(var era in erasers){
            era.Init(cameraUI, brushScale, grad);
        }
        firstTips.gameObject.SetActive(false);
	}


    float progress{
        get{
            float val = 0;
            foreach (var era in erasers)
            {
                val += era.Progress;
            }

            return val / erasers.Length; 
        }
    }

    private void OnEnable()
    {
        if(PlayerPrefs.GetInt("isFirstCloud" + Time.time, 1) == 1)
        {
            firstTips.text = UI_Helper.GetTextByLanguageID(1037);
            StartCoroutine(ShowFirstTips(1.0f));
        }
        else
        {
            firstTips.gameObject.SetActive(false);
        }

        PlayerPrefs.SetInt("isFirstCloud", 0);
    }

    IEnumerator ShowFirstTips(float delay)
    {
        yield return new WaitForSeconds(delay);
        firstTips.gameObject.SetActive(true);
    }

	// Update is called once per frame
	void FixedUpdate () {
        if (isMove)
        {
            foreach (var era in erasers)
            {
                era.OnMouseMove(Input.mousePosition);
            }

            if (progress > clearValue)
            {
                firstTips.gameObject.SetActive(false);
                gameObject.SetActive(false);
                isMove = false;
                Weather.Instance.ClearCloud();

                int rainDayCount = Weather.Instance.RainDayCount;
                float waterNeedTotal = float.Parse(DBManager.Instance.m_kDisperse.GetEntryPtr(10001)._Val1);
                float water = waterNeedTotal * Random.Range(waterRatMin, waterRatMax) / rainDayCount;
                var GamePlayer = World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer;
                GamePlayer.AddWater((decimal)Mathf.Ceil(water));
            }
        }
	}

    bool isMove;

    public void OnPointerDown(PointerEventData data)
    {
        isMove = true;
    }

    public void OnPointerUp(PointerEventData data)
    {
        isMove = false;
        foreach (var era in erasers)
        {
            era.OnPointerUp();
        }
    }

}

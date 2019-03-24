using System.Collections;
using System.Collections.Generic;

using QTFramework;

using UnityEngine;
using UnityEngine.UI;

public class UI_Launch : MonoBehaviour
{

    public Text m_kText_Tips;
    public Slider m_kSlider_Progress;
    public Text m_kText_Begin;
    public Transform m_kTransformBegin;

    public Button m_kButton_Beain;
    public void SetTips(string Tips)
    {
        m_kText_Tips.text = Tips;
    }

    public void SetSlider(float _Progress)
    {
        m_kSlider_Progress.value = _Progress;
    }

    public void onClick_Begin()
    {
        this.gameObject.SetActive(false);
        World.Scene.GetComponent<GameStateMachineComponent>().ChangeState<GameState_LoadingComponent>();
        World.Scene.GetComponent<PlayerManagerComponent>().GamePlayer.SetTaskNumber(TaskType.Login);
    }
}
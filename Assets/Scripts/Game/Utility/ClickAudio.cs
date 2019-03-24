using QTFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;


#if UNITY_EDITOR
using UnityEditor;
[System.Serializable]
[CustomEditor(typeof(ClickAudio))]
public class ED_UIClickAudio : Editor
{
    public ClickAudio m_kUIClickAudio;

    public AudioClip _audioClip;
    void OnEnable()
    {
        m_kUIClickAudio = target as ClickAudio;
    }
    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        _audioClip = EditorGUILayout.ObjectField(null, typeof(AudioClip), false) as AudioClip;
        if (EditorGUI.EndChangeCheck())
        {
            string oo =   AssetDatabase.GetAssetPath(_audioClip);
            m_kUIClickAudio.m_ClickAudio = Path.Combine("Assets/Data/Audio", Path.GetFileName(oo)).Replace("\\", "/");
        }
        EditorGUILayout.LabelField(m_kUIClickAudio.m_ClickAudio);
    }
}
#endif

public class ClickAudio : MonoBehaviour
{
    public string m_ClickAudio = "";
    public void PlayClickAudio()
    {
        if (!string.IsNullOrEmpty(m_ClickAudio))
        {
            World.Scene.GetComponent<AudioManagerComponent>().PlayAudio(AudioChannel.AudioChannelType.SoundEffect, m_ClickAudio);
        }
    }
}

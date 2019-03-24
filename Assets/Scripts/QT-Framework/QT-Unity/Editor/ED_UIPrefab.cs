using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
[CustomEditor(typeof(UIPrefab))]
public class ED_UIPrefab : Editor
{
    public UIPrefab m_kUIPrefab;
    private AudioClip _OpenUIAudioClip;
    private AudioClip _CloseUIAudioClip;
    void OnEnable()
    {
        m_kUIPrefab = target as UIPrefab;
    }

    private int NodeConut = 0;
    public override void OnInspectorGUI()
    {
        if (null == m_kUIPrefab) return;
        //base.OnInspectorGUI();

        NodeConut = m_kUIPrefab.m_kElements.Count;

        GUILayout.Space(10);
        EditorGUILayout.BeginHorizontal();
        GUI.color = Color.white;
        EditorGUILayout.LabelField("UI类型：", GUILayout.Width(50));
        GUI.contentColor = Color.white;
        m_kUIPrefab.m_kUIType = (UILayer)EditorGUILayout.EnumPopup(m_kUIPrefab.m_kUIType, GUILayout.Width(120));
        EditorGUILayout.EndHorizontal();


        GUILayout.Space(5);

        if (m_kUIPrefab.m_kUIType != UILayer.Item)
        {
            EditorGUILayout.BeginHorizontal();
            GUI.color = Color.white;
            EditorGUILayout.LabelField("UI锚点：", GUILayout.Width(50));
            GUI.contentColor = Color.white;
            m_kUIPrefab.m_kUIAnchorType = (UIAnchorType)EditorGUILayout.EnumPopup(m_kUIPrefab.m_kUIAnchorType, GUILayout.Width(120));
            EditorGUILayout.EndHorizontal();
        }



        GUILayout.Space(5);
        EditorGUILayout.BeginHorizontal();
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.LabelField("打开音效：", GUILayout.Width(50));
        _OpenUIAudioClip = EditorGUILayout.ObjectField(_OpenUIAudioClip, typeof(AudioClip), false, GUILayout.Width(150)) as AudioClip;
        if (EditorGUI.EndChangeCheck())
        {
            string oo = AssetDatabase.GetAssetPath(_OpenUIAudioClip);
            m_kUIPrefab.m_kOpenAudioClipPath = oo.Replace("\\", "/");
        }
        EditorGUILayout.LabelField(m_kUIPrefab.m_kOpenAudioClipPath, GUILayout.MinWidth(200));
        GUI.color = Color.red;
        if (GUILayout.Button("清除", GUILayout.Width(40f)))
        {
            m_kUIPrefab.m_kOpenAudioClipPath = "";
        }
        GUI.color = Color.white;
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(5);

        EditorGUILayout.BeginHorizontal();
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.LabelField("关闭音效：", GUILayout.Width(50));
        _CloseUIAudioClip = EditorGUILayout.ObjectField(_CloseUIAudioClip, typeof(AudioClip), false, GUILayout.Width(150)) as AudioClip;
        if (EditorGUI.EndChangeCheck())
        {
            string oo = AssetDatabase.GetAssetPath(_CloseUIAudioClip);
            m_kUIPrefab.m_kCloseAudioClipPath = oo.Replace("\\", "/");
        }
        EditorGUILayout.LabelField(m_kUIPrefab.m_kCloseAudioClipPath, GUILayout.MinWidth(200));
        GUI.color = Color.red;
        if (GUILayout.Button("清除", GUILayout.Width(40f)))
        {
            m_kUIPrefab.m_kCloseAudioClipPath = "";
        }
        GUI.color = Color.white;
        EditorGUILayout.EndHorizontal();


        GUILayout.Space(10);
        EditorGUILayout.BeginHorizontal();
        GUI.color = Color.white;
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("节点数：", GUILayout.Width(50));
        NodeConut = EditorGUILayout.IntField(NodeConut, GUILayout.Width(60));
        //GUI.color = Color.red;
        if (GUILayout.Button("添加", GUILayout.Width(40f)))
        {
            NodeConut++;
        }
       // GUI.color = Color.white;
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();


        if (NodeConut != m_kUIPrefab.m_kElements.Count)
        {
            if (NodeConut > m_kUIPrefab.m_kElements.Count)
            {
                int count = m_kUIPrefab.m_kElements.Count;
                for (int m =0; m < NodeConut - count; m++)
                {
                    m_kUIPrefab.m_kElements.Add(null);
                }
            }
            else if (NodeConut < m_kUIPrefab.m_kElements.Count)
            {
                int count = m_kUIPrefab.m_kElements.Count;
                for (int m = 0; m < count - NodeConut; m++)
                {
                    m_kUIPrefab.m_kElements.RemoveAt(m_kUIPrefab.m_kElements.Count-1);
                } 
            }
        }


        int deleteIndex = -1;
        for (int i = 0; i < NodeConut; i++)
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"       {i}：", GUILayout.Width(50));

            if (m_kUIPrefab.m_kElements[i] == null)
            {
                GUI.contentColor = Color.red;
            }
            else
            {
                GUI.contentColor = Color.green;
            }
            m_kUIPrefab.m_kElements[i] = EditorGUILayout.ObjectField(m_kUIPrefab.m_kElements[i], typeof(Component), true, GUILayout.Width(220)) as Component;
            GUI.contentColor = Color.white;

            if (m_kUIPrefab.m_kElements[i] != null)
            {
                Component[] comList = m_kUIPrefab.m_kElements[i].gameObject.GetComponents<Component>();
                List<string> srt = new List<string>();
                for (int k = 0; k < comList.Length; k++)
                {
                    srt.Add(comList[k].GetType().Name);
                }
                int selectindex = 0;
                for (int p = 0; p < srt.Count; p++)
                {
                    if (m_kUIPrefab.m_kElements[i].GetType().Name == srt[p])
                    {
                        selectindex = p;
                    }
                }

                GUI.contentColor = Color.white;
                selectindex = EditorGUILayout.Popup(selectindex, srt.ToArray(), GUILayout.Width(120));
                GUI.color = Color.white;


                m_kUIPrefab.m_kElements[i] = comList[selectindex];
            }

            GUI.color = Color.red;
            if (GUILayout.Button("删除", GUILayout.Width(40f)))
            {
                deleteIndex = i;
            }
            GUI.color = Color.white;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

        }
        if (deleteIndex != -1)
        {
            m_kUIPrefab.m_kElements.RemoveAt(deleteIndex);
        }      
    }

}

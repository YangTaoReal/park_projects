using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
[CustomEditor(typeof(LoopGridLayoutGroup))]
public class ED_LoopGridLayoutGroup : Editor
{
    public LoopGridLayoutGroup m_kLoopGridLayoutGroup;

    void OnEnable()
    {
        m_kLoopGridLayoutGroup = target as LoopGridLayoutGroup;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }

}

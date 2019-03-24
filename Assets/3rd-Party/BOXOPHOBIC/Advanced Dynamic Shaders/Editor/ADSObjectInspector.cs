// Advanced Dynamic Shaders
// Copyright Cristian Pop - https://boxophobic.com/

using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(ADSObject))]

public class ADSVertexPosInspector : Editor 
{
	private ADSObject targetScript;

//	private static readonly string excludeScript = "m_Script";

	private Color bannerColor;
    private string bannerText;

	private GUIStyle titleStyle = new GUIStyle();

	void OnEnable()
    {
		
		targetScript = (ADSObject)target;

        titleStyle.richText = true;
        titleStyle.alignment = TextAnchor.MiddleCenter;

        // Check if Light or Dark Unity Skin
        // Set the Banner and Logo Textures
        if (EditorGUIUtility.isProSkin) 
		{
			bannerColor = new Color (0.870f, 0.870f, 0.870f);
            bannerText = "<size=18><color=#383838><b>ADS</b> Object</color></size>";
        } 
		else 
		{
			bannerColor = new Color (0.250f, 0.250f, 0.250f);
            bannerText = "<size=18><color=#dddddd><b>ADS</b> Object</color></size>";
        }

	}

	public override void OnInspectorGUI()
    {

		DrawBanner ();
//		DrawInspector ();
		DrawWarnings ();

	}

	void DrawBanner()
    {
		
		GUILayout.Space(20);
		var bannerRect = GUILayoutUtility.GetRect(0, 0, 40, 0);
		EditorGUI.DrawRect(bannerRect, bannerColor);
        EditorGUI.LabelField(bannerRect, bannerText, titleStyle);

        if (GUI.Button(bannerRect, "", new GUIStyle()))
		{
			Application.OpenURL("https://docs.google.com/document/d/112srM2CP18r9ePrZJy1WQxNFn4aQ3O0Cog-atM_RiIE/edit#heading=h.c1r4qu7kyv24");
		}
		GUILayout.Space(20);

	}

//	void DrawInspector(){
//
//		serializedObject.Update ();
//
//		DrawPropertiesExcluding (serializedObject, excludeScript);
//
//		serializedObject.ApplyModifiedProperties ();
//
//		GUILayout.Space(20);
//
//	} 

	void DrawWarnings()
    {

		if (targetScript.warningMissingADSMesh == true) 
		{

			EditorGUILayout.HelpBox ("The gameobject should have valid MeshFilter component with a Mesh attached!", MessageType.Warning, true);
			GUILayout.Space (20);

		}

		if (targetScript.warningMissingADSGlobals == true) 
		{

			EditorGUILayout.HelpBox ("A valid ADS Globals gameobject should exist in the scene!", MessageType.Warning, true);
			GUILayout.Space (20);

		}
	}
}

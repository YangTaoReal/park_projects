// Advanced Dynamic Shaders
// Copyright Cristian Pop - https://boxophobic.com/

#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
//using System;

[CanEditMultipleObjects]
[CustomEditor(typeof(ADSGlobals))]

public class ASDGlobalsInspector : Editor 
{
	private ADSGlobals targetScript;

    private string[] excludeProps = new string[] { "m_Script"};

    private Color bannerColor;
    private string bannerText;
	private Texture2D logoImage;

	private GUIStyle titleStyle = new GUIStyle();

	void OnEnable(){

		targetScript = (ADSGlobals)target;

        titleStyle.richText = true;
        titleStyle.alignment = TextAnchor.MiddleCenter;

		// Check if Light or Dark Unity Skin
		// Set the Banner and Logo Textures
		if (EditorGUIUtility.isProSkin) 
		{
            //bannerColor = new Color (0.870f, 0.870f, 0.870f);
            bannerColor = new Color(1f, 0.754f, 0.186f);
            bannerText = "<size=18><color=#383838><b>ADS</b> GLOBALS</color></size>";
            logoImage = Resources.Load ("Boxophobic - LogoDark") as Texture2D;
		} 
		else 
		{
			bannerColor = new Color (0.250f, 0.250f, 0.250f);
            bannerText = "<size=18><color=#dddddd><b>ADS</b> GLOBALS</color></size>";
            logoImage = Resources.Load ("Boxophobic - LogoLight") as Texture2D;
		}

	}

	public override void OnInspectorGUI(){
		

		DrawBanner ();
		DrawInspector ();
//		DrawWarnings ();
		DrawLogo ();


	}

	void DrawBanner(){
		
		GUILayout.Space(20);
		var bannerRect = GUILayoutUtility.GetRect(0, 0, 40, 0);
		EditorGUI.DrawRect(bannerRect, bannerColor);
        EditorGUI.LabelField(bannerRect, bannerText, titleStyle);

		if (GUI.Button(bannerRect, "", new GUIStyle()))
		{
			Application.OpenURL("https://docs.google.com/document/d/13vul0zDF478he8hhteKjnxoLYgfW47G0Z9TSox21_J0/edit#heading=h.rp8ji698m9wz");
		}
		GUILayout.Space(20);

	}

	void DrawInspector(){

		serializedObject.Update ();

        if (targetScript.grassTintMode == ADSGlobals.GrassTintModeEnum.colors)
        {
            excludeProps = new string[] { "m_Script"};
        }
        else
        {
            excludeProps = new string[] { "m_Script", "grassTintColorOne", "grassTintColorTwo" };
        }

        DrawPropertiesExcluding(serializedObject, excludeProps);

        serializedObject.ApplyModifiedProperties ();

		GUILayout.Space (20);

	} 

//	void DrawWarnings(){
//
//	}

	void DrawLogo(){

		GUILayout.BeginHorizontal();
		GUILayout.Label ("");

		if (GUILayout.Button (logoImage, GUI.skin.label)) 
		{
			Application.OpenURL ("https://boxophobic.com/");
		}

		GUILayout.EndHorizontal ();
		GUILayout.Space (20);

	}


}

#endif
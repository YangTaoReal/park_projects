using System.Collections;
using System.Collections.Generic;

using BE;

using UnityEditor;

using UnityEngine;

[CustomEditor (typeof (MobileRTSCam))]
public class MobileRTSCamEditor : Editor
{

	private static MobileRTSCam instance;
	private GUIContent cont;

	void Awake ( )
	{
		instance = (MobileRTSCam) target;
		EditorUtility.SetDirty (instance);
	}

	public override void OnInspectorGUI ( )
	{
		instance = (MobileRTSCam) target;
		GUI.changed = false;

		EditorGUILayout.Space ( );

        /*		GUILayout.BeginHorizontal();
				GUILayout.Label("Zoom");
				cont=new GUIContent("Min", "");
				instance.zoomMin=EditorGUILayout.FloatField(cont, instance.zoomMin);
				GUILayout.Label(" ~ ");
				cont=new GUIContent("Max", "");
				instance.zoomMax=EditorGUILayout.FloatField(cont, instance.zoomMax);
				GUILayout.EndHorizontal();
		*/
        
        cont = new GUIContent("CameraFocusTime", "");
        instance.CameraFocusTime = EditorGUILayout.FloatField(cont, instance.CameraFocusTime);
        cont = new GUIContent ("Zoom Min", "");
		instance.zoomMin = EditorGUILayout.FloatField (cont, instance.zoomMin);
		cont = new GUIContent ("Zoom Max", "");
		instance.zoomMax = EditorGUILayout.FloatField (cont, instance.zoomMax);
		cont = new GUIContent ("zoom Focus", "");
		instance.zoomFocus = EditorGUILayout.FloatField (cont, instance.zoomFocus);
		cont = new GUIContent ("Zoom Speed", "");
		instance.zoomSpeed = EditorGUILayout.FloatField (cont, instance.zoomSpeed);
		cont = new GUIContent ("zoom Start", "");
		instance.zoomStart = EditorGUILayout.FloatField (cont, instance.zoomStart);

		cont = new GUIContent ("Long Tab Periodd", "");
		instance.LongTabPeriod = EditorGUILayout.FloatField (cont, instance.LongTabPeriod);

		cont = new GUIContent ("Inertia Use", "");
		instance.InertiaUse = EditorGUILayout.Toggle (cont, instance.InertiaUse);

		cont = new GUIContent ("XRotation Use", "");
		instance.UseXRotation = EditorGUILayout.Toggle (cont, instance.UseXRotation);

		cont = new GUIContent ("YRotation Use", "");
		instance.UseYRotation = EditorGUILayout.Toggle (cont, instance.UseYRotation);

		instance.borderType = (BorderType) EditorGUILayout.EnumPopup ("Border Type", instance.borderType);
		if (instance.borderType == BorderType.Rect)
		{
			cont = new GUIContent ("X Min", "");
			instance.XMin = EditorGUILayout.FloatField (cont, instance.XMin);
			cont = new GUIContent ("X Max", "");
			instance.XMax = EditorGUILayout.FloatField (cont, instance.XMax);
			cont = new GUIContent ("Z Min", "");
			instance.ZMin = EditorGUILayout.FloatField (cont, instance.ZMin);
			cont = new GUIContent ("Z Max", "");
			instance.ZMax = EditorGUILayout.FloatField (cont, instance.ZMax);
		}
		else if (instance.borderType == BorderType.Rect)
		{
			cont = new GUIContent ("Radius", "");
			instance.CircleBorderRadius = EditorGUILayout.FloatField (cont, instance.CircleBorderRadius);
		}
		else
		{}

		EditorGUILayout.Space ( );

		if (GUI.changed)
		{
			EditorUtility.SetDirty (instance);
			//instance.Build();
		}
	}

}

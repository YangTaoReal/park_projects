// Advanced Dynamic Shaders
// Copyright Cristian Pop - https://boxophobic.com/

using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[HelpURL("https://docs.google.com/document/d/13vul0zDF478he8hhteKjnxoLYgfW47G0Z9TSox21_J0/edit#heading=h.rp8ji698m9wz")]
[DisallowMultipleComponent]
[ExecuteInEditMode]
public class ADSObject : MonoBehaviour {

	#if UNITY_EDITOR
//	[HideInInspector]
	public bool warningMissingADSMesh = false;
//	[HideInInspector]
	public bool warningMissingADSGlobals = false;
	#endif

	private Mesh sharedMesh;

	void Awake () {

		// Check for MeshFilter component
		if (gameObject.GetComponent<MeshFilter> () != null && gameObject.GetComponent<MeshFilter> ().sharedMesh != null) 
		{
            #if UNITY_EDITOR
            warningMissingADSMesh = false;
            #endif
		} 
		else 
		{
            #if UNITY_EDITOR
            warningMissingADSMesh = true;
			#endif
			return;

		}

		// Add shared mesh to DynamicMeshes list so UpdateUV3() is only executed once per sharedmesh
		if (GameObject.FindObjectOfType<ADSGlobals>() != null) 
		{	
			#if UNITY_EDITOR
			warningMissingADSGlobals = false;
			#endif

			var ADSGlobals = GameObject.FindObjectOfType<ADSGlobals>().GetComponent<ADSGlobals> ();

			sharedMesh = gameObject.GetComponent<MeshFilter> ().sharedMesh;

			if (ADSGlobals.ADSObjects.Contains (sharedMesh) == false) 
			{

                ADSGlobals.ADSObjects.Add (sharedMesh);

				UpdateUV3 ();

			}



		} 
		else 
		{
			#if UNITY_EDITOR
			warningMissingADSGlobals = true;
			#endif

			return;

		}


	}

	// Copy vertex position to UV3 
	void UpdateUV3(){

		var vertexPos = new List<Vector3>();

		for (int i = 0; i < sharedMesh.vertices.Length; i++) 
		{

			vertexPos.Add (new Vector4 (sharedMesh.vertices [i].x, sharedMesh.vertices [i].y, sharedMesh.vertices [i].z));
		}

        sharedMesh.SetUVs (3, vertexPos);

		vertexPos = new List<Vector3>();

	}


}

// Advanced Dynamic Shaders
// Copyright Cristian Pop - https://boxophobic.com/

using UnityEngine;
using UnityEditor;

public class ADSShaderGUI : ShaderGUI
{

	private float blendMode;
	private float oldBlendMode = -1.0f;
    private Material material;

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
	{

        material = materialEditor.target as Material;

        if (material == null)
            return;

        GUILayout.Space(10);

#if AMPLIFY_SHADER_EDITOR
        if (GUILayout.Button("Open in Shader Editor"))
        {
            AmplifyShaderEditor.AmplifyShaderEditorWindow.ConvertShaderToASE(material.shader);
        }
#endif

        //Check if ADS Globals exist in scene
        if (GameObject.FindObjectOfType<ADSGlobals>() != null)
        {
            if (GUILayout.Button("Global Settings"))
            {
                Selection.activeGameObject = GameObject.FindObjectOfType<ADSGlobals>().gameObject;
            }           
        }

        GUILayout.Space(10);
        base.OnGUI(materialEditor, props);

        if (material.HasProperty("_Mode") == true)
        {
            blendMode = material.GetFloat("_Mode");

            if (oldBlendMode != blendMode)
            {
                SetBlendMode();
            }                
        }

        

        materialEditor.LightmapEmissionProperty(0);

		foreach (Material target in materialEditor.targets)
        {
            target.globalIlluminationFlags &= ~MaterialGlobalIlluminationFlags.EmissiveIsBlack;
        }
			
    }

    void SetBlendMode()
    {
        if (blendMode == 0)
        {
            material.SetOverrideTag("RenderType", "");
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            material.SetInt("_ZWrite", 1);
            material.renderQueue = -1;

            material.EnableKeyword("_RENDERTYPE_OPAQUE");
            material.DisableKeyword("_RENDERTYPE_CUT");
            material.DisableKeyword("_RENDERTYPE_FADE");
            material.DisableKeyword("_RENDERTYPE_TRANSPARENT");
        }

        if (blendMode == 1)
        {
            material.SetOverrideTag("RenderType", "TransparentCutout");
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            material.SetInt("_ZWrite", 1);
            material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;

            material.DisableKeyword("_RENDERTYPE_OPAQUE");
            material.EnableKeyword("_RENDERTYPE_CUT");
            material.DisableKeyword("_RENDERTYPE_FADE");
            material.DisableKeyword("_RENDERTYPE_TRANSPARENT");
        }

        if (blendMode == 2)
        {
            material.SetOverrideTag("RenderType", "Transparent");
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            material.SetInt("_ZWrite", 0);
            material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

            material.DisableKeyword("_RENDERTYPE_OPAQUE");
            material.DisableKeyword("_RENDERTYPE_CUT");
            material.EnableKeyword("_RENDERTYPE_FADE");
            material.DisableKeyword("_RENDERTYPE_TRANSPARENT");
        }

        if (blendMode == 3)
        {
            material.SetOverrideTag("RenderType", "Transparent");
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            material.SetInt("_ZWrite", 0);
            material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

            material.DisableKeyword("_RENDERTYPE_OPAQUE");
            material.DisableKeyword("_RENDERTYPE_CUT");
            material.DisableKeyword("_RENDERTYPE_FADE");
            material.EnableKeyword("_RENDERTYPE_TRANSPARENT");            
        }

        oldBlendMode = blendMode;
    }
}

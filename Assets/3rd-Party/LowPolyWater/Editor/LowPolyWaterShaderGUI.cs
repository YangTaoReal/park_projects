using System;
using UnityEditor;
using UnityEngine;

public class LowPolyWaterShaderGUI : ShaderGUI {

    enum Shading { Flat, VertexLit, PixelLit};

    MaterialProperty _Color = null;
    MaterialProperty _Opacity = null;
	MaterialProperty _Gloss = null;
	MaterialProperty _Specular = null;
	MaterialProperty _SpecColor = null;
	MaterialProperty _Smoothness = null;
	MaterialProperty _FresnelTex = null;
    MaterialProperty _Shading = null;

	MaterialProperty _Waves = null;
	MaterialProperty _Length = null;
	MaterialProperty _Stretch = null;
	MaterialProperty _Speed = null;
	MaterialProperty _Height = null;
	MaterialProperty _Steepness = null;
	MaterialProperty _Direction = null;

	MaterialProperty _RSpeed = null;
	MaterialProperty _RHeight = null;

	MaterialProperty _EdgeBlend = null;
	MaterialProperty _ShoreColor = null;
	MaterialProperty _ShoreIntensity = null;
    MaterialProperty _ShoreDistance = null;

    MaterialProperty _NoiseTex = null;
    MaterialProperty _ZWrite = null;

    MaterialProperty __Direction = null;
    MaterialProperty __Scale = null;
    MaterialProperty __RHeight = null;
    MaterialProperty __RSpeed = null;
    MaterialProperty __TexSize = null;
    MaterialProperty __Speed = null;
    MaterialProperty __Height = null;

    static readonly GUIContent fresnelLbl = new GUIContent("Fresnel (A)");
    static readonly GUIContent noiseLbl = new GUIContent("Noise Texture (A)");

    public void FindProperties(MaterialProperty[] props) {
        _Color = FindProperty("_Color", props);
        _Opacity = FindProperty("_Opacity", props);
        _Gloss = FindProperty("_Gloss", props);
        _Specular = FindProperty("_Specular", props);
        _SpecColor = FindProperty("_SpecColor", props);
        _Smoothness = FindProperty("_Smoothness", props);
        _FresnelTex = FindProperty("_FresnelTex", props);
        _Shading = FindProperty("_Shading", props);

        _Waves = FindProperty("_Waves", props);
        _Length = FindProperty("_Length", props);
        _Stretch = FindProperty("_Stretch", props);
        _Speed = FindProperty("_Speed", props);
        _Height = FindProperty("_Height", props);
        _Steepness = FindProperty("_Steepness", props);
        _Direction = FindProperty("_Direction", props);

        _RSpeed = FindProperty("_RSpeed", props);
        _RHeight = FindProperty("_RHeight", props);

        _EdgeBlend = FindProperty("_EdgeBlend", props);
        _ShoreColor = FindProperty("_ShoreColor", props);
        _ShoreIntensity = FindProperty("_ShoreIntensity", props);
        _ShoreDistance = FindProperty("_ShoreDistance", props);

        _NoiseTex = FindProperty("_NoiseTex", props);
        _ZWrite = FindProperty("_ZWrite", props);

        __Direction = FindProperty("_Direction_", props);
        __Scale = FindProperty("_Scale_", props);
        __RHeight = FindProperty("_RHeight_", props);
        __RSpeed = FindProperty("_RSpeed_", props);
        __TexSize = FindProperty("_TexSize_", props);
        __Speed = FindProperty("_Speed_", props);
        __Height = FindProperty("_Height_", props);
    }

    const int space = 10;

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props) {
        FindProperties(props);

        materialEditor.SetDefaultGUIWidths();
        materialEditor.UseDefaultMargins();
        var scale = __Scale.floatValue;

        GUILayout.Label("Lighting", EditorStyles.boldLabel);
        materialEditor.ShaderProperty(_Color, _Color.displayName);
        materialEditor.ShaderProperty(_Opacity, _Opacity.displayName);
        materialEditor.ShaderProperty(_Gloss, _Gloss.displayName);
        materialEditor.ShaderProperty(_Specular, _Specular.displayName);
        materialEditor.ShaderProperty(_SpecColor, _SpecColor.displayName);
        materialEditor.ShaderProperty(_Smoothness, _Smoothness.displayName);
        materialEditor.TexturePropertySingleLine(fresnelLbl, _FresnelTex);
        EditorGUIUtility.labelWidth -= 30f;
        materialEditor.ShaderProperty(_Shading, _Shading.displayName);
        EditorGUIUtility.labelWidth += 30f;

        GUILayout.Space(space);

        GUILayout.Label("Ripples", EditorStyles.boldLabel);
        materialEditor.ShaderProperty(_RSpeed, _RSpeed.displayName);
        materialEditor.ShaderProperty(_RHeight, _RHeight.displayName);
        __RHeight.floatValue = _RHeight.floatValue * scale;
        __RSpeed.floatValue = _RSpeed.floatValue * scale;

        GUILayout.Space(space);

        GUILayout.Label("Waves", EditorStyles.boldLabel);
        EditorGUIUtility.labelWidth -= 30f;
        materialEditor.ShaderProperty(_Waves, _Waves.displayName);
        EditorGUIUtility.labelWidth += 30f;
        EditorGUI.BeginDisabledGroup(_Waves.floatValue < .5f);
            materialEditor.ShaderProperty(_Length, _Length.displayName);
            materialEditor.ShaderProperty(_Stretch, _Stretch.displayName);
            materialEditor.ShaderProperty(_Speed, _Speed.displayName);
            materialEditor.ShaderProperty(_Height, _Height.displayName);
            materialEditor.ShaderProperty(_Steepness, _Steepness.displayName);
            materialEditor.ShaderProperty(_Direction, _Direction.displayName);
            var steepness = _Steepness.floatValue * _Length.floatValue;
            var angle = Mathf.Deg2Rad * _Direction.floatValue;
            var cos = Mathf.Cos(angle);
            var sin = Mathf.Sin(angle);
            __Direction.vectorValue = new Vector4(cos, sin, cos * steepness, sin * steepness);
            __Height.floatValue = _Height.floatValue * scale;
            __Speed.floatValue = _Speed.floatValue * scale;
        EditorGUI.EndDisabledGroup();

        GUILayout.Space(space);

        GUILayout.Label("Shore Blend", EditorStyles.boldLabel);
        materialEditor.ShaderProperty(_EdgeBlend, _EdgeBlend.displayName);
        EditorGUI.BeginDisabledGroup(_EdgeBlend.floatValue < .5f);
            materialEditor.ShaderProperty(_ShoreColor, _ShoreColor.displayName);
            materialEditor.ShaderProperty(_ShoreIntensity, _ShoreIntensity.displayName);
            materialEditor.ShaderProperty(_ShoreDistance, _ShoreDistance.displayName);
        EditorGUI.EndDisabledGroup();

        GUILayout.Space(space);

        GUILayout.Label("Other", EditorStyles.boldLabel);
        materialEditor.TexturePropertySingleLine(noiseLbl, _NoiseTex);
        if(_NoiseTex.textureValue!= null)
            __TexSize.floatValue = _NoiseTex.textureValue.height * scale;
        materialEditor.ShaderProperty(_ZWrite, _ZWrite.displayName);

        //base.OnGUI(materialEditor, props);
    }

}

// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "BOXOPHOBIC/Advanced Dynamic Shaders/Mobile Grass"
{
	Properties
	{
		[Enum(Opaque,0,Cutout,1,Fade,2,Transparent,3)]_Mode("Blend Mode", Float) = 0
		[Enum(Off,0,Front,1,Back,2)]_CullMode("Cull Mode", Float) = 0
		[Space(10)]_Cutoff("Cutout", Range( 0 , 1)) = 0.5
		[Header(Main)]_Color("Main Color", Color) = (1,1,1,1)
		[NoScaleOffset]_MainTex("Main Texture", 2D) = "white" {}
		_BumpScale("Normal Scale", Float) = 1
		[NoScaleOffset]_BumpMap("Normal Texture", 2D) = "bump" {}
		_SpecularColor("Specular Color", Color) = (1,1,1,1)
		_Shininess("Glossiness", Float) = 0
		[Space(10)]_MainUVs("Main UVs", Vector) = (1,1,0,0)
		[Toggle][Header(Globals)]_MotionNoise("Motion Noise", Float) = 1
		[Toggle]_GrassTint("Grass Tint", Float) = 1
		[Toggle]_GrassSize("Grass Size", Float) = 1
		[Header(Motion)]_MotionAmplitude("Motion Amplitude", Float) = 1
		_MotionSpeed("Motion Speed", Float) = 1
		_MotionScale("Motion Scale", Float) = 1
		[Enum(Vertex Red,0,ADS Object,1)][Space(10)]_MaskType("Mask Type", Float) = 0
		[Enum(X Axis,0,Y Axis,1,Z Axis,2)]_MaskAxis("Mask Axis", Float) = 1
		[Space(10)]_MaskMin("Mask Min", Float) = 0
		_MaskMax("Mask Max", Float) = 1
		[HideInInspector]_Show_MaskGeneric("Show_MaskGeneric", Float) = 1
		[HideInInspector]_ZWrite("_ZWrite", Float) = 1
		[HideInInspector]_SrcBlend("_SrcBlend", Float) = 1
		[HideInInspector]_DstBlend("_DstBlend", Float) = 10
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		LOD 200
		Cull [_CullMode]
		ZWrite [_ZWrite]
		Blend [_SrcBlend] [_DstBlend]
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#include "UnityStandardUtils.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#pragma multi_compile_instancing
		#pragma shader_feature _RENDERTYPE_OPAQUE _RENDERTYPE_CUT _RENDERTYPE_FADE _RENDERTYPE_TRANSPARENT
		#include "VS_indirect.cginc"
		#pragma instancing_options procedural:setup
		#pragma multi_compile GPU_FRUSTUM_ON __
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float3 worldPos;
			float2 uv_texcoord;
			float3 vertexToFrag49_g887;
			float3 worldNormal;
			INTERNAL_DATA
			float4 vertexToFrag1088;
			half ASEVFace : VFACE;
		};

		struct SurfaceOutputCustomLightingCustom
		{
			half3 Albedo;
			half3 Normal;
			half3 Emission;
			half Metallic;
			half Smoothness;
			half Occlusion;
			half Alpha;
			Input SurfInput;
			UnityGIInput GIData;
		};

		uniform half _DstBlend;
		uniform half _SrcBlend;
		uniform half _CullMode;
		uniform half _Cutoff;
		uniform half _Mode;
		uniform half _ZWrite;
		uniform half ADS_GlobalScale;
		uniform half _MotionScale;
		uniform half ADS_GlobalSpeed;
		uniform half _MotionSpeed;
		uniform half ADS_GlobalAmplitude;
		uniform half _MotionAmplitude;
		uniform half ADS_NoiseTex_ON;
		uniform float _MotionNoise;
		uniform sampler2D ADS_NoiseTex;
		uniform half ADS_NoiseSpeed;
		uniform half3 ADS_GlobalDirection;
		uniform half ADS_NoiseScale;
		uniform half ADS_NoiseContrast;
		uniform half _Show_MaskGeneric;
		uniform half _MaskAxis;
		uniform half _MaskType;
		uniform half _MaskMin;
		uniform half _MaskMax;
		uniform half ADS_GrassSizeTex_ON;
		uniform half _GrassSize;
		uniform half ADS_GrassSizeMin;
		uniform half ADS_GrassSizeMax;
		uniform sampler2D ADS_GrassSizeTex;
		uniform half4 ADS_GrassSizeScaleOffset;
		uniform sampler2D _MainTex;
		uniform half4 _MainUVs;
		uniform half4 _Color;
		uniform half ADS_GrassTintTex_ON;
		uniform half _GrassTint;
		uniform sampler2D ADS_GrassTintTex;
		uniform half4 ADS_GrassTintScaleOffset;
		uniform half4 ADS_GrassTintColorOne;
		uniform half4 ADS_GrassTintColorTwo;
		uniform half ADS_GrassTintModeColors;
		uniform half ADS_GrassTintIntensity;
		uniform half _BumpScale;
		uniform sampler2D _BumpMap;
		uniform half _Shininess;
		uniform half4 _SpecularColor;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			half MotionScale60_g916 = ( ADS_GlobalScale * _MotionScale );
			half MotionSpeed62_g916 = ( ADS_GlobalSpeed * _MotionSpeed );
			float mulTime90_g916 = _Time.y * MotionSpeed62_g916;
			float2 appendResult115_g913 = (float2(ase_worldPos.x , ase_worldPos.z));
			float2 panner73_g913 = ( _Time.y * ( ADS_NoiseSpeed * (-ADS_GlobalDirection).xz ) + ( appendResult115_g913 * ADS_NoiseScale ));
			float ifLocalVar94_g913 = 0;
			UNITY_BRANCH 
			if( ( ADS_NoiseTex_ON * _MotionNoise ) > 0.01 )
				ifLocalVar94_g913 = saturate( pow( abs( tex2Dlod( ADS_NoiseTex, float4( panner73_g913, 0, 0.0) ).r ) , ADS_NoiseContrast ) );
			else if( ( ADS_NoiseTex_ON * _MotionNoise ) < 0.01 )
				ifLocalVar94_g913 = 1.0;
			half MotionlAmplitude58_g916 = ( ADS_GlobalAmplitude * _MotionAmplitude * ifLocalVar94_g913 );
			half3 MotionDirection59_g916 = ( ADS_GlobalDirection + 0.0001 );
			float temp_output_25_0_g912 = _MaskAxis;
			float lerpResult24_g912 = lerp( v.texcoord3.xyz.x , v.texcoord3.xyz.y , saturate( temp_output_25_0_g912 ));
			float lerpResult21_g912 = lerp( lerpResult24_g912 , v.texcoord3.xyz.z , step( 2.0 , temp_output_25_0_g912 ));
			half THREE27_g912 = lerpResult21_g912;
			float lerpResult42_g910 = lerp( v.color.r , THREE27_g912 , _MaskType);
			float temp_output_7_0_g911 = _MaskMin;
			float lerpResult31_g910 = lerp( 0.0 , 1.0 , saturate( ( ( lerpResult42_g910 - temp_output_7_0_g911 ) / ( _MaskMax - temp_output_7_0_g911 ) ) ));
			half MotionMask137_g916 = ( _Show_MaskGeneric * lerpResult31_g910 );
			float lerpResult116_g915 = lerp( ADS_GrassSizeMin , ADS_GrassSizeMax , tex2Dlod( ADS_GrassSizeTex, float4( ( ( (ase_worldPos).xz * (ADS_GrassSizeScaleOffset).xy ) + (ADS_GrassSizeScaleOffset).zw ), 0, 0.0) ).r);
			float3 temp_cast_3 = (0.0).xxx;
			float3 ifLocalVar96_g915 = 0;
			UNITY_BRANCH 
			if( ( ADS_GrassSizeTex_ON * _GrassSize ) > 0.5 )
				ifLocalVar96_g915 = ( lerpResult116_g915 * v.texcoord3.xyz );
			else if( ( ADS_GrassSizeTex_ON * _GrassSize ) < 0.5 )
				ifLocalVar96_g915 = temp_cast_3;
			v.vertex.xyz += ( mul( unity_WorldToObject, float4( ( ( ( ( sin( ( ( ( ( ase_worldPos + (ase_worldPos).zxy ) * MotionScale60_g916 ) + mulTime90_g916 ) + ( v.color.g * 1.756 ) ) ) * MotionlAmplitude58_g916 ) + ( MotionlAmplitude58_g916 * saturate( MotionScale60_g916 ) ) ) * MotionDirection59_g916 ) * MotionMask137_g916 ) , 0.0 ) ).xyz + ifLocalVar96_g915 );
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_worldlightDir = 0;
			#else //aseld
			float3 ase_worldlightDir = Unity_SafeNormalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			float3 ase_worldNormal = UnityObjectToWorldNormal( v.normal );
			half3 NORMAL62_g887 = ase_worldNormal;
			float3 normalizeResult3_g887 = normalize( NORMAL62_g887 );
			float dotResult5_g887 = dot( ase_worldlightDir , normalizeResult3_g887 );
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aselc
			float4 ase_lightColor = 0;
			#else //aselc
			float4 ase_lightColor = _LightColor0;
			#endif //aselc
			float3 temp_output_8_0_g887 = ( max( dotResult5_g887 , 0.0 ) * ( ase_lightColor.rgb * ase_lightColor.a ) );
			o.vertexToFrag49_g887 = temp_output_8_0_g887;
			float4 temp_cast_4 = (1.0).xxxx;
			float4 tex2DNode75_g885 = tex2Dlod( ADS_GrassTintTex, float4( ( ( (ase_worldPos).xz * (ADS_GrassTintScaleOffset).xy ) + (ADS_GrassTintScaleOffset).zw ), 0, 0.0) );
			float4 lerpResult115_g885 = lerp( ADS_GrassTintColorOne , ADS_GrassTintColorTwo , tex2DNode75_g885.r);
			float4 lerpResult121_g885 = lerp( tex2DNode75_g885 , lerpResult115_g885 , ADS_GrassTintModeColors);
			float4 lerpResult126_g885 = lerp( temp_cast_4 , ( lerpResult121_g885 * ADS_GrassTintIntensity ) , saturate( ADS_GrassTintIntensity ));
			float4 temp_cast_5 = (1.0).xxxx;
			float4 ifLocalVar96_g885 = 0;
			UNITY_BRANCH 
			if( ( ADS_GrassTintTex_ON * _GrassTint ) > 0.5 )
				ifLocalVar96_g885 = saturate( lerpResult126_g885 );
			else if( ( ADS_GrassTintTex_ON * _GrassTint ) < 0.5 )
				ifLocalVar96_g885 = temp_cast_5;
			o.vertexToFrag1088 = ifLocalVar96_g885;
		}

		inline half4 LightingStandardCustomLighting( inout SurfaceOutputCustomLightingCustom s, half3 viewDir, UnityGI gi )
		{
			UnityGIInput data = s.GIData;
			Input i = s.SurfInput;
			half4 c = 0;
			#ifdef UNITY_PASS_FORWARDBASE
			float ase_lightAtten = data.atten;
			if( _LightColor0.a == 0)
			ase_lightAtten = 0;
			#else
			float3 ase_lightAttenRGB = gi.light.color / ( ( _LightColor0.rgb ) + 0.000001 );
			float ase_lightAtten = max( max( ase_lightAttenRGB.r, ase_lightAttenRGB.g ), ase_lightAttenRGB.b );
			#endif
			#if defined(HANDLE_SHADOWS_BLENDING_IN_GI)
			half bakedAtten = UnitySampleBakedOcclusion(data.lightmapUV.xy, data.worldPos);
			float zDist = dot(_WorldSpaceCameraPos - data.worldPos, UNITY_MATRIX_V[2].xyz);
			float fadeDist = UnityComputeShadowFadeDistance(data.worldPos, zDist);
			ase_lightAtten = UnityMixRealtimeAndBakedShadows(data.atten, bakedAtten, UnityComputeShadowFade(fadeDist));
			#endif
			float2 appendResult564 = (float2(_MainUVs.x , _MainUVs.y));
			float2 appendResult565 = (float2(_MainUVs.z , _MainUVs.w));
			half2 MainUVs587 = ( ( i.uv_texcoord * appendResult564 ) + appendResult565 );
			float4 tex2DNode18 = tex2D( _MainTex, MainUVs587 );
			half MainTexAlpha616 = tex2DNode18.a;
			half MainColorAlpha1057 = _Color.a;
			float temp_output_1_0_g884 = ( MainTexAlpha616 * MainColorAlpha1057 );
			#if defined(_RENDERTYPE_OPAQUE)
				float staticSwitch15_g884 = 1.0;
			#elif defined(_RENDERTYPE_CUT)
				float staticSwitch15_g884 = 1.0;
			#elif defined(_RENDERTYPE_FADE)
				float staticSwitch15_g884 = temp_output_1_0_g884;
			#elif defined(_RENDERTYPE_TRANSPARENT)
				float staticSwitch15_g884 = temp_output_1_0_g884;
			#else
				float staticSwitch15_g884 = 1.0;
			#endif
			#if defined(_RENDERTYPE_OPAQUE)
				float staticSwitch23_g884 = 1.0;
			#elif defined(_RENDERTYPE_CUT)
				float staticSwitch23_g884 = temp_output_1_0_g884;
			#elif defined(_RENDERTYPE_FADE)
				float staticSwitch23_g884 = 1.0;
			#elif defined(_RENDERTYPE_TRANSPARENT)
				float staticSwitch23_g884 = 1.0;
			#else
				float staticSwitch23_g884 = 1.0;
			#endif
			half4 MainTex487 = tex2DNode18;
			half4 MainColor486 = _Color;
			#if defined(_RENDERTYPE_OPAQUE)
				float staticSwitch24_g884 = 1.0;
			#elif defined(_RENDERTYPE_CUT)
				float staticSwitch24_g884 = 1.0;
			#elif defined(_RENDERTYPE_FADE)
				float staticSwitch24_g884 = 1.0;
			#elif defined(_RENDERTYPE_TRANSPARENT)
				float staticSwitch24_g884 = temp_output_1_0_g884;
			#else
				float staticSwitch24_g884 = 1.0;
			#endif
			half RenderTransparent1096 = staticSwitch24_g884;
			half ATTENUATION77_g887 = ase_lightAtten;
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			half3 NORMAL62_g887 = ase_worldNormal;
			UnityGI gi11_g887 = gi;
			float3 diffNorm11_g887 = NORMAL62_g887;
			gi11_g887 = UnityGI_Base( data, 1, diffNorm11_g887 );
			float3 indirectDiffuse11_g887 = gi11_g887.indirect.diffuse + diffNorm11_g887 * 0.0001;
			half3 INDIRECT72_g887 = indirectDiffuse11_g887;
			float switchResult12_g886 = (((i.ASEVFace>0)?(1.0):(-1.0)));
			half3 NORMAL620 = ( switchResult12_g886 * UnpackScaleNormal( tex2D( _BumpMap, MainUVs587 ), _BumpScale ) );
			half3 NORMAL99_g914 = WorldNormalVector( i , NORMAL620 );
			float3 normalizeResult73_g914 = normalize( NORMAL99_g914 );
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = Unity_SafeNormalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_worldlightDir = 0;
			#else //aseld
			float3 ase_worldlightDir = Unity_SafeNormalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			float3 normalizeResult40_g914 = normalize( ( ase_worldViewDir + ase_worldlightDir ) );
			float dotResult45_g914 = dot( normalizeResult73_g914 , normalizeResult40_g914 );
			half4 CUSTOM_LIGHTING1082 = ( ( ( MainTex487 * MainColor486 ) * RenderTransparent1096 * float4( ( ( i.vertexToFrag49_g887 * ATTENUATION77_g887 ) + INDIRECT72_g887 ) , 0.0 ) * i.vertexToFrag1088 ) + ( pow( max( dotResult45_g914 , 0.0 ) , ( 128.0 * max( _Shininess , 0.01 ) ) ) * _SpecularColor ) );
			c.rgb = CUSTOM_LIGHTING1082.rgb;
			c.a = staticSwitch15_g884;
			clip( staticSwitch23_g884 - _Cutoff );
			return c;
		}

		inline void LightingStandardCustomLighting_GI( inout SurfaceOutputCustomLightingCustom s, UnityGIInput data, inout UnityGI gi )
		{
			s.GIData = data;
		}

		void surf( Input i , inout SurfaceOutputCustomLightingCustom o )
		{
			o.SurfInput = i;
			o.Normal = float3(0,0,1);
			float2 appendResult564 = (float2(_MainUVs.x , _MainUVs.y));
			float2 appendResult565 = (float2(_MainUVs.z , _MainUVs.w));
			half2 MainUVs587 = ( ( i.uv_texcoord * appendResult564 ) + appendResult565 );
			float4 tex2DNode18 = tex2D( _MainTex, MainUVs587 );
			half4 MainTex487 = tex2DNode18;
			o.Albedo = MainTex487.rgb;
		}

		ENDCG
		CGPROGRAM
		#pragma exclude_renderers d3d9 gles 
		#pragma surface surf StandardCustomLighting keepalpha fullforwardshadows noambient novertexlights noforwardadd vertex:vertexDataFunc 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float3 customPack2 : TEXCOORD2;
				float4 customPack3 : TEXCOORD3;
				float4 tSpace0 : TEXCOORD4;
				float4 tSpace1 : TEXCOORD5;
				float4 tSpace2 : TEXCOORD6;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				vertexDataFunc( v, customInputData );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.customPack2.xyz = customInputData.vertexToFrag49_g887;
				o.customPack3.xyzw = customInputData.vertexToFrag1088;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				surfIN.vertexToFrag49_g887 = IN.customPack2.xyz;
				surfIN.vertexToFrag1088 = IN.customPack3.xyzw;
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				SurfaceOutputCustomLightingCustom o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputCustomLightingCustom, o )
				surf( surfIN, o );
				UnityGI gi;
				UNITY_INITIALIZE_OUTPUT( UnityGI, gi );
				o.Alpha = LightingStandardCustomLighting( o, worldViewDir, gi ).a;
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Mobile/Bumped Specular"
	CustomEditor "ADSShaderGUI"
}
/*ASEBEGIN
Version=15500
1927;29;1906;1014;2592.783;3646.085;2.475593;True;False
Node;AmplifyShaderEditor.CommentaryNode;712;-1320.379,-944;Float;False;1101.309;480.4385;Main UVs;7;587;575;562;565;564;561;563;MAIN LAYER;0.4980392,1,0,1;0;0
Node;AmplifyShaderEditor.Vector4Node;563;-1280,-672;Half;False;Property;_MainUVs;Main UVs;10;0;Create;True;0;0;False;1;Space(10);1,1,0,0;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexCoordVertexDataNode;561;-1280,-896;Float;False;0;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;564;-1024,-672;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;565;-1024,-592;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;562;-832,-896;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;575;-624,-896;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;760;-192,-944;Float;False;1635.028;429.4401;Main Texture and Color;7;588;18;487;616;409;486;1057;;0,0.751724,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;587;-448,-896;Half;False;MainUVs;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;588;-128,-896;Float;False;587;0;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;18;80,-896;Float;True;Property;_MainTex;Main Texture;4;1;[NoScaleOffset];Create;False;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;409;768,-896;Half;False;Property;_Color;Main Color;3;0;Create;False;0;0;False;1;Header(Main);1,1,1,1;1,1,1,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;708;1488,-944;Float;False;1113;294.4401;Normal Texture;5;620;1064;607;604;655;;0,0.751724,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1057;1024,-800;Half;False;MainColorAlpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;655;1536,-768;Half;False;Property;_BumpScale;Normal Scale;5;0;Create;False;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;604;1536,-896;Float;False;587;0;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;616;384,-768;Half;False;MainTexAlpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1059;-1280,-2976;Float;False;1057;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;791;-1280,-3072;Float;False;616;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;1075;-1328,-2096;Float;False;1247.739;600.9233;Custom Lighting;12;1080;1081;1082;1090;1079;1097;1098;1088;1076;1099;1077;1087;LIGHTING;1,0.7686275,0,1;0;0
Node;AmplifyShaderEditor.SamplerNode;607;1808,-896;Float;True;Property;_BumpMap;Normal Texture;6;1;[NoScaleOffset];Create;False;0;0;False;0;None;None;True;0;False;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1058;-1024,-3072;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;487;384,-896;Half;False;MainTex;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;1077;-1280,-2048;Float;False;487;0;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;1099;-1280,-1984;Float;False;486;0;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;1064;2112,-896;Float;False;Switch Back Normal;-1;;886;121446c878db06f4c847f9c5afed7cfe;0;1;13;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;486;1024,-896;Half;False;MainColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;1073;-768,-3072;Float;False;ADS Render Type;-1;;884;fcf5ead2adb2e514895d795dcf8514b1;0;1;1;FLOAT;0;False;3;FLOAT;0;FLOAT;13;FLOAT;25
Node;AmplifyShaderEditor.FunctionNode;1087;-1280,-1728;Float;False;ADS Grass Tint;14;;885;b810a046229fab449ab4dfc3f9df6e7f;0;0;1;COLOR;85
Node;AmplifyShaderEditor.GetLocalVarNode;1097;-1280,-1888;Float;False;1096;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;1079;-1280,-1808;Float;False;Lighting Lambert;-1;;887;3e225d4a5a55ac8439718a5f6f59857f;5,56,0,57,1,75,2,70,2,66,1;4;14;FLOAT3;0,0,0;False;59;FLOAT3;0,0,0;False;82;FLOAT;0;False;85;FLOAT;0;False;1;FLOAT3;12
Node;AmplifyShaderEditor.RegisterLocalVarNode;620;2368,-896;Half;False;NORMAL;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1098;-1024,-2048;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;1076;-1280,-1600;Float;False;620;0;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1096;-256,-2880;Half;False;RenderTransparent;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexToFragmentNode;1088;-1088,-1728;Float;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;1080;-896,-1600;Float;False;Lighting Specular;7;;914;503457513f257784c866265a383a60d7;1,103,2;2;100;FLOAT3;0,0,0;False;102;FLOAT3;0,0,0;False;1;COLOR;12
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1090;-832,-2048;Float;False;4;4;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT3;0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;1053;-1280,-2816;Float;False;ADS Mask Generic;24;;910;2cfc3815568565c4585aebb38bd7a29b;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;1054;-1280,-2752;Float;False;ADS Motion Noise;11;;913;047eb809542f42d40b4b5066e22cee72;0;0;1;FLOAT;85
Node;AmplifyShaderEditor.SimpleAddOpNode;1081;-576,-2048;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;1026;-1024,-2816;Float;False;ADS Motion Global;20;;916;a8838de3869103540a427ac470da4da6;0;2;136;FLOAT;0;False;133;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.FunctionNode;1100;-1024,-2688;Float;False;ADS Grass Size;17;;915;6675a46c54a0e244fb369c824eead1af;0;0;1;FLOAT3;85
Node;AmplifyShaderEditor.CommentaryNode;683;-1328,-3632;Float;False;1496;152;;6;862;925;743;549;550;553;OPTIONS;1,0,0.503,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;549;-896,-3584;Half;False;Property;_Mode;Blend Mode;0;1;[Enum];Create;False;4;Opaque;0;Cutout;1;Fade;2;Transparent;3;0;True;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;862;-128,-3584;Half;False;Property;_Cutoff;Cutout;2;0;Create;False;3;Off;0;Front;1;Back;2;0;True;1;Space(10);0.5;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1082;-368,-2048;Half;False;CUSTOM_LIGHTING;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;925;-384,-3584;Half;False;Property;_ZWrite;_ZWrite;30;1;[HideInInspector];Create;True;2;Off;0;On;1;0;True;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1084;-1280,-3328;Float;False;487;0;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;553;-1120,-3584;Half;False;Property;_DstBlend;_DstBlend;32;1;[HideInInspector];Create;True;0;0;True;0;10;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;1095;-640,-2816;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;550;-1280,-3584;Half;False;Property;_SrcBlend;_SrcBlend;31;1;[HideInInspector];Create;True;0;0;True;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1085;-1280,-3200;Float;False;1082;0;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;743;-640,-3584;Half;False;Property;_CullMode;Cull Mode;1;1;[Enum];Create;True;3;Off;0;Front;1;Back;2;0;True;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-256,-3328;Float;False;True;2;Float;ADSShaderGUI;200;0;CustomLighting;BOXOPHOBIC/Advanced Dynamic Shaders/Mobile Grass;False;False;False;False;True;True;False;False;False;False;False;True;False;False;False;False;True;False;False;False;Off;0;True;925;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0;True;True;0;True;Opaque;;Geometry;All;False;True;True;False;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;1;5;True;550;10;True;553;0;1;False;550;10;False;553;0;True;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;200;Mobile/Bumped Specular;-1;-1;-1;-1;0;False;0;0;True;743;-1;0;True;862;3;Include;VS_indirect.cginc;Pragma;instancing_options procedural:setup;Pragma;multi_compile GPU_FRUSTUM_ON __;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;564;0;563;1
WireConnection;564;1;563;2
WireConnection;565;0;563;3
WireConnection;565;1;563;4
WireConnection;562;0;561;0
WireConnection;562;1;564;0
WireConnection;575;0;562;0
WireConnection;575;1;565;0
WireConnection;587;0;575;0
WireConnection;18;1;588;0
WireConnection;1057;0;409;4
WireConnection;616;0;18;4
WireConnection;607;1;604;0
WireConnection;607;5;655;0
WireConnection;1058;0;791;0
WireConnection;1058;1;1059;0
WireConnection;487;0;18;0
WireConnection;1064;13;607;0
WireConnection;486;0;409;0
WireConnection;1073;1;1058;0
WireConnection;620;0;1064;0
WireConnection;1098;0;1077;0
WireConnection;1098;1;1099;0
WireConnection;1096;0;1073;25
WireConnection;1088;0;1087;85
WireConnection;1080;102;1076;0
WireConnection;1090;0;1098;0
WireConnection;1090;1;1097;0
WireConnection;1090;2;1079;12
WireConnection;1090;3;1088;0
WireConnection;1081;0;1090;0
WireConnection;1081;1;1080;12
WireConnection;1026;136;1053;0
WireConnection;1026;133;1054;85
WireConnection;1082;0;1081;0
WireConnection;1095;0;1026;0
WireConnection;1095;1;1100;85
WireConnection;0;0;1084;0
WireConnection;0;9;1073;0
WireConnection;0;10;1073;13
WireConnection;0;13;1085;0
WireConnection;0;11;1095;0
ASEEND*/
//CHKSM=33BEA615F10797A265771296119D13FDB681B23B
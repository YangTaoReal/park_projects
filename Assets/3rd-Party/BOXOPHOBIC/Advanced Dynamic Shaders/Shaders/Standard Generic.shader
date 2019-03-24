// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "BOXOPHOBIC/Advanced Dynamic Shaders/Standard Generic"
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
		_Metallic("Surface Metallic", Range( 0 , 1)) = 0
		_Glossiness("Surface Smoothness", Range( 0 , 1)) = 0.5
		[NoScaleOffset]_MetallicGlossMap("Surface Texture", 2D) = "white" {}
		[Space(10)]_MainUVs("Main UVs", Vector) = (1,1,0,0)
		[Toggle][Header(Globals)]_MotionNoise("Motion Noise", Float) = 1
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
		LOD 300
		Cull [_CullMode]
		ZWrite [_ZWrite]
		Blend [_SrcBlend] [_DstBlend]
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityStandardUtils.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#pragma multi_compile_instancing
		#pragma shader_feature _RENDERTYPE_OPAQUE _RENDERTYPE_CUT _RENDERTYPE_FADE _RENDERTYPE_TRANSPARENT
		#include "VS_indirect.cginc"
		#pragma instancing_options procedural:setup
		#pragma multi_compile GPU_FRUSTUM_ON __
		struct Input
		{
			float3 worldPos;
			half ASEVFace : VFACE;
			float2 uv_texcoord;
		};

		uniform half _Mode;
		uniform half _CullMode;
		uniform half _Cutoff;
		uniform half _ZWrite;
		uniform half _SrcBlend;
		uniform half _DstBlend;
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
		uniform half _BumpScale;
		uniform sampler2D _BumpMap;
		uniform half4 _MainUVs;
		uniform sampler2D _MainTex;
		uniform half4 _Color;
		uniform sampler2D _MetallicGlossMap;
		uniform half _Metallic;
		uniform half _Glossiness;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			half MotionScale60_g889 = ( ADS_GlobalScale * _MotionScale );
			half MotionSpeed62_g889 = ( ADS_GlobalSpeed * _MotionSpeed );
			float mulTime90_g889 = _Time.y * MotionSpeed62_g889;
			float2 appendResult115_g883 = (float2(ase_worldPos.x , ase_worldPos.z));
			float2 panner73_g883 = ( _Time.y * ( ADS_NoiseSpeed * (-ADS_GlobalDirection).xz ) + ( appendResult115_g883 * ADS_NoiseScale ));
			float ifLocalVar94_g883 = 0;
			UNITY_BRANCH 
			if( ( ADS_NoiseTex_ON * _MotionNoise ) > 0.01 )
				ifLocalVar94_g883 = saturate( pow( abs( tex2Dlod( ADS_NoiseTex, float4( panner73_g883, 0, 0.0) ).r ) , ADS_NoiseContrast ) );
			else if( ( ADS_NoiseTex_ON * _MotionNoise ) < 0.01 )
				ifLocalVar94_g883 = 1.0;
			half MotionlAmplitude58_g889 = ( ADS_GlobalAmplitude * _MotionAmplitude * ifLocalVar94_g883 );
			half3 MotionDirection59_g889 = ( ADS_GlobalDirection + 0.0001 );
			float temp_output_25_0_g886 = _MaskAxis;
			float lerpResult24_g886 = lerp( v.texcoord3.xyz.x , v.texcoord3.xyz.y , saturate( temp_output_25_0_g886 ));
			float lerpResult21_g886 = lerp( lerpResult24_g886 , v.texcoord3.xyz.z , step( 2.0 , temp_output_25_0_g886 ));
			half THREE27_g886 = lerpResult21_g886;
			float lerpResult42_g884 = lerp( v.color.r , THREE27_g886 , _MaskType);
			float temp_output_7_0_g885 = _MaskMin;
			float lerpResult31_g884 = lerp( 0.0 , 1.0 , saturate( ( ( lerpResult42_g884 - temp_output_7_0_g885 ) / ( _MaskMax - temp_output_7_0_g885 ) ) ));
			half MotionMask137_g889 = ( _Show_MaskGeneric * lerpResult31_g884 );
			v.vertex.xyz += mul( unity_WorldToObject, float4( ( ( ( ( sin( ( ( ( ( ase_worldPos + (ase_worldPos).zxy ) * MotionScale60_g889 ) + mulTime90_g889 ) + ( v.color.g * 1.756 ) ) ) * MotionlAmplitude58_g889 ) + ( MotionlAmplitude58_g889 * saturate( MotionScale60_g889 ) ) ) * MotionDirection59_g889 ) * MotionMask137_g889 ) , 0.0 ) ).xyz;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float switchResult12_g888 = (((i.ASEVFace>0)?(1.0):(-1.0)));
			float2 appendResult564 = (float2(_MainUVs.x , _MainUVs.y));
			float2 appendResult565 = (float2(_MainUVs.z , _MainUVs.w));
			half2 MainUVs587 = ( ( i.uv_texcoord * appendResult564 ) + appendResult565 );
			half3 NORMAL620 = ( switchResult12_g888 * UnpackScaleNormal( tex2D( _BumpMap, MainUVs587 ), _BumpScale ) );
			o.Normal = NORMAL620;
			float4 tex2DNode18 = tex2D( _MainTex, MainUVs587 );
			half4 MainTex487 = tex2DNode18;
			half4 MainColor486 = _Color;
			half MainTexAlpha616 = tex2DNode18.a;
			half MainColorAlpha1057 = _Color.a;
			float temp_output_1_0_g887 = ( MainTexAlpha616 * MainColorAlpha1057 );
			#if defined(_RENDERTYPE_OPAQUE)
				float staticSwitch24_g887 = 1.0;
			#elif defined(_RENDERTYPE_CUT)
				float staticSwitch24_g887 = 1.0;
			#elif defined(_RENDERTYPE_FADE)
				float staticSwitch24_g887 = 1.0;
			#elif defined(_RENDERTYPE_TRANSPARENT)
				float staticSwitch24_g887 = temp_output_1_0_g887;
			#else
				float staticSwitch24_g887 = 1.0;
			#endif
			o.Albedo = ( ( MainTex487 * MainColor486 ) * staticSwitch24_g887 ).rgb;
			float4 tex2DNode645 = tex2D( _MetallicGlossMap, MainUVs587 );
			half SurfaceTexRed646 = tex2DNode645.r;
			half METALLIC748 = ( SurfaceTexRed646 * _Metallic );
			o.Metallic = METALLIC748;
			half SurfaceTexAlpha744 = tex2DNode645.a;
			half SMOOTHNESS660 = ( SurfaceTexAlpha744 * _Glossiness );
			o.Smoothness = SMOOTHNESS660;
			#if defined(_RENDERTYPE_OPAQUE)
				float staticSwitch15_g887 = 1.0;
			#elif defined(_RENDERTYPE_CUT)
				float staticSwitch15_g887 = 1.0;
			#elif defined(_RENDERTYPE_FADE)
				float staticSwitch15_g887 = temp_output_1_0_g887;
			#elif defined(_RENDERTYPE_TRANSPARENT)
				float staticSwitch15_g887 = temp_output_1_0_g887;
			#else
				float staticSwitch15_g887 = 1.0;
			#endif
			o.Alpha = staticSwitch15_g887;
			#if defined(_RENDERTYPE_OPAQUE)
				float staticSwitch23_g887 = 1.0;
			#elif defined(_RENDERTYPE_CUT)
				float staticSwitch23_g887 = temp_output_1_0_g887;
			#elif defined(_RENDERTYPE_FADE)
				float staticSwitch23_g887 = 1.0;
			#elif defined(_RENDERTYPE_TRANSPARENT)
				float staticSwitch23_g887 = 1.0;
			#else
				float staticSwitch23_g887 = 1.0;
			#endif
			clip( staticSwitch23_g887 - _Cutoff );
		}

		ENDCG
		CGPROGRAM
		#pragma exclude_renderers d3d9 gles 
		#pragma surface surf Standard keepalpha fullforwardshadows vertex:vertexDataFunc 

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
				float3 worldPos : TEXCOORD2;
				float4 tSpace0 : TEXCOORD3;
				float4 tSpace1 : TEXCOORD4;
				float4 tSpace2 : TEXCOORD5;
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
				o.worldPos = worldPos;
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
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
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
	Fallback "Standard"
	CustomEditor "ADSShaderGUI"
}
/*ASEBEGIN
Version=15500
1927;29;1906;1014;-2433.486;1316.966;1;True;False
Node;AmplifyShaderEditor.CommentaryNode;712;-1328,-944;Float;False;1124.93;486.9999;Main UVs;7;563;564;561;565;587;575;562;MAIN LAYER;0.4980392,1,0,1;0;0
Node;AmplifyShaderEditor.Vector4Node;563;-1280,-672;Half;False;Property;_MainUVs;Main UVs;10;0;Create;True;0;0;False;1;Space(10);1,1,0,0;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;564;-1024,-672;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;561;-1280,-896;Float;False;0;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;562;-832,-896;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;565;-1024,-592;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;760;-176,-944;Float;False;1635.028;429.4401;Main Texture and Color;7;588;18;487;616;409;486;1057;;0,0.751724,1,1;0;0
Node;AmplifyShaderEditor.SimpleAddOpNode;575;-624,-896;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;587;-448,-896;Half;False;MainUVs;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;588;-128,-896;Float;False;587;0;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;715;2640,-944;Float;False;990;294.4401;Smoothness Texture;4;645;646;644;744;;0,0.751724,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;18;80,-896;Float;True;Property;_MainTex;Main Texture;4;1;[NoScaleOffset];Create;False;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;708;1488,-944;Float;False;1113;294.4401;Normal Texture;5;620;1064;607;604;655;;0,0.751724,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;644;2688,-896;Float;False;587;0;1;FLOAT2;0
Node;AmplifyShaderEditor.ColorNode;409;768,-896;Half;False;Property;_Color;Main Color;3;0;Create;False;0;0;False;1;Header(Main);1,1,1,1;1,1,1,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;1059;-1280,-1504;Float;False;1057;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;791;-1280,-1600;Float;False;616;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;655;1536,-768;Half;False;Property;_BumpScale;Normal Scale;5;0;Create;False;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;604;1536,-896;Float;False;587;0;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1057;1024,-800;Half;False;MainColorAlpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;616;384,-768;Half;False;MainTexAlpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;751;3664,-944;Float;False;802;433.4401;Metallic / Smoothness;8;660;748;657;745;294;656;749;750;;1,0.7450981,0,1;0;0
Node;AmplifyShaderEditor.SamplerNode;645;2944,-896;Float;True;Property;_MetallicGlossMap;Surface Texture;9;1;[NoScaleOffset];Create;False;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1058;-1024,-1600;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1076;-1280,-1984;Float;False;486;0;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;607;1808,-896;Float;True;Property;_BumpMap;Normal Texture;6;1;[NoScaleOffset];Create;False;0;0;False;0;None;None;True;0;False;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;744;3344,-768;Half;False;SurfaceTexAlpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;36;-1280,-2048;Float;False;487;0;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;656;3712,-896;Float;False;646;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;486;1024,-896;Half;False;MainColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;749;3712,-704;Float;False;744;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;487;384,-896;Half;False;MainTex;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;750;3712,-816;Half;False;Property;_Metallic;Surface Metallic;7;0;Create;False;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;646;3344,-896;Half;False;SurfaceTexRed;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;294;3712,-624;Half;False;Property;_Glossiness;Surface Smoothness;8;0;Create;False;0;0;False;0;0.5;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;745;4032,-720;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;1077;-768,-1600;Float;False;ADS Render Type;-1;;887;fcf5ead2adb2e514895d795dcf8514b1;0;1;1;FLOAT;0;False;3;FLOAT;0;FLOAT;13;FLOAT;25
Node;AmplifyShaderEditor.FunctionNode;1064;2112,-896;Float;False;Switch Back Normal;-1;;888;121446c878db06f4c847f9c5afed7cfe;0;1;13;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;657;4032,-896;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;1078;-768,-1408;Float;False;ADS Motion Noise;11;;883;047eb809542f42d40b4b5066e22cee72;0;0;1;FLOAT;85
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1075;-1024,-2048;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;683;-1328,-2608;Float;False;1496;152;;6;862;925;743;549;550;553;OPTIONS;1,0,0.503,1;0;0
Node;AmplifyShaderEditor.FunctionNode;1053;-768,-1472;Float;False;ADS Mask Generic;18;;884;2cfc3815568565c4585aebb38bd7a29b;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;550;-1280,-2560;Half;False;Property;_SrcBlend;_SrcBlend;25;1;[HideInInspector];Create;True;0;0;True;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;654;-1280,-1728;Float;False;660;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;925;-384,-2560;Half;False;Property;_ZWrite;_ZWrite;24;1;[HideInInspector];Create;True;2;Off;0;On;1;0;True;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;553;-1120,-2560;Half;False;Property;_DstBlend;_DstBlend;26;1;[HideInInspector];Create;True;0;0;True;0;10;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1074;-512,-2048;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;620;2368,-896;Half;False;NORMAL;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;624;-1280,-1888;Float;False;620;0;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;752;-1280,-1808;Float;False;748;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;549;-896,-2560;Half;False;Property;_Mode;Blend Mode;0;1;[Enum];Create;False;4;Opaque;0;Cutout;1;Fade;2;Transparent;3;0;True;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;748;4224,-896;Half;False;METALLIC;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;1092;-512,-1472;Float;False;ADS Motion Global;14;;889;a8838de3869103540a427ac470da4da6;0;2;136;FLOAT;0;False;133;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;660;4224,-720;Half;False;SMOOTHNESS;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;862;-128,-2560;Half;False;Property;_Cutoff;Cutout;2;0;Create;False;3;Off;0;Front;1;Back;2;0;True;1;Space(10);0.5;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;743;-640,-2560;Half;False;Property;_CullMode;Cull Mode;1;1;[Enum];Create;True;3;Off;0;Front;1;Back;2;0;True;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-256,-2048;Float;False;True;2;Float;ADSShaderGUI;300;0;Standard;BOXOPHOBIC/Advanced Dynamic Shaders/Standard Generic;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;Off;0;True;925;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0;True;True;0;True;Opaque;;Geometry;All;False;True;True;False;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;1;5;True;550;10;True;553;0;1;False;550;10;False;553;0;True;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;300;Standard;-1;-1;-1;-1;0;False;0;0;True;743;-1;0;True;862;3;Include;VS_indirect.cginc;Pragma;instancing_options procedural:setup;Pragma;multi_compile GPU_FRUSTUM_ON __;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;564;0;563;1
WireConnection;564;1;563;2
WireConnection;562;0;561;0
WireConnection;562;1;564;0
WireConnection;565;0;563;3
WireConnection;565;1;563;4
WireConnection;575;0;562;0
WireConnection;575;1;565;0
WireConnection;587;0;575;0
WireConnection;18;1;588;0
WireConnection;1057;0;409;4
WireConnection;616;0;18;4
WireConnection;645;1;644;0
WireConnection;1058;0;791;0
WireConnection;1058;1;1059;0
WireConnection;607;1;604;0
WireConnection;607;5;655;0
WireConnection;744;0;645;4
WireConnection;486;0;409;0
WireConnection;487;0;18;0
WireConnection;646;0;645;1
WireConnection;745;0;749;0
WireConnection;745;1;294;0
WireConnection;1077;1;1058;0
WireConnection;1064;13;607;0
WireConnection;657;0;656;0
WireConnection;657;1;750;0
WireConnection;1075;0;36;0
WireConnection;1075;1;1076;0
WireConnection;1074;0;1075;0
WireConnection;1074;1;1077;25
WireConnection;620;0;1064;0
WireConnection;748;0;657;0
WireConnection;1092;136;1053;0
WireConnection;1092;133;1078;85
WireConnection;660;0;745;0
WireConnection;0;0;1074;0
WireConnection;0;1;624;0
WireConnection;0;3;752;0
WireConnection;0;4;654;0
WireConnection;0;9;1077;0
WireConnection;0;10;1077;13
WireConnection;0;11;1092;0
ASEEND*/
//CHKSM=8468B654D7533DB001AAFF23EC2D9897F4114BB7
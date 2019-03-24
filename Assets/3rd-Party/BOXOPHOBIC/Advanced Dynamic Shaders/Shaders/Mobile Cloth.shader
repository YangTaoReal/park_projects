// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "BOXOPHOBIC/Advanced Dynamic Shaders/Mobile Cloth"
{
	Properties
	{
		[Enum(Opaque,0,Cutout,1,Fade,2,Transparent,3)]_Mode("Blend Mode", Float) = 0
		[Enum(Off,0,Front,1,Back,2)]_CullMode("Cull Mode", Float) = 0
		[Space(10)]_Cutoff("Cutout", Range( 0 , 1)) = 0.5
		[Header(Alpha)][KeywordEnum(Main,Alpha)] _AlphaFrom("Alpha From", Float) = 0
		[NoScaleOffset]_AlphaTexture("Alpha Texture", 2D) = "white" {}
		_AlphaUVs("Alpha UVs", Vector) = (1,1,0,0)
		[Header(Main)]_Color("Main Color", Color) = (1,1,1,1)
		[NoScaleOffset]_MainTex("Main Texture", 2D) = "white" {}
		_BumpScale("Normal Scale", Float) = 1
		[NoScaleOffset]_BumpMap("Normal Texture", 2D) = "bump" {}
		_SpecularColor("Specular Color", Color) = (1,1,1,1)
		_Shininess("Glossiness", Float) = 0
		[Space(10)]_MainUVs("Main UVs", Vector) = (1,1,0,0)
		[Enum(Multiplied,0,Sticker,1)][Header(Symbol)]_SymbolMode("Symbol Mode", Float) = 0
		_SymbolColor("Symbol Color", Color) = (1,1,1,1)
		[NoScaleOffset]_SymbolTexture("Symbol Texture", 2D) = "gray" {}
		_SymbolRotation("Symbol Rotation", Range( 0 , 360)) = 0
		_SymbolUVs("Symbol UVs", Vector) = (1,1,1,0)
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
		#pragma shader_feature _ALPHAFROM_MAIN _ALPHAFROM_ALPHA
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
			half ASEVFace : VFACE;
			float3 vertexToFrag49_g867;
			float3 worldNormal;
			INTERNAL_DATA
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

		uniform half _CullMode;
		uniform half _ZWrite;
		uniform half _Cutoff;
		uniform half _Mode;
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
		uniform half4 _Color;
		uniform sampler2D _MainTex;
		uniform half4 _MainUVs;
		uniform half4 _SymbolColor;
		uniform sampler2D _SymbolTexture;
		uniform half4 _SymbolUVs;
		uniform half _SymbolRotation;
		uniform half _SymbolMode;
		uniform sampler2D _AlphaTexture;
		uniform half4 _AlphaUVs;
		uniform half _BumpScale;
		uniform sampler2D _BumpMap;
		uniform half _Shininess;
		uniform half4 _SpecularColor;


		inline half2 RotateUV453( half2 UV , half Angle )
		{
			return mul( UV - half2( 0.5,0.5 ) , half2x2( cos(Angle) , sin(Angle), -sin(Angle) , cos(Angle) )) + half2( 0.5,0.5 );;
		}


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			half MotionScale60_g875 = ( ADS_GlobalScale * _MotionScale );
			half MotionSpeed62_g875 = ( ADS_GlobalSpeed * _MotionSpeed );
			float mulTime90_g875 = _Time.y * MotionSpeed62_g875;
			float2 appendResult115_g871 = (float2(ase_worldPos.x , ase_worldPos.z));
			float2 panner73_g871 = ( _Time.y * ( ADS_NoiseSpeed * (-ADS_GlobalDirection).xz ) + ( appendResult115_g871 * ADS_NoiseScale ));
			float ifLocalVar94_g871 = 0;
			UNITY_BRANCH 
			if( ( ADS_NoiseTex_ON * _MotionNoise ) > 0.01 )
				ifLocalVar94_g871 = saturate( pow( abs( tex2Dlod( ADS_NoiseTex, float4( panner73_g871, 0, 0.0) ).r ) , ADS_NoiseContrast ) );
			else if( ( ADS_NoiseTex_ON * _MotionNoise ) < 0.01 )
				ifLocalVar94_g871 = 1.0;
			half MotionlAmplitude58_g875 = ( ADS_GlobalAmplitude * _MotionAmplitude * ifLocalVar94_g871 );
			half3 MotionDirection59_g875 = ( ADS_GlobalDirection + 0.0001 );
			float temp_output_25_0_g874 = _MaskAxis;
			float lerpResult24_g874 = lerp( v.texcoord3.xyz.x , v.texcoord3.xyz.y , saturate( temp_output_25_0_g874 ));
			float lerpResult21_g874 = lerp( lerpResult24_g874 , v.texcoord3.xyz.z , step( 2.0 , temp_output_25_0_g874 ));
			half THREE27_g874 = lerpResult21_g874;
			float lerpResult42_g872 = lerp( v.color.r , THREE27_g874 , _MaskType);
			float temp_output_7_0_g873 = _MaskMin;
			float lerpResult31_g872 = lerp( 0.0 , 1.0 , saturate( ( ( lerpResult42_g872 - temp_output_7_0_g873 ) / ( _MaskMax - temp_output_7_0_g873 ) ) ));
			half MotionMask137_g875 = ( _Show_MaskGeneric * lerpResult31_g872 );
			v.vertex.xyz += mul( unity_WorldToObject, float4( ( ( ( ( sin( ( ( ( ( ase_worldPos + (ase_worldPos).zxy ) * MotionScale60_g875 ) + mulTime90_g875 ) + ( v.color.g * 1.756 ) ) ) * MotionlAmplitude58_g875 ) + ( MotionlAmplitude58_g875 * saturate( MotionScale60_g875 ) ) ) * MotionDirection59_g875 ) * MotionMask137_g875 ) , 0.0 ) ).xyz;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_worldlightDir = 0;
			#else //aseld
			float3 ase_worldlightDir = Unity_SafeNormalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			float3 ase_worldNormal = UnityObjectToWorldNormal( v.normal );
			half3 NORMAL62_g867 = ase_worldNormal;
			float3 normalizeResult3_g867 = normalize( NORMAL62_g867 );
			float dotResult5_g867 = dot( ase_worldlightDir , normalizeResult3_g867 );
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aselc
			float4 ase_lightColor = 0;
			#else //aselc
			float4 ase_lightColor = _LightColor0;
			#endif //aselc
			float3 temp_output_8_0_g867 = ( max( dotResult5_g867 , 0.0 ) * ( ase_lightColor.rgb * ase_lightColor.a ) );
			o.vertexToFrag49_g867 = temp_output_8_0_g867;
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
			float2 appendResult598 = (float2(_AlphaUVs.x , _AlphaUVs.y));
			float2 appendResult601 = (float2(_AlphaUVs.z , _AlphaUVs.w));
			half2 AlphaUV603 = ( ( i.uv_texcoord * appendResult598 ) + appendResult601 );
			half AlphaTextureRed595 = tex2D( _AlphaTexture, AlphaUV603 ).r;
			#if defined(_ALPHAFROM_MAIN)
				float staticSwitch615 = MainTexAlpha616;
			#elif defined(_ALPHAFROM_ALPHA)
				float staticSwitch615 = AlphaTextureRed595;
			#else
				float staticSwitch615 = MainTexAlpha616;
			#endif
			half ALPHA407 = staticSwitch615;
			half MainColorAlpha1057 = _Color.a;
			float temp_output_1_0_g866 = ( ALPHA407 * MainColorAlpha1057 );
			#if defined(_RENDERTYPE_OPAQUE)
				float staticSwitch15_g866 = 1.0;
			#elif defined(_RENDERTYPE_CUT)
				float staticSwitch15_g866 = 1.0;
			#elif defined(_RENDERTYPE_FADE)
				float staticSwitch15_g866 = temp_output_1_0_g866;
			#elif defined(_RENDERTYPE_TRANSPARENT)
				float staticSwitch15_g866 = temp_output_1_0_g866;
			#else
				float staticSwitch15_g866 = 1.0;
			#endif
			#if defined(_RENDERTYPE_OPAQUE)
				float staticSwitch23_g866 = 1.0;
			#elif defined(_RENDERTYPE_CUT)
				float staticSwitch23_g866 = temp_output_1_0_g866;
			#elif defined(_RENDERTYPE_FADE)
				float staticSwitch23_g866 = 1.0;
			#elif defined(_RENDERTYPE_TRANSPARENT)
				float staticSwitch23_g866 = 1.0;
			#else
				float staticSwitch23_g866 = 1.0;
			#endif
			half4 MainColor486 = _Color;
			half4 MainTex487 = tex2DNode18;
			float4 temp_output_518_0 = ( MainColor486 * MainTex487 );
			half4 SymbolColor492 = _SymbolColor;
			float2 temp_cast_2 = (0.5).xx;
			float2 appendResult870 = (float2(_SymbolUVs.x , _SymbolUVs.y));
			float2 appendResult579 = (float2(_SymbolUVs.z , _SymbolUVs.w));
			half2 UV453 = ( ( ( ( i.uv_texcoord - temp_cast_2 ) * appendResult870 ) + 0.5 ) + appendResult579 );
			half Angle453 = radians( _SymbolRotation );
			half2 localRotateUV453 = RotateUV453( UV453 , Angle453 );
			half2 SymbolUVs488 = localRotateUV453;
			float4 tex2DNode401 = tex2D( _SymbolTexture, SymbolUVs488 );
			half4 SymbolTex490 = tex2DNode401;
			half SymbolTexAlpha963 = tex2DNode401.a;
			float4 lerpResult967 = lerp( temp_output_518_0 , ( SymbolColor492 * SymbolTex490 * saturate( ( MainTex487 + _SymbolMode ) ) ) , SymbolTexAlpha963);
			float4 switchResult478 = (((i.ASEVFace>0)?(lerpResult967):(temp_output_518_0)));
			float4 ALBEDO416 = switchResult478;
			#if defined(_RENDERTYPE_OPAQUE)
				float staticSwitch24_g866 = 1.0;
			#elif defined(_RENDERTYPE_CUT)
				float staticSwitch24_g866 = 1.0;
			#elif defined(_RENDERTYPE_FADE)
				float staticSwitch24_g866 = 1.0;
			#elif defined(_RENDERTYPE_TRANSPARENT)
				float staticSwitch24_g866 = temp_output_1_0_g866;
			#else
				float staticSwitch24_g866 = 1.0;
			#endif
			half RenderTransparent1091 = staticSwitch24_g866;
			half ATTENUATION77_g867 = ase_lightAtten;
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			half3 NORMAL62_g867 = ase_worldNormal;
			UnityGI gi11_g867 = gi;
			float3 diffNorm11_g867 = NORMAL62_g867;
			gi11_g867 = UnityGI_Base( data, 1, diffNorm11_g867 );
			float3 indirectDiffuse11_g867 = gi11_g867.indirect.diffuse + diffNorm11_g867 * 0.0001;
			half3 INDIRECT72_g867 = indirectDiffuse11_g867;
			float switchResult12_g865 = (((i.ASEVFace>0)?(1.0):(-1.0)));
			half3 NORMAL620 = ( switchResult12_g865 * UnpackScaleNormal( tex2D( _BumpMap, MainUVs587 ), _BumpScale ) );
			half3 NORMAL99_g868 = WorldNormalVector( i , NORMAL620 );
			float3 normalizeResult73_g868 = normalize( NORMAL99_g868 );
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = Unity_SafeNormalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_worldlightDir = 0;
			#else //aseld
			float3 ase_worldlightDir = Unity_SafeNormalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			float3 normalizeResult40_g868 = normalize( ( ase_worldViewDir + ase_worldlightDir ) );
			float dotResult45_g868 = dot( normalizeResult73_g868 , normalizeResult40_g868 );
			half4 CUSTOM_LIGHTING1082 = ( ( ALBEDO416 * RenderTransparent1091 * float4( ( ( i.vertexToFrag49_g867 * ATTENUATION77_g867 ) + INDIRECT72_g867 ) , 0.0 ) ) + ( pow( max( dotResult45_g868 , 0.0 ) , ( 128.0 * max( _Shininess , 0.01 ) ) ) * _SpecularColor ) );
			c.rgb = CUSTOM_LIGHTING1082.rgb;
			c.a = staticSwitch15_g866;
			clip( staticSwitch23_g866 - _Cutoff );
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
			half4 MainColor486 = _Color;
			float2 appendResult564 = (float2(_MainUVs.x , _MainUVs.y));
			float2 appendResult565 = (float2(_MainUVs.z , _MainUVs.w));
			half2 MainUVs587 = ( ( i.uv_texcoord * appendResult564 ) + appendResult565 );
			float4 tex2DNode18 = tex2D( _MainTex, MainUVs587 );
			half4 MainTex487 = tex2DNode18;
			float4 temp_output_518_0 = ( MainColor486 * MainTex487 );
			half4 SymbolColor492 = _SymbolColor;
			float2 temp_cast_0 = (0.5).xx;
			float2 appendResult870 = (float2(_SymbolUVs.x , _SymbolUVs.y));
			float2 appendResult579 = (float2(_SymbolUVs.z , _SymbolUVs.w));
			half2 UV453 = ( ( ( ( i.uv_texcoord - temp_cast_0 ) * appendResult870 ) + 0.5 ) + appendResult579 );
			half Angle453 = radians( _SymbolRotation );
			half2 localRotateUV453 = RotateUV453( UV453 , Angle453 );
			half2 SymbolUVs488 = localRotateUV453;
			float4 tex2DNode401 = tex2D( _SymbolTexture, SymbolUVs488 );
			half4 SymbolTex490 = tex2DNode401;
			half SymbolTexAlpha963 = tex2DNode401.a;
			float4 lerpResult967 = lerp( temp_output_518_0 , ( SymbolColor492 * SymbolTex490 * saturate( ( MainTex487 + _SymbolMode ) ) ) , SymbolTexAlpha963);
			float4 switchResult478 = (((i.ASEVFace>0)?(lerpResult967):(temp_output_518_0)));
			float4 ALBEDO416 = switchResult478;
			o.Albedo = ALBEDO416.rgb;
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
				o.customPack2.xyz = customInputData.vertexToFrag49_g867;
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
				surfIN.vertexToFrag49_g867 = IN.customPack2.xyz;
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
1927;29;1906;1014;3083.932;3994.902;2.91936;True;False
Node;AmplifyShaderEditor.CommentaryNode;962;-1330,846;Float;False;1381;487;Symbol UVs;14;578;586;577;870;580;581;582;579;583;873;457;872;453;488;SYMBOL LAYER;0.4980392,1,0,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;712;-1329.93,-51.44009;Float;False;1124.93;486.9999;Main UVs;7;563;564;561;565;587;575;562;MAIN LAYER;0.4980392,1,0,1;0;0
Node;AmplifyShaderEditor.Vector4Node;586;-1280,1120;Half;False;Property;_SymbolUVs;Symbol UVs;18;0;Create;True;0;0;False;0;1,1,1,0;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexCoordVertexDataNode;578;-1280,896;Float;False;0;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;577;-1280,1024;Half;False;Constant;_Float12;Float 12;34;0;Create;True;0;0;False;0;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector4Node;563;-1279.93,222.5598;Half;False;Property;_MainUVs;Main UVs;13;0;Create;True;0;0;False;1;Space(10);1,1,0,0;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;710;-1330,-946;Float;False;1125;487;Alpha UVs;7;597;599;598;600;601;602;603;CUSTOM ALPHA;0.4980392,1,0,1;0;0
Node;AmplifyShaderEditor.DynamicAppendNode;870;-1024,1120;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;580;-1024,896;Float;False;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;561;-1279.93,-1.440094;Float;False;0;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;564;-1023.93,222.5598;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector4Node;597;-1280,-672;Half;False;Property;_AlphaUVs;Alpha UVs;5;0;Create;True;0;0;False;0;1,1,0,0;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;582;-832,1024;Half;False;Constant;_Float13;Float 13;34;0;Create;True;0;0;False;0;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;565;-1023.93,302.5598;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;562;-832,0;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;598;-1024,-672;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;599;-1280,-896;Float;False;0;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;581;-832,896;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;760;-177.9295,-51.44009;Float;False;1635.028;429.4401;Main Texture and Color;7;588;18;487;616;409;486;1057;;0,0.751724,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;873;-832,1216;Half;False;Property;_SymbolRotation;Symbol Rotation;17;0;Create;True;0;0;False;0;0;0;0;360;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;575;-624,0;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;579;-1024,1200;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;601;-1024,-592;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;600;-896,-896;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;583;-640,896;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;587;-448,0;Half;False;MainUVs;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RadiansOpNode;457;-512,1216;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;711;-178,-946;Float;False;876;280;Alpha Texture;3;593;594;595;;0,0.751724,1,1;0;0
Node;AmplifyShaderEditor.SimpleAddOpNode;602;-704,-896;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;872;-448,896;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;588;-127.9295,-1.440094;Float;False;587;0;1;FLOAT2;0
Node;AmplifyShaderEditor.CustomExpressionNode;453;-336,1200;Half;False;mul( UV - half2( 0.5,0.5 ) , half2x2( cos(Angle) , sin(Angle), -sin(Angle) , cos(Angle) )) + half2( 0.5,0.5 )@;2;False;2;True;UV;FLOAT2;0,0;In;;True;Angle;FLOAT;0;In;;Rotate UV;True;False;0;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;675;80,848;Float;False;1432;265;Symbol Texture and Color;7;489;490;492;411;401;520;963;;0,0.751724,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;18;80.0705,-1.440094;Float;True;Property;_MainTex;Main Texture;7;1;[NoScaleOffset];Create;False;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;593;-128,-896;Float;False;603;0;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;603;-448,-896;Half;False;AlphaUV;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;728;1614,846;Float;False;1454.944;533.5228;Symbol Layer combined with Main Layer;14;416;478;518;516;517;515;970;975;964;976;965;966;967;968;;0,0.751724,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;515;1664,1024;Float;False;487;0;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;487;384,0;Half;False;MainTex;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;488;-192,896;Half;False;SymbolUVs;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;520;128,896;Float;False;488;0;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;723;718,-946;Float;False;1011.356;275.8141;Alpha Mode;4;407;615;618;619;;0,1,0.4980392,1;0;0
Node;AmplifyShaderEditor.SamplerNode;594;80,-896;Float;True;Property;_AlphaTexture;Alpha Texture;4;1;[NoScaleOffset];Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;970;1664,1104;Half;False;Property;_SymbolMode;Symbol Mode;14;1;[Enum];Create;True;2;Multiplied;0;Sticker;1;0;False;1;Header(Symbol);0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;619;768,-768;Float;False;595;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;975;1856,1056;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;595;432,-896;Half;False;AlphaTextureRed;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;616;384,128;Half;False;MainTexAlpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;401;320,896;Float;True;Property;_SymbolTexture;Symbol Texture;16;1;[NoScaleOffset];Create;True;0;0;False;0;None;None;True;0;False;gray;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;618;768,-896;Float;False;616;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;411;1024,896;Half;False;Property;_SymbolColor;Symbol Color;15;0;Create;True;0;0;False;0;1,1,1,1;1,1,1,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;409;768.0704,-1.440094;Half;False;Property;_Color;Main Color;6;0;Create;False;0;0;False;1;Header(Main);1,1,1,1;1,1,1,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;976;1984,1056;Float;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;708;1488,-48;Float;False;1113;294.4401;Normal Texture;5;620;1064;607;604;655;;0,0.751724,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;492;1280,896;Half;False;SymbolColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;517;1664,1216;Float;False;486;0;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;486;1024,0;Half;False;MainColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;516;1664,1280;Float;False;487;0;1;COLOR;0
Node;AmplifyShaderEditor.StaticSwitch;615;1088,-896;Float;False;Property;_AlphaFrom;Alpha From;3;0;Create;True;0;0;False;1;Header(Alpha);0;0;0;True;;KeywordEnum;2;Main;Alpha;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;965;1664,896;Float;False;492;0;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;964;1664,960;Float;False;490;0;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;490;640,896;Half;False;SymbolTex;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;966;2176,896;Float;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;604;1536,0;Float;False;587;0;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;963;640,1024;Half;False;SymbolTexAlpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;791;-1280,-2816;Float;False;407;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1059;-1280,-2720;Float;False;1057;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;407;1472,-896;Half;False;ALPHA;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1057;1024,96;Half;False;MainColorAlpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;518;1920,1216;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;655;1536,128;Half;False;Property;_BumpScale;Normal Scale;8;0;Create;False;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;968;2176,1216;Float;False;963;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1058;-1024,-2816;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;967;2400,896;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;607;1808,0;Float;True;Property;_BumpMap;Normal Texture;9;1;[NoScaleOffset];Create;False;0;0;False;0;None;None;True;0;False;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;1094;-768,-2816;Float;False;ADS Render Type;-1;;866;fcf5ead2adb2e514895d795dcf8514b1;0;1;1;FLOAT;0;False;3;FLOAT;0;FLOAT;13;FLOAT;25
Node;AmplifyShaderEditor.CommentaryNode;1075;-1328,-2096;Float;False;1251.136;421.3876;Custom Lighting;8;1082;1080;1081;1087;1090;1076;1092;1077;LIGHTING;1,0.7686275,0,1;0;0
Node;AmplifyShaderEditor.FunctionNode;1064;2112,0;Float;False;Switch Back Normal;-1;;865;121446c878db06f4c847f9c5afed7cfe;0;1;13;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SwitchByFaceNode;478;2592,960;Float;False;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;620;2368,0;Half;False;NORMAL;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1091;-256,-2752;Half;False;RenderTransparent;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1077;-1280,-2048;Float;False;416;0;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;1092;-1280,-1984;Float;False;1091;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;416;2816,896;Float;False;ALBEDO;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;1076;-1280,-1792;Float;False;620;0;1;FLOAT3;0
Node;AmplifyShaderEditor.FunctionNode;1090;-1280,-1920;Float;False;Lighting Lambert;-1;;867;3e225d4a5a55ac8439718a5f6f59857f;5,56,0,57,1,75,2,70,2,66,1;4;14;FLOAT3;0,0,0;False;59;FLOAT3;0,0,0;False;82;FLOAT;0;False;85;FLOAT;0;False;1;FLOAT3;12
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1087;-768,-2048;Float;False;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT3;0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;1080;-1024,-1792;Float;False;Lighting Specular;10;;868;503457513f257784c866265a383a60d7;1,103,2;2;100;FLOAT3;0,0,0;False;102;FLOAT3;0,0,0;False;1;COLOR;12
Node;AmplifyShaderEditor.CommentaryNode;683;-1328,-3504;Float;False;1496;152;;6;862;925;743;549;550;553;OPTIONS;1,0,0.503,1;0;0
Node;AmplifyShaderEditor.SimpleAddOpNode;1081;-576,-2048;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;1054;-768,-2624;Float;False;ADS Motion Noise;19;;871;047eb809542f42d40b4b5066e22cee72;0;0;1;FLOAT;85
Node;AmplifyShaderEditor.FunctionNode;1053;-768,-2688;Float;False;ADS Mask Generic;26;;872;2cfc3815568565c4585aebb38bd7a29b;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;550;-1280,-3456;Half;False;Property;_SrcBlend;_SrcBlend;33;1;[HideInInspector];Create;True;0;0;True;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;549;-896,-3456;Half;False;Property;_Mode;Blend Mode;0;1;[Enum];Create;False;4;Opaque;0;Cutout;1;Fade;2;Transparent;3;0;True;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1084;-1280,-3200;Float;False;416;0;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;553;-1120,-3456;Half;False;Property;_DstBlend;_DstBlend;34;1;[HideInInspector];Create;True;0;0;True;0;10;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;1026;-512,-2688;Float;False;ADS Motion Global;22;;875;a8838de3869103540a427ac470da4da6;0;2;136;FLOAT;0;False;133;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;1085;-1280,-3072;Float;False;1082;0;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;489;640,960;Half;False;SymbolTexRed;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1082;-352,-2048;Half;False;CUSTOM_LIGHTING;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;743;-640,-3456;Half;False;Property;_CullMode;Cull Mode;1;1;[Enum];Create;True;3;Off;0;Front;1;Back;2;0;True;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;862;-128,-3456;Half;False;Property;_Cutoff;Cutout;2;0;Create;False;3;Off;0;Front;1;Back;2;0;True;1;Space(10);0.5;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;925;-384,-3456;Half;False;Property;_ZWrite;_ZWrite;32;1;[HideInInspector];Create;True;2;Off;0;On;1;0;True;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-256,-3200;Float;False;True;2;Float;ADSShaderGUI;200;0;CustomLighting;BOXOPHOBIC/Advanced Dynamic Shaders/Mobile Cloth;False;False;False;False;True;True;False;False;False;False;False;True;False;False;False;False;True;False;False;False;Off;0;True;925;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0;True;True;0;True;Opaque;;Geometry;All;False;True;True;False;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;1;5;True;550;10;True;553;0;1;False;550;10;False;553;0;True;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;200;Mobile/Bumped Specular;-1;-1;-1;-1;0;False;0;0;True;743;-1;0;True;862;3;Include;VS_indirect.cginc;Pragma;instancing_options procedural:setup;Pragma;multi_compile GPU_FRUSTUM_ON __;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;870;0;586;1
WireConnection;870;1;586;2
WireConnection;580;0;578;0
WireConnection;580;1;577;0
WireConnection;564;0;563;1
WireConnection;564;1;563;2
WireConnection;565;0;563;3
WireConnection;565;1;563;4
WireConnection;562;0;561;0
WireConnection;562;1;564;0
WireConnection;598;0;597;1
WireConnection;598;1;597;2
WireConnection;581;0;580;0
WireConnection;581;1;870;0
WireConnection;575;0;562;0
WireConnection;575;1;565;0
WireConnection;579;0;586;3
WireConnection;579;1;586;4
WireConnection;601;0;597;3
WireConnection;601;1;597;4
WireConnection;600;0;599;0
WireConnection;600;1;598;0
WireConnection;583;0;581;0
WireConnection;583;1;582;0
WireConnection;587;0;575;0
WireConnection;457;0;873;0
WireConnection;602;0;600;0
WireConnection;602;1;601;0
WireConnection;872;0;583;0
WireConnection;872;1;579;0
WireConnection;453;0;872;0
WireConnection;453;1;457;0
WireConnection;18;1;588;0
WireConnection;603;0;602;0
WireConnection;487;0;18;0
WireConnection;488;0;453;0
WireConnection;594;1;593;0
WireConnection;975;0;515;0
WireConnection;975;1;970;0
WireConnection;595;0;594;1
WireConnection;616;0;18;4
WireConnection;401;1;520;0
WireConnection;976;0;975;0
WireConnection;492;0;411;0
WireConnection;486;0;409;0
WireConnection;615;1;618;0
WireConnection;615;0;619;0
WireConnection;490;0;401;0
WireConnection;966;0;965;0
WireConnection;966;1;964;0
WireConnection;966;2;976;0
WireConnection;963;0;401;4
WireConnection;407;0;615;0
WireConnection;1057;0;409;4
WireConnection;518;0;517;0
WireConnection;518;1;516;0
WireConnection;1058;0;791;0
WireConnection;1058;1;1059;0
WireConnection;967;0;518;0
WireConnection;967;1;966;0
WireConnection;967;2;968;0
WireConnection;607;1;604;0
WireConnection;607;5;655;0
WireConnection;1094;1;1058;0
WireConnection;1064;13;607;0
WireConnection;478;0;967;0
WireConnection;478;1;518;0
WireConnection;620;0;1064;0
WireConnection;1091;0;1094;25
WireConnection;416;0;478;0
WireConnection;1087;0;1077;0
WireConnection;1087;1;1092;0
WireConnection;1087;2;1090;12
WireConnection;1080;102;1076;0
WireConnection;1081;0;1087;0
WireConnection;1081;1;1080;12
WireConnection;1026;136;1053;0
WireConnection;1026;133;1054;85
WireConnection;489;0;401;1
WireConnection;1082;0;1081;0
WireConnection;0;0;1084;0
WireConnection;0;9;1094;0
WireConnection;0;10;1094;13
WireConnection;0;13;1085;0
WireConnection;0;11;1026;0
ASEEND*/
//CHKSM=C9CDC0CE0735EA047FAC1C1BC4B2213334E7D719
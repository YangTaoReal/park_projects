// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Utils/ADS Debug"
{
	Properties
	{
		[Toggle]_GrassTint("Grass Tint", Float) = 1
		[Header(Motion)]_MotionAmplitude("Motion Amplitude", Float) = 1
		_MotionSpeed("Motion Speed", Float) = 1
		_MotionScale("Motion Scale", Float) = 1
		[Enum(Opaque,0,Cutout,1,Fade,2)]_Mode("Blend Mode", Float) = 0
		[Toggle]_GrassSize("Grass Size", Float) = 1
		[Toggle][Header(Globals)]_MotionNoise("Motion Noise", Float) = 1
		_Show_MaskGeneric("Show_MaskGeneric", Float) = 1
		_Debug_Arrow("Debug_Arrow", Float) = 1
		_GrassSize("Grass Size", Float) = 1
		_GrassTint("Grass Tint", Float) = 1
		_MotionNoise("MotionNoise", Float) = 1
		[Enum(Off,0,Front,1,Back,2)]_CullMode("Cull Mode", Float) = 0
		[Space(10)]_Cutoff("Cutout/Fade", Range( 0 , 1)) = 1
		[NoScaleOffset]_MainTex("Main Texture", 2D) = "white" {}
		[Space(10)]_MainUVs("Main UVs", Vector) = (1,1,0,0)
		[Enum(Vertex Red,0,Vertex Position,1)][Space(10)]_MaskType("Mask Type", Float) = 0
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
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "AlphaTest+0" "IgnoreProjector" = "True" "ForceNoShadowCasting" = "True" "IsEmissive" = "true"  }
		LOD 300
		Cull [_CullMode]
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#include "VS_indirect.cginc"
		#pragma instancing_options procedural:setup
		#pragma multi_compile GPU_FRUSTUM_ON __
		#pragma exclude_renderers d3d9 gles 
		#pragma surface surf Lambert keepalpha noshadow noambient novertexlights nolightmap  nodynlightmap nodirlightmap nofog nometa noforwardadd vertex:vertexDataFunc 
		struct Input
		{
			float3 worldPos;
			float4 vertexColor : COLOR;
			float3 worldNormal;
			float2 uv_texcoord;
		};

		uniform half _Cutoff;
		uniform half _ZWrite;
		uniform half _DstBlend;
		uniform half _Mode;
		uniform half _CullMode;
		uniform half _SrcBlend;
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
		uniform half ADS_DebugMode;
		uniform half ADS_GrassTintTex_ON;
		uniform half _GrassTint;
		uniform sampler2D ADS_GrassTintTex;
		uniform half4 ADS_GrassTintScaleOffset;
		uniform half4 ADS_GrassTintColorOne;
		uniform half4 ADS_GrassTintColorTwo;
		uniform half ADS_GrassTintModeColors;
		uniform half ADS_GrassTintIntensity;
		uniform half _Debug_Arrow;
		uniform sampler2D _MainTex;
		uniform half4 _MainUVs;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			half MotionScale60_g1254 = ( ADS_GlobalScale * _MotionScale );
			half MotionSpeed62_g1254 = ( ADS_GlobalSpeed * _MotionSpeed );
			float mulTime90_g1254 = _Time.y * MotionSpeed62_g1254;
			float2 appendResult115_g1115 = (float2(ase_worldPos.x , ase_worldPos.z));
			float2 panner73_g1115 = ( _Time.y * ( ADS_NoiseSpeed * (-ADS_GlobalDirection).xz ) + ( appendResult115_g1115 * ADS_NoiseScale ));
			float ifLocalVar94_g1115 = 0;
			UNITY_BRANCH 
			if( ( ADS_NoiseTex_ON * _MotionNoise ) > 0.01 )
				ifLocalVar94_g1115 = saturate( pow( abs( tex2Dlod( ADS_NoiseTex, float4( panner73_g1115, 0, 0.0) ).r ) , ADS_NoiseContrast ) );
			else if( ( ADS_NoiseTex_ON * _MotionNoise ) < 0.01 )
				ifLocalVar94_g1115 = 1.0;
			half MotionlAmplitude58_g1254 = ( ADS_GlobalAmplitude * _MotionAmplitude * ifLocalVar94_g1115 );
			half3 MotionDirection59_g1254 = ( ADS_GlobalDirection + 0.0001 );
			float temp_output_25_0_g1114 = _MaskAxis;
			float lerpResult24_g1114 = lerp( v.texcoord3.xyz.x , v.texcoord3.xyz.y , saturate( temp_output_25_0_g1114 ));
			float lerpResult21_g1114 = lerp( lerpResult24_g1114 , v.texcoord3.xyz.z , step( 2.0 , temp_output_25_0_g1114 ));
			half THREE27_g1114 = lerpResult21_g1114;
			float lerpResult42_g1112 = lerp( v.color.r , THREE27_g1114 , _MaskType);
			float temp_output_7_0_g1113 = _MaskMin;
			float lerpResult31_g1112 = lerp( 0.0 , 1.0 , saturate( ( ( lerpResult42_g1112 - temp_output_7_0_g1113 ) / ( _MaskMax - temp_output_7_0_g1113 ) ) ));
			half MotionMask137_g1254 = ( _Show_MaskGeneric * lerpResult31_g1112 );
			float lerpResult116_g1253 = lerp( ADS_GrassSizeMin , ADS_GrassSizeMax , tex2Dlod( ADS_GrassSizeTex, float4( ( ( (ase_worldPos).xz * (ADS_GrassSizeScaleOffset).xy ) + (ADS_GrassSizeScaleOffset).zw ), 0, 0.0) ).r);
			float3 temp_cast_3 = (0.0).xxx;
			float3 ifLocalVar96_g1253 = 0;
			UNITY_BRANCH 
			if( ( ADS_GrassSizeTex_ON * _GrassSize ) > 0.5 )
				ifLocalVar96_g1253 = ( lerpResult116_g1253 * v.texcoord3.xyz );
			else if( ( ADS_GrassSizeTex_ON * _GrassSize ) < 0.5 )
				ifLocalVar96_g1253 = temp_cast_3;
			v.vertex.xyz += ( mul( unity_WorldToObject, float4( ( ( ( ( sin( ( ( ( ( ase_worldPos + (ase_worldPos).zxy ) * MotionScale60_g1254 ) + mulTime90_g1254 ) + ( v.color.g * 1.756 ) ) ) * MotionlAmplitude58_g1254 ) + ( MotionlAmplitude58_g1254 * saturate( MotionScale60_g1254 ) ) ) * MotionDirection59_g1254 ) * MotionMask137_g1254 ) , 0.0 ) ).xyz + ifLocalVar96_g1253 );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			half DebugMode89_g1248 = ADS_DebugMode;
			float4 ifLocalVar145_g1248 = 0;
			UNITY_BRANCH 
			if( DebugMode89_g1248 == 1.0 )
				ifLocalVar145_g1248 = ( half4(1,0,0,0) * i.vertexColor.r );
			float4 ifLocalVar148_g1248 = 0;
			UNITY_BRANCH 
			if( DebugMode89_g1248 == 2.0 )
				ifLocalVar148_g1248 = ( half4(0,1,0,0) * i.vertexColor.g );
			float4 ifLocalVar153_g1248 = 0;
			UNITY_BRANCH 
			if( DebugMode89_g1248 == 3.0 )
				ifLocalVar153_g1248 = ( half4(0,0,1,0) * i.vertexColor.b );
			float ifLocalVar156_g1248 = 0;
			UNITY_BRANCH 
			if( DebugMode89_g1248 == 4.0 )
				ifLocalVar156_g1248 = i.vertexColor.a;
			half OtherColor181_g1248 = 0.01;
			float4 temp_cast_0 = (OtherColor181_g1248).xxxx;
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float temp_output_25_0_g1252 = _MaskAxis;
			float lerpResult24_g1252 = lerp( ase_vertex3Pos.x , ase_vertex3Pos.y , saturate( temp_output_25_0_g1252 ));
			float lerpResult21_g1252 = lerp( lerpResult24_g1252 , ase_vertex3Pos.z , step( 2.0 , temp_output_25_0_g1252 ));
			half THREE27_g1252 = lerpResult21_g1252;
			float lerpResult227_g1248 = lerp( i.vertexColor.r , THREE27_g1252 , _MaskType);
			float temp_output_7_0_g1249 = _MaskMin;
			float lerpResult234_g1248 = lerp( 0.0 , 1.0 , saturate( ( ( lerpResult227_g1248 - temp_output_7_0_g1249 ) / ( _MaskMax - temp_output_7_0_g1249 ) ) ));
			half CustomMaskGeneric237_g1248 = lerpResult234_g1248;
			float4 lerpResult199_g1248 = lerp( temp_cast_0 , ( CustomMaskGeneric237_g1248 * half4(1,0,0.103,0) ) , _Show_MaskGeneric);
			float4 ifLocalVar125_g1248 = 0;
			UNITY_BRANCH 
			if( DebugMode89_g1248 == 11.0 )
				ifLocalVar125_g1248 = lerpResult199_g1248;
			float4 temp_cast_1 = (OtherColor181_g1248).xxxx;
			float3 ase_worldPos = i.worldPos;
			float2 appendResult115_g1251 = (float2(ase_worldPos.x , ase_worldPos.z));
			float2 panner73_g1251 = ( _Time.y * ( ADS_NoiseSpeed * (-ADS_GlobalDirection).xz ) + ( appendResult115_g1251 * ADS_NoiseScale ));
			float ifLocalVar94_g1251 = 0;
			UNITY_BRANCH 
			if( ( ADS_NoiseTex_ON * _MotionNoise ) > 0.01 )
				ifLocalVar94_g1251 = saturate( pow( abs( tex2D( ADS_NoiseTex, panner73_g1251 ).r ) , ADS_NoiseContrast ) );
			else if( ( ADS_NoiseTex_ON * _MotionNoise ) < 0.01 )
				ifLocalVar94_g1251 = 1.0;
			float4 lerpResult177_g1248 = lerp( temp_cast_1 , ( ifLocalVar94_g1251 * half4(0.5019608,0.7686275,0,0) ) , _MotionNoise);
			float4 ifLocalVar128_g1248 = 0;
			UNITY_BRANCH 
			if( DebugMode89_g1248 == 12.0 )
				ifLocalVar128_g1248 = lerpResult177_g1248;
			float4 temp_cast_2 = (OtherColor181_g1248).xxxx;
			float4 temp_cast_3 = (1.0).xxxx;
			float4 tex2DNode75_g1250 = tex2D( ADS_GrassTintTex, ( ( (ase_worldPos).xz * (ADS_GrassTintScaleOffset).xy ) + (ADS_GrassTintScaleOffset).zw ) );
			float4 lerpResult115_g1250 = lerp( ADS_GrassTintColorOne , ADS_GrassTintColorTwo , tex2DNode75_g1250.r);
			float4 lerpResult121_g1250 = lerp( tex2DNode75_g1250 , lerpResult115_g1250 , ADS_GrassTintModeColors);
			float4 lerpResult126_g1250 = lerp( temp_cast_3 , ( lerpResult121_g1250 * ADS_GrassTintIntensity ) , saturate( ADS_GrassTintIntensity ));
			float4 temp_cast_4 = (1.0).xxxx;
			float4 ifLocalVar96_g1250 = 0;
			UNITY_BRANCH 
			if( ( ADS_GrassTintTex_ON * _GrassTint ) > 0.5 )
				ifLocalVar96_g1250 = saturate( lerpResult126_g1250 );
			else if( ( ADS_GrassTintTex_ON * _GrassTint ) < 0.5 )
				ifLocalVar96_g1250 = temp_cast_4;
			float4 lerpResult201_g1248 = lerp( temp_cast_2 , ifLocalVar96_g1250 , _GrassTint);
			float4 ifLocalVar130_g1248 = 0;
			UNITY_BRANCH 
			if( DebugMode89_g1248 == 21.0 )
				ifLocalVar130_g1248 = lerpResult201_g1248;
			float4 temp_cast_5 = (OtherColor181_g1248).xxxx;
			float lerpResult247_g1248 = lerp( ADS_GrassSizeMin , ADS_GrassSizeMax , tex2D( ADS_GrassSizeTex, ( ( (ase_worldPos).xz * (ADS_GrassSizeScaleOffset).xy ) + (ADS_GrassSizeScaleOffset).zw ) ).r);
			float ifLocalVar257_g1248 = 0;
			UNITY_BRANCH 
			if( ADS_GrassSizeTex_ON > 0.5 )
				ifLocalVar257_g1248 = lerpResult247_g1248;
			else if( ADS_GrassSizeTex_ON < 0.5 )
				ifLocalVar257_g1248 = 0.0;
			half CustomGrassSize263_g1248 = ifLocalVar257_g1248;
			float4 lerpResult213_g1248 = lerp( half4(0,0.1254902,0.5019608,0) , half4(0,0.5019608,1,0) , CustomGrassSize263_g1248);
			float4 lerpResult204_g1248 = lerp( temp_cast_5 , lerpResult213_g1248 , _GrassSize);
			float4 ifLocalVar132_g1248 = 0;
			UNITY_BRANCH 
			if( DebugMode89_g1248 == 22.0 )
				ifLocalVar132_g1248 = lerpResult204_g1248;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = i.worldNormal;
			float fresnelNdotV183_g1248 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode183_g1248 = ( 0.0 + 1.5 * pow( 1.0 - fresnelNdotV183_g1248, 2.0 ) );
			float4 lerpResult187_g1248 = lerp( half4(1,0.5019608,0,0) , half4(1,0.809,0,0) , saturate( fresnelNode183_g1248 ));
			half4 ArrowColor191_g1248 = lerpResult187_g1248;
			half ArrowDebug194_g1248 = _Debug_Arrow;
			float4 lerpResult193_g1248 = lerp( ( ( ifLocalVar145_g1248 + ifLocalVar148_g1248 + ifLocalVar153_g1248 + ifLocalVar156_g1248 ) + ( ifLocalVar125_g1248 + ifLocalVar128_g1248 + ifLocalVar130_g1248 + ifLocalVar132_g1248 ) ) , ArrowColor191_g1248 , ArrowDebug194_g1248);
			float4 temp_output_1150_0 = ( lerpResult193_g1248 * 0.5 );
			o.Albedo = temp_output_1150_0.rgb;
			o.Emission = temp_output_1150_0.rgb;
			o.Alpha = 1;
			float2 appendResult564 = (float2(_MainUVs.x , _MainUVs.y));
			float2 appendResult565 = (float2(_MainUVs.z , _MainUVs.w));
			half2 MainUVs587 = ( ( i.uv_texcoord * appendResult564 ) + appendResult565 );
			float4 tex2DNode18 = tex2D( _MainTex, MainUVs587 );
			half MainTexAlpha616 = tex2DNode18.a;
			clip( MainTexAlpha616 - _Cutoff );
		}

		ENDCG
	}
	CustomEditor "ADSShaderGUI"
}
/*ASEBEGIN
Version=15500
1927;29;1906;1014;3137.277;3001.691;3.245479;True;False
Node;AmplifyShaderEditor.CommentaryNode;712;-1328,-816;Float;False;1124.93;486.9999;Main UVs;7;563;564;561;565;587;575;562;MAIN LAYER;0.4980392,1,0,1;0;0
Node;AmplifyShaderEditor.Vector4Node;563;-1280,-544;Half;False;Property;_MainUVs;Main UVs;26;0;Create;True;0;0;False;1;Space(10);1,1,0,0;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;564;-1024,-544;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;561;-1280,-768;Float;False;0;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;565;-1024,-464;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;562;-832,-768;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;575;-624,-768;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;760;-176,-816;Float;False;1636.929;429.4401;Main Texture and Color;6;588;18;487;616;409;486;;0,0.751724,1,1;0;0
Node;AmplifyShaderEditor.FunctionNode;1155;-1280,-1600;Float;False;ADS Motion Noise;8;;1115;047eb809542f42d40b4b5066e22cee72;0;0;1;FLOAT;85
Node;AmplifyShaderEditor.GetLocalVarNode;588;-128,-768;Float;False;587;0;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;587;-448,-768;Half;False;MainUVs;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FunctionNode;1187;-1280,-1664;Float;False;ADS Mask Generic;27;;1112;2cfc3815568565c4585aebb38bd7a29b;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;1203;-1024,-1568;Float;False;ADS Grass Size;5;;1253;6675a46c54a0e244fb369c824eead1af;0;0;1;FLOAT3;85
Node;AmplifyShaderEditor.FunctionNode;1114;-1024,-1664;Float;False;ADS Motion Global;0;;1254;a8838de3869103540a427ac470da4da6;0;2;136;FLOAT;0;False;133;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;1151;-1280,-2080;Half;False;Constant;_Float0;Float 0;20;0;Create;True;0;0;False;0;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;18;80,-768;Float;True;Property;_MainTex;Main Texture;25;1;[NoScaleOffset];Create;False;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;683;-1328,-2608;Float;False;1351.18;158.9082;;6;862;553;550;743;925;549;OPTIONS;1,0,0.503,1;0;0
Node;AmplifyShaderEditor.FunctionNode;1210;-1280,-2176;Float;False;ADS Debug Output;11;;1248;b1cb64986a81a724095fc49c3cc079c2;0;0;1;COLOR;43
Node;AmplifyShaderEditor.RegisterLocalVarNode;616;384,-640;Half;False;MainTexAlpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;409;768,-768;Half;False;Property;_Color;Main Color;24;0;Create;False;0;0;False;1;Header(Main);1,1,1,1;1,1,1,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;743;-640,-2560;Half;False;Property;_CullMode;Cull Mode;22;1;[Enum];Create;True;3;Off;0;Front;1;Back;2;0;True;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;791;-1280,-1824;Float;False;616;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;550;-1280,-2560;Half;False;Property;_SrcBlend;_SrcBlend;34;1;[HideInInspector];Create;True;0;0;True;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1150;-1072,-2176;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;486;1024,-768;Half;False;MainColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;487;384,-768;Half;False;MainTex;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;1143;-768,-1664;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;862;-256,-2560;Half;False;Property;_Cutoff;Cutout/Fade;23;0;Create;False;3;Off;0;Front;1;Back;2;0;True;1;Space(10);1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;549;-896,-2560;Half;False;Property;_Mode;Blend Mode;4;1;[Enum];Create;False;3;Opaque;0;Cutout;1;Fade;2;0;True;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;553;-1120,-2560;Half;False;Property;_DstBlend;_DstBlend;35;1;[HideInInspector];Create;True;0;0;True;0;10;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;925;-448,-2560;Half;False;Property;_ZWrite;_ZWrite;33;1;[HideInInspector];Create;True;2;Off;0;On;1;0;True;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-452,-2052;Float;False;True;2;Float;ADSShaderGUI;300;0;Lambert;Utils/ADS Debug;False;False;False;False;True;True;True;True;True;True;True;True;False;False;True;True;False;False;False;False;Off;0;False;925;0;False;-1;False;0;False;-1;0;False;-1;False;0;Masked;0;True;False;0;False;TransparentCutout;;AlphaTest;All;False;True;True;False;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;0;1;True;550;1;True;553;0;0;False;550;0;False;553;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;300;;-1;-1;-1;-1;0;False;0;0;True;743;-1;0;True;862;3;Include;VS_indirect.cginc;Pragma;instancing_options procedural:setup;Pragma;multi_compile GPU_FRUSTUM_ON __;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;564;0;563;1
WireConnection;564;1;563;2
WireConnection;565;0;563;3
WireConnection;565;1;563;4
WireConnection;562;0;561;0
WireConnection;562;1;564;0
WireConnection;575;0;562;0
WireConnection;575;1;565;0
WireConnection;587;0;575;0
WireConnection;1114;136;1187;0
WireConnection;1114;133;1155;85
WireConnection;18;1;588;0
WireConnection;616;0;18;4
WireConnection;1150;0;1210;43
WireConnection;1150;1;1151;0
WireConnection;486;0;409;0
WireConnection;487;0;18;0
WireConnection;1143;0;1114;0
WireConnection;1143;1;1203;85
WireConnection;0;0;1150;0
WireConnection;0;2;1150;0
WireConnection;0;10;791;0
WireConnection;0;11;1143;0
ASEEND*/
//CHKSM=E84115B437442262DFC5A0282A354EAD404F3B9C
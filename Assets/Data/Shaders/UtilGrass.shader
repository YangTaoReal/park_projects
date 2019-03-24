// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/Grass"
{
Properties { 
_MainTex ("Grass Texture", 2D) = "white" {} 
_TimeScale ("Time Scale", float) = 1 
_alphaValue("alphavalue",range(0,1))=0.5
_Color("Color",Color) = (1,1,1,1) 
} 

SubShader{ 
Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
Pass{ 
Tags{ "LightMode"="ForwardBase" } 
ZWrite On
Cull Back 
CGPROGRAM 
#pragma vertex vert 
#pragma fragment frag 
#pragma multi_compile_fog

#include "UnityCG.cginc" 
#include "Lighting.cginc"

sampler2D _MainTex; 
float4 _Color;
half _TimeScale; 
fixed _alphaValue;
struct a2v { 
float4 vertex : POSITION; 
float2 texcoord : TEXCOORD0; 
float2 texcoord1 : TEXCOORD1;
}; 
struct v2f { 
float4 pos : SV_POSITION; 
float2 uv : TEXCOORD0;
float2 uv2 : TEXCOORD1;
UNITY_FOG_COORDS(2)
}; 
v2f vert(a2v v){ 
v2f o; 
float4 offset = float4(0,0,0,0); 
offset.x = sin(3.1416 * _Time.y * clamp(v.texcoord.y-0.5, 0, 1)) * _TimeScale; 
o.pos = UnityObjectToClipPos(v.vertex + offset); 
o.uv = v.texcoord.xy; 
o.uv2 = ((v.texcoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
UNITY_TRANSFER_FOG(o,o.pos);
return o; 
} 
fixed4 frag(v2f i) : SV_Target{ 
fixed4 col = tex2D(_MainTex, i.uv); 
col.rgb *= DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uv2)) * _Color;
clip(col.a - _alphaValue);
UNITY_APPLY_FOG(i.fogCoord, col); 
return col;
} 
ENDCG 
} 
} 
FallBack "Mobile/Diffuse" 
}
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//X光效果
//by：puppet_master
//2017.6.20
 
Shader "Custom/BuildingPlane"
{
    Properties
    {
        _XRayColor("XRay Color", Color) = (1,1,1,1)
    }
 
    SubShader
    {
        Tags{ "Queue" = "Transparent+100" "RenderType" = "Opaque" }

        //渲染X光效果的Pass
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            //Blend SrcAlpha One
            ZWrite Off
            ZTest Greater
 
            CGPROGRAM
            #include "Lighting.cginc"
            fixed4 _XRayColor;
            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 normal : normal;
                float3 viewDir : TEXCOORD0;
            };
 
            v2f vert (appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.viewDir = ObjSpaceViewDir(v.vertex);
                o.normal = v.normal;
                return o;
            }
 
            fixed4 frag(v2f i) : SV_Target
            {
                //float3 normal = normalize(i.normal);
                //float3 viewDir = normalize(i.viewDir);
                //float rim = 1 - dot(normal, viewDir);
                return _XRayColor * 1;
            }
            #pragma vertex vert
            #pragma fragment frag
            ENDCG
        }
        
        //正常渲染的Pass
        Pass
        {
 
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite On
 
            CGPROGRAM
            #include "Lighting.cginc"
            fixed4 _XRayColor;
            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 normal : normal;
                float3 viewDir : TEXCOORD0;
            };
 
            v2f vert (appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.viewDir = ObjSpaceViewDir(v.vertex);
                o.normal = v.normal;
                return o;
            }
 
            fixed4 frag(v2f i) : SV_Target
            {
                //float3 normal = normalize(i.normal);
                //float3 viewDir = normalize(i.viewDir);
                //float rim = 1 - dot(normal, viewDir);
                return _XRayColor * 1;
            }
            #pragma vertex vert
            #pragma fragment frag
            ENDCG
        }
    }
    
    FallBack "Diffuse"
}


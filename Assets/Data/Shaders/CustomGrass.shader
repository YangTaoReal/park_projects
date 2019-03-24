// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Grass" {
    Properties {
        _MainTex ("Grass Texture", 2D) = "white" {}
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Settings ("Settings", Vector) = (1.5,0.1,0.5,0)
        //_VerticalBillboarding ("Vertical Restraints", Range(-500, 500)) = 0

    }
 
    SubShader{
        Tags{"Queue"="Transparent-10" "RenderType"="Grass" "IgnoreProject"="True"}// "DisableBatching"="True"}
        LOD 120
        Pass{
            Tags{"LightMode"="ForwardBase"}
 
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Back
 
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
 
            sampler2D _MainTex;
            sampler2D _NoiseTex;
            float4 _NoiseTex_ST;
            uniform fixed4 _Settings;
            fixed4 _Color;
            fixed _VerticalBillboarding;
 
            struct a2v {
                fixed4 vertex : POSITION;
                fixed4 texcoord : TEXCOORD0;
                fixed4 vertexColor : COLOR;
            };
            
            struct v2f {
                fixed4 pos : SV_POSITION;
                fixed2 uv : TEXCOORD0;
                fixed3 noiseST_light : COLOR0;                     
            };

            fixed3 cac(a2v v)
            {
                fixed3 center = fixed3(0, 0, 0);                // 模型空间的原点作为广告牌锚点
                fixed3 viewer = mul(unity_WorldToObject,fixed4(_WorldSpaceCameraPos, 1));   // 获取模型空间下视角
                fixed3 normalDir = viewer - center;
                normalDir.y = 0;
                normalDir = normalize(normalDir);
                fixed3 upDir = fixed3(0, 1, 0);
                fixed3 rightDir = normalize(cross(upDir, normalDir));
                upDir = normalize(cross(normalDir, rightDir));  
                fixed3 centerOffs = v.vertex.xyz - center;
                fixed3 localPos = center + rightDir * centerOffs.x + upDir * centerOffs.y + normalDir * centerOffs.z;

                return localPos;
            }
 
            v2f vert(a2v v)
            {
                v2f o;
                fixed4 worldPos = mul(unity_ObjectToWorld, v.vertex);
                fixed2 settings = (_Settings.b*worldPos.rgb).rb;
                v.vertex.xz += v.vertexColor.r * _Settings.g * sin((_Settings.r * _Time.g) + settings.r + settings.g);
                //o.pos = UnityObjectToClipPos(fixed4(cac(v), 1));      // 模型转裁剪空间
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord.xy;
                worldPos.xz +=_NoiseTex_ST.zw;
                worldPos.xz /= _NoiseTex_ST.xy;
                o.noiseST_light.xy = worldPos.xz;
                o.noiseST_light.z = _LightColor0.a + 0.25;
                if(o.noiseST_light.z > 1) o.noiseST_light.z = 1;
                return o;
            }

 
            fixed4 frag(v2f i) : SV_Target{
                fixed4 noiseTex = tex2D(_NoiseTex, i.noiseST_light.xy);
                return tex2D(_MainTex, i.uv) * _Color.rgba * noiseTex * fixed4(i.noiseST_light.z, i.noiseST_light.z, i.noiseST_light.z, 1);
            }
 
            ENDCG
        }
    }

    SubShader{
        LOD 120
        UsePass "Unlit/Transparent"
    }

    FallBack "Unlit/Transparent"
}


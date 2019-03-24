// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Plant" {
Properties {
    _MainTex ("Base (RGB)", 2D) = "white" {}
    _Settings ("Settings", Vector) = (1.5,0.1,0.5,0)
}
SubShader {
    Tags { "RenderType"="Opaque" }
    LOD 150

CGPROGRAM
#pragma surface surf Lambert noforwardadd vertex:vert

sampler2D _MainTex;
uniform fixed4 _Settings;

struct Input {
    fixed2 uv_MainTex;
};

void vert (inout appdata_full v) {
          fixed2 settings = (_Settings.b*mul(unity_ObjectToWorld, v.vertex).rgb).rb;
          v.vertex.xz += v.color.r * _Settings.g * sin((_Settings.r * _Time.g) + settings.r + settings.g);
      }

void surf (Input IN, inout SurfaceOutput o) {
    fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
    o.Albedo = c.rgb;
    o.Alpha = c.a;
}
ENDCG
}

Fallback "Mobile/VertexLit"
}
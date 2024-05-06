Shader "TulipMania/CYMKNoise"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ResX ("Resolution - X", float) = 0
        _ResY ("Resolution - Y", float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Overlay" }
        LOD 100

        Pass
        {
            Cull Off
            Lighting Off
            ZWrite Off
            Fog { Mode Off }
            Offset -1, -1
            ColorMask RGB
            AlphaTest Greater .01
            Blend SrcAlpha OneMinusSrcAlpha
            ColorMaterial AmbientAndDiffuse
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float _ResX;
            float _ResY;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            int call = 0;
            float random (float2 uv)
            {
                ++call;
                return frac(sin(_Time.y * call * dot(uv,float2(12.9898,78.233)))*43758.5453123);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                call = 0;
                float scX = round(i.uv.x * _ResX)/_ResX;
                float scY = round(i.uv.y * _ResY)/_ResY;
                float2 sc = float2(scX, scY);
                float value = random(float2(_Time.y, random(sc)));
                fixed4 col = fixed4(step(0.67, random(sc)) * value, step(0.67, random(sc)) * value, step(0.67, random(sc)) * value, 0.4f);
                return col;
            }
            ENDCG
        }
    }
}

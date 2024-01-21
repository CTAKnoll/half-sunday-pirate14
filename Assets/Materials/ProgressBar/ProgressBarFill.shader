Shader "IdleGame/ProgressBarFill"
{
    Properties
    {
        _MainTex("Texture", 2D) = "gray" {}
        _Fill("Fill", float) = 0
        _BackgroundColor("Background Color", Color) = (.5, .5, .5, 1)
        _FillColor("Fill Color", Color) = (0, 0, 1, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
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
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            
            float _Fill;
            float4 _BackgroundColor;
            float4 _FillColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float isFilled = step(i.uv.y, _Fill);
                fixed4 col = (1 - isFilled) * _BackgroundColor + (isFilled) * _FillColor;
                return col;
            }
            ENDCG
        }
    }
}

Shader "TulipMania/TriColorFill"
{
    Properties
    {
        _MainTex("Texture", 2D) = "gray" {}
        _Fill("Fill", float) = 0
        _BackgroundColor("Background Color", Color) = (.5, .5, .5, 1)
        _TopFillColor("Top Fill Color", Color) = (0, 0, 1, 1)
        _MidFillColor("Middle Fill Color", Color) = (0, 0, 1, 1)
        _BotFillColor("Bottom Fill Color", Color) = (0, 0, 1, 1)
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
            float4 _TopFillColor;
            float4 _MidFillColor;
            float4 _BotFillColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float isFilled = step(i.uv.x, _Fill);
                float bottomThird = step(i.uv.x, .33);
                float middleThird = step(.33, i.uv.x) * step(i.uv.x, .67);
                float topThird = step(.67, i.uv.x);
                fixed4 col = (1 - isFilled) * _BackgroundColor + isFilled * (bottomThird * _BotFillColor + middleThird * _MidFillColor + topThird * _TopFillColor);
                return col;
            }
            ENDCG
        }
    }
}

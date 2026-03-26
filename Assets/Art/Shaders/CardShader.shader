Shader "Card"
{
    Properties
    {
        _MainTex("Front", 2D) = "white" {}
        _BackTex("Back Mask", 2D) = "black" {}
        _Card ("Color Card", Color) = (1,1,1,1)
        _Back0 ("Color Back 0", Color) = (0,0,0,0)
        _Back1 ("Color Back 1", Color) = (1,1,1,1)
        _Add("Color Add", Color) = (0,0,0,0)
        _Multiply("Color Multiply", Color) = (1,1,1,1)
        _ReflectionValue("Reflection", Range(0,1)) = 0
        _BurnNoise("Burn Noise", 2D) = "white" {}
        _BurnValue ("Burn Amount", Range(0,1)) = 0
        _Burn ("Burn Color", Color) = (0,0,0,0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }


        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
            #include "cginc/Utils.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 col : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float3 col : COLOR;
                float2 uvFront : TEXCOORD0;
                float2 uvBack : TEXCOORD1;
                float3 localPos : TEXCOORD2;
                UNITY_FOG_COORDS(3)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _BackTex;
            float4 _BackTex_ST;
            float4 _Back0;
            float4 _Back1;
            float4 _Card;
            float4 _Add;
            float4 _Multiply;
            float _ReflectionValue;
            sampler2D _BurnNoise;
            float4 _BurnNoise_ST;
            float _BurnValue;
            float4 _Burn;

            v2f vert (appdata v)
            {
                v2f o;

                o.localPos = v.vertex;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uvFront = TRANSFORM_TEX(v.uv, _MainTex);
                o.uvBack = TRANSFORM_TEX(v.uv, _BackTex);
                o.col = v.col;

                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            float inverseLerp(float a, float b, float v) 
            {
                return saturate((v - a) / (b - a));
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 burnUV = i.localPos.xy * _BurnNoise_ST.xy + _BurnNoise_ST.zw;
                fixed4 burnMask = tex2D(_BurnNoise, burnUV);
                float burnRawValue = saturate(1-burnMask.r);
                
                float burn = lerp(-0.25, 1.25, _BurnValue);

                float burnMaskValue = step(burn, burnRawValue);
                float burnEdge = 1.0 - step(burn + 0.2, burnRawValue);

                clip(burnMaskValue-0.5);

                float cardHeight = 0.6;
                fixed4 col = (1,1,1,1);

                fixed4 frontTex = tex2D(_MainTex, i.uvFront);
                float frontValue = (frontTex.r + frontTex.g + frontTex.b) / 3.0;
                fixed4 frontColor = lerp(_Card, frontTex, step(frontValue, 0.9));

                fixed4 backMask = tex2D(_BackTex, i.uvBack);
                fixed4 backColor = lerp(_Back0, _Back1, backMask.r);

                col = frontColor * i.col.b + backColor * i.col.g + _Card * i.col.r;

                float2 reflectionDir = normalize(float2(1, 1));
                float projectedPos = dot(i.localPos.xy, reflectionDir);
                float reflectionInput = lerp(cardHeight * -2, cardHeight * 2, _ReflectionValue);
                float blendedReflection = saturate(abs(projectedPos + reflectionInput) / (cardHeight * 0.5));
                float hardReflection = step(blendedReflection, 0.6) * 0.8;

                col += hardReflection;
                col += _Add;
                col *= _Multiply;
                col = lerp(col, _Burn, burnEdge);

                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);

                return col;
            }
            ENDCG
        }
    }
}

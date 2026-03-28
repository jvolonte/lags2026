Shader "Sticker"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Add("Color Add", Color) = (0,0,0,0)
        _Multiply("Color Multiply", Color) = (1,1,1,1)
        _ReflectionValue("Reflection", Range(0,1)) = 0
        _BurnNoise("Burn Noise", 2D) = "white" {}
        _BurnValue ("Burn Amount", Range(0,1)) = 0
        _Burn ("Burn Color", Color) = (0,0,0,0)
        [Toggle(WORLD_CLIP_ON)] _WorldClipEnabled("World Clip Enabled", Float) = 0
        _BottomThreshold("Bottom World Threshold", Float) = 10
        _TopThreshold("Top World Threshold", Float) = 20
        _DitherEdge("Dither Edge Width", Float) = 0.5
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
            #pragma shader_feature_local WORLD_CLIP_ON

            #include "UnityCG.cginc"
            #include "cginc/Utils.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float3 localPos : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
                UNITY_FOG_COORDS(3)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Add;
            float4 _Multiply;
            float _ReflectionValue;
            sampler2D _BurnNoise;
            float4 _BurnNoise_ST;
            float _BurnValue;
            float4 _Burn;
            float _BottomThreshold;
            float _TopThreshold;
            float _DitherEdge;

            v2f vert (appdata v)
            {
                v2f o;

                o.localPos = v.vertex;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex); 

                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            float invLerp(float a, float b, float v)
            {
                return (v - a) / (b - a);
            }

            float Dither4x4(float2 screenPos)
            {
                const float dither[16] = {
                    0.0/16.0,  8.0/16.0,  2.0/16.0, 10.0/16.0,
                    12.0/16.0, 4.0/16.0, 14.0/16.0,  6.0/16.0,
                    3.0/16.0, 11.0/16.0,  1.0/16.0,  9.0/16.0,
                    15.0/16.0, 7.0/16.0, 13.0/16.0,  5.0/16.0
                };
                uint2 pixel = uint2(screenPos) % 4;
                return dither[pixel.y * 4 + pixel.x];
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 tex = tex2D(_MainTex, i.uv);

                clip(tex.a - 0.5);

                float2 burnUV = i.localPos.xy * _BurnNoise_ST.xy + _BurnNoise_ST.zw;
                fixed4 burnMask = tex2D(_BurnNoise, burnUV);
                float burnRawValue = saturate(1-burnMask.r);
                
                float burn = lerp(-0.25, 1.25, _BurnValue);

                float burnMaskValue = step(burn, burnRawValue);
                float burnEdge = 1.0 - step(burn + 0.2, burnRawValue);

                clip(burnMaskValue-0.5);

                #if defined(WORLD_CLIP_ON)
                    float ditherThreshold = Dither4x4(i.vertex.xy);
                    float fromBottom = i.worldPos.y - _BottomThreshold;
                    float fromTop = _TopThreshold - i.worldPos.y;
                    float bottomFade = saturate(fromBottom / _DitherEdge);
                    float topFade = saturate(fromTop / _DitherEdge);
                    float worldMask = min(bottomFade, topFade);
                    clip(worldMask - ditherThreshold);
                #endif

                fixed4 col = (1,1,1,1);

                float stickerHeight = 0.5;
                float2 reflectionDir = normalize(float2(1, 1));
                float projectedPos = dot(i.localPos.xy, reflectionDir);
                float reflectionInput = lerp(stickerHeight * -2, stickerHeight * 2, _ReflectionValue);
                float blendedReflection = saturate(abs(projectedPos + reflectionInput) / (stickerHeight * 0.5));
                float hardReflection = step(blendedReflection, 0.6) * 0.8; 

                col.xyz = tex.rgb;

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

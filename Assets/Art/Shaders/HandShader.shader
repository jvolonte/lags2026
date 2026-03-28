Shader "Hand"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Color0("Color 1", Color) = (0,0,0,0)
        _Color1("Color 2", Color) = (1,1,1,1)
        _Add("Color Add", Color) = (0,0,0,0)
        _Multiply("Color Multiply", Color) = (1,1,1,1)
        _Noise0("Noise 01", 2D) = "black" {}
        _Noise1("Noise 02", 2D) = "black" {}
        _NoiseScroll("Noise Scroll Speed", Vector) = (0.1, 0.1, 0, 0)
        _GradientApex("Gradient Apex Color", Color) = (1,1,1,1)
        _GradientFade("Gradient Fade Color", Color) = (0,0,0,0)
        _GradientScale("Gradient Scale", Float) = 1.0
        _GradientScroll("Gradient Scroll Speed", Float) = 0.5
        _DitherThreshold("Dither Threshold", Range(0, 1)) = 0.5
        _DitherScale("Dither Scale", Float) = 1.0
        _EffectBlend("Effect Blend", Range(0, 1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Cull Off
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
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
                float4 screenPos : TEXCOORD2;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _Noise0;
            float4 _Noise0_ST;
            sampler2D _Noise1;
            float4 _Noise1_ST;
            float4 _NoiseScroll;
            float4 _GradientApex;
            float4 _GradientFade;
            float _GradientScale;
            float _GradientScroll;
            float4 _Add;
            float4 _Multiply;
            float4 _Color0;
            float4 _Color1;
            float _DitherThreshold;
            float _DitherScale;
            float _EffectBlend;

            // 4x4 Bayer matrix dithering
            float Bayer4x4(float2 screenPos)
            {
                const float4x4 bayerMatrix = float4x4(
                     0.0/16.0,  8.0/16.0,  2.0/16.0, 10.0/16.0,
                    12.0/16.0,  4.0/16.0, 14.0/16.0,  6.0/16.0,
                     3.0/16.0, 11.0/16.0,  1.0/16.0,  9.0/16.0,
                    15.0/16.0,  7.0/16.0, 13.0/16.0,  5.0/16.0
                );
                int2 coord = int2(fmod(screenPos, 4.0));
                return bayerMatrix[coord.x][coord.y];
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.screenPos = ComputeScreenPos(o.vertex);
                UNITY_TRANSFER_FOG(o, o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 tex = tex2D(_MainTex, i.uv);
                clip(tex.a - 0.5);

                // Scroll UVs in opposite directions
                float2 scrollOffset = _NoiseScroll.xy * _Time.y;
                float2 uv0 = i.uv * _Noise0_ST.xy + _Noise0_ST.zw + scrollOffset;
                float2 uv1 = i.uv * _Noise1_ST.xy + _Noise1_ST.zw - scrollOffset;

                // Sample both noise textures
                float n0 = tex2D(_Noise0, uv0).r;
                float n1 = tex2D(_Noise1, uv1).r;

                // Cancel each other: difference remapped to 0-1
                float noiseValue = abs(n0 - n1);

                fixed4 col = lerp(_Color0, _Color1, tex.r);

                // Check if inside threshold zone
                if (noiseValue < _DitherThreshold)
                {
                    // Get screen pixel position for dither pattern
                    float2 screenPixel = (i.screenPos.xy / i.screenPos.w) * _ScreenParams.xy / _DitherScale;
                    float dither = Bayer4x4(screenPixel);
                    
                    // Normalize noise within threshold for dither blend
                    float blend = noiseValue / _DitherThreshold;

                    // Scrolling vertical gradient with apex in center
                    float gradientT = frac(i.uv.y * _GradientScale + _Time.y * _GradientScroll);
                    // Convert 0->1 to 0->1->0 (tent shape: apex at center)
                    float intensity = 1.0 - abs(gradientT * 2.0 - 1.0);
                    fixed4 gradientColor = lerp(_GradientFade, _GradientApex, intensity);
                    
                    // Apply dithered gradient
                    col = (blend > dither) ? col : lerp(col, gradientColor, _EffectBlend);
                }

                col += _Add;
                col *= _Multiply;

                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
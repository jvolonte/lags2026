Shader "Sticker"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Add("Color Add", Color) = (0,0,0,0)
        _Multiply("Color Multiply", Color) = (1,1,1,1)
        _ReflectionValue("Reflection", Range(0,1)) = 0
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
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float3 localPos : TEXCOORD1;
                UNITY_FOG_COORDS(2)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Add;
            float4 _Multiply;
            float _ReflectionValue;

            v2f vert (appdata v)
            {
                v2f o;

                o.localPos = v.vertex;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex); 

                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 tex = tex2D(_MainTex, i.uv);

                clip(tex.a - 0.5);

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

                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}

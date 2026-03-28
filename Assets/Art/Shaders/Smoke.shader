Shader "Smoke"
{
    Properties
    {
        _Noise("Noise", 2D) = "black" {}
        _Color("Color", Color) = (0,0,0,0)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent"  "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off


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
                float3 worldPos : TEXCOORD1;
                UNITY_FOG_COORDS(2)
                float4 vertex : SV_POSITION;
            };

            sampler2D _Noise;
            float4 _Noise_ST;
            float4 _Color;

            v2f vert (appdata v)
            {
                v2f o;

                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv; 

                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 noiseUV = i.worldPos * _Noise_ST.xy;
                noiseUV.y -= _Time.x;
                float4 noise = tex2D(_Noise, noiseUV);

                float centeredX = i.uv.x - 0.5;
                
                float distanceFromCenter = abs(centeredX);
                float maxAllowedDistance = 0.5 * i.uv.y * noise.r;
                
                float smokeSilouhette = maxAllowedDistance - distanceFromCenter;

                clip(smokeSilouhette);

                fixed4 col = _Color;
                col.a = 1-i.uv.y;

                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}

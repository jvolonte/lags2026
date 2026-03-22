Shader "Vertex Color Folliage"
{
    Properties
    {
        _OverrideColor ("Override Color", Color) = (0,0,0,0)
        _Wind ("Wind", Vector) = (0,0,0,0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Cull Off
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
            #include "cginc/VertexWiggle.cginc"
            #include "cginc/Utils.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 col : COLOR;
            };

            struct v2f
            {
                float4 col : COLOR;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            float4 _OverrideColor;
            float4 _Wind;

            float2 windVertexDisplacement (appdata v, float offset) 
            {
                float phase = v.vertex.x * 0.7 + v.vertex.y * 0.3 + offset; 

                float steppedTime = _Time.y;

                float sway1 = sin(steppedTime * _Wind.z + phase);               
                float sway2 = sin(steppedTime * _Wind.z * 2.3 + phase * 1.7) * 0.5; 
                float sway3 = sin(steppedTime * _Wind.z * 0.4 + phase * 0.6) * 0.3;
                float sway4 = sin(steppedTime * _Wind.z * 0.1 + phase * 0.9) * 0.4;

                float swayY = sway1 + sway2 + sway3;
                float swayX = sway2 + sway4 + sway3;

                float windY = max(_Wind.y * swayY * _Wind.w * (1-v.col.a), 0);
                float windX = _Wind.x * swayX * _Wind.w * (1-v.col.a);

                return float2(windX, windY);
            }

            v2f vert (appdata v)
            {
                v2f o;

                float3 worldOrigin = UNITY_MATRIX_M._m03_m13_m23;
                float offset = worldOrigin.x + worldOrigin.z;

                v.vertex.xz += windVertexDisplacement(v, sin(offset*0.5));
                o.vertex = UnityObjectToClipPos(v.vertex);
                
                //o.vertex = VertexWiggle(v.vertex);
                o.col = v.col;

                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = (1,1,1,1);
                float3 vertCol = GAMMA_CORRECTION(i.col);
                col.xyz = lerp(vertCol, _OverrideColor, _OverrideColor.a);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}

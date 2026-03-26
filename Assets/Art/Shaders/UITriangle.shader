Shader "UI/Triangle"
{
    Properties
    {
        // -- Image source texture (auto-bound by Unity UI) --
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}

        // -- Triangle vertices (normalized 0-1 inside rect) --
        _PointA ("Point A", Vector) = (0.5, 1.0, 0, 0)
        _PointB ("Point B", Vector) = (0.0, 0.0, 0, 0)
        _PointC ("Point C", Vector) = (1.0, 0.0, 0, 0)

        // -- UI stencil / masking plumbing --
        [HideInInspector]
        _StencilComp ("Stencil Comparison", Float) = 8
        [HideInInspector]
        _Stencil     ("Stencil ID",         Float) = 0
        [HideInInspector]
        _StencilOp   ("Stencil Operation",  Float) = 0
        [HideInInspector]
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        [HideInInspector]
        _StencilReadMask  ("Stencil Read Mask",  Float) = 255

        [HideInInspector]
        _ColorMask ("Color Mask", Float) = 15

        [HideInInspector]
        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"           = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType"      = "Transparent"
            "PreviewType"     = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Stencil
        {
            Ref       [_Stencil]
            Comp      [_StencilComp]
            Pass      [_StencilOp]
            ReadMask  [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend One OneMinusSrcAlpha // Premultiplied alpha (Unity UI standard)
        ColorMask [_ColorMask]

        Pass
        {
            Name "Default"

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            // ---------------------------------------------------------------
            // Structs
            // ---------------------------------------------------------------
            struct appdata
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;   // Image component color
                float2 uv       : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 uv       : TEXCOORD0;
                float4 worldPos : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            // ---------------------------------------------------------------
            // Uniforms
            // ---------------------------------------------------------------
            sampler2D _MainTex;
            float4    _MainTex_ST;
            float4    _ClipRect;

            float4 _PointA;
            float4 _PointB;
            float4 _PointC;

            // ---------------------------------------------------------------
            // Vertex
            // ---------------------------------------------------------------
            v2f vert(appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.worldPos = v.vertex;
                o.vertex   = UnityObjectToClipPos(v.vertex);
                o.uv       = TRANSFORM_TEX(v.uv, _MainTex);
                o.color    = v.color; // pass Image color through
                return o;
            }

            // ---------------------------------------------------------------
            // Point-in-triangle via sign of cross products
            // Returns 1 inside, 0 outside.
            // ---------------------------------------------------------------
            float Sign2D(float2 p1, float2 p2, float2 p3)
            {
                return (p1.x - p3.x) * (p2.y - p3.y)
                     - (p2.x - p3.x) * (p1.y - p3.y);
            }

            float PointInTriangle(float2 pt, float2 a, float2 b, float2 c)
            {
                float d1 = Sign2D(pt, a, b);
                float d2 = Sign2D(pt, b, c);
                float d3 = Sign2D(pt, c, a);

                bool hasNeg = (d1 < 0.0) || (d2 < 0.0) || (d3 < 0.0);
                bool hasPos = (d1 > 0.0) || (d2 > 0.0) || (d3 > 0.0);

                return (hasNeg && hasPos) ? 0.0 : 1.0;
            }

            // ---------------------------------------------------------------
            // Fragment
            // ---------------------------------------------------------------
            fixed4 frag(v2f i) : SV_Target
            {
                // Triangle test in normalised UV space
                float inside = PointInTriangle(
                    i.uv,
                    _PointA.xy,
                    _PointB.xy,
                    _PointC.xy
                );

                // Image color (RGBA) with triangle mask baked into alpha
                half4 col = i.color;
                col.a *= inside;

                // Premultiplied alpha (matches the Blend mode)
                col.rgb *= col.a;

                // UI RectMask2D support
                #ifdef UNITY_UI_CLIP_RECT
                col.a *= UnityGet2DClipping(i.worldPos.xy, _ClipRect);
                #endif

                // Alpha-clip masking support
                #ifdef UNITY_UI_ALPHACLIP
                clip(col.a - 0.001);
                #endif

                return col;
            }
            ENDCG
        }
    }
}

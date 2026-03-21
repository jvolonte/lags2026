//Shader by Minions Art
//https://www.patreon.com/posts/wind-waker-style-78831006
// Pass 1: Mask          ? stamps bit 1 (back faces behind geometry)
// Pass 2: Marker Outside? stamps bit 2 where light is visible (no color)
// Pass 3: Render Outside? draws light, clears bit 1 (including Zfail)
// Pass 4: Marker Inside ? stamps bit 2 only where bit 1 survives (camera inside)
// Pass 5: Render Inside ? draws light, clears bit 1
// Interleaving ensures Render Outside kills bit 1 on occluded front-face
// pixels BEFORE Marker Inside runs, preventing false bit 2 stamps.

Shader "Aftersun/Stencil Light"
{
    Properties
    {
        [HDR]_Color("Color",Color) = (1,1,1,0.5)       
    }   
    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
        }    
        CGINCLUDE
        float4 _Color;
        struct appdata
        {
            float4 vertex : POSITION;
        };
        
        struct v2f
        {
            float4 vertex : SV_POSITION;
        };

        v2f vert(appdata v)
        {
            v2f o;
            o.vertex = UnityObjectToClipPos(v.vertex);
            return o;
        }
        
        fixed4 frag(v2f i) : SV_Target
        {
            return _Color * _Color.a;
        }

        ENDCG

        // ?? Pass 1: Mask ??
        // Back faces behind scene geometry ? stamp bit 1
        Pass
        {
            Name "Mask"
            Ztest Greater
            Zwrite Off
            Cull Front
            Colormask 0
            
            Stencil
            {
                Ref 1
                ReadMask 1
                WriteMask 1
                Comp Greater
                Pass Replace
            }
        }

        // ?? Pass 2: Marker Outside ??
        // Front faces in front of geometry where bit 1 is set ? stamp bit 2
        // Zfail Keep: do NOT stamp bit 2 on depth-failed (occluded) pixels
        // Fail Keep: preserve bit 2 from other lights
        Pass
        {
            Name "Marker Outside"
            Zwrite Off
            Ztest Lequal
            Cull Back
            Colormask 0

            Stencil
            {
                Ref 2
                ReadMask 1
                WriteMask 2
                Comp NotEqual
                Pass Replace
                Fail Keep
                Zfail Keep
            }
        }

        // ?? Pass 3: Render Outside ??
        // Draws light. Clears bit 1 on BOTH Pass and Zfail.
        // This kills bit 1 for ALL front-face pixels, including occluded ones,
        // so that Marker Inside cannot false-stamp bit 2 on those pixels.
        Pass
        {
            Name "Light Outside"
            Zwrite Off
            Ztest Lequal
            Cull Back
            Blend DstColor One
            
            Stencil
            {
                Ref 1
                ReadMask 1
                WriteMask 1
                Comp Equal
                Pass Zero
                Fail Keep
                Zfail Zero
            }
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            ENDCG
        }

        // ?? Pass 4: Marker Inside ??
        // Only fires where bit 1 survived Pass 3 (camera inside volume).
        // For convex meshes, front faces cover the same footprint as back faces,
        // so Pass 3 clears bit 1 everywhere when camera is outside.
        Pass
        {
            Name "Marker Inside"
            ZTest Off
            ZWrite Off
            Cull Front
            Colormask 0

            Stencil
            {
                Ref 2
                ReadMask 1
                WriteMask 2
                Comp NotEqual
                Pass Replace
                Fail Keep
            }
        }

        // ?? Pass 5: Render Inside ??
        // Draws light from inside volume. Clears bit 1.
        Pass
        {
            Name "Light Inside"
            ZTest Off
            ZWrite Off
            Cull Front
            Blend DstColor One
            
            Stencil
            {
                Ref 1
                ReadMask 1
                WriteMask 1
                Comp Equal
                Pass Zero
            }
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            ENDCG
        }
    }
}

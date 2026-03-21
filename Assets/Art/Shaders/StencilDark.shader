//Shader by Minions Art
//https://www.patreon.com/posts/wind-waker-style-78831006
// Modified: Queue Transparent+1 (always after light)
// Mask stamps bit 4. Render passes read bits 2+4 (ReadMask 6).
// If light left bit 2 set, shadow is blocked in that area.

Shader "Aftersun/Stencil Shadow"
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
            "Queue" = "Transparent+1"
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
        Pass
        {
            Name "Mask"
            Ztest Greater
            Zwrite Off
            Cull Front
            Colormask 0
            
            Stencil
            {
                Ref 4
                ReadMask 4
                WriteMask 4
                Comp Greater
                Pass Replace
            }
        }
        
        Pass
        {
            Name "Shadow Outside"
            Zwrite Off
            Ztest Lequal
            Cull Back
            Blend DstColor Zero
            
            Stencil
            {
                Ref 4
                ReadMask 6
                WriteMask 4
                Comp Equal
                Pass Zero
                Fail Zero
                Zfail Zero
            }
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            ENDCG
        }
        
        Pass
        {
            Name "Shadow Inside"
            ZTest Off
            ZWrite Off
            Cull Front
            Blend DstColor Zero
            
            Stencil
            {
                Ref 4
                ReadMask 6
                WriteMask 4
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

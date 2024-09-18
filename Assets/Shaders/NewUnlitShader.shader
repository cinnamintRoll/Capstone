Shader "Unlit/UnlitWithStencil"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {} // Ensure the texture property is correctly defined
    }

    SubShader
    {
        Tags {"Queue" = "Overlay" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
        LOD 100

        Pass
        {
            Stencil
            {
                Ref 1
                Comp Always
                Pass Replace
            }

            Blend SrcAlpha OneMinusSrcAlpha // Correctly blend the texture with transparency

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float2 texcoord : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;          // Define the main texture sampler
            float4 _MainTex_ST;          // Transformation for texture

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.texcoord); // Sample the texture using UV coordinates
                return col;  // Return the color from the texture
            }
            ENDCG
        }
    }

    FallBack "Unlit/Transparent"
}

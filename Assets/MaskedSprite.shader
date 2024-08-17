Shader "Custom/MaskedSprite" {
    Properties{
        _MainTex("Sprite Texture", 2D) = "white" {}
        _MaskTex("Mask Texture", 2D) = "white" {}
    }
        SubShader{
            Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
            Blend SrcAlpha OneMinusSrcAlpha
            Pass {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                struct appdata_t {
                    float4 vertex : POSITION;
                    float2 texcoord : TEXCOORD0;
                };

                struct v2f {
                    float2 texcoord : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                };

                sampler2D _MainTex;
                sampler2D _MaskTex;

                v2f vert(appdata_t v) {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.texcoord = v.texcoord;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target {
                    fixed4 color = tex2D(_MainTex, i.texcoord);
                    fixed4 mask = tex2D(_MaskTex, i.texcoord);

                    // Ensure only areas within the mask are visible
                    if (mask.r == 0) {
                        discard; // Discard the fragment if the mask is fully transparent
                    }

                    color.a *= mask.r; // Adjust transparency based on mask
                    return color;
                }
                ENDCG
            }
    }
}

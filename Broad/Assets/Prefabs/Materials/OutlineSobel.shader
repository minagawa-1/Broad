Shader "EdgeDetection" {
    Properties{
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}

        _Thickness("Line Thickness", float) = 0.1
        _Color("Line Color", Color) = (1, 0, 0, 1)
    }

        SubShader{
            Tags { "Queue" = "Transparent" }
            Pass {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                struct appdata_t {
                    float4 vertex : POSITION;
                };

                struct v2f {
                    float4 vertex : SV_POSITION;
                    float2 uv : TEXCOORD0;
                };

                sampler2D _MainTex;
                float _Thickness;
                float4 _Color;

                v2f vert(appdata_t v) {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.vertex.xy; // Use vertex position as UV
                    return o;
                }

                half4 frag(v2f i) : SV_Target {
                    // Sample the screen texture and its neighbors for edge detection
                    float center = tex2D(_MainTex, i.uv).r;
                    float left   = tex2D(_MainTex, i.uv + float2(-1,  0) * _Thickness).r;
                    float right  = tex2D(_MainTex, i.uv + float2( 1,  0) * _Thickness).r;
                    float top    = tex2D(_MainTex, i.uv + float2( 0,  1) * _Thickness).r;
                    float bottom = tex2D(_MainTex, i.uv + float2( 0, -1) * _Thickness).r;

                    // Compute the gradient magnitude
                    float gradient = length(float2(right - left, top - bottom));

                    // Apply a threshold to create a binary edge image
                    float edge = saturate(gradient - 0.5);

                    // Mix the edge color with the background color based on the edge value
                    half4 result = lerp(_Color, tex2D(_MainTex, i.uv), edge);

                    return result;
                }
                ENDCG
            }
    }
}
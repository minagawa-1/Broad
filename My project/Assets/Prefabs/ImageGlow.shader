Shader "Custom/ImageGlow"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _GlowColor("GlowColor", Color) = (1,1,1,1)
        _GlowPower("GlowPower", Range(0, 1)) = 0.5
    }
        SubShader{
            Tags {"Queue" = "Transparent" "RenderType" = "Transparent" }
            LOD 200
            Blend SrcAlpha OneMinusSrcAlpha
            Pass {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                struct appdata {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct v2f {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                };

                sampler2D _MainTex;
                float4 _GlowColor;
                float _GlowPower;

                v2f vert(appdata v) {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target {
                    fixed4 col = tex2D(_MainTex, i.uv);

                fixed4 glow = _GlowColor * _GlowPower;

                return fixed4(col.rgb + glow.rgb, col.a);
            }
            ENDCG
        }
        }
            FallBack "Diffuse"
}

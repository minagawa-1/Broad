Shader "Custom/Contrast"
{
    Properties
    {
        _Contrast("Contrast", Range(0, 10)) = 0.5
    }
    SubShader
    {
        Cull Off
        ZTest Always
        ZWrite Off

        Tags { "RenderType" = "Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv     : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            // 変数のプロトタイプ宣言
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Contrast;

            // 関数のプロトタイプ宣言
            fixed contrast(fixed previousValue);

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex    = UnityObjectToClipPos(v.vertex);
                o.uv        = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 画面のカラーを取得
                fixed4 color = tex2D(_MainTex, i.uv);

                // RGBのコントラストを変化させる
                fixed r = contrast(color.r);
                fixed g = contrast(color.g);
                fixed b = contrast(color.b);

                return fixed4(r, g, b, color.a);
            }

            // コントラストを増減する関数
            fixed contrast(fixed previousValue)
            {
                // 0.5 と コントラストで増減した値の平均を返す
                return clamp((previousValue - 0.5) * _Contrast + 0.5, 0.0, 1.0);
            }

            ENDCG
        }
    }
    FallBack "Diffuse"
}

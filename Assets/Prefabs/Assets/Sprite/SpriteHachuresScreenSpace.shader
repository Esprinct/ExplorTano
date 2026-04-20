Shader "Custom/SpriteHachuresScreenSpace"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _StripeColor ("Stripe Color", Color) = (1,0,0,1)
        _StripeWidth ("Stripe Width (pixels)", Float) = 4
        _StripeSpacing ("Stripe Spacing (pixels)", Float) = 12
        _AlphaMultiplier ("Alpha Multiplier", Range(0,1)) = 1
        _Angle ("Angle", Float) = 45
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        Lighting Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                float4 screenPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            fixed4 _StripeColor;
            float _StripeWidth;
            float _StripeSpacing;
            float _AlphaMultiplier;
            float _Angle;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color;
                OUT.screenPos = ComputeScreenPos(OUT.vertex);
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 baseCol = tex2D(_MainTex, IN.texcoord) * IN.color;

                if (baseCol.a <= 0.001)
                    discard;

                float2 screenUV = IN.screenPos.xy / IN.screenPos.w;
                float2 pixelPos = screenUV * _ScreenParams.xy;

                float angleRad = radians(_Angle);
                float2 dir = float2(cos(angleRad), sin(angleRad));

                float coord = dot(pixelPos, dir);
                float period = max(_StripeSpacing, 1.0);
                float stripePos = frac(coord / period) * period;

                float aa = fwidth(coord);
                float stripeMask = 1.0 - smoothstep(_StripeWidth - aa, _StripeWidth + aa, stripePos);

                fixed4 stripeCol = _StripeColor;
                stripeCol.a *= baseCol.a * _AlphaMultiplier;

                return stripeCol * stripeMask;
            }
            ENDCG
        }
    }
}
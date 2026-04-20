Shader "Custom/SpriteDoubleHachuresParallelesScreenSpace"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}

        _ColorA ("Stripe Color A", Color) = (0,0,1,1)
        _ColorB ("Stripe Color B", Color) = (1,0,0,1)

        _StripeWidth ("Stripe Width (world units)", Float) = 0.15
        _StripeAngle ("Stripe Angle", Float) = 45
        _AlphaMultiplier ("Alpha Multiplier", Range(0,1)) = 1
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
                float2 worldPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            fixed4 _ColorA;
            fixed4 _ColorB;
            float _StripeWidth;
            float _StripeAngle;
            float _AlphaMultiplier;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color;

                float3 world = mul(unity_ObjectToWorld, IN.vertex).xyz;
                OUT.worldPos = world.xy;

                return OUT;
            }

            float BandMask(float pos, float start, float end, float aa)
            {
                float inMask = smoothstep(start - aa, start + aa, pos);
                float outMask = 1.0 - smoothstep(end - aa, end + aa, pos);
                return saturate(inMask * outMask);
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 baseCol = tex2D(_MainTex, IN.texcoord) * IN.color;

                if (baseCol.a <= 0.001)
                    discard;

                float angleRad = radians(_StripeAngle);
                float2 dir = float2(cos(angleRad), sin(angleRad));
                float coord = dot(IN.worldPos, dir);

                float stripeWidth = max(_StripeWidth, 0.0001);

                // Alternance A / B sans trou
                float period = stripeWidth * 2.0;

                float pos = fmod(coord, period);
                if (pos < 0.0)
                    pos += period;

                float aa = max(fwidth(coord) * 0.75, stripeWidth * 0.15);

                float maskA = BandMask(pos, 0.0, stripeWidth, aa);
                float maskB = BandMask(pos, stripeWidth, stripeWidth * 2.0, aa);

                fixed4 colA = _ColorA;
                fixed4 colB = _ColorB;

                colA.a *= baseCol.a * _AlphaMultiplier;
                colB.a *= baseCol.a * _AlphaMultiplier;

                fixed4 result = fixed4(0, 0, 0, 0);
                result.rgb = colA.rgb * maskA + colB.rgb * maskB;
                result.a = colA.a * maskA + colB.a * maskB;

                return result;
            }
            ENDCG
        }
    }
}
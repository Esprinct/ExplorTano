Shader "Custom/SpriteHachures"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _StripeColor ("Stripe Color", Color) = (1,0,0,1)
        _StripeWidth ("Stripe Width", Float) = 0.12
        _StripeSpacing ("Stripe Spacing", Float) = 0.24
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
            fixed4 _StripeColor;
            float _StripeWidth;
            float _StripeSpacing;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color;
                OUT.worldPos = mul(unity_ObjectToWorld, IN.vertex).xy;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 c = tex2D(_MainTex, IN.texcoord) * IN.color;

                float diag = frac((IN.worldPos.x + IN.worldPos.y) / _StripeSpacing);
                float stripe = step(diag, _StripeWidth / _StripeSpacing);

                fixed4 stripeColor = _StripeColor;
                stripeColor.a *= c.a;

                return lerp(fixed4(0,0,0,0), stripeColor, stripe) * c.a;
            }
            ENDCG
        }
    }
}
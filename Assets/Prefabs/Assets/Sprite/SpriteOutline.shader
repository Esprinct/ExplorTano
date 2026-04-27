Shader "Custom/SpriteExplorationOutline"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}

        _OutlineColor ("Outline Color", Color) = (1,1,1,1)
        _OutlineSize ("Outline Size", Float) = 4

        _UseHachure ("Use Hachure", Float) = 0
        _ColorA ("Color A", Color) = (1,1,1,1)
        _ColorB ("Color B", Color) = (0,0,0,1)

        // Ici ce sont des valeurs en unités monde, pas en pixels.
        _StripeWidth ("Stripe Width", Float) = 0.08
        _StripeSpacing ("Stripe Spacing", Float) = 0.18
        _StripeAngle ("Stripe Angle", Float) = 45
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

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;

            fixed4 _OutlineColor;
            float _OutlineSize;

            float _UseHachure;
            fixed4 _ColorA;
            fixed4 _ColorB;
            float _StripeWidth;
            float _StripeSpacing;
            float _StripeAngle;

            v2f vert(appdata_t v)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = v.texcoord;
                o.color = v.color;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

                return o;
            }

            fixed4 GetHachureColor(v2f i)
            {
                float angleRad = radians(_StripeAngle);

                float direction =
                    i.worldPos.x * cos(angleRad) +
                    i.worldPos.y * sin(angleRad);

                float spacing = max(_StripeSpacing, 0.001);
                float width = clamp(_StripeWidth / spacing, 0.05, 0.95);

                float stripe01 = frac(direction / spacing);
                float maskA = 1.0 - step(width, stripe01);

                fixed4 result = lerp(_ColorB, _ColorA, maskA);
                result.a = 1;

                return result;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 spriteColor = tex2D(_MainTex, i.texcoord);

                // Ne pas dessiner l'intérieur de la province.
                if (spriteColor.a > 0.01)
                    discard;

                float2 offset = _MainTex_TexelSize.xy * _OutlineSize;

                float alpha = 0;

                alpha += tex2D(_MainTex, i.texcoord + float2(offset.x, 0)).a;
                alpha += tex2D(_MainTex, i.texcoord + float2(-offset.x, 0)).a;
                alpha += tex2D(_MainTex, i.texcoord + float2(0, offset.y)).a;
                alpha += tex2D(_MainTex, i.texcoord + float2(0, -offset.y)).a;

                alpha += tex2D(_MainTex, i.texcoord + float2(offset.x, offset.y)).a;
                alpha += tex2D(_MainTex, i.texcoord + float2(-offset.x, offset.y)).a;
                alpha += tex2D(_MainTex, i.texcoord + float2(offset.x, -offset.y)).a;
                alpha += tex2D(_MainTex, i.texcoord + float2(-offset.x, -offset.y)).a;

                if (alpha <= 0.01)
                    discard;

                if (_UseHachure > 0.5)
                    return GetHachureColor(i);

                fixed4 finalColor = _OutlineColor;
                finalColor.a = 1;
                return finalColor;
            }
            ENDCG
        }
    }
}
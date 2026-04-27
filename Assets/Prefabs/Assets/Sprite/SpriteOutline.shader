Shader "Custom/SpriteExplorationOutline"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}

        _OutlineColor ("Outline Color", Color) = (1,1,1,1)
        _OutlineSize ("Outline Size", Float) = 2

        _UseHachure ("Use Hachure", Float) = 0
        _ColorA ("Color A", Color) = (1,1,1,1)
        _ColorB ("Color B", Color) = (0,0,0,1)
        _StripeWidth ("Stripe Width", Float) = 4
        _StripeSpacing ("Stripe Spacing", Float) = 12
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
                float4 screenPos : TEXCOORD1;
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
                o.screenPos = ComputeScreenPos(o.vertex);
                return o;
            }

            fixed4 GetHachureColor(v2f i)
            {
                float2 screenUV = i.screenPos.xy / i.screenPos.w;
                float2 pixelPos = screenUV * _ScreenParams.xy;

                float angleRad = radians(_StripeAngle);
                float direction = pixelPos.x * cos(angleRad) + pixelPos.y * sin(angleRad);

                float stripe = fmod(direction, _StripeSpacing);

                if (stripe < _StripeWidth)
                    return _ColorA;

                return _ColorB;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 spriteColor = tex2D(_MainTex, i.texcoord);

                // Ne dessine pas l'intérieur du sprite.
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

                // Important :
                // Pour les contours simples, on utilise la couleur du SpriteRenderer.
                // Comme ça, si _OutlineColor reste blanc dans le material,
                // le contour prend quand même la bonne couleur.
                fixed4 finalColor = i.color;
                finalColor.a = 1;
                return finalColor;
            }
            ENDCG
        }
    }
}
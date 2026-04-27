Shader "Custom/SpriteOutline"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (1,1,1,1)
        _OutlineSize ("Outline Size", Float) = 2
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
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            fixed4 _OutlineColor;
            float _OutlineSize;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = v.texcoord;
                o.color = v.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 spriteColor = tex2D(_MainTex, i.texcoord);

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

                return _OutlineColor;
            }
            ENDCG
        }
    }
}
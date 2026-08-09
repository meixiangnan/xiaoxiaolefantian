Shader "Custom/UIEdgeGlow"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _GlowColor ("Glow Color", Color) = (1,1,0,1) // 发光颜色
        _GlowWidth ("Glow Width", Range(0, 0.1)) = 0.02 // 发光宽度
        _GlowIntensity ("Glow Intensity", Range(1, 10)) = 2 // 发光强度
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
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
            #include "UnityUI.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord  : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
            };

            fixed4 _Color;
            fixed4 _GlowColor;
            float _GlowWidth;
            float _GlowIntensity;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _ClipRect;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.worldPosition = IN.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
                OUT.texcoord = TRANSFORM_TEX(IN.texcoord, _MainTex);
                OUT.color = IN.color * _Color;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                // 裁剪UI区域
                clip (UnityGet2DClipping(IN.worldPosition.xy, _ClipRect));

                // 采样主纹理（UI原图）
                fixed4 mainColor = tex2D(_MainTex, IN.texcoord) * IN.color;

                // 计算边缘距离（通过采样周围像素的透明度差异）
                float2 offsets[4] = {
                    float2(-1, 0), float2(1, 0),
                    float2(0, -1), float2(0, 1)
                };
                float edge = 0;
                for (int i = 0; i < 4; i++)
                {
                    float2 uv = IN.texcoord + offsets[i] * _GlowWidth;
                    edge += 1 - tex2D(_MainTex, uv).a; // 周围像素越透明，边缘越明显
                }
                edge = saturate(edge); // 限制在0~1范围

                // 计算发光颜色（边缘区域叠加发光色）
                fixed4 glow = _GlowColor * edge * _GlowIntensity;

                // 混合原图和发光效果
                return mainColor + glow * (1 - mainColor.a); // 避免在不透明区域叠加发光
            }
            ENDCG
        }
    }
}
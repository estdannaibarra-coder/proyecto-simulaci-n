Shader "Custom/SpriteOutline"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        [HDR] _OutlineColor ("Outline Color", Color) = (0,1,1,1)
        _OutlineSize ("Outline Size (px)", Range(0,16)) = 3
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "Queue"="Transparent" "RenderType"="Transparent" }
        Cull Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            Tags { "LightMode"="Universal2D" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes { float4 positionOS : POSITION; float4 color : COLOR; float2 uv : TEXCOORD0; };
            struct Varyings   { float4 positionCS : SV_POSITION; float4 color : COLOR; float2 uv : TEXCOORD0; };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _MainTex_TexelSize;

            CBUFFER_START(UnityPerMaterial)
                half4 _Color;
                half4 _OutlineColor;
                float _OutlineSize;
            CBUFFER_END

            Varyings vert(Attributes v)
            {
                Varyings o;
                o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = v.uv;
                o.color = v.color * _Color;
                return o;
            }

            half SampleA(float2 uv)
            {
                return SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv).a;
            }

            half4 frag(Varyings i) : SV_Target
            {
                half4 c = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv) * i.color;
                float2 px = _MainTex_TexelSize.xy * _OutlineSize;

                half a = 0;
                a = max(a, SampleA(i.uv + float2( px.x, 0)));
                a = max(a, SampleA(i.uv + float2(-px.x, 0)));
                a = max(a, SampleA(i.uv + float2(0,  px.y)));
                a = max(a, SampleA(i.uv + float2(0, -px.y)));
                a = max(a, SampleA(i.uv + float2( px.x,  px.y)));
                a = max(a, SampleA(i.uv + float2(-px.x,  px.y)));
                a = max(a, SampleA(i.uv + float2( px.x, -px.y)));
                a = max(a, SampleA(i.uv + float2(-px.x, -px.y)));

                c.rgb *= c.a;
                half4 o = _OutlineColor;
                o.a *= a;
                o.rgb *= o.a;
                return c + o * (1 - c.a);
            }
            ENDHLSL
        }
    }
}
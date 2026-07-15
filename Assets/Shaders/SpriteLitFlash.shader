Shader "Custom/2D/Sprite-Lit-Flash"
{
    Properties
    {
        [MainTexture] _MainTex ("Sprite Texture", 2D) = "white" {}
        [MainColor] _Color ("Tint", Color) = (1,1,1,1)

        _FlashColor ("Flash Color", Color) = (1,1,1,1)
        _FlashAmount ("Flash Amount", Range(0,1)) = 0

        // Эти скрытые параметры нужны, чтобы SpriteRenderer мог записывать в них
        // цвет и флип через MaterialPropertyBlock (неинстансированный путь)
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,0,0)
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
            "Queue"="Transparent"
            "RenderPipeline"="UniversalPipeline"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            Name "Universal2D"
            Tags { "LightMode"="Universal2D" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float4 _FlashColor;
                float _FlashAmount;
            CBUFFER_END

            // Неинстансированный путь — SpriteRenderer пишет сюда через MaterialPropertyBlock
            CBUFFER_START(UnityPerDrawSprite)
                float4 _RendererColor;
                float4 _Flip;   // SpriteRenderer передаёт Vector4, берём .xy
            CBUFFER_END

            // Инстансированный путь
            #ifdef UNITY_INSTANCING_ENABLED
                UNITY_INSTANCING_BUFFER_START(PerDrawSprite)
                    UNITY_DEFINE_INSTANCED_PROP(float4, unity_SpriteRendererColorArray)
                    UNITY_DEFINE_INSTANCED_PROP(float2, unity_SpriteFlipArray)
                UNITY_INSTANCING_BUFFER_END(PerDrawSprite)
                #define _RendererColorInst  UNITY_ACCESS_INSTANCED_PROP(PerDrawSprite, unity_SpriteRendererColorArray)
                #define _FlipInst           UNITY_ACCESS_INSTANCED_PROP(PerDrawSprite, unity_SpriteFlipArray)
            #endif

            Varyings vert (Attributes v)
            {
                Varyings o;
                UNITY_SETUP_INSTANCE_ID(v);

                // Получаем флип и цвет в зависимости от режима
                #ifdef UNITY_INSTANCING_ENABLED
                    float2 flip = _FlipInst;
                    float4 rendererColor = _RendererColorInst;
                #else
                    float2 flip = _Flip.xy;
                    float4 rendererColor = _RendererColor;
                #endif

                // Флип ГЕОМЕТРИИ – зеркалим позиции вершин
                v.positionOS.xy *= flip;

                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = v.uv;
                o.color = v.color * _Color * rendererColor;

                return o;
            }

            half4 frag (Varyings i) : SV_Target
            {
                half4 tex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv) * i.color;

                // Flash-эффект
                tex.rgb = lerp(tex.rgb, _FlashColor.rgb, _FlashAmount);

                return tex;
            }

            ENDHLSL
        }
    }
}
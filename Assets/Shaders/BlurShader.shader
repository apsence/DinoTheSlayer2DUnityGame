Shader "Custom/2D/URP_BackgroundBlur_FullScreen"
{
    Properties
    {
        _BlurSize ("Blur Size", Range(0, 20)) = 2.0
        _Color ("Tint Color", Color) = (0, 0, 0, 0.3)
    }

    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" }

        Cull Off
        ZWrite Off
        ZTest Always

        Pass
        {
            Name "BackgroundBlurFullScreen"

            HLSLPROGRAM
            // Vert, структуры Attributes/Varyings, а также TEXTURE2D_X(_BlitTexture) и
            // sampler_LinearClamp уже объявлены внутри Blit.hlsl — переопределять их нельзя.
            #pragma vertex Vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float _BlurSize;
                float4 _Color;
            CBUFFER_END

            // _BlitTexture_TexelSize уже объявлен в Blit.hlsl вместе с _BlitTexture — свой не нужен

            half4 frag (Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                float2 uv = input.texcoord;
                float2 texelSize = _BlitTexture_TexelSize.xy * _BlurSize;

                float4 blurredColor = float4(0, 0, 0, 0);
                blurredColor += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv + float2(-1, -1) * texelSize);
                blurredColor += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv + float2( 0, -1) * texelSize);
                blurredColor += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv + float2( 1, -1) * texelSize);

                blurredColor += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv + float2(-1,  0) * texelSize);
                blurredColor += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv + float2( 0,  0) * texelSize);
                blurredColor += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv + float2( 1,  0) * texelSize);

                blurredColor += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv + float2(-1,  1) * texelSize);
                blurredColor += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv + float2( 0,  1) * texelSize);
                blurredColor += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv + float2( 1,  1) * texelSize);

                blurredColor /= 9.0;

                return lerp(blurredColor, float4(_Color.rgb, 1.0), _Color.a);
            }
            ENDHLSL
        }
    }
}

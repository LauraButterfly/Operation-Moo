
Shader "Rubba/URPSpriteEmission"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Sprite Tint", Color) = (1,1,1,1)

        [NoScaleOffset] _EmissionMap ("Emission Mask", 2D) = "black" {}
        [HDR] _EmissionColor ("Emission Color", Color) = (0.5,0.5,1,1)
        _EmissionIntensity ("Emission Intensity", Range(0,10)) = 3

        [Enum(Constant,0,HorizontalWave,1,VerticalWave,2)]
        _EmissionMode ("Emission Mode", Float) = 0

        _WaveSpeed ("Wave Speed", Range(0,10)) = 1
        _WaveWidth ("Wave Width", Range(0.01,1)) = 0.25
        _WaveSoftness ("Wave Softness", Range(0.001,1)) = 0.15
        _MinimumEmission ("Minimum Emission", Range(0,1)) = 0

        [HideInInspector] _RendererColor ("Renderer Color", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
            "CanUseSpriteAtlas" = "True"
            "PreviewType" = "Plane"
        }

        Cull Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
{
    Name "SpriteEmission"
    Tags { "LightMode" = "Universal2D" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            TEXTURE2D(_EmissionMap);
            SAMPLER(sampler_EmissionMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float4 _EmissionColor;
                float _EmissionIntensity;
                float _EmissionMode;
                float _WaveSpeed;
                float _WaveWidth;
                float _WaveSoftness;
                float _MinimumEmission;
            CBUFFER_END

            float4 _RendererColor;
            float4 _Flip;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                float3 position = IN.positionOS.xyz;
                position.xy *= _Flip.xy;

                OUT.positionCS = TransformObjectToHClip(position);
                OUT.uv = IN.uv;
                OUT.color = IN.color * _Color * _RendererColor;

                return OUT;
            }

            float4 frag(Varyings IN) : SV_Target
            {
                float4 sprite =
                    SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv)
                    * IN.color;

                float3 maskColor =
                    SAMPLE_TEXTURE2D(_EmissionMap, sampler_EmissionMap, IN.uv).rgb;

                float mask = dot(
                    maskColor,
                    float3(0.2126, 0.7152, 0.0722)
                );

                float wave = 1.0;

                if (_EmissionMode > 0.5)
                {
                    float coordinate = IN.uv.x;

                    if (_EmissionMode > 1.5)
                        coordinate = 1.0 - IN.uv.y;

                    float center = frac(_Time.y * _WaveSpeed);
                    float distanceToWave = abs(coordinate - center);

                    distanceToWave = min(
                        distanceToWave,
                        1.0 - distanceToWave
                    );

                    float halfWidth = _WaveWidth * 0.5;

                    wave = 1.0 - smoothstep(
                        halfWidth,
                        halfWidth + _WaveSoftness,
                        distanceToWave
                    );

                    wave = lerp(_MinimumEmission, 1.0, wave);
                }

                float3 emission =
                    mask *
                    _EmissionColor.rgb *
                    _EmissionIntensity *
                    wave;

                float3 finalRGB = sprite.rgb + emission;

                return float4(finalRGB * sprite.a, sprite.a);
            }

            ENDHLSL
        }
    }

    Fallback Off
}

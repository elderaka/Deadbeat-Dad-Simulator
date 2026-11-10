Shader "Bapak Simulator/Pond Surface"
{
    Properties
    {
        _Water ("Water tint", Color) = (0.12, 0.48, 0.48, 0.34)
        _Ripple ("Ripple tint", Color) = (0.62, 0.85, 0.74, 1)
        _RippleScale ("Ripple scale", Float) = 1.9
        _RippleSpeed ("Ripple speed", Float) = 0.85
    }
    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" "Queue" = "Transparent" "RenderType" = "Transparent" }
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                half4 _Water;
                half4 _Ripple;
                float _RippleScale;
                float _RippleSpeed;
            CBUFFER_END

            struct Attributes { float4 positionOS : POSITION; };
            struct Varyings { float4 positionCS : SV_POSITION; float3 positionWS : TEXCOORD0; };

            Varyings Vert(Attributes input)
            {
                Varyings output;
                output.positionWS = TransformObjectToWorld(input.positionOS.xyz);
                output.positionCS = TransformWorldToHClip(output.positionWS);
                return output;
            }

            half4 Frag(Varyings input) : SV_Target
            {
                float2 p = input.positionWS.xz * _RippleScale;
                float time = _Time.y * _RippleSpeed;
                float wave = sin(p.x * 1.7 + p.y * 2.2 + time + sin(p.y * 1.2 - time * 0.6));
                wave += 0.45 * sin(p.x * 2.1 - p.y * 1.4 - time);
                half crest = smoothstep(0.9, 1.42, wave);
                half grazing = 1 - saturate(normalize(_WorldSpaceCameraPos - input.positionWS).y);
                half3 color = lerp(_Water.rgb, _Ripple.rgb, crest * 0.38);
                return half4(color, saturate(_Water.a + crest * 0.12 + grazing * grazing * 0.13));
            }
            ENDHLSL
        }
    }
}

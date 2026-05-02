Shader "Custom/URPARGlow"
{
    Properties
    {
        [Header(Tekstur Utama)]
        _BaseColor ("Warna Utama", Color) = (1,1,1,1)
        _BaseMap ("Tekstur Dasar (RGB)", 2D) = "white" {}
        
        [Header(PBR Maps)]
        [NoScaleOffset] _NormalMap ("Normal Map", 2D) = "bump" {}
        _BumpScale ("Skala Normal", Range(0, 2)) = 1.0
        
        [NoScaleOffset] _MetallicMap ("Metallic Map (R)", 2D) = "white" {}
        _Metallic ("Intensitas Metallic", Range(0, 1)) = 0.0
        
        [NoScaleOffset] _RoughnessMap ("Roughness Map (R)", 2D) = "white" {}
        _Smoothness ("Intensitas Smoothness", Range(0, 1)) = 0.5

        [Header(Pengaturan Glow)]
        [HDR] _RimColor ("Warna Glow (Rim)", Color) = (0, 0.8, 1, 1)
        _RimPower ("Ketebalan Glow", Range(0.5, 8.0)) = 3.0
        _EmissionIntensity ("Kekuatan Cahaya", Range(0, 10)) = 1.5
    }

    SubShader
    {
        Tags 
        { 
            "RenderType" = "Opaque" 
            "RenderPipeline" = "UniversalPipeline" 
            "Queue" = "Geometry" 
        }
        LOD 200

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float2 uv           : TEXCOORD0;
                float3 normalOS     : NORMAL;
                float4 tangentOS    : TANGENT;
            };

            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
                float2 uv           : TEXCOORD0;
                float3 normalWS     : TEXCOORD1;
                float4 tangentWS    : TEXCOORD2; 
                float3 viewDirWS    : TEXCOORD3;
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);
            TEXTURE2D(_NormalMap);
            SAMPLER(sampler_NormalMap);
            TEXTURE2D(_MetallicMap);
            SAMPLER(sampler_MetallicMap);
            TEXTURE2D(_RoughnessMap);
            SAMPLER(sampler_RoughnessMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                half4 _BaseColor;
                half4 _RimColor;
                half _RimPower;
                half _EmissionIntensity;
                half _Metallic;
                half _Smoothness;
                half _BumpScale;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output;
                
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionHCS = vertexInput.positionCS;
                output.uv = TRANSFORM_TEX(input.uv, _BaseMap);

                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);
                output.normalWS = normalInput.normalWS;
                output.tangentWS = float4(normalInput.tangentWS, input.tangentOS.w);
                
                output.viewDirWS = GetWorldSpaceViewDir(vertexInput.positionWS);
                
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                // 1. Sampling Tekstur Dasar
                half4 baseMap = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv);
                half3 albedo = baseMap.rgb * _BaseColor.rgb;

                // 2. Normal Mapping
                half4 normalSample = SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, input.uv);
                half3 normalTS = UnpackNormalScale(normalSample, _BumpScale);
                
                float3 bitangentWS = cross(input.normalWS, input.tangentWS.xyz) * input.tangentWS.w;
                float3x3 tangentToWorld = float3x3(input.tangentWS.xyz, bitangentWS, input.normalWS);
                float3 normalWS = normalize(mul(normalTS, tangentToWorld));

                // 3. PBR Maps Sampling (Metallic & Roughness)
                half metallicMap = SAMPLE_TEXTURE2D(_MetallicMap, sampler_MetallicMap, input.uv).r;
                half roughnessMap = SAMPLE_TEXTURE2D(_RoughnessMap, sampler_RoughnessMap, input.uv).r;
                
                half finalMetallic = metallicMap * _Metallic;
                // Inversi Roughness menjadi Smoothness untuk standar URP
                half finalSmoothness = (1.0 - roughnessMap) * _Smoothness;

                // 4. Kalkulasi Rim Glow
                float3 viewDirWS = normalize(input.viewDirWS);
                half rim = 1.0 - saturate(dot(viewDirWS, normalWS));
                half3 emission = _RimColor.rgb * pow(rim, _RimPower) * _EmissionIntensity;

                // 5. Simulasi Pencahayaan PBR Sederhana
                // Memberikan interaksi cahaya berdasarkan Metallic dan Smoothness
                half3 specular = finalMetallic * albedo * finalSmoothness;
                half3 diffuse = albedo * (1.0 - finalMetallic);
                
                half3 finalColor = diffuse + specular + emission;

                return half4(finalColor, baseMap.a * _BaseColor.a);
            }
            ENDHLSL
        }
    }
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
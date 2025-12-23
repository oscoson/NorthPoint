Shader "Hidden/Shader/FloorPostProcessing"
{
    Properties
    {
        // This property is necessary to make the CommandBuffer.Blit bind the source texture to _MainTex
        _MainTex("Main Texture", 2DArray) = "grey" {}
    }

    HLSLINCLUDE

    #pragma target 4.5
    #pragma only_renderers d3d11 playstation xboxone xboxseries vulkan metal switch

    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/PostProcessing/Shaders/FXAA.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/PostProcessing/Shaders/RTUpscale.hlsl"

    struct Attributes
    {
        uint vertexID : SV_VertexID;
        UNITY_VERTEX_INPUT_INSTANCE_ID
    };

    struct Varyings
    {
        float4 positionCS : SV_POSITION;
        float2 texcoord   : TEXCOORD0;
        UNITY_VERTEX_OUTPUT_STEREO
    };

    Varyings Vert(Attributes input)
    {
        Varyings output;
        UNITY_SETUP_INSTANCE_ID(input);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
        output.positionCS = GetFullScreenTriangleVertexPosition(input.vertexID);
        output.texcoord = GetFullScreenTriangleTexCoord(input.vertexID);
        return output;
    }

    // List of properties to control your post process effect
    float _Intensity = 1.0;
    TEXTURE2D_X(_MainTex);

    // Input textures
    TEXTURE2D(_Input0);
    SAMPLER(sampler_Input0);

    TEXTURE2D(_Input1);
    SAMPLER(sampler_Input1);

    TEXTURE2D(_Input2);
    SAMPLER(sampler_Input2);

    TEXTURE2D(_Input3);
    SAMPLER(sampler_Input3);

    TEXTURE2D(_Input4);
    SAMPLER(sampler_Input4);
    

    float4 CustomPostProcess(Varyings input) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

        float2 uv = input.texcoord;

        // Region bounds
        float2 minTL = float2(0.0, 0.63);
        float2 maxTL = float2(0.5, 1.0);

        // float2 minTR = float2(0.5, 0.633);
        // float2 maxTR = float2(1.0, 1.0);

        bool inTL =
            uv.x >= minTL.x && uv.x <= maxTL.x &&
            uv.y >= minTL.y && uv.y <= maxTL.y;

        // bool inTR =
        //     uv.x >= minTR.x && uv.x <= maxTR.x &&
        //     uv.y >= minTR.y && uv.y <= maxTR.y;

        // Mask out everything except the two top regions
        if (!(inTL))
            return float4(0,0,0,1);

        float3 sourceColor = SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, ClampAndScaleUVForBilinearPostProcessTexture(input.texcoord.xy)).xyz;

        // Apply greyscale effect
        float3 color = lerp(sourceColor, Luminance(sourceColor), _Intensity);
        
        return SAMPLE_TEXTURE2D(_Input0, sampler_Input0, uv);
    }

    ENDHLSL

    SubShader
    {
        Tags{ "RenderPipeline" = "HDRenderPipeline" }
        Pass
        {
            Name "FloorPostProcessing"

            ZWrite Off
            ZTest Always
            Blend Off
            Cull Off

            HLSLPROGRAM
                #pragma fragment CustomPostProcess
                #pragma vertex Vert
            ENDHLSL
        }
    }
    Fallback Off
}

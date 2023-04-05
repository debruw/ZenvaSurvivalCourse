Shader "Custom/WorldPosMask"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        // SubShader Tags define when and under which conditions a SubShader block or
        // a pass is executed.
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline" }

        Pass
        {
            // The HLSL code block. Unity SRP uses the HLSL language.
            HLSLPROGRAM
            // This line defines the name of the vertex shader. 
            #pragma vertex vert
            // This line defines the name of the fragment shader. 
            #pragma fragment frag

            // The Core.hlsl file contains definitions of frequently used HLSL
            // macros and functions, and also contains #include references to other
            // HLSL files (for example, Common.hlsl, SpaceTransforms.hlsl, etc.).
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"            

            // The structure definition defines which variables it contains.
            // This example uses the Attributes structure as an input structure in
            // the vertex shader.
            struct Attributes
            {
                // The positionOS variable contains the vertex positions in object
                // space.
                float4 positionOS   : POSITION;                 
            };

            struct Varyings
            {
                // The positions in this struct must have the SV_POSITION semantic.
                float4 positionHCS  : SV_POSITION;
            };            

            // The vertex shader definition with properties defined in the Varyings 
            // structure. The type of the vert function must match the type (struct)
            // that it returns.
            Varyings vert(Attributes IN)
            {
                // Declaring the output object (OUT) with the Varyings struct.
                Varyings OUT;
                // The TransformObjectToHClip function transforms vertex positions
                // from object space to homogenous space
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                // Returning the output.
                return OUT;
            }

            struct Input
            {
                float3 worldPos;
            };
        
            float4 _ShaderInteractorsPositions[100];
            float _ShaderInteractorsRadiuses[100];
            float4 _ShaderInteractorsBoxBounds[100];
            float4 _ShaderInteractorRotation[100];

            float _ShapeCutOff;
            float _ShapeSmoothness;

            float3 Unity_RotateAboutAxis_Degrees_float(float3 In, float3 Axis, float Rotation)
            {
                Rotation = radians(Rotation);
                float s = sin(Rotation);
                float c = cos(Rotation);
                float one_minus_c = 1.0 - c;

                Axis = normalize(Axis);
                float3x3 rot_mat =
                {
                    one_minus_c * Axis.x * Axis.x + c, one_minus_c * Axis.x * Axis.y - Axis.z * s,
                    one_minus_c * Axis.z * Axis.x + Axis.y * s,
                    one_minus_c * Axis.x * Axis.y + Axis.z * s, one_minus_c * Axis.y * Axis.y + c,
                    one_minus_c * Axis.y * Axis.z - Axis.x * s,
                    one_minus_c * Axis.z * Axis.x - Axis.y * s, one_minus_c * Axis.y * Axis.z + Axis.x * s,
                    one_minus_c * Axis.z * Axis.z + c
                };
                return float3(mul(rot_mat, In));
            }

            float InsideTheFor(float i, float4 POS)
            {
                float boxes;
                float3 dis = distance(_ShaderInteractorsPositions[i].xyz, POS);
                float sphereR = 1 - saturate(dis / _ShaderInteractorsRadiuses[i]).r;
                sphereR = (smoothstep(_ShapeCutOff, _ShapeCutOff + _ShapeSmoothness, sphereR));
                sphereR += (sphereR);

                float3 rotation = _ShaderInteractorRotation[i].xyz;
                float3 rotatedPos = Unity_RotateAboutAxis_Degrees_float(_ShaderInteractorsPositions[i].xyz - POS, float3(-1,0,0), rotation.x);
                rotatedPos = Unity_RotateAboutAxis_Degrees_float(rotatedPos, float3(0, -1, 0), rotation.y);
                rotatedPos = Unity_RotateAboutAxis_Degrees_float(rotatedPos, float3(0, 0, -1), rotation.z);

                float3 scale = _ShaderInteractorsBoxBounds[i].xyz;
                float3 BoxPos = saturate(scale - abs(rotatedPos));

                float3 boxCutoff = (smoothstep(_ShapeCutOff, _ShapeCutOff - _ShapeSmoothness, BoxPos));
                boxes = saturate(boxCutoff.r * boxCutoff.g * boxCutoff.b);
                return boxes;
            }

            /*void surf (Input IN, inout v2f_vertex_lit o)
            {
                float spheres = 0;
                float boxes = 0;
                for (int i = 0; i < 100; i++)
                {
                    boxes += InsideTheFor(i, IN);
                }
                float combineRadius = boxes + spheres;
                o.uv = float3(combineRadius, combineRadius, combineRadius);
            }*/
            
            half4 frag(Varyings IN): SV_TARGET
            {
                float spheres = 0;
                float boxes = 0;
                for (int i = 0; i < 100; i++)
                {
                    boxes += InsideTheFor(i, IN.positionHCS);
                }
                float combineRadius = boxes + spheres;
                
                half4 customColor;
                customColor = (0, 1, 0, 0);
                return customColor;
            }
            ENDHLSL
        }
    }
}

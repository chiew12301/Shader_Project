Shader "Custom/Shader_FurryHair"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {}    // Texture for the fur
        _HairDensity ("Hair Density", Float) = 50.0   // Controls hair density
        _HairLength ("Hair Length", Float) = 0.05     // Controls the length of the fur
        _Color ("Fur Color", Color) = (1,1,1,1)       // Base fur color
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
                float3 normal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
            };

            sampler2D _MainTex;
            float _HairDensity;
            float _HairLength;
            float4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.normal = normalize(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            // Procedural noise function
            float noise(float3 pos)
            {
                return frac(sin(dot(pos ,float3(12.9898, 78.233, 45.164))) * 43758.5453);
            }

            // Generate random hair direction based on normals and noise
            float3 generateHair(float3 normal, float3 worldPos, float hairLength)
            {
                float3 direction = normalize(normal + noise(worldPos) * 0.5); // More randomness
                return direction * hairLength;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Sample the base texture
                fixed4 baseCol = tex2D(_MainTex, i.uv) * _Color;

                // Hair generation
                float3 hairNormal = generateHair(i.normal, i.worldPos, _HairLength);

                // Hair density modulation using noise
                float densityFactor = noise(i.worldPos * _HairDensity);

                // Modulate base color by hair density
                baseCol.rgb *= densityFactor;

                // Simple Lambert lighting
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz); // Direction of the main light
                float NdotL = max(0, dot(hairNormal, lightDir));       // Diffuse lighting factor
                
                // Apply lighting to the hair color
                baseCol.rgb *= NdotL;

                // Return the final color
                return baseCol;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}

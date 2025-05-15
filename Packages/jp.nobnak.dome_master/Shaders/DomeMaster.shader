Shader "Unlit/DomeMaster" {
    Properties {
        [KeywordEnum(Forward, Backward)] _Dir ("Direction", float) = 0
    }
    SubShader {
        Tags { "RenderType"="Opaque" }

        Pass {
            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag
            
            #pragma multi_compile ___ Blit
            #pragma multi_compile _DIR_FORWARD _DIR_BACKWARD

            #define PI      3.141592
            #define PI_HALF (0.5 * PI)
            #define PI_TWO  (2.0 * PI)
            
            #include "UnityCG.cginc"
            #include "UnityShaderVariables.cginc"
            #include "Packages/jp.nobnak.dome_master/ShaderLibrary/DomeMaster.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            UNITY_DECLARE_TEXCUBE(_DomeMasterCube);
            
            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target {
                float dir = 1;
                #if defined(_DIR_BACKWARD)
                dir *= -1;
                #endif

                float3 v = DomeMaster_Decode(i.uv);
                v = mul(unity_ObjectToWorld, float4(dir * v, 0));
                float4 c = UNITY_SAMPLE_TEXCUBE(_DomeMasterCube, v);
                
                float2 xy = 2.0 * i.uv - 1.0;
                float r = sqrt(dot(xy, xy));
                return r <= 1.0 ? c : 0;
            }
            ENDCG
        }
    }
}

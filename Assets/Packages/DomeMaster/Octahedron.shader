// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/Equirectangular" {
    Properties {
        _Lod ("LOD", Range(0, 32)) = 0
    }
    SubShader {
        Tags { "RenderType"="Opaque" }

        Pass {
            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag

            #define PI			3.141592
            #define PI_HALF		(0.5 * PI)
            #define PI_TWO		(2.0 * PI)
            
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
            float _Lod;
            float _Dir;
            
            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(float4(float3(2, 1, 1) * v.vertex.xyz, 1));
                o.uv = v.uv;
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target {
                float3 v = Octahedron_Decode(i.uv);
                v = mul(unity_ObjectToWorld, float4(v, 0));
                float4 c = UNITY_SAMPLE_TEXCUBE_LOD(_DomeMasterCube, v, _Lod);
                return c;
            }
            ENDCG
        }
    }
}

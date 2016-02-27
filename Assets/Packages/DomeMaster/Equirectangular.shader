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
				float2 xy = float2(PI, PI_HALF) * (2.0 * v.uv - 1.0);

				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, float4(float3(2, 1, 1) * v.vertex.xyz, 1));
				o.uv = xy;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target {
				float2 xy = i.uv;

				float projxz = cos(xy.y);
                float3 v = float3(sin(xy.x), 1.0, cos(xy.x)) * float3(projxz, sin(xy.y), projxz);
                v = mul(_Object2World, float4(v, 0));

				float4 c = UNITY_SAMPLE_TEXCUBE_LOD(_DomeMasterCube, v, _Lod);
				return c;
			}
			ENDCG
		}
	}
}

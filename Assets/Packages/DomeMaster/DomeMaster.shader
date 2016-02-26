Shader "Unlit/DomeMaster" {
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

			#define PI      3.141592
            #define PI_HALF (0.5 * PI)
            #define PI_TWO  (2.0 * PI)
			
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

			UNITY_DECLARE_TEXCUBE(_GlobalDome);
            float _Lod;
			
			v2f vert (appdata v) {
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target {
				float2 xy = 2.0 * frac(float2(2.0, 1.0) * i.uv) - 1.0;
				float dir = sign(2.0 * i.uv.x - 1.0);

                float r = sqrt(dot(xy, xy));
                float theta = atan2(xy.y, xy.x);
                float phi = r * PI_HALF;

                float projxy = sin(phi);
                float3 v = float3(cos(theta), sin(theta), cos(phi)) * float3(projxy, projxy, 1);
                v = mul(_Object2World, float4(dir * v, 0));
				float4 c = UNITY_SAMPLE_TEXCUBE_LOD(_GlobalDome, v, _Lod);
				return r <= 1.0 ? c : 0;
			}
			ENDCG
		}
	}
}

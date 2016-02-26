Shader "Unlit/DomeMaster" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
        _Lod ("LOD", Range(0, 32)) = 0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }

		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

            #define PI_HALF 1.57079
			
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

			sampler2D _MainTex;
			float4 _MainTex_ST;
            float _Lod;
			
			v2f vert (appdata v) {
                float2 xy = TRANSFORM_TEX((2 * v.uv - 1), _MainTex);

				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = xy;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target {
                float r = saturate(sqrt(dot(i.uv, i.uv)));
                float theta = atan2(i.uv.y, i.uv.x);
                float phi = r * PI_HALF;

                float projxy = sin(phi);
                float3 v = float3(cos(theta), sin(theta), cos(phi)) * float3(projxy, projxy, 1);
                v = mul(_Object2World, float4(v, 0));
				float4 c = UNITY_SAMPLE_TEXCUBE_LOD(unity_SpecCube0, v, _Lod);
				return c;
			}
			ENDCG
		}
	}
}

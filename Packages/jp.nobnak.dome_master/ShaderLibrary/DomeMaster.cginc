#ifndef __DomeMaster__CGINC__
#define __DomeMaster__CGINC__

#define PI      3.141592
#define PI_HALF (0.5 * PI)
#define PI_TWO  (2.0 * PI)

float3 DomeMaster_Decode(float2 uv) {
    float2 xy = 2.0 * uv - 1.0;
    float r = sqrt(dot(xy, xy));
    float theta = atan2(xy.y, xy.x);
    float phi = r * PI_HALF;

    float projxy = sin(phi);
    float3 v = float3(cos(theta), sin(theta), cos(phi)) * float3(projxy, projxy, 1);
    return v;
}

float3 EquiRectangular_Decode(float2 uv) {
    float2 xy = float2(PI, PI_HALF) * (2.0 * uv - 1.0);
    float projxz = cos(xy.y);
    float3 v = float3(sin(xy.x), 1.0, cos(xy.x)) * float3(projxz, sin(xy.y), projxz);
    return v;
}

// Signed Octahedron Normal Encoding
// https://johnwhite3d.blogspot.com/
float3 Octahedron_Decode(float2 uv) {
    float v = 2 * uv.x;
    float3 n = float3(frac(v), uv.y, floor(v));
    
    float3 OutN;
    OutN.x = (n.x - n.y);
    OutN.y = (n.x + n.y) - 1.0;
    OutN.z = n.z * 2.0 - 1.0;
    OutN.z = OutN.z * (1.0 - abs(OutN.x) - abs(OutN.y));
 
    OutN = normalize(OutN);
    return OutN;
}

#endif

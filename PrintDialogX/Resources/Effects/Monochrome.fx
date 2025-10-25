sampler2D input : register(s0);

float left : register(c0);
float top : register(c1);
float width : register(c2);
float height : register(c3);

float4 main(float2 uv : TEXCOORD) : COLOR
{
    float4 color = tex2D(input, uv);
    float2 position = frac((uv * float2(width, height) + float2(left, top)) / 256.0);
    float dither = frac(sin(dot(floor(position * 2048.0), float2(12.34, 45.67))) * 4321.1234);
    float grayscale = dot(color.rgb, float3(0.3, 0.59, 0.11));
    color.rgb = 1.0 - smoothstep(grayscale - 0.001, grayscale + 0.01, dither);

    return color;
}
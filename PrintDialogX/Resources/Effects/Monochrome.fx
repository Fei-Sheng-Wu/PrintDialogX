sampler2D input : register(s0);
float left : register(c0);
float top : register(c1);
float width : register(c2);
float height : register(c3);

float4 main(float2 uv : TEXCOORD) : COLOR
{
    float4 color = tex2D(input, uv);
    float2 position = uv * float2(width, height) + float2(left, top);
    color.rgb = step(frac(4321.1234 * sin(dot(floor(2048.0 * frac(position / 256.0)), float2(12.34, 45.67)))), dot(color.rgb, float3(0.3, 0.59, 0.11)));

    return color;
}

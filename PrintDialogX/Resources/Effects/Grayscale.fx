sampler2D input : register(s0);

float4 main(float2 uv : TEXCOORD) : COLOR
{
    float4 color = tex2D(input, uv);
    color.rgb = dot(color.rgb, float3(0.3, 0.59, 0.11));

    return color;
}
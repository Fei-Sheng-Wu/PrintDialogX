sampler2D input : register(s0);

float4 main(float2 uv : TEXCOORD) : COLOR
{
	float4 color = tex2D(input, uv);
    float dither = frac(sin(dot(floor(uv * 2048.0), float2(12.34, 45.67))) * 4321.1234);
    color.rgb = dither < dot(color.rgb, float3(0.3, 0.59, 0.11)) ? 1.0 : 0.0;

	return color;
}
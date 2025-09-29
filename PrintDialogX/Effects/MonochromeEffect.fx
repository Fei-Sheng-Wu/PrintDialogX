sampler2D implicitInput : register(s0);

sampler2D g_BlueNoise_LDR_LLL1_0_Rv : register(s1);

float viewportTop : register(c0);
float viewportLeft : register(c1);
float viewportWidth : register(c2);
float viewportHeight : register(c3);

// @return 0..1
float BlueNoiseTex64x64(float2 uv : TEXCOORD) : COLOR1
{
    uv.x = fmod(((uv.x * viewportWidth) + viewportLeft) / 64.0f, 1.0f);
    uv.y = fmod(((uv.y * viewportHeight) + viewportTop) / 64.0f, 1.0f);
    return tex2D(g_BlueNoise_LDR_LLL1_0_Rv, uv).r;
}

float4 main(float2 uv : TEXCOORD) : COLOR
{
    float4 color = tex2D(implicitInput, uv);
    
    if (color.a == 0)
        return color;

    float gray = color.r * 0.3 + color.g * 0.59 + color.b * 0.11;

    float4 RGBA = 0;

    RGBA += BlueNoiseTex64x64(uv) < saturate(gray);
    RGBA.a = 1;

    return RGBA;
}
